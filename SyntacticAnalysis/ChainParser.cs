using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntacticAnalysis
{
    delegate T ParserFunction<out T>(ChainParser chain) where T : Element;
    delegate void TokenAction<T>(T self, Token token) where T : Element;
    delegate void ElementAction<T, E>(T self, E element) where T : Element where E : Element;

    class ChainParser
    {
        protected TokenCollection collection;
        protected int index;
        protected TextPosition beginPosition;
        protected TextPosition endPosition;

        public ChainParser(TokenCollection collection, int index = 0)
        {
            this.collection = collection;
            this.index = index;
            beginPosition = collection.GetTextPosition(index);
            beginPosition.Length = 0;
            endPosition = beginPosition;
        }

        public ChainParser<T> Begin<T>() where T : Element, new ()
        {
            var resurt = new ChainParser<T>(collection, index);
            resurt.EndEvent += i => index = i;
            return resurt;
        }
    }

    class ChainParser<T> : ChainParser where T : Element, new()
    {
        public event Action<int> EndEvent;
        private T self;
        internal bool failure { get; set; }
        private bool justFailure;
        private bool phaseIf;
        private bool phaseSkip;
        private bool phaseOrSkip;
        private bool phaseNot;
        private bool phaseOpt;
        
        public ChainParser(TokenCollection collection, int index)
            : base(collection, index)
        {
            self = new T();
        }

        public T End()
        {
            if (phaseIf)
            {
                throw new InvalidOperationException();
            }
            if(failure)
            {
                return null;
            }
            self.Position = beginPosition.AlterLength(endPosition);
            EndEvent(index);
            return self;
        }

        public ChainParser<T> Text(params string[] text)
        {
            return Text(null, text);
        }

        public ChainParser<T> Text(TokenAction<T> action, params string[] text)
        {
            if (IsSkip())
            {
                return this;
            }
            var s = collection.CheckText(index, text);
            if (s)
            {
                if (action != null)
                {
                    action(self, collection.Read(index));
                }
                endPosition = collection.GetTextPosition(index);
                ++index;
            }
            Post(s);
            return this;
        }

        public ChainParser<T> Type(params TokenType[] type)
        {
            return Type(null, type);
        }

        public ChainParser<T> Type(TokenAction<T> action, params TokenType[] type)
        {
            if (IsSkip())
            {
                return this;
            }
            var s = collection.CheckToken(index, type);
            if (s)
            {
                if (action != null)
                {
                    action(self, collection.Read(index));
                }
                endPosition = collection.GetTextPosition(index);
                ++index;
            }
            Post(s);
            return this;
        }

        public ChainParser<T> Token(TokenAction<T> action)
        {
            if (IsSkip())
            {
                return this;
            }
            var s = collection.IsReadable(index);
            if (s)
            {
                action(self, collection.Read(index));
                endPosition = collection.GetTextPosition(index);
                ++index;
            }
            Post(s);
            return this;
        }

        public ChainParser<T> Transfer<E>(ElementAction<T, E> action, ParserFunction<E> func) where E : Element
        {
            return Transfer<E>(action, new ParserFunction<E>[] { func });
        }

        public ChainParser<T> Transfer<E>(ElementAction<T, E> action, params ParserFunction<E>[] func) where E : Element
        {
            if (IsSkip())
            {
                return this;
            }
            E result = null;
            foreach (var f in func)
            {
                result = f(this);
                if (result != null)
                {
                    if (action != null)
                    {
                        action(self, result);
                    }
                    endPosition = result.Position;
                    break;
                }
            }
            Post(result != null);
            return this;
        }

        public ChainParser<T> Self(Action<T> action)
        {
            if (IsSkip())
            {
                return this;
            }
            action(self);
            Post(true);
            return this;
        }
        
        public ChainParser<T> Readble()
        {
            if (!IsSkip())
            {
                Post(collection.IsReadable(index));
            }
            return this;
        }

        public ChainParser<T> Is(bool value)
        {
            if (!IsSkip())
            {
                Post(value);
            }
            return this;
        }

        public ChainParser<T> Ignore(params TokenType[] type)
        {
            while (collection.CheckToken(index, type))
            {
                ++index;
            }
            return this;
        }

        public ChainParser<T> Lt()
        {
            return Ignore(TokenType.LineTerminator);
        }

        public ChainParser<T> If()
        {
            if (!IsSkip())
            {
                phaseIf = true;
            }
            return this;
        }

        public ChainParser<T> ElseIf()
        {
            if (phaseIf)
            {
                if (phaseSkip)
                {
                    phaseSkip = false;
                }
                else
                {
                    phaseIf = false;
                    phaseSkip = true;
                }
            }
            return this;
        }

        public ChainParser<T> Else()
        {
            if (phaseIf)
            {
                phaseSkip = !phaseSkip;
            }
            return this;
        }

        public ChainParser<T> Than()
        {
            if (phaseIf)
            {
                phaseSkip = failure;
                failure = false;
            }
            return this;
        }

        public ChainParser<T> EndIf()
        {
            phaseIf = false;
            phaseSkip = false;
            return this;
        }

        public ChainParser<T> And()
        {
            return this;
        }

        public ChainParser<T> Or()
        {
            if(justFailure)
            {
                failure = false;
                justFailure = false;
            }
            else
            {
                phaseOrSkip = true;
            }
            return this;
        }

        public ChainParser<T> Not()
        {
            phaseNot = true;
            return this;
        }

        public ChainParser<T> Opt()
        {
            phaseOpt = true;
            return this;
        }

        public ChainParser<T> Error()
        {
            if (IsSkip())
            {
                return this;
            }
            var s = collection.IsReadable(index);
            if (s)
            {
                collection.AddError(index);
                ++index;
            }
            Post(s);
            return this;
        }

        public LoopChainParser<T> Loop()
        {
            var resurt = new LoopChainParser<T>(collection, index, this);
            return resurt;
        }

        private bool IsSkip()
        {
            justFailure = false;
            if(phaseOrSkip)
            {
                phaseOrSkip = false;
                return true;
            }
            return failure || phaseSkip;
        }

        private void Post(bool success)
        {
            if (phaseNot)
            {
                failure = success;
                justFailure = failure;
            }
            else if (!phaseOpt)
            {
                failure = !success;
                justFailure = failure;
            }
            phaseNot = false;
            phaseOpt = false;
        }
    }

    class LoopChainParser<T> : ChainParser where T : Element, new()
    {
        private ChainParser<T> self;
        public List<Action<ChainParser<T>>> LoopList;
        private bool condFailure;

        public LoopChainParser(TokenCollection collection, int index, ChainParser<T> self)
            : base(collection, index)
        {
            LoopList = new List<Action<ChainParser<T>>>();
            this.self = self;
        }

        public LoopChainParser<T> Text(params string[] text)
        {
            LoopList.Add(cp => cp.Text(text));
            return this;
        }

        public LoopChainParser<T> Text(TokenAction<T> action, params string[] text)
        {
            LoopList.Add(cp => cp.Text(action, text));
            return this;
        }

        public LoopChainParser<T> Type(params TokenType[] type)
        {
            LoopList.Add(cp => cp.Type(type));
            return this;
        }

        public LoopChainParser<T> Type(TokenAction<T> action, params TokenType[] type)
        {
            LoopList.Add(cp => cp.Type(action, type));
            return this;
        }

        public LoopChainParser<T> Token(TokenAction<T> action)
        {
            LoopList.Add(cp => cp.Token(action));
            return this;
        }

        public LoopChainParser<T> Transfer<E>(ElementAction<T, E> action, ParserFunction<E> func) where E : Element
        {
            LoopList.Add(cp => cp.Transfer(action, func));
            return this;
        }

        public LoopChainParser<T> Transfer<E>(ElementAction<T, E> action, params ParserFunction<E>[] func) where E : Element
        {
            LoopList.Add(cp => cp.Transfer(action, func));
            return this;
        }

        public LoopChainParser<T> Self(Action<T> action)
        {
            LoopList.Add(cp => cp.Self(action));
            return this;
        }

        public LoopChainParser<T> Readble()
        {
            LoopList.Add(cp => cp.Readble());
            return this;
        }

        public LoopChainParser<T> Is(bool value)
        {
            LoopList.Add(cp => cp.Is(value));
            return this;
        }

        public LoopChainParser<T> Ignore(params TokenType[] type)
        {
            LoopList.Add(cp => cp.Ignore(type));
            return this;
        }

        public LoopChainParser<T> Lt()
        {
            LoopList.Add(cp => cp.Lt());
            return this;
        }

        public LoopChainParser<T> If()
        {
            LoopList.Add(cp => cp.If());
            return this;
        }

        public LoopChainParser<T> ElseIf()
        {
            LoopList.Add(cp => cp.ElseIf());
            return this;
        }

        public LoopChainParser<T> Else()
        {
            LoopList.Add(cp => cp.Else());
            return this;
        }

        public LoopChainParser<T> Than()
        {
            LoopList.Add(cp => cp.Than());
            return this;
        }

        public LoopChainParser<T> EndIf()
        {
            LoopList.Add(cp => cp.EndIf());
            return this;
        }

        public LoopChainParser<T> And()
        {
            LoopList.Add(cp => cp.And());
            return this;
        }

        public LoopChainParser<T> Or()
        {
            LoopList.Add(cp => cp.Or());
            return this;
        }

        public LoopChainParser<T> Not()
        {
            LoopList.Add(cp => cp.Not());
            return this;
        }

        public LoopChainParser<T> Opt()
        {
            LoopList.Add(cp => cp.Opt());
            return this;
        }

        public LoopChainParser<T> Error()
        {
            LoopList.Add(cp => cp.Error());
            return this;
        }

        public LoopChainParser<T> Do()
        {
            LoopList.Add(cp => DoFunc(cp));
            return this;
        }

        public void DoFunc(ChainParser<T> cp)
        {
            condFailure = cp.failure;
            cp.failure = false;
        }

        public ChainParser<T> EndLoop()
        {
            while (!condFailure && !self.failure)
            {
                foreach(var f in LoopList)
                {
                    f(self);
                    if(condFailure)
                    {
                        break;
                    }
                }
            }
            return self;
        }
    }
}

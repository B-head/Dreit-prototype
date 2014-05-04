using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntacticAnalysis
{
    delegate Element ParserFunction(ChainParser chain);
    delegate void TokenAction<T>(T self, Token token) where T : Element;
    delegate void ElementAction<T>(T self, Element element) where T : Element;

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
        private bool failure;
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

        public ChainParser<T> Transfer(ElementAction<T> action, params ParserFunction[] func)
        {
            if (IsSkip())
            {
                return this;
            }
            Element result = null;
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
}

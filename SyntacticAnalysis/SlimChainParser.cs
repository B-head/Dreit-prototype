using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntacticAnalysis
{
    delegate T MakeElement<T>(TextPosition tp) where T : Element;
    delegate SlimChainParser InsideParser(SlimChainParser cp);
    delegate void TokenAction(Token token);
    delegate void ElementAction<T>(T element) where T : Element;
    delegate T TransferParser<T>(SlimChainParser cp) where T : Element;

    struct SlimChainParser
    {
        private TokenCollection collection;
        private int index;
        private TextPosition beginPosition;
        private TextPosition endPosition;
        private bool failure;
        private bool ifResult;
        private bool phaseNot;
        private bool phaseOpt;

        public SlimChainParser(TokenCollection collection)
        {
            this.collection = collection;
            this.index = 0;
            this.beginPosition = new TextPosition();
            this.endPosition = new TextPosition();
            this.failure = false;
            this.ifResult = false;
            this.phaseNot = false;
            this.phaseOpt = false;
        }

        public static implicit operator bool(SlimChainParser cp)
        {
            return cp.failure;
        }

        public SlimChainParser Begin
        {
            get
            {
                var tb = collection.GetTextPosition(index);
                tb.Length = 0;
                return new SlimChainParser
                {
                    collection = collection,
                    index = index,
                    beginPosition = tb,
                    endPosition = tb,
                };
            }
        }

        public T End<T>(MakeElement<T> make) where T : Element
        {
            var tp = beginPosition.AlterLength(endPosition);
            return failure ? null : make(tp); 
        }

        private bool PostProcess(bool success)
        {
            var ret = failure;
            if (phaseNot)
            {
                ret = success;
            }
            else if (!phaseOpt)
            {
                ret = !success;
            }
            phaseNot = false;
            phaseOpt = false;
            return ret;
        }

        public SlimChainParser Not
        {
            get
            {
                phaseNot = true;
                return this;
            }
        }

        public SlimChainParser Opt
        {
            get
            {
                phaseOpt = true;
                return this;
            }
        }

        public SlimChainParser If(InsideParser inside)
        {
            if (failure) return this;
            var ret = inside(this);
            if(ret)
            {
                ifResult = false;
                return this;
            }
            else
            {
                ifResult = true;
                return ret;
            }
        }

        public SlimChainParser Than(InsideParser inside)
        {
            if (failure) return this;
            var ret = this;
            if (ifResult)
            {
                ret = inside(this);
                ret.failure = PostProcess(!ret.failure);
            }
            return ret;
        }

        public SlimChainParser Else(InsideParser inside)
        {
            if (failure) return this;
            var ret = this;
            if (!ifResult)
            {
                ret = inside(this);
                ret.failure = PostProcess(!ret.failure);
            }
            return ret;
        }

        public SlimChainParser ElseIf(InsideParser inside)
        {
            if (failure) return this;
            var ret = this;
            if (!ifResult)
            {
                ret = inside(this);
                ret.failure = PostProcess(!ret.failure);
            }
            return ret;
        }

        public SlimChainParser Any(params InsideParser[] insides)
        {
            if (failure) return this;
            foreach(var f in insides)
            {
                var r = f(this);
                if(!r)
                {
                    return r;
                }
            }
            return this;
        }

        public SlimChainParser Loop(InsideParser cond, InsideParser block = null)
        {
            if (failure) return this;
            SlimChainParser r;
            while (r = cond(this))
            {
                this = (block == null ? r : block(r));
            }
            return this;
        }

        public SlimChainParser Take(TokenAction action)
        {
            if (failure) return this;
            var s = collection.IsReadable(index);
            if (s)
            {
                action(collection.Read(index));
                endPosition = collection.GetTextPosition(index);
                ++index;
            }
            failure = PostProcess(s);
            return this;
        }

        public SlimChainParser Text(params string[] text)
        {
            return Text(null, text);
        }

        public SlimChainParser Text(TokenAction action, params string[] text)
        {
            if (failure) return this;
            var s = collection.CheckText(index, text);
            if (s)
            {
                if (action != null)
                {
                    action(collection.Read(index));
                }
                endPosition = collection.GetTextPosition(index);
                ++index;
            }
            failure = PostProcess(s);
            return this;
        }

        public SlimChainParser Type(params TokenType[] type)
        {
            return Type(null, type);
        }

        public SlimChainParser Type(TokenAction action, params TokenType[] type)
        {
            if (failure) return this;
            var s = collection.CheckToken(index, type);
            if (s)
            {
                if (action != null)
                {
                    action(collection.Read(index));
                }
                endPosition = collection.GetTextPosition(index);
                ++index;
            }
            failure = PostProcess(s);
            return this;
        }

        //public SlimChainParser Transfer<T>(ElementAction<T> action, TransferParser<T> func) where T : Element
        //{
        //    return Transfer<T>(action, new ParserFunction<T>[] { func });
        //}

        public SlimChainParser Transfer<T>(ElementAction<T> action, params TransferParser<T>[] func) where T : Element
        {
            if (failure) return this;
            T result = null;
            foreach (var f in func)
            {
                result = f(this);
                if (result != null)
                {
                    if (action != null)
                    {
                        action(result);
                    }
                    endPosition = result.Position;
                    break;
                }
            }
            failure = PostProcess(result != null);
            return this;
        }

        public SlimChainParser Readable
        {
            get
            {
                if (failure) return this;
                var r = collection.IsReadable(index);
                failure = PostProcess(r);
                return this;
            }
        }

        public SlimChainParser Is(bool cond)
        {
            if (failure) return this;
            failure = PostProcess(cond);
            return this;
        }

        public SlimChainParser Ignore(params TokenType[] type)
        {
            while (collection.CheckToken(index, type))
            {
                ++index;
            }
            return this;
        }

        public SlimChainParser Lt
        {
            get { return Ignore(TokenType.LineTerminator); }
        }

        public SlimChainParser AddError
        {
            get
            {
                if (failure) return this;
                var s = collection.IsReadable(index);
                if (s)
                {
                    collection.AddError(index);
                    ++index;
                }
                failure = PostProcess(s);
                return this;
            }
        }
    }
}

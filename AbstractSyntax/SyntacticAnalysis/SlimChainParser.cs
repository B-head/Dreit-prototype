using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SyntacticAnalysis
{
    delegate T MakeElement<T>(TextPosition tp) where T : Element;
    delegate T TransferParser<T>(SlimChainParser cp) where T : Element;
    delegate void InsideParser(SlimChainParser cp);
    delegate void TokenAction(Token token);
    delegate void ElementAction<T>(T element) where T : Element;

    class SlimChainParser
    {
        private SlimChainParser parent;
        private TokenCollection collection;
        private int index;
        private TextPosition beginPosition;
        private TextPosition endPosition;
        private bool failure;
        private bool ifResult;
        private bool ifSkip;
        private bool phaseNot;
        private bool phaseOpt;

        public SlimChainParser(TokenCollection collection)
        {
            this.collection = collection;
        }

        private SlimChainParser(SlimChainParser parent, TokenCollection collection, int index, TextPosition position)
        {
            this.parent = parent;
            this.collection = collection;
            this.index = index;
            this.beginPosition = position;
            this.endPosition = position;
        }

        public SlimChainParser Begin
        {
            get
            {
                var tb = collection.GetTextPosition(index);
                tb.Length = 0;
                return new SlimChainParser(this, collection, index, tb);
            }
        }

        public T End<T>(MakeElement<T> make) where T : Element
        {
            if (failure)
            {
                return null;
            }
            else
            {
                parent.index = index;
                parent.endPosition = endPosition;
                var tp = beginPosition.AlterLength(endPosition);
                return make(tp);
            }
        }

        public void End()
        {
            parent.failure = failure;
            if (!failure)
            {
                parent.index = index;
                parent.endPosition = endPosition;
            }
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

        private void PostProcess()
        {
            if (phaseNot)
            {
                failure = !failure;
            }
            else if (phaseOpt)
            {
                failure = false;
            }
            phaseNot = false;
            phaseOpt = false;
        }

        public SlimChainParser Not
        {
            get
            {
                if (failure) return this;
                phaseNot = true;
                return this;
            }
        }

        public SlimChainParser Opt
        {
            get
            {
                if (failure) return this;
                phaseOpt = true;
                return this;
            }
        }

        public SlimChainParser If(InsideParser inside)
        {
            if (failure) return this;
            inside(this);
            if(failure)
            {
                ifResult = false;
                failure = false;
                return this;
            }
            else
            {
                ifResult = true;
                return this;
            }
        }

        public SlimChainParser Than(InsideParser inside)
        {
            if (failure) return this;
            if (!ifSkip && ifResult)
            {
                inside(this);
            }
            return this;
        }

        public SlimChainParser Else(InsideParser inside)
        {
            if (failure) return this;
            if (!ifSkip && !ifResult)
            {
                inside(this);
            }
            ifSkip = false;
            return this;
        }

        public SlimChainParser ElseIf(InsideParser inside)
        {
            if (failure) return this;
            if (!ifSkip && !ifResult)
            {
                If(inside);
            }
            else
            {
                ifSkip = true;
            }
            return this;
        }

        public SlimChainParser Any(params InsideParser[] insides)
        {
            if (failure) return this;
            foreach(var f in insides)
            {
                var child = Begin;
                f(child);
                child.End();
                if(!failure)
                {
                    PostProcess();
                    return this;
                }
                failure = false;
            }
            failure = true; 
            PostProcess();
            return this;
        }

        public SlimChainParser Loop(InsideParser cond, InsideParser block = null)
        {
            if (failure) return this;
            for (cond(this); !failure; cond(this))
            {
                if(block == null)
                {
                    continue;
                }
                var child = Begin;
                block(child);
                child.End();
                if(failure)
                {
                    PostProcess();
                    return this;
                }
            }
            failure = false;
            PostProcess();
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

        public SlimChainParser Call<T>(Action<T> action, Func<SlimChainParser, T> func) where T : class
        {
            if (failure) return this;
            var child = Begin;
            T result = func(child); 
            if (result != null && action != null)
            {
                action(result);
            }
            child.End(); 
            PostProcess();
            return this;
        }

        public SlimChainParser Call(Action<SlimChainParser> func)
        {
            if (failure) return this;
            var child = Begin;
            func(child);
            child.End();
            PostProcess();
            return this;
        }

        public SlimChainParser Self(Action func)
        {
            if (failure) return this;
            func();
            return this;
        }

        public SlimChainParser Readable()
        {
            if (failure) return this;
            var r = collection.IsReadable(index);
            failure = PostProcess(r);
            return this;
        }

        public SlimChainParser Is(bool cond)
        {
            if (failure) return this;
            failure = PostProcess(cond);
            return this;
        }

        public SlimChainParser Ignore(params TokenType[] type)
        {
            if (failure) return this;
            while (collection.CheckToken(index, type))
            {
                ++index;
            }
            return this;
        }

        public SlimChainParser Lt()
        {
            return Ignore(TokenType.LineTerminator);
        }

        public SlimChainParser AddError()
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

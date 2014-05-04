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
            return new ChainParser<T>(collection, index);
        }
    }

    class ChainParser<T> : ChainParser where T : Element, new()
    {
        private T self;
        private bool failure;
        
        public ChainParser(TokenCollection collection, int index = 0)
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
            return self;
        }

        public ChainParser<T> Text(params string[] text)
        {
            return Text(null, text);
        }

        public ChainParser<T> Text(TokenAction<T> action, params string[] text)
        {
            if (failure)
            {
                return this;
            }
            var s = collection.CheckText(index, text);
            if (s)
            {
                action(self, collection.Read(index));
                endPosition = collection.GetTextPosition(index);
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
            if (failure)
            {
                return this;
            }
            var s = collection.CheckToken(index, type);
            if (s)
            {
                action(self, collection.Read(index));
                endPosition = collection.GetTextPosition(index);
            }
            Post(s);
            return this;
        }

        public ChainParser<T> Transfer(ElementAction<T> action, params ParserFunction[] func)
        {
            if (failure)
            {
                return this;
            }
            Element result = null;
            foreach (var f in func)
            {
                result = f(this);
                if (result != null)
                {
                    action(self, result);
                    endPosition = result.Position;
                    break;
                }
            }
            Post(result != null);
            return this;
        }

        private void Post(bool success)
        {
            failure = !success;
            if(success)
            {
                ++index;
            }
        }
    }
}

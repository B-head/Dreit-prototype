using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntacticAnalysis
{
    delegate T ParserFunction<T>(ChainParser chain) where T : Element;

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
            if (failure)
            {
                return this;
            }
            var s = collection.CheckText(index, text);
            Post(s);
            return this;
        }

        public ChainParser<T> Type(params TokenType[] type)
        {
            if (failure)
            {
                return this;
            }
            var s = collection.CheckToken(index, type);
            Post(s);
            return this;
        }

        private void Post(bool success)
        {
            failure = !success;
            if(success)
            {
                endPosition = collection.GetTextPosition(index);
                ++index;
            }
        }
    }
}

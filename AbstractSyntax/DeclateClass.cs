using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class DeclateClass : Scope
    {
        public ClassTranslator ClassTrans { get; private set; }
        public Identifier Ident { get; set; }
        public Element Generic { get; set; }
        public Element Inherit { get; set; }
        public Element Block { get; set; }
        public List<DeclateClass> InheritRefer { get; private set; }

        internal override Scope DataType
        {
            get { return this; }
        }

        public override int Count
        {
            get { return 4; }
        }

        public override Element Child(int index)
        {
            switch (index)
            {
                case 0: return Ident;
                case 1: return Generic;
                case 2: return Inherit;
                case 3: return Block;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override string CreateName()
        {
            return Ident == null ? null : Ident.Value;
        }

        internal override void SpreadTranslate(Translator trans)
        {
            if (IsImport)
            {
                base.SpreadTranslate(trans);
            }
            else
            {
                ClassTrans = trans.CreateClass(FullPath);
                base.SpreadTranslate(ClassTrans);
            }
        }

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
            var refer = new List<DeclateClass>();
            if (Inherit is TupleList)
            {
                foreach (var v in Inherit)
                {
                    var temp = v.DataType as DeclateClass;
                    if (temp == null)
                    {
                        CompileError("継承元はクラスである必要があります。");
                    }
                    refer.Add(temp);
                }
            }
            else if(Inherit != null)
            {
                var temp = Inherit.DataType as DeclateClass;
                if (temp == null)
                {
                    CompileError("継承元はクラスである必要があります。");
                }
                refer.Add(temp);
            }
            InheritRefer = refer;
        }

        internal override void Translate(Translator trans)
        {
            base.Translate(ClassTrans);
        }

        public bool IsContain(DeclateClass other)
        {
            return Object.ReferenceEquals(this, other);
        }

        public bool IsConvert(DeclateClass other)
        {
            if(IsContain(other))
            {
                return true;
            }
            foreach(var v in InheritRefer)
            {
                if(v.IsConvert(other))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

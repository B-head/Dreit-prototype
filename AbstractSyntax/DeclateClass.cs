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
        public TupleList Generic { get; set; }
        public TupleList Inherit { get; set; }
        public DirectiveList Block { get; set; }
        public List<DeclateClass> InheritRefer { get; private set; }

        public override bool IsVoidValue
        {
            get { return true; }
        }

        public override int Count
        {
            get { return 4; }
        }

        public override Element GetChild(int index)
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

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
            if(Inherit == null)
            {
                return;
            }
            var refer = new List<DeclateClass>();
            foreach (var v in Inherit)
            {
                var temp = v.DataType as DeclateClass;
                if (temp == null)
                {
                    CompileError("継承元はクラスである必要があります。");
                }
                refer.Add(temp);
            }
            InheritRefer = refer;
        }

        internal override void PreSpreadTranslate(Translator trans)
        {
            ClassTrans = trans.CreateClass(FullPath);
            base.PreSpreadTranslate(ClassTrans);
        }

        internal override void PostSpreadTranslate(Translator trans)
        {
            base.PostSpreadTranslate(ClassTrans);
        }

        internal override void Translate(Translator trans)
        {
            Block.Translate(ClassTrans);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class DeclateArgument : Scope
    {
        public IdentifierAccess Ident { get; set; }
        public Element ExplicitArgumentType { get; set; }
        private Scope _DataType;

        internal override Scope DataType
        {
            get { return _DataType; }
        }

        public override bool IsAssignable
        {
            get { return true; }
        }

        public override int Count
        {
            get { return 2; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Ident;
                case 1: return ExplicitArgumentType;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override string CreateName()
        {
            return Ident == null ? null : Ident.Value;
        }

        internal override void PostSpreadTranslate(Translator trans)
        {
            if (!IsImport)
            {
                RoutineTranslator routTrans = trans as RoutineTranslator;
                routTrans.CreateArgument(FullPath, DataType.FullPath);
                base.PostSpreadTranslate(trans);
            }
        }

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
            if (ExplicitArgumentType != null)
            {
                _DataType = ExplicitArgumentType.DataType;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace CliTranslate
{
    public class VariantTranslator : Translator
    {
        public Translator TypeTrans { get; private set; }
        public LocalBuilder Local { get; private set; }
        public FieldInfo Field { get; private set; }

        public VariantTranslator(string name, Translator parent, FieldInfo field = null)
            : base(name, parent)
        {
            Field = field;
        }

        protected override void SpreadBuilder()
        {
            CreateBuilder((dynamic)Parent);
            base.SpreadBuilder();
        }

        private void CreateBuilder(NameSpaceTranslator trans)
        {
            Field = trans.GlobalField.DefineField(Name, GetDataType(), FieldAttributes.Static);
        }

        private void CreateBuilder(ClassTranslator trans)
        {

        }

        private void CreateBuilder(RoutineTranslator trans)
        {

        }

        public override void SetBaseType(Translator type)
        {
            TypeTrans = type;
        }

        private Type GetDataType()
        {
            return GetDataType((dynamic)TypeTrans);
        }

        private Type GetDataType(ClassTranslator trans)
        {
            return trans.TypeInfo;
        }

        private Type GetDataType(EnumTranslator trans)
        {
            return null;
        }

        private Type GetDataType(PolyTranslator trans)
        {
            return null;
        }
    }
}

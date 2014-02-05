using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    class NameSpaceTranslator : ContextTranslator
    {
        public MethodBuilder GlobalContext { get; private set; }
        public ILGenerator Gen { get; private set; }

        public NameSpaceTranslator(string name, Translator parent)
            : base(name, parent)
        {

        }

        public override Translator CreateNameSpace(string name)
        {
            return new NameSpaceTranslator(name, this);
        }

        protected void RegisterField(List<string> name, FieldInfo field)
        {
            if (name.Count <= 1)
            {
                new VariantTranslator(name[0], this, field);
                return;
            }
            Translator next;
            if (!Child.TryGetValue(name[0], out next))
            {
                next = CreateNameSpace(name[0]);
            }
            name.RemoveAt(0);
            ((NameSpaceTranslator)next).RegisterField(name, field);
        }

        protected void RegisterMethod(List<string> name, MethodInfo method)
        {
            if (name.Count <= 1)
            {
                new RoutineTranslator(name[0], this, method);
                return;
            }
            Translator next;
            if (!Child.TryGetValue(name[0], out next))
            {
                next = CreateNameSpace(name[0]);
            }
            name.RemoveAt(0);
            ((NameSpaceTranslator)next).RegisterMethod(name, method);
        }

        protected void RegisterType(List<string> name, Type type)
        {
            if(name.Count <= 1)
            {
                if(type.IsEnum)
                {
                    new EnumTranslator(name[0], this, type);
                }
                else
                {
                    new ClassTranslator(name[0], this, type);
                }
                return;
            }
            Translator next;
            if(!Child.TryGetValue(name[0], out next))
            {
                next = CreateNameSpace(name[0]);
            }
            name.RemoveAt(0);
            ((NameSpaceTranslator)next).RegisterType(name, type);
        }

        protected override void SpreadBuilder()
        {
            if(Code.Count > 0)
            {
                var attr = MethodAttributes.Static | MethodAttributes.SpecialName;
                GlobalContext = Root.Module.DefineGlobalMethod(GetSpecialName("global"), attr, null, null);
                Gen = GlobalContext.GetILGenerator();
                Root.Assembly.SetEntryPoint(GlobalContext);
            }
            base.SpreadBuilder();
        }

        protected override void Translate()
        {
            if (GlobalContext != null)
            {
                BuildCode(Gen);
                var stdout = NameResolution("stdout");
                if(stdout != null && stdout is VariantTranslator)
                {
                    Gen.EmitWriteLine(((VariantTranslator)stdout).Local);
                }
                Gen.Emit(OpCodes.Ret);
            }
            base.Translate();
        }
    }
}

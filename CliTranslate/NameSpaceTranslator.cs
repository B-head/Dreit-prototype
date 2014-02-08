using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Common;

namespace CliTranslate
{
    public class NameSpaceTranslator : ContextTranslator
    {
        public TypeBuilder GlobalField { get; private set; }
        public MethodBuilder EntryContext { get; private set; }

        public NameSpaceTranslator(FullPath path, Translator parent)
            : base(path, parent)
        {

        }

        public override Translator CreateNameSpace(FullPath path)
        {
            return new NameSpaceTranslator(path, this);
        }

        /*protected void RegisterField(List<string> name, FieldInfo field)
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
        }*/

        protected void SpreadBuilder()
        {
            if(Code.Count > 0)
            {
                GlobalField = Root.Module.DefineType(GetSpecialName("global"), TypeAttributes.SpecialName);
                EntryContext = Root.Module.DefineGlobalMethod(GetSpecialName("entry"), MethodAttributes.SpecialName | MethodAttributes.Static, null, null);
                Root.Assembly.SetEntryPoint(EntryContext);
            }
            //base.SpreadBuilder();
        }

        protected override void Translate()
        {
            if (EntryContext != null)
            {
                var gen = EntryContext.GetILGenerator();
                BuildCode(gen);
                /*var stdout = NameResolution("stdout");
                if(stdout != null && stdout is VariantTranslator)
                {
                    gen.EmitWriteLine(((VariantTranslator)stdout).Field);
                }*/
                gen.Emit(OpCodes.Ret);
            }
            base.Translate();
            if (GlobalField != null)
            {
                GlobalField.CreateType();
            }
        }
    }
}

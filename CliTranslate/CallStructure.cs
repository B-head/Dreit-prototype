using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class CallStructure : ExpressionStructure
    {
        public BuilderStructure Call { get; private set; }
        public ExpressionStructure Pre { get; private set; }
        public CilStructure Access { get; private set; }
        public CilStructure Variant { get; private set; }
        public IReadOnlyList<ExpressionStructure> Arguments { get; private set; }
        public bool IsVariadic { get; private set; }

        public CallStructure(TypeStructure rt, BuilderStructure call, ExpressionStructure pre, CilStructure variant)
            : base(rt)
        {
            Call = call;
            Pre = pre;
            Access = pre;
            Variant = variant;
            Arguments = new List<ExpressionStructure>();
            if (Access != null)
            {
                AppendChild(Access);
            }
            if (Arguments != null)
            {
                AppendChild(Arguments);
            }
        }

        public CallStructure(TypeStructure rt, BuilderStructure call, ExpressionStructure pre, CilStructure access, CilStructure variant, IReadOnlyList<ExpressionStructure> args, bool isVariadic = false)
            :base(rt)
        {
            Call = call;
            Pre = pre;
            Access = access;
            Variant = variant;
            Arguments = args;
            IsVariadic = isVariadic;
            if (Access != null)
            {
                AppendChild(Access);
            }
            if (Arguments != null)
            {
                AppendChild(Arguments);
            }
        }

        internal override void BuildCode()
        {
            if(CurrentContainer.IsDataTypeContext)
            {
                return;
            }
            var cg = CurrentContainer.GainGenerator();
            if (Pre != null)
            {
                Pre.BuildCode();
            }
            if (Call == null)
            {
                return;
            }
            if (IsVariadic)
            {
                var arr = new LocalStructure(GetVariadicType(Call), cg);
                cg.GenerateArray(GetVariadicLangth(Call), arr.DataType.GetBaseType());
                cg.GenerateStore(arr);
                var vi = GetVariadicIndex(Call);
                for (var i = 0; i < Arguments.Count; ++i)
                {
                    if (i < vi)
                    {
                        Arguments[i].BuildCode();
                    }
                    else
                    {
                        cg.GenerateLoad(arr);
                        cg.GeneratePrimitive(i - vi);
                        Arguments[i].BuildCode();
                        cg.GenerateBoxing(Arguments[i].ResultType, arr.DataType.GetBaseType());
                        cg.GenerateStoreElement(arr.DataType.GetBaseType());
                    }
                }
                cg.GenerateLoad(arr);
            }
            else
            {
                foreach (var v in Arguments)
                {
                    v.BuildCode();
                }
            }
            var lss = Call as LoadStoreStructure;
            var gms = Call as GenericMethodStructure;
            if (lss != null)
            {
                lss.BuildCall(Variant, cg);
            }
            else if(gms != null)
            {
                gms.BuildCall(Variant, cg);
            }
            else
            {
                Call.BuildCall(cg);
            }
        }

        private TypeStructure GetVariadicType(BuilderStructure call)
        {
            var c = call as MethodBaseStructure;
            if (c == null)
            {
                return null;
            }
            return c.Arguments.Last().ParamType;
        }

        private int GetVariadicLangth(BuilderStructure call)
        {
            var c = call as MethodBaseStructure;
            if (c == null)
            {
                return -1;
            }
            return Arguments.Count - c.Arguments.Count + 1;
        }

        private int GetVariadicIndex(BuilderStructure call)
        {
            var c = call as MethodBaseStructure;
            if(c == null)
            {
                return -1;
            }
            return c.Arguments.Count - 1;
        }
    }
}

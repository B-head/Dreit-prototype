using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class ArrayLiteral : Element
    {
        public IReadOnlyList<Element> Values { get; private set; }
        private Scope _BaseType;
        private Scope _ReturnType;

        public ArrayLiteral(TextPosition tp, IReadOnlyList<Element> values)
            :base(tp)
        {
            Values = values;
            AppendChild(Values);
        }

        public Scope BaseType
        {
            get
            {
                if(_BaseType != null)
                {
                    return _BaseType;
                }
                if(Values.Count == 0)
                {
                    _BaseType = CurrentScope.NameResolution("Object").FindDataType();
                }
                else
                {
                    _BaseType = Values[0].ReturnType;
                }
                return _BaseType;
            }
        }

        public override Scope ReturnType
        {
            get
            {
                if (_ReturnType != null)
                {
                    return _ReturnType;
                }
                var list = CurrentScope.NameResolution("List");
                _ReturnType = Root.TemplateInstanceManager.Issue(list, new Scope[] { BaseType });
                return _ReturnType;
            }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            foreach (var v in Values)
            {
                if (BaseType != v.ReturnType)
                {
                    cmm.CompileError("disagree-array-type", this);
                }
            }
        }
    }
}

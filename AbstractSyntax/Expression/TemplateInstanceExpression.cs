using AbstractSyntax.Literal;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class TemplateInstanceExpression : Element
    {
        public Element Access { get; private set; }
        public TupleLiteral DecParameters { get; private set; }
        private OverLoadCallMatch? _Match;
        private IReadOnlyList<TypeSymbol> _Parameter;

        public TemplateInstanceExpression(TextPosition tp, Element acs, TupleLiteral args)
            : base(tp)
        {
            Access = acs;
            DecParameters = args;
            AppendChild(Access);
            AppendChild(DecParameters);
        }

        public override TypeSymbol ReturnType
        {
            get { return OverLoad.FindDataType().Type; }
        }

        public VariantSymbol ReferVariant
        {
            get { return OverLoad.FindVariant(); }
        }

        public RoutineSymbol CallRoutine
        {
            get { return Match.Call; }
        }

        public Scope AccessSymbol
        {
            get { return CallRoutine.IsAliasCall ? (Scope)ReferVariant : (Scope)CallRoutine; }
        }

        public OverLoadCallMatch Match
        {
            get
            {
                if (_Match != null)
                {
                    return _Match.Value;
                }
                _Match = OverLoad.CallSelect();
                return _Match.Value;
            }
        }

        public override OverLoad OverLoad
        {
            get { return new OverLoadModify(Access.OverLoad, Parameter); }
        }

        public override bool IsConstant
        {
            get { return Access.IsConstant; }
        }

        public IReadOnlyList<TypeSymbol> Parameter
        {
            get
            {
                if (_Parameter != null)
                {
                    return _Parameter;
                }
                var pt = new List<TypeSymbol>();
                foreach (var v in DecParameters)
                {
                    var temp = v.OverLoad.FindDataType().Type;
                    pt.Add(temp);
                }
                _Parameter = pt;
                return _Parameter;
            }
        }

    }
}

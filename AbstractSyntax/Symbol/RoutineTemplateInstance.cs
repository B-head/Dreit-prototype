using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class RoutineTemplateInstance : RoutineSymbol
    {
        public RoutineSymbol Routine { get; private set; }
        public IReadOnlyList<TypeSymbol> Parameters { get; private set; }

        public RoutineTemplateInstance(RoutineSymbol routine, IReadOnlyList<TypeSymbol> parameter)
            :base(RoutineType.Unknown, TokenType.Unknoun)
        {
            Routine = routine;
            Parameters = parameter;
        }

        protected override string ElementInfo
        {
            get
            {
                if (Parameters.Count == 0)
                {
                    return string.Format("{0}", Routine.Name);
                }
                else
                {
                    return string.Format("{0}!({1})", Routine.Name, Parameters.ToNames());
                }
            }
        }

        public override TypeSymbol ReturnType
        {
            get { return Root.ErrorType; } //todo インスタンス化したデリゲート型を返すようにする。
        }

        public override OverLoad OverLoad
        {
            get { return Root.SimplexManager.Issue(this); } //todo インスタンス化したオーバーロードを返す。
        }

        public override TypeSymbol CallReturnType
        {
            get { return _CallReturnType ?? Root.ErrorType; } //todo インスタンス化した返り値を返す。
        }
    }
}

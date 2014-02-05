using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DlightObject
{
    public class Binary64
    {
        private Double Value;

        public Binary64(Double value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public Binary64 opAdd(Binary64 arg)
        {
            return new Binary64(Value + arg.Value);
        }

        public Binary64 opSubtract(Binary64 arg)
        {
            return new Binary64(Value - arg.Value);
        }

        public Binary64 opMultiply(Binary64 arg)
        {
            return new Binary64(Value * arg.Value);
        }

        public Binary64 opDivide(Binary64 arg)
        {
            return new Binary64(Value / arg.Value);
        }

        public Binary64 opModulo(Binary64 arg)
        {
            return new Binary64(Value % arg.Value);
        }
    }
}

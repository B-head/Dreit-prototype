using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DlightObject
{
    public class Integer32
    {
        private Int32 Value;

        public Integer32(Int32 value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public Integer32 opAdd(Integer32 arg)
        {
            return new Integer32(Value + arg.Value);
        }

        public Integer32 opSubtract(Integer32 arg)
        {
            return new Integer32(Value - arg.Value);
        }

        public Integer32 opMultiply(Integer32 arg)
        {
            return new Integer32(Value * arg.Value);
        }

        public Integer32 opDivide(Integer32 arg)
        {
            return new Integer32(Value / arg.Value);
        }

        public Integer32 opModulo(Integer32 arg)
        {
            return new Integer32(Value % arg.Value);
        }
    }
}

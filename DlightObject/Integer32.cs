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

        public Integer32 opCall()
        {
            return this;
        }

        public Integer32 opCall(Integer32 arg)
        {
            return new Integer32(Value = arg.Value);
        }

        public bool opEqual(Integer32 arg)
        {
            return Value == arg.Value;
        }

        public bool opNotEqual(Integer32 arg)
        {
            return Value != arg.Value;
        }

        public bool opLessThan(Integer32 arg)
        {
            return Value < arg.Value;
        }

        public bool opLessThanOrEqual(Integer32 arg)
        {
            return Value <= arg.Value;
        }

        public bool opGreaterThan(Integer32 arg)
        {
            return Value > arg.Value;
        }

        public bool opGreaterThanOrEqual(Integer32 arg)
        {
            return Value >= arg.Value;
        }

        public bool opIncompare(Integer32 arg)
        {
            return false;
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

        public Integer32 opExponent(Integer32 arg)
        {
            return new Integer32((Int32)Math.Pow(Value, arg.Value));
        }

        public Integer32 opOr(Integer32 arg)
        {
            return new Integer32(Value | arg.Value);
        }

        public Integer32 opAnd(Integer32 arg)
        {
            return new Integer32(Value & arg.Value);
        }

        public Integer32 opXor(Integer32 arg)
        {
            return new Integer32(Value ^ arg.Value);
        }

        public Integer32 opLeftShift(Integer32 arg)
        {
            return new Integer32(Value << arg.Value);
        }

        public Integer32 opRightShift(Integer32 arg)
        {
            return new Integer32(Value >> arg.Value);
        }

        public Integer32 opNegate()
        {
            return new Integer32(-Value);
        }

        public Integer32 opInvert()
        {
            return new Integer32(~Value);
        }
    }
}

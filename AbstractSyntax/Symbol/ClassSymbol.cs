using AbstractSyntax.Daclate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    public class ClassSymbol : DataType
    {
        protected List<ClassSymbol> _InheritRefer; 
        
        public virtual List<ClassSymbol> InheritRefer
        {
            get { return _InheritRefer; }
        }

        public bool IsContain(ClassSymbol other)
        {
            return Object.ReferenceEquals(this, other);
        }

        public bool IsConvert(ClassSymbol other)
        {
            if (IsContain(other))
            {
                return true;
            }
            foreach (var v in InheritRefer)
            {
                if (v.IsConvert(other))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

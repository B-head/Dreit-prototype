using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class PlainText : Element
    {
        public string Value { get; set; }
        private Lazy<IDataType> LazyReturnType;

        public PlainText()
        {
            LazyReturnType = new Lazy<IDataType>(InitReturnType);
        }

        public override IDataType ReturnType
        {
            get { return LazyReturnType.Value; }
        }

        private IDataType InitReturnType()
        {
            return CurrentScope.NameResolution("String").FindDataType();
        }

        public string ShowValue
        {
            get { return Regex.Replace(Value, @"\\.", TrimEscape); }
        }

        private string TrimEscape(Match m)
        {
            return m.Value.Substring(1);
        }
    }
}

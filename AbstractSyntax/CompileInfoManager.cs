using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public class CompileInfoManager : IReadOnlyList<CompileInfo>
    {
        private List<CompileInfo> List;
        public int MessageCount { get; private set; }
        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }

        public CompileInfoManager()
        {
            List = new List<CompileInfo>();
        }

        public void Append(CompileInfo info)
        {
            switch(info.Type)
            {
                case CompileInfoType.Message: ++MessageCount; break;
                case CompileInfoType.Error: ++ErrorCount; break;
                case CompileInfoType.Warning: ++WarningCount; break;
            }
            List.Add(info);
        }

        public override string ToString()
        {
            return "Error = " + ErrorCount + ", Warning = " + WarningCount + ", Message = " + MessageCount;
        }

        public CompileInfo this[int index]
        {
            get { return List[index]; }
        }

        public int Count
        {
            get { return List.Count; }
        }

        public IEnumerator<CompileInfo> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct CompileInfo
    {
        public CompileInfoType Type { get; set; } 
        public string Key { get; set; }
        public TextPosition Position { get; set; }
        public object Target { get; set; }
    }

    public enum CompileInfoType
    {
        Message,
        Error,
        Warning,
    }
}

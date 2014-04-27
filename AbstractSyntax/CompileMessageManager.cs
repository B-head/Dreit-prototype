using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public class CompileMessageManager : IReadOnlyList<CompileMessage>
    {
        private List<CompileMessage> List;
        public int InfoCount { get; private set; }
        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }

        public CompileMessageManager()
        {
            List = new List<CompileMessage>();
        }

        public void Append(CompileMessage info)
        {
            switch(info.Type)
            {
                case CompileMessageType.Info: ++InfoCount; break;
                case CompileMessageType.Error: ++ErrorCount; break;
                case CompileMessageType.Warning: ++WarningCount; break;
            }
            List.Add(info);
        }

        public override string ToString()
        {
            return "Error = " + ErrorCount + ", Warning = " + WarningCount + ", Information = " + InfoCount;
        }

        public CompileMessage this[int index]
        {
            get { return List[index]; }
        }

        public int Count
        {
            get { return List.Count; }
        }

        public IEnumerator<CompileMessage> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Serializable]
    public struct CompileMessage
    {
        public CompileMessageType Type { get; set; } 
        public string Key { get; set; }
        public TextPosition Position { get; set; }
        public object Target { get; set; }

        public string GetPrefix()
        {
            switch (Type)
            {
                case CompileMessageType.Info: return "Info";
                case CompileMessageType.Error: return "Error";
                case CompileMessageType.Warning: return "Warning";
            }
            throw new ArgumentException();
        }

        public override string ToString()
        {
            return GetPrefix() + ": " + Key;
        }
    }

    public enum CompileMessageType
    {
        Info,
        Error,
        Warning,
    }
}

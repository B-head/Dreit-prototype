/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
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
        public int MessageCount { get; private set; }
        public int InfoCount { get; private set; }
        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }

        public CompileMessageManager()
        {
            List = new List<CompileMessage>();
        }

        public void CompileInfo(string key, object target)
        {
            Append(key, target, CompileMessageType.Info);
        }

        public void CompileError(string key, object target)
        {
            Append(key, target, CompileMessageType.Error);
        }

        public void CompileWarning(string key, object target)
        {
            Append(key, target, CompileMessageType.Warning);
        }

        private void Append(string key, object target, CompileMessageType type)
        {
            ++MessageCount;
            switch (type)
            {
                case CompileMessageType.Info: ++InfoCount; break;
                case CompileMessageType.Error: ++ErrorCount; break;
                case CompileMessageType.Warning: ++WarningCount; break;
            }
            var pos = new TextPosition();
            var t = target as Token?;
            if(t != null)
            {
                pos = t.Value.Position;
            }
            var e = target as Element;
            if(e != null)
            {
                pos = e.Position;
            }
            CompileMessage info = new CompileMessage
            {
                MessageType = type,
                Key = key,
                Position = pos,
                Target = target,
            };
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
        public CompileMessageType MessageType { get; set; } 
        public string Key { get; set; }
        public TextPosition Position { get; set; }
        public object Target { get; set; }

        public string StringPrefix
        {
            get
            {
                switch (MessageType)
                {
                    case CompileMessageType.Info: return "Info";
                    case CompileMessageType.Error: return "Error";
                    case CompileMessageType.Warning: return "Warning";
                }
                throw new InvalidOperationException();
            }
        }

        public override string ToString()
        {
            return StringPrefix + ": " + Key;
        }
    }

    public enum CompileMessageType
    {
        Info,
        Error,
        Warning,
    }
}

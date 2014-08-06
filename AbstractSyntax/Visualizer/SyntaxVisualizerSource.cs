using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbstractSyntax.Visualizer
{
    public class SyntaxVisualizerSource : VisualizerObjectSource
    {
        private int NextId;
        private List<object> IdList;
        private Dictionary<object, SyntaxVisualizerTree> TreeDictionary;
        private const BindingFlags showMenber = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public SyntaxVisualizerSource()
        {
            IdList = new List<object>();
            TreeDictionary = new Dictionary<object, SyntaxVisualizerTree>();
        }

        public override void GetData(object target, Stream outgoingData)
        {
            dynamic temp = target;
            var root = temp.Root;
            var data = new InitalizeData();
            data.Root = MakeTree(root);
            data.Target = MakeTree(target);
            Serialize(outgoingData, data);
        }

        public override void TransferData(object target, Stream incomingData, Stream outgoingData)
        {
            var id = (int)Deserialize(incomingData);
            object tree = IdList[id];
            var child = new List<SyntaxVisualizerTree>();
            foreach (var v in (IEnumerable)tree)
            {
                child.Add(MakeTree(v));
            }
            var type = tree.GetType();
            var prop = new Dictionary<string, object>();
            foreach (var v in type.GetProperties(showMenber))
            {
                object obj;
                try
                {
                    obj = v.GetValue(tree) ?? "<null>";
                }
                catch (Exception e)
                {
                    obj = e.ToString();
                }
                obj = DeepClone(obj);
                prop.Add(v.Name, obj);
            }
            var data = new TreeData();
            data.Child = child;
            data.PropertyValues = prop;
            Serialize(outgoingData, data);
        }

        private SyntaxVisualizerTree MakeTree(dynamic data)
        {
            if(data == null)
            {
                return null;
            }
            if(TreeDictionary.ContainsKey(data))
            {
                return TreeDictionary[data];
            }
            var parent = MakeTree(data.Parent);
            var id = NextId++;
            var ret = new SyntaxVisualizerTree(id, data.ToString(), parent);
            IdList.Add(data);
            TreeDictionary.Add(data, ret);
            return ret;
        }

        private object DeepClone(object obj)
        {
            if (obj == null)
            {
                return "<null>";
            }
            if (obj.GetType().GetInterface("IReadOnlyTree`1") != null)
            {
                return MakeTree(obj);
            }
            if (obj.GetType().GetInterface("IReadOnlyList`1") != null)
            {
                var list = new List<object>();
                foreach(var v in (IEnumerable)obj)
                {
                    list.Add(DeepClone(v));
                }
                return list;
            }
            return obj.ToString();
        }
    }

    [Serializable]
    struct InitalizeData
    {
        public SyntaxVisualizerTree Root { get; set; }
        public SyntaxVisualizerTree Target { get; set; }
    }

    [Serializable]
    struct TreeData
    {
        public IReadOnlyList<SyntaxVisualizerTree> Child { get; set; }
        public IReadOnlyDictionary<string, object> PropertyValues { get; set; }
    }
}

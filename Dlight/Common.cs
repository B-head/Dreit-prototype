using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    interface Translator
    {
        Translator CreateModule(Scope<Element> scope);
        Translator CreateVariable(Scope<Element> scope, string fullName);
        void GenelateLoad(string fullName);
        void GenelateStore(string fullName);
        void GenelateNumber(int value);
        void GenelateBinomial(string fullName, TokenType operation);
    }

    static class Common
    {
        public static string Indent(int indent)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < indent; i++)
            {
                result.Append(" ");
            }
            return result.ToString();
        }

        public static bool Match<V, T>(this V value, IEnumerable<T> list) where V : IEquatable<T>
        {
            foreach (T v in list)
            {
                if (value.Equals(v))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Match<V>(this V value, V stert, V end) where V : IComparable<V>
        {
            return value.CompareTo(stert) >= 0 && value.CompareTo(end) <= 0;
        }
    }

    class ErrorManager
    {
        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }

        public void Error(string format, params object[] args)
        {
            Console.WriteLine("Error: " + format, args);
            ErrorCount++;
        }

        public void Warning(string format, params object[] args)
        {
            Console.WriteLine("Warning: " + format, args);
            WarningCount++;
        }

        public override string ToString()
        {
            return "Error = " + ErrorCount + ", Warning = " + WarningCount; 
        }
    }

    class Scope<V>
    {
        public V Value { get; private set; }
        public string Name { get; private set; }
        public Scope<V> Parent { get; private set; }
        private Dictionary<string, Scope<V>> Child;

        public Scope(V value, string name = null, Scope<V> parent = null)
        {
            Value = value;
            Name = name;
            Parent = parent;
            Child = new Dictionary<string, Scope<V>>();
        }

        public Scope<V> CreateChild(V value, string name)
        {
            Scope<V> result = new Scope<V>(value, name, this);
            Child.Add(name, result);
            return result;
        }

        public Scope<V> GetRoot()
        {
            return Parent ?? this;
        }

        public IReadOnlyDictionary<string, Scope<V>> GetChild()
        {
            return Child;
        }

        public string GetFullName()
        {
            if (Parent == null)
            {
                return Name;
            }
            string p = Parent.GetFullName();
            return p == null ? Name : p + "." + Name;
        }

        public Scope<V> NameResolution(string name)
        {
            if (name == Name)
            {
                return this;
            }
            Scope<V> temp;
            if (Child.TryGetValue(name, out temp))
            {
                return temp;
            }
            if (Parent == null)
            {
                return null;
            }
            return Parent.NameResolution(name);
        }
    }
}

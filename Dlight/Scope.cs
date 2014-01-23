using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
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
            return Parent.NameResolution(name);
        }
    }
}

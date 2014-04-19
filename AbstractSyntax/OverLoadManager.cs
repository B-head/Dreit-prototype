using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Daclate;

namespace AbstractSyntax
{
    class OverLoadManager
    {
        private List<Element> _OverLoad;
        public IReadOnlyList<Element> OverLoad { get { return _OverLoad; } }

        public OverLoadManager()
        {
            _OverLoad = new List<Element>();
        }

        public void Append(Element elemant)
        {
            _OverLoad.Add(elemant);
        }

        public bool CheckType()
        {
            Type temp = null;
            for (var i = 0; i < _OverLoad.Count; i++)
            {
                if (i == 0)
                {
                    temp = _OverLoad[i].GetType();
                    continue;
                }
                if (!(_OverLoad[i] is DeclateRoutine || _OverLoad[i] is DeclateClass))
                {
                    return false;
                }
                if (temp != _OverLoad[i].GetType())
                {
                    return false;
                }
            }
            return true;
        }


    }
}

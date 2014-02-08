using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Common;

namespace CliTranslate
{
    public class OverLoadTranslator : Translator
    {
        private List<Translator> _OverLoad;
        public IReadOnlyList<Translator> OverLoad { get { return _OverLoad; } }

        public OverLoadTranslator(FullPath path, Translator parent)
            : base(path, parent)
        {
            _OverLoad = new List<Translator>();
        }

        public void AppendRoutine(string name, MethodInfo method = null)
        {
            //var temp = new RoutineTranslator(name, this, method);
            //_OverLoad.Add(temp);
        }

        public void AppendClass(string name, Type type = null)
        {
            //var temp = new ClassTranslator(name, this, type);
            //_OverLoad.Add(temp);
        }

        public Translator TypeResolution(params Translator[] trans)
        {
            return null;
        }
    }
}

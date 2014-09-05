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
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class RootStructure : ContainerStructure
    {
        [NonSerialized]
        private AssemblyBuilder Assembly;
        [NonSerialized]
        private ModuleBuilder Module;
        public string Name { get; private set; }
        public string FileName { get; private set; }
        public MethodStructure EntryContext { get; private set; }

        public RootStructure(string name, string dir = null)
        {
            Name = name;
            FileName = name + ".exe";
            Assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Name), AssemblyBuilderAccess.RunAndSave, dir);
            Module = Assembly.DefineDynamicModule(Name, FileName, true);
            var attr = MethodAttributes.PrivateScope | MethodAttributes.SpecialName | MethodAttributes.Static;
            var arg = new List<ParameterStructure>();
            var gnr = new List<GenericParameterStructure>();
            EntryContext = new MethodStructure();
            EntryContext.Initialize("@@entry", false, attr, gnr, arg, null);
            AppendChild(EntryContext);
        }

        internal void TraversalBuildCode()
        {
            TraversalPreBuild(this);
            ChildBuildCode(this);
            TraversalPostBuild(this);
        }

        internal void TraversalPreBuild(CilStructure stru)
        {
            var c = stru as BuilderStructure;
            if (c != null)
            {
                c.RelayPreBuild();
            }
            foreach (var v in stru)
            {
                TraversalPreBuild(v);
            }
        }

        internal void TraversalPostBuild(CilStructure stru)
        {
            foreach (var v in stru.Reverse())
            {
                TraversalPostBuild(v);
            }
            var c = stru as BuilderStructure;
            if(c != null)
            {
                c.RelayPostBuild();
            }
        }

        public void Save()
        {
            Assembly.Save(FileName);
        }

        public void Run()
        {
            //var entry = Assembly;
            //entry = Module.GetMethod(entry.Name);
            //entry.Invoke(null, null);
        }

        internal override void PostBuild()
        {
            Assembly.SetEntryPoint(EntryContext.GainMethod());
            Module.CreateGlobalFunctions();
        }

        internal override CodeGenerator GainGenerator()
        {
            return EntryContext.GainGenerator();
        }

        internal override TypeBuilder CreateType(string name, TypeAttributes attr)
        {
            return Module.DefineType(name, attr);
        }

        internal override MethodBuilder CreateMethod(string name, MethodAttributes attr)
        {
            return Module.DefineGlobalMethod(name, attr, typeof(void), Type.EmptyTypes);
        }
    }
}

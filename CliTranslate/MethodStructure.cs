﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class MethodStructure : MethodBaseStructure
    {
        public string Name { get; private set; }
        public IReadOnlyList<GenericParameterStructure> Generics { get; private set; }
        public TypeStructure ReturnType { get; private set; }
        public bool IsDefaultThisReturn { get; private set; }
        [NonSerialized]
        private MethodBuilder Builder;
        [NonSerialized]
        private MethodInfo Info;

        public void Initialize(string name, bool isInstance, MethodAttributes attr, IReadOnlyList<GenericParameterStructure> gnr, IReadOnlyList<ParameterStructure> arg, TypeStructure ret, BlockStructure block = null, bool isDtr = false, MethodInfo info = null)
        {
            Name = name;
            Generics = gnr;
            ReturnType = ret;
            IsDefaultThisReturn = isDtr;
            AppendChild(Generics);
            Info = info;
            base.Initialize(isInstance, attr, arg, block);
        }

        protected override void PreBuild()
        {
            if (Info != null)
            {
                return;
            }
            var cont = CurrentContainer;
            Builder = cont.CreateMethod(Name, Attributes);
            Info = Builder;
            if (Generics.Count > 0)
            {
                var gb = Builder.DefineGenericParameters(Generics.ToNames());
                Generics.RegisterBuilders(gb);
            }
            if (ReturnType != null)
            {
                Builder.SetReturnType(ReturnType.GainType()); //todo ジェネリクスに対応したTypeを生成する。
            }
            Builder.SetParameters(Arguments.ToTypes());
            Arguments.RegisterBuilders(Builder, IsInstance);
            SpreadGenerator();
        }

        public bool IsVoidReturn
        {
            get { return ReturnType == null || ReturnType.IsVoid; }
        }

        public bool IsVirtual
        {
            get { return Attributes.HasFlag(MethodAttributes.Virtual); }
        }

        internal override void PostBuild()
        {
            if(Generator == null)
            {
                return;
            }
            if (Block == null || !(Block.Last() is ReturnStructure))
            {
                if (IsDefaultThisReturn)
                {
                    Generator.GenerateCode(OpCodes.Ldarg_0);
                }
                Generator.GenerateCode(OpCodes.Ret);
            }
        }

        internal override void BuildCall(CodeGenerator cg)
        {
            cg.GenerateCall(this);
        }

        protected override ILGenerator GainILGenerator()
        {
            return Builder.GetILGenerator();
        }

        internal MethodInfo GainMethod()
        {
            return Info;
        }

        internal override BuilderStructure RenewInstance(TypeStructure type)
        {
            var ret = new MethodStructure();
            ret.Info = type.RenewMethod(this);
            return ret;
        }
    }
}

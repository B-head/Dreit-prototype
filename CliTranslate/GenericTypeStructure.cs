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
using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class GenericTypeStructure : TypeStructure
    {
        public TypeStructure BaseType { get; private set; }
        public IReadOnlyList<TypeStructure> GenericParameter { get; private set; }

        public void  Initialize(TypeStructure baseType, IReadOnlyList<TypeStructure> prms, Type info = null)
        {
            BaseType = baseType;
            GenericParameter = prms;
            Info = info;
        }

        internal override Type GainType()
        {
            if(Info != null)
            {
                return Info;
            }
            var m = BaseType as ModifyTypeStructure;
            if(m == null)
            {
                Info = BaseType.GainType().MakeGenericType(GenericParameter.GainTypes());
            }
            else
            {
                var ft = GenericParameter[0].GainType();
                switch(m.ModifyType)
                {
                    case ModifyType.Refer: Info = ft.MakeByRefType(); break;
                    case ModifyType.Typeof: break;
                    case ModifyType.Nullable: break;
                    case ModifyType.Pointer: Info = ft.MakePointerType(); break;
                    case ModifyType.EmbedArray: Info = ft.MakeArrayType(); break;
                    default: throw new InvalidOperationException();
                }
            }
            return Info;
        }
    }
}

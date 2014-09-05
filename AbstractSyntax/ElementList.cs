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

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class ElementList<ELEMENT> : Element, IReadOnlyList<ELEMENT> where ELEMENT : Element
    {
        public ElementList()
        {

        }

        public ElementList(TextPosition tp, IReadOnlyList<Element> child)
            :base(tp)
        {
            AppendChild(child);
        }

        public new ELEMENT this[int index]
        {
            get { return (ELEMENT)base[index]; }
        }

        public new IEnumerator<ELEMENT> GetEnumerator()
        {
            for (var i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

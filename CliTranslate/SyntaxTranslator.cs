using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Directive;
using AbstractSyntax.Expression;
using AbstractSyntax.Literal;
using AbstractSyntax.Pragma;
using AbstractSyntax.Statement;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    public class SyntaxTranslator
    {
        private Dictionary<Element, CilStructure> TransDictionary;

        private SyntaxTranslator()
        {
            TransDictionary = new Dictionary<Element, CilStructure>();
        }

        public static RootStructure ToStructure(Root root)
        {
            var trans = new SyntaxTranslator();
            return trans.RelayTranslate(root);
        }

        private void ChildTranslate(Element element)
        {
            foreach (var v in element)
            {
                RelayTranslate(v);
                ChildTranslate(v);
            }
        }

        private dynamic RelayTranslate(Element element)
        {
            if (TransDictionary.ContainsKey(element))
            {
                return TransDictionary[element];
            }
            var ret = Translate((dynamic)element);
            TransDictionary.Add(element, ret);
            return ret;
        }

        private void CollectChild(CilStructure parent, Element element)
        {
            foreach (var v in element)
            {
                var child = RelayTranslate(v);
                parent.AppendChild(child);
            }
        }

        private IReadOnlyList<T> CollectList<T>(IReadOnlyList<Element> elements) where T : CilStructure
        {
            var ret = new List<T>();
            foreach (var v in elements)
            {
                var child = (T)RelayTranslate(v);
                ret.Add(child);
            }
            return ret;
        }

        private CilStructure Translate(Element element)
        {
            return null;
        }

        private RootStructure Translate(Root element)
        {
            var ret = new RootStructure();
            CollectChild(ret, element);
            return ret;
        }

        private CilStructure Translate(VoidSymbol element)
        {
            return null; //todo void型のTypeStructureを生成する。
        }

        private GlobalContextStructure Translate(DeclateModule element)
        {
            var ret = new GlobalContextStructure();
            CollectChild(ret, element);
            return ret;
        }

        private TypeStructure Translate(DeclateClass element)
        {
            var attr = element.Attribute.MakeTypeAttributes(element.IsTrait);
            var gnr = CollectList<GenericTypeParameterStructure>(element.Generics);
            var bt = RelayTranslate(element.InheritClass);
            var imp = CollectList<TypeStructure>(element.InheritTraits);
            var ret = new TypeStructure(attr, gnr, bt, imp);
            CollectChild(ret, element);
            return ret;
        }

        private MethodBaseStructure Translate(DeclateRoutine element)
        {
            var attr = element.Attribute.MakeMethodAttributes(element.IsVirtual, element.IsAbstract);
            var gnr = CollectList<GenericTypeParameterStructure>(element.Generics);
            var arg = CollectList<ParameterStructure>(element.Arguments);
            var rt = RelayTranslate(element.ReturnType);
            MethodBaseStructure ret;
            if (element.IsConstructor)
            {
                ret = new ConstructorStructure(attr, gnr, arg, rt);
            }
            else
            {
                ret = new MethodStructure(attr, gnr, arg, rt);
            }
            CollectChild(ret, element);
            return ret;
        }

        private EnumStructure Translate(DeclateEnum element)
        {
            var ret = new EnumStructure();
            return ret;
        }

        private CilStructure Translate(DeclateVariant element)
        {
            var ret = new FieldStructure();
            return ret;
        }

        private ParameterStructure Translate(DeclateArgument element)
        {
            var ret = new ParameterStructure();
            return ret;
        }

        private GenericTypeParameterStructure Translate(DeclateGeneric element)
        {
            var ret = new GenericTypeParameterStructure();
            return ret;
        }
    }
}

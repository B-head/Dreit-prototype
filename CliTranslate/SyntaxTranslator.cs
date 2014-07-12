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
        private Dictionary<Element, CacheStructure> CacheDictionary;

        private SyntaxTranslator()
        {
            TransDictionary = new Dictionary<Element, CilStructure>();
            CacheDictionary = new Dictionary<Element, CacheStructure>();
        }

        public static RootStructure ToStructure(Root root, string name, string dir = null)
        {
            var trans = new SyntaxTranslator();
            var ret = new RootStructure(name, dir);
            trans.CollectChild(ret, root);
            ret.BuildCode();
            return ret;
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

        private CacheStructure GainCacheStructure(Element element)
        {
            if (CacheDictionary.ContainsKey(element))
            {
                return CacheDictionary[element];
            }
            var s = RelayTranslate(element);
            var c = new CacheStructure(s);
            CacheDictionary.Add(element, c);
            return c;
        }

        private CilStructure Translate(Element element)
        {
            return null;
        }

        private CilStructure Translate(VoidSymbol element)
        {
            return null; //todo void型のTypeStructureを生成する。
        }

        private GlobalContextStructure Translate(DeclateModule element)
        {
            var ret = new GlobalContextStructure(element.FullName);
            CollectChild(ret, element.Directives);
            return ret;
        }

        private TypeStructure Translate(DeclateClass element)
        {
            var attr = element.Attribute.MakeTypeAttributes(element.IsTrait);
            var gnr = CollectList<GenericTypeParameterStructure>(element.Generics);
            var bt = RelayTranslate(element.InheritClass);
            var imp = CollectList<TypeStructure>(element.InheritTraits);
            var ret = new TypeStructure(attr, gnr, bt, imp);
            CollectChild(ret, element.Block);
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
            CollectChild(ret, element.Block);
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

        private CilStructure Translate(AliasDirective element)
        {
            return null; //todo CILにエイリアスを反映させる方法を調査する。
        }

        private GotoStructure Translate(BreakDirective element)
        {
            var ret = new GotoStructure(null); //todo ループ出口のラベルを渡す。
            return ret;
        }

        private GotoStructure Translate(ContinueDirective element)
        {
            var ret = new GotoStructure(null); //todo ループ入口のラベルを渡す。
            return ret;
        }

        private CallStructure Translate(EchoDirective element)
        {
            var ret = new CallStructure(); //todo Console.WhiteLine関数を呼び出す。
            return ret;
        }

        private ReturnStructure Translate(ReturnDirective element)
        {
            var exp = RelayTranslate(element.Exp);
            var ret = new ReturnStructure(exp);
            return ret;
        }

        private DyadicOperationStructure Translate(Calculate element)
        {
            var left = RelayTranslate(element.Left);
            var right = RelayTranslate(element.Right);
            var call = RelayTranslate(element.CallScope);
            var ret = new DyadicOperationStructure(left, right, call);
            return ret;
        }

        private CallStructure Translate(CallRoutine element)
        {
            var call = RelayTranslate(element.CallScope);
            var access = RelayTranslate(element.Access) as AccessStructure;
            CallStructure ret;
            if (element.IsCalculate)
            {
                var left = RelayTranslate(element.Left);
                var right = RelayTranslate(element.Right);
                var calcall = RelayTranslate(element.CalculateCallScope);
                var cal = new DyadicOperationStructure(left, right, calcall);
                var args = new List<CilStructure>();
                args.Add(cal);
                ret = new CallStructure(call, access.Pre, args);
            }
            else
            {
                var args = CollectList<CilStructure>(element.Arguments);
                ret = new CallStructure(call, access.Pre, args);
            }
            return ret;
        }

        private DyadicOperationStructure Translate(Condition element)
        {
            CilStructure left;
            if(element.IsLeftConnection)
            {
                left = GainCacheStructure(element.Left);
            }
            else
            {
                left = RelayTranslate(element.Left);
            }
            CilStructure right;
            if(element.IsRightConnection)
            {
                right = GainCacheStructure(element.VirtualRight);
            }
            else
            {
                right = RelayTranslate(element.VirtualRight);
            }
            var call = RelayTranslate(element.CallScope);
            var ret = new DyadicOperationStructure(left, right, call);
            return ret;
        }

        private AccessStructure Translate(IdentifierAccess element)
        {
            var call = RelayTranslate(element.CallScope);
            var ret = new AccessStructure(call);
            return ret;
        }

        private CilStructure Translate(Logical element)
        {
            return null; //todo 分岐用のクラスを利用する。
        }

        private AccessStructure Translate(MemberAccess element)
        {
            var call = RelayTranslate(element.CallScope);
            var access = RelayTranslate(element.Access);
            var ret = new AccessStructure(call, access);
            return ret;
        }

        private CilStructure Translate(Postfix element)
        {
            return null; //todo 参照や型情報を返すようにする。
        }

        private CilStructure Translate(Prefix element)
        {
            return null;
        }

        private NumberStructure Translate(NumberLiteral element)
        {
            var ret = new NumberStructure(element.Parse());
            return ret;
        }

        private TextStructure Translate(PlainText element)
        {
            var ret = new TextStructure(element.Value);
            return ret;
        }

        private CilStructure Translate(StringLiteral element)
        {
            return null; //todo 文字列の生成をする。
        }

        private CilStructure Translate(IfStatement element)
        {
            return null;
        }

        private CilStructure Translate(LoopStatement element)
        {
            return null;
        }

        private CilStructure Translate(UnStatement element)
        {
            return null;
        }
    }
}

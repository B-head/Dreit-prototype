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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    public class SyntaxTranslator
    {
        private Dictionary<Element, CilStructure> TransDictionary;
        private Dictionary<Element, CacheStructure> CacheDictionary;
        private Dictionary<Element, object> ImportDictionary;

        private SyntaxTranslator()
        {
            TransDictionary = new Dictionary<Element, CilStructure>();
            CacheDictionary = new Dictionary<Element, CacheStructure>();
            ImportDictionary = new Dictionary<Element, object>();
        }

        public static RootStructure ToStructure(Root root, CilImport manager, string name, string dir = null)
        {
            var trans = new SyntaxTranslator();
            var ret = new RootStructure(name, dir);
            trans.CollectChild(root, ret, root);
            ret.TraversalBuildCode();
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
            if(element == null)
            {
                return null;
            }
            if (TransDictionary.ContainsKey(element))
            {
                return TransDictionary[element];
            }
            var ret = Translate((dynamic)element);
            if (!TransDictionary.ContainsKey(element))
            {
                TransDictionary.Add(element, ret);
            }
            return ret;
        }

        private void CollectChild(Element parent, CilStructure structure, Element element)
        {
            TransDictionary.Add(parent, structure);
            foreach (var v in element)
            {
                var child = RelayTranslate(v);
                structure.AppendChild(child);
            }
        }

        private IReadOnlyList<T> CollectList<T>(IReadOnlyList<Element> elements) where T : CilStructure
        {
            var ret = new List<T>();
            if(elements == null)
            {
                return ret;
            }
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
            var rt = RelayTranslate(element.ReturnType);
            var c = new CacheStructure(rt, s);
            CacheDictionary.Add(element, c);
            return c;
        }

        private CilStructure Translate(Element element)
        {
            return null;
        }

        private TypeStructure Translate(VoidSymbol element)
        {
            var gnr = new List<GenericParameterStructure>();
            var imp = new List<TypeStructure>();
            var ti = typeof(void);
            var ret = new TypeStructure(ti.Name, ti.Attributes, gnr, null, imp, ti);
            return ret;
        }

        private ParameterStructure Translate(ThisSymbol element)
        {
            return null; //todo 対応するParameterStructureを返す。
        }

        private GlobalContextStructure Translate(DeclateModule element)
        {
            var ret = new GlobalContextStructure(element.FullName);
            CollectChild(element, ret, element.Directives);
            return ret;
        }

        private TypeStructure Translate(DeclateClass element)
        {
            var attr = element.Attribute.MakeTypeAttributes(element.IsTrait);
            var gnr = CollectList<GenericParameterStructure>(element.Generics);
            var bt = RelayTranslate(element.InheritClass);
            var imp = CollectList<TypeStructure>(element.InheritTraits);
            var ret = new TypeStructure(element.FullName, attr, gnr, bt, imp);
            CollectChild(element, ret, element.Block);
            return ret;
        }

        private MethodBaseStructure Translate(DeclateRoutine element)
        {
            var attr = element.Attribute.MakeMethodAttributes(element.IsVirtual, element.IsAbstract);
            var gnr = CollectList<GenericParameterStructure>(element.Generics);
            var arg = CollectList<ParameterStructure>(element.Arguments);
            var rt = RelayTranslate(element.ReturnType);
            MethodBaseStructure ret;
            if (element.IsConstructor)
            {
                ret = new ConstructorStructure(attr, arg);
            }
            else
            {
                ret = new MethodStructure(element.Name, attr, gnr, arg, rt);
            }
            CollectChild(element, ret, element.Block);
            return ret;
        }

        private EnumStructure Translate(DeclateEnum element)
        {
            var ret = new EnumStructure();
            return ret;
        }

        private CilStructure Translate(DeclateVariant element)
        {
            if (element.IsField || element.IsGlobal)
            {
                var attr = element.Attribute.MakeFieldAttributes();
                var dt = RelayTranslate(element.CallReturnType);
                var ret = new FieldStructure(element.Name, attr, dt);
                return ret;
            }
            else
            {
                var dt = RelayTranslate(element.CallReturnType);
                var ret = new LocalStructure(element.Name, dt);
                return ret;
            }
        }

        private ParameterStructure Translate(DeclateArgument element)
        {
            var attr = ParameterAttributes.None;
            var pt = RelayTranslate(element.CallReturnType);
            var ret = new ParameterStructure(element.Name, attr, pt, null);
            return ret;
        }

        private GenericParameterStructure Translate(DeclateGeneric element)
        {
            var attr = GenericParameterAttributes.None;
            var ret = new GenericParameterStructure(element.Name, attr, null);
            return ret;
        }

        private CilStructure Translate(AliasDirective element)
        {
            return null; //todo CILにエイリアスを反映させる方法を調査する。
        }

        private GotoStructure Translate(BreakDirective element)
        {
            var loop = (LoopStructure)RelayTranslate(element.CurrentLoop());
            var rt = RelayTranslate(element.ReturnType);
            var ret = new GotoStructure(rt, loop.BreakLabel);
            return ret;
        }

        private GotoStructure Translate(ContinueDirective element)
        {
            var loop = (LoopStructure)RelayTranslate(element.CurrentLoop());
            var rt = RelayTranslate(element.ReturnType);
            var ret = new GotoStructure(rt, loop.ContinueLabel);
            return ret;
        }

        private BlockStructure Translate(DirectiveList element)
        {
            var exps = CollectList<ExpressionStructure>(element);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new BlockStructure(rt, exps);
            return ret;
        }

        private WriteLineStructure Translate(EchoDirective element)
        {
            var exp = RelayTranslate(element.Exp);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new WriteLineStructure(rt, exp);
            return ret;
        }

        private ReturnStructure Translate(ReturnDirective element)
        {
            var exp = RelayTranslate(element.Exp);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new ReturnStructure(rt, exp);
            return ret;
        }

        private DyadicOperationStructure Translate(Calculate element)
        {
            var left = RelayTranslate(element.Left);
            var right = RelayTranslate(element.Right);
            var call = RelayTranslate(element.CallScope);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new DyadicOperationStructure(rt, left, right, call);
            return ret;
        }

        private CallStructure Translate(CallRoutine element)
        {
            var call = RelayTranslate(element.CallScope);
            var access = RelayTranslate(element.Access) as AccessStructure;
            var pre = access == null ? null : access.Pre;
            CallStructure ret;
            if (element.IsCalculate)
            {
                var left = RelayTranslate(element.Left);
                var right = RelayTranslate(element.Right);
                var calcall = RelayTranslate(element.CalculateCallScope);
                var crt = RelayTranslate(element.CalculateCallScope.CallReturnType);
                var cal = new DyadicOperationStructure(crt, left, right, calcall);
                var args = new List<ExpressionStructure>();
                args.Add(cal);
                var rt = RelayTranslate(element.ReturnType);
                ret = new CallStructure(rt, call, pre, args);
            }
            else
            {
                var args = CollectList<ExpressionStructure>(element.Arguments);
                var rt = RelayTranslate(element.ReturnType);
                ret = new CallStructure(rt, call, pre, args);
            }
            return ret;
        }

        private DyadicOperationStructure Translate(Condition element)
        {
            ExpressionStructure left;
            if(element.IsLeftConnection)
            {
                left = GainCacheStructure(element.Left);
            }
            else
            {
                left = RelayTranslate(element.Left);
            }
            ExpressionStructure right;
            if(element.IsRightConnection)
            {
                right = GainCacheStructure(element.VirtualRight);
            }
            else
            {
                right = RelayTranslate(element.VirtualRight);
            }
            var call = RelayTranslate(element.CallScope);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new DyadicOperationStructure(rt, left, right, call);
            return ret;
        }

        private AccessStructure Translate(IdentifierAccess element)
        {
            var call = RelayTranslate(element.CallScope);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new AccessStructure(rt, call);
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

        private ValueStructure Translate(NumberLiteral element)
        {
            var rt = RelayTranslate(element.ReturnType);
            var ret = new ValueStructure(rt, element.Parse());
            return ret;
        }

        private ValueStructure Translate(PlainText element)
        {
            var rt = RelayTranslate(element.ReturnType);
            var ret = new ValueStructure(rt, element.Value);
            return ret;
        }

        private StringStructure Translate(StringLiteral element)
        {
            var exps = CollectList<ExpressionStructure>(element.Texts);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new StringStructure(rt, exps);
            return ret;
        }

        private BranchStructure Translate(IfStatement element)
        {
            var cond = RelayTranslate(element.Condition);
            var then = RelayTranslate(element.Then);
            var els = RelayTranslate(element.Else);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new BranchStructure(rt, cond, then, els);
            return ret;
        }

        private LoopStructure Translate(LoopStatement element)
        {
            var cond = RelayTranslate(element.Condition);
            var on = RelayTranslate(element.On);
            var by = RelayTranslate(element.By);
            var block = RelayTranslate(element.Block);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new LoopStructure(rt, cond, on, by, block);
            return ret;
        }

        private MonadicOperationStructure Translate(UnStatement element)
        {
            return null; //todo 論理否定の関数を用意する。
        }
    }
}

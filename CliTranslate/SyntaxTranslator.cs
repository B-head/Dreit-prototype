using AbstractSyntax;
using AbstractSyntax.Declaration;
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

        public static RootStructure ToStructure(Root root, CilImport import, string name, string dir = null)
        {
            var trans = new SyntaxTranslator();
            trans.PrepareImport(import);
            var ret = new RootStructure(name, dir);
            foreach (var v in root)
            {
                var child = trans.RelayTranslate(v);
                ret.AppendChild(child);
            }
            ret.TraversalBuildCode();
            return ret;
        }

        private void PrepareImport(CilImport import)
        {
            foreach(var v in import.ImportDictionary)
            {
                ImportDictionary.Add(v.Value, v.Key);
            }
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
            dynamic ret;
            if (ImportDictionary.ContainsKey(element))
            {
                ret = Translate((dynamic)element, (dynamic)ImportDictionary[element]);
            }
            else
            {
                ret = Translate((dynamic)element);
            }
            if (!TransDictionary.ContainsKey(element))
            {
                TransDictionary.Add(element, ret);
            }
            return ret;
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

        private CacheStructure MakeCacheStructure(Element element)
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

        private PureTypeStructure Translate(VoidSymbol element)
        {
            var gnr = new List<GenericParameterStructure>();
            var imp = new List<TypeStructure>();
            var ti = typeof(void);
            var ret = new PureTypeStructure();
            ret.Initialize(ti.Name, ti.Attributes, gnr, null, imp, null, ti);
            return ret;
        }

        private ValueStructure Translate(BooleanSymbol element)
        {
            var rt = RelayTranslate(element.ReturnType);
            var ret = new ValueStructure(rt, element.Value);
            return ret;
        }

        private ConstructorStructure Translate(DefaultSymbol element)
        {
            var ret = new ConstructorStructure();
            ret.InitializeDefault();
            return ret;
        }

        private ParameterStructure Translate(ThisSymbol element)
        {
            var dt = RelayTranslate(element.CallReturnType);
            var ret = new ParameterStructure(dt); 
            return ret;
        }

        private LoadStoreStructure Translate(PropertyPragma element)
        {
            var variant = RelayTranslate(element.Variant);
            var ret = new LoadStoreStructure(variant, element.IsSet);
            return ret;
        }

        private CastStructure Translate(CastPragma element)
        {
            var ret = new CastStructure(element.PrimitiveType);
            return ret;
        }

        private OperationStructure Translate(MonadicOperatorPragma element)
        {
            var ret = new OperationStructure(element.CalculateType);
            return ret;
        }

        private OperationStructure Translate(DyadicOperatorPragma element)
        {
            var ret = new OperationStructure(element.CalculateType);
            return ret;
        }

        private GlobalContextStructure Translate(ModuleDeclaration element)
        {
            var block = RelayTranslate(element.Directives);
            var ret = new GlobalContextStructure(element.FullName, block);
            return ret;
        }

        private PureTypeStructure Translate(ClassSymbol element, Type info = null)
        {
            var ret = new PureTypeStructure();
            TransDictionary.Add(element, ret);
            var attr = element.Attribute.MakeTypeAttributes(element.IsTrait);
            var gnr = CollectList<GenericParameterStructure>(element.Generics);
            var bt = RelayTranslate(element.InheritClass);
            var imp = CollectList<TypeStructure>(element.InheritTraits);
            var block = RelayTranslate(element.Block);
            ret.Initialize(element.FullName, attr, gnr, bt, imp, block, info);
            if (element.IsDefaultConstructor)
            {
                var def = RelayTranslate(element.Default);
                ret.RegisterDefault(def);
            }
            return ret;
        }

        private MethodBaseStructure Translate(RoutineSymbol element, MethodBase info = null)
        {
            if (element.IsConstructor)
            {
                var ret = new ConstructorStructure();
                TransDictionary.Add(element, ret);
                var attr = element.Attribute.MakeMethodAttributes(false, false);
                var gnr = CollectList<GenericParameterStructure>(element.Generics);
                var arg = CollectList<ParameterStructure>(element.Arguments);
                var block = RelayTranslate(element.Block);
                ret.Initialize(attr, arg, block, (ConstructorInfo)info);
                var super = RelayTranslate(element.InheritInitializer);
                ret.RegisterSuperConstructor(super);
                return ret;
            }
            else
            {
                var ret = new MethodStructure();
                TransDictionary.Add(element, ret);
                var attr = element.Attribute.MakeMethodAttributes(element.IsVirtual, element.IsAbstract);
                var gnr = CollectList<GenericParameterStructure>(element.Generics);
                var arg = CollectList<ParameterStructure>(element.Arguments);
                var block = RelayTranslate(element.Block);
                var crt = RelayTranslate(element.CallReturnType);
                ret.Initialize(element.Name, element.IsInstanceMember, attr, gnr, arg, crt, block, (MethodInfo)info);
                return ret;
            }
        }

        private EnumStructure Translate(EnumSymbol element, Type info = null)
        {
            var ret = new EnumStructure();
            return ret;
        }

        private CilStructure Translate(VariantSymbol element, FieldInfo info = null)
        {
            var attr = element.Attribute.MakeFieldAttributes();
            var dt = RelayTranslate(element.CallReturnType);
            if (element.IsField || element.IsGlobal)
            {
                var ret = new FieldStructure(element.Name, attr, dt, info);
                return ret;
            }
            else
            {
                var ret = new LocalStructure(element.Name, dt);
                return ret;
            }
        }

        private ParameterStructure Translate(ArgumentSymbol element, ParameterInfo info = null)
        {
            var attr = ParameterAttributes.None;
            var pt = RelayTranslate(element.CallReturnType);
            var ret = new ParameterStructure(element.Name, attr, pt, null);
            return ret;
        }

        private GenericParameterStructure Translate(GenericSymbol element, Type info = null)
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
            var exps = CollectList<CilStructure>(element);
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

        private CallStructure Translate(CallRoutine element)
        {
            var call = RelayTranslate(element.CallScope);
            var access = RelayTranslate(element.Access);
            var pre = access is CallStructure ? access.Pre: null;
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
                ret = new CallStructure(rt, call, pre, null, args);
            }
            else
            {
                var args = CollectList<ExpressionStructure>(element.Arguments);
                var rt = RelayTranslate(element.ReturnType);
                ret = new CallStructure(rt, call, pre, access, args);
            }
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

        private DyadicOperationStructure Translate(Compare element)
        {
            ExpressionStructure left;
            if(element.IsLeftConnection)
            {
                left = MakeCacheStructure(element.Left);
            }
            else
            {
                left = RelayTranslate(element.Left);
            }
            ExpressionStructure right;
            ExpressionStructure next = null;
            if(element.IsRightConnection)
            {
                right = MakeCacheStructure(element.VirtualRight);
                next = RelayTranslate(element.Right);
            }
            else
            {
                right = RelayTranslate(element.VirtualRight);
            }
            var call = RelayTranslate(element.CallScope);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new DyadicOperationStructure(rt, left, right, call, next);
            return ret;
        }

        private LogicalStructure Translate(Logical element)
        {
            var left = RelayTranslate(element.Left);
            var right = RelayTranslate(element.Right);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new LogicalStructure(rt, left, right, element.IsOr);
            return ret;
        }

        private CallStructure Translate(Identifier element)
        {
            var call = RelayTranslate(element.CallScope);
            var rt = RelayTranslate(element.ReturnType);
            if (element.IsTacitThis)
            {
                var thiscall = RelayTranslate(element.ThisReference.Getter);
                var thisrt = RelayTranslate(element.ThisReference.CallReturnType);
                var pre = new CallStructure(thisrt, thiscall, null);
                var ret = new CallStructure(rt, call, pre);
                return ret;
            }
            else
            {
                var ret = new CallStructure(rt, call, null);
                return ret;
            }
        }

        private CallStructure Translate(MemberAccess element)
        {
            var call = RelayTranslate(element.CallScope);
            var pre = RelayTranslate(element.Access);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new CallStructure(rt, call, pre);
            return ret;
        }

        private CilStructure Translate(Postfix element)
        {
            return null; //todo 参照や型情報を返すようにする。
        }

        private MonadicOperationStructure Translate(Prefix element)
        {
            var exp = RelayTranslate(element.Exp);
            var call = RelayTranslate(element.CallScope);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new MonadicOperationStructure(rt, exp, call);
            return ret;
        }

        private ValueStructure Translate(NumericLiteral element)
        {
            var rt = RelayTranslate(element.ReturnType);
            var ret = new ValueStructure(rt, element.Parse());
            return ret;
        }

        private ValueStructure Translate(PlainText element)
        {
            var rt = RelayTranslate(element.ReturnType);
            var ret = new ValueStructure(rt, element.ShowValue);
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
            var rt = RelayTranslate(element.ReturnType);
            var ret = new LoopStructure(rt);
            TransDictionary.Add(element, ret);
            var cond = RelayTranslate(element.Condition);
            var on = RelayTranslate(element.On);
            var by = RelayTranslate(element.By);
            var block = RelayTranslate(element.Block);
            ret.Initialize(cond, on, by, block);
            return ret;
        }

        private MonadicOperationStructure Translate(UnStatement element)
        {
            var exp = RelayTranslate(element.Exp);
            var call = RelayTranslate(element.CallScope);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new MonadicOperationStructure(rt, exp, call);
            return ret;
        }
    }
}

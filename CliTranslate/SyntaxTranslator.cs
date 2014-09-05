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
using AbstractSyntax;
using AbstractSyntax.Declaration;
using AbstractSyntax.Expression;
using AbstractSyntax.Literal;
using AbstractSyntax.SpecialSymbol;
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
                ImportDictionary[v.Value] = v.Key;
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

        private CilStructure Translate(ErrorRoutineSymbol element)
        {
            throw new ArgumentException("error routine");
        }

        private CilStructure Translate(ErrorTypeSymbol element)
        {
            throw new ArgumentException("error type");
        }

        private CilStructure Translate(ErrorVariantSymbol element)
        {
            return null;
        }

        private CilStructure Translate(UnknownSymbol element)
        {
            throw new ArgumentException("unknown");
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
            if (element.IsConstructor)
            {
                var ret = new ConstructorStructure();
                ret.InitializeDefault();
                var super = RelayTranslate(element.InheritInitializer);
                ret.RegisterSuperConstructor(super);
                return ret;
            }
            else
            {
                return null;
            }
        }

        private ParameterStructure Translate(ThisSymbol element)
        {
            var dt = RelayTranslate(element.DataType);
            var ret = new ParameterStructure(dt); 
            return ret;
        }

        private LoadStoreStructure Translate(PropertySymbol element)
        {
            var ret = new LoadStoreStructure(element.IsSet);
            return ret;
        }

        private CastStructure Translate(CastSymbol element)
        {
            var ret = new CastStructure(element.PrimitiveType);
            return ret;
        }

        private OperationStructure Translate(MonadicOperatorSymbol element)
        {
            var ret = new OperationStructure(element.CalculateType);
            return ret;
        }

        private OperationStructure Translate(DyadicOperatorSymbol element)
        {
            var ret = new OperationStructure(element.CalculateType);
            return ret;
        }

        private ModifyTypeStructure Translate(ModifyTypeSymbol element)
        {
            var ret = new ModifyTypeStructure(element.ModifyType);
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
            return ret;
        }

        private GenericTypeStructure Translate(ClassTemplateInstance element, Type info = null)
        {
            var ret = new GenericTypeStructure();
            TransDictionary.Add(element, ret);
            var bt = RelayTranslate(element.Type);
            var gnr = CollectList<TypeStructure>(element.Parameters);
            ret.Initialize(bt, gnr);
            return ret;
        }

        private CilStructure Translate(RoutineTemplateInstance element, MethodBase info = null)
        {
            var ret = new GenericMethodStructure();
            TransDictionary.Add(element, ret);
            var br = RelayTranslate(element.Routine);
            var gnr = CollectList<TypeStructure>(element.Parameters);
            var di = RelayTranslate(element.DeclaringInstance);
            ret.Initialize(br, gnr, di);
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
            else if(element.IsDestructor)
            {
                var ret = new MethodStructure();
                TransDictionary.Add(element, ret);
                var attr = MethodAttributes.Family | MethodAttributes.RTSpecialName | MethodAttributes.Virtual;
                var gnr = new List<GenericParameterStructure>();
                var arg = new List<ParameterStructure>();
                var block = RelayTranslate(element.Block);
                var crt = RelayTranslate(element.CallReturnType);
                ret.Initialize("Finalize", element.IsInstanceMember, attr, gnr, arg, crt, block, element.IsDefaultThisReturn, (MethodInfo)info);
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
                ret.Initialize(element.Name, element.IsInstanceMember, attr, gnr, arg, crt, block, element.IsDefaultThisReturn, (MethodInfo)info);
                return ret;
            }
        }

        private EnumStructure Translate(EnumSymbol element, Type info = null)
        {
            var ret = new EnumStructure();
            TransDictionary.Add(element, ret);
            var attr = element.Attribute.MakeTypeAttributes(false);
            var block = RelayTranslate(element.Block);
            ret.Initialize(element.FullName, attr, block, info);
            return ret;
        }

        private CilStructure Translate(VariantSymbol element, FieldInfo info = null)
        {
            var attr = element.Attribute.MakeFieldAttributes(element.IsDefinedConstantValue);
            var dt = RelayTranslate(element.DataType);
            if (element.IsClassField || element.IsGlobal)
            {
                var def = element.IsClassField ? element.GenerateConstantValue() : null;
                var ret = new FieldStructure(element.Name, attr, dt, def, false, info);
                return ret;
            }
            else if (element.IsEnumField)
            {
                var def = element.GenerateConstantValue();
                var ret = new FieldStructure(element.Name, attr, dt, def, true, info);
                return ret;
            }
            else if (element.IsLoopVariant)
            {
                var def = RelayTranslate(element.DefaultValue);
                var ret = new LoopLocalStructure(element.Name, dt, def);
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
            var pt = RelayTranslate(element.DataType);
            //todo 無限再帰に対処する。
            //var def = RelayTranslate(element.DefaultValue);
            var ret = new ParameterStructure(element.Name, attr, pt, null);
            return ret;
        }

        private GenericParameterStructure Translate(GenericSymbol element, Type info = null)
        {
            var attr = GenericParameterAttributes.None;
            var ret = new GenericParameterStructure(element.Name, attr, null);
            return ret;
        }

        private CilStructure Translate(AliasDeclaration element)
        {
            return null; //todo CILにエイリアスを反映させる方法を調査する。
        }

        private GotoStructure Translate(BreakStatement element)
        {
            var loop = (LoopStructure)RelayTranslate(element.CurrentLoop());
            var rt = RelayTranslate(element.ReturnType);
            var ret = new GotoStructure(rt, loop.BreakLabel);
            return ret;
        }

        private GotoStructure Translate(ContinueStatement element)
        {
            var loop = (LoopStructure)RelayTranslate(element.CurrentLoop());
            var rt = RelayTranslate(element.ReturnType);
            var ret = new GotoStructure(rt, loop.ContinueLabel);
            return ret;
        }

        private BlockStructure Translate(ProgramContext element)
        {
            var exps = CollectList<CilStructure>(element);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new BlockStructure(rt, exps, element.IsInline);
            return ret;
        }

        private ReturnStructure Translate(ReturnStatement element)
        {
            var exp = RelayTranslate(element.Exp);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new ReturnStructure(rt, exp);
            return ret;
        }

        private CallStructure Translate(CallExpression element)
        {
            var call = RelayTranslate(element.CallRoutine);
            var access = RelayTranslate(element.Access);
            dynamic pre;
            if (element.IsReferVeriant && !element.IsStoreCall)
            {
                pre = access;
            }
            else
            {
                pre = access is CallStructure ? access.Pre : null;
            }
            var variant = RelayTranslate(element.ReferVariant);
            var isVariadic = element.CallRoutine.HasVariadicArguments();
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
                var convs = new List<BuilderStructure>();
                convs.Add(null);
                var rt = RelayTranslate(element.ReturnType);
                ret = new CallStructure(rt, call, pre, null, variant, args, convs, isVariadic);
            }
            else
            {
                var args = CollectList<ExpressionStructure>(element.VirtualArgument);
                var convs = CollectList<BuilderStructure>(element.CallConverter);
                var rt = RelayTranslate(element.ReturnType);
                if (element.IsConnectPipeline)
                {
                    ret = new CallStructure(null, null, pre, access, variant, null, null, isVariadic);
                }
                else
                {
                    ret = new CallStructure(rt, call, pre, access, variant, args, convs, isVariadic);
                }
            }
            return ret;
        }

        private DyadicOperationStructure Translate(Calculate element)
        {
            var left = RelayTranslate(element.Left);
            var right = RelayTranslate(element.Right);
            var call = RelayTranslate(element.CallRoutine);
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
            var call = RelayTranslate(element.CallRoutine);
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
            var call = RelayTranslate(element.CallRoutine);
            var variant = RelayTranslate(element.ReferVariant);
            var rt = RelayTranslate(element.ReturnType);
            if (element.IsTacitThis)
            {
                var thiscall = RelayTranslate(element.ThisCallRoutine);
                var thisrt = RelayTranslate(element.ThisReference.DataType);
                var thisvar = RelayTranslate(element.ThisReference);
                var pre = new CallStructure(thisrt, thiscall, null, thisvar);
                var ret = new CallStructure(rt, call, pre, variant);
                return ret;
            }
            else
            {
                var ret = new CallStructure(rt, call, null, variant);
                return ret;
            }
        }

        private CallStructure Translate(TemplateInstanceExpression element)
        {
            var call = RelayTranslate(element.CallRoutine);
            var variant = RelayTranslate(element.ReferVariant);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new CallStructure(rt, call, null, variant);
            return ret;
        }

        private CallStructure Translate(MemberAccess element)
        {
            var call = RelayTranslate(element.CallRoutine);
            var pre = RelayTranslate(element.Access);
            var variant = RelayTranslate(element.ReferVariant);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new CallStructure(rt, call, pre, variant);
            return ret;
        }

        private CilStructure Translate(Postfix element)
        {
            return null; //todo 参照や型情報を返すようにする。
        }

        private MonadicOperationStructure Translate(Prefix element)
        {
            var exp = RelayTranslate(element.Exp);
            var call = RelayTranslate(element.CallRoutine);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new MonadicOperationStructure(rt, exp, call);
            return ret;
        }

        private CilStructure Translate(GroupingExpression element)
        {
            return RelayTranslate(element.Exp);
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

        private ArrayStructure Translate(ArrayLiteral element)
        {
            var exps = CollectList<ExpressionStructure>(element.Values);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new ArrayStructure(rt, exps);
            return ret;
        }

        private IfStructure Translate(IfStatement element)
        {
            var cond = RelayTranslate(element.Condition);
            var then = RelayTranslate(element.Then);
            var els = RelayTranslate(element.Else);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new IfStructure(rt, cond, then, els);
            return ret;
        }

        private LoopStructure Translate(LoopStatement element)
        {
            var rt = RelayTranslate(element.ReturnType);
            var ret = new LoopStructure(rt);
            TransDictionary.Add(element, ret);
            var cond = RelayTranslate(element.Condition);
            var use = RelayTranslate(element.Use);
            var by = RelayTranslate(element.By);
            var block = RelayTranslate(element.Block);
            ret.Initialize(cond, use, by, block);
            return ret;
        }

        private MonadicOperationStructure Translate(UnStatement element)
        {
            var exp = RelayTranslate(element.Exp);
            var call = RelayTranslate(element.CallRoutine);
            var rt = RelayTranslate(element.ReturnType);
            var ret = new MonadicOperationStructure(rt, exp, call);
            return ret;
        }
    }
}

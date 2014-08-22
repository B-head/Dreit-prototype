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
using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Declaration
{
    [Serializable]
    public class ModuleDeclaration : NameSpaceSymbol
    {
        public ProgramContext Directives { get; private set; }
        public string SourceText { get; private set; }
        public IReadOnlyList<Token> ErrorToken { get; private set; }

        public ModuleDeclaration(TextPosition tp, ProgramContext drcs, string name, string source, IReadOnlyList<Token> error)
            :base(tp)
        {
            Directives = drcs;
            Name = name;
            SourceText = source;
            ErrorToken = error;
            AppendChild(Directives);
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            foreach (Token v in ErrorToken)
            {
                if (v.TokenType == TokenType.OtherString)
                {
                    cmm.CompileError("invalid-token", v);
                }
                else
                {
                    cmm.CompileError("error-token", v);
                }
            }
        }
    }
}

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

namespace AbstractSyntax
{
    [Serializable]
    public struct Token
    {
        public TokenType TokenType { get; set; }
        public TextPosition Position { get; set; }
        public string Text { get; set; }

        public static readonly Token Empty = new Token { Text = string.Empty };

        public override string ToString()
        {
            return string.Format("{0}: {1} => {2}", Position, TokenType, Text.Replace('\x0A', '\x20').Replace('\x0D', '\x20'));
        }

        public static implicit operator bool(Token token)
        {
            return !string.IsNullOrEmpty(token.Text);
        }
    }

    [Flags]
    public enum TokenType
    {
        Unknoun,
        LineTerminator,
        WhiteSpace,
        BlockComment,
        LineCommnet,
        QuoteSeparator,
        EfficientQuoteSeparator,
        PlainText,
        LetterStartString,
        DigitStartString,
        OtherString,

        EndExpression,
        ReturnArrow,
        Pair,
        Separator,
        List,
        Access,
        Range,
        Zone,
        Attribute,
        Pragma,
        Macro,
        Lambda,
        Reject,
        Nullable,
        Or,
        And,
        Not,
        Plus,
        Minus,
        Template,
        Typeof,
        Refer,
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Incomparable,
        Combine,
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        LeftCompose,
        RightCompose,
        LeftParenthesis,
        RightParenthesis,
        LeftBracket,
        RightBracket,
        LeftBrace,
        RightBrace,
        Swap,
        LeftPipeline = 0x10000,
        RightPipeline = 0x20000,
    }
}

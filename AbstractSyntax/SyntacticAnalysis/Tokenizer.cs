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

namespace AbstractSyntax.SyntacticAnalysis
{
    class Tokenizer
    {
        public string Text { get; private set; }
        private TextPosition _Position;
        public TextPosition Position { get { return _Position; } }

        public Tokenizer(string text, string fileName)
        {
            Text = text;
            _Position = new TextPosition { File = fileName, Line = 1 };
        }

        public bool IsReadable(int index = 0)
        {
            return _Position.Total + index < Text.Length;
        }

        public char Read(int index = 0)
        {
            return Text[_Position.Total + index];
        }

        public string Read(int index, int length)
        {
            int start = _Position.Total + index;
            if (start + length <= Text.Length)
            {
                return Text.Substring(start, length);
            }
            else
            {
                return string.Empty;
            }
        }

        public bool MatchAny(int index, string list)
        {
            var c = Read(index);
            foreach (var v in list)
            {
                if (v == c)
                {
                    return true;
                }
            }
            return false;
        }

        public bool MatchRange(int index, char start, char end)
        {
            var c = Read(index);
            return start <= c && c <= end;
        }

        public Token TakeToken(int length, TokenType type)
        {
            if (length == 0)
            {
                return Token.Empty;
            }
            string text = Read(0, length);
            Token token = new Token { Text = text, TokenType = type, Position = _Position.AlterLength(length) };
            _Position.AddCount(length);
            if (type == TokenType.LineTerminator)
            {
                _Position.LineTerminate();
            }
            return token;
        }
    }
}

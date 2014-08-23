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
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AbstractSyntax;

namespace Dlight
{
    public static class CompileMessageBuilder
    {
        private static Dictionary<string, string> messageBase;
        private static readonly XNamespace ns = "CompileMessageSchema.xsd";

        static CompileMessageBuilder()
        {
            string directory;
            var assembly = Assembly.GetEntryAssembly();
            if(assembly == null)
            {
                directory = Directory.GetCurrentDirectory();
            }
            else
            {
                directory = Path.GetDirectoryName(assembly.Location);
            }
            Console.WriteLine(directory);
            messageBase = new Dictionary<string, string>();
            foreach (var file in Directory.EnumerateFiles(directory, "*.xml"))
            {
                var element = XElement.Load(file);
                if(element.Name != ns + "compile-message")
                {
                    continue;
                }
                AppendBase(element);
            }
        }

        private static void AppendBase(XElement element)
        {
            foreach(var v in element.Descendants(ns + "msg"))
            {
                var key = (string)v.Attribute("key");
                var msg = (string)v;
                messageBase.Add(key, msg);
            }
        }

        public static string Build(CompileMessageManager manager)
        {
            var builder = new StringBuilder();
            foreach(var v in manager)
            {
                builder.AppendLine(Build(v));
            }
            builder.Append(manager);
            return builder.ToString();
        }

        public static string Build(CompileMessage message)
        {
            var builder = new StringBuilder();
            builder.Append(message.StringPrefix).Append(": ");
            builder.Append(message.Position).Append(": ");
            var msg = messageBase[message.Key];
            var current = 0;
            var match = Regex.Match(msg, @"\{.*?\}");
            while(match.Success)
            {
                builder.Append(msg.Substring(current, match.Index - current));
                current = match.Index + match.Length;
                builder.Append(GetValue(match.Value.Trim('{', '}').Trim(), message.Target));
                match = match.NextMatch();
            }
            builder.Append(msg.Substring(current, msg.Length - current));
            return builder.ToString();
        }

        private static string GetValue(string exp, object target)
        {
            object current = target;
            const BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            foreach(var s in exp.Split('.'))
            {
                var type = current.GetType();
                var prop = type.GetProperty(s, bf);
                if(prop != null && prop.CanRead)
                {
                    current = prop.GetValue(current);
                    continue;
                }
                var field = type.GetField(s, bf);
                if(field != null)
                {
                    current = field.GetValue(current);
                    continue;
                }
                throw new CompileMessageBuildExcepsion(exp, target);
            }
            return current.ToString();
        }
    }

    [Serializable]
    public class CompileMessageBuildExcepsion : Exception
    {
        public string Exp { get; set; }
        public object Target { get; set; }

        public CompileMessageBuildExcepsion(string exp, object target)
            : base(BuildMessage(exp, target))
        {
            Exp = exp;
            Target = target;
        }

        private static string BuildMessage(string exp, object target)
        {
            var builder = new StringBuilder();
            object current = target;
            const BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            foreach (var s in exp.Split('.'))
            {
                var type = current.GetType();
                if (target == current)
                {
                    builder.Append(type.Name);
                }
                else
                {
                    builder.Append("[").Append(type.Name).Append("]");
                }
                builder.Append(".").Append(s);
                var prop = type.GetProperty(s, bf);
                if (prop != null && prop.CanRead)
                {
                    current = prop.GetValue(current);
                    continue;
                }
                var field = type.GetField(s, bf);
                if (field != null)
                {
                    current = field.GetValue(current);
                    continue;
                }
                builder.Append(" is not found");
                return builder.ToString();
            }
            throw new ArgumentException();
        }
    }
}

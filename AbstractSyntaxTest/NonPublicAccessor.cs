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
using System.Reflection;

namespace AbstractSyntaxTest
{
    public static class NonPublicAccessor
    {
        public static object Invoke(this Type type, string name, params object[] args)
        {
            var method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.OptionalParamBinding);
            return method.Invoke(null, args);
        }

        public static object Invoke(this object obj, string name, params object[] args)
        {
            var method = obj.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.OptionalParamBinding);
            return method.Invoke(obj, args);
        }
    }
}

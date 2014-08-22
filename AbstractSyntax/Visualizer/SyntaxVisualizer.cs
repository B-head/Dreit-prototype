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
using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace AbstractSyntax.Visualizer
{
    public interface IReadOnlyTree<out T> : IReadOnlyList<T> where T : IReadOnlyTree<T>
    {
        T Root { get; }
        T Parent { get; }
    }

    public class SyntaxVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null) throw new ArgumentNullException("windowService");
            if (objectProvider == null) throw new ArgumentNullException("objectProvider");

            using (var displayForm = new SyntaxVisualizerForm(objectProvider))
            {
                windowService.ShowDialog(displayForm);
            }
        }

        public static void TestShowVisualizer(object visualize)
        {
            var visualizerHost = new VisualizerDevelopmentHost(visualize, typeof(SyntaxVisualizer), typeof(SyntaxVisualizerSource), false);
            visualizerHost.ShowVisualizer();
        }
    }
}

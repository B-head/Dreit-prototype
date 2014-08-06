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

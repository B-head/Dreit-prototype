using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
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

            var data = objectProvider.GetObject();
            using (var displayForm = new SyntaxVisualizerForm(data))
            {
                windowService.ShowDialog(displayForm);
            }
        }

        public static void TestShowVisualizer(object visualize)
        {
            var visualizerHost = new VisualizerDevelopmentHost(visualize, typeof(SyntaxVisualizer));
            visualizerHost.ShowVisualizer();
        }
    }
}

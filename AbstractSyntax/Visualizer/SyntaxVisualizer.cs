using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Windows.Forms;

namespace AbstractSyntax.Visualizer
{
    public class SyntaxVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null) throw new ArgumentNullException("windowService");
            if (objectProvider == null) throw new ArgumentNullException("objectProvider");

            Element data = (Element)objectProvider.GetObject();
            using (var displayForm = new SyntaxVisualizerForm(data))
            {
                windowService.ShowDialog(displayForm);
            }
        }

        public static void TestShowVisualizer(object objectToVisualize)
        {
            VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(SyntaxVisualizer));
            visualizerHost.ShowVisualizer();
        }
    }
}

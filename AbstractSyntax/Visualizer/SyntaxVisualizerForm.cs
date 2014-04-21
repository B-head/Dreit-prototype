using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstractSyntax.Visualizer
{
    public partial class SyntaxVisualizerForm : Form
    {
        private Root root;
        private Element target;

        public SyntaxVisualizerForm(Element target)
        {
            InitializeComponent();
            root = target.Root;
            this.target = target;
        }

        private void LoadHandler(object sender, EventArgs e)
        {
            Text = target.ToString();
            AddTree(syntaxTree.Nodes, root);
            var node = GetTreeNode(target);
            node.EnsureVisible();
            node.Expand();
            syntaxTree.SelectedNode = node;
        }

        private void AddTree(TreeNodeCollection nodes, Element element)
        {
            if (element == null)
            {
                var node = new TreeNode("<null>");
                nodes.Add(node);
            }
            else
            {
                var node = new TreeNode(element.ToString());
                node.Tag = element;
                nodes.Add(node);
                foreach (var v in element)
                {
                    AddTree(node.Nodes, v);
                }
            }
        }

        private TreeNode GetTreeNode(Element element)
        {
            TreeNodeCollection nodes;
            if (element is Root)
            {
                nodes = syntaxTree.Nodes;
            }
            else
            {
                var node = GetTreeNode(element.Parent);
                nodes = node.Nodes;
            }
            foreach(TreeNode v in nodes)
            {
                if(v.Tag == element)
                {
                    return v;
                }
            }
            throw new ArgumentException();
        }
    }
}

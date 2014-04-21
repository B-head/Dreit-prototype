using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstractSyntax.Visualizer
{
    public partial class SyntaxVisualizerForm : Form
    {
        private Root root;
        private Element target;
        private const BindingFlags showMenber = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public SyntaxVisualizerForm(Element target)
        {
            InitializeComponent();
            root = target.Root;
            this.target = target;
        }

        private void LoadHandler(object sender, EventArgs e)
        {
            AddTree(syntaxTree.Nodes, root);
            SelectElement(target);
        }

        private void AddSelectHandler(object sender, TreeViewEventArgs e)
        {
            var node = syntaxTree.SelectedNode;
            Text = node.Text;
            ShowValue((Element)node.Tag);
        }

        private void ItemActivateHandler(object sender, EventArgs e)
        {
            var item = valueList.SelectedItems[0];
            var element = item.Tag as Element;
            if(element == null)
            {
                return;
            }
            SelectElement(element);
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

        private void ShowValue(Element element)
        {
            valueList.BeginUpdate();
            valueList.Items.Clear();
            var type = element.GetType();
            foreach(var v in type.GetFields(showMenber))
            {
                object obj = v.GetValue(element) ?? "<null>";
                var texts = new string[]{ v.Name, obj.ToString() };
                var item = new ListViewItem(texts);
                item.Tag = obj;
                valueList.Items.Add(item);
            }
            foreach (var v in type.GetProperties(showMenber))
            {
                object obj;
                try
                {
                    obj = v.GetValue(element) ?? "<null>";
                }
                catch (Exception e)
                {
                    obj = e.ToString();
                }
                var texts = new string[] { v.Name, obj.ToString() };
                var item = new ListViewItem(texts);
                item.Tag = obj;
                valueList.Items.Add(item);
            }
            valueList.EndUpdate();
        }

        public void SelectElement(Element element)
        {
            var node = GetTreeNode(element);
            if(node == null)
            {
                MessageBox.Show(this, "指定されたElementはツリー内にありませんでした。", "探索結果");
                return;
            }
            node.EnsureVisible();
            node.Expand();
            syntaxTree.SelectedNode = node;
        }

        private TreeNode GetTreeNode(Element element)
        {
            TreeNodeCollection nodes;
            if (element == null)
            {
                return null;
            }
            else if (element is Root)
            {
                nodes = syntaxTree.Nodes;
            }
            else
            {
                var node = GetTreeNode(element.Parent);
                if(node == null)
                {
                    return null;
                }
                nodes = node.Nodes;
            }
            foreach(TreeNode v in nodes)
            {
                if(v.Tag == element)
                {
                    return v;
                }
            }
            return null;
        }
    }
}

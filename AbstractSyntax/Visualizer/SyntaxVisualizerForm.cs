using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace AbstractSyntax.Visualizer
{
    partial class SyntaxVisualizerForm : Form
    {
        private IVisualizerObjectProvider Provider;
        private SyntaxVisualizerTree Root;
        private SyntaxVisualizerTree Target;

        public SyntaxVisualizerForm(IVisualizerObjectProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            InitializeComponent();
            Provider = provider;
            var data = (InitalizeData)provider.GetObject();
            Root = data.Root;
            Target = data.Target;
        }

        private void ShownHandler(object sender, EventArgs e)
        {
            syntaxTree.BeginUpdate();
            var node = AppendTree(syntaxTree.Nodes, Root);
            AppendTrees(node, Root);
            SelectElement(Target);
            syntaxTree.EndUpdate();
        }

        private void AddSelectHandler(object sender, TreeViewEventArgs e)
        {
            syntaxTree.BeginUpdate();
            var node = syntaxTree.SelectedNode;
            Text = node.Text;
            ShowValueList((SyntaxVisualizerTree)node.Tag);
            syntaxTree.EndUpdate();
        }

        private void ItemActivateHandler(object sender, EventArgs e)
        {
            syntaxTree.BeginUpdate();
            var item = valueList.SelectedItems[0];
            SelectElement((SyntaxVisualizerTree)item.Tag);
            syntaxTree.EndUpdate();
        }

        private void AfterExpandHandler(object sender, TreeViewEventArgs e)
        {
            syntaxTree.BeginUpdate();
            foreach (TreeNode n in e.Node.Nodes)
            {
                AppendTrees(n, (SyntaxVisualizerTree)n.Tag);
            }
            syntaxTree.EndUpdate();
        }

        private TreeNode AppendTree(TreeNodeCollection nodes, SyntaxVisualizerTree data)
        {
            var node = nodes.Add(data.ToString());
            node.Tag = data;
            return node;
        }

        private void AppendTrees(TreeNode node, SyntaxVisualizerTree data)
        {
            var nodes = node.Nodes;
            if(nodes.Count > 0)
            {
                return;
            }
            data.TransferValues(Provider);
            foreach (var v in data.Child)
            {
                AppendTree(nodes, v);
            }
        }

        private void ShowValueList(SyntaxVisualizerTree data)
        {
            if (data == null)
            {
                valueList.Items.Clear();
                return;
            }
            valueList.BeginUpdate();
            valueList.Items.Clear();
            data.TransferValues(Provider);
            foreach(var v in data.PropertyValues)
            {
                AddValue(v.Key, v.Value);
            }
            valueList.EndUpdate();
        }

        private ListViewItem AddValue(string itemName, object obj)
        {
            var texts = new string[] { itemName, obj.ToString() };
            var item = new ListViewItem(texts);
            item.Tag = obj;
            valueList.Items.Add(item);
            var list = obj as IReadOnlyList<object>;
            if (obj is SyntaxVisualizerTree)
            {
                item.BackColor = Color.LightBlue;
            }
            else if (list != null)
            {
                for(var i = 0; i < list.Count; ++i)
                {
                    AddValue(itemName + "[" + i + "]", list[i]);
                }
            }
            return item;
        }

        private void SelectElement(SyntaxVisualizerTree data)
        {
            var node = GetTreeNode(data);
            if(node == null)
            {
                MessageBox.Show(this, "指定されたElementはツリー内にありませんでした。", "探索結果");
                return;
            }
            node.EnsureVisible();
            syntaxTree.SelectedNode = node;
        }

        private TreeNode GetTreeNode(SyntaxVisualizerTree data)
        {
            TreeNodeCollection nodes;
            if (data == null)
            {
                return null;
            }
            else if (data.Equals(Root))
            {
                nodes = syntaxTree.Nodes;
            }
            else
            {
                var node = GetTreeNode(data.Parent);
                if(node == null)
                {
                    return null;
                }
                AppendTrees(node, data.Parent);
                node.Expand();
                nodes = node.Nodes;
            }
            foreach(TreeNode node in nodes)
            {
                if(node.Tag.Equals(data))
                {
                    return node;
                }
            }
            return null;
        }
    }
}

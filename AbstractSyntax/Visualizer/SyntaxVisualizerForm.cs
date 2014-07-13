using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace AbstractSyntax.Visualizer
{
    public partial class SyntaxVisualizerForm : Form
    {
        private dynamic root;
        private dynamic target;
        private const BindingFlags showMenber = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public SyntaxVisualizerForm(dynamic target)
        {
            if(target == null)
            {
                throw new ArgumentNullException("target");
            }
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
            ShowValueList(node.Tag);
        }

        private void ItemActivateHandler(object sender, EventArgs e)
        {
            var item = valueList.SelectedItems[0];
            SelectElement(item.Tag);
        }

        private void AddTree(TreeNodeCollection nodes, dynamic data)
        {
            if (data == null)
            {
                var node = new TreeNode("<null>");
                nodes.Add(node);
            }
            else
            {
                var node = new TreeNode(data.ToString());
                node.Tag = data;
                nodes.Add(node);
                foreach (var v in data)
                {
                    AddTree(node.Nodes, v);
                }
            }
        }

        private void ShowValueList(dynamic data)
        {
            valueList.BeginUpdate();
            valueList.Groups.Clear();
            valueList.Items.Clear();
            if (data != null)
            {
                var type = data.GetType();
                AddGroup(data, type);
            }
            valueList.EndUpdate();
        }

        private ListViewGroup AddGroup(dynamic data, Type type)
        {
            var group = new ListViewGroup(type.Name);
            group.Tag = type;
            valueList.Groups.Add(group);
            foreach (var v in type.GetProperties(showMenber))
            {
                object obj;
                try
                {
                    obj = v.GetValue(data) ?? "<null>";
                }
                catch (Exception e)
                {
                    obj = e.ToString();
                }
                AddValue(group, v.Name, obj);
            }
            return group;
        }

        private ListViewItem AddValue(ListViewGroup group, string itemName, object obj)
        {
            var texts = new string[] { itemName, obj.ToString() };
            var item = new ListViewItem(texts);
            item.Tag = obj;
            valueList.Items.Add(item);
            group.Items.Add(item);
            var list = obj as IReadOnlyList<object>;
            if (obj.GetType().GetInterface("IReadOnlyTree`1") != null)
            {
                item.BackColor = Color.LightBlue;
            }
            else if (list != null)
            {
                for(var i = 0; i < list.Count; ++i)
                {
                    AddValue(group, itemName + "[" + i + "]", list[i]);
                }
            }
            return item;
        }

        private void SelectElement(dynamic data)
        {
            var node = GetTreeNode(data);
            if(node == null)
            {
                MessageBox.Show(this, "指定されたElementはツリー内にありませんでした。", "探索結果");
                return;
            }
            node.EnsureVisible();
            node.Expand();
            syntaxTree.SelectedNode = node;
        }

        private TreeNode GetTreeNode(dynamic data)
        {
            TreeNodeCollection nodes;
            if (data == null)
            {
                return null;
            }
            else if (data.Equals(root))
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
                nodes = node.Nodes;
            }
            foreach(TreeNode v in nodes)
            {
                if(v.Tag.Equals(data))
                {
                    return v;
                }
            }
            return null;
        }
    }
}

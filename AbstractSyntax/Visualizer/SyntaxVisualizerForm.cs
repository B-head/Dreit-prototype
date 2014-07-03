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
        private Root root;
        private Element target;
        private const BindingFlags showMenber = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public SyntaxVisualizerForm(Element target)
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
            ShowValueList((Element)node.Tag);
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
                    AddTree(node.Nodes, (Element)v);
                }
            }
        }

        private void ShowValueList(Element element)
        {
            valueList.BeginUpdate();
            valueList.Groups.Clear();
            valueList.Items.Clear();
            if (element != null)
            {
                var type = element.GetType();
                AddGroup(element, type);
                //while(type != typeof(object))
                //{
                //    AddGroup(element, type);
                //    type = type.BaseType;
                //}
            }
            valueList.EndUpdate();
        }

        private ListViewGroup AddGroup(Element element, Type type)
        {
            var group = new ListViewGroup(type.Name);
            group.Tag = type;
            valueList.Groups.Add(group);
            foreach (var v in type.GetFields(showMenber))
            {
                object obj = v.GetValue(element) ?? "<null>";
                AddValue(group, v.Name, obj);
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
            if(obj is Element)
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

        private void SelectElement(Element element)
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

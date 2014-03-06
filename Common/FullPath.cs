﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum CodeType
    {
        Nop,
        Pop,
        Ret,
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        Echo,
    }

    public interface PathNode
    {
        string Name { get; }
        int Id { get; }
    }

    public class FullPath : IEquatable<FullPath>
    {
        private List<PathNode> _Path;
        public IReadOnlyList<PathNode> Path { get { return _Path; } }

        public FullPath()
        {
            _Path = new List<PathNode>();
        }

        public void Append(PathNode node)
        {
            _Path.Add(node);
        }

        public string Name
        {
            get { return _Path[_Path.Count - 1].Name; }
        }

        public int Id
        {
            get { return _Path[_Path.Count - 1].Id; }
        }

        public bool Equals(FullPath other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (_Path.Count != other._Path.Count)
            {
                return false;
            }
            for (int i = 0; i < _Path.Count; i++)
            {
                if (_Path[i].Id != other._Path[i].Id)
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as FullPath);
        }

        public override int GetHashCode()
        {
            int result = 0;
            foreach (var v in _Path)
            {
                result ^= v.Id;
            }
            return result;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < _Path.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(".");
                }
                builder.Append(_Path[i].Name);
            }
            return builder.ToString();
        }

        public static bool operator ==(FullPath arg1, FullPath arg2)
        {
            if (Object.ReferenceEquals(arg1, null) && Object.ReferenceEquals(arg2, null))
            {
                return true;
            }
            return arg1.Equals(arg2);
        }

        public static bool operator !=(FullPath arg1, FullPath arg2)
        {
            return !(arg1 == arg2);
        }
    }
}

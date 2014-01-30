using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class FullName : IEquatable<FullName>
    {
        public List<string> NameList { get; set; }
        public List<int> IdList { get; set; }

        public FullName()
        {
            NameList = new List<string>();
            IdList = new List<int>();
        }

        public void Append(string name, int id)
        {
            NameList.Add(name);
            IdList.Add(id);
        }

        public string Name
        {
            get { return NameList[NameList.Count - 1]; }
        }

        public bool Equals(FullName other)
        {
            if(Object.ReferenceEquals(other, null))
            {
                return false;
            }
            if(Object.ReferenceEquals(this, other))
            {
                return true;
            }
            if(IdList.Count != other.IdList.Count)
            {
                return false;
            }
            for(int i = 0; i < IdList.Count; i++)
            {
                if(IdList[i] != other.IdList[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as FullName);
        }

        public override int GetHashCode()
        {
            int result = 0;
            foreach(int v in IdList)
            {
                result ^= v;
            }
            return result;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < NameList.Count; i++)
            {
                if(i > 0)
                {
                    builder.Append(".");
                }
                builder.Append(NameList[i]);
            }
            return builder.ToString();
        }

        public static bool operator ==(FullName arg1, FullName arg2)
        {
            if (Object.ReferenceEquals(arg1, null) && Object.ReferenceEquals(arg2, null))
            {
                return true;
            }
            return arg1.Equals(arg2);
        }

        public static bool operator !=(FullName arg1, FullName arg2)
        {
            return !(arg1 == arg2);
        }
    }
}

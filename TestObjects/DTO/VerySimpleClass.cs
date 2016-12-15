using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class VerySimpleClass
    {
        public string MyString;
        public int MyInt;

        public VerySimpleClass() { }

        public VerySimpleClass(Business.VerySimpleClass addr)
        {
            MyString = addr.MyString;
            MyInt = addr.MyInt;
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(VerySimpleClass)) return false;
            var target = obj as VerySimpleClass;
            return MyString == target.MyString
                && MyInt == target.MyInt;
        }
    }
}

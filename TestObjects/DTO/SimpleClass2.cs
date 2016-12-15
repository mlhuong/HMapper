using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class SimpleClass2
    {
        public int Key { get; set; }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(SimpleClass2)) return false;
            var target = obj as SimpleClass2;
            return Key == target.Key;
        }

        public SimpleClass2() { }

        public SimpleClass2(Business.SimpleClass set)
        {
            Key = set.Id;
        }
    }
}

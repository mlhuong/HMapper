using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class SimpleSet
    {
        public VerySimpleClass[] VerySimpleClasses { get; set; }
        public override int GetHashCode()
        {
            return 1;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(SimpleSet)) return false;
            var target = obj as SimpleSet;
            return VerySimpleClasses.EnumerableEquals(target.VerySimpleClasses);
        }

        public SimpleSet() { }

        public SimpleSet(Business.SimpleSet set)
        {
            if (set.VerySimpleClasses != null)
            {
                
                VerySimpleClasses = set.VerySimpleClasses.Select(a => a==null? null : new VerySimpleClass()
                {
                    MyInt = a.MyInt,
                    MyString = a.MyString
                }

                ).ToArray();
               
            }
        }
    }
}

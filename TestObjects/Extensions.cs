using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects
{
    public static class Extensions
    {

        public static bool EnumerableEquals(this IEnumerable e1, IEnumerable e2)
        {
            if (e1 == null) return e2 == null;
            if (e2 == null) return false;
            object[] tab1 = e1.Cast<object>().ToArray();
            object[] tab2 = e2.Cast<object>().ToArray();
            if (tab1.Length != tab2.Length) return false;
            for (int i = 0; i < tab1.Length; i++)
                if (!tab1[i].NullOrEquals(tab2[i]))
                    return false;
            
            return true;
        }


        public static bool NullOrEquals(this object o1, object o2)
        {
            if (o1 == null) return o2 == null;
            return o1.Equals(o2);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class VerySimpleClass
    {
        public string MyString;
        public int MyInt;

        public static VerySimpleClass Create()
        {
            var rnd = new Random();
            return new VerySimpleClass()
            {
                MyString = $"str{rnd.Next()}",
                MyInt = rnd.Next()
            };
        }

        public static VerySimpleClass[] CreateMany(int nb)
        {
            return Enumerable.Range(1, nb).Select(i => Create()).ToArray();
        }
    }
}

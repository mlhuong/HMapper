using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class PolymorphicSubClass : PolymorphicBaseClass
    {
        public string Name;

        protected PolymorphicSubClass(int id)
            :base(id)
        {
            Name = Guid.NewGuid().ToString();
        }
        public static new PolymorphicSubClass Create(int id)
        {
            return new PolymorphicSubClass(id);
        }

        public static new PolymorphicSubClass[] CreateMany(int nb)
        {
            return Enumerable.Range(1, nb).Select(i => Create(i)).ToArray();
        }
    }

    
}

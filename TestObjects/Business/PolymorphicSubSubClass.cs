using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class PolymorphicSubSubClass : PolymorphicSubClass
    {
        public Guid Guid;

        protected PolymorphicSubSubClass(int id)
            :base(id)
        {
            Guid = Guid.NewGuid();
        }
        public static new PolymorphicSubSubClass Create(int id)
        {
            return new PolymorphicSubSubClass(id);
        }

        public static new PolymorphicSubSubClass[] CreateMany(int nb)
        {
            return Enumerable.Range(1, nb).Select(i => Create(i)).ToArray();
        }
    }

    
}

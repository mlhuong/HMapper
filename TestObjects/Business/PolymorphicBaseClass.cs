using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class PolymorphicBaseClass : IPolymorphic
    {
        public int Id;
        public string AString;

        protected PolymorphicBaseClass(int id)
        {
            Id = id;
            AString = "astring";
        }

        public static PolymorphicBaseClass Create(int id)
        {
            return new PolymorphicBaseClass(id);
        }

        public static PolymorphicBaseClass[] CreateMany(int nb)
        {
            return Enumerable.Range(1, nb).Select(i => Create(i)).ToArray();
        }
    }

    
}

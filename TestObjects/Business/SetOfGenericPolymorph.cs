using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class SetOfGenericPolymorph
    {
        public int Id;
        public IList<MappedObjectGeneric<PolymorphicBaseClass>> Set;

        public static SetOfGenericPolymorph Create(int id)
        {
            Random rnd = new Random();

            return new SetOfGenericPolymorph()
            {
                Id = id,
                Set = new List<MappedObjectGeneric<PolymorphicBaseClass>> () {
                    MappedObjectGeneric<PolymorphicBaseClass>.Create(1, PolymorphicBaseClass.Create(10)),
                    MappedObjectGeneric<PolymorphicBaseClass>.Create(2, PolymorphicSubClass.Create(11)),
                    MappedObjectGeneric<PolymorphicBaseClass>.Create(3, PolymorphicSubSubClass.Create(12))
                }
            };
        }

        public static SetOfGenericPolymorph[] CreateMany(int nb)
        {
            return Enumerable.Range(1, nb).Select(i => Create(i)).ToArray();
        }
    
    }
}

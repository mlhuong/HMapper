using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class SetOfPolymorphic
    {
        public int Id;
        public IList<PolymorphicBaseClass> PolymorphicBaseClasses;

        public static SetOfPolymorphic Create(int id)
        {
            Random rnd = new Random();

            return new SetOfPolymorphic()
            {
                Id = id,
                PolymorphicBaseClasses = new List<PolymorphicBaseClass>() {
                    PolymorphicBaseClass.Create(rnd.Next()),
                    PolymorphicSubClass.Create(rnd.Next()),
                    PolymorphicSubSubClass.Create(rnd.Next())
                }
            };
        }

        public static SetOfPolymorphic[] CreateMany(int nb)
        {
            return Enumerable.Range(1, nb).Select(i => Create(i)).ToArray();
        }
    
    }
}

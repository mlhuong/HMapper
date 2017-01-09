using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class SetOfPolymorphic
    {
        public int Id;
        public PolymorphicBaseClass[] PolymorphicBaseClasses;

        public SetOfPolymorphic() { }

        public SetOfPolymorphic(Business.SetOfPolymorphic source)
        {
            Id = source.Id;
            if (source.PolymorphicBaseClasses != null)
            {
                PolymorphicBaseClasses = source.PolymorphicBaseClasses.Select(x => PolymorphicBaseClass.GetMostConcrete(x)).ToArray();
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(SetOfPolymorphic)) return false;
            var target = obj as SetOfPolymorphic;
            return Id == target.Id
                && PolymorphicBaseClasses.EnumerableEquals(target.PolymorphicBaseClasses);
        }
    }

    
}

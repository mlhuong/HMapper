using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class SetOfGenericPolymorph
    {
        public int Id;
        public IList<MappedObjectGeneric<PolymorphicBaseClass>> Set;

        public SetOfGenericPolymorph() { }

        public SetOfGenericPolymorph(Business.SetOfGenericPolymorph source)
        {
            Id = source.Id;
            if (source.Set != null)
            {
                Set = source.Set.Select(x => new MappedObjectGeneric<PolymorphicBaseClass>(x)).ToArray();
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(SetOfGenericPolymorph)) return false;
            var target = obj as SetOfGenericPolymorph;
            return Id == target.Id
                && Set.EnumerableEquals(target.Set);
        }
    }

    
}

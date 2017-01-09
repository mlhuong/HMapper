using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class NonGenericSet
    {
        public int Id { get; set; }
        
        public ArrayList Set { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(NonGenericSet)) return false;
            var target = obj as NonGenericSet;

            if (Id != target.Id) return false;

            if (Set == null) return target.Set == null;
            if (target.Set == null) return false;

            // Set can be randomly SimpleClass or SimpleClass2
            if (Set.Count != target.Set.Count) return false;
            for (int i = 0; i < Set.Count; i++)
            {
                if (Set[i] is SimpleClass || Set[i] is SimpleClass2)
                    continue;
                if (!Set[i].Equals(target.Set[i]))
                    return false;
            }
            return true;

        }
        

        public NonGenericSet(Business.NonGenericSet set)
        {
            Id = set.Id;
            Set = new ArrayList();
            foreach (var item in set.Set)
            {
                if (item is Business.SimpleClass)
                    Set.Add(new SimpleClass((Business.SimpleClass)item));
                else
                    Set.Add(item);
            }
        }
    }
}

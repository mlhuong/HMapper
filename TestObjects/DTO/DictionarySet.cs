using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class DictionarySet
    {
        public Guid Id { get; set; }
        public IDictionary<string, int> DictSimple { get; set; }
        public IDictionary<SimpleClass, VerySimpleClass> DictObjects { get; set; }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(DictionarySet)) return false;
            var target = obj as DictionarySet;
            return Id == target.Id
                && DictSimple.EnumerableEquals(target.DictSimple)
                && DictObjects.EnumerableEquals(target.DictObjects);
        }

        public DictionarySet() { }

        public DictionarySet(Business.DictionarySet set)
        {
            Id = set.Id;
            if (set.DictSimple != null)
                DictSimple = set.DictSimple.ToDictionary(x => x.Key, x => x.Value);
            if (set.DictObjects != null)
                DictObjects = set.DictObjects.ToDictionary(x => new SimpleClass(x.Key), x => new VerySimpleClass(x.Value));
        }
    }
}

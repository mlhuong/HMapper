using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class DictionarySet
    {
        public Guid Id { get; set; }
        public IDictionary<string, int> DictSimple { get; set; }
        public IDictionary<SimpleClass, VerySimpleClass> DictObjects { get; set; }

        public static DictionarySet Create(int count)
        {
            Random rnd = new Random();
            var result = new DictionarySet()
            {
                Id = Guid.NewGuid(),
                DictSimple = new Dictionary<string, int>(),
                DictObjects = new Dictionary<SimpleClass, VerySimpleClass>()
            };

            Enumerable.Range(1, count).ToList().ForEach(i => result.DictSimple.Add(Guid.NewGuid().ToString(), i));
            Enumerable.Range(1, count).ToList().ForEach(i => result.DictObjects.Add(SimpleClass.Create(i), VerySimpleClass.Create()));
            return result;

        }

        public static DictionarySet[] CreateMany(int nb, int nbItems)
        {
            return Enumerable.Range(1, nb).Select(i => Create(nbItems)).ToArray();
        }
    }
}

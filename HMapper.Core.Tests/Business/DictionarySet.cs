using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Business
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

            foreach (var i in Enumerable.Range(1, count))
                result.DictSimple.Add(Guid.NewGuid().ToString(), i);

            foreach (var i in Enumerable.Range(1, count))
                result.DictObjects.Add(SimpleClass.Create(i), VerySimpleClass.Create());

            return result;
        }

        public static DictionarySet[] CreateMany(int nb, int nbItems)
        {
            return Enumerable.Range(1, nb).Select(i => Create(nbItems)).ToArray();
        }
    }
}

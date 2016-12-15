using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class DictionarySetCircular
    {
        public Guid Id { get; set; }
        public IDictionary<DictionarySetCircular, int> DictObjects { get; set; }

        public static DictionarySetCircular Create()
        {
            Random rnd = new Random();
            var result = new DictionarySetCircular()
            {
                Id = Guid.NewGuid(),
                DictObjects = new Dictionary<DictionarySetCircular, int>()
            };
            result.DictObjects.Add(result, 5);
            return result;
        }
    }
}

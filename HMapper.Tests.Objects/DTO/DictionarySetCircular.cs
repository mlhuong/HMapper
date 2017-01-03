using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Objects.DTO
{
    public class DictionarySetCircular
    {
        public Guid Id { get; set; }
        public IDictionary<DictionarySetCircular, int> DictObjects { get; set; }

        public DictionarySetCircular() { }

        public DictionarySetCircular(Business.DictionarySetCircular set)
        {
            Id = set.Id;
            if (set.DictObjects != null)
                DictObjects = set.DictObjects.ToDictionary(x => new DictionarySetCircular(x.Key), x => x.Value);
        }
    }
}

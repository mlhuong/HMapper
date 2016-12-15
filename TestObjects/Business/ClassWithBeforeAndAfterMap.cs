using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class ClassWithBeforeAndAfterMap
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int AnInt { get; set; }

        public static ClassWithBeforeAndAfterMap Create()
        {
            var ret = new ClassWithBeforeAndAfterMap()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                AnInt = 55 
            };
            return ret;
        }
    }
}

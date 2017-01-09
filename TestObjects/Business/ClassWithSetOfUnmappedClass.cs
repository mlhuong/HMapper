using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class ClassWithSetOfUnmappedClass
    {
        public Guid Id;
        public List<Stream> Set;

        public static ClassWithSetOfUnmappedClass Create()
        {
            return new ClassWithSetOfUnmappedClass()
            {
                Id = Guid.NewGuid(),
                Set = new List<Stream>()
            };
        }
    }
}

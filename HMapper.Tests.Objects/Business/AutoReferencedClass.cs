using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Objects.Business
{
    public class AutoReferencedClass
    {
        public int Id;
        public AutoReferencedClass Child;
        public AutoReferencedClass Parent;

        public static AutoReferencedClass Create(int id)
        {
            var ret = new AutoReferencedClass()
            {
                Id = id,
                Child = new AutoReferencedClass() { Id = id + 1000000 },
                Parent = null
            };
            ret.Child.Parent = ret;
            return ret;
        }

        public static AutoReferencedClass[] CreateMany(int nb)
        {
            return Enumerable.Range(1, nb).Select(i => Create(i)).ToArray();
        }
    }
}

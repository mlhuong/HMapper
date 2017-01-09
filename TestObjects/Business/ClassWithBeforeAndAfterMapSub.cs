using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class ClassWithBeforeAndAfterMapSub : ClassWithBeforeAndAfterMap
    {
        public string StringFromSub;

        public static new ClassWithBeforeAndAfterMapSub Create()
        {
            var ret = new ClassWithBeforeAndAfterMapSub()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                AnInt = 55,
                StringFromSub = "init"
            };
            return ret;
        }
    }
}

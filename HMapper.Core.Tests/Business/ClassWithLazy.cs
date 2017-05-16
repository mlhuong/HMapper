using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMapper.Tests.Business
{
    public class ClassWithLazy
    {
        public Guid Id;
        public Lazy<VerySimpleClass> LazySimpleClass;
        
        public static ClassWithLazy Create()
        {
            return new ClassWithLazy()
            {
                Id = Guid.NewGuid(),
                LazySimpleClass = new Lazy<Business.VerySimpleClass>(() => GetClass().GetAwaiter().GetResult())
            };

        }

        private static async Task<VerySimpleClass> GetClass()
        {
            return await Task.FromResult(new VerySimpleClass() { MyInt = 5, MyString = "aaa" });
        }
    }
}

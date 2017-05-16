using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMapper.Tests.DTO
{
    public class ClassWithAsyncCallToMapper
    {
        public VerySimpleClass SimpleClass;

        public static async Task<VerySimpleClass> GetClass()
        {
            return await Task.FromResult(HMapper.Mapper.Map<Business.VerySimpleClass, DTO.VerySimpleClass>(Business.VerySimpleClass.Create())).ConfigureAwait(false);
        }

        public ClassWithAsyncCallToMapper()
        {
            SimpleClass = GetClass().GetAwaiter().GetResult();
        }
    }
}

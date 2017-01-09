using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class FuncMappingPolymorphSub : FuncMappingPolymorph
    {
        public string Name;

        protected FuncMappingPolymorphSub(int id)
            :base(id)
        {
            Name = Guid.NewGuid().ToString();
        }


        public static new FuncMappingPolymorphSub Create(int id)
        {
            return new FuncMappingPolymorphSub(id);
        }
    }

    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Objects.Business
{
    public class FuncMappingPolymorph : IInterfaceForFuncMappingPolymorph
    {
        public int Id;
        public DateTime ADate;
        public string MyString;
        public int IntFromInterface { get; set; }

        protected FuncMappingPolymorph(int id)
        {
            Id = id;
            MyString = "astring";
            ADate = DateTime.Today;
            IntFromInterface = id + 10;
        }

        public static FuncMappingPolymorph Create(int id)
        {
            return new FuncMappingPolymorph(id);
        }
    }

    
}

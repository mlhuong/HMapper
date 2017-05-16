using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Business
{
    public class ClassWithException
    {
        public Guid Id { get; set; }
        public string AString
        {
            get
            {
                string s = null;
                return s.ToUpper();
            }
        }

        public static ClassWithException Create()
        {
            return new ClassWithException()
            {
                Id = Guid.NewGuid()
            };
        }
    }
}

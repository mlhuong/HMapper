using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.DTO
{
    public class ClassWithException
    {
        public Guid Id { get; set; }
        public string AString { get; set; }

        public ClassWithException()
        {

        }

        public ClassWithException(Business.ClassWithException source)
        {
            Id = source.Id;
            AString = source.AString;
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ClassWithException)) return false;
            var target = obj as ClassWithException;
            return Id == target.Id && AString == target.AString;
        }
    }

    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HMapper.Tests.DTO
{
    public class ClassWithSetOfUnmappedClass
    {
        public Guid Id;
        public List<XDocument> Set;

        public ClassWithSetOfUnmappedClass()
        {
        }

        public ClassWithSetOfUnmappedClass(Business.ClassWithSetOfUnmappedClass source)
        {
            Id = source.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ClassWithSetOfUnmappedClass)) return false;
            var target = obj as ClassWithSetOfUnmappedClass;
            return Id == target.Id;
        }
    }
}

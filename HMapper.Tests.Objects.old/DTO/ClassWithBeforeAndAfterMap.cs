using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Objects.DTO
{
    public class ClassWithBeforeAndAfterMap
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int AnInt { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ClassWithBeforeAndAfterMap)) return false;
            var target = obj as ClassWithBeforeAndAfterMap;
            return Id == target.Id
                && Name == target.Name
                && AnInt == target.AnInt;
        }
    }
}

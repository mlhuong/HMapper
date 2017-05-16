using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMapper.Tests.DTO
{
    public class ClassWithLazy
    {
        public Guid Id;
        public VerySimpleClass SimpleClass;

        public ClassWithLazy(Business.ClassWithLazy source)
        {
            Id = source.Id;
            SimpleClass = new VerySimpleClass(source.LazySimpleClass.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ClassWithLazy)) return false;
            var target = obj as ClassWithLazy;
            return Id == target.Id
                && SimpleClass.NullOrEquals(target.SimpleClass);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

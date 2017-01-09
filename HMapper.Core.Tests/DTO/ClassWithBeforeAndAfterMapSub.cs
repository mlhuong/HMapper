using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.DTO
{
    public class ClassWithBeforeAndAfterMapSub : ClassWithBeforeAndAfterMap
    {
        public string StringFromSub;

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ClassWithBeforeAndAfterMapSub)) return false;
            var target = obj as ClassWithBeforeAndAfterMapSub;
            return Id == target.Id
                && Name == target.Name
                && StringFromSub == target.StringFromSub;
        }
    }
}

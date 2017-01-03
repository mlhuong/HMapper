using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.DTO
{
    public class SubSimpleClass : SimpleClass
    {
        public string AdditionalInfo { get; set; }

        public SubSimpleClass()
        { }

        public SubSimpleClass(Business.SimpleClass simpleClass, string additionalInfo)
            :base(simpleClass)
        {
            AdditionalInfo = additionalInfo;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(SubSimpleClass)) return false;
            var target = obj as SubSimpleClass;
            if (AdditionalInfo != target.AdditionalInfo) return false;
            return _Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

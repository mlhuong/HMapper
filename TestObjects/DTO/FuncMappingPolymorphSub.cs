using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class FuncMappingPolymorphSub : FuncMappingPolymorph
    {
        public string Name;

        public FuncMappingPolymorphSub()
        { }

        public FuncMappingPolymorphSub(Business.FuncMappingPolymorphSub source)
            :base(source)
        {
            Name = source.Name;
            ADate = source.ADate.AddDays(1);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(FuncMappingPolymorphSub)) return false;
            var target = obj as FuncMappingPolymorphSub;
            return Id == target.Id
                && target.Name == Name
                && target.AString == AString
                && target.ADate == ADate
                && FromInterface == target.FromInterface;
        }
    }
}

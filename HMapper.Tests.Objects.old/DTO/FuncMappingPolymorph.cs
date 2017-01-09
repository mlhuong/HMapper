using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Objects.DTO
{
    public class FuncMappingPolymorph
    {
        public int Id;
        public string AString;
        public DateTime ADate;
        public int FromInterface;

        public FuncMappingPolymorph()
        { }

        public FuncMappingPolymorph(Business.FuncMappingPolymorph source)
        {
            Id = source.Id;
            AString = source.MyString.ToUpper();
            ADate = source.ADate;
            FromInterface = source.IntFromInterface;
        }

        public static FuncMappingPolymorph GetMostConcrete(Business.FuncMappingPolymorph source)
        {
            if (source is Business.FuncMappingPolymorphSub)
                return new FuncMappingPolymorphSub((Business.FuncMappingPolymorphSub)source);
            return new FuncMappingPolymorph(source);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(FuncMappingPolymorph)) return false;
            var target = obj as FuncMappingPolymorph;
            return target.Id == Id
                && AString == target.AString
                && ADate == target.ADate
                && FromInterface == target.FromInterface;
        }
    }
}

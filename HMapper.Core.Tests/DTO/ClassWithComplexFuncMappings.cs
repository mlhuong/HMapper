using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.DTO
{
    public class ClassWithComplexFuncMappings
    {
        public VerySimpleClass[] VerySimpleClasses;
        public VerySimpleClass VerySimpleClass;
        public string AString;
        public DateTime? ADate;
        public ClassWithStructAndEnum.MyStruct AStruct;
        public string AnotherString;
        public int AnInt;
        public int AnInt2;
        public static List<VerySimpleClass> AFunction()
        {
            return new List<DTO.VerySimpleClass> { new VerySimpleClass() { MyString = "dfdf" } };
        }

        public override int GetHashCode()
        {
            return 1;
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ClassWithComplexFuncMappings)) return false;
            var target = obj as ClassWithComplexFuncMappings;
            return VerySimpleClasses.EnumerableEquals(target.VerySimpleClasses)
                && VerySimpleClass.NullOrEquals(target.VerySimpleClass)
                && AString == target.AString
                && ADate == target.ADate
                && AStruct == target.AStruct;
        }
    }

   
}

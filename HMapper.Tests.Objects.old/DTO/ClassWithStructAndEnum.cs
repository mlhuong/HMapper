using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Objects.DTO
{
    public class ClassWithStructAndEnum
    {
        public Guid Id;
        public MyStruct AStruct;
        public MyEnum1 AnEnum1;
        public MyEnum2 AnEnum2;

        public struct MyStruct
        {
            public string Field1;
            public string AnotherField2;

            public static bool operator ==(MyStruct val1, MyStruct val2)
            {
                return val1.Field1 == val2.Field1
                    && val1.AnotherField2 == val2.AnotherField2;
            }

            public static bool operator !=(MyStruct val1, MyStruct val2)
            {
                return val1.Field1 != val2.Field1
                    || val1.AnotherField2 != val2.AnotherField2;
            }

        }

        public enum MyEnum1
        {
            Entry1 = 0,
            Entry2 = 1
        }
        public enum MyEnum2
        {
            Entry1 = 0,
            Entry2 = 1
        }

        public static MyEnum2 Convert(Business.ClassWithStructAndEnum.MyEnum2 val)
        {
            switch (val)
            {
                case Business.ClassWithStructAndEnum.MyEnum2.Entry1:
                case Business.ClassWithStructAndEnum.MyEnum2.Entry2:
                    return MyEnum2.Entry1;
                default: return MyEnum2.Entry2;
            }
        }

        public ClassWithStructAndEnum(Business.ClassWithStructAndEnum source)
        {
            Id = source.Id;
            AStruct = new MyStruct()
            {
                Field1 = source.AStruct.Field1,
                AnotherField2 = source.AStruct.Field2
            };
            AnEnum1 = (DTO.ClassWithStructAndEnum.MyEnum1)(int)source.AnEnum1;
            AnEnum2 = Convert(source.AnEnum2);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ClassWithStructAndEnum)) return false;
            var target = obj as ClassWithStructAndEnum;
            return Id == target.Id
                && AStruct == target.AStruct
                && AnEnum1 == target.AnEnum1
                && AnEnum2 == target.AnEnum2;
        }

    }
}

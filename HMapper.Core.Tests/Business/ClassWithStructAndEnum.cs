using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Business
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
            public string Field2;

            public MyStruct(string a, string b) { Field1 = a; Field2 = b; }
        }

        public enum MyEnum1
        {
            Entry1 = 0,
            Entry2 = 1
        }

        public enum MyEnum2
        {
            Entry1 = 0,
            Entry2 = 1,
            Entry3 = 2
        }

        public static ClassWithStructAndEnum Create()
        {
            return new ClassWithStructAndEnum()
            {
                Id = Guid.NewGuid(),
                AStruct = new MyStruct { Field1 = Guid.NewGuid().ToString(), Field2 = Guid.NewGuid().ToString() },
                AnEnum1 = MyEnum1.Entry2,
                AnEnum2 = MyEnum2.Entry3
            };
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMapper.Tests.Business
{
    public class ClassWithSimpleTypes
    {
        public Guid Guid;
        public string NonConvertibleType;
        public float Value1;
        public double Value2;
        public decimal Value3;
        public int Value4;
        public Int64 Value5;
        public long Value6;
        public long Value7;

        public float? Value11;
        public double? Value12;
        public decimal? Value13;
        public int? Value14;
        public Int64? Value15;
        public long? Value16;
        public long? Value17;

        public static ClassWithSimpleTypes Create()
        {
            return new ClassWithSimpleTypes()
            {
                Guid = Guid.NewGuid(),
                NonConvertibleType = "foo",
                Value1 = 5.4f,
                Value2 = 4.7,
                Value3 = 7.9M,
                Value4 = 7,
                Value5 = 789,
                Value6 = 5,
                Value7 = long.MaxValue,
                Value11 = 1.1f,
                Value12 = 66.7,
                Value13 = 6.8M,
                Value14 = 5,
                Value15 = 6,
                Value16 = 88,
                Value17 = long.MaxValue
            };
        }
    }
}

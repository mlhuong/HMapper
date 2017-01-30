using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMapper.Tests.DTO
{
    public class ClassWithSimpleTypes
    {
        public Guid Guid;
        public int NonConvertibleType;
        public decimal Value1;
        public float Value2;
        public double Value3;
        public long Value4;
        public short Value5;
        public int Value6;
        public int Value7;

        public decimal? Value11;
        public float? Value12;
        public double? Value13;
        public long? Value14;
        public short? Value15;
        public int? Value16;
        public int? Value17;

        public ClassWithSimpleTypes()
        { }

        public ClassWithSimpleTypes(Business.ClassWithSimpleTypes source)
        {
            Guid = source.Guid;
            Value1 = (decimal)source.Value1;
            Value2 = (float)source.Value2;
            Value3 = (double)source.Value3;
            Value4 = source.Value4;
            Value5 = (short)source.Value5;
            Value6 = (int)source.Value6;
            Value7 = (int)source.Value7;

            Value11 = (decimal)source.Value11;
            Value12 = (float)source.Value12.GetValueOrDefault();
            Value13 = (double)source.Value13.GetValueOrDefault();
            Value14 = source.Value14.GetValueOrDefault();
            Value15 = (short)source.Value15.GetValueOrDefault();
            Value16 = (int)source.Value16.GetValueOrDefault();
            Value17 = (int)source.Value17.GetValueOrDefault();
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ClassWithSimpleTypes)) return false;
            var target = obj as ClassWithSimpleTypes;
            return Guid == target.Guid
                && NonConvertibleType == target.NonConvertibleType
                && Value1 == target.Value1
                && Value2 == target.Value2
                && Value3 == target.Value3
                && Value4 == target.Value4
                && Value5 == target.Value5
                && Value6 == target.Value6
                && Value7 == target.Value7
                && Value11 == target.Value11
                && Value12 == target.Value12
                && Value13 == target.Value13
                && Value14 == target.Value14
                && Value15 == target.Value15
                && Value16 == target.Value16
                && Value17 == target.Value17;
        }
    }
}

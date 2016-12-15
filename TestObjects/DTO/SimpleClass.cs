using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public interface ISimpleClass
    {

    }
    public class SimpleClass : ISimpleClass
    {
        public int Key;
        public int Integer { get; set; }

        public string String { get; set; }
        public DateTime Date { get; set; }
        public DateTime? NullableDate { get; set; }
        public DateTime? OtherDate { get; set; }
        public bool Bool { get; set; }
        public string StringToBeIgnored { get; set; }

        public VerySimpleClass VerySimpleClass { get; set; }
        public int[] IntArray { get; set; }
        public DateTime Date_Plus_2 { get; set; }
        public string VerySimpleClass2String { get; set; }

        public SimpleClass() { }

        public SimpleClass(Business.SimpleClass simpleClass)
        {
            Key = simpleClass.Id;
            Integer = simpleClass.Integer;
            String = simpleClass.String;
            Date = simpleClass.Date;
            NullableDate = simpleClass.NullableDate;
            Bool = simpleClass.Bool;
            IntArray = simpleClass.IntArray;
            Date_Plus_2 = simpleClass.Date.AddDays(2);
            VerySimpleClass2String = simpleClass.VerySimpleClass2?.MyString;
            if (simpleClass.VerySimpleClass != null)
                VerySimpleClass = new VerySimpleClass(simpleClass.VerySimpleClass);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(SimpleClass)) return false;
            return _Equals(obj);
        }

        protected internal bool _Equals(object obj)
        {
            var target = obj as SimpleClass;
            return Key == target.Key
                && Integer == target.Integer
                && String == target.String
                && Date == target.Date
                && Date_Plus_2 == target.Date_Plus_2
                && NullableDate == target.NullableDate
                && Bool == target.Bool
                && IntArray.EnumerableEquals(target.IntArray)
                && VerySimpleClass.NullOrEquals(target.VerySimpleClass)
                && VerySimpleClass2String == target.VerySimpleClass2String;
        }
    }

    
}

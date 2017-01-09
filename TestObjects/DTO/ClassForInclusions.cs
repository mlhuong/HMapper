using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class ClassForInclusions
    {
        public Guid Id;
        public string AString;
        public int AnInt;
        public DateTime ADate;

        public ClassForInclusions(Business.ClassForInclusions source)
        {
            Id = source.Id;
            AString = source.AString;
            AnInt = source.AnInt;
            ADate = DateTime.Today;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ClassForInclusions)) return false;
            var target = obj as ClassForInclusions;
            return Id == target.Id
                && AString == target.AString
                && AnInt == target.AnInt
                && ADate == target.ADate;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }


    }
}

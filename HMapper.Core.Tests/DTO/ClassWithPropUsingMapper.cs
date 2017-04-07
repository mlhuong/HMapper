using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMapper.Tests.DTO
{
    public class ClassWithPropUsingMapper
    {
        public int Id { get; set; }

        public virtual ICollection<Section> Sections { get; set; }

        public Section[] AllSections { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ClassWithPropUsingMapper)) return false;
            var target = obj as ClassWithPropUsingMapper;
            return Id == target.Id
                && Sections.EnumerableEquals(target.Sections)
                && AllSections.EnumerableEquals(target.AllSections);
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public ClassWithPropUsingMapper(Business.ClassWithPropUsingMapper source)
        {
            Id = source.Id;
            Sections = source.Sections.Select(x => new Section(x)).ToList();
            AllSections = source.AllSections.Select(x => new Section(x)).ToArray();
        }

        public class Section
        {

            public int Id { get; set; }

            public Section()
            {
            }
            public Section(Business.ClassWithPropUsingMapper.Section section)
            {
                Id = section.Id;
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != typeof(Section)) return false;
                var target = obj as Section;
                return Id == target.Id;
            }

            public override int GetHashCode()
            {
                return 1;
            }
        }
    }

    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMapper.Tests.Business
{
    public class ClassWithPropUsingMapper
    {

        public ClassWithPropUsingMapper()
        {
            Sections = new HashSet<Section>() { new Section() { Id = 5 } };

            allSections = new Lazy<Section[]>(() =>
            {
                var result = new List<Section>();
                foreach (var section in this.Sections)
                {
                    // null exception when using mapper.
                    var sectionWithoutChildrenProperties = Mapper.Map<Section, Section>(section, null);
                    result.Add(sectionWithoutChildrenProperties);
                }

                return result.ToArray();
            });
        }

        public int Id { get; set; }

        public virtual ICollection<Section> Sections { get; set; }
        
        private Lazy<Section[]> allSections;

        public Section[] AllSections => allSections.Value;

        public class Section
        {
            public Section()
            {
            }

            public int Id { get; set; }
        }
    }

    
}

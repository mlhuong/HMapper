using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class ContainerOfManuallyMappedClass
    {
        public int Id { get; set; }
        public Tuple<int, string> Tuple { get; set; }

        public ContainerOfManuallyMappedClass()
        {
        }

        public ContainerOfManuallyMappedClass(Business.ContainerOfManuallyMappedClass source)
        {
            Id = source.Id;
            if (source.Content != null)
                Tuple = System.Tuple.Create(source.Content.Id, source.Content.Title);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ContainerOfManuallyMappedClass)) return false;
            var target = obj as ContainerOfManuallyMappedClass;
            return Id == target.Id
                && Tuple.NullOrEquals(target.Tuple);
        }


    }
}

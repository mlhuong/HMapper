using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Objects.Business
{
    public class ContainerOfManuallyMappedClass
    {
        public int Id { get; set; }
        public ManuallyMappedClass Content { get; set; }

        public class ManuallyMappedClass
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }

        public static ContainerOfManuallyMappedClass Create(int id)
        {
            return new ContainerOfManuallyMappedClass()
            {
                Id = id,
                Content = new ManuallyMappedClass() { Id = id + 1, Title = Guid.NewGuid().ToString() }
            };
        }
    }
}

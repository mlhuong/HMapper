using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public interface ISimpleClass
    { }

    public class SimpleClass : ISimpleClass
    {
        public int Id { get; set; }
        public int Integer { get; set; }

        public string String { get; set; }
        public DateTime Date { get; set; }
        public DateTime? NullableDate { get; set; }
        public DateTime OtherDate { get; set; }
        public bool Bool { get; set; }
        public string StringToBeIgnored { get; set; }
        
        public VerySimpleClass VerySimpleClass { get; set; }
        public VerySimpleClass VerySimpleClass2 { get; set; }
        public int[] IntArray { get; set; }

        public static SimpleClass Create(int id)
        {
            var person = new SimpleClass()
            {
                Id = id,
                String = Guid.NewGuid().ToString(),
                Integer = id + 10,
                Date = DateTime.Now,
                NullableDate = null,
                Bool = false,
                StringToBeIgnored = "ignored property",
                IntArray = new int[] { 1, 2, 3 },
                VerySimpleClass = Business.VerySimpleClass.Create()
            };
            return person;
        }

        public static SimpleClass[] CreateMany(int nb)
        {
            return Enumerable.Range(1, nb).Select(i => Create(i)).ToArray();
        }
    }
}

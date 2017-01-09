using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class NonGenericSet
    {
        public int Id { get; set; }
        
        public ArrayList Set { get; set; } 

        public static NonGenericSet Create(int id)
        {
            Random rnd = new Random();
            return new NonGenericSet()
            {
                Id = id,
                Set = new ArrayList() { 1, SimpleClass.Create(rnd.Next()), "hello" }
            };
            
        }

        public static NonGenericSet[] CreateMany(int nb)
        {
            return Enumerable.Range(1, nb).Select(i => Create(i)).ToArray();
        }
    }
}

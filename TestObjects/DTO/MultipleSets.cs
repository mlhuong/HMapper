using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class MultipleSets
    {
        public int Id { get; set; }
        public List<SimpleClass> SimpleClasses { get; set; }
        public IEnumerable<string> StringSet { get; set; }
        public HashSet<int> IntegerSet { get; set; }
        public Collection<double> DoubleSet { get; set; }
        public ICollection<DateTime> DateSet { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(MultipleSets)) return false;
            var target = obj as MultipleSets;
            return Id == target.Id
                && SimpleClasses.EnumerableEquals(target.SimpleClasses)
                && StringSet.EnumerableEquals(target.StringSet)
                && IntegerSet.EnumerableEquals(target.IntegerSet)
                && DoubleSet.EnumerableEquals(target.DoubleSet)
                && DateSet.EnumerableEquals(target.DateSet);
        }

        public MultipleSets() { }

        public MultipleSets(Business.MultipleSets set)
        {
            Id = set.Id;
            if (set.SimpleClasses != null)
                SimpleClasses = set.SimpleClasses.Select(p => new SimpleClass(p)).ToList();
            if (set.StringSet != null)
                StringSet = set.StringSet.ToList();
            if (set.IntegerSet != null)
                IntegerSet = new HashSet<int>(set.IntegerSet);
            if (set.DoubleSet != null)
                DoubleSet = new Collection<double>(set.DoubleSet.ToList());
            if (set.DateSet != null)
                DateSet = set.DateSet.ToList();
        }
    }
}

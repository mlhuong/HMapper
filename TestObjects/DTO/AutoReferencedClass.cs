using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class AutoReferencedClass
    {
        public int Id;
        public AutoReferencedClass Child;
        public AutoReferencedClass Parent;
        static Dictionary<Business.AutoReferencedClass, AutoReferencedClass> _Cache = new Dictionary<Business.AutoReferencedClass, AutoReferencedClass>();

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(AutoReferencedClass)) return false;
            var target = obj as AutoReferencedClass;
            // incomplete test . Should be recursive, but hard to avoid infinite loop.
            return Id == target.Id
                && Child.NullOrEquals(target.Child);
        }

        public AutoReferencedClass()
        {
        }

        public static AutoReferencedClass Create(Business.AutoReferencedClass source)
        {
            if (source == null) return null;

            AutoReferencedClass ret;
            if (!_Cache.TryGetValue(source, out ret))
            {
                ret = new AutoReferencedClass();
                _Cache.Add(source, ret);
                ret.Id = source.Id;
                ret.Child = Create(source.Child);
                ret.Parent = Create(source.Parent);
            }
            return ret;
        }
    }
}

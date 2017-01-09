using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects
{
    public class Cache
    {
        static ConcurrentDictionary<Tuple<object, Type>, object> _Cache = new ConcurrentDictionary<Tuple<object, Type>, object>();
    }
}

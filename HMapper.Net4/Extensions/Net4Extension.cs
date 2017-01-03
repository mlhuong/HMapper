using Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Extensions
{
    public static class Net4Extension
    {
        public static Expression CreateInstance(this Type type)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            return ctor != null ? Expression.New(type)
                : Expression.Call(Meta.Method(() => FormatterServices.GetUninitializedObject(null)), Expression.Constant(type))
                .Convert(type);
        }
    }
}

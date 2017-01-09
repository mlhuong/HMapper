using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace HMapper.Dynamic
{
    internal static class DynamicModule
    {
        public static AssemblyBuilder AssemblyBuilder { get; }
        public static ModuleBuilder ModuleBuilder { get; }

        static DynamicModule()
        {
            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("HMapper.DynAssembly"), AssemblyBuilderAccess.Run);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule("HMapper.DynModule");
        }
    }
}

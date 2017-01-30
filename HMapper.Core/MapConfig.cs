using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using HMapper.Collections;

namespace HMapper
{
    /// <summary>
    /// Class that configures the mappings.
    /// </summary>
    public class MapConfig
    {
        static bool _MappingInitialized;
        static object _Mutex;
        internal static MapperAPIInitializer _MapperAPIInitializer;

        static MapConfig()
        {
            _MappingInitialized = false;
            _Mutex = new object();
            _MapperAPIInitializer = new MapperAPIInitializer();

            
        }

        /// <summary>
        /// Initialize the mappings with an action that takes an API initializer as argument.
        /// </summary>
        /// <param name="initialize">Action that takes an API initializer as argument.</param>
        public static void Initialize(Action<IMapperAPIInitializer> initialize)
        {
            lock (_Mutex)
            {
                if (_MappingInitialized)
                    throw new Exception("Mapping initialization is already done.");

                initialize(_MapperAPIInitializer);
                _MappingInitialized = true;

                // Creation of mapInfos for collections
                _MapperAPIInitializer.MapExpression<IEnumerable<TGen1>, TGen1[]>(ArrayBuilder.GetExpression);
                _MapperAPIInitializer.MapExpression<IEnumerable<TGen1>, List<TGen1>>(ListBuilder.GetExpression);
                _MapperAPIInitializer.MapExpression<IEnumerable<TGen1>, Collection<TGen1>>(CollectionBuilder.GetExpression);
                _MapperAPIInitializer.MapExpression<IEnumerable<TGen1>, HashSet<TGen1>>(HashSetBuilder.GetExpression);
                _MapperAPIInitializer.MapExpression<IDictionary<TGen1, TGen2>, Dictionary<TGen1, TGen2>>(DictionaryBuilder.GetExpression);
                _MapperAPIInitializer.ManualMap<IEnumerable, ArrayList>((listEntity) => new ArrayList(listEntity.Cast<object>().Select(item => Mapper.Map<object, object>(item)).ToArray()));

                // Validation of mapInfos.
                foreach (var mapInfo in MapInfo._CacheGenericMaps)
                    mapInfo.Validate();
            }
        }
    }
}

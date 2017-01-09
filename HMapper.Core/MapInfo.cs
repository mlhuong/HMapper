using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HMapper.Extensions;
using Metadata;

namespace HMapper
{
    /// <summary>
    /// Class storing meta information on the mapping between a source type and a target type.
    /// </summary>
    public class MapInfo
    {
        #region static fields

        /// <summary>
        /// Dictionary of tuples of Target/Source. Each value represents the mapping info for a target/source pair.
        /// </summary>        
        static internal ConcurrentDictionary<Tuple<Type,Type>, MapInfo> _CacheGenericMaps;
        static internal ConcurrentDictionary<Tuple<Type, Type, bool>, Tuple<MapInfo, Dictionary<Type, GenericAssociation>>> _CacheConcreteMaps;
        static internal List<Type> _PolymorphTypes;

        #endregion

        #region instance fields

        /// <summary>
        /// Source type.
        /// </summary>
        public Type SourceType { get; private set; }

        /// <summary>
        /// Target type.
        /// </summary>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Dictionary storing the MemberInfo of the source type (value) that is associated to a MemberInfo of the target type (key). 
        /// </summary>
        public Dictionary<MemberInfo, IMapping> Members { get; private set; }

        /// <summary>
        /// Builder used for manual mapping.
        /// </summary>
        public LambdaExpression ManualBuilder { get; internal set; }

        /// <summary>
        /// Expression builder used internally
        /// </summary>
        internal MapBuilder.MapExpressionDlg ExpressionBuilder { get; set; }

        /// <summary>
        /// Delegate to be called before the mapping starts
        /// </summary>
        public List<Delegate> BeforeMaps { get; internal set; }

        /// <summary>
        /// Delegate to be called after the mapping is done.
        /// </summary>
        public List<Delegate> AfterMaps { get; internal set; }

        /// <summary>
        /// Indicates if items cache must be used.
        /// </summary>
        public bool UseItemsCache { get; internal set; }

        #endregion 

        #region static initialization

        /// <summary>
        /// Static initialization. This is where we find the matchings between source and target objects.
        /// </summary>
        static MapInfo()
        {
            _CacheGenericMaps = new ConcurrentDictionary<Tuple<Type, Type>, MapInfo>();
            _CacheConcreteMaps = new ConcurrentDictionary<Tuple<Type, Type, bool>, Tuple<MapInfo, Dictionary<Type, GenericAssociation>>>();
            _PolymorphTypes = new List<Type>() { typeof(object) };
        }

        #endregion

        #region static methods

        /// <summary>
        /// Indicates if the generic source source is compatible with target type (using IGeneric arguments)
        /// </summary>
        /// <param name="mappedType"></param>
        /// <param name="typeToMatch"></param>
        /// <param name="genericTypeAssociation"></param>
        /// <returns></returns>
        internal static bool IsCompatibleSourceType(Type mappedType, Type typeToMatch, out Dictionary<Type, Type> genericTypeAssociation)
        {
            genericTypeAssociation = new Dictionary<Type, Type>();
            var genericArgsMappedType = mappedType.IsArray ? new[] { mappedType.GetElementType() }
                : mappedType.GetTypeInfo().IsGenericType ? mappedType.GetGenericArguments() : new Type[0];

            var genericArgsTypeToMatch = typeToMatch.IsArray ? new[] { typeToMatch.GetElementType() }
                : typeToMatch.GetTypeInfo().IsGenericType ? typeToMatch.GetGenericArguments() : new Type[0];

            if (genericArgsMappedType.Length > 0)
            {
                if (mappedType.IsArray)
                {
                    if (genericArgsTypeToMatch.Length == 0) return false;
                    genericTypeAssociation[genericArgsMappedType[0]] = genericArgsTypeToMatch[0];
                    return genericArgsMappedType[0].MakeArrayType().IsAssignableFrom(typeToMatch);
                }
                if (genericArgsTypeToMatch.Length < genericArgsMappedType.Length) return false;
                int index = 0;
                foreach (var arg in genericArgsMappedType)
                {
                    genericTypeAssociation[arg] = genericArgsTypeToMatch[index++];
                }
                var newMappedType = mappedType.GetGenericTypeDefinition().MakeGenericType(genericArgsTypeToMatch.Take(genericArgsMappedType.Length).ToArray());
                return newMappedType.IsAssignableFrom(typeToMatch);
            }

            return mappedType.IsAssignableFrom(typeToMatch);
        }

        static bool IsCompatibleTargetType(bool fillMode, TypeInfo mappedType, TypeInfo typeToMatch, Dictionary<Type, Type> sourceAssociations, out Dictionary<Type, GenericAssociation> resultAssociations)
        {
            resultAssociations = sourceAssociations.ToDictionary(x => x.Key, x => new GenericAssociation() { SourceType = x.Value });
    
            if (MatchTargetType(fillMode, mappedType, typeToMatch))
            {
                foreach (var kp in resultAssociations)
                    kp.Value.TargetType = MapInfo.Get(kp.Value.SourceType, typeof(object), fillMode).TargetType;

                return true;
            }

            if (mappedType.IsArray && typeToMatch.IsArray)
            {
                if (typeof(IGeneric).IsAssignableFrom(mappedType.GetElementType()))
                {
                    if (MatchTargetType(fillMode, sourceAssociations[mappedType.GetElementType()].GetTypeInfo(), typeToMatch.GetElementType().GetTypeInfo()))
                    {
                        resultAssociations[mappedType.GetElementType()].TargetType = typeToMatch.GetElementType();
                        return true;
                    }
                    if (MapInfo.Get(sourceAssociations[mappedType.GetElementType()], typeToMatch.GetElementType(), fillMode) == null)
                        return false;

                    resultAssociations[mappedType.GetElementType()].TargetType = typeToMatch.GetElementType();
                    return true;
                }
                return MatchTargetType(fillMode, mappedType.GetElementType().GetTypeInfo(), typeToMatch.GetElementType().GetTypeInfo());
            }

            if ((!mappedType.IsGenericType) || (!typeToMatch.IsGenericType))
                return false;

            if (mappedType.GetGenericTypeDefinition() != typeToMatch.GetGenericTypeDefinition())
            {
                if (mappedType.BaseType != null)
                {
                    if (IsCompatibleTargetType(fillMode, mappedType.BaseType.GetTypeInfo(), typeToMatch, sourceAssociations, out resultAssociations))
                        return true;
                }
                foreach (var interf in mappedType.GetInterfaces())
                {
                    if (IsCompatibleTargetType(fillMode, interf.GetTypeInfo(), typeToMatch, sourceAssociations, out resultAssociations))
                        return true;
                }
                return false;
            }

            Type[] sourceArgs = mappedType.GetGenericArguments();
            Type[] targetArgs = typeToMatch.GetGenericArguments();
            for (int i = 0; i < sourceArgs.Length; i++)
            {
                if (typeof(IGeneric).IsAssignableFrom(sourceArgs[i]))
                    resultAssociations[sourceArgs[i]].TargetType = targetArgs[i];
                else if (sourceArgs[i] != targetArgs[i])
                    return false;
            }
            return true;
        }

        private static bool MatchTargetType(bool fillMode, TypeInfo mappedType, TypeInfo typeToMatch)
        {
            if (fillMode) return mappedType.IsAssignableFrom(typeToMatch) || typeToMatch.IsAssignableFrom(mappedType);
            return typeToMatch.IsAssignableFrom(mappedType);
        }

        /// <summary>
        /// Get the most concrete MapInfo given a target type and a source type.
        /// </summary>
        /// <param name="targetType">Target type.</param>
        /// <param name="sourceType">Source type.</param>
        /// <returns></returns>
        public static MapInfo Get(Type sourceType, Type targetType, bool fillMode)
        {
            Dictionary<Type, GenericAssociation> genericTypeAssociation;
            return Get(sourceType, targetType, fillMode, out genericTypeAssociation);
        }

        /// <summary>
        /// Get the most concrete MapInfo given a target type and a source type.
        /// </summary>
        /// <param name="targetType">Target type.</param>
        /// <param name="sourceType">Source type.</param>
        /// <param name="fillMode"></param>
        /// <param name="genericTypeAssociation">Dictionary of generic types association.</param>
        /// <returns></returns>
        internal static MapInfo Get(Type sourceType, Type targetType, bool fillMode, out Dictionary<Type, GenericAssociation> genericTypeAssociation)
        {
            var result = _CacheConcreteMaps.GetOrAdd(Tuple.Create(sourceType, targetType, fillMode), t => _Get(t.Item1, t.Item2, fillMode));
            genericTypeAssociation = result?.Item2;
            return result?.Item1;
        }

        static Tuple<MapInfo, Dictionary<Type, GenericAssociation>> _Get(Type sourceType, Type targetType, bool fillMode)
        {
            var sourceTypeAssociation = new Dictionary<Type, Type>();
            var results = new List<Tuple<MapInfo, Dictionary<Type, GenericAssociation>>>();

                foreach (var kp in _CacheGenericMaps)
                {
                    if (results.Any(x => x.Item1 == kp.Value))
                        continue;

                    if (IsCompatibleSourceType(kp.Key.Item1, sourceType, out sourceTypeAssociation))
                    { 
                        Dictionary<Type, GenericAssociation> resultDic;
                        if (IsCompatibleTargetType(fillMode, kp.Key.Item2.GetTypeInfo(), targetType.GetTypeInfo(), sourceTypeAssociation, out resultDic))
                            results.Add(Tuple.Create(kp.Value, resultDic));
                    }
                }

            if (!results.Any())
            {
                if (sourceType.IsSimpleType())
                    return Tuple.Create((MapInfo)MapInfoForSimpleTypes.Get(Tuple.Create(sourceType, targetType)), new Dictionary<Type, GenericAssociation>());
                return null;
            }

            Tuple<MapInfo, Dictionary<Type, GenericAssociation>> mostConcreteResult=results.First();
            foreach (var result in results.Skip(1))
            {
                if (mostConcreteResult.Item1.SourceType.IsAssignableFrom(result.Item1.SourceType))
                    mostConcreteResult = result;
            }
            return mostConcreteResult;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Internal constructor used by static initialization.
        /// </summary>
        /// <param name="tuple"></param>
        internal MapInfo(Tuple<Type, Type> tuple, bool useItemsCache)
        {
            SourceType = tuple.Item1;
            TargetType = tuple.Item2;
            Members = new Dictionary<MemberInfo, IMapping>();
            BeforeMaps = new List<Delegate>();
            AfterMaps = new List<Delegate>();
            UseItemsCache = useItemsCache;
        }

        #endregion

        #region instance methods

        /// <summary>
        /// Method called at the end of the initialization phase.
        /// </summary>
        internal void Validate()
        {
            if (ExpressionBuilder == null && ManualBuilder == null)
            {
                var targetMembers = TargetType.GetMembers(BindingFlags.Instance | BindingFlags.Public).Where(x => x is FieldInfo || (x is PropertyInfo && (x as PropertyInfo).GetSetMethod() != null)).ToArray();

                // Get inherit member mappings and before/after map delegates
                foreach (var kp in _CacheGenericMaps.Where(x => x.Value != this))
                {
                    if (kp.Key.Item2.IsAssignableFrom(TargetType) && kp.Key.Item1.IsAssignableFrom(SourceType))
                    {
                        if (!_PolymorphTypes.Contains(kp.Key.Item1))
                            _PolymorphTypes.Add(kp.Key.Item1);

                        foreach (var member in kp.Value.Members)
                        {
                            var memberInfo = targetMembers.Single(x => x.Name == member.Key.Name);
                            if (!Members.Any(x => x.Key.Name == memberInfo.Name))
                                Members.Add(memberInfo, member.Value);
                        }
                        var baseBeforeMaps = kp.Value.BeforeMaps.ToList();
                        baseBeforeMaps.AddRange(BeforeMaps);
                        BeforeMaps = baseBeforeMaps;
                        var baseAfterMaps = kp.Value.AfterMaps.ToList();
                        baseAfterMaps.AddRange(AfterMaps);
                        AfterMaps = baseAfterMaps;
                    }
                }

                var sourceMembers = SourceType.GetMembers(BindingFlags.Instance | BindingFlags.Public).Where(x => (x is FieldInfo || x is PropertyInfo));

                foreach (var targetMember in targetMembers)
                {
                    var sourceMember = sourceMembers.SingleOrDefault(x => x.Name == targetMember.Name);
                    if (sourceMember != null && !Members.Any(x=>x.Key.Name==targetMember.Name))
                    {
                        Members.Add(targetMember, new MemberMapping(sourceMember, RetrievalMode.Default));
                    }
                }
            }
        }

        #endregion
    }
}

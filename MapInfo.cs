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
            genericTypeAssociation = new Dictionary<Type,Type>();
            if (mappedType == typeToMatch)
                return true;

            if (mappedType.IsArray && typeToMatch.IsArray)
            {
                if (typeof(IGeneric).IsAssignableFrom(mappedType.GetElementType()))
                {
                    genericTypeAssociation[mappedType.GetElementType()] = typeToMatch.GetElementType();
                    return true;
                }
                return IsCompatibleSourceType(mappedType.GetElementType(), typeToMatch.GetElementType(), out genericTypeAssociation);
            }

            if ((!mappedType.IsGenericType) || (!typeToMatch.IsGenericType))
                return false;

            if (mappedType.GetGenericTypeDefinition() != typeToMatch.GetGenericTypeDefinition())
            {
                if (typeToMatch.BaseType == null)
                    return false;
                return IsCompatibleSourceType(mappedType, typeToMatch.BaseType, out genericTypeAssociation);
            }

            Type[] sourceArgs = mappedType.GetGenericArguments();
            Type[] targetArgs = typeToMatch.GetGenericArguments();
            for (int i = 0; i < sourceArgs.Length; i++)
            {
                if (typeof(IGeneric).IsAssignableFrom(sourceArgs[i]))
                    genericTypeAssociation[sourceArgs[i]] = targetArgs[i];
                else if (sourceArgs[i] != targetArgs[i])
                    return false;
            }
            return true;
        }

        static bool IsCompatibleTargetType(bool fillMode, Type mappedType, Type typeToMatch, Dictionary<Type, Type> sourceAssociations, out Dictionary<Type, GenericAssociation> resultAssociations)
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
                    if (MatchTargetType(fillMode, sourceAssociations[mappedType.GetElementType()], typeToMatch.GetElementType()))
                    {
                        resultAssociations[mappedType.GetElementType()].TargetType = typeToMatch.GetElementType();
                        return true;
                    }
                    if (MapInfo.Get(sourceAssociations[mappedType.GetElementType()], typeToMatch.GetElementType(), fillMode) == null)
                        return false;

                    resultAssociations[mappedType.GetElementType()].TargetType = typeToMatch.GetElementType();
                    return true;
                }
                return MatchTargetType(fillMode, mappedType.GetElementType(), typeToMatch.GetElementType());
            }

            if ((!mappedType.IsGenericType) || (!typeToMatch.IsGenericType))
                return false;

            if (mappedType.GetGenericTypeDefinition() != typeToMatch.GetGenericTypeDefinition())
            {
                if (mappedType.BaseType != null)
                {
                    if (IsCompatibleTargetType(fillMode, mappedType.BaseType, typeToMatch, sourceAssociations, out resultAssociations))
                        return true;
                }
                foreach (var interf in mappedType.GetInterfaces())
                {
                    if (IsCompatibleTargetType(fillMode, interf, typeToMatch, sourceAssociations, out resultAssociations))
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

        private static bool MatchTargetType(bool fillMode, Type mappedType, Type typeToMatch)
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
            var tuple = _Get(sourceType, targetType, fillMode);
            genericTypeAssociation = tuple?.Item2;
            return tuple?.Item1;
        }

        static Tuple<MapInfo, Dictionary<Type, GenericAssociation>> _Get(Type sourceType, Type targetType, bool fillMode)
        {
            Type tempSource = sourceType;
            var sourceTypeAssociation = new Dictionary<Type, Type>();
            var results = new List<Tuple<MapInfo, Dictionary<Type, GenericAssociation>>>();
            while (tempSource != null)
            {
                foreach (var kp in _CacheGenericMaps)
                {
                    if (results.Any(x => x.Item1 == kp.Value))
                        continue;
                    bool matched = false;
                    if (!IsCompatibleSourceType(kp.Key.Item1, tempSource, out sourceTypeAssociation))
                    {
                        foreach (Type interf in tempSource.GetInterfaces())
                        {
                            if (IsCompatibleSourceType(kp.Key.Item1, interf, out sourceTypeAssociation))
                            {
                                matched = true;
                                break;
                            }
                        }
                    }
                    else
                        matched = true;

                    if (matched)
                    {
                        Dictionary<Type, GenericAssociation> resultDic;
                        if (IsCompatibleTargetType(fillMode, kp.Key.Item2, targetType, sourceTypeAssociation, out resultDic))
                            results.Add(Tuple.Create(kp.Value, resultDic));
                    }
                }

                tempSource = tempSource.BaseType;
            }
            if (!results.Any())
                return null;

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

using System;
using System.Linq;

namespace HMapper.Tests
{
    public static class InitMappings
    {
        public static void Init()
        {
            MapConfig.Initialize(initializer =>
            {
                initializer.Map<Business.VerySimpleClass, DTO.VerySimpleClass>();
                initializer.Map<Business.SimpleClass, DTO.SimpleClass>()
                    .WithMember(x => x.Key, api => api.LinkTo(x => x.Id))
                    .WithMember(x => x.Date_Plus_2, api => api.LinkTo(x => x.Date.AddDays(2)))
                    .WithMember(nameof(DTO.SimpleClass.StringToBeIgnored), api => api.Ignore())
                    .WithMember(x => x.VerySimpleClass2String, api => api.LinkTo(x => x.VerySimpleClass2.MyString));
                initializer.Map<Business.SimpleClass, DTO.SimpleClass2>()
                    .WithMember(x => x.Key, api => api.LinkTo(nameof(Business.SimpleClass.Id)));
                initializer.Map<int[], DTO.ClassWithComplexFuncMappings>()
                    .WithMember(x => x.VerySimpleClasses, api => api.LinkTo(arr => arr.Select(item => new Business.VerySimpleClass() { MyInt = item, MyString = item.ToString() }).ToArray()))
                    .WithMember(x => x.VerySimpleClass, api => api.LinkTo(arr => new DTO.VerySimpleClass() { MyInt = 77, MyString = "test" }))
                    .WithMember(x => x.ADate, api => api.LinkTo(arr => ((DTO.SimpleClass)null).Date))
                    .WithMember(x => x.AStruct, api => api.LinkTo(arr => ((DTO.ClassWithStructAndEnum)null).AStruct))
                    .WithMember(x => x.AString, api => api.LinkTo(arr => (DTO.ClassWithComplexFuncMappings.AFunction().FirstOrDefault(x => x.MyInt == 5).MyString)))
                    .WithMember(x => x.AnotherString, api => api.LinkTo(arr => ((DTO.ClassWithStructAndEnum)null).AStruct.Field1))
                    .WithMember(x => x.AnInt, api => api.LinkTo(arr => (DTO.ClassWithComplexFuncMappings.AFunction().FirstOrDefault(x => x.MyInt == 5).MyInt)))
                    .WithMember(x => x.AnInt2, api => api.LinkTo(arr => ((DTO.SimpleClass)null).GetHashCode()));
                initializer.Map<Business.SimpleSet, DTO.SimpleSet>();
                initializer.Map<Business.MultipleSets, DTO.MultipleSets>();
                initializer.Map<Business.DictionarySet, DTO.DictionarySet>();
                initializer.Map<Business.DictionarySetCircular, DTO.DictionarySetCircular>()
                    .EnableItemsCache();
                initializer.Map<Business.ClassWithNullableTypes, DTO.ClassWithNullableTypes>();
                initializer.Map<Business.SimpleGeneric<TGen1>, DTO.SimpleGeneric<TGen1>>();
                initializer.Map<Business.SimpleGeneric2<TGen1>, DTO.SimpleGeneric2<TGen1>>()
                    .WithMember(x => x.AnotherGenericProperty, api => api.LinkTo(x => x.GenericProperty))
                    .WithMember(x => x.ToBeIngored, api => api.Ignore());
                initializer.Map<Business.MappedObjectGeneric<TGen1>, DTO.MappedObjectGeneric<TGen1>>();
                initializer.Map<Business.MultipleGenerics<TGen2, TGen1>, DTO.MultipleGenerics<TGen1, TGen2>>();
                initializer.Map<Business.PolymorphicSubClass, DTO.PolymorphicSubClass>();
                initializer.Map<Business.PolymorphicBaseClass, DTO.PolymorphicBaseClass>();
                initializer.Map<Business.IInterfaceForFuncMappingPolymorph, DTO.FuncMappingPolymorphSub>()
                    .WithMember(x => x.FromInterface, api => api.LinkTo(x => x.IntFromInterface));
                initializer.Map<Business.FuncMappingPolymorph, DTO.FuncMappingPolymorph>()
                    .WithMember(x => x.AString, api => api.LinkTo(x => x.MyString.ToUpper()));
                initializer.Map<Business.FuncMappingPolymorphSub, DTO.FuncMappingPolymorphSub>()
                    .WithMember(x => x.ADate, api => api.LinkTo(x => x.ADate.AddDays(1)));
                initializer.Map<Business.SetOfPolymorphic, DTO.SetOfPolymorphic>();
                initializer.Map<Business.SetOfGenericPolymorph, DTO.SetOfGenericPolymorph>();
                initializer.Map<Business.AutoReferencedClass, DTO.AutoReferencedClass>()
                    .EnableItemsCache();
                initializer.Map<Business.ContainerOfManuallyMappedClass, DTO.ContainerOfManuallyMappedClass>()
                    .WithMember(x => x.Tuple, api => api.LinkTo(x => x.Content));
                initializer.ManualMap((Business.ContainerOfManuallyMappedClass.ManuallyMappedClass x) => Tuple.Create(x.Id, x.Title));
                initializer.Map<Business.ClassWithStructAndEnum.MyStruct, DTO.ClassWithStructAndEnum.MyStruct>()
                    .WithMember(x => x.AnotherField2, api => api.LinkTo(x => x.Field2));
                initializer.Map<Business.ClassWithStructAndEnum.MyEnum1, DTO.ClassWithStructAndEnum.MyEnum1>();
                initializer.ManualMap((Business.ClassWithStructAndEnum.MyEnum2 val) => DTO.ClassWithStructAndEnum.Convert(val));
                initializer.Map<Business.ClassWithStructAndEnum, DTO.ClassWithStructAndEnum>();
                initializer.Map<Business.ClassWithBeforeAndAfterMap, DTO.ClassWithBeforeAndAfterMap>()
                    .BeforeMap((source, target) => { source.Name = "beforeMap"; })
                    .AfterMap((source, target) => { source.Name = "afterMap"; });
                initializer.Map<Business.ClassWithBeforeAndAfterMapSub, DTO.ClassWithBeforeAndAfterMapSub>()
                    .WithMember(x => x.AnInt, api => api.Ignore())
                    .BeforeMap((source, target) => { source.StringFromSub = "beforeMap"; })
                    .AfterMap((source, target) => { source.StringFromSub = "afterMap"; });
                initializer.Map<Business.ClassForInclusions, DTO.ClassForInclusions>()
                    .WithMember(x => x.AString, api => api.LinkTo(x => x.AString, RetrievalMode.RetrievedWhenSpecified))
                    .WithMember(x => x.ADate, api => api.LinkTo(x => DateTime.Today, RetrievalMode.RetrievedWhenSpecified));
                initializer.Map<Business.ClassWithSetOfUnmappedClass, DTO.ClassWithSetOfUnmappedClass>();
                initializer.Map<Business.ClassWithSimpleTypes, DTO.ClassWithSimpleTypes>();
            });

            
        }
    }
}

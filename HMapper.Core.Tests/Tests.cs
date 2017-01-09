using System;
using System.Collections.Generic;
using System.Linq;
using HMapper.Tests.Business;
using Xunit;

namespace HMapper.Tests
{
    public class Tests : IClassFixture<MapperFixture>
    {
        [Fact]
        public void TestVerySimpleObject()
        {
            Run(VerySimpleClass.Create(), x => new DTO.VerySimpleClass(x));
        }

        [Fact]
        public void TestSimpleObject()
        {
            // We have 2 target types mapped to the same source type.
            Run(SimpleClass.Create(5), x => new DTO.SimpleClass(x));
            Run(SimpleClass.Create(6), x => new DTO.SimpleClass2(x));
        }

        [Fact]
        public void TestSimpleObjectWithInclude()
        {
            var source = MultipleSets.Create(5);
            var myDTO = Mapper.Map<MultipleSets, DTO.MultipleSets>(source, x => x.SimpleClasses.Select(y => y.IntArray), x => x.IntegerSet);

            var manual = new DTO.MultipleSets()
            {
                Id = source.Id,
                IntegerSet = new HashSet<int>(source.IntegerSet),
                SimpleClasses = source.SimpleClasses.Select(p => { var res = new DTO.SimpleClass(p); res.VerySimpleClass = null; return res; }).ToList()
            };
            Assert.Equal(myDTO, manual);
        }

        [Fact]
        public void TestSimpleObjectWithNoInclude()
        {
            var source = MultipleSets.Create(5);
            var myDTO = Mapper.Map<MultipleSets, DTO.MultipleSets>(source, null);

            var manual = new DTO.MultipleSets()
            {
                Id = source.Id
            };
            Assert.Equal(myDTO, manual);
        }

        [Fact]
        public void TestSimpleObjectWithExclude()
        {
            var source = MultipleSets.Create(5);
            var myDTO = Mapper.MapExclude<MultipleSets, DTO.MultipleSets>(source, x => x.SimpleClasses.Select(y => y.IntArray));
            var manual = new DTO.MultipleSets(source);
            manual.SimpleClasses.ForEach(x => x.IntArray = null);

            Assert.Equal(myDTO, manual);
        }


        [Fact]
        public void TestSimpleCollection()
        {
            var source = SimpleSet.Create(10);
            source.VerySimpleClasses[0] = null;
            Run(source, x => new DTO.SimpleSet(x));
        }

        [Fact]
        public void TestMultipleCollections()
        {
            var tmp = Mapper.Map<int, int>(5);
            Run(MultipleSets.Create(1), x => new DTO.MultipleSets(x));
        }

        [Fact]
        public void TestDictionaries()
        {
            Run(DictionarySet.Create(1), x => new DTO.DictionarySet(x));
        }

        [Fact]
        public void TestDictionariesCircularReferences()
        {
            var source = DictionarySetCircular.Create();
            var myDTO = Mapper.Map<DictionarySetCircular, DTO.DictionarySetCircular>(source);
            Assert.Equal(myDTO.Id, source.Id);
            Assert.True(myDTO.DictObjects != null);
            Assert.True(myDTO.DictObjects.Count == 1);
            Assert.True(myDTO.DictObjects.First().Key == myDTO);
            Assert.True(myDTO.DictObjects.First().Value == source.DictObjects.First().Value);
        }

        [Fact]
        public void TestNullableTypes()
        {
            var source = ClassWithNullableTypes.Create();
            Run(source, x => new DTO.ClassWithNullableTypes(x));

            var DTO = new DTO.ClassWithNullableTypes() { Int1 = 5 };
            var manual = new DTO.ClassWithNullableTypes(source);
            Mapper.Fill(source, DTO);
            Assert.Equal(DTO, manual);
        }

        [Fact]
        public void TestSimpleGenericOfInt()
        {
            Run(SimpleGeneric<int>.Create(1, 100), x => new DTO.SimpleGeneric<int>(x));
            Run(SimpleGeneric2<int>.Create(2, 200), x => new DTO.SimpleGeneric2<int>(x));
        }

        [Fact]
        public void TestMappedObjectGeneric()
        {
            Run(MappedObjectGeneric<VerySimpleClass>.Create(1, VerySimpleClass.Create()), x => new DTO.MappedObjectGeneric<DTO.VerySimpleClass>(x));
        }

        [Fact]
        public void TestMultipleGenerics()
        {
            Run(MultipleGenerics<int, string>.Create(1, 5, "some string"), x => new DTO.MultipleGenerics<string, int>(x));
        }

        [Fact]
        public void TestPolymorphicClass()
        {
            Run(PolymorphicSubSubClass.Create(1), x => new DTO.PolymorphicSubClass(x));
            Run<PolymorphicBaseClass, DTO.PolymorphicBaseClass>(PolymorphicSubSubClass.Create(1), x => new DTO.PolymorphicSubClass((PolymorphicSubSubClass)x));
            Run(PolymorphicSubSubClass.Create(1), x => DTO.PolymorphicBaseClass.GetMostConcrete(x));
        }

        [Fact]
        public void TestPolymorphicInterface()
        {
            Run<PolymorphicBaseClass, DTO.IPolymorphic>(PolymorphicSubSubClass.Create(1), x => new DTO.PolymorphicSubClass((PolymorphicSubClass)x));
        }

        [Fact]
        public void TestTargetAsInterface()
        {
            var source = SimpleClass.Create(5);
            var myDTO = Mapper.Map<SimpleClass, DTO.ISimpleClass>(source);
            var manual = new DTO.SimpleClass(source);
            Assert.Equal(myDTO, manual);
        }

        [Fact]
        public void TestSetOfPolymorphicClass()
        {
            Run(SetOfPolymorphic.Create(1), x => new DTO.SetOfPolymorphic(x));
        }

        [Fact]
        public void TestGenericOfPolymorphicClass()
        {
            Run(MappedObjectGeneric<PolymorphicBaseClass>.Create(1, PolymorphicBaseClass.Create(2)), x => new DTO.MappedObjectGeneric<DTO.PolymorphicBaseClass>(x));
            Run(MappedObjectGeneric<PolymorphicBaseClass>.Create(1, PolymorphicSubSubClass.Create(2)), x => new DTO.MappedObjectGeneric<DTO.PolymorphicBaseClass>(x));
        }

        [Fact]
        public void TestSetOfGenericPolymorph()
        {
            Run(SetOfGenericPolymorph.Create(1), x => new DTO.SetOfGenericPolymorph(x));
        }

        [Fact]
        public void TestAutoReferencedClasses()
        {
            var source = AutoReferencedClass.Create(1);
            var myDTO = Mapper.Map<AutoReferencedClass, DTO.AutoReferencedClass>(AutoReferencedClass.Create(1));
            var manual = DTO.AutoReferencedClass.Create(source);
            Assert.Equal(myDTO, manual);
            Assert.True(myDTO.Parent == null);
            Assert.True(myDTO.Child?.Parent == myDTO);
        }
        
        [Fact]
        public void TestManualMap()
        {
            Run(new ContainerOfManuallyMappedClass.ManuallyMappedClass() { Id = 5, Title = "test" }, x => Tuple.Create(x.Id, x.Title));

            Run(ContainerOfManuallyMappedClass.Create(1), x => new DTO.ContainerOfManuallyMappedClass(x));
        }

        [Fact]
        public void TestFillMap()
        {
            var source = SimpleClass.Create(1);
            var targetDTO = new DTO.SubSimpleClass() { AdditionalInfo = "addInfo" };
            Mapper.Fill<SimpleClass, DTO.SimpleClass>(source, targetDTO);
            var manual = new DTO.SubSimpleClass(source, "addInfo");
            Assert.Equal(targetDTO, manual);

            targetDTO = new DTO.SubSimpleClass() { AdditionalInfo = "addInfo" };
            Mapper.Fill<object, object>(source, targetDTO);
            Assert.Equal(targetDTO, manual);

            targetDTO = new DTO.SubSimpleClass() { AdditionalInfo = "addInfo" };
            Mapper.Fill<SimpleClass, DTO.SimpleClass>(source, targetDTO, x => x.VerySimpleClass);
            manual.IntArray = null;
            Assert.Equal(targetDTO, manual);

            targetDTO = new DTO.SubSimpleClass() { AdditionalInfo = "addInfo" };
            Mapper.Fill<SimpleClass, DTO.SimpleClass>(source, targetDTO, x => x.VerySimpleClass);
            Assert.Equal(targetDTO, manual);
        }

        [Fact]
        public void TestFillMapPolymorph()
        {
            var source = PolymorphicSubClass.Create(5);
            var manual = new DTO.PolymorphicSubClass(source);

            var dto = new DTO.PolymorphicSubClass();
            Mapper.Fill(source, dto);
            Assert.Equal(dto, manual);

            dto = new DTO.PolymorphicSubClass();
            Mapper.Fill<PolymorphicBaseClass, DTO.PolymorphicBaseClass>(source, dto);
            Assert.Equal(dto, manual);

            dto = new DTO.PolymorphicSubClass();
            Mapper.Fill<PolymorphicBaseClass, object>(source, dto);
            Assert.Equal(dto, manual);

            var dto2 = new DTO.PolymorphicBaseClass();
            Mapper.Fill<PolymorphicBaseClass, object>(source, dto2);
            var manual2 = new DTO.PolymorphicBaseClass(source);
            Assert.Equal(dto2, manual2);

            var dto3 = new DTO.PolymorphicSubSubClass();
            Mapper.Fill(source, dto3);
            dto = new DTO.PolymorphicSubClass() { Id = dto3.Id, AString = dto3.AString, Name = dto3.Name };
            Assert.Equal(dto, manual);
        }


        [Fact]
        public void TestMapList()
        {
            var source = VerySimpleClass.CreateMany(5);
            var MyDTO  = Mapper.Map<IEnumerable<VerySimpleClass>, IEnumerable<DTO.VerySimpleClass>>(source);
            var manual = source.Select(x => new DTO.VerySimpleClass(x)).ToArray();
            Assert.True(MyDTO.EnumerableEquals(manual));
        }

        [Fact]
        public void TestComplexFuncProperty()
        {
            var arr = new int[] { 1, 2, 3 };
            var MyDTO = Mapper.Map<int[], DTO.ClassWithComplexFuncMappings>(arr);
            var manual = new DTO.ClassWithComplexFuncMappings()
            {
                ADate = default(DateTime),
                VerySimpleClasses = arr.Select(item => new DTO.VerySimpleClass(new VerySimpleClass() { MyInt = item, MyString = item.ToString() })).ToArray(),
                VerySimpleClass = new DTO.VerySimpleClass() { MyInt = 77, MyString = "test" }
            };
            Assert.Equal(MyDTO, manual);
        }

        [Fact]
        public void TestStructEnums()
        {
            Run(ClassWithStructAndEnum.Create(), x => new DTO.ClassWithStructAndEnum(x));
        }

        [Fact]
        public void TestBeforeAfterMap()
        {
            var source = ClassWithBeforeAndAfterMap.Create();
            Run(source, x => new DTO.ClassWithBeforeAndAfterMap()
            {
                Id = source.Id,
                Name = "beforeMap",
                AnInt = source.AnInt
            });
            Assert.True(source.Name == "afterMap");
        }

        [Fact]
        public void TestBeforeAfterMapWithPolymorph()
        {
            var source = ClassWithBeforeAndAfterMapSub.Create();
            Run(source, x => new DTO.ClassWithBeforeAndAfterMapSub()
            {
                Id = source.Id,
                Name = "beforeMap",
                StringFromSub = "beforeMap"
            });
            Assert.True(source.Name == "afterMap");
            Assert.True(source.StringFromSub == "afterMap");
        }

        [Fact]
        public void TestFuncMappingWithPolymorph()
        {
            Run(FuncMappingPolymorphSub.Create(5), x => new DTO.FuncMappingPolymorphSub(x));
        }

        [Fact]
        public void TestInclusions()
        {
            var source = ClassForInclusions.Create();
            var dto = Mapper.Map<ClassForInclusions, DTO.ClassForInclusions>(source, x => x.ADate);
            var manual = new DTO.ClassForInclusions(source);
            manual.AString = null;
            Assert.Equal(dto, manual);

            dto = Mapper.Map<ClassForInclusions, DTO.ClassForInclusions>(source, x => x.AString);
            manual = new DTO.ClassForInclusions(source);
            manual.ADate = default(DateTime);
            Assert.Equal(dto, manual);
        }

        [Fact]
        public void TestSetOfUnmapped()
        {
            Run(ClassWithSetOfUnmappedClass.Create(), x => new DTO.ClassWithSetOfUnmappedClass(x));

        }

        private void Run<TSource, TTarget>(TSource source, Func<TSource, TTarget> targetFactory)
        {
            var myDTO = Mapper.Map<TSource,TTarget>(source);
            var manual = targetFactory(source);
            Assert.Equal(myDTO, manual);
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using HMapper.Tests.Business;

namespace HMapper.Tests
{
    [TestClass]
    public class Tests
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            InitMappings.Init();
        }

        [TestMethod]
        public void TestVerySimpleObject()
        {
            Run(VerySimpleClass.Create(), x => new DTO.VerySimpleClass(x));
        }

        [TestMethod]
        public void TestSimpleObject()
        {
            // We have 2 target types mapped to the same source type.
            Run(SimpleClass.Create(5), x => new DTO.SimpleClass(x));
            Run(SimpleClass.Create(6), x => new DTO.SimpleClass2(x));
        }

        [TestMethod]
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
            Assert.AreEqual(myDTO, manual);
        }

        [TestMethod]
        public void TestSimpleObjectWithNoInclude()
        {
            var source = MultipleSets.Create(5);
            var myDTO = Mapper.Map<MultipleSets, DTO.MultipleSets>(source, null);

            var manual = new DTO.MultipleSets()
            {
                Id = source.Id
            };
            Assert.AreEqual(myDTO, manual);
        }

        [TestMethod]
        public void TestSimpleObjectWithExclude()
        {
            var source = MultipleSets.Create(5);
            var myDTO = Mapper.MapExclude<MultipleSets, DTO.MultipleSets>(source, x => x.SimpleClasses.Select(y => y.IntArray));
            var manual = new DTO.MultipleSets(source);
            manual.SimpleClasses.ForEach(x => x.IntArray = null);

            Assert.AreEqual(myDTO, manual);
        }


        [TestMethod]
        public void TestSimpleCollection()
        {
            var source = SimpleSet.Create(10);
            source.VerySimpleClasses[0] = null;
            Run(source, x => new DTO.SimpleSet(x));
        }

        [TestMethod]
        public void TestMultipleCollections()
        {
            var tmp = Mapper.Map<int, int>(5);
            Run(MultipleSets.Create(1), x => new DTO.MultipleSets(x));
        }

        [TestMethod]
        public void TestDictionaries()
        {
            Run(DictionarySet.Create(1), x => new DTO.DictionarySet(x));
        }

        [TestMethod]
        public void TestDictionariesCircularReferences()
        {
            var source = DictionarySetCircular.Create();
            var myDTO = Mapper.Map<DictionarySetCircular, DTO.DictionarySetCircular>(source);
            Assert.AreEqual(myDTO.Id, source.Id);
            Assert.IsTrue(myDTO.DictObjects != null);
            Assert.IsTrue(myDTO.DictObjects.Count == 1);
            Assert.IsTrue(myDTO.DictObjects.First().Key == myDTO);
            Assert.IsTrue(myDTO.DictObjects.First().Value == source.DictObjects.First().Value);
        }

        [TestMethod]
        public void TestNullableTypes()
        {
            var source = ClassWithNullableTypes.Create();
            Run(source, x => new DTO.ClassWithNullableTypes(x));

            var DTO = new DTO.ClassWithNullableTypes() { Int1 = 5 };
            var manual = new DTO.ClassWithNullableTypes(source);
            Mapper.Fill(source, DTO);
            Assert.AreEqual(DTO, manual);
        }

        [TestMethod]
        public void TestSimpleTypes()
        {
            Run(ClassWithSimpleTypes.Create(), x => new DTO.ClassWithSimpleTypes(x));
        }

        [TestMethod]
        public void TestSimpleGenericOfInt()
        {
            Run(SimpleGeneric<int>.Create(1, 100), x => new DTO.SimpleGeneric<int>(x));
            Run(SimpleGeneric2<int>.Create(2, 200), x => new DTO.SimpleGeneric2<int>(x));
        }

        [TestMethod]
        public void TestMappedObjectGeneric()
        {
            Run(MappedObjectGeneric<VerySimpleClass>.Create(1, VerySimpleClass.Create()), x => new DTO.MappedObjectGeneric<DTO.VerySimpleClass>(x));
        }

        [TestMethod]
        public void TestMultipleGenerics()
        {
            Run(MultipleGenerics<int, string>.Create(1, 5, "some string"), x => new DTO.MultipleGenerics<string, int>(x));
        }

        [TestMethod]
        public void TestPolymorphicClass()
        {
            Run(PolymorphicSubSubClass.Create(1), x => new DTO.PolymorphicSubSubClass(x));
            Run<PolymorphicBaseClass, DTO.PolymorphicBaseClass>(PolymorphicSubSubClass.Create(1), x => new DTO.PolymorphicSubSubClass((PolymorphicSubSubClass)x));
        }

        [TestMethod]
        public void TestPolymorphicInterface()
        {
            Run<PolymorphicBaseClass, DTO.IPolymorphic>(PolymorphicSubSubClass.Create(1), x => new DTO.PolymorphicSubSubClass((PolymorphicSubSubClass)x));
        }

        [TestMethod]
        public void TestTargetAsInterface()
        {
            var source = SimpleClass.Create(5);
            var myDTO = Mapper.Map<SimpleClass, DTO.ISimpleClass>(source);
            var manual = new DTO.SimpleClass(source);
            Assert.AreEqual(myDTO, manual);
        }

        [TestMethod]
        public void TestSetOfPolymorphicClass()
        {
            Run(SetOfPolymorphic.Create(1), x => new DTO.SetOfPolymorphic(x));
        }

        [TestMethod]
        public void TestGenericOfPolymorphicClass()
        {
            Run(MappedObjectGeneric<PolymorphicBaseClass>.Create(1, PolymorphicBaseClass.Create(2)), x => new DTO.MappedObjectGeneric<DTO.PolymorphicBaseClass>(x));
            //Run(MappedObjectGeneric<PolymorphicBaseClass>.Create(1, PolymorphicSubSubClass.Create(2)), x => new DTO.MappedObjectGeneric<DTO.PolymorphicBaseClass>(x));
        }

        [TestMethod]
        public void TestSetOfGenericPolymorph()
        {
            Run(SetOfGenericPolymorph.Create(1), x => new DTO.SetOfGenericPolymorph(x));
        }

        [TestMethod]
        public void TestAutoReferencedClasses()
        {
            var source = AutoReferencedClass.Create(1);
            var myDTO = Mapper.Map<AutoReferencedClass, DTO.AutoReferencedClass>(AutoReferencedClass.Create(1));
            var manual = DTO.AutoReferencedClass.Create(source);
            Assert.AreEqual(myDTO, manual);
            Assert.IsTrue(myDTO.Parent == null);
            Assert.IsTrue(myDTO.Child?.Parent == myDTO);
        }

        [TestMethod]
        public void TestManualMap()
        {
            Run(new ContainerOfManuallyMappedClass.ManuallyMappedClass() { Id = 5, Title = "test" }, x => Tuple.Create(x.Id, x.Title));

            Run(ContainerOfManuallyMappedClass.Create(1), x => new DTO.ContainerOfManuallyMappedClass(x));
        }

        [TestMethod]
        public void TestFillMap()
        {
            var source = SimpleClass.Create(1);
            var targetDTO = new DTO.SubSimpleClass() { AdditionalInfo = "addInfo" };
            Mapper.Fill<SimpleClass, DTO.SimpleClass>(source, targetDTO);
            var manual = new DTO.SubSimpleClass(source, "addInfo");
            Assert.AreEqual(targetDTO, manual);

            targetDTO = new DTO.SubSimpleClass() { AdditionalInfo = "addInfo" };
            Mapper.Fill<object, object>(source, targetDTO);
            Assert.AreEqual(targetDTO, manual);

            targetDTO = new DTO.SubSimpleClass() { AdditionalInfo = "addInfo" };
            Mapper.Fill<SimpleClass, DTO.SimpleClass>(source, targetDTO, x => x.VerySimpleClass);
            manual.IntArray = null;
            Assert.AreEqual(targetDTO, manual);

            targetDTO = new DTO.SubSimpleClass() { AdditionalInfo = "addInfo" };
            Mapper.Fill<SimpleClass, DTO.SimpleClass>(source, targetDTO, x => x.VerySimpleClass);
            Assert.AreEqual(targetDTO, manual);
        }

        [TestMethod]
        public void TestFillMapPolymorph()
        {
            var source = PolymorphicSubClass.Create(5);
            var manual = new DTO.PolymorphicSubClass(source);

            var dto = new DTO.PolymorphicSubClass();
            Mapper.Fill(source, dto);
            Assert.AreEqual(dto, manual);

            dto = new DTO.PolymorphicSubClass();
            Mapper.Fill<PolymorphicBaseClass, DTO.PolymorphicBaseClass>(source, dto);
            Assert.AreEqual(dto, manual);

            dto = new DTO.PolymorphicSubClass();
            Mapper.Fill<PolymorphicBaseClass, object>(source, dto);
            Assert.AreEqual(dto, manual);

            var dto2 = new DTO.PolymorphicBaseClass();
            Mapper.Fill<PolymorphicBaseClass, object>(source, dto2);
            var manual2 = new DTO.PolymorphicBaseClass
            {
                Id = source.Id,
                // AString is still filled according to the subclass mapping
                AString = source.AString.ToUpper()
            };
            Assert.AreEqual(dto2, manual2);

            var dto3 = new DTO.PolymorphicSubSubClass();
            Mapper.Fill(source, dto3);
            dto = new DTO.PolymorphicSubClass() { Id = dto3.Id, AString = dto3.AString, Name = dto3.Name };
            Assert.AreEqual(dto, manual);
        }


        [TestMethod]
        public void TestMapList()
        {
            var source = VerySimpleClass.CreateMany(5);
            var MyDTO = Mapper.Map<IEnumerable<VerySimpleClass>, IEnumerable<DTO.VerySimpleClass>>(source);
            var manual = source.Select(x => new DTO.VerySimpleClass(x)).ToArray();
            Assert.IsTrue(MyDTO.EnumerableEquals(manual));
        }

        [TestMethod]
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
            Assert.AreEqual(MyDTO, manual);
        }

        [TestMethod]
        public void TestStructEnums()
        {
            Run(ClassWithStructAndEnum.Create(), x => new DTO.ClassWithStructAndEnum(x));
        }

        [TestMethod]
        public void TestBeforeAfterMap()
        {
            var source = ClassWithBeforeAndAfterMap.Create();
            Run(source, x => new DTO.ClassWithBeforeAndAfterMap()
            {
                Id = source.Id,
                Name = "beforeMap",
                AnInt = source.AnInt
            });
            Assert.IsTrue(source.Name == "afterMap");
        }

        [TestMethod]
        public void TestBeforeAfterMapWithPolymorph()
        {
            var source = ClassWithBeforeAndAfterMapSub.Create();
            Run(source, x => new DTO.ClassWithBeforeAndAfterMapSub()
            {
                Id = source.Id,
                Name = "beforeMap",
                StringFromSub = "beforeMap"
            });
            Assert.IsTrue(source.Name == "afterMap");
            Assert.IsTrue(source.StringFromSub == "afterMap");
        }

        [TestMethod]
        public void TestFuncMappingWithPolymorph()
        {
            Run(FuncMappingPolymorphSub.Create(5), x => new DTO.FuncMappingPolymorphSub(x));
        }

        [TestMethod]
        public void TestInclusions()
        {
            var source = ClassForInclusions.Create();
            var dto = Mapper.Map<ClassForInclusions, DTO.ClassForInclusions>(source, x => x.ADate);
            var manual = new DTO.ClassForInclusions(source);
            manual.AString = null;
            Assert.AreEqual(dto, manual);

            dto = Mapper.Map<ClassForInclusions, DTO.ClassForInclusions>(source, x => x.AString);
            manual = new DTO.ClassForInclusions(source);
            manual.ADate = default(DateTime);
            Assert.AreEqual(dto, manual);
        }

        [TestMethod]
        public void TestSetOfUnmapped()
        {
            Run(ClassWithSetOfUnmappedClass.Create(), x => new DTO.ClassWithSetOfUnmappedClass(x));

        }

        private void Run<TSource, TTarget>(TSource source, Func<TSource, TTarget> targetFactory)
        {
            var myDTO = Mapper.Map<TSource, TTarget>(source);
            var manual = targetFactory(source);
            Assert.AreEqual(myDTO, manual);
        }
    }
}

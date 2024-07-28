using Comfast.Commons.Utils;
using FluentAssertions;

namespace EasyDriver.Tests.Unit;

public class ReflectionUtilTest {
    private readonly TestClass _test = new();
    private readonly ParentClass _parent = new();

    class TestClass {
        public string PublicProp { get; set; } = "PublicProp value";
        protected string ProtectedProp { get; set; } = "ProtectedProp value";
        private string PrivateProp { get; set; } = "PrivateProp value";
        private string ReadonlyProp { get; } = "ReadonlyProp value";
        private string ReadonlyPrivateProp { get; } = "ReadonlyPrivateProp value";
        private string ReadonlyMethodPrivateProp => "ReadonlyMethodPrivateProp value";
        public string PublicField = "PublicField value";
        protected string ProtectedField = "ProtectedField value";
        private string _privateField = "_privateField value";
        private NestedClass _nested = new();
        public NestedClass? nullClass = null;
        public string? nullField = null;
        private string NoSetterProp { get; } = "NoSetterProp value";
    }

    class ParentClass {
        public TestClass TestProp { get; set; } = new();
        private TestClass TestPrivateProp { get; } = new();
        private TestClass TestMethodProp => new();
        public TestClass TestReadonlyProp { get; } = new();
        private TestClass TestReadonlyPrivateProp { get; } = new();
        private TestClass TestReadonlyMethodProp => new();
        public TestClass TestField = new();
        private TestClass _testPrivateField = new();
    }

    class NestedClass {
        private string ReadonlyPrivateProp { get; } = "ReadonlyPrivateProp value";
        private string PublicProp { get; set; } = "PublicProp value";
    }

    [Fact] void ReadFieldTest() {
        //@formatter:off
        ShouldReadField(_test, "PublicProp",                "PublicProp value");
        ShouldReadField(_test, "ProtectedProp",             "ProtectedProp value");
        ShouldReadField(_test, "PrivateProp",               "PrivateProp value");
        ShouldReadField(_test, "ReadonlyProp",              "ReadonlyProp value");
        ShouldReadField(_test, "ReadonlyPrivateProp",       "ReadonlyPrivateProp value");
        ShouldReadField(_test, "ReadonlyMethodPrivateProp", "ReadonlyMethodPrivateProp value");
        ShouldReadField(_test, "PublicField",               "PublicField value");
        ShouldReadField(_test, "ProtectedField",            "ProtectedField value");
        ShouldReadField(_test, "_privateField",             "_privateField value");
        //@formatter:on
    }

    [Fact] void ReadNestedField() {
        //@formatter:off
        ShouldReadField(_parent, "TestProp._privateField",                "_privateField value");
        ShouldReadField(_parent, "TestPrivateProp._privateField",         "_privateField value");
        ShouldReadField(_parent, "TestMethodProp._privateField",          "_privateField value");
        ShouldReadField(_parent, "TestReadonlyProp._privateField",        "_privateField value");
        ShouldReadField(_parent, "TestReadonlyPrivateProp._privateField", "_privateField value");
        ShouldReadField(_parent, "TestReadonlyMethodProp._privateField",  "_privateField value");
        ShouldReadField(_parent, "TestField._privateField",               "_privateField value");
        ShouldReadField(_parent, "_testPrivateField._privateField",       "_privateField value");
        ShouldReadField(_parent, "TestProp.PrivateProp",                  "PrivateProp value");
        ShouldReadField(_parent, "TestPrivateProp.PrivateProp",           "PrivateProp value");
        ShouldReadField(_parent, "TestMethodProp.PrivateProp",            "PrivateProp value");
        ShouldReadField(_parent, "TestReadonlyProp.PrivateProp",          "PrivateProp value");
        ShouldReadField(_parent, "TestReadonlyPrivateProp.PrivateProp",   "PrivateProp value");
        ShouldReadField(_parent, "TestReadonlyMethodProp.PrivateProp",    "PrivateProp value");
        ShouldReadField(_parent, "TestField.PrivateProp",                 "PrivateProp value");
        ShouldReadField(_parent, "_testPrivateField.PrivateProp",         "PrivateProp value");
        ShouldReadField(_parent, "_testPrivateField._nested.ReadonlyPrivateProp",       "ReadonlyPrivateProp value");
        //@formatter:on
    }

    [Fact] void ReadFieldIgnoreCaseTest() {
        ShouldReadField(_test, "priVAtePrOp", "PrivateProp value");
        ShouldReadField(_test, "_pRivaTefielD", "_privateField value");

        ShouldReadField(_parent, "TeSTFielD.priVAtePrOp", "PrivateProp value");
        ShouldReadField(_parent, "TeSTFielD._pRivaTefielD", "_privateField value");
    }

    [Fact] void ReadFieldNullTest() {
        ShouldReadField(_test, "nullField", null);
        ShouldReadField(_test, "nullClass", null);

        ShouldReadField(_parent, "TestField.nullField", null);
        ShouldReadField(_parent, "TestField.nullClass", null);
    }

    [Fact] void ReadObjectTest() {
        //given
        TestClass testObject = new() { PublicField = "abc" };
        _parent.TestProp = testObject;

        //expect
        _parent.ReadField<TestClass>("TestProp").Should().Be(testObject);
        _parent.ReadField<TestClass>("TestPrivateProp").Should().NotBe(testObject);
        _parent.ReadField<string>("TestProp.PublicField").Should().Be("abc");
    }

    [Fact] void NotFoundFieldTest() {
        ShouldThrow(() => _parent.ReadField<string>("lol"), "Not found field 'lol' in ParentClass");
        ShouldThrow(() => _parent.ReadField<string>("TestProp.lol"), "Not found field 'lol' in TestClass");
        ShouldThrow(() => _parent.ReadField<string>("TestProp.lol.someField"), "Not found field 'lol' in TestClass");

        ShouldThrow(() => _parent.WriteField("lol", ""), "Not found field 'lol' in ParentClass");
        ShouldThrow(() => _parent.WriteField("TestProp.lol", ""), "Not found field 'lol' in TestClass");
        ShouldThrow(() => _parent.WriteField("TestProp.lol.someField", ""), "Not found field 'lol' in TestClass");
    }

    [Fact] void NullParentFieldTest() {
        ShouldThrow(() => _parent.ReadField<string>("TestField.nullClass.lol.child"),
            "Can't get field 'lol', because parent 'TestField.nullClass' is null");

        ShouldThrow(() => _test.ReadField<string>("nullClass.lol.child"),
            "Can't get field 'lol', because parent 'nullClass' is null");

        ShouldThrow(() => _parent.ReadField<string>("TestField.nullField.lol.child"),
            "Can't get field 'lol', because parent 'TestField.nullField' is null");
    }

    [Fact] void InvalidTypeReadTest() {
        ShouldThrow(() => _parent.ReadField<int>("TestProp"),
            "Unable to cast object of type 'TestClass' to type 'System.Int32'.");
        ShouldThrow(() => _parent.ReadField<TestClass>("TestProp.PublicProp"),
            "Unable to cast object of type 'System.String' to type 'TestClass'.");
    }

    [Fact] void SetFieldTest() {
        _test.WriteField("PublicProp", "lol1");
        _test.WriteField("PrivateProp", "lol2");
        _test.WriteField("PublicField", "lol3");
        _test.WriteField("_privateField", "lol4");
        _test.WriteField("_privateField", "lol4");

        _test.PublicProp.Should().Be("lol1");
        _test.ReadField<string>("PrivateProp").Should().Be("lol2");
        _test.PublicField.Should().Be("lol3");
        _test.ReadField<string>("_privateField").Should().Be("lol4");

        _parent.WriteField("TestProp._nested.PublicProp", "lol5");
        _parent.ReadField<string>("TestProp._nested.PublicProp").Should().Be("lol5");
    }

    [Fact] void SetFieldFailedTest() {
        ShouldThrow(() => _test.WriteField("NoSetterProp", "xd"),
            "Can't set Property 'NoSetterProp' without setter");

        ShouldThrow(() => _test.WriteField("NullClass.PublicProp", "xd"),
            "Can't get field 'PublicProp', because parent 'NullClass' is null");
    }

    private void ShouldReadField(object target, string propName, string? expectedValue) {
        target.ReadField<string>(propName).Should().Be(expectedValue);
    }
}
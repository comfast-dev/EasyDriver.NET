using Comfast.Commons.Utils;
using FluentAssertions;

namespace EasyDriver.Tests.Unit;

public class TempFileTest {
    [Fact] void ReadWriteTest() {
        TempFile file = new("unitTests/someTempFildfae.txt");

        file.Delete();
        file.Delete();
        file.Write("lolxd");
        file.Read().Should().Be("lolxd");
    }
}
using Comfast.EasyDriver;
using Comfast.EasyDriver.Ui;
using Xunit;

using static Comfast.EasyDriver.CfApi;
namespace EasyDriver.Test.Integration;

public class ShadowRootsTest {

    [Fact] public void openShadowDom() {
        new BrowserContent().OpenResourceFile("test.html");

        Assert.Equal("Hello from shadow", S("my-div >> h3").Text);
        Assert.Equal("We need go deeper", S("my-div >> my-div >> h5").Text);
    }
}

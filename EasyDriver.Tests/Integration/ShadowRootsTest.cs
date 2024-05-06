using Comfast.EasyDriver.Ui;
using static Comfast.EasyDriver.DriverApi;

namespace EasyDriver.Tests.Integration;

public class ShadowRootsTest {

    [Fact] public void OpenShadowDom() {
        new BrowserContent().OpenResourceFile("test.html");

        Assert.Equal("Hello from shadow", S("my-div >> h3").Text);
        Assert.Equal("We need go deeper", S("my-div >> my-div >> h5").Text);
    }
}
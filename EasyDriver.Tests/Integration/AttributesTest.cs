using Comfast.EasyDriver.Ui;
using Xunit.Abstractions;
using static Comfast.EasyDriver.DriverApi;

namespace EasyDriver.Tests.Integration;

public class AttributesTest {
    private readonly ITestOutputHelper _output;

    public AttributesTest(ITestOutputHelper output) {
        new BrowserContent().OpenResourceFile("test.html");
        _output = output;
    }

    [Fact] public void TextAndHtml() {
        var p = S("#textAndHtml p");

        Assert.Equal("Hello World !", p.Text);
        Assert.Equal("Hello World !", p.GetAttribute("innerText"));
        Assert.Equal($" Hello World{Environment.NewLine}    !", p.GetAttribute("textContent"));
        Assert.Equal($" Hello <span>World</span>{Environment.NewLine}    !", p.InnerHtml);
        Assert.Equal($"<p> Hello <span>World</span>{Environment.NewLine}    !</p>", p.OuterHtml);
    }


    [Fact] void IsDisplayed() {
        Assert.True(S("#isDisplayed .default").Exists);
        Assert.True(S("#isDisplayed .default").IsDisplayed, "default");
        Assert.True(S("#isDisplayed .displayNone").Exists);
        Assert.False(S("#isDisplayed .displayNone").IsDisplayed, "display:none");
        Assert.True(S("#isDisplayed .visibilityHidden").Exists);
        Assert.False(S("#isDisplayed .visibilityHidden").IsDisplayed, "visibility:hidden");
        Assert.True(S("#isDisplayed .opacity01").Exists);
        Assert.True(S("#isDisplayed .opacity01").IsDisplayed, "opacity > 0");

        //playwright returns true, selenium false here
        Assert.True(S("#isDisplayed .opacity0").Exists);
        Assert.False(S("#isDisplayed .opacity0").IsDisplayed, "opacity == 0");
    }

    [Fact] void Attribute() {
        var input = S(".text input");
        Assert.Equal("text", input.GetAttribute("type"));
        Assert.Equal("", input.GetAttribute("value"));
        Assert.Null(input.GetAttribute("required"));
        Assert.Null(input.GetAttribute("required"));
        Assert.Null(input.GetAttribute("notattribute"));

        // JS attributes
        Assert.False(input.GetAttribute("formAction").Length == 0, "formAction");
        Assert.Equal("input", input.GetAttribute("localName"));
        Assert.Equal("inherit", input.GetAttribute("contentEditable"));
    }

    [Fact] void GetCssValueTest() {
        Assert.Equal("rgba(0, 0, 0, 0)", S("h1").GetCssValue("background-color"));
    }

    [Fact] public void HasAttribute() {
        new BrowserContent().SetBody("<input type='text' value='' required>");

        Assert.True(S("input").HasAttribute("type"));
        Assert.True(S("input").HasAttribute("value"));
        Assert.True(S("input").HasAttribute("required"));
        Assert.False(S("input").HasAttribute("notattribute"));
    }

    [Fact] public void HasClass() {
        new BrowserContent().SetBody("<input class='myclass disabled' />");

        var input = S("input");
        Assert.True(input.HasClass("myclass"));
        Assert.True(input.HasClass("disabled"));
        Assert.False(input.HasClass("lol"));
        Assert.False(input.HasClass("myclass disabled"));
    }

    [Fact] void Value() {
        Assert.Equal("abc", S(".text_abc input").Value);
        Assert.Null(S(".text_abc label").Value);
    }
}
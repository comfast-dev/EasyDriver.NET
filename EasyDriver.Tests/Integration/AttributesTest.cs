using Comfast.EasyDriver;
using Comfast.EasyDriver.Ui;
using Xunit;

namespace EasyDriver.Test.Integration;

public class AttributesTest {
    
    public AttributesTest() {
        new BrowserContent().OpenResourceFile("test.html"); 
    }
    
    [Fact] public void TextAndHtml() {
        var p = CfApi.S("#textAndHtml p");
        
        Assert.Equal("Hello World !", p.Text);
        Assert.Equal("Hello World !", p.GetAttribute("innerText"));
        Assert.Equal($" Hello World{Environment.NewLine}    !", p.GetAttribute("textContent"));
        Assert.Equal($" Hello <span>World</span>{Environment.NewLine}    !", p.InnerHtml);
        Assert.Equal($"<p> Hello <span>World</span>{Environment.NewLine}    !</p>", p.OuterHtml);
    }
}
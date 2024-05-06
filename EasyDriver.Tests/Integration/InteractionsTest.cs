using Comfast.EasyDriver.Ui;
using Xunit;

using static Comfast.EasyDriver.CfApi;
namespace EasyDriver.Test.Integration;

public class InteractionsTest {
    
    public InteractionsTest() {        
        new BrowserContent().OpenResourceFile("test.html");
    }
    
    [Fact] public void click() {
        var btn = S("#clicker button");
        var count = S("#clicker span");

        Assert.Equal("0", count.Text);

        btn.Click().Click().Click();
        Assert.Equal("3", count.Text);
    }

    // [Fact] public void focusAndHover() {
    //     var one = S("#focusAndHover #one");
    //     var two = S("#focusAndHover #two");
    //     var three = S("#focusAndHover #three");
    //
    //     one.hover();
    //     two.focus();
    //     assertAll(
    //         () -> assertEquals("rgba(0, 0, 255, 1)", one.getCssValue("color")),
    //         () -> assertEquals("rgba(255, 0, 0, 1)", two.getCssValue("color")),
    //         () -> assertEquals("rgba(0, 0, 0, 1)", three.getCssValue("color"))
    //     );
    // }

    // [Fact] public void dragAndDrop() {
    //     var dropZone1 = S("#dragAndDrop div").nth(1);
    //     var dropzone2 = S("#dragAndDrop div").nth(2);
    //     var dragMe = S("#dragMe");
    //     var currentDropZone = S("#dragMe >> ..");
    //
    //     dragMe.dragTo(dropzone2);
    //     assertThat(currentDropZone.getText()).contains("drop zone 2");
    //
    //     dragMe.dragTo(dropZone1);
    //     assertThat(currentDropZone.getText()).contains("drop zone 1");
    // }

    // @Disabled("Native WebDriver Actions Drag&Drop does not work fine")
    // [Fact] public void webdriverDragAndDrop() {
    //     //assign tracer
    //     driverEvents.addListener("tracer", new Tracer());
    //     var div = getDriver().findElement(By.cssSelector("#dragAndDrop div"));
    //     var dragMe = getDriver().findElement(By.cssSelector("#dragAndDrop #dragMe"));
    //     var dragMeParent = getDriver().findElement(By.xpath("//*[@id='dragMe']/.."));
    //
    //     new Actions(getDriver())
    //         .dragAndDrop(dragMe, div)
    //         .perform();
    //
    //     assertThat(dragMeParent.getAttribute("id")).startsWith("div");
    //
    //     driverEvents.removeListener("tracer");
    // }
    //
    // [Fact] public void type() {
    //     final String TEXT = "xyz";
    //     var el = S("input");
    //
    //     el.clear();
    //     el.type(TEXT + "\n");
    //
    //     assertEquals(TEXT, el.getValue());
    //     //todo more complicated features like .type("{Backspace+ABC}")
    // }
    //
    // [Fact] public void equalsTest() {
    //     var button1 = S("button");
    //     var button2 = S("button");
    //     var input = S("input");
    //     assertThat(button1).isEqualTo(button2);
    //     assertThat(button1).isNotEqualTo(input);
    // }
}
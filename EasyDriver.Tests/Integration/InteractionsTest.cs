using Comfast.EasyDriver;
using Comfast.EasyDriver.Ui;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using static Comfast.EasyDriver.DriverApi;

namespace EasyDriver.Tests.Integration;

public class InteractionsTest {

    public InteractionsTest() {
        new BrowserContent().OpenResourceFile("test.html");
    }

    [Fact] public void Click() {
        var btn = S("#clicker button");
        var count = S("#clicker span");

        Assert.Equal("0", count.Text);

        btn.Click().Click().Click();
        Assert.Equal("3", count.Text);
    }

    [Fact] public void FocusAndHover() {
        var one = S("#focusAndHover #one");
        var two = S("#focusAndHover #two");
        var three = S("#focusAndHover #three");

        one.Hover();
        two.Focus();
        Assert.Equal("rgba(0, 0, 255, 1)", one.GetCssValue("color"));
        Assert.Equal("rgba(255, 0, 0, 1)", two.GetCssValue("color"));
        Assert.Equal("rgba(0, 0, 0, 1)", three.GetCssValue("color"));
    }

    [Fact] public void DragAndDrop() {
        var dropZone1 = S("#dragAndDrop div").Nth(1);
        var dropzone2 = S("#dragAndDrop div").Nth(2);
        var dragMe = S("#dragMe");
        var currentDropZone = S("#dragMe >> ..");

        dragMe.DragTo(dropzone2);

        currentDropZone.Text.Should().Contain("drop zone 2");

        dragMe.DragTo(dropZone1);
        currentDropZone.Text.Should().Contain("drop zone 1");
    }

    [Fact(Skip = "webdriver implementation is failing")]
    public void WebdriverDragAndDrop() {
        var driver = GetDriver();
        var div = driver.FindElement(By.CssSelector("#dragAndDrop div"));
        var dragMe = driver.FindElement(By.CssSelector("#dragAndDrop #dragMe"));
        var dragMeParent = driver.FindElement(By.XPath("//*[@id='dragMe']/.."));

        new Actions(driver)
            .DragAndDrop(dragMe, div)
            .Perform();

        dragMeParent.GetAttribute("id").Should().StartWith("div");
    }

    [Fact] public void SendKeys() {
        const string text = "xyz";
        var el = S("input");

        el.Clear();
        el.SendKeys(text + "\n");

        Assert.Equal(text, el.Value);
    }
}
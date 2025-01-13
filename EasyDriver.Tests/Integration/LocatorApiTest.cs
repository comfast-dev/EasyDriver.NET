namespace EasyDriver.Tests.Integration;

public class LocatorTest {
    [Fact] void LocatorDescription() {
        var myLocator = S(".some.selector");
        Assert.Empty(myLocator.Description);

        const string description = "Submit button";
        var myLocatorWithDescription = myLocator.As(description);

        Assert.Equal(myLocatorWithDescription, myLocator);
        Assert.Equal(description, myLocatorWithDescription.Description);
        Assert.Equal(description, myLocator.Description);
    }

    [Fact] void SubLocatorDescription() {
        var myLocator = S(".some.selector").As("Login form");
        var subLocator = myLocator.SubLocator("button").As("Login button");

        Assert.Equal("Login button", subLocator.Description);
    }
}
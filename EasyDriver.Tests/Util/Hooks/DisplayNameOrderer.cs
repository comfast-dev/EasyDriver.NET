using Xunit.Abstractions;

[assembly: TestCollectionOrderer("EasyDriver.Tests.Util.Hooks.DisplayNameOrderer", "EasyDriver.Tests")]

namespace EasyDriver.Tests.Util.Hooks;

public class DisplayNameOrderer : ITestCollectionOrderer {
    public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections) {
        return testCollections.OrderBy(collection => collection.DisplayName);
    }
}
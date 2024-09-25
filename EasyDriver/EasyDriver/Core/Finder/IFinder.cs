namespace Comfast.EasyDriver.Core.Finder;

internal interface IFinder<T> {
    T Find();
    IList<T> FindAll();
}
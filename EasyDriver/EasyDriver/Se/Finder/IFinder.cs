namespace Comfast.EasyDriver.Se.Finder;

internal interface IFinder<T> {
    T Find();
    IList<T> FindAll();
}
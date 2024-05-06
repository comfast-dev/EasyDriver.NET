using System.Collections.ObjectModel;

namespace Comfast.EasyDriver.Se.Finder;

internal interface IFinder<T> {
    T Find();
    ReadOnlyCollection<T> FindAll();
}
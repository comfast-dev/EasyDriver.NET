using System.Collections.ObjectModel;

namespace Comfast.EasyDriver;

internal interface IFinder<T> {
    T Find();
    ReadOnlyCollection<T> FindAll();
}
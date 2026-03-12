using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.FailoverClusters.Framework;

public interface IInputParameter : IList, ICollection, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
{
}

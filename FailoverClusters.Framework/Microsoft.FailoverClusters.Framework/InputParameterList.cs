using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace FailoverClusters.Framework;

public class InputParameterList<T> : ObservableCollection<T>, IInputParameter, IList, ICollection, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
{
	public InputParameterList()
	{
	}

	public InputParameterList(IEnumerable<T> collection)
		: base(collection)
	{
	}
}


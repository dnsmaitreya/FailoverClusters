using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace Microsoft.FailoverClusters.Framework;

public class ReadOnlyCommandCollection : ReadOnlyCollection<ICommand>, INotifyCollectionChanged, INotifyPropertyChanged
{
	public event NotifyCollectionChangedEventHandler CollectionChanged;

	public event PropertyChangedEventHandler PropertyChanged;

	public ReadOnlyCommandCollection(CommandCollection commandCollection)
		: base((IList<ICommand>)commandCollection)
	{
		((INotifyCollectionChanged)base.Items).CollectionChanged += HandleCollectionChanged;
		((INotifyPropertyChanged)base.Items).PropertyChanged += HandlePropertyChanged;
	}

	private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		OnCollectionChanged(e);
	}

	private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		OnPropertyChanged(e);
	}

	protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
	{
		if (this.CollectionChanged != null)
		{
			this.CollectionChanged(this, args);
		}
	}

	protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, args);
		}
	}
}

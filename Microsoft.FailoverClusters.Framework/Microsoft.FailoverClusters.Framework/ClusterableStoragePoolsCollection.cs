using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterableStoragePoolsCollection : ObservableCollection<ClusterableStoragePool>
{
	private bool loaded;

	private Exception error;

	public bool IsLoaded
	{
		get
		{
			return loaded;
		}
		internal set
		{
			if (loaded != value)
			{
				loaded = value;
				OnPropertyChanged(new PropertyChangedEventArgs("IsLoaded"));
				this.Loaded.SafeCall(this, new ClusterableStoragePoolsCollectionLoadedChangedEventArgs(value));
			}
		}
	}

	public Exception Error
	{
		get
		{
			return error;
		}
		internal set
		{
			error = value;
			OnPropertyChanged(new PropertyChangedEventArgs("Error"));
		}
	}

	public event EventHandler<ClusterableStoragePoolsCollectionLoadedChangedEventArgs> Loaded;
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class CommandCollection : ObservableCollection<ICommand>
{
	private readonly ClusterCommandCollectionId collectionCategory;

	private string name = string.Empty;

	public ClusterCommandCollectionId Category => collectionCategory;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (!(name == value))
			{
				name = value;
				this.Renamed?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public ClusterCommand this[string nameIndex]
	{
		get
		{
			using (IEnumerator<ICommand> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ClusterCommand clusterCommand = (ClusterCommand)enumerator.Current;
					if (clusterCommand.Name.Equals(nameIndex, StringComparison.OrdinalIgnoreCase))
					{
						return clusterCommand;
					}
				}
			}
			throw new KeyNotFoundException(ExceptionResources.ClusterCommandNotFoundInCollection.FormatCurrentCulture(nameIndex));
		}
	}

	public event EventHandler Renamed;

	public CommandCollection(ClusterCommandCollectionId category)
	{
		collectionCategory = category;
	}

	public void RemoveAll(Func<ClusterCommand, bool> predicate)
	{
		Exceptions.ThrowIfNull(predicate, "predicate");
		for (int num = base.Count - 1; num >= 0; num--)
		{
			ClusterCommand arg = (ClusterCommand)base[num];
			if (predicate(arg))
			{
				RemoveItem(num);
			}
		}
	}

	internal void SignalRefresh()
	{
		using (BlockReentrancy())
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}
}

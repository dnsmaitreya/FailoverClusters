using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterFileShareErrorEventArgs : ClusterEventArgs
{
	public IList<FileShareErrorItem> ErrorItems { get; private set; }

	public ClusterFileShareErrorEventArgs(IList<FileShareErrorItem> errorItems)
		: base(Guid.Empty, null)
	{
		ErrorItems = new ReadOnlyCollection<FileShareErrorItem>(errorItems);
	}

	public ClusterFileShareErrorEventArgs(FileShareErrorItem errorItem)
		: this(new List<FileShareErrorItem> { errorItem })
	{
	}
}

using System;
using System.Drawing;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterResourceImageEventArgs : ClusterEventArgs
{
	public Bitmap Bitmap { get; internal set; }

	public ClusterResourceImageEventArgs(Guid id, Bitmap bitmap, ClusterException exception)
		: base(id, exception)
	{
		Bitmap = bitmap;
	}
}

using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class RenameCheckpointParameters
{
	public Checkpoint Checkpoint { get; set; }

	public string NewCheckpointName { get; set; }

	public RenameCheckpointParameters(Checkpoint checkpoint)
	{
		Exceptions.ThrowIfNull(checkpoint, "checkpoint");
		Checkpoint = checkpoint;
	}
}

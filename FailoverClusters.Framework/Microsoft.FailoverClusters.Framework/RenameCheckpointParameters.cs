using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

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


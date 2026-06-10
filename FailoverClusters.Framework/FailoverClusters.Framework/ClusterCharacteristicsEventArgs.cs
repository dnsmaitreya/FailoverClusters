using System;

namespace FailoverClusters.Framework;

public class ClusterCharacteristicsEventArgs : ClusterEventArgs
{
	public Characteristics? Characteristics { get; internal set; }

	public ClusterCharacteristicsEventArgs(Guid id, Characteristics? newCharacteristics, ClusterException exception)
		: base(id, exception)
	{
		Characteristics = newCharacteristics;
	}
}


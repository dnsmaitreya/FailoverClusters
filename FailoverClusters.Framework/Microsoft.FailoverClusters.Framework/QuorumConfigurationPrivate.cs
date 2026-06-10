using System;

namespace FailoverClusters.Framework;

public class QuorumConfigurationPrivate : IEquatable<QuorumConfigurationPrivate>
{
	public Guid QuorumResourceId { get; set; }

	public QuorumType QuorumType { get; set; }

	public bool Equals(QuorumConfigurationPrivate other)
	{
		if (other == null)
		{
			return false;
		}
		if (other.QuorumResourceId == QuorumResourceId)
		{
			return other.QuorumType == QuorumType;
		}
		return false;
	}
}


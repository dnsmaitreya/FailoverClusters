using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterFullyQualifiedDomainChangedEventArgs : ClusterEventArgs
{
	public string NewFullyQualifiedDomainName { get; internal set; }

	public ClusterFullyQualifiedDomainChangedEventArgs(Guid id, string newFullyQualifiedDomainName)
		: base(id, null)
	{
		NewFullyQualifiedDomainName = newFullyQualifiedDomainName;
	}
}

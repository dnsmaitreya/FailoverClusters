using System;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class QuorumConfiguration : ViewModelBase
{
	private Resource quorumResource;

	private static readonly string QuorumResourcePropertyName = ViewModelBaseExtensions.GetMemberNameFromPropertyExpression((QuorumConfiguration me) => me.QuorumResource);

	private static readonly string QuorumTypePropertyName = ViewModelBaseExtensions.GetMemberNameFromPropertyExpression((QuorumConfiguration me) => me.QuorumType);

	public QuorumType QuorumType { get; private set; }

	public Guid QuorumResourceId { get; private set; }

	public Resource QuorumResource
	{
		get
		{
			if (quorumResource == null && QuorumResourceId != Guid.Empty)
			{
				Resource.Get(Cluster, QuorumResourceId, delegate(OperationResult<Resource> r)
				{
					quorumResource = r.Result;
					UIHelper.ExecuteOnDispatcher(delegate
					{
						NotifyPropertyChanged(QuorumResourcePropertyName);
						NotifyPropertyChanged(QuorumTypePropertyName);
					}, OperationType.Async);
				}, OperationType.Async);
			}
			return quorumResource;
		}
	}

	private Cluster Cluster { get; set; }

	internal QuorumConfiguration(Cluster cluster, QuorumConfigurationPrivate quorumConfigurationPrivate)
	{
		Cluster = cluster;
		QuorumResourceId = quorumConfigurationPrivate.QuorumResourceId;
		QuorumType = quorumConfigurationPrivate.QuorumType;
	}
}

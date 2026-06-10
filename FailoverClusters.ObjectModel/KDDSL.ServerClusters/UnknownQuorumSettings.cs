using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public class UnknownQuorumSettings : QuorumSettings, IHasQuorumResource
{
	private ClusterResource m_resource;

	public virtual ClusterResource QuorumResource => m_resource;

	public override QuorumType QuorumType => QuorumType.Unknown;

	internal override void Configure()
	{
	}

	internal override void Cleanup()
	{
	}

	internal UnknownQuorumSettings(ClusterResource resource)
		: base(resource.Cluster)
	{
		m_resource = resource;
	}

	public override void VerifySettings()
	{
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public override bool AreQuorumSettingsEqual(QuorumSettings settings)
	{
		return false;
	}
}

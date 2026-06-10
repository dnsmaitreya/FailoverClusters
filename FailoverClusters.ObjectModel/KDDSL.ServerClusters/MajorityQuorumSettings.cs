using System;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public class MajorityQuorumSettings : QuorumSettings
{
	public override QuorumType QuorumType => QuorumType.MajorityOfNodes;

	internal override void Configure()
	{
		//Discarded unreachable code: IL_0032
		try
		{
			ReportOperationProcess(50, Resources.MajorityOfNodesSettingAsQuorum_Text);
			base.Cluster.SetMajorityQuorum(null, null);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[1] { Resources.MajorityOfNodesSettingAsQuorumFailed_Text });
		}
	}

	internal override void Cleanup()
	{
	}

	internal MajorityQuorumSettings(Cluster cluster)
		: base(cluster)
	{
	}

	public override void VerifySettings()
	{
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public override bool AreQuorumSettingsEqual(QuorumSettings settings)
	{
		bool flag = false;
		return QuorumType == settings.QuorumType || flag;
	}
}

using System;

namespace KDDSL.ServerClusters;

internal class ClusterControlExecutor : ControlExecutor, ICommonControlCodes
{
	private Cluster m_cluster;

	public virtual uint UnknownCode => 117440512u;

	public virtual uint ValidatePrivatePropertiesCode => 117440649u;

	public virtual uint SetPrivatePropertiesCode => 121634950u;

	public virtual uint GetPrivatePropertiesFormatCode => 117440653u;

	public virtual uint GetReadOnlyPrivatePropertiesCode => 117440637u;

	public virtual uint GetPrivatePropertiesCode => 117440641u;

	public virtual uint EnumPrivatePropertiesCode => 117440633u;

	public virtual uint ValidateCommonPropertiesCode => 117440609u;

	public virtual uint SetCommonPropertiesCode => 121634910u;

	public virtual uint GetCommonPropertiesFormatCode => 117440613u;

	public virtual uint GetReadOnlyCommonPropertiesCode => 117440597u;

	public virtual uint GetCommonPropertiesCode => 117440601u;

	public virtual uint EnumCommonPropertiesCode => 117440593u;

	protected unsafe override uint ExecuteControl(ClusterNode node, uint controlCode, void* pInBuffer, uint dwInBufferSize, void* pOutBuffer, uint dwOutBufferSize, uint* pdwBytesReturned)
	{
		//IL_000f: Expected I, but got I8
		_HNODE* ptr = ((node == null) ? null : node.Handle);
		if (m_controlCodesMonitor != null)
		{
			DateTime now = DateTime.Now;
			m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: false, now, "Cluster", m_cluster.Name, (int)m_cluster.Handle, node, (int)controlCode, StringControlCode(controlCode), 0, DebugLog.GetStackTrace()));
		}
		uint num = global::_003CModule_003E.ClusterControl(m_cluster.Handle, ptr, controlCode, pInBuffer, dwInBufferSize, pOutBuffer, dwOutBufferSize, pdwBytesReturned);
		if (m_controlCodesMonitor != null)
		{
			DateTime now2 = DateTime.Now;
			m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: true, now2, "Cluster", m_cluster.Name, (int)m_cluster.Handle, node, (int)controlCode, StringControlCode(controlCode), (int)num, DebugLog.GetStackTrace()));
		}
		return num;
	}

	public ClusterControlExecutor(Cluster cluster)
		: base(cluster)
	{
		m_cluster = cluster;
	}
}

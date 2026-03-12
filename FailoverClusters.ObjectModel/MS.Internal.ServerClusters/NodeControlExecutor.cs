using System;

namespace MS.Internal.ServerClusters;

internal class NodeControlExecutor : ControlExecutor, ICommonControlCodes, IClusterItemControlCodes
{
	private ClusterNode m_node;

	private unsafe _HNODE* m_nodeHandle;

	private SafeNodeHandle m_safeNodeHandle;

	public virtual uint GetStorageNodeStateCode => 67117557u;

	public virtual uint GetNameCode => 67108905u;

	public virtual uint GetIdCode => 67108921u;

	public virtual uint GetFlagsCode => 67108873u;

	public virtual uint GetCharacteristicsCode => 67108869u;

	public virtual uint UnknownCode => 67108864u;

	public virtual uint ValidatePrivatePropertiesCode => 67109001u;

	public virtual uint SetPrivatePropertiesCode => 71303302u;

	public virtual uint GetPrivatePropertiesFormatCode => 67109005u;

	public virtual uint GetReadOnlyPrivatePropertiesCode => 67108989u;

	public virtual uint GetPrivatePropertiesCode => 67108993u;

	public virtual uint EnumPrivatePropertiesCode => 67108985u;

	public virtual uint ValidateCommonPropertiesCode => 67108961u;

	public virtual uint SetCommonPropertiesCode => 71303262u;

	public virtual uint GetCommonPropertiesFormatCode => 67108965u;

	public virtual uint GetReadOnlyCommonPropertiesCode => 67108949u;

	public virtual uint GetCommonPropertiesCode => 67108953u;

	public virtual uint EnumCommonPropertiesCode => 67108945u;

	protected unsafe override uint ExecuteControl(ClusterNode node, uint controlCode, void* pInBuffer, uint dwInBufferSize, void* pOutBuffer, uint dwOutBufferSize, uint* pdwBytesReturned)
	{
		//IL_0011: Expected I, but got I8
		_HNODE* ptr = ((node == null) ? null : node.Handle);
		if (m_controlCodesMonitor != null)
		{
			string controlCodeOwnerName = m_node?.Name;
			DateTime now = DateTime.Now;
			m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: false, now, "Node", controlCodeOwnerName, (int)m_nodeHandle, node, (int)controlCode, StringControlCode(controlCode), 0, DebugLog.GetStackTrace()));
		}
		uint num = global::_003CModule_003E.ClusterNodeControl(m_nodeHandle, ptr, controlCode, pInBuffer, dwInBufferSize, pOutBuffer, dwOutBufferSize, pdwBytesReturned);
		if (m_controlCodesMonitor != null)
		{
			string controlCodeOwnerName2 = m_node?.Name;
			DateTime now2 = DateTime.Now;
			m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: true, now2, "Node", controlCodeOwnerName2, (int)m_nodeHandle, node, (int)controlCode, StringControlCode(controlCode), (int)num, DebugLog.GetStackTrace()));
		}
		return num;
	}

	public unsafe NodeControlExecutor(SafeNodeHandle nodeHandle, Cluster cluster)
		: base(cluster)
	{
		m_safeNodeHandle = nodeHandle;
		m_nodeHandle = nodeHandle.DangerousGetNodeHandle();
	}

	public unsafe NodeControlExecutor(ClusterNode node, Cluster cluster)
		: base(cluster)
	{
		m_node = node;
		m_nodeHandle = node.Handle;
	}
}

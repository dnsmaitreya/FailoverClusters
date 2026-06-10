using System;

namespace KDDSL.ServerClusters;

internal class GroupControlExecutor : ControlExecutor, ICommonControlCodes, IClusterItemControlCodes
{
	private ClusterGroup m_group;

	private unsafe _HGROUP* m_groupHandle;

	private SafeGroupHandle m_safeGroupHandle;

	public virtual uint GetNameCode => 50331689u;

	public virtual uint GetIdCode => 50331705u;

	public virtual uint GetFlagsCode => 50331657u;

	public virtual uint GetCharacteristicsCode => 50331653u;

	public virtual uint UnknownCode => 50331648u;

	public virtual uint SetFailoverCountCode => 54534182u;

	public virtual uint ValidatePrivatePropertiesCode => 50331785u;

	public virtual uint SetPrivatePropertiesCode => 54526086u;

	public virtual uint GetPrivatePropertiesFormatCode => 50331789u;

	public virtual uint GetReadOnlyPrivatePropertiesCode => 50331773u;

	public virtual uint GetPrivatePropertiesCode => 50331777u;

	public virtual uint EnumPrivatePropertiesCode => 50331769u;

	public virtual uint ValidateCommonPropertiesCode => 50331745u;

	public virtual uint SetCommonPropertiesCode => 54526046u;

	public virtual uint GetCommonPropertiesFormatCode => 50331749u;

	public virtual uint GetReadOnlyCommonPropertiesCode => 50331733u;

	public virtual uint GetCommonPropertiesCode => 50331737u;

	public virtual uint EnumCommonPropertiesCode => 50331729u;

	protected unsafe override uint ExecuteControl(ClusterNode node, uint controlCode, void* pInBuffer, uint dwInBufferSize, void* pOutBuffer, uint dwOutBufferSize, uint* pdwBytesReturned)
	{
		//IL_0011: Expected I, but got I8
		_HNODE* ptr = ((node == null) ? null : node.Handle);
		if (m_controlCodesMonitor != null)
		{
			string controlCodeOwnerName = m_group?.Name;
			DateTime now = DateTime.Now;
			m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: false, now, "Group", controlCodeOwnerName, (int)m_groupHandle, node, (int)controlCode, StringControlCode(controlCode), 0, DebugLog.GetStackTrace()));
		}
		uint num = global::_003CModule_003E.ClusterGroupControl(m_groupHandle, ptr, controlCode, pInBuffer, dwInBufferSize, pOutBuffer, dwOutBufferSize, pdwBytesReturned);
		if (m_controlCodesMonitor != null)
		{
			string controlCodeOwnerName2 = m_group?.Name;
			DateTime now2 = DateTime.Now;
			m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: true, now2, "Group", controlCodeOwnerName2, (int)m_groupHandle, node, (int)controlCode, StringControlCode(controlCode), (int)num, DebugLog.GetStackTrace()));
		}
		return num;
	}

	public unsafe GroupControlExecutor(SafeGroupHandle groupHandle, Cluster cluster)
		: base(cluster)
	{
		m_safeGroupHandle = groupHandle;
		m_groupHandle = groupHandle.DangerousGetGroupHandle();
	}

	public unsafe GroupControlExecutor(ClusterGroup group, Cluster cluster)
		: base(cluster)
	{
		m_group = group;
		m_groupHandle = group.Handle;
	}
}

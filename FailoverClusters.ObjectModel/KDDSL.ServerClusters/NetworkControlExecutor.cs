using System;

namespace KDDSL.ServerClusters;

internal class NetworkControlExecutor : ControlExecutor, ICommonControlCodes, IClusterItemControlCodes
{
	private ClusterNetwork m_network;

	private unsafe _HNETWORK* m_networkHandle;

	private SafeNetworkHandle m_safeNetworkHandle;

	public virtual uint GetNameCode => 83886121u;

	public virtual uint GetIdCode => 83886137u;

	public virtual uint GetFlagsCode => 83886089u;

	public virtual uint GetCharacteristicsCode => 83886085u;

	public virtual uint UnknownCode => 83886080u;

	public virtual uint ValidatePrivatePropertiesCode => 83886217u;

	public virtual uint SetPrivatePropertiesCode => 88080518u;

	public virtual uint GetPrivatePropertiesFormatCode => 83886221u;

	public virtual uint GetReadOnlyPrivatePropertiesCode => 83886205u;

	public virtual uint GetPrivatePropertiesCode => 83886209u;

	public virtual uint EnumPrivatePropertiesCode => 83886201u;

	public virtual uint ValidateCommonPropertiesCode => 83886177u;

	public virtual uint SetCommonPropertiesCode => 88080478u;

	public virtual uint GetCommonPropertiesFormatCode => 83886181u;

	public virtual uint GetReadOnlyCommonPropertiesCode => 83886165u;

	public virtual uint GetCommonPropertiesCode => 83886169u;

	public virtual uint EnumCommonPropertiesCode => 83886161u;

	protected unsafe override uint ExecuteControl(ClusterNode node, uint controlCode, void* pInBuffer, uint dwInBufferSize, void* pOutBuffer, uint dwOutBufferSize, uint* pdwBytesReturned)
	{
		//IL_0011: Expected I, but got I8
		_HNODE* ptr = ((node == null) ? null : node.Handle);
		if (m_controlCodesMonitor != null)
		{
			string controlCodeOwnerName = m_network?.Name;
			DateTime now = DateTime.Now;
			m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: false, now, "Network", controlCodeOwnerName, (int)m_networkHandle, node, (int)controlCode, StringControlCode(controlCode), 0, DebugLog.GetStackTrace()));
		}
		uint num = global::_003CModule_003E.ClusterNetworkControl(m_networkHandle, ptr, controlCode, pInBuffer, dwInBufferSize, pOutBuffer, dwOutBufferSize, pdwBytesReturned);
		if (m_controlCodesMonitor != null)
		{
			string controlCodeOwnerName2 = m_network?.Name;
			DateTime now2 = DateTime.Now;
			m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: true, now2, "Network", controlCodeOwnerName2, (int)m_networkHandle, node, (int)controlCode, StringControlCode(controlCode), (int)num, DebugLog.GetStackTrace()));
		}
		return num;
	}

	public unsafe NetworkControlExecutor(SafeNetworkHandle networkHandle, Cluster cluster)
		: base(cluster)
	{
		m_safeNetworkHandle = networkHandle;
		m_networkHandle = networkHandle.DangerousGetNetworkHandle();
	}

	public unsafe NetworkControlExecutor(ClusterNetwork network, Cluster cluster)
		: base(cluster)
	{
		m_network = network;
		m_networkHandle = network.Handle;
	}
}

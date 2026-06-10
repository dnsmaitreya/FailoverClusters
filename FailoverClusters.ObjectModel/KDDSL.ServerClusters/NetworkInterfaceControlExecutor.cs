using System;

namespace KDDSL.ServerClusters;

internal class NetworkInterfaceControlExecutor : ControlExecutor, ICommonControlCodes, IClusterItemControlCodes
{
	private ClusterNetworkInterface m_networkInterface;

	private unsafe _HNETINTERFACE* m_networkInterfaceHandle;

	private SafeNetworkInterfaceHandle m_safeNetworkInterfaceHandle;

	public virtual uint GetNameCode => 100663337u;

	public virtual uint GetIdCode => 100663353u;

	public virtual uint GetFlagsCode => 100663305u;

	public virtual uint GetCharacteristicsCode => 100663301u;

	public virtual uint UnknownCode => 100663296u;

	public virtual uint ValidatePrivatePropertiesCode => 100663433u;

	public virtual uint SetPrivatePropertiesCode => 104857734u;

	public virtual uint GetPrivatePropertiesFormatCode => 100663437u;

	public virtual uint GetReadOnlyPrivatePropertiesCode => 100663421u;

	public virtual uint GetPrivatePropertiesCode => 100663425u;

	public virtual uint EnumPrivatePropertiesCode => 100663417u;

	public virtual uint ValidateCommonPropertiesCode => 100663393u;

	public virtual uint SetCommonPropertiesCode => 104857694u;

	public virtual uint GetCommonPropertiesFormatCode => 100663397u;

	public virtual uint GetReadOnlyCommonPropertiesCode => 100663381u;

	public virtual uint GetCommonPropertiesCode => 100663385u;

	public virtual uint EnumCommonPropertiesCode => 100663377u;

	protected unsafe override uint ExecuteControl(ClusterNode node, uint controlCode, void* pInBuffer, uint dwInBufferSize, void* pOutBuffer, uint dwOutBufferSize, uint* pdwBytesReturned)
	{
		//IL_0011: Expected I, but got I8
		_HNODE* ptr = ((node == null) ? null : node.Handle);
		if (m_controlCodesMonitor != null)
		{
			string controlCodeOwnerName = m_networkInterface?.Name;
			DateTime now = DateTime.Now;
			m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: false, now, "NetInterface", controlCodeOwnerName, (int)m_networkInterfaceHandle, node, (int)controlCode, StringControlCode(controlCode), 0, DebugLog.GetStackTrace()));
		}
		uint num = global::_003CModule_003E.ClusterNetInterfaceControl(m_networkInterfaceHandle, ptr, controlCode, pInBuffer, dwInBufferSize, pOutBuffer, dwOutBufferSize, pdwBytesReturned);
		if (m_controlCodesMonitor != null)
		{
			string controlCodeOwnerName2 = m_networkInterface?.Name;
			DateTime now2 = DateTime.Now;
			m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: true, now2, "NetInterface", controlCodeOwnerName2, (int)m_networkInterfaceHandle, node, (int)controlCode, StringControlCode(controlCode), (int)num, DebugLog.GetStackTrace()));
		}
		return num;
	}

	public unsafe NetworkInterfaceControlExecutor(SafeNetworkInterfaceHandle networkInterfaceHandle, Cluster cluster)
		: base(cluster)
	{
		m_safeNetworkInterfaceHandle = networkInterfaceHandle;
		m_networkInterfaceHandle = networkInterfaceHandle.DangerousGetNetworkInterfaceHandle();
	}

	public unsafe NetworkInterfaceControlExecutor(ClusterNetworkInterface networkInterface, Cluster cluster)
		: base(cluster)
	{
		m_networkInterface = networkInterface;
		m_networkInterfaceHandle = networkInterface.Handle;
	}
}

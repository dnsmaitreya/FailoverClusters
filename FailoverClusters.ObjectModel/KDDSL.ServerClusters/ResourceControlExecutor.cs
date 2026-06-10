using System;

namespace KDDSL.ServerClusters;

internal class ResourceControlExecutor : ControlExecutor, ICommonControlCodes, IClusterItemControlCodes
{
	private ClusterResource m_resource;

	private unsafe _HRESOURCE* m_resourceHandle;

	private SafeResourceHandle m_safeResourceHandle;

	public static uint GetRegisterDnsCode => 16777586u;

	public static uint Storage_GetDirty => 16777753u;

	public static uint VM_Configuration_UpdateConfiguration => 23068676u;

	public uint VM_SetNextOfflineAction => 23068692u;

	public uint VM_CancelMigration => 23068680u;

	public uint VM_StartMigration => 23068676u;

	public uint IPAddress_RenewLeaseCode => 20971966u;

	public uint IPAddress_ReleaseLeaseCode => 20971970u;

	public uint PhysicalDisk_SharedVolumeState => 20972194u;

	public uint PhysicalDisk_SharedVolumeLinkName => 16777765u;

	public uint Partition_MountPoints => 16777745u;

	public uint Storage_IsPathValid => 16777625u;

	public uint Storage_SetDriveLetter => 20972010u;

	public uint Storage_GetDiskInfoCodeEx => 16777713u;

	public uint Storage_GetDiskInfoCode => 16777617u;

	public uint GetRequiredDependenciesCode => 16777233u;

	public uint GetClassInfoCode => 16777229u;

	public uint DeleteRegistryCheckPointCode => 20971686u;

	public uint AddRegistryCheckPointCode => 20971682u;

	public uint GetRegistryCheckPointCode => 16777385u;

	public uint DeleteCryptoCheckPointCode => 20971698u;

	public uint AddCryptoCheckPointCode => 20971694u;

	public uint GetCryptoCheckPointCode => 16777397u;

	public virtual uint GetNameCode => 16777257u;

	public virtual uint GetIdCode => 16777273u;

	public virtual uint GetFlagsCode => 16777225u;

	public virtual uint GetCharacteristicsCode => 16777221u;

	public virtual uint UnknownCode => 16777216u;

	public virtual uint ValidatePrivatePropertiesCode => 16777353u;

	public virtual uint SetPrivatePropertiesCode => 20971654u;

	public virtual uint GetPrivatePropertiesFormatCode => 16777357u;

	public virtual uint GetReadOnlyPrivatePropertiesCode => 16777341u;

	public virtual uint GetPrivatePropertiesCode => 16777345u;

	public virtual uint EnumPrivatePropertiesCode => 16777337u;

	public virtual uint ValidateCommonPropertiesCode => 16777313u;

	public virtual uint SetCommonPropertiesCode => 20971614u;

	public virtual uint GetCommonPropertiesFormatCode => 16777317u;

	public virtual uint GetReadOnlyCommonPropertiesCode => 16777301u;

	public virtual uint GetCommonPropertiesCode => 16777305u;

	public virtual uint EnumCommonPropertiesCode => 16777297u;

	protected unsafe override uint ExecuteControl(ClusterNode node, uint controlCode, void* pInBuffer, uint dwInBufferSize, void* pOutBuffer, uint dwOutBufferSize, uint* pdwBytesReturned)
	{
		//IL_0011: Expected I, but got I8
		_HNODE* ptr = ((node == null) ? null : node.Handle);
		if (m_controlCodesMonitor != null)
		{
			string controlCodeOwnerName = m_resource?.Name;
			DateTime now = DateTime.Now;
			m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: false, now, "Resource", controlCodeOwnerName, (int)m_resourceHandle, node, (int)controlCode, StringControlCode(controlCode), 0, DebugLog.GetStackTrace()));
		}
		uint num = global::_003CModule_003E.ClusterResourceControl(m_resourceHandle, ptr, controlCode, pInBuffer, dwInBufferSize, pOutBuffer, dwOutBufferSize, pdwBytesReturned);
		if (m_controlCodesMonitor != null)
		{
			string controlCodeOwnerName2 = m_resource?.Name;
			DateTime now2 = DateTime.Now;
			m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: true, now2, "Resource", controlCodeOwnerName2, (int)m_resourceHandle, node, (int)controlCode, StringControlCode(controlCode), (int)num, DebugLog.GetStackTrace()));
		}
		return num;
	}

	public unsafe ResourceControlExecutor(SafeResourceHandle resourceHandle, Cluster cluster)
		: base(cluster)
	{
		m_safeResourceHandle = resourceHandle;
		m_resourceHandle = resourceHandle.DangerousGetResourceHandle();
	}

	public unsafe ResourceControlExecutor(ClusterResource resource, Cluster cluster)
		: base(cluster)
	{
		m_resource = resource;
		m_resourceHandle = resource.Handle;
	}
}

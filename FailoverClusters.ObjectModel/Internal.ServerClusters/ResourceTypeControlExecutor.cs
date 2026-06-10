using System;

namespace MS.Internal.ServerClusters;

internal class ResourceTypeControlExecutor : ControlExecutor, ICommonControlCodes, IClusterItemControlCodes
{
	private string m_resourceTypeName;

	private Cluster m_cluster;

	public static uint NetworkNameGetOU => 37749358u;

	public static uint FileShareWitnessValidatePath => 33554993u;

	public static uint GenericScriptValidatePath => 33554993u;

	public static uint GenericApplicationValidatePath => 33554993u;

	public uint Storage_RemoveOwnership => 37749262u;

	public uint Storage_IsClusterable => 33554953u;

	public uint PhysicalDisk_IsFileOnSharedVolume => 16777769u;

	public uint Storage_GetDiskId => 33554949u;

	public static uint StorageGetAvailableDriveLetters => 33554925u;

	public static uint StorageGetAvailableDisksCode => 33554933u;

	public virtual uint GetNameCode
	{
		get
		{
			//Discarded unreachable code: IL_0006
			throw new InvalidOperationException();
		}
	}

	public virtual uint GetIdCode
	{
		get
		{
			//Discarded unreachable code: IL_0006
			throw new InvalidOperationException();
		}
	}

	public virtual uint GetFlagsCode => 33554441u;

	public virtual uint GetCharacteristicsCode => 33554437u;

	public virtual uint GetTypeClassInfoCode => 33554445u;

	public virtual uint UnknownCode => 33554432u;

	public virtual uint ValidatePrivatePropertiesCode => 33554569u;

	public virtual uint SetPrivatePropertiesCode => 37748870u;

	public virtual uint GetPrivatePropertiesFormatCode => 33554573u;

	public virtual uint GetReadOnlyPrivatePropertiesCode => 33554557u;

	public virtual uint GetPrivatePropertiesCode => 33554561u;

	public virtual uint EnumPrivatePropertiesCode => 33554553u;

	public virtual uint ValidateCommonPropertiesCode => 33554529u;

	public virtual uint SetCommonPropertiesCode => 37748830u;

	public virtual uint GetCommonPropertiesFormatCode => 33554533u;

	public virtual uint GetReadOnlyCommonPropertiesCode => 33554517u;

	public virtual uint GetCommonPropertiesCode => 33554521u;

	public virtual uint EnumCommonPropertiesCode => 33554513u;

	protected unsafe override uint ExecuteControl(ClusterNode node, uint controlCode, void* pInBuffer, uint dwInBufferSize, void* pOutBuffer, uint dwOutBufferSize, uint* pdwBytesReturned)
	{
		//IL_001b: Expected I, but got I8
		ushort* ptr = InteropHelp.StringToWstr(m_resourceTypeName);
		_HNODE* ptr2 = ((node == null) ? null : node.Handle);
		uint num;
		try
		{
			if (m_controlCodesMonitor != null)
			{
				DateTime now = DateTime.Now;
				m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: false, now, "ResourceType", m_resourceTypeName, 0, node, (int)controlCode, StringControlCode(controlCode), 0, DebugLog.GetStackTrace()));
			}
			num = global::_003CModule_003E.ClusterResourceTypeControl(m_cluster.Handle, ptr, ptr2, controlCode, pInBuffer, dwInBufferSize, pOutBuffer, dwOutBufferSize, pdwBytesReturned);
			if (m_controlCodesMonitor != null)
			{
				DateTime now2 = DateTime.Now;
				m_controlCodesMonitor(this, new ControlCodesEventArgs(isResponse: true, now2, "ResourceType", m_resourceTypeName, 0, node, (int)controlCode, StringControlCode(controlCode), (int)num, DebugLog.GetStackTrace()));
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
		return num;
	}

	public ResourceTypeControlExecutor(string resourceTypeName, Cluster cluster)
		: base(cluster)
	{
		m_cluster = cluster;
		m_resourceTypeName = resourceTypeName;
	}
}

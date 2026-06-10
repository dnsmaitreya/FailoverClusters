namespace KDDSL.ServerClusters;

internal class StorageOnlyNodeControlExecutor : ControlExecutor, ICommonControlCodes, IClusterItemControlCodes
{
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
		return 0u;
	}

	public StorageOnlyNodeControlExecutor(ClusterStorageOnlyNode storageEnclosure, Cluster cluster)
		: base(cluster)
	{
	}
}

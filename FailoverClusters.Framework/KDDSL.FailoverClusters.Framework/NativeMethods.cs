using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.AccessControl;
using System.Text;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal static class NativeMethods
{
	internal enum GetWindow_Cmd
	{
		GW_HWNDFIRST = 0,
		GW_HWNDLAST = 1,
		GW_HWNDNEXT = 2,
		GW_HWNDPREV = 3,
		GW_OWNER = 4,
		GW_CHILD = 5,
		GW_ENABLEDPOPUP = 6,
		GW_MAX = 6
	}

	internal enum LanguageId : ushort
	{
		Neutral = 0,
		Invariant = 127
	}

	internal enum SubLanaguageId : ushort
	{
		Neutral,
		Default
	}

	[Flags]
	internal enum FormatMessageFlags
	{
		IgnoreInserts = 0x200,
		FromString = 0x400,
		FromHModule = 0x800,
		FromSystem = 0x1000,
		ArgumentArray = 0x2000,
		MaxWidthMask = 0xFF
	}

	internal enum CLUSTER_CSV_VOLUME_FAULT_STATE : uint
	{
		VolumeStateNoFaults = 0u,
		VolumeStateNoDirectIO = 1u,
		VolumeStateNoAccess = 2u,
		VolumeStateInMaintenance = 4u,
		VolumeStateDismounted = 8u
	}

	internal enum CLUSTER_SHARED_VOLUME_BACKUP_STATE : uint
	{
		VolumeBackupNone,
		VolumeBackupInProgress
	}

	internal enum CLUSTER_SHARED_VOLUME_STATE : uint
	{
		SharedVolumeStateUnavailable,
		SharedVolumeStatePaused,
		SharedVolumeStateActive
	}

	internal enum NetworkRole
	{
		None,
		InternalUse,
		ClientAccess,
		InternalAndClient
	}

	internal enum CM_NOTIFY_FILTER_TYPE : uint
	{
		CM_NOTIFY_FILTER_TYPE_DEVICEINTERFACE,
		CM_NOTIFY_FILTER_TYPE_DEVICEHANDLE,
		CM_NOTIFY_FILTER_TYPE_DEVICEINSTANCE,
		CM_NOTIFY_FILTER_TYPE_MAX
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct CM_NOTIFY_EVENT_DATA
	{
		internal CM_NOTIFY_FILTER_TYPE FilterType;

		private uint Reserved;

		internal Guid EventGuid;

		private int NameOffset;

		private uint DataSize;

		internal SP_NOTIFICATION_INFO Data;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct SP_ID
	{
		internal Guid PoolId;

		internal Guid Id;
	}

	internal enum SP_NOTIFICATION_TYPE : uint
	{
		SpNotificationTypeUnknown,
		SpNotificationTypePoolInfoChange,
		SpNotificationTypePoolCreate,
		SpNotificationTypePoolDelete,
		SpNotificationTypePoolArrival,
		SpNotificationTypePoolRemoval,
		SpNotificationTypePoolRefresh,
		SpNotificationTypeDriveInfoChange,
		SpNotificationTypeDriveAdd,
		SpNotificationTypeDriveRemove,
		SpNotificationTypeSpaceInfoChange,
		SpNotificationTypeSpaceCreate,
		SpNotificationTypeSpaceDelete,
		SpNotificationTypeSpaceRepairNeedPhase2,
		SpNotificationTypeSpaceRepairNeedPhase3,
		SpNotificationTypeEnclosureInfoChange,
		SpNotificationTypeEnclosureArrival,
		SpNotificationTypeEnclosureRemoval,
		SpNotificationTypeSpaceNeedRebalance
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct SP_NOTIFICATION_INFO
	{
		internal SP_ID Id;

		internal SP_NOTIFICATION_TYPE Type;

		internal uint Flags;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct CLUS_CSV_VOLUME_INFO
	{
		public ulong VolumeOffset;

		public uint PartitionNumber;

		public CLUSTER_CSV_VOLUME_FAULT_STATE FaultState;

		public CLUSTER_SHARED_VOLUME_BACKUP_STATE BackupState;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szVolumeFriendlyName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
		public string szVolumeName;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct CLUSTER_SHARED_VOLUME_STATE_INFO
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szVolumeName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szNodeName;

		public CLUSTER_SHARED_VOLUME_STATE VolumeState;
	}

	internal enum CLUSTER_SHARED_VOLUME_REDIRECTED_IO_REASON : ulong
	{
		ReasonNoDiskConnectivity = 1uL,
		ReasonStorageSpaceNotAttached = 2uL,
		Max = 9223372036854775808uL
	}

	internal enum CLUSTER_SHARED_REDIRECTED_IO_REASON : ulong
	{
		UserRequest = 1uL,
		UnsafeFileSystemFilter = 2uL,
		UnsafeVolumeFilter = 4uL,
		FileSystemTiering = 8uL,
		Max = 9223372036854775808uL
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct CLUSTER_SHARED_VOLUME_STATE_INFO_EX
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szVolumeName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szNodeName;

		public CLUSTER_SHARED_VOLUME_STATE VolumeState;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szVolumeFriendlyName;

		public CLUSTER_SHARED_REDIRECTED_IO_REASON RedirectedIoReason;

		public CLUSTER_SHARED_VOLUME_REDIRECTED_IO_REASON VolumeRedirectedIoReason;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct CLUS_PARTITION_INFO
	{
		public uint flags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string deviceName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string volumeLabel;

		public uint serialNumber;

		public uint rgdwMaximumComponentLength;

		public uint fileSystemFlags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string fileSystem;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct CLUS_PARTITION_INFO_EX
	{
		public uint flags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string deviceName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string volumeLabel;

		public uint serialNumber;

		public uint rgdwMaximumComponentLength;

		public uint fileSystemFlags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string fileSystem;

		public ulong totalSizeInBytes;

		public ulong freeSizeInBytes;

		public uint deviceNumber;

		public uint partitionNumber;

		public Guid volumeGuid;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct CLUS_PARTITION_INFO_EX2
	{
		public uint flags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string deviceName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string volumeLabel;

		public uint serialNumber;

		public uint rgdwMaximumComponentLength;

		public uint fileSystemFlags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string fileSystem;

		public ulong totalSizeInBytes;

		public ulong freeSizeInBytes;

		public uint deviceNumber;

		public uint partitionNumber;

		public Guid volumeGuid;

		public Guid gptPartitionId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string partitionName;

		public uint bitLockerFlags;
	}

	internal enum CLUSTER_REG_COMMAND
	{
		CLUSREG_COMMAND_NONE,
		CLUSREG_SET_VALUE,
		CLUSREG_CREATE_KEY,
		CLUSREG_DELETE_KEY,
		CLUSREG_DELETE_VALUE,
		CLUSREG_SET_KEY_SECURITY,
		CLUSREG_VALUE_DELETED,
		CLUSREG_READ_KEY,
		CLUSREG_READ_VALUE,
		CLUSREG_READ_ERROR,
		CLUSREG_LAST_COMMAND
	}

	[Flags]
	internal enum CLUSTER_CHANGE : uint
	{
		CLUSTER_CHANGE_NODE_STATE = 1u,
		CLUSTER_CHANGE_NODE_DELETED = 2u,
		CLUSTER_CHANGE_NODE_ADDED = 4u,
		CLUSTER_CHANGE_NODE_PROPERTY = 8u,
		CLUSTER_CHANGE_REGISTRY_NAME = 0x10u,
		CLUSTER_CHANGE_REGISTRY_ATTRIBUTES = 0x20u,
		CLUSTER_CHANGE_REGISTRY_VALUE = 0x40u,
		CLUSTER_CHANGE_REGISTRY_SUBTREE = 0x80u,
		CLUSTER_CHANGE_RESOURCE_STATE = 0x100u,
		CLUSTER_CHANGE_RESOURCE_DELETED = 0x200u,
		CLUSTER_CHANGE_RESOURCE_ADDED = 0x400u,
		CLUSTER_CHANGE_RESOURCE_PROPERTY = 0x800u,
		CLUSTER_CHANGE_GROUP_STATE = 0x1000u,
		CLUSTER_CHANGE_GROUP_DELETED = 0x2000u,
		CLUSTER_CHANGE_GROUP_ADDED = 0x4000u,
		CLUSTER_CHANGE_GROUP_PROPERTY = 0x8000u,
		CLUSTER_CHANGE_RESOURCE_TYPE_DELETED = 0x10000u,
		CLUSTER_CHANGE_RESOURCE_TYPE_ADDED = 0x20000u,
		CLUSTER_CHANGE_RESOURCE_TYPE_PROPERTY = 0x40000u,
		CLUSTER_CHANGE_CLUSTER_RECONNECT = 0x80000u,
		CLUSTER_CHANGE_NETWORK_STATE = 0x100000u,
		CLUSTER_CHANGE_NETWORK_DELETED = 0x200000u,
		CLUSTER_CHANGE_NETWORK_ADDED = 0x400000u,
		CLUSTER_CHANGE_NETWORK_PROPERTY = 0x800000u,
		CLUSTER_CHANGE_NETINTERFACE_STATE = 0x1000000u,
		CLUSTER_CHANGE_NETINTERFACE_DELETED = 0x2000000u,
		CLUSTER_CHANGE_NETINTERFACE_ADDED = 0x4000000u,
		CLUSTER_CHANGE_NETINTERFACE_PROPERTY = 0x8000000u,
		CLUSTER_CHANGE_QUORUM_STATE = 0x10000000u,
		CLUSTER_CHANGE_CLUSTER_STATE = 0x20000000u,
		CLUSTER_CHANGE_CLUSTER_PROPERTY = 0x40000000u,
		CLUSTER_CHANGE_HANDLE_CLOSE = 0x80000000u,
		CLUSTER_CHANGE_ALL = uint.MaxValue
	}

	[Flags]
	internal enum TASKDIALOG_COMMON_BUTTON_FLAGS
	{
		TDCBF_OK_BUTTON = 1,
		TDCBF_YES_BUTTON = 2,
		TDCBF_NO_BUTTON = 4,
		TDCBF_CANCEL_BUTTON = 8,
		TDCBF_RETRY_BUTTON = 0x10,
		TDCBF_CLOSE_BUTTON = 0x20
	}

	[Flags]
	internal enum CLUSTER_CHANGE_CLUSTER_V2 : ulong
	{
		CLUSTER_CHANGE_CLUSTER_RECONNECT_V2 = 1uL,
		CLUSTER_CHANGE_CLUSTER_STATE_V2 = 2uL,
		CLUSTER_CHANGE_CLUSTER_GROUP_ADDED_V2 = 4uL,
		CLUSTER_CHANGE_CLUSTER_HANDLE_CLOSE_V2 = 8uL,
		CLUSTER_CHANGE_CLUSTER_NETWORK_ADDED_V2 = 0x10uL,
		CLUSTER_CHANGE_CLUSTER_NODE_ADDED_V2 = 0x20uL,
		CLUSTER_CHANGE_CLUSTER_RESOURCE_TYPE_ADDED_V2 = 0x40uL,
		CLUSTER_CHANGE_CLUSTER_COMMON_PROPERTY_V2 = 0x80uL,
		CLUSTER_CHANGE_CLUSTER_PRIVATE_PROPERTY_V2 = 0x100uL,
		CLUSTER_CHANGE_CLUSTER_LOST_NOTIFICATIONS_V2 = 0x200uL,
		CLUSTER_CHANGE_CLUSTER_RENAME_V2 = 0x400uL,
		CLUSTER_CHANGE_CLUSTER_MEMBERSHIP_V2 = 0x800uL,
		CLUSTER_CHANGE_CLUSTER_ALL_V2 = 0xFFFuL
	}

	[Flags]
	internal enum CLUSTER_CHANGE_GROUP_V2 : ulong
	{
		CLUSTER_CHANGE_GROUP_DELETED_V2 = 1uL,
		CLUSTER_CHANGE_GROUP_COMMON_PROPERTY_V2 = 2uL,
		CLUSTER_CHANGE_GROUP_PRIVATE_PROPERTY_V2 = 4uL,
		CLUSTER_CHANGE_GROUP_STATE_V2 = 8uL,
		CLUSTER_CHANGE_GROUP_OWNER_NODE_V2 = 0x10uL,
		CLUSTER_CHANGE_GROUP_PREFERRED_OWNERS_V2 = 0x20uL,
		CLUSTER_CHANGE_GROUP_RESOURCE_ADDED_V2 = 0x40uL,
		CLUSTER_CHANGE_GROUP_RESOURCE_GAINED_V2 = 0x80uL,
		CLUSTER_CHANGE_GROUP_RESOURCE_LOST_V2 = 0x100uL,
		CLUSTER_CHANGE_GROUP_HANDLE_CLOSE_V2 = 0x200uL,
		CLUSTER_CHANGE_GROUP_ALL_V2 = 0x3FFuL
	}

	[Flags]
	internal enum CLUSTER_CHANGE_RESOURCE_V2 : ulong
	{
		CLUSTER_CHANGE_RESOURCE_COMMON_PROPERTY_V2 = 1uL,
		CLUSTER_CHANGE_RESOURCE_PRIVATE_PROPERTY_V2 = 2uL,
		CLUSTER_CHANGE_RESOURCE_STATE_V2 = 4uL,
		CLUSTER_CHANGE_RESOURCE_OWNER_GROUP_V2 = 8uL,
		CLUSTER_CHANGE_RESOURCE_DEPENDENCIES_V2 = 0x10uL,
		CLUSTER_CHANGE_RESOURCE_DEPENDENTS_V2 = 0x20uL,
		CLUSTER_CHANGE_RESOURCE_POSSIBLE_OWNERS_V2 = 0x40uL,
		CLUSTER_CHANGE_RESOURCE_DELETED_V2 = 0x80uL,
		CLUSTER_CHANGE_RESOURCE_DLL_UPGRADED_V2 = 0x100uL,
		CLUSTER_CHANGE_RESOURCE_HANDLE_CLOSE_V2 = 0x200uL,
		CLUSTER_CHANGE_RESOURCE_TERMINAL_STATE_V2 = 0x400uL,
		CLUSTER_CHANGE_RESOURCE_ALL_V2 = 0x7FFuL
	}

	[Flags]
	internal enum CLUSTER_CHANGE_RESOURCE_TYPE_V2 : ulong
	{
		CLUSTER_CHANGE_RESOURCE_TYPE_DELETED_V2 = 1uL,
		CLUSTER_CHANGE_RESOURCE_TYPE_COMMON_PROPERTY_V2 = 2uL,
		CLUSTER_CHANGE_RESOURCE_TYPE_PRIVATE_PROPERTY_V2 = 4uL,
		CLUSTER_CHANGE_RESOURCE_TYPE_POSSIBLE_OWNERS_V2 = 8uL,
		CLUSTER_CHANGE_RESOURCE_TYPE_DLL_UPGRADED_V2 = 0x10uL,
		CLUSTER_RESOURCE_TYPE_SPECIFIC_V2 = 0x20uL,
		CLUSTER_CHANGE_RESOURCE_TYPE_ALL_V2 = 0x3FuL
	}

	[Flags]
	internal enum CLUSTER_CHANGE_NETINTERFACE_V2 : ulong
	{
		CLUSTER_CHANGE_NETINTERFACE_DELETED_V2 = 1uL,
		CLUSTER_CHANGE_NETINTERFACE_COMMON_PROPERTY_V2 = 2uL,
		CLUSTER_CHANGE_NETINTERFACE_PRIVATE_PROPERTY_V2 = 4uL,
		CLUSTER_CHANGE_NETINTERFACE_STATE_V2 = 8uL,
		CLUSTER_CHANGE_NETINTERFACE_HANDLE_CLOSE_V2 = 0x10uL,
		CLUSTER_CHANGE_NETINTERFACE_ALL_V2 = 0x1FuL
	}

	[Flags]
	internal enum CLUSTER_CHANGE_NETWORK_V2 : ulong
	{
		CLUSTER_CHANGE_NETWORK_DELETED_V2 = 1uL,
		CLUSTER_CHANGE_NETWORK_COMMON_PROPERTY_V2 = 2uL,
		CLUSTER_CHANGE_NETWORK_PRIVATE_PROPERTY_V2 = 4uL,
		CLUSTER_CHANGE_NETWORK_STATE_V2 = 8uL,
		CLUSTER_CHANGE_NETWORK_HANDLE_CLOSE_V2 = 0x10uL,
		CLUSTER_CHANGE_NETWORK_ALL_V2 = 0x1FuL
	}

	[Flags]
	internal enum CLUSTER_CHANGE_NODE_V2 : ulong
	{
		CLUSTER_CHANGE_NODE_NETINTERFACE_ADDED_V2 = 1uL,
		CLUSTER_CHANGE_NODE_DELETED_V2 = 2uL,
		CLUSTER_CHANGE_NODE_COMMON_PROPERTY_V2 = 4uL,
		CLUSTER_CHANGE_NODE_PRIVATE_PROPERTY_V2 = 8uL,
		CLUSTER_CHANGE_NODE_STATE_V2 = 0x10uL,
		CLUSTER_CHANGE_NODE_GROUP_GAINED_V2 = 0x20uL,
		CLUSTER_CHANGE_NODE_GROUP_LOST_V2 = 0x40uL,
		CLUSTER_CHANGE_NODE_HANDLE_CLOSE_V2 = 0x80uL,
		CLUSTER_CHANGE_NODE_ALL_V2 = 0xFFuL
	}

	[Flags]
	internal enum CLUSTER_CHANGE_REGISTRY_V2 : ulong
	{
		CLUSTER_CHANGE_REGISTRY_ATTRIBUTES_V2 = 1uL,
		CLUSTER_CHANGE_REGISTRY_NAME_V2 = 2uL,
		CLUSTER_CHANGE_REGISTRY_SUBTREE_V2 = 4uL,
		CLUSTER_CHANGE_REGISTRY_VALUE_V2 = 8uL,
		CLUSTER_CHANGE_REGISTRY_HANDLE_CLOSE_V2 = 0x10uL,
		CLUSTER_CHANGE_REGISTRY_ALL_V2 = 0x1FuL
	}

	[Flags]
	internal enum CLUSTER_CHANGE_QUORUM_V2 : ulong
	{
		CLUSTER_CHANGE_QUORUM_STATE_V2 = 1uL,
		CLUSTER_CHANGE_QUORUM_ALL_V2 = 1uL
	}

	[Flags]
	internal enum CLUSTER_CHANGE_SHARED_VOLUME_V2 : ulong
	{
		CLUSTER_CHANGE_SHARED_VOLUME_STATE_V2 = 1uL,
		CLUSTER_CHANGE_SHARED_VOLUME_ADDED_V2 = 2uL,
		CLUSTER_CHANGE_SHARED_VOLUME_REMOVED_V2 = 4uL,
		CLUSTER_CHANGE_SHARED_VOLUME_ALL_V2 = 7uL
	}

	internal enum CLUSTER_CHANGE_SPACEPORT_V2 : ulong
	{
		CLUSTER_CHANGE_SPACEPORT_CUSTOM_PNP_V2 = 1uL
	}

	internal enum CLUSTER_CHANGE_NODE_UPGRADE_PHASE_V2
	{
		CLUSTER_CHANGE_UPGRADE_NODE_PREPARE = 1,
		CLUSTER_CHANGE_UPGRADE_NODE_COMMIT = 2,
		CLUSTER_CHANGE_UPGRADE_NODE_POSTCOMMIT = 4,
		CLUSTER_CHANGE_UPGRADE_ALL_V2 = 7
	}

	internal enum CLUSTER_OBJECT_TYPE : uint
	{
		CLUSTER_OBJECT_TYPE_NONE = 0u,
		CLUSTER_OBJECT_TYPE_CLUSTER = 1u,
		CLUSTER_OBJECT_TYPE_GROUP = 2u,
		CLUSTER_OBJECT_TYPE_RESOURCE = 3u,
		CLUSTER_OBJECT_TYPE_RESOURCE_TYPE = 4u,
		CLUSTER_OBJECT_TYPE_NETWORK_INTERFACE = 5u,
		CLUSTER_OBJECT_TYPE_NETWORK = 6u,
		CLUSTER_OBJECT_TYPE_NODE = 7u,
		CLUSTER_OBJECT_TYPE_REGISTRY = 8u,
		CLUSTER_OBJECT_TYPE_QUORUM = 9u,
		CLUSTER_OBJECT_TYPE_SHARED_VOLUME = 10u,
		CLUSTER_OBJECT_TYPE_SPACEPORT = 11u,
		CLUSTER_OBJECT_TYPE_UPGRADE = 12u,
		CLUSTER_OBJECT_TYPE_UNKNOWN = uint.MaxValue
	}

	[Flags]
	internal enum CLUSTER_GROUP_OVERRIDES : uint
	{
		CLUSAPI_GROUP_IGNORE_RESOURCE_STATUS = 1u,
		CLUSAPI_GROUP_MOVE_RETURN_TO_SOURCE_NODE_ON_ERROR = 2u,
		CLUSAPI_GROUP_MOVE_QUEUE_ENABLED = 4u,
		CLUSAPI_GROUP_MOVE_HIGH_PRIORITY_START = 8u,
		CLUSAPI_GROUP_MOVE_FAILBACK = 0x10u,
		CLUSAPI_GROUP_ONLINE_BEST_POSSIBLE_NODE = 4u
	}

	[Flags]
	internal enum CLUSTER_RESOURCE_OVERRIDES : uint
	{
		CLUSAPI_RESOURCE_IGNORE_RESOURCE_STATUS = 1u,
		CLUSAPI_RESOURCE_OFFLINE_FORCE_WITH_TERMINATION = 2u,
		CLUSAPI_RESOURCE_ONLINE_BEST_POSSIBLE_NODE = 8u
	}

	internal enum CLUSTER_CONTROL_OBJECT
	{
		CLUS_OBJECT_INVALID = 0,
		CLUS_OBJECT_RESOURCE = 1,
		CLUS_OBJECT_RESOURCE_TYPE = 2,
		CLUS_OBJECT_GROUP = 3,
		CLUS_OBJECT_NODE = 4,
		CLUS_OBJECT_NETWORK = 5,
		CLUS_OBJECT_NETINTERFACE = 6,
		CLUS_OBJECT_CLUSTER = 7,
		CLUS_OBJECT_USER = 128
	}

	[Flags]
	internal enum GroupEnumType : uint
	{
		Resource = 1u,
		Node = 2u,
		All = 3u
	}

	[Flags]
	internal enum NetworkEnumType : uint
	{
		NetInterface = 1u,
		All = 1u
	}

	[Flags]
	internal enum ResourceTypeEnumType : uint
	{
		Node = 1u,
		Resource = 2u,
		All = 3u
	}

	[Flags]
	internal enum ResourceEnumType : uint
	{
		Dependencies = 1u,
		Dependants = 2u,
		Nodes = 4u
	}

	[Flags]
	internal enum ClusterEnumType : uint
	{
		Node = 1u,
		ResourceType = 2u,
		Resource = 4u,
		Group = 8u,
		Network = 0x10u,
		NetworkInterface = 0x20u,
		ClusterFileSystem = 0x40000000u,
		InternalNetwork = 0x80000000u,
		All = 0x4000003Fu
	}

	[Flags]
	internal enum NodePauseDrainFlag : uint
	{
		None = 0u,
		NodePauseRemainOnPausedNodeOnMoveError = 1u
	}

	internal enum ClusterRegistryValueType
	{
		None,
		String,
		ExpandedString,
		Binary,
		Integer,
		IntegerBigEndian,
		MultiString
	}

	internal enum ClusterPropertySyntax : uint
	{
		DiskGuid = 720899u,
		DiskNumber = 458754u,
		DiskSerialNumber = 655363u,
		DiskSignature = 327682u,
		DiskSize = 786438u,
		EndMark = 0u,
		ListValueBinary = 65537u,
		ListValueDWord = 65538u,
		ListValueExpandSz = 65540u,
		ListValueExpandedSz = 65544u,
		ListValueSecurityDescriptor = 65545u,
		ListValueFileTime = 65548u,
		ListValueLong = 65543u,
		ListValueMultiSz = 65541u,
		ListValueSz = 65539u,
		ListValueUnsignedLargeInt = 65542u,
		Name = 262147u,
		PartitionInfo = 524289u,
		PartitionInfoEx = 851969u,
		PartitionInfoEx2 = 917505u,
		ResourceClass = 131074u,
		SCSIAddress = 393218u
	}

	internal enum ClusterResourceCreateFlags
	{
		DefaultMonitor,
		SeparateMonitor
	}

	internal struct CLUSDSK_DISKID
	{
		public DiskIdType DiskIdType;

		public ulong DiskSignature;

		public Guid DiskGuid;

		public ulong DeviceNumber;
	}

	internal struct CLUSTER_READ_BATCH_COMMAND
	{
		public CLUSTER_REG_COMMAND Command;

		public int dwOptions;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string wzSubkeyName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string wzValueName;

		public IntPtr lpData;

		public int cbData;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct CLUSPROP_SYNTAX
	{
		[FieldOffset(0)]
		public uint dw;

		[FieldOffset(0)]
		public ushort wType;

		[FieldOffset(2)]
		public ushort wFormat;
	}

	internal struct CLUSPROP_VALUE
	{
		public CLUSPROP_SYNTAX syntax;

		public int cbLength;
	}

	internal struct CLUSPROP_BINARY_CSV_INFO
	{
		public CLUSPROP_VALUE value;

		public CLUS_CSV_VOLUME_INFO csvInfo;
	}

	internal struct CLUSPROP_BINARY_CSV_STATE_INFO
	{
		public CLUSPROP_VALUE value;

		public CLUSTER_SHARED_VOLUME_STATE_INFO csvStateInfo;
	}

	internal struct CLUSPROP_BINARY_CSV_STATE_INFO_EX
	{
		public CLUSPROP_VALUE value;

		public CLUSTER_SHARED_VOLUME_STATE_INFO_EX csvStateInfo;
	}

	internal struct CLUSPROP_RESOURCE_CLASS
	{
		public CLUSPROP_VALUE value;

		public ResourceClass rc;
	}

	internal struct CLUSPROP_DWORD
	{
		public CLUSPROP_VALUE value;

		public uint dw;
	}

	internal struct CLUSPROP_ULARGE_INTEGER
	{
		public CLUSPROP_VALUE value;

		public ulong li;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Size = 4096)]
	internal struct CLUSPROP_SZ
	{
		public CLUSPROP_VALUE value;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2043)]
		public string sz;
	}

	internal struct CLUSPROP_DISK_SIGNATURE
	{
		public CLUSPROP_VALUE value;

		public uint dw;
	}

	internal struct CLUSPROP_PARTITION_INFO
	{
		public CLUSPROP_VALUE value;

		public CLUS_PARTITION_INFO partitionInfo;
	}

	internal struct CLUSPROP_PARTITION_INFO_EX
	{
		public CLUSPROP_VALUE value;

		public CLUS_PARTITION_INFO_EX partitionInfoEx;
	}

	internal struct CLUSPROP_PARTITION_INFO_EX2
	{
		public CLUSPROP_VALUE value;

		public CLUS_PARTITION_INFO_EX2 partitionInfoEx2;
	}

	internal struct CLUSPROP_DISK_NUMBER
	{
		public CLUSPROP_VALUE value;

		public uint dw;
	}

	internal struct CLUSPROP_REQUIRED_DEPENDENCY_ResourceClass
	{
		public CLUSPROP_RESOURCE_CLASS ResClass;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal struct CLUSPROP_REQUIRED_DEPENDENCY_ResourceType
	{
		public CLUSPROP_SZ ResTypeName;
	}

	internal struct CLUSTER_CREATE_GROUP_INFO
	{
		public int version;

		public int groupType;

		public CLUSTER_CREATE_GROUP_INFO(int groupType)
		{
			version = CLUSTER_CREATE_GROUP_INFO_VERSION_1;
			this.groupType = groupType;
		}
	}

	internal struct CLUSTER_ENUM_ITEM
	{
		public int dwVersion;

		public int dwType;

		public int cbId;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpszId;

		public int cbName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpszName;
	}

	internal struct CLUSTER_FILTER_GROUP_ITEM
	{
		public int dwVersion;

		public int cbId;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpszId;

		public int cbName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpszName;

		public GroupState state;

		public int cbOwnerNode;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpszOwnerNode;

		public int dwFlags;

		public int cbProperties;

		public IntPtr pProperties;

		public int cbRoProperties;

		public IntPtr pRoProperties;
	}

	internal struct CLUSTER_FILTER_RESOURCE_ITEM
	{
		public int dwVersion;

		public int cbId;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpszId;

		public int cbName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpszName;

		public int cbOwnerGroupName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpszOwnerGroupName;

		public int cbOwnerGroupId;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpszOwnerGroupId;

		public int cbProperties;

		public IntPtr pProperties;

		public int cbRoProperties;

		public IntPtr pRoProperties;
	}

	[StructLayout(LayoutKind.Sequential, Size = 284)]
	internal struct CLUSTERVERSIONINFO
	{
		internal int dwVersionInfoSize;

		internal short MajorVersion;

		internal short MinorVersion;

		internal short BuildNumber;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		internal string szVendorId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		internal string szCSDVersion;

		internal int dwClusterHighestVersion;

		internal int dwClusterLowestVersion;

		internal int dwFlags;

		internal int dwReserved;
	}

	internal struct NOTIFY_FILTER_AND_TYPE
	{
		internal CLUSTER_OBJECT_TYPE ObjectType;

		internal ulong FilterFlags;

		public NOTIFY_FILTER_AND_TYPE(CLUSTER_OBJECT_TYPE dwObjectType, ulong FilterFlags)
		{
			ObjectType = dwObjectType;
			this.FilterFlags = FilterFlags;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(ObjectType.ToString());
			stringBuilder.Append("/");
			switch (ObjectType)
			{
			case CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_CLUSTER:
			{
				CLUSTER_CHANGE_CLUSTER_V2 filterFlags10 = (CLUSTER_CHANGE_CLUSTER_V2)FilterFlags;
				stringBuilder.Append(filterFlags10.ToString());
				break;
			}
			case CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_GROUP:
			{
				CLUSTER_CHANGE_GROUP_V2 filterFlags9 = (CLUSTER_CHANGE_GROUP_V2)FilterFlags;
				stringBuilder.Append(filterFlags9.ToString());
				break;
			}
			case CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_RESOURCE:
			{
				CLUSTER_CHANGE_RESOURCE_V2 filterFlags8 = (CLUSTER_CHANGE_RESOURCE_V2)FilterFlags;
				stringBuilder.Append(filterFlags8.ToString());
				break;
			}
			case CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_RESOURCE_TYPE:
			{
				CLUSTER_CHANGE_RESOURCE_TYPE_V2 filterFlags7 = (CLUSTER_CHANGE_RESOURCE_TYPE_V2)FilterFlags;
				stringBuilder.Append(filterFlags7.ToString());
				break;
			}
			case CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NETWORK_INTERFACE:
			{
				CLUSTER_CHANGE_NETINTERFACE_V2 filterFlags6 = (CLUSTER_CHANGE_NETINTERFACE_V2)FilterFlags;
				stringBuilder.Append(filterFlags6.ToString());
				break;
			}
			case CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NETWORK:
			{
				CLUSTER_CHANGE_NETWORK_V2 filterFlags5 = (CLUSTER_CHANGE_NETWORK_V2)FilterFlags;
				stringBuilder.Append(filterFlags5.ToString());
				break;
			}
			case CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NODE:
			{
				CLUSTER_CHANGE_NODE_V2 filterFlags4 = (CLUSTER_CHANGE_NODE_V2)FilterFlags;
				stringBuilder.Append(filterFlags4.ToString());
				break;
			}
			case CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_REGISTRY:
			{
				CLUSTER_CHANGE_REGISTRY_V2 filterFlags3 = (CLUSTER_CHANGE_REGISTRY_V2)FilterFlags;
				stringBuilder.Append(filterFlags3.ToString());
				break;
			}
			case CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_QUORUM:
			{
				CLUSTER_CHANGE_QUORUM_V2 filterFlags2 = (CLUSTER_CHANGE_QUORUM_V2)FilterFlags;
				stringBuilder.Append(filterFlags2.ToString());
				break;
			}
			case CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_SHARED_VOLUME:
			{
				CLUSTER_CHANGE_SHARED_VOLUME_V2 filterFlags = (CLUSTER_CHANGE_SHARED_VOLUME_V2)FilterFlags;
				stringBuilder.Append(filterFlags.ToString());
				break;
			}
			}
			return stringBuilder.ToString();
		}
	}

	internal struct CLUS_DISK_NUMBER_INFO
	{
		public uint diskNumber;

		public uint bytesPerSector;
	}

	internal struct CLUSTER_MAINTENANCE_MODE_INFO
	{
		public bool bInMaintenance;
	}

	internal delegate bool ClusterSetupProgressCallback(IntPtr callbackArgs, ClusterSetupPhrase setupPhrase, ClusterSetupPhraseType phraseType, ClusterSetupPhraseSeverity phraseSeverity, int percentComplete, string objectName, int status);

	public delegate bool ProgressCallback(IntPtr callbackArg, ClusterSetupPhrase phase, ClusterSetupPhraseType type, ClusterSetupPhraseSeverity severity, int percent, [MarshalAs(UnmanagedType.LPWStr)] string name, int status);

	internal delegate int PFTASKDIALOGCALLBACK(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr lpRefData);

	[Flags]
	internal enum TASKDIALOG_FLAGS
	{
		NONE = 0,
		TDF_ENABLE_HYPERLINKS = 1,
		TDF_USE_HICON_MAIN = 2,
		TDF_USE_HICON_FOOTER = 4,
		TDF_ALLOW_DIALOG_CANCELLATION = 8,
		TDF_USE_COMMAND_LINKS = 0x10,
		TDF_USE_COMMAND_LINKS_NO_ICON = 0x20,
		TDF_EXPAND_FOOTER_AREA = 0x40,
		TDF_EXPANDED_BY_DEFAULT = 0x80,
		TDF_VERIFICATION_FLAG_CHECKED = 0x100,
		TDF_SHOW_PROGRESS_BAR = 0x200,
		TDF_SHOW_MARQUEE_PROGRESS_BAR = 0x400,
		TDF_CALLBACK_TIMER = 0x800,
		TDF_POSITION_RELATIVE_TO_WINDOW = 0x1000,
		TDF_RTL_LAYOUT = 0x2000,
		TDF_NO_DEFAULT_RADIO_BUTTON = 0x4000
	}

	internal enum ErrorCode
	{
		None = 0,
		FileNotFound = 2,
		DeletePending = 303,
		SharingPaused = 70,
		EndpointNotRegistered = 1753,
		AccessDenied = 5,
		RpcCallCanceled = 2,
		RpcServerUnavailable = 1722,
		RpcCallFailed = 1726,
		RpcCallFailedDne = 1727,
		RpcSecurityPackage = 1825,
		InvalidFunction = 1,
		MaxNumberOfConnections = 71,
		InvalidParameter = 87,
		InvalidState = 5023,
		InvalidHandle = 6,
		ClusterEvictWithoutCleanup = 5896,
		BadNetPath = 53,
		DependencyNotFound = 5002,
		InsufficientBuffer = 122,
		NoMoreItems = 259,
		EventMessageNotFound = 15027,
		EventMessageIdNotFound = 15028,
		UserNotFound = 2221,
		DependencyAlreadyExists = 5003,
		MoreData = 234,
		NotFound = 1168,
		ClusterNoQuorum = 5925,
		ResourceOnline = 5019,
		ClusterLastInternalNetwork = 5066,
		ServiceDoesNotExist = 1060,
		ObjectAlreadyExists = 5010,
		ResourceNotFound = 5007,
		ResourceNotAvailable = 5006,
		GroupNotFound = 5013,
		GroupNotAvailable = 5012,
		GroupMoving = 5908,
		NoSecurityOnObject = 1350,
		NodeNotFound = 5042,
		NodeNotAvailable = 5036,
		NetworkNotFound = 5045,
		NetworkNotAvailable = 5035,
		NetworkInterfaceNotFound = 5047,
		IOPending = 997,
		DnsNameError = 9003,
		ErrorAlreadyExists = 183,
		AllNodesNotAvailable = 5037,
		TimeOut = 258,
		ResourcePropertiesStored = 5024,
		ResourceFailed = 5038,
		ResourceTypeNotFound = 5078,
		ResourceTypeNotSupported = 5079,
		DiskNotCsvCapable = 5964,
		PropertiesNotAvailable = 5968,
		ResourceLocked = 5960,
		ClusterInvalidRequest = 5048,
		FileShareResourceConflict = 5938,
		ResourceIsInMaintenanceMode = 5970,
		ResourceIsReplicaVirtualMachine = 5972,
		ClusterUpgradeIncompatibleVersions = 5973,
		ClusterUpgradeFixQuorumNotSupported = 5974,
		ClusterUpgradeRestartRequired = 5975,
		ResourceNotOnline = 5004,
		InvalidData = 13
	}

	internal enum WVR_EVENT_TYPE
	{
		WvrEventTypeMin,
		WvrEventTypeGroupCreated,
		WvrEventTypeGroupDeleted,
		WvrEventTypeGroupModified,
		WvrEventTypeAddReplica,
		WvrEventTypeRemoveReplica,
		WvrEventTypePartnershipCreated,
		WvrEventTypePartnershipDestroyed,
		WvrEventTypeRoleSwitched,
		WvrEventTypeReplicationStatusChanged,
		WvrEventTypeRecoveryStatusChanged,
		WvrEventTypeGroupRoleChanged,
		WvrEventTypeGroupPartnershipChanged,
		WvrEventTypeMax
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct WVR_RESOURCE_EVENT_HEADER
	{
		internal uint Version;

		internal WVR_EVENT_TYPE EventType;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct WVR_RESOURCE_TYPE_REPLICA_STATE_NOTIFICATION
	{
		internal WVR_RESOURCE_EVENT_HEADER Header;

		internal uint ReplicationState;

		internal uint PercentageRecovered;

		internal Guid PartitionId;

		internal Guid ReplicationGroupId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		internal string ReplicationGroupName;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct WVR_RESOURCE_TYPE_REPLICATION_GROUP_NOTIFICATION
	{
		internal WVR_RESOURCE_EVENT_HEADER Header;

		internal Guid ReplicationGroupId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		internal string ReplicationGroupName;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct WVR_RESOURCE_TYPE_REPLICATION_GROUP_MODIFIED_NOTIFICATION
	{
		internal WVR_RESOURCE_EVENT_HEADER Header;

		internal Guid ClusterGroupId;

		internal uint GroupType;

		internal Guid ReplicationGroupId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		internal string ReplicationGroupName;

		internal SR_RESOURCE_TYPE_REPLICATED_DISKS_RESULT ReplicatedDisks;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct WVR_RESOURCE_TYPE_REPLICATION_PARTNERSHIP_NOTIFICATION
	{
		internal WVR_RESOURCE_EVENT_HEADER Header;

		internal Guid SourceClusterGroupId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		internal string SourceReplicationGroupName;

		internal uint SourceGroupType;

		internal Guid TargetClusterGroupId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		internal string TargetReplicationGroupName;

		internal uint TargetGroupType;

		internal SR_RESOURCE_TYPE_REPLICATED_DISKS_RESULT ReplicatedDisks;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct SR_RESOURCE_TYPE_REPLICATED_DISK
	{
		internal ReplicationDiskType Role;

		internal Guid ClusterResourceId;

		internal Guid ReplicationGroupId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string ReplicationGroupName;
	}

	internal struct SR_RESOURCE_TYPE_REPLICATED_DISKS_RESULT
	{
		internal ushort Count;

		internal IntPtr ReplicatedDisks;
	}

	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
	internal struct TASKDIALOGCONFIG_ICON_UNION
	{
		[FieldOffset(0)]
		internal int hMainIcon;

		[FieldOffset(0)]
		internal int pszIcon;

		[FieldOffset(0)]
		internal IntPtr spacer;

		internal TASKDIALOGCONFIG_ICON_UNION(int i)
		{
			spacer = IntPtr.Zero;
			pszIcon = 0;
			hMainIcon = i;
		}
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
	internal class TASKDIALOGCONFIG
	{
		internal uint cbSize;

		internal IntPtr hwndParent;

		internal IntPtr hInstance;

		internal TASKDIALOG_FLAGS dwFlags;

		internal TASKDIALOG_COMMON_BUTTON_FLAGS dwCommonButtons;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pszWindowTitle;

		internal TASKDIALOGCONFIG_ICON_UNION MainIcon;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pszMainInstruction;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pszContent;

		internal uint cButtons;

		internal IntPtr pButtons;

		internal int nDefaultButton;

		internal uint cRadioButtons;

		internal IntPtr pRadioButtons;

		internal int nDefaultRadioButton;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pszVerificationText;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pszExpandedInformation;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pszExpandedControlText;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pszCollapsedControlText;

		internal TASKDIALOGCONFIG_ICON_UNION FooterIcon;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pszFooter;

		internal PFTASKDIALOGCALLBACK pfCallback;

		internal IntPtr lpCallbackData;

		internal uint cxWidth;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct GRPICONDIRENTRY
	{
		public byte bWidth;

		public byte bHeight;

		public byte bColorCount;

		public byte bReserved;

		public ushort wPlanes;

		public ushort wBitCount;

		public uint dwBytesInRes;

		public ushort nID;

		public GRPICONDIRENTRY(Stream stream)
		{
			this = default(GRPICONDIRENTRY);
			Read(stream);
		}

		public unsafe void Read(Stream stream)
		{
			byte[] array = new byte[sizeof(GRPICONDIRENTRY)];
			stream.Read(array, 0, sizeof(GRPICONDIRENTRY));
			fixed (byte* ptr = array)
			{
				this = *(GRPICONDIRENTRY*)ptr;
			}
		}

		public unsafe void Write(Stream stream)
		{
			byte[] array = new byte[sizeof(GRPICONDIRENTRY)];
			fixed (GRPICONDIRENTRY* ptr = &this)
			{
				Marshal.Copy((IntPtr)ptr, array, 0, sizeof(GRPICONDIRENTRY));
			}
			stream.Write(array, 0, sizeof(GRPICONDIRENTRY));
		}

		public ICONDIRENTRY ToIconDirEntry()
		{
			ICONDIRENTRY result = default(ICONDIRENTRY);
			result.bColorCount = bColorCount;
			result.bHeight = bHeight;
			result.bReserved = bReserved;
			result.bWidth = bWidth;
			result.dwBytesInRes = dwBytesInRes;
			result.wBitCount = wBitCount;
			result.wPlanes = wPlanes;
			return result;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct ICONDIRENTRY
	{
		public byte bWidth;

		public byte bHeight;

		public byte bColorCount;

		public byte bReserved;

		public ushort wPlanes;

		public ushort wBitCount;

		public uint dwBytesInRes;

		public uint dwImageOffset;

		public ICONDIRENTRY(Stream stream)
		{
			this = default(ICONDIRENTRY);
			Read(stream);
		}

		public unsafe void Read(Stream stream)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			byte[] array = new byte[sizeof(ICONDIRENTRY)];
			binaryReader.Read(array, 0, sizeof(ICONDIRENTRY));
			fixed (byte* ptr = array)
			{
				this = *(ICONDIRENTRY*)ptr;
			}
		}

		public unsafe void Write(Stream stream)
		{
			byte[] array = new byte[sizeof(ICONDIRENTRY)];
			fixed (ICONDIRENTRY* ptr = &this)
			{
				Marshal.Copy((IntPtr)ptr, array, 0, sizeof(ICONDIRENTRY));
			}
			stream.Write(array, 0, sizeof(ICONDIRENTRY));
		}

		public GRPICONDIRENTRY ToGrpIconEntry()
		{
			GRPICONDIRENTRY result = default(GRPICONDIRENTRY);
			result.bColorCount = bColorCount;
			result.bHeight = bHeight;
			result.bReserved = bReserved;
			result.bWidth = bWidth;
			result.dwBytesInRes = dwBytesInRes;
			result.wBitCount = wBitCount;
			result.wPlanes = wPlanes;
			return result;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct ICONDIR
	{
		public ushort idReserved;

		public ushort idType;

		public ushort idCount;

		public static ICONDIR Initalizated => new ICONDIR(0, 1, 0);

		public ICONDIR(ushort reserved, ushort type, ushort count)
		{
			idReserved = reserved;
			idType = type;
			idCount = count;
		}

		public ICONDIR(Stream stream)
		{
			this = default(ICONDIR);
			Read(stream);
		}

		public unsafe void Read(Stream stream)
		{
			byte[] array = new byte[sizeof(ICONDIR)];
			stream.Read(array, 0, sizeof(ICONDIR));
			fixed (byte* ptr = array)
			{
				this = *(ICONDIR*)ptr;
			}
		}

		public unsafe void Write(Stream stream)
		{
			byte[] array = new byte[sizeof(ICONDIR)];
			fixed (ICONDIR* ptr = &this)
			{
				Marshal.Copy((IntPtr)ptr, array, 0, sizeof(ICONDIR));
			}
			stream.Write(array, 0, sizeof(ICONDIR));
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct BITMAPINFO
	{
		public BITMAPINFOHEADER icHeader;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public RGBQUAD[] icColors;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct BITMAPINFOHEADER
	{
		public uint biSize;

		public uint biWidth;

		public uint biHeight;

		public ushort biPlanes;

		public ushort biBitCount;

		public IconImageFormat biCompression;

		public uint biSizeImage;

		public int biXPelsPerMeter;

		public int biYPelsPerMeter;

		public uint biClrUsed;

		public uint biClrImportant;

		public BITMAPINFOHEADER(Stream stream)
		{
			this = default(BITMAPINFOHEADER);
			Read(stream);
		}

		public unsafe void Read(Stream stream)
		{
			byte[] array = new byte[sizeof(BITMAPINFOHEADER)];
			stream.Read(array, 0, array.Length);
			fixed (byte* ptr = array)
			{
				this = *(BITMAPINFOHEADER*)ptr;
			}
		}

		public unsafe void Write(Stream stream)
		{
			byte[] array = new byte[sizeof(BITMAPINFOHEADER)];
			fixed (BITMAPINFOHEADER* ptr = &this)
			{
				Marshal.Copy((IntPtr)ptr, array, 0, sizeof(BITMAPINFOHEADER));
			}
			stream.Write(array, 0, sizeof(BITMAPINFOHEADER));
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct RGBQUAD
	{
		public byte rgbBlue;

		public byte rgbGreen;

		public byte rgbRed;

		public byte rgbReserved;

		public void Set(byte r, byte g, byte b)
		{
			rgbRed = r;
			rgbGreen = g;
			rgbBlue = b;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct ICONINFO
	{
		public bool fIcon;

		public uint xHotspot;

		public uint yHotspot;

		public IntPtr hbmMask;

		public IntPtr hbmColor;
	}

	internal struct ACTCTX
	{
		public int cbSize;

		public uint dwFlags;

		public string lpSource;

		public ushort wProcessorArchitecture;

		public ushort wLangId;

		public string lpAssemblyDirectory;

		public string lpResourceName;

		public string lpApplicationName;
	}

	internal struct SECURITY_DESCRIPTOR
	{
		public byte revision;

		public byte size;

		public short control;

		public IntPtr owner;

		public IntPtr group;

		public IntPtr sacl;

		public IntPtr dacl;
	}

	internal struct SHARE_INFO_503
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string netName;

		public ShareInfoType type;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string remark;

		public SharePermissions permissions;

		public int maxUses;

		public int currentUses;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string path;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string password;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string serverName;

		public int reserved;

		public IntPtr securityDescriptor;
	}

	internal struct SHARE_INFO_502
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string netName;

		public ShareInfoType type;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string remark;

		public SharePermissions permissions;

		public int maxUses;

		public int currentUses;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string path;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string password;

		public int reserved;

		public IntPtr securityDescriptor;
	}

	internal class UnmanagedBuffer : IDisposable
	{
		public IntPtr IntPtr { get; private set; }

		public bool IsMemoryValid => IntPtr != IntPtr.Zero;

		public int Size { get; private set; }

		private bool IsUnderlyingObjectString { get; set; }

		public UnmanagedBuffer(int size)
		{
			try
			{
				Size = size;
				IntPtr = Alloc(size);
				IsUnderlyingObjectString = false;
			}
			catch (OutOfMemoryException)
			{
				IntPtr = IntPtr.Zero;
			}
		}

		private UnmanagedBuffer(string s)
		{
			try
			{
				IntPtr = Marshal.StringToHGlobalAuto(s);
				IsUnderlyingObjectString = true;
				Size = (s.Length + 1) * 2;
			}
			catch (OutOfMemoryException)
			{
				IntPtr = IntPtr.Zero;
			}
		}

		public static UnmanagedBuffer Create<TObject>(TObject obj) where TObject : struct
		{
			Exceptions.ThrowIfNull(obj, "obj");
			UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer(Marshal.SizeOf(typeof(TObject)));
			Marshal.StructureToPtr(obj, unmanagedBuffer.IntPtr, fDeleteOld: false);
			return unmanagedBuffer;
		}

		public static UnmanagedBuffer Create(int i)
		{
			UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer(4);
			Marshal.WriteInt32(unmanagedBuffer.IntPtr, i);
			return unmanagedBuffer;
		}

		public static UnmanagedBuffer Create(string s)
		{
			Exceptions.ThrowIfNull(s, "s");
			return new UnmanagedBuffer(s);
		}

		~UnmanagedBuffer()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (IntPtr != IntPtr.Zero)
			{
				if (IsUnderlyingObjectString)
				{
					Marshal.FreeHGlobal(IntPtr);
				}
				else
				{
					Free(IntPtr);
				}
				IntPtr = IntPtr.Zero;
			}
		}
	}

	internal struct SERVICE_FAILURE_ACTIONS
	{
		public int resetPeriod;

		public string rebootMsg;

		public string command;

		public int numActions;

		public IntPtr actions;
	}

	internal struct SC_ACTION
	{
		public SC_ACTION_TYPE type;

		public uint delay;
	}

	internal enum SC_ACTION_TYPE
	{
		SC_ACTION_NONE,
		SC_ACTION_RESTART,
		SC_ACTION_REBOOT,
		SC_ACTION_RUN_COMMAND
	}

	internal struct QUERY_SERVICE_CONFIG
	{
		public int serviceType;

		public int startType;

		public int errorControl;

		public string binaryPathName;

		public string loadOrderGroup;

		public int tagId;

		public string dependencies;

		public string serviceStartName;

		public string displayName;
	}

	internal struct SERVICE_DESCRIPTION
	{
		public string description;
	}

	[Flags]
	public enum TaskRunFlags : uint
	{
		TaskRunNoFlags = 0u,
		TaskRunAsSelf = 1u,
		TaskRunIgnoreConstraints = 2u,
		TaskRunUseSessionId = 4u,
		TaskRunUserSid = 8u
	}

	[Flags]
	public enum TaskEnumFlags
	{
		TaskEnumNoFlags = 0,
		TaskEnumHidden = 1
	}

	public enum TaskLogonType : uint
	{
		TaskLogonNone,
		TaskLogonPassword,
		TaskLogonS4u,
		TaskLogonInteractiveToken,
		TaskLogonGroup,
		TaskLogonServiceAccount,
		TaskLogonInteractiveTokenOrPassword
	}

	public enum TaskRunlevelType
	{
		TaskRunlevelLua,
		TaskRunlevelHighest
	}

	[Flags]
	public enum TaskState
	{
		TaskStateUnknown = 0,
		TaskStateDisabled = 1,
		TaskStateQueued = 2,
		TaskStateReady = 3,
		TaskStateRunning = 4
	}

	[Flags]
	public enum TaskCreation : uint
	{
		None = 0u,
		TaskValidateOnly = 1u,
		TaskCreate = 2u,
		TaskUpdate = 4u,
		TaskCreateOrUpdate = 6u,
		TaskDisable = 8u,
		TaskDontAddPrincipleAce = 0x10u,
		TaskIgnoreRegistrationTriggers = 0x20u
	}

	public enum TaskTriggerType
	{
		TaskTriggerEvent = 0,
		TaskTriggerTime = 1,
		TaskTriggerDaily = 2,
		TaskTriggerWeekly = 3,
		TaskTriggerMonthly = 4,
		TaskTriggerMonthlyDow = 5,
		TaskTriggerIdle = 6,
		TaskTriggerRegistration = 7,
		TaskTriggerBoot = 8,
		TaskTriggerLogon = 9,
		TaskTriggerSessionStateChange = 11
	}

	public enum TaskSessionStateChangeType
	{
		TaskConsoleConnect = 1,
		TaskConsoleDisconnect = 2,
		TaskRemoteConnect = 3,
		TaskRemoteDisconnect = 4,
		TaskSessionLock = 7,
		TaskSessionUnlock = 8
	}

	public enum TaskActionType
	{
		TaskActionExec = 0,
		TaskActionComHandler = 5,
		TaskActionSendEmail = 6,
		TaskActionShowMessage = 7
	}

	public enum TaskInstancesPolicy
	{
		TaskInstancesParallel,
		TaskInstancesQueue,
		TaskInstancesIgnoreNew,
		TaskInstancesStopExisting
	}

	public enum TaskCompatibility
	{
		TaskCompatibilityAt,
		TaskCompatibilityV1,
		TaskCompatibilityV2,
		TaskCompatibilityV3
	}

	public struct SystemTime
	{
		public ushort wYear;

		public ushort wMonth;

		public ushort wDayOfWeek;

		public ushort wDay;

		public ushort wHour;

		public ushort wMinute;

		public ushort wSecond;

		public ushort wMilliseconds;
	}

	public static class TaskSchedulerProgIDs
	{
		public const string TaskService = "Schedule.Service";
	}

	public static class TaskSchedulerServiceGuids
	{
		public const string TaskSchedulerGuid = "0f87369f-a4e5-4cfc-bd3e-73e6154572dd";

		public const string TaskServiceGuid = "2faba4c7-4da9-4013-9697-20cc3fd40f85";

		public const string TaskFolderGuid = "8cfac062-a080-4c15-9a88-aa7c2af80dfc";

		public const string TaskDefinitionGuid = "f5bc8fc5-536d-4f77-b852-fbc1356fdeb6";

		public const string RegisteredTaskCollectionGuid = "86627eb4-42a7-41e4-a4d9-ac33a72f2d52";

		public const string RegisteredTaskGuid = "9c86f320-dee3-4dd1-b972-a303f26b061e";

		public const string RunningTaskCollectionGuid = "6a67614b-6828-4fec-aa54-6d52e8f1f2db";

		public const string RunningTaskGuid = "653758fb-7b9a-4f1e-a471-beeb8e9b834e";

		public const string TaskFolderCollectionGuid = "79184a66-8664-423f-97f1-637356a5d812";
	}

	[Guid("653758fb-7b9a-4f1e-a471-beeb8e9b834e")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IRunningTask
	{
		[DispId(1)]
		string Name { get; }

		[DispId(0)]
		string InstanceGuid { get; }

		[DispId(2)]
		string Path { get; }

		[DispId(3)]
		TaskState State { get; }

		[DispId(4)]
		string CurrentAction { get; }

		[DispId(7)]
		int EnginePID { get; }

		[DispId(5)]
		void Stop();

		[DispId(6)]
		void Refresh();
	}

	[Guid("6a67614b-6828-4fec-aa54-6d52e8f1f2db")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IRunningTaskCollection : IEnumerable
	{
		[DispId(1)]
		int Count { get; }

		[DispId(0)]
		IRunningTask GetItem(object index);

		[DispId(-4)]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		new IEnumerator GetEnumerator();
	}

	[Guid("9c86f320-dee3-4dd1-b972-a303f26b061e")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IRegisteredTask
	{
		[DispId(1)]
		string Name { get; }

		[DispId(0)]
		string Path { get; }

		[DispId(2)]
		TaskState State { get; }

		[DispId(3)]
		bool Enabled { get; set; }

		[DispId(8)]
		DateTime LastRunTime { get; }

		[DispId(9)]
		uint LastTaskResult { get; }

		[DispId(11)]
		int NumberOfMissedRuns { get; }

		[DispId(12)]
		DateTime NextRunTime { get; }

		[DispId(13)]
		ITaskDefinition Definition { get; }

		[DispId(14)]
		string XML { get; }

		[DispId(5)]
		IRunningTask Run([In][MarshalAs(UnmanagedType.Struct)] object parameters);

		[DispId(6)]
		IRunningTask RunEx([In][MarshalAs(UnmanagedType.Struct)] object parameters, TaskRunFlags flags, int sessionId, string user);

		[DispId(7)]
		IRunningTaskCollection GetInstances(TaskEnumFlags flags);

		[DispId(15)]
		string GetSecurityDescriptor(int securityInformation);

		[DispId(16)]
		void SetSecurityDescriptor(string sddl, int flags);

		[DispId(17)]
		void Stop(int flags);

		void GetRunTimes(SystemTime begin, SystemTime end, out int count, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out SystemTime[] times);
	}

	[Guid("86627eb4-42a7-41e4-a4d9-ac33a72f2d52")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IRegisteredTaskCollection : IEnumerable
	{
		[DispId(1)]
		int Count { get; }

		[DispId(0)]
		IRegisteredTask GetItem(object index);

		[DispId(-4)]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		new IEnumerator GetEnumerator();
	}

	[Guid("8cfac062-a080-4c15-9a88-aa7c2af80dfc")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITaskFolder
	{
		[DispId(1)]
		string Name { get; }

		[DispId(0)]
		string Path { get; }

		[DispId(3)]
		ITaskFolder GetFolder(string path);

		[DispId(4)]
		ITaskFolderCollection GetFolders(TaskEnumFlags flags);

		[DispId(5)]
		ITaskFolder CreateFolder(string folderName, [In][MarshalAs(UnmanagedType.Struct)] object sddl);

		[DispId(6)]
		void DeleteFolder(string folderName, int flags);

		[DispId(7)]
		IRegisteredTask GetTask(string path);

		[DispId(8)]
		IRegisteredTaskCollection GetTasks(TaskEnumFlags flags);

		[DispId(9)]
		void DeleteTask(string name, int flags);

		[PreserveSig]
		[DispId(10)]
		uint RegisterTask([In][MarshalAs(UnmanagedType.BStr)] string name, [In][MarshalAs(UnmanagedType.BStr)] string xmlText, TaskCreation flags, [In][MarshalAs(UnmanagedType.Struct)] object userId, [In] object password, TaskLogonType logonType, [In][MarshalAs(UnmanagedType.Struct)] object sddl, out IRegisteredTask registeredTask);

		[PreserveSig]
		[DispId(11)]
		uint RegisterTaskDefinition(string name, ITaskDefinition pDefinition, TaskCreation flags, object userId, object password, TaskLogonType logonType, object sddl, out IRegisteredTask pRegisteredTask);

		[DispId(12)]
		string GetSecurityDescriptor(int securityInformation);

		[DispId(13)]
		void SetSecurityDescriptor(string sddl, int flags);
	}

	[Guid("79184a66-8664-423f-97f1-637356a5d812")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITaskFolderCollection : IEnumerable
	{
		[DispId(1)]
		int Count { get; }

		[DispId(0)]
		ITaskFolder GetItem(object index);

		[DispId(-4)]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		new IEnumerator GetEnumerator();
	}

	[Guid("2faba4c7-4da9-4013-9697-20cc3fd40f85")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITaskService
	{
		[DispId(5)]
		bool Connected { get; }

		[DispId(0)]
		string TargetServer { get; }

		[DispId(6)]
		string ConnectedUser { get; }

		[DispId(7)]
		string ConnectedDomain { get; }

		[DispId(8)]
		int HighestVersion { get; }

		[DispId(1)]
		ITaskFolder GetFolder([In][MarshalAs(UnmanagedType.BStr)] string path);

		[DispId(2)]
		IRunningTaskCollection GetRunningTasks(TaskEnumFlags flags);

		[DispId(3)]
		ITaskDefinition NewTask(int flags);

		[DispId(4)]
		uint Connect([In][MarshalAs(UnmanagedType.Struct)] object serverName, [In][MarshalAs(UnmanagedType.Struct)] object user, [In][MarshalAs(UnmanagedType.Struct)] object domain, [In] object password);
	}

	[Guid("839d7762-5121-4009-9234-4f0d19394f04")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITaskHandler
	{
		void Start([In][MarshalAs(UnmanagedType.IUnknown)] object handlerServices, string data);

		uint Stop();

		void Pause();

		void Resume();
	}

	[Guid("eaec7a8f-27a0-4ddc-8675-14726a01a38a")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITaskHandlerStatus
	{
		void UpdateStatus(short percentComplete, string statusMessage);

		void TaskCompleted([In][MarshalAs(UnmanagedType.Error)] int taskErrorCode);
	}

	[Guid("3e4c9351-d966-4b8b-bb87-ceba68bb0107")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITaskVariables
	{
		string GetInput();

		void SetOutput(string input);

		string GetContext();
	}

	[Guid("84594461-0053-4342-A8FD-088FABF11F32")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IIdleSettings
	{
		[DispId(1)]
		string IdleDuration { get; set; }

		[DispId(2)]
		string WaitTimeout { get; set; }

		[DispId(3)]
		bool StopOnIdleEnd { get; set; }

		[DispId(4)]
		bool RestartOnIdle { get; set; }
	}

	[Guid("9F7DEA84-C30B-4245-80B6-00E9F646F1B4")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface INetworkSettings
	{
		[DispId(1)]
		string Name { get; set; }

		[DispId(2)]
		string Id { get; set; }
	}

	[Guid("7FB9ACF1-26BE-400e-85B5-294B9C75DFD6")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IRepetitionPattern
	{
		[DispId(1)]
		string Interval { get; set; }

		[DispId(2)]
		string Duration { get; set; }

		[DispId(3)]
		bool StopAtDurationEnd { get; set; }
	}

	[Guid("39038068-2B46-4afd-8662-7BB6F868D221")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITaskNamedValuePair
	{
		[DispId(0)]
		string Name { get; set; }

		[DispId(1)]
		string Value { get; set; }
	}

	[Guid("B4EF826B-63C3-46e4-A504-EF69E4F7EA4D")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITaskNamedValueCollection : IEnumerable
	{
		[DispId(1)]
		int Count { get; }

		[DispId(0)]
		[return: MarshalAs(UnmanagedType.Interface)]
		ITaskNamedValuePair GetItem([In][MarshalAs(UnmanagedType.I4)] int index);

		[DispId(-4)]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		new IEnumerator GetEnumerator();

		[DispId(0)]
		[return: MarshalAs(UnmanagedType.Interface)]
		ITaskNamedValuePair Create(string name, string value);

		[DispId(4)]
		void Remove([In][MarshalAs(UnmanagedType.I4)] int index);

		[DispId(5)]
		void Clear();
	}

	[Guid("D98D51E5-C9B4-496a-A9C1-18980261CF0F")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IPrincipal
	{
		[DispId(1)]
		string Id { get; set; }

		[DispId(2)]
		string DisplayName { get; set; }

		[DispId(3)]
		string UserId { get; set; }

		[DispId(4)]
		TaskLogonType LogonType { get; set; }

		[DispId(5)]
		string GroupId { get; set; }

		[DispId(6)]
		TaskRunlevelType RunLevel { get; set; }
	}

	[Guid("09941815-ea89-4b5b-89e0-2a773801fac3")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITrigger
	{
		[DispId(1)]
		TaskTriggerType Type { get; }

		[DispId(2)]
		string Id { get; set; }

		[DispId(3)]
		IRepetitionPattern Repetition { get; set; }

		[DispId(4)]
		string ExecutionTimeLimit { get; set; }

		[DispId(5)]
		string StartBoundary { get; set; }

		[DispId(6)]
		string EndBoundary { get; set; }

		[DispId(7)]
		bool Enabled { get; set; }
	}

	[Guid("d45b0167-9653-4eef-b94f-0732ca7af251")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IEventTrigger : ITrigger
	{
		[DispId(1)]
		new TaskTriggerType Type { get; }

		[DispId(2)]
		new string Id { get; set; }

		[DispId(3)]
		new IRepetitionPattern Repetition { get; set; }

		[DispId(4)]
		new string ExecutionTimeLimit { get; set; }

		[DispId(5)]
		new string StartBoundary { get; set; }

		[DispId(6)]
		new string EndBoundary { get; set; }

		[DispId(7)]
		new bool Enabled { get; set; }

		[DispId(20)]
		string Subscription { get; set; }

		[DispId(21)]
		string Delay { get; set; }

		[DispId(22)]
		ITaskNamedValueCollection ValueQueries { get; set; }
	}

	[Guid("b45747e0-eba7-4276-9f29-85c5bb300006")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITimeTrigger : ITrigger
	{
		[DispId(1)]
		new TaskTriggerType Type { get; }

		[DispId(2)]
		new string Id { get; set; }

		[DispId(3)]
		new IRepetitionPattern Repetition { get; set; }

		[DispId(4)]
		new string ExecutionTimeLimit { get; set; }

		[DispId(5)]
		new string StartBoundary { get; set; }

		[DispId(6)]
		new string EndBoundary { get; set; }

		[DispId(7)]
		new bool Enabled { get; set; }

		[DispId(20)]
		string RandomDelay { get; set; }
	}

	[Guid("126c5cd8-b288-41d5-8dbf-e491446adc5c")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IDailyTrigger : ITrigger
	{
		[DispId(1)]
		new TaskTriggerType Type { get; }

		[DispId(2)]
		new string Id { get; set; }

		[DispId(3)]
		new IRepetitionPattern Repetition { get; set; }

		[DispId(4)]
		new string ExecutionTimeLimit { get; set; }

		[DispId(5)]
		new string StartBoundary { get; set; }

		[DispId(6)]
		new string EndBoundary { get; set; }

		[DispId(7)]
		new bool Enabled { get; set; }

		[DispId(25)]
		short DaysInterval { get; set; }

		[DispId(20)]
		string RandomDelay { get; set; }
	}

	[Guid("5038fc98-82ff-436d-8728-a512a57c9dc1")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IWeeklyTrigger : ITrigger
	{
		[DispId(1)]
		new TaskTriggerType Type { get; }

		[DispId(2)]
		new string Id { get; set; }

		[DispId(3)]
		new IRepetitionPattern Repetition { get; set; }

		[DispId(4)]
		new string ExecutionTimeLimit { get; set; }

		[DispId(5)]
		new string StartBoundary { get; set; }

		[DispId(6)]
		new string EndBoundary { get; set; }

		[DispId(7)]
		new bool Enabled { get; set; }

		[DispId(25)]
		short DaysOfWeek { get; set; }

		[DispId(26)]
		short WeeksInterval { get; set; }

		[DispId(20)]
		string RandomDelay { get; set; }
	}

	[Guid("97c45ef1-6b02-4a1a-9c0e-1ebfba1500ac")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IMonthlyTrigger : ITrigger
	{
		[DispId(1)]
		new TaskTriggerType Type { get; }

		[DispId(2)]
		new string Id { get; set; }

		[DispId(3)]
		new IRepetitionPattern Repetition { get; set; }

		[DispId(4)]
		new string ExecutionTimeLimit { get; set; }

		[DispId(5)]
		new string StartBoundary { get; set; }

		[DispId(6)]
		new string EndBoundary { get; set; }

		[DispId(7)]
		new bool Enabled { get; set; }

		[DispId(25)]
		int DaysOfMonth { get; set; }

		[DispId(26)]
		short MonthsOfYear { get; set; }

		[DispId(27)]
		bool RunOnLastDayOfMonth { get; set; }

		[DispId(20)]
		string RandomDelay { get; set; }
	}

	[Guid("77d025a3-90fa-43aa-b52e-cda5499b946a")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IMonthlyDOWTrigger : ITrigger
	{
		[DispId(1)]
		new TaskTriggerType Type { get; }

		[DispId(2)]
		new string Id { get; set; }

		[DispId(3)]
		new IRepetitionPattern Repetition { get; set; }

		[DispId(4)]
		new string ExecutionTimeLimit { get; set; }

		[DispId(5)]
		new string StartBoundary { get; set; }

		[DispId(6)]
		new string EndBoundary { get; set; }

		[DispId(7)]
		new bool Enabled { get; set; }

		[DispId(25)]
		short DaysOfWeek { get; set; }

		[DispId(26)]
		short WeeksOfMonth { get; set; }

		[DispId(27)]
		short MonthsOfYear { get; set; }

		[DispId(28)]
		bool RunOnLastWeekOfMonth { get; set; }

		[DispId(20)]
		string RandomDelay { get; set; }
	}

	[Guid("d537d2b0-9fb3-4d34-9739-1ff5ce7b1ef3")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IIdleTrigger : ITrigger
	{
		[DispId(1)]
		new TaskTriggerType Type { get; }

		[DispId(2)]
		new string Id { get; set; }

		[DispId(3)]
		new IRepetitionPattern Repetition { get; set; }

		[DispId(4)]
		new string ExecutionTimeLimit { get; set; }

		[DispId(5)]
		new string StartBoundary { get; set; }

		[DispId(6)]
		new string EndBoundary { get; set; }

		[DispId(7)]
		new bool Enabled { get; set; }
	}

	[Guid("72DADE38-FAE4-4b3e-BAF4-5D009AF02B1C")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ILogonTrigger : ITrigger
	{
		[DispId(1)]
		new TaskTriggerType Type { get; }

		[DispId(2)]
		new string Id { get; set; }

		[DispId(3)]
		new IRepetitionPattern Repetition { get; set; }

		[DispId(4)]
		new string ExecutionTimeLimit { get; set; }

		[DispId(5)]
		new string StartBoundary { get; set; }

		[DispId(6)]
		new string EndBoundary { get; set; }

		[DispId(7)]
		new bool Enabled { get; set; }

		[DispId(20)]
		string Delay { get; set; }

		[DispId(21)]
		string UserId { get; set; }
	}

	[Guid("754DA71B-4385-4475-9DD9-598294FA3641")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ISessionStateChangeTrigger : ITrigger
	{
		[DispId(1)]
		new TaskTriggerType Type { get; }

		[DispId(2)]
		new string Id { get; set; }

		[DispId(3)]
		new IRepetitionPattern Repetition { get; set; }

		[DispId(4)]
		new string ExecutionTimeLimit { get; set; }

		[DispId(5)]
		new string StartBoundary { get; set; }

		[DispId(6)]
		new string EndBoundary { get; set; }

		[DispId(7)]
		new bool Enabled { get; set; }

		[DispId(20)]
		string Delay { get; set; }

		[DispId(21)]
		string UserId { get; set; }

		[DispId(22)]
		TaskSessionStateChangeType StateChange { get; set; }
	}

	[Guid("2A9C35DA-D357-41f4-BBC1-207AC1B1F3CB")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IBootTrigger : ITrigger
	{
		[DispId(1)]
		new TaskTriggerType Type { get; }

		[DispId(2)]
		new string Id { get; set; }

		[DispId(3)]
		new IRepetitionPattern Repetition { get; set; }

		[DispId(4)]
		new string ExecutionTimeLimit { get; set; }

		[DispId(5)]
		new string StartBoundary { get; set; }

		[DispId(6)]
		new string EndBoundary { get; set; }

		[DispId(7)]
		new bool Enabled { get; set; }

		[DispId(20)]
		string Delay { get; set; }
	}

	[Guid("4c8fec3a-c218-4e0c-b23d-629024db91a2")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IRegistrationTrigger : ITrigger
	{
		[DispId(1)]
		new TaskTriggerType Type { get; }

		[DispId(2)]
		new string Id { get; set; }

		[DispId(3)]
		new IRepetitionPattern Repetition { get; set; }

		[DispId(4)]
		new string ExecutionTimeLimit { get; set; }

		[DispId(5)]
		new string StartBoundary { get; set; }

		[DispId(6)]
		new string EndBoundary { get; set; }

		[DispId(7)]
		new bool Enabled { get; set; }

		[DispId(20)]
		string Delay { get; set; }
	}

	[Guid("85df5081-1b24-4f32-878a-d9d14df4cb77")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITriggerCollection : IEnumerable
	{
		[DispId(1)]
		int Count { get; }

		[DispId(0)]
		[return: MarshalAs(UnmanagedType.Interface)]
		ITrigger GetItem([In][MarshalAs(UnmanagedType.I4)] int index);

		[DispId(-4)]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		new IEnumerator GetEnumerator();

		[DispId(2)]
		[return: MarshalAs(UnmanagedType.Interface)]
		ITrigger Create(TaskTriggerType type);

		[DispId(4)]
		void Remove([In][MarshalAs(UnmanagedType.Struct)] object index);

		[DispId(5)]
		void Clear();
	}

	[Guid("BAE54997-48B1-4cbe-9965-D6BE263EBEA4")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IAction
	{
		[DispId(1)]
		string Id { get; set; }

		[DispId(2)]
		TaskActionType Type { get; }
	}

	[Guid("4c3d624d-fd6b-49a3-b9b7-09cb3cd3f047")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IExecAction : IAction
	{
		[DispId(1)]
		new string Id { get; set; }

		[DispId(2)]
		new TaskActionType Type { get; }

		[DispId(10)]
		string Path { get; set; }

		[DispId(11)]
		string Arguments { get; set; }

		[DispId(12)]
		string WorkingDirectory { get; set; }
	}

	[Guid("505E9E68-AF89-46b8-A30F-56162A83D537")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IShowMessageAction : IAction
	{
		[DispId(1)]
		new string Id { get; set; }

		[DispId(2)]
		new TaskActionType Type { get; }

		[DispId(10)]
		string Title { get; set; }

		[DispId(11)]
		string MessageBody { get; set; }
	}

	[Guid("6D2FD252-75C5-4f66-90BA-2A7D8CC3039F")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IComHandlerAction : IAction
	{
		[DispId(1)]
		new string Id { get; set; }

		[DispId(2)]
		new TaskActionType Type { get; }

		[DispId(10)]
		string ClassId { get; set; }

		[DispId(11)]
		string Data { get; set; }
	}

	[Guid("02820E19-7B98-4ed2-B2E8-FDCCCEFF619B")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IActionCollection : IEnumerable
	{
		[DispId(1)]
		int Count { get; }

		[DispId(2)]
		string XmlText { get; set; }

		[DispId(6)]
		string Context { get; set; }

		[DispId(0)]
		[return: MarshalAs(UnmanagedType.Interface)]
		IAction GetItem([In][MarshalAs(UnmanagedType.I4)] int index);

		[DispId(-4)]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		new IEnumerator GetEnumerator();

		[DispId(3)]
		[return: MarshalAs(UnmanagedType.Interface)]
		IAction Create([MarshalAs(UnmanagedType.I4)] TaskActionType type);

		[DispId(4)]
		void Remove([In][MarshalAs(UnmanagedType.Struct)] object index);

		[DispId(5)]
		void Clear();
	}

	[Guid("416D8B73-CB41-4ea1-805C-9BE9A5AC4A74")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IRegistrationInfo
	{
		[DispId(1)]
		string Description { get; set; }

		[DispId(2)]
		string Author { get; set; }

		[DispId(4)]
		string Version { get; set; }

		[DispId(5)]
		string Date { get; set; }

		[DispId(6)]
		string Documentation { get; set; }

		[DispId(9)]
		string XmlText { get; set; }

		[DispId(10)]
		string URI { get; set; }

		[DispId(11)]
		object SecurityDescriptor { get; set; }

		[DispId(12)]
		string Source { get; set; }
	}

	[Guid("8FD4711D-2D02-4c8c-87E3-EFF699DE127E")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITaskSettings
	{
		[DispId(3)]
		bool AllowDemandStart { get; set; }

		[DispId(4)]
		string RestartInterval { get; set; }

		[DispId(5)]
		int RestartCount { get; set; }

		[DispId(6)]
		TaskInstancesPolicy MultipleInstances { get; set; }

		[DispId(7)]
		bool StopIfGoingOnBatteries { get; set; }

		[DispId(8)]
		bool DisallowStartIfOnBatteries { get; set; }

		[DispId(9)]
		bool AllowHardTerminate { get; set; }

		[DispId(10)]
		bool StartWhenAvailable { get; set; }

		[DispId(11)]
		string XmlText { get; set; }

		[DispId(12)]
		bool RunOnlyIfNetworkAvailable { get; set; }

		[DispId(13)]
		string ExecutionTimeLimit { get; set; }

		[DispId(14)]
		bool Enabled { get; set; }

		[DispId(15)]
		string DeleteExpiredTaskAfter { get; set; }

		[DispId(16)]
		int Priority { get; set; }

		[DispId(17)]
		TaskCompatibility Compatibility { get; set; }

		[DispId(18)]
		bool Hidden { get; set; }

		[DispId(19)]
		IIdleSettings IdleSettings { get; set; }

		[DispId(20)]
		bool RunOnlyIfIdle { get; set; }

		[DispId(21)]
		bool WakeToRun { get; set; }

		[DispId(22)]
		INetworkSettings NetworkSettings { get; set; }
	}

	[Guid("f5bc8fc5-536d-4f77-b852-fbc1356fdeb6")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITaskDefinition
	{
		[DispId(1)]
		IRegistrationInfo RegistrationInfo { get; set; }

		[DispId(2)]
		ITriggerCollection Triggers { get; set; }

		[DispId(7)]
		ITaskSettings Settings { get; set; }

		[DispId(11)]
		string Data { get; set; }

		[DispId(12)]
		IPrincipal Principal { get; set; }

		[DispId(13)]
		IActionCollection Actions { get; set; }

		[DispId(14)]
		string XmlText { get; set; }
	}

	internal enum APPLICATION_STATE
	{
		ApplicationStateHealthy,
		ApplicationStateCritical
	}

	[Guid("267a0284-848f-447e-a096-5e10a1a76bca")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	internal interface IVmApplicationHealthMonitor
	{
		[DispId(1)]
		uint SetApplicationState([In] string id, [In] string name, [In] APPLICATION_STATE state);

		[DispId(2)]
		uint ResetAllApplicationState();
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 2608)]
	internal struct CLUS_POOL_DRIVE_INFO
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string DriveName;

		[MarshalAs(UnmanagedType.Bool)]
		public bool IncursSeekPenalty;

		[MarshalAs(UnmanagedType.U4)]
		public uint DriveHealth;

		[MarshalAs(UnmanagedType.U4)]
		public uint DriveState;

		[MarshalAs(UnmanagedType.U8)]
		public ulong TotalCapacity;

		[MarshalAs(UnmanagedType.U8)]
		public ulong ConsumeCapacity;

		[MarshalAs(UnmanagedType.U4)]
		public uint Usage;

		[MarshalAs(UnmanagedType.U4)]
		public uint BusType;

		[MarshalAs(UnmanagedType.U4)]
		public int Slot;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string EnclosureName;
	}

	public static uint FACILITY_RPC = 1u;

	public static uint FACILITY_WIN32 = 7u;

	public static uint NT9_MAJOR_VERSION = 8u;

	public static uint NT10_MAJOR_VERSION = 9u;

	public static int NERR_BASE = 2100;

	public static int MAXIMUM_ALLOWED = 33554432;

	public static int NoFlags = 0;

	public const int MaxPath = 260;

	public const int CLUS_HYBRID_QUORUM = 1024;

	public const int CLUS_NODE_MAJORITY_QUORUM = 0;

	public const int CLUS_LEGACY_QUORUM = 4194304;

	public static string CLUSREG_NAME_CLUS_CLUSTER_GROUP = "ClusterGroup";

	public static string CLUSREG_NAME_CLUS_AVAIL_STORAGE_GROUP = "AvailableStorage";

	public static int CLUSTER_CREATE_GROUP_INFO_VERSION_1 = 1;

	public static int CLUSTER_CREATE_GROUP_INFO_VERSION = CLUSTER_CREATE_GROUP_INFO_VERSION_1;

	public static int CLUSCTL_ACCESS_SHIFT;

	public static int CLUSCTL_FUNCTION_SHIFT = 2;

	public static int KDDSL_INTERNAL_SHIFT = 20;

	public static int KDDSL_USER_SHIFT = 21;

	public static int KDDSL_MODIFY_SHIFT = 22;

	public static int KDDSL_GLOBAL_SHIFT = 23;

	public static int CLUSCTL_OBJECT_SHIFT = 24;

	public static int CLUS_ACCESS_ANY;

	public static int CLUS_ACCESS_READ = 1;

	public static int CLUS_ACCESS_WRITE = 2;

	public static int CLUS_NO_MODIFY;

	public static int CLUS_MODIFY = 1;

	public static int KDDSL_INTERNAL_MASK = 1 << KDDSL_INTERNAL_SHIFT;

	public static int KDDSL_USER_MASK = 1 << KDDSL_USER_SHIFT;

	public static int KDDSL_MODIFY_MASK = 1 << KDDSL_MODIFY_SHIFT;

	public static int KDDSL_GLOBAL_MASK = 1 << KDDSL_GLOBAL_SHIFT;

	public static int CLUSCTL_CONTROL_CODE_MASK = 4194303;

	public static int CLUSCTL_OBJECT_MASK = 255;

	public static int CLUSCTL_ACCESS_MODE_MASK = 3;

	public static int KDDSL_CLUSTER_BASE;

	public static int KDDSL_USER_BASE = 1 << KDDSL_USER_SHIFT;

	public static int KDDSL_UNKNOWN = KDDSL_EXTERNAL_CODE(0, CLUS_ACCESS_ANY, CLUS_NO_MODIFY);

	public static int KDDSL_GET_CHARACTERISTICS = KDDSL_EXTERNAL_CODE(1, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_FLAGS = KDDSL_EXTERNAL_CODE(2, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_CLASS_INFO = KDDSL_EXTERNAL_CODE(3, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_REQUIRED_DEPENDENCIES = KDDSL_EXTERNAL_CODE(4, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_ARB_TIMEOUT = KDDSL_EXTERNAL_CODE(5, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_NAME = KDDSL_EXTERNAL_CODE(10, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_RESOURCE_TYPE = KDDSL_EXTERNAL_CODE(11, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_NODE = KDDSL_EXTERNAL_CODE(12, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_NETWORK = KDDSL_EXTERNAL_CODE(13, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_ID = KDDSL_EXTERNAL_CODE(14, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_FQDN = KDDSL_EXTERNAL_CODE(15, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_CLUSTER_SERVICE_ACCOUNT_NAME = KDDSL_EXTERNAL_CODE(16, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_CHECK_VOTER_EVICT = KDDSL_EXTERNAL_CODE(17, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_CHECK_VOTER_DOWN = KDDSL_EXTERNAL_CODE(18, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_SHUTDOWN = KDDSL_EXTERNAL_CODE(19, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_ENUM_COMMON_PROPERTIES = KDDSL_EXTERNAL_CODE(20, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_RO_COMMON_PROPERTIES = KDDSL_EXTERNAL_CODE(21, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_COMMON_PROPERTIES = KDDSL_EXTERNAL_CODE(22, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_SET_COMMON_PROPERTIES = KDDSL_EXTERNAL_CODE(23, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_VALIDATE_COMMON_PROPERTIES = KDDSL_EXTERNAL_CODE(24, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_COMMON_PROPERTY_FMTS = KDDSL_EXTERNAL_CODE(25, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_COMMON_RESOURCE_PROPERTY_FMTS = KDDSL_EXTERNAL_CODE(26, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_ENUM_PRIVATE_PROPERTIES = KDDSL_EXTERNAL_CODE(30, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_RO_PRIVATE_PROPERTIES = KDDSL_EXTERNAL_CODE(31, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_PRIVATE_PROPERTIES = KDDSL_EXTERNAL_CODE(32, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_SET_PRIVATE_PROPERTIES = KDDSL_EXTERNAL_CODE(33, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_VALIDATE_PRIVATE_PROPERTIES = KDDSL_EXTERNAL_CODE(34, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_PRIVATE_PROPERTY_FMTS = KDDSL_EXTERNAL_CODE(35, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_PRIVATE_RESOURCE_PROPERTY_FMTS = KDDSL_EXTERNAL_CODE(36, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_ADD_REGISTRY_CHECKPOINT = KDDSL_EXTERNAL_CODE(40, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_DELETE_REGISTRY_CHECKPOINT = KDDSL_EXTERNAL_CODE(41, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_GET_REGISTRY_CHECKPOINTS = KDDSL_EXTERNAL_CODE(42, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_ADD_CRYPTO_CHECKPOINT = KDDSL_EXTERNAL_CODE(43, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_DELETE_CRYPTO_CHECKPOINT = KDDSL_EXTERNAL_CODE(44, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_GET_CRYPTO_CHECKPOINTS = KDDSL_EXTERNAL_CODE(45, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_RESOURCE_UPGRADE_DLL = KDDSL_EXTERNAL_CODE(46, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_ADD_REGISTRY_CHECKPOINT_64BIT = KDDSL_EXTERNAL_CODE(47, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_ADD_REGISTRY_CHECKPOINT_32BIT = KDDSL_EXTERNAL_CODE(48, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_GET_LOADBAL_PROCESS_LIST = KDDSL_EXTERNAL_CODE(50, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_GET_NETWORK_NAME = KDDSL_EXTERNAL_CODE(90, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_NETNAME_GET_VIRTUAL_SERVER_TOKEN = KDDSL_EXTERNAL_CODE(91, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_NETNAME_REGISTER_DNS_RECORDS = KDDSL_EXTERNAL_CODE(92, CLUS_ACCESS_WRITE, CLUS_NO_MODIFY);

	public static int KDDSL_GET_DNS_NAME = KDDSL_EXTERNAL_CODE(93, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_NETNAME_SET_PWD_INFO = KDDSL_EXTERNAL_CODE(94, CLUS_ACCESS_WRITE, CLUS_NO_MODIFY);

	public static int KDDSL_NETNAME_DELETE_CO = KDDSL_EXTERNAL_CODE(95, CLUS_ACCESS_WRITE, CLUS_NO_MODIFY);

	public static int KDDSL_NETNAME_VALIDATE_VCO = KDDSL_EXTERNAL_CODE(96, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_NETNAME_RESET_VCO = KDDSL_EXTERNAL_CODE(97, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_NETNAME_REPAIR_VCO = KDDSL_EXTERNAL_CODE(99, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_GET_DISK_INFO = KDDSL_EXTERNAL_CODE(100, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_GET_AVAILABLE_DISKS = KDDSL_EXTERNAL_CODE(101, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_IS_PATH_VALID = KDDSL_EXTERNAL_CODE(102, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_SYNC_CLUSDISK_DB = KDDSL_EXTERNAL_CODE(103, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_STORAGE_GET_DISK_NUMBER_INFO = KDDSL_EXTERNAL_CODE(104, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_REPLICATION_GET_REPLICATED_DISKS = KDDSL_EXTERNAL_CODE(2133, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_REPLICATION_GET_RESOURCE_GROUP = KDDSL_EXTERNAL_CODE(2136, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_REPLICATION_GET_LOG_INFO = KDDSL_EXTERNAL_CODE(2129, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_QUERY_DELETE = KDDSL_EXTERNAL_CODE(110, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_IPADDRESS_RENEW_LEASE = KDDSL_EXTERNAL_CODE(111, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_IPADDRESS_RELEASE_LEASE = KDDSL_EXTERNAL_CODE(112, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_QUERY_MAINTENANCE_MODE = KDDSL_EXTERNAL_CODE(120, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_SET_MAINTENANCE_MODE = KDDSL_EXTERNAL_CODE(121, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_STORAGE_SET_DRIVELETTER = KDDSL_EXTERNAL_CODE(122, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_STORAGE_GET_DRIVELETTERS = KDDSL_EXTERNAL_CODE(123, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_GET_DISK_INFO_EX = KDDSL_EXTERNAL_CODE(124, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_GET_DISK_INFO_EX2 = KDDSL_EXTERNAL_CODE(126, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_GET_AVAILABLE_DISKS_EX = KDDSL_EXTERNAL_CODE(125, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_REMAP_DRIVELETTER = KDDSL_EXTERNAL_CODE(128, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_GET_DISKID = KDDSL_EXTERNAL_CODE(129, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_IS_CLUSTERABLE = KDDSL_EXTERNAL_CODE(130, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_REMOVE_VM_OWNERSHIP = KDDSL_EXTERNAL_CODE(131, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_STORAGE_GET_MOUNTPOINTS = KDDSL_EXTERNAL_CODE(132, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_GET_DIRTY = KDDSL_EXTERNAL_CODE(134, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_GET_SHARED_VOLUME_INFO = KDDSL_EXTERNAL_CODE(137, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_IS_CSV_FILE = KDDSL_EXTERNAL_CODE(138, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_VALIDATE_PATH = KDDSL_EXTERNAL_CODE(140, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_VALIDATE_NETNAME = KDDSL_EXTERNAL_CODE(141, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_VALIDATE_DIRECTORY = KDDSL_EXTERNAL_CODE(142, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_BATCH_BLOCK_KEY = KDDSL_EXTERNAL_CODE(143, CLUS_ACCESS_WRITE, CLUS_NO_MODIFY);

	public static int KDDSL_BATCH_UNBLOCK_KEY = KDDSL_EXTERNAL_CODE(144, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_FILESERVER_SHARE_ADD = KDDSL_EXTERNAL_CODE(145, CLUS_ACCESS_READ, CLUS_MODIFY);

	public static int KDDSL_FILESERVER_SHARE_DEL = KDDSL_EXTERNAL_CODE(146, CLUS_ACCESS_READ, CLUS_MODIFY);

	public static int KDDSL_FILESERVER_SHARE_MODIFY = KDDSL_EXTERNAL_CODE(147, CLUS_ACCESS_READ, CLUS_MODIFY);

	public static int KDDSL_FILESERVER_SHARE_REPORT = KDDSL_EXTERNAL_CODE(148, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_ENABLE_SHARED_VOLUME_DIRECTIO = KDDSL_EXTERNAL_CODE(162, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_DISABLE_SHARED_VOLUME_DIRECTIO = KDDSL_EXTERNAL_CODE(163, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_GET_SHARED_VOLUME_ID = KDDSL_EXTERNAL_CODE(164, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_SET_CSV_MAINTENANCE_MODE = KDDSL_EXTERNAL_CODE(165, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_SET_SHARED_VOLUME_BACKUP_MODE = KDDSL_EXTERNAL_CODE(166, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_STORAGE_GET_SHARED_VOLUME_PARTITION_NAMES = KDDSL_EXTERNAL_CODE(167, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_GET_SHARED_VOLUME_STATES = KDDSL_EXTERNAL_CODE(168, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_POOL_GET_DRIVE_INFO = KDDSL_EXTERNAL_CODE(173, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_DELETE = KDDSL_INTERNAL_CODE(1, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_INSTALL_NODE = KDDSL_INTERNAL_CODE(2, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_EVICT_NODE = KDDSL_INTERNAL_CODE(3, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_ADD_DEPENDENCY = KDDSL_INTERNAL_CODE(4, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_REMOVE_DEPENDENCY = KDDSL_INTERNAL_CODE(5, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_ADD_OWNER = KDDSL_INTERNAL_CODE(6, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_REMOVE_OWNER = KDDSL_INTERNAL_CODE(7, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_SET_NAME = KDDSL_INTERNAL_CODE(9, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_CLUSTER_NAME_CHANGED = KDDSL_INTERNAL_CODE(10, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_CLUSTER_VERSION_CHANGED = KDDSL_INTERNAL_CODE(11, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_FIXUP_ON_UPGRADE = KDDSL_INTERNAL_CODE(12, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_STARTING_PHASE1 = KDDSL_INTERNAL_CODE(13, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_STARTING_PHASE2 = KDDSL_INTERNAL_CODE(14, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_HOLD_IO = KDDSL_INTERNAL_CODE(15, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_RESUME_IO = KDDSL_INTERNAL_CODE(16, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_FORCE_QUORUM = KDDSL_INTERNAL_CODE(17, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_INITIALIZE = KDDSL_INTERNAL_CODE(18, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_STATE_CHANGE_REASON = KDDSL_INTERNAL_CODE(19, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_PROVIDER_STATE_CHANGE = KDDSL_INTERNAL_CODE(20, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_LEAVING_GROUP = KDDSL_INTERNAL_CODE(21, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_JOINING_GROUP = KDDSL_INTERNAL_CODE(22, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_FSWITNESS_GET_EPOCH_INFO = KDDSL_INTERNAL_CODE(23, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_FSWITNESS_SET_EPOCH_INFO = KDDSL_INTERNAL_CODE(24, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_FSWITNESS_RELEASE_LOCK = KDDSL_INTERNAL_CODE(25, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_NETNAME_CREDS_NOTIFYCAM = KDDSL_INTERNAL_CODE(26, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_STORAGE_GET_DISK_NUMBER = KDDSL_INTERNAL_CODE(27, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_STORAGE_GET_CSV_DISK_INFO = KDDSL_INTERNAL_CODE(28, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_SET_CLUSTER_MEMBERSHIP = KDDSL_INTERNAL_CODE(29, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_SET_SHARED_PR_KEY = KDDSL_INTERNAL_CODE(30, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	public static int KDDSL_QUERY_CSV_MAINTENANCE_MODE = KDDSL_INTERNAL_CODE(31, CLUS_ACCESS_READ, CLUS_NO_MODIFY);

	public static int KDDSL_OFFLINE_DEPENDENT_GROUPS = KDDSL_EXTERNAL_CODE(2014, CLUS_ACCESS_WRITE, CLUS_MODIFY);

	private const int CLUS_SP_NAME_LENGTH = 512;

	public static int CLUSCTL_CLUSTER_UNKNOWN = CLUSCTL_CLUSTER_CODE(KDDSL_UNKNOWN);

	public static int CLUSCTL_CLUSTER_GET_FQDN = CLUSCTL_CLUSTER_CODE(KDDSL_GET_FQDN);

	public static int CLUSCTL_CLUSTER_ENUM_COMMON_PROPERTIES = CLUSCTL_CLUSTER_CODE(KDDSL_ENUM_COMMON_PROPERTIES);

	public static int CLUSCTL_CLUSTER_GET_RO_COMMON_PROPERTIES = CLUSCTL_CLUSTER_CODE(KDDSL_GET_RO_COMMON_PROPERTIES);

	public static int CLUSCTL_CLUSTER_GET_COMMON_PROPERTIES = CLUSCTL_CLUSTER_CODE(KDDSL_GET_COMMON_PROPERTIES);

	public static int CLUSCTL_CLUSTER_SET_COMMON_PROPERTIES = CLUSCTL_CLUSTER_CODE(KDDSL_SET_COMMON_PROPERTIES);

	public static int CLUSCTL_CLUSTER_VALIDATE_COMMON_PROPERTIES = CLUSCTL_CLUSTER_CODE(KDDSL_VALIDATE_COMMON_PROPERTIES);

	public static int CLUSCTL_CLUSTER_ENUM_PRIVATE_PROPERTIES = CLUSCTL_CLUSTER_CODE(KDDSL_ENUM_PRIVATE_PROPERTIES);

	public static int CLUSCTL_CLUSTER_GET_RO_PRIVATE_PROPERTIES = CLUSCTL_CLUSTER_CODE(KDDSL_GET_RO_PRIVATE_PROPERTIES);

	public static int CLUSCTL_CLUSTER_GET_PRIVATE_PROPERTIES = CLUSCTL_CLUSTER_CODE(KDDSL_GET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_CLUSTER_SET_PRIVATE_PROPERTIES = CLUSCTL_CLUSTER_CODE(KDDSL_SET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_CLUSTER_VALIDATE_PRIVATE_PROPERTIES = CLUSCTL_CLUSTER_CODE(KDDSL_VALIDATE_PRIVATE_PROPERTIES);

	public static int CLUSCTL_CLUSTER_GET_COMMON_PROPERTY_FMTS = CLUSCTL_CLUSTER_CODE(KDDSL_GET_COMMON_PROPERTY_FMTS);

	public static int CLUSCTL_CLUSTER_GET_PRIVATE_PROPERTY_FMTS = CLUSCTL_CLUSTER_CODE(KDDSL_GET_PRIVATE_PROPERTY_FMTS);

	public static int CLUSCTL_CLUSTER_CHECK_VOTER_EVICT = CLUSCTL_CLUSTER_CODE(KDDSL_CHECK_VOTER_EVICT);

	public static int CLUSCTL_CLUSTER_CHECK_VOTER_DOWN = CLUSCTL_CLUSTER_CODE(KDDSL_CHECK_VOTER_DOWN);

	public static int CLUSCTL_CLUSTER_SHUTDOWN = CLUSCTL_CLUSTER_CODE(KDDSL_SHUTDOWN);

	public static int CLUSCTL_CLUSTER_BATCH_BLOCK_KEY = CLUSCTL_CLUSTER_CODE(KDDSL_BATCH_BLOCK_KEY);

	public static int CLUSCTL_CLUSTER_BATCH_UNBLOCK_KEY = CLUSCTL_CLUSTER_CODE(KDDSL_BATCH_UNBLOCK_KEY);

	public static int CLUSCTL_CLUSTER_GET_SHARED_VOLUME_ID = CLUSCTL_CLUSTER_CODE(KDDSL_GET_SHARED_VOLUME_ID);

	public static int CLUSCTL_GROUP_UNKNOWN = CLUSCTL_GROUP_CODE(KDDSL_UNKNOWN);

	public static int CLUSCTL_GROUP_GET_CHARACTERISTICS = CLUSCTL_GROUP_CODE(KDDSL_GET_CHARACTERISTICS);

	public static int CLUSCTL_GROUP_GET_FLAGS = CLUSCTL_GROUP_CODE(KDDSL_GET_FLAGS);

	public static int CLUSCTL_GROUP_GET_NAME = CLUSCTL_GROUP_CODE(KDDSL_GET_NAME);

	public static int CLUSCTL_GROUP_GET_ID = CLUSCTL_GROUP_CODE(KDDSL_GET_ID);

	public static int CLUSCTL_GROUP_ENUM_COMMON_PROPERTIES = CLUSCTL_GROUP_CODE(KDDSL_ENUM_COMMON_PROPERTIES);

	public static int CLUSCTL_GROUP_GET_RO_COMMON_PROPERTIES = CLUSCTL_GROUP_CODE(KDDSL_GET_RO_COMMON_PROPERTIES);

	public static int CLUSCTL_GROUP_GET_COMMON_PROPERTIES = CLUSCTL_GROUP_CODE(KDDSL_GET_COMMON_PROPERTIES);

	public static int CLUSCTL_GROUP_SET_COMMON_PROPERTIES = CLUSCTL_GROUP_CODE(KDDSL_SET_COMMON_PROPERTIES);

	public static int CLUSCTL_GROUP_VALIDATE_COMMON_PROPERTIES = CLUSCTL_GROUP_CODE(KDDSL_VALIDATE_COMMON_PROPERTIES);

	public static int CLUSCTL_GROUP_ENUM_PRIVATE_PROPERTIES = CLUSCTL_GROUP_CODE(KDDSL_ENUM_PRIVATE_PROPERTIES);

	public static int CLUSCTL_GROUP_GET_RO_PRIVATE_PROPERTIES = CLUSCTL_GROUP_CODE(KDDSL_GET_RO_PRIVATE_PROPERTIES);

	public static int CLUSCTL_GROUP_GET_PRIVATE_PROPERTIES = CLUSCTL_GROUP_CODE(KDDSL_GET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_GROUP_SET_PRIVATE_PROPERTIES = CLUSCTL_GROUP_CODE(KDDSL_SET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_GROUP_VALIDATE_PRIVATE_PROPERTIES = CLUSCTL_GROUP_CODE(KDDSL_VALIDATE_PRIVATE_PROPERTIES);

	public static int CLUSCTL_GROUP_QUERY_DELETE = CLUSCTL_GROUP_CODE(KDDSL_QUERY_DELETE);

	public static int CLUSCTL_GROUP_GET_COMMON_PROPERTY_FMTS = CLUSCTL_GROUP_CODE(KDDSL_GET_COMMON_PROPERTY_FMTS);

	public static int CLUSCTL_GROUP_GET_PRIVATE_PROPERTY_FMTS = CLUSCTL_GROUP_CODE(KDDSL_GET_PRIVATE_PROPERTY_FMTS);

	public static int CLUSCTL_NODE_UNKNOWN = CLUSCTL_NODE_CODE(KDDSL_UNKNOWN);

	public static int CLUSCTL_NODE_GET_CHARACTERISTICS = CLUSCTL_NODE_CODE(KDDSL_GET_CHARACTERISTICS);

	public static int CLUSCTL_NODE_GET_FLAGS = CLUSCTL_NODE_CODE(KDDSL_GET_FLAGS);

	public static int CLUSCTL_NODE_GET_NAME = CLUSCTL_NODE_CODE(KDDSL_GET_NAME);

	public static int CLUSCTL_NODE_GET_ID = CLUSCTL_NODE_CODE(KDDSL_GET_ID);

	public static int CLUSCTL_NODE_ENUM_COMMON_PROPERTIES = CLUSCTL_NODE_CODE(KDDSL_ENUM_COMMON_PROPERTIES);

	public static int CLUSCTL_NODE_GET_RO_COMMON_PROPERTIES = CLUSCTL_NODE_CODE(KDDSL_GET_RO_COMMON_PROPERTIES);

	public static int CLUSCTL_NODE_GET_COMMON_PROPERTIES = CLUSCTL_NODE_CODE(KDDSL_GET_COMMON_PROPERTIES);

	public static int CLUSCTL_NODE_SET_COMMON_PROPERTIES = CLUSCTL_NODE_CODE(KDDSL_SET_COMMON_PROPERTIES);

	public static int CLUSCTL_NODE_VALIDATE_COMMON_PROPERTIES = CLUSCTL_NODE_CODE(KDDSL_VALIDATE_COMMON_PROPERTIES);

	public static int CLUSCTL_NODE_ENUM_PRIVATE_PROPERTIES = CLUSCTL_NODE_CODE(KDDSL_ENUM_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NODE_GET_RO_PRIVATE_PROPERTIES = CLUSCTL_NODE_CODE(KDDSL_GET_RO_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NODE_GET_PRIVATE_PROPERTIES = CLUSCTL_NODE_CODE(KDDSL_GET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NODE_SET_PRIVATE_PROPERTIES = CLUSCTL_NODE_CODE(KDDSL_SET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NODE_VALIDATE_PRIVATE_PROPERTIES = CLUSCTL_NODE_CODE(KDDSL_VALIDATE_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NODE_GET_COMMON_PROPERTY_FMTS = CLUSCTL_NODE_CODE(KDDSL_GET_COMMON_PROPERTY_FMTS);

	public static int CLUSCTL_NODE_GET_PRIVATE_PROPERTY_FMTS = CLUSCTL_NODE_CODE(KDDSL_GET_PRIVATE_PROPERTY_FMTS);

	public static int CLUSCTL_NODE_GET_CLUSTER_SERVICE_ACCOUNT_NAME = CLUSCTL_NODE_CODE(KDDSL_GET_CLUSTER_SERVICE_ACCOUNT_NAME);

	public static int CLUSCTL_RESOURCE_UNKNOWN = CLUSCTL_RESOURCE_CODE(KDDSL_UNKNOWN);

	public static int CLUSCTL_RESOURCE_GET_CHARACTERISTICS = CLUSCTL_RESOURCE_CODE(KDDSL_GET_CHARACTERISTICS);

	public static int CLUSCTL_RESOURCE_GET_FLAGS = CLUSCTL_RESOURCE_CODE(KDDSL_GET_FLAGS);

	public static int CLUSCTL_RESOURCE_GET_CLASS_INFO = CLUSCTL_RESOURCE_CODE(KDDSL_GET_CLASS_INFO);

	public static int CLUSCTL_RESOURCE_GET_REQUIRED_DEPENDENCIES = CLUSCTL_RESOURCE_CODE(KDDSL_GET_REQUIRED_DEPENDENCIES);

	public static int CLUSCTL_RESOURCE_GET_NAME = CLUSCTL_RESOURCE_CODE(KDDSL_GET_NAME);

	public static int CLUSCTL_RESOURCE_GET_ID = CLUSCTL_RESOURCE_CODE(KDDSL_GET_ID);

	public static int CLUSCTL_RESOURCE_GET_RESOURCE_TYPE = CLUSCTL_RESOURCE_CODE(KDDSL_GET_RESOURCE_TYPE);

	public static int CLUSCTL_RESOURCE_ENUM_COMMON_PROPERTIES = CLUSCTL_RESOURCE_CODE(KDDSL_ENUM_COMMON_PROPERTIES);

	public static int CLUSCTL_RESOURCE_GET_RO_COMMON_PROPERTIES = CLUSCTL_RESOURCE_CODE(KDDSL_GET_RO_COMMON_PROPERTIES);

	public static int CLUSCTL_RESOURCE_GET_COMMON_PROPERTIES = CLUSCTL_RESOURCE_CODE(KDDSL_GET_COMMON_PROPERTIES);

	public static int CLUSCTL_RESOURCE_SET_COMMON_PROPERTIES = CLUSCTL_RESOURCE_CODE(KDDSL_SET_COMMON_PROPERTIES);

	public static int CLUSCTL_RESOURCE_VALIDATE_COMMON_PROPERTIES = CLUSCTL_RESOURCE_CODE(KDDSL_VALIDATE_COMMON_PROPERTIES);

	public static int CLUSCTL_RESOURCE_GET_COMMON_PROPERTY_FMTS = CLUSCTL_RESOURCE_CODE(KDDSL_GET_COMMON_PROPERTY_FMTS);

	public static int CLUSCTL_RESOURCE_ENUM_PRIVATE_PROPERTIES = CLUSCTL_RESOURCE_CODE(KDDSL_ENUM_PRIVATE_PROPERTIES);

	public static int CLUSCTL_RESOURCE_GET_RO_PRIVATE_PROPERTIES = CLUSCTL_RESOURCE_CODE(KDDSL_GET_RO_PRIVATE_PROPERTIES);

	public static int CLUSCTL_RESOURCE_GET_PRIVATE_PROPERTIES = CLUSCTL_RESOURCE_CODE(KDDSL_GET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_RESOURCE_SET_PRIVATE_PROPERTIES = CLUSCTL_RESOURCE_CODE(KDDSL_SET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_RESOURCE_VALIDATE_PRIVATE_PROPERTIES = CLUSCTL_RESOURCE_CODE(KDDSL_VALIDATE_PRIVATE_PROPERTIES);

	public static int CLUSCTL_RESOURCE_GET_PRIVATE_PROPERTY_FMTS = CLUSCTL_RESOURCE_CODE(KDDSL_GET_PRIVATE_PROPERTY_FMTS);

	public static int CLUSCTL_RESOURCE_ADD_REGISTRY_CHECKPOINT = CLUSCTL_RESOURCE_CODE(KDDSL_ADD_REGISTRY_CHECKPOINT);

	public static int CLUSCTL_RESOURCE_DELETE_REGISTRY_CHECKPOINT = CLUSCTL_RESOURCE_CODE(KDDSL_DELETE_REGISTRY_CHECKPOINT);

	public static int CLUSCTL_RESOURCE_GET_REGISTRY_CHECKPOINTS = CLUSCTL_RESOURCE_CODE(KDDSL_GET_REGISTRY_CHECKPOINTS);

	public static int CLUSCTL_RESOURCE_ADD_CRYPTO_CHECKPOINT = CLUSCTL_RESOURCE_CODE(KDDSL_ADD_CRYPTO_CHECKPOINT);

	public static int CLUSCTL_RESOURCE_DELETE_CRYPTO_CHECKPOINT = CLUSCTL_RESOURCE_CODE(KDDSL_DELETE_CRYPTO_CHECKPOINT);

	public static int CLUSCTL_RESOURCE_GET_CRYPTO_CHECKPOINTS = CLUSCTL_RESOURCE_CODE(KDDSL_GET_CRYPTO_CHECKPOINTS);

	public static int CLUSCTL_RESOURCE_GET_LOADBAL_PROCESS_LIST = CLUSCTL_RESOURCE_CODE(KDDSL_GET_LOADBAL_PROCESS_LIST);

	public static int CLUSCTL_RESOURCE_GET_NETWORK_NAME = CLUSCTL_RESOURCE_CODE(KDDSL_GET_NETWORK_NAME);

	public static int CLUSCTL_RESOURCE_NETNAME_GET_VIRTUAL_SERVER_TOKEN = CLUSCTL_RESOURCE_CODE(KDDSL_NETNAME_GET_VIRTUAL_SERVER_TOKEN);

	public static int CLUSCTL_RESOURCE_NETNAME_SET_PWD_INFO = CLUSCTL_RESOURCE_CODE(KDDSL_NETNAME_SET_PWD_INFO);

	public static int CLUSCTL_RESOURCE_NETNAME_DELETE_CO = CLUSCTL_RESOURCE_CODE(KDDSL_NETNAME_DELETE_CO);

	public static int CLUSCTL_RESOURCE_NETNAME_VALIDATE_VCO = CLUSCTL_RESOURCE_CODE(KDDSL_NETNAME_VALIDATE_VCO);

	public static int CLUSCTL_RESOURCE_NETNAME_RESET_VCO = CLUSCTL_RESOURCE_CODE(KDDSL_NETNAME_RESET_VCO);

	public static int CLUSCTL_RESOURCE_NETNAME_REGISTER_DNS_RECORDS = CLUSCTL_RESOURCE_CODE(KDDSL_NETNAME_REGISTER_DNS_RECORDS);

	public static int CLUSCTL_RESOURCE_NETNAME_REPAIR_VCO = CLUSCTL_RESOURCE_CODE(KDDSL_NETNAME_REPAIR_VCO);

	public static int CLUSCTL_RESOURCE_GET_DNS_NAME = CLUSCTL_RESOURCE_CODE(KDDSL_GET_DNS_NAME);

	public static int CLUSCTL_RESOURCE_STORAGE_GET_DISK_INFO = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_GET_DISK_INFO);

	public static int CLUSCTL_RESOURCE_STORAGE_IS_PATH_VALID = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_IS_PATH_VALID);

	public static int CLUSCTL_RESOURCE_QUERY_DELETE = CLUSCTL_RESOURCE_CODE(KDDSL_QUERY_DELETE);

	public static int CLUSCTL_RESOURCE_UPGRADE_DLL = CLUSCTL_RESOURCE_CODE(KDDSL_RESOURCE_UPGRADE_DLL);

	public static int CLUSCTL_RESOURCE_IPADDRESS_RENEW_LEASE = CLUSCTL_RESOURCE_CODE(KDDSL_IPADDRESS_RENEW_LEASE);

	public static int CLUSCTL_RESOURCE_IPADDRESS_RELEASE_LEASE = CLUSCTL_RESOURCE_CODE(KDDSL_IPADDRESS_RELEASE_LEASE);

	public static int CLUSCTL_RESOURCE_ADD_REGISTRY_CHECKPOINT_64BIT = CLUSCTL_RESOURCE_CODE(KDDSL_ADD_REGISTRY_CHECKPOINT_64BIT);

	public static int CLUSCTL_RESOURCE_ADD_REGISTRY_CHECKPOINT_32BIT = CLUSCTL_RESOURCE_CODE(KDDSL_ADD_REGISTRY_CHECKPOINT_32BIT);

	public static int CLUSCTL_RESOURCE_QUERY_MAINTENANCE_MODE = CLUSCTL_RESOURCE_CODE(KDDSL_QUERY_MAINTENANCE_MODE);

	public static int CLUSCTL_RESOURCE_SET_MAINTENANCE_MODE = CLUSCTL_RESOURCE_CODE(KDDSL_SET_MAINTENANCE_MODE);

	public static int CLUSCTL_RESOURCE_STORAGE_SET_DRIVELETTER = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_SET_DRIVELETTER);

	public static int CLUSCTL_RESOURCE_STORAGE_GET_DISK_INFO_EX = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_GET_DISK_INFO_EX);

	public static int CLUSCTL_RESOURCE_STORAGE_GET_DISK_INFO_EX2 = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_GET_DISK_INFO_EX2);

	public static int CLUSCTL_RESOURCE_FILESERVER_SHARE_ADD = CLUSCTL_RESOURCE_CODE(KDDSL_FILESERVER_SHARE_ADD);

	public static int CLUSCTL_RESOURCE_FILESERVER_SHARE_DEL = CLUSCTL_RESOURCE_CODE(KDDSL_FILESERVER_SHARE_DEL);

	public static int CLUSCTL_RESOURCE_FILESERVER_SHARE_MODIFY = CLUSCTL_RESOURCE_CODE(KDDSL_FILESERVER_SHARE_MODIFY);

	public static int CLUSCTL_RESOURCE_FILESERVER_SHARE_REPORT = CLUSCTL_RESOURCE_CODE(KDDSL_FILESERVER_SHARE_REPORT);

	public static int CLUSCTL_RESOURCE_STORAGE_GET_MOUNTPOINTS = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_GET_MOUNTPOINTS);

	public static int CLUSCTL_RESOURCE_STORAGE_GET_DIRTY = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_GET_DIRTY);

	public static int CLUSCTL_RESOURCE_STORAGE_GET_SHARED_VOLUME_INFO = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_GET_SHARED_VOLUME_INFO);

	public static int CLUSCTL_RESOURCE_STORAGE_GET_SHARED_VOLUME_STATES = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_GET_SHARED_VOLUME_STATES);

	public static int CLUSCTL_RESOURCE_SET_CSV_MAINTENANCE_MODE = CLUSCTL_RESOURCE_CODE(KDDSL_SET_CSV_MAINTENANCE_MODE);

	public static int CLUSCTL_RESOURCE_ENABLE_SHARED_VOLUME_DIRECTIO = CLUSCTL_RESOURCE_CODE(KDDSL_ENABLE_SHARED_VOLUME_DIRECTIO);

	public static int CLUSCTL_RESOURCE_DISABLE_SHARED_VOLUME_DIRECTIO = CLUSCTL_RESOURCE_CODE(KDDSL_DISABLE_SHARED_VOLUME_DIRECTIO);

	public static int CLUSCTL_RESOURCE_SET_SHARED_VOLUME_BACKUP_MODE = CLUSCTL_RESOURCE_CODE(KDDSL_SET_SHARED_VOLUME_BACKUP_MODE);

	public static int CLUSCTL_RESOURCE_STORAGE_GET_SHARED_VOLUME_PARTITION_NAMES = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_GET_SHARED_VOLUME_PARTITION_NAMES);

	public static int CLUSCTL_RESOURCE_POOL_GET_DRIVE_INFO = CLUSCTL_RESOURCE_CODE(KDDSL_POOL_GET_DRIVE_INFO);

	public static int CLUSCTL_RESOURCE_DELETE = CLUSCTL_RESOURCE_CODE(KDDSL_DELETE);

	public static int CLUSCTL_RESOURCE_INSTALL_NODE = CLUSCTL_RESOURCE_CODE(KDDSL_INSTALL_NODE);

	public static int CLUSCTL_RESOURCE_EVICT_NODE = CLUSCTL_RESOURCE_CODE(KDDSL_EVICT_NODE);

	public static int CLUSCTL_RESOURCE_ADD_DEPENDENCY = CLUSCTL_RESOURCE_CODE(KDDSL_ADD_DEPENDENCY);

	public static int CLUSCTL_RESOURCE_REMOVE_DEPENDENCY = CLUSCTL_RESOURCE_CODE(KDDSL_REMOVE_DEPENDENCY);

	public static int CLUSCTL_RESOURCE_ADD_OWNER = CLUSCTL_RESOURCE_CODE(KDDSL_ADD_OWNER);

	public static int CLUSCTL_RESOURCE_REMOVE_OWNER = CLUSCTL_RESOURCE_CODE(KDDSL_REMOVE_OWNER);

	public static int CLUSCTL_RESOURCE_SET_NAME = CLUSCTL_RESOURCE_CODE(KDDSL_SET_NAME);

	public static int CLUSCTL_RESOURCE_CLUSTER_NAME_CHANGED = CLUSCTL_RESOURCE_CODE(KDDSL_CLUSTER_NAME_CHANGED);

	public static int CLUSCTL_RESOURCE_CLUSTER_VERSION_CHANGED = CLUSCTL_RESOURCE_CODE(KDDSL_CLUSTER_VERSION_CHANGED);

	public static int CLUSCTL_RESOURCE_FORCE_QUORUM = CLUSCTL_RESOURCE_CODE(KDDSL_FORCE_QUORUM);

	public static int CLUSCTL_RESOURCE_INITIALIZE = CLUSCTL_RESOURCE_CODE(KDDSL_INITIALIZE);

	public static int CLUSCTL_RESOURCE_STATE_CHANGE_REASON = CLUSCTL_RESOURCE_CODE(KDDSL_STATE_CHANGE_REASON);

	public static int CLUSCTL_RESOURCE_PROVIDER_STATE_CHANGE = CLUSCTL_RESOURCE_CODE(KDDSL_PROVIDER_STATE_CHANGE);

	public static int CLUSCTL_RESOURCE_LEAVING_GROUP = CLUSCTL_RESOURCE_CODE(KDDSL_LEAVING_GROUP);

	public static int CLUSCTL_RESOURCE_JOINING_GROUP = CLUSCTL_RESOURCE_CODE(KDDSL_JOINING_GROUP);

	public static int CLUSCTL_RESOURCE_FSWITNESS_GET_EPOCH_INFO = CLUSCTL_RESOURCE_CODE(KDDSL_FSWITNESS_GET_EPOCH_INFO);

	public static int CLUSCTL_RESOURCE_FSWITNESS_SET_EPOCH_INFO = CLUSCTL_RESOURCE_CODE(KDDSL_FSWITNESS_SET_EPOCH_INFO);

	public static int CLUSCTL_RESOURCE_FSWITNESS_RELEASE_LOCK = CLUSCTL_RESOURCE_CODE(KDDSL_FSWITNESS_RELEASE_LOCK);

	public static int CLUSCTL_RESOURCE_NETNAME_CREDS_NOTIFYCAM = CLUSCTL_RESOURCE_CODE(KDDSL_NETNAME_CREDS_NOTIFYCAM);

	public static int CLUSCTL_RESOURCE_SET_SHARED_PR_KEY = CLUSCTL_RESOURCE_CODE(KDDSL_SET_SHARED_PR_KEY);

	public static int CLUSCTL_RESOURCE_STORAGE_GET_DISK_NUMBER = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_GET_DISK_NUMBER);

	public static int CLUSCTL_RESOURCE_STORAGE_GET_DISK_NUMBER_INFO = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_GET_DISK_NUMBER_INFO);

	public static int CLUSCTL_RESOURCE_STORAGE_GET_CSV_DISK_INFO = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_GET_CSV_DISK_INFO);

	public static int CLUSCTL_RESOURCE_OFFLINE_DEPENDENT_GROUPS = CLUSCTL_RESOURCE_CODE(KDDSL_OFFLINE_DEPENDENT_GROUPS);

	public static int CLUSCTL_RESOURCE_TYPE_UNKNOWN = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_UNKNOWN);

	public static int CLUSCTL_RESOURCE_TYPE_GET_CHARACTERISTICS = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_CHARACTERISTICS);

	public static int CLUSCTL_RESOURCE_TYPE_GET_FLAGS = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_FLAGS);

	public static int CLUSCTL_RESOURCE_TYPE_GET_CLASS_INFO = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_CLASS_INFO);

	public static int CLUSCTL_RESOURCE_TYPE_GET_REQUIRED_DEPENDENCIES = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_REQUIRED_DEPENDENCIES);

	public static int CLUSCTL_RESOURCE_TYPE_GET_ARB_TIMEOUT = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_ARB_TIMEOUT);

	public static int CLUSCTL_RESOURCE_TYPE_ENUM_COMMON_PROPERTIES = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_ENUM_COMMON_PROPERTIES);

	public static int CLUSCTL_RESOURCE_TYPE_GET_RO_COMMON_PROPERTIES = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_RO_COMMON_PROPERTIES);

	public static int CLUSCTL_RESOURCE_TYPE_GET_COMMON_PROPERTIES = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_COMMON_PROPERTIES);

	public static int CLUSCTL_RESOURCE_TYPE_VALIDATE_COMMON_PROPERTIES = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_VALIDATE_COMMON_PROPERTIES);

	public static int CLUSCTL_RESOURCE_TYPE_SET_COMMON_PROPERTIES = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_SET_COMMON_PROPERTIES);

	public static int CLUSCTL_RESOURCE_TYPE_GET_COMMON_PROPERTY_FMTS = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_COMMON_PROPERTY_FMTS);

	public static int CLUSCTL_RESOURCE_TYPE_GET_COMMON_RESOURCE_PROPERTY_FMTS = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_COMMON_RESOURCE_PROPERTY_FMTS);

	public static int CLUSCTL_RESOURCE_TYPE_ENUM_PRIVATE_PROPERTIES = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_ENUM_PRIVATE_PROPERTIES);

	public static int CLUSCTL_RESOURCE_TYPE_GET_RO_PRIVATE_PROPERTIES = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_RO_PRIVATE_PROPERTIES);

	public static int CLUSCTL_RESOURCE_TYPE_GET_PRIVATE_PROPERTIES = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_RESOURCE_TYPE_SET_PRIVATE_PROPERTIES = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_SET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_RESOURCE_TYPE_VALIDATE_PRIVATE_PROPERTIES = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_VALIDATE_PRIVATE_PROPERTIES);

	public static int CLUSCTL_RESOURCE_TYPE_GET_PRIVATE_PROPERTY_FMTS = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_PRIVATE_PROPERTY_FMTS);

	public static int CLUSCTL_RESOURCE_TYPE_GET_PRIVATE_RESOURCE_PROPERTY_FMTS = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_PRIVATE_RESOURCE_PROPERTY_FMTS);

	public static int CLUSCTL_RESOURCE_TYPE_GET_REGISTRY_CHECKPOINTS = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_REGISTRY_CHECKPOINTS);

	public static int CLUSCTL_RESOURCE_TYPE_GET_CRYPTO_CHECKPOINTS = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_GET_CRYPTO_CHECKPOINTS);

	public static int CLUSCTL_RESOURCE_TYPE_STORAGE_GET_AVAILABLE_DISKS = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_STORAGE_GET_AVAILABLE_DISKS);

	public static int CLUSCTL_RESOURCE_TYPE_STORAGE_SYNC_CLUSDISK_DB = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_STORAGE_SYNC_CLUSDISK_DB);

	public static int CLUSCTL_RESOURCE_TYPE_NETNAME_VALIDATE_NETNAME = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_VALIDATE_NETNAME);

	public static int CLUSCTL_RESOURCE_TYPE_GEN_APP_VALIDATE_PATH = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_VALIDATE_PATH);

	public static int CLUSCTL_RESOURCE_TYPE_GEN_APP_VALIDATE_DIRECTORY = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_VALIDATE_DIRECTORY);

	public static int CLUSCTL_RESOURCE_TYPE_GEN_SCRIPT_VALIDATE_PATH = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_VALIDATE_PATH);

	public static int CLUSCTL_RESOURCE_TYPE_QUERY_DELETE = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_QUERY_DELETE);

	public static int CLUSCTL_RESOURCE_TYPE_STORAGE_GET_DRIVELETTERS = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_STORAGE_GET_DRIVELETTERS);

	public static int CLUSCTL_RESOURCE_TYPE_STORAGE_GET_AVAILABLE_DISKS_EX = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_STORAGE_GET_AVAILABLE_DISKS_EX);

	public static int CLUSCTL_RESOURCE_TYPE_STORAGE_REMAP_DRIVELETTER = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_STORAGE_REMAP_DRIVELETTER);

	public static int CLUSCTL_RESOURCE_TYPE_STORAGE_GET_DISKID = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_STORAGE_GET_DISKID);

	public static int CLUSCTL_RESOURCE_TYPE_STORAGE_IS_CLUSTERABLE = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_STORAGE_IS_CLUSTERABLE);

	public static int CLUSCTL_RESOURCE_TYPE_STORAGE_REMOVE_VM_OWNERSHIP = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_STORAGE_REMOVE_VM_OWNERSHIP);

	public static int CLUSCTL_RESOURCE_TYPE_STORAGE_IS_CSV_FILE = CLUSCTL_RESOURCE_CODE(KDDSL_STORAGE_IS_CSV_FILE);

	public static int CLUSCTL_RESOURCE_TYPE_WITNESS_VALIDATE_PATH = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_VALIDATE_PATH);

	public static int CLUSCTL_RESOURCE_TYPE_REPLICATION_GET_REPLICATED_DISKS = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_REPLICATION_GET_REPLICATED_DISKS);

	public static int CLUSCTL_RESOURCE_TYPE_REPLICATION_GET_RESOURCE_GROUP = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_REPLICATION_GET_RESOURCE_GROUP);

	public static int CLUSCTL_RESOURCE_TYPE_REPLICATION_GET_LOG_INFO = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_REPLICATION_GET_LOG_INFO);

	public static int CLUSCTL_NETINTERFACE_ENUM_COMMON_PROPERTIES = CLUSCTL_NETINTERFACE_CODE(KDDSL_ENUM_COMMON_PROPERTIES);

	public static int CLUSCTL_NETINTERFACE_ENUM_PRIVATE_PROPERTIES = CLUSCTL_NETINTERFACE_CODE(KDDSL_ENUM_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NETINTERFACE_GET_CHARACTERISTICS = CLUSCTL_NETINTERFACE_CODE(KDDSL_GET_CHARACTERISTICS);

	public static int CLUSCTL_NETINTERFACE_GET_COMMON_PROPERTIES = CLUSCTL_NETINTERFACE_CODE(KDDSL_GET_COMMON_PROPERTIES);

	public static int CLUSCTL_NETINTERFACE_GET_COMMON_PROPERTY_FMTS = CLUSCTL_NETINTERFACE_CODE(KDDSL_GET_COMMON_PROPERTY_FMTS);

	public static int CLUSCTL_NETINTERFACE_GET_FLAGS = CLUSCTL_NETINTERFACE_CODE(KDDSL_GET_FLAGS);

	public static int CLUSCTL_NETINTERFACE_GET_ID = CLUSCTL_NETINTERFACE_CODE(KDDSL_GET_ID);

	public static int CLUSCTL_NETINTERFACE_GET_NAME = CLUSCTL_NETINTERFACE_CODE(KDDSL_GET_NAME);

	public static int CLUSCTL_NETINTERFACE_GET_NETWORK = CLUSCTL_NETINTERFACE_CODE(KDDSL_GET_NETWORK);

	public static int CLUSCTL_NETINTERFACE_GET_NODE = CLUSCTL_NETINTERFACE_CODE(KDDSL_GET_NODE);

	public static int CLUSCTL_NETINTERFACE_GET_PRIVATE_PROPERTIES = CLUSCTL_NETINTERFACE_CODE(KDDSL_GET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NETINTERFACE_GET_PRIVATE_PROPERTY_FMTS = CLUSCTL_NETINTERFACE_CODE(KDDSL_GET_PRIVATE_PROPERTY_FMTS);

	public static int CLUSCTL_NETINTERFACE_GET_RO_COMMON_PROPERTIES = CLUSCTL_NETINTERFACE_CODE(KDDSL_GET_RO_COMMON_PROPERTIES);

	public static int CLUSCTL_NETINTERFACE_GET_RO_PRIVATE_PROPERTIES = CLUSCTL_NETINTERFACE_CODE(KDDSL_GET_RO_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NETINTERFACE_SET_COMMON_PROPERTIES = CLUSCTL_NETINTERFACE_CODE(KDDSL_SET_COMMON_PROPERTIES);

	public static int CLUSCTL_NETINTERFACE_SET_PRIVATE_PROPERTIES = CLUSCTL_NETINTERFACE_CODE(KDDSL_SET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NETINTERFACE_UNKNOWN = CLUSCTL_NETINTERFACE_CODE(KDDSL_UNKNOWN);

	public static int CLUSCTL_NETINTERFACE_VALIDATE_COMMON_PROPERTIES = CLUSCTL_NETINTERFACE_CODE(KDDSL_VALIDATE_COMMON_PROPERTIES);

	public static int CLUSCTL_NETINTERFACE_VALIDATE_PRIVATE_PROPERTIES = CLUSCTL_NETINTERFACE_CODE(KDDSL_VALIDATE_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NETWORK_ENUM_COMMON_PROPERTIES = CLUSCTL_NETWORK_CODE(KDDSL_ENUM_COMMON_PROPERTIES);

	public static int CLUSCTL_NETWORK_ENUM_PRIVATE_PROPERTIES = CLUSCTL_NETWORK_CODE(KDDSL_ENUM_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NETWORK_GET_CHARACTERISTICS = CLUSCTL_NETWORK_CODE(KDDSL_GET_CHARACTERISTICS);

	public static int CLUSCTL_NETWORK_GET_COMMON_PROPERTIES = CLUSCTL_NETWORK_CODE(KDDSL_GET_COMMON_PROPERTIES);

	public static int CLUSCTL_NETWORK_GET_COMMON_PROPERTY_FMTS = CLUSCTL_NETWORK_CODE(KDDSL_GET_COMMON_PROPERTY_FMTS);

	public static int CLUSCTL_NETWORK_GET_FLAGS = CLUSCTL_NETWORK_CODE(KDDSL_GET_FLAGS);

	public static int CLUSCTL_NETWORK_GET_ID = CLUSCTL_NETWORK_CODE(KDDSL_GET_ID);

	public static int CLUSCTL_NETWORK_GET_NAME = CLUSCTL_NETWORK_CODE(KDDSL_GET_NAME);

	public static int CLUSCTL_NETWORK_GET_NETWORK = CLUSCTL_NETWORK_CODE(KDDSL_GET_NETWORK);

	public static int CLUSCTL_NETWORK_GET_NODE = CLUSCTL_NETWORK_CODE(KDDSL_GET_NODE);

	public static int CLUSCTL_NETWORK_GET_PRIVATE_PROPERTIES = CLUSCTL_NETWORK_CODE(KDDSL_GET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NETWORK_GET_PRIVATE_PROPERTY_FMTS = CLUSCTL_NETWORK_CODE(KDDSL_GET_PRIVATE_PROPERTY_FMTS);

	public static int CLUSCTL_NETWORK_GET_RO_COMMON_PROPERTIES = CLUSCTL_NETWORK_CODE(KDDSL_GET_RO_COMMON_PROPERTIES);

	public static int CLUSCTL_NETWORK_GET_RO_PRIVATE_PROPERTIES = CLUSCTL_NETWORK_CODE(KDDSL_GET_RO_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NETWORK_SET_COMMON_PROPERTIES = CLUSCTL_NETWORK_CODE(KDDSL_SET_COMMON_PROPERTIES);

	public static int CLUSCTL_NETWORK_SET_PRIVATE_PROPERTIES = CLUSCTL_NETWORK_CODE(KDDSL_SET_PRIVATE_PROPERTIES);

	public static int CLUSCTL_NETWORK_UNKNOWN = CLUSCTL_NETWORK_CODE(KDDSL_UNKNOWN);

	public static int CLUSCTL_NETWORK_VALIDATE_COMMON_PROPERTIES = CLUSCTL_NETWORK_CODE(KDDSL_VALIDATE_COMMON_PROPERTIES);

	public static int CLUSCTL_NETWORK_VALIDATE_PRIVATE_PROPERTIES = CLUSCTL_NETWORK_CODE(KDDSL_VALIDATE_PRIVATE_PROPERTIES);

	public static int CLUSCTL_RESOURCE_TYPE_INSTALL_NODE = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_INSTALL_NODE);

	public static int CLUSCTL_RESOURCE_TYPE_EVICT_NODE = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_EVICT_NODE);

	public static int CLUSCTL_RESOURCE_TYPE_CLUSTER_VERSION_CHANGED = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_CLUSTER_VERSION_CHANGED);

	public static int CLUSCTL_RESOURCE_TYPE_FIXUP_ON_UPGRADE = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_FIXUP_ON_UPGRADE);

	public static int CLUSCTL_RESOURCE_TYPE_STARTING_PHASE1 = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_STARTING_PHASE1);

	public static int CLUSCTL_RESOURCE_TYPE_STARTING_PHASE2 = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_STARTING_PHASE2);

	public static int CLUSCTL_RESOURCE_TYPE_HOLD_IO = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_HOLD_IO);

	public static int CLUSCTL_RESOURCE_TYPE_RESUME_IO = CLUSCTL_RESOURCE_TYPE_CODE(KDDSL_RESUME_IO);

	public static int CLUSCTL_RESOURCE_VM_START_MIGRATION = 23068676;

	public static int CLUSCTL_RESOURCE_VM_CANCEL_MIGRATION = 23068680;

	public static int CLUSCTL_RESOURCE_VM_HALT_MIGRATION = 23068688;

	public static int CLUSCTL_RESOURCE_VM_ACCEPT_MIGRATION = 23068684;

	public static int CLUSCTL_RESOURCE_VM_CONFIG_UPDATE = 23068676;

	public static int CLUSCTL_RESOURCE_VM_CREATE_SWITCHPORTS = 23068680;

	public static int CLUSCTL_RESOURCE_VM_ENABLE_PORT_CREATION = 23068684;

	public static int CLUSCTL_RESOURCE_VM_DISABLE_PORT_CREATION = 23068688;

	public static int CLUSCTL_RESOURCE_VM_SET_NEXT_OFFLINE_ACTION = 23068692;

	public static uint VM_E_TASK_CANCELED = 2147753985u;

	internal const int STANDARD_RIGHTS_REQUIRED = 983040;

	internal const int SC_MANAGER_CONNECT = 1;

	internal const int SC_MANAGER_CREATE_SERVICE = 2;

	internal const int SC_MANAGER_ENUMERATE_SERVICE = 4;

	internal const int SC_MANAGER_LOCK = 8;

	internal const int SC_MANAGER_QUERY_LOCK_STATUS = 16;

	internal const int SC_MANAGER_MODIFY_BOOT_CONFIG = 32;

	internal const int SC_MANAGER_ALL_ACCESS = 983103;

	internal const int SERVICE_QUERY_CONFIG = 1;

	internal const int SERVICE_CHANGE_CONFIG = 2;

	internal const int SERVICE_QUERY_STATUS = 4;

	internal const int SERVICE_ENUMERATE_DEPENDENTS = 8;

	internal const int SERVICE_START = 16;

	internal const int SERVICE_STOP = 32;

	internal const int SERVICE_PAUSE_CONTINUE = 64;

	internal const int SERVICE_INTERROGATE = 128;

	internal const int SERVICE_USER_DEFINED_CONTROL = 256;

	internal const int SERVICE_ALL_ACCESS = 983551;

	internal const int SC_STATUS_PROCESS_INFO = 0;

	internal const int SERVICE_BOOT_START = 0;

	internal const int SERVICE_SYSTEM_START = 1;

	internal const int SERVICE_AUTO_START = 2;

	internal const int SERVICE_DEMAND_START = 3;

	internal const int SERVICE_DISABLED = 4;

	internal const int SERVICE_CONFIG_DESCRIPTION = 1;

	internal const int SERVICE_CONFIG_FAILURE_ACTIONS = 2;

	internal const int SERVICE_CONFIG_DELAYED_AUTO_START_INFO = 3;

	internal const int SERVICE_CONFIG_FAILURE_ACTIONS_FLAG = 4;

	internal const int SERVICE_CONFIG_SERVICE_SID_INFO = 5;

	internal const int SERVICE_CONFIG_REQUIRED_PRIVILEGES_INFO = 6;

	internal const int SERVICE_CONFIG_PRESHUTDOWN_INFO = 7;

	internal const int SERVICE_CONFIG_TRIGGER_INFO = 8;

	internal const int SERVICE_CONFIG_PREFERRED_NODE = 9;

	internal const int SERVICE_CONFIG_RUNLEVEL_INFO = 10;

	internal const int ERROR_FILE_NOT_FOUND = 2;

	internal const int ERROR_INSUFFICIENT_BUFFER = 122;

	internal const int ERROR_ALREADY_EXISTS = 183;

	internal const int ERROR_SERVICE_DOES_NOT_EXIST = 1060;

	internal const int ERROR_INVALID_PARAMETER = 87;

	internal const int ERROR_CLUSTER_INVALID_REQUEST = 5048;

	internal const string AppHealthMonitorProgId = "HyperV.AppHealthMonitor";

	private const int DriveNameSize = 256;

	private const int EnclosureNameSize = 1024;

	private const int PoolDriveInfoSize = 2608;

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterHandle OpenCluster(string clusterName);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterHandle OpenClusterEx(string clusterName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, EntryPoint = "OpenClusterEx", ExactSpelling = true, SetLastError = true)]
	internal static extern IntPtr OpenClusterEx2(string clusterName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool CloseCluster(IntPtr cluster);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterEnumHandle ClusterOpenEnumEx(SafeClusterHandle clusterHandle, ClusterEnumType type, IntPtr options);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterCloseEnumEx(IntPtr enumHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterGetEnumCountEx(IntPtr enumHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterGetEnumCountEx(SafeClusterEnumHandle enumHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterEnumEx(IntPtr enumHandle, int index, IntPtr enumItem, ref int enumItemSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterEnumEx(SafeClusterEnumHandle enumHandle, int index, IntPtr enumItem, ref int enumItemSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterControl(SafeClusterHandle clusterHandle, SafeClusterNodeHandle nodeHandle, int controlCode, IntPtr inBuffer, int inBufferSize, IntPtr outBuffer, int outBufferSize, ref int bytesReturned);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern IntPtr CreateClusterNotifyPort(IntPtr notificationHandle, SafeClusterHandle clusterHandle, CLUSTER_CHANGE filterType, IntPtr notifyKey);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterNotifyPortHandle CreateClusterNotifyPortV2(IntPtr notificationHandle, IntPtr clusterHandle, ref NOTIFY_FILTER_AND_TYPE filterType, int filterSize, IntPtr notifyKey);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterNotifyPortHandle CreateClusterNotifyPortV2(IntPtr notificationHandle, SafeClusterHandle clusterHandle, ref NOTIFY_FILTER_AND_TYPE filterType, int filterSize, IntPtr notifyKey);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int RegisterClusterNotify(IntPtr notificationHandle, CLUSTER_CHANGE filterType, IntPtr objectHandle, IntPtr notifyKey);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int RegisterClusterNotifyV2(IntPtr notificationHandle, NOTIFY_FILTER_AND_TYPE filterAndType, IntPtr objectHandle, IntPtr notifyKey);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool CloseClusterNotifyPort(IntPtr notificationHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetClusterNotify(IntPtr notificationHandle, out int notifyKey, out CLUSTER_CHANGE filterType, StringBuilder objectName, ref int objectNameSize, int timeOut);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetClusterInformation(SafeClusterHandle cluster, StringBuilder clusterName, ref int clusterNameSize, ref CLUSTERVERSIONINFO clusterInfo);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetClusterNotifyV2(SafeClusterNotifyPortHandle notificationHandle, out int notifyKey, ref NOTIFY_FILTER_AND_TYPE filterAndType, IntPtr buffer, ref int bufferSize, StringBuilder objectId, ref int objectIdSize, StringBuilder parentId, ref int parentIdSize, StringBuilder objectName, ref int objectNameSize, StringBuilder objectType, ref int objectTypeSize, int timeOut);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterGroupEnumHandleEx ClusterGroupOpenEnumEx(SafeClusterHandle clusterHandle, IntPtr lpszProperties, int cbProperties, IntPtr lpszRoProperties, int cbRoProperties, int flags);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterGroupGetEnumCountEx(SafeClusterGroupEnumHandleEx groupBulkLoadHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterGroupEnumEx(SafeClusterGroupEnumHandleEx groupBulkLoadHandle, int index, IntPtr enumItem, ref int enumItemSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterGroupCloseEnumEx(IntPtr groupBulkLoadHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterGroupEnumHandle ClusterGroupOpenEnum(SafeClusterGroupHandle groupHandle, GroupEnumType type);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterGroupCloseEnum(IntPtr enumHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterGroupGetEnumCount(SafeClusterGroupEnumHandle enumHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterGroupEnum(SafeClusterGroupEnumHandle enumHandle, int index, ref GroupEnumType enumType, StringBuilder resourceName, ref int resourceNameSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterGroupHandle OpenClusterGroupEx(SafeClusterHandle clusterHandle, string groupName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool CloseClusterGroup(IntPtr groupHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int MoveClusterGroupEx(SafeClusterGroupHandle groupHandle, SafeClusterNodeHandle nodeHandle, int moveFlags, IntPtr inBuffer, int inBufferSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterKeyHandle GetClusterGroupKey(SafeClusterGroupHandle groupHandle, RegistryRights rights);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterRegQueryValue(SafeClusterKeyHandle key, string valueName, IntPtr valueType, out int data, ref int dataSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterRegQueryValue(SafeClusterKeyHandle key, string valueName, IntPtr valueType, StringBuilder data, ref int dataSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterRegCloseKey(IntPtr key);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetClusterGroupState(SafeClusterGroupHandle groupHandle, StringBuilder nodeName, ref int nodeNameSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterGroupControl(SafeClusterGroupHandle groupHandle, SafeClusterNodeHandle hostNodeHandle, int controlCode, IntPtr inBuffer, int inBufferSize, IntPtr outBuffer, int outBufferSize, ref int bytesReturned);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int DeleteClusterGroup(SafeClusterGroupHandle groupHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int DestroyClusterGroup(SafeClusterGroupHandle groupHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterGroupHandle CreateClusterGroupEx(SafeClusterHandle clusterHandle, string groupName, ref CLUSTER_CREATE_GROUP_INFO groupInfo);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int DestroyCluster(SafeClusterHandle clusterHandle, ClusterSetupProgressCallback callback, IntPtr callbackArgs, bool deleteVirtualComputerObjects);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int SetClusterGroupName(SafeClusterGroupHandle groupHandle, string groupName);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int OnlineClusterGroup(SafeClusterGroupHandle groupHandle, IntPtr nodeHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int OnlineClusterGroupEx(SafeClusterGroupHandle groupHandle, IntPtr nodeHandle, uint flags, IntPtr data, int dataSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int OfflineClusterGroup(SafeClusterGroupHandle groupHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int OfflineClusterGroupEx(SafeClusterGroupHandle groupHandle, uint flags, IntPtr data, int dataSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int CancelClusterGroupOperation(SafeClusterGroupHandle groupHandle, int cancelFlags);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int MoveClusterGroup(SafeClusterGroupHandle groupHandle, SafeClusterNodeHandle nodeHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int MoveClusterGroupEx(SafeClusterGroupHandle groupHandle, SafeClusterNodeHandle nodeHandle, uint flags, IntPtr data, int dataSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int SetClusterGroupNodeList(SafeClusterGroupHandle groupHandle, int nodeCount, IntPtr[] nodes);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterNodeHandle OpenClusterNodeEx(SafeClusterHandle clusterHandle, string nodeName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool CloseClusterNode(IntPtr nodeHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterNodeControl(SafeClusterNodeHandle nodeHandle, SafeClusterNodeHandle hostNodeHandle, int controlCode, IntPtr inBuffer, int inBufferSize, IntPtr outBuffer, int outBufferSize, ref int bytesReturned);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetClusterNodeState(SafeClusterNodeHandle nodeHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterNodeHandle AddClusterNode(SafeClusterHandle clusterHandle, string nodeName, ProgressCallback progressCallback, ref IntPtr callbackArg);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern void AddClusterStorageEnclosure(SafeClusterHandle clusterHandle, string nodeName, ProgressCallback progressCallback, ref IntPtr callbackArg);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern void RemoveClusterStorageEnclosure(SafeClusterHandle clusterHandle, string nodeName, uint timeout, uint flags);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int EvictClusterNodeEx(SafeClusterNodeHandle nodeHandle, uint timeout, out int cleanupStatus);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int PauseClusterNodeEx(SafeClusterNodeHandle nodeHandle, bool drain, NodePauseDrainFlag flags, IntPtr targetNodeHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ResumeClusterNodeEx(SafeClusterNodeHandle nodeHandle, int failbackType, uint flags);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterResourceEnumExHandle ClusterResourceOpenEnumEx(IntPtr clusterHandle, IntPtr lpszProperties, int cbProperties, IntPtr lpszRoProperties, int cbRoProperties, int flags);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterResourceEnumHandle ClusterResourceOpenEnum(SafeClusterResourceHandle resourceHandle, ResourceEnumType type);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterResourceGetEnumCountEx(SafeClusterResourceEnumExHandle resourceBulkLoadHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterResourceEnumEx(SafeClusterResourceEnumExHandle resourceBulkLoadHandle, int index, IntPtr enumItem, ref int enumItemSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterResourceCloseEnumEx(IntPtr resourceBulkLoadHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int RemoveClusterResourceDependency(SafeClusterResourceHandle resourceHandle, SafeClusterResourceHandle dependsOnResourceHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterResourceCloseEnum(IntPtr resourceHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterResourceGetEnumCount(SafeClusterResourceEnumHandle enumHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterResourceEnum(SafeClusterResourceEnumHandle enumHandle, int index, ref ResourceEnumType enumType, StringBuilder resourceName, ref int resourceNameSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterResourceControl(SafeClusterResourceHandle resourceHandle, SafeClusterNodeHandle hostNodeHandle, int controlCode, IntPtr inBuffer, int inBufferSize, IntPtr outBuffer, int outBufferSize, ref int bytesReturned);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterResourceHandle OpenClusterResourceEx(SafeClusterHandle clusterHandle, string resourceName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool CloseClusterResource(IntPtr resourceHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int SetClusterResourceName(SafeClusterResourceHandle resourceHandle, string groupName);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetClusterResourceState(SafeClusterResourceHandle resourceHandle, StringBuilder nodeName, ref int nodeNameSize, StringBuilder groupName, ref int groupNameSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int DeleteClusterResource(SafeClusterResourceHandle resourceHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int OnlineClusterResource(SafeClusterResourceHandle resourceHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int OnlineClusterResourceEx(SafeClusterResourceHandle resourceHandle, uint flags, IntPtr data, int dataSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int OfflineClusterResource(SafeClusterResourceHandle resourceHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int OfflineClusterResourceEx(SafeClusterResourceHandle resourceHandle, uint flags, IntPtr data, int dataSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int FailClusterResource(SafeClusterResourceHandle resourceHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterResourceHandle CreateClusterResource(SafeClusterGroupHandle groupHandle, string resourceName, string resourceType, ClusterResourceCreateFlags flags);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int AddClusterResourceNode(SafeClusterResourceHandle resourceHandle, SafeClusterNodeHandle nodeHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int RemoveClusterResourceNode(SafeClusterResourceHandle resourceHandle, SafeClusterNodeHandle nodeHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterResourceTypeEnumHandle ClusterResourceTypeOpenEnum(SafeClusterHandle clusterHandle, string resourceTypeName, ResourceTypeEnumType type);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterResourceTypeCloseEnum(IntPtr enumHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterResourceTypeEnum(SafeClusterResourceTypeEnumHandle enumHandle, int index, ref ResourceTypeEnumType enumType, StringBuilder resourceTypeName, ref int resourceTypeNameSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterResourceTypeGetEnumCount(SafeClusterResourceTypeEnumHandle enumHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int CreateClusterResourceType(IntPtr clusterHandle, string resourceTypeName, string displayName, string resourceTypeDll, int looksAlivePollInterval, int isAlivePollInterval);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int DeleteClusterResourceType(IntPtr clusterHandle, string resourceTypeName);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterResourceTypeControl(SafeClusterHandle clusterHandle, string resourceTypeName, SafeClusterNodeHandle hostNodeHandle, int controlCode, IntPtr inBuffer, int inBufferSize, IntPtr outBuffer, int outBufferSize, ref int bytesReturned);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int SetClusterResourceDependencyExpression(SafeClusterResourceHandle resourceHandle, string dependencyExpression);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetClusterResourceDependencyExpression(SafeClusterResourceHandle resourceHandle, StringBuilder dependencyExpression, ref int dependencyExpressionSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterNetworkHandle OpenClusterNetworkEx(SafeClusterHandle clusterHandle, string networkName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern bool CloseClusterNetwork(IntPtr networkHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterNetworkControl(SafeClusterNetworkHandle networkHandle, SafeClusterNodeHandle hostNodeHandle, int controlCode, IntPtr inBuffer, int inBufferSize, IntPtr outBuffer, int outBufferSize, ref int bytesReturned);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterNetworkEnumHandle ClusterNetworkOpenEnum(SafeClusterNetworkHandle networkHandle, uint type);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterNetworkCloseEnum(IntPtr enumHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterNetworkGetEnumCount(SafeClusterNetworkEnumHandle enumHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterNetworkEnum(SafeClusterNetworkEnumHandle enumHandle, int index, ref uint enumType, StringBuilder networkName, ref int networkNameSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetClusterNetworkState(SafeClusterNetworkHandle networkHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int SetClusterNetworkName(SafeClusterNetworkHandle networkHandle, string newNetworkName);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterNetworkInterfaceHandle OpenClusterNetInterfaceEx(SafeClusterHandle clusterHandle, string networkInterfaceName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern bool CloseClusterNetInterface(IntPtr networkInterfaceHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterNetInterfaceControl(SafeClusterNetworkInterfaceHandle networkInterfacehandle, SafeClusterNodeHandle hostNodeHandle, int controlCode, IntPtr inBuffer, int inBufferSize, IntPtr outBuffer, int outBufferSize, ref int bytesReturned);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetClusterNetInterfaceState(SafeClusterNetworkInterfaceHandle networkInterfaceHandle);

	[DllImport("ResUtils.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ResUtilFindDwordProperty(IntPtr propertyList, int propertyListSize, string propertyName, ref int propertyValue);

	[DllImport("ResUtils.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ResUtilFindSzProperty(IntPtr propertyList, int propertyListSize, string propertyName, ref string propertyValue);

	[DllImport("ResUtils.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ResUtilFindMultiSzProperty(IntPtr propertyList, int propertyListSize, string propertyName, out IntPtr propertyValue, out int propertyValueSize);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterKeyHandle GetClusterKey(SafeClusterHandle clusterHandle, RegistryRights rights);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterRegCreateReadBatch(SafeClusterKeyHandle hKey, out SafeClusterKeyBatchReadHandle registryReadBatch);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterRegReadBatchAddCommand(SafeClusterKeyBatchReadHandle registryReadBatch, string subKeyName, string valueName);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterRegCloseReadBatch(IntPtr registryReadBatch, out SafeClusterKeyBatchReplyHandle registryBatchReply);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterRegReadBatchReplyNextCommand(SafeClusterKeyBatchReplyHandle registryBatchReply, IntPtr batchCommand);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterRegCloseReadBatchReply(IntPtr registryBatchReply);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int AddResourceToClusterSharedVolumes(SafeClusterResourceHandle resourceHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int RemoveResourceFromClusterSharedVolumes(SafeClusterResourceHandle resourceHandle);

	[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetClusterQuorumResource(SafeClusterHandle clusterHandle, [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder resourceName, [In][Out] ref int resourceNameSize, [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder deviceName, [In][Out] ref int deviceNameSize, out int quorumLogSize);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterPropertyListHandle CreatePropList(IntPtr propertyList, int propertyListSize);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int DestroyPropList(IntPtr propertyList);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int AddWordProperty(SafeClusterPropertyListHandle clusPropListHandle, string propertyName, ushort propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int AddDwordProperty(SafeClusterPropertyListHandle clusPropListHandle, string propertyName, uint propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int AddStringProperty(SafeClusterPropertyListHandle clusPropListHandle, string propertyName, string propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int AddLongProperty(SafeClusterPropertyListHandle clusPropListHandle, string propertyName, int propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int AddLong64Property(SafeClusterPropertyListHandle clusPropListHandle, string propertyName, long propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int AddULong64Property(SafeClusterPropertyListHandle clusPropListHandle, string propertyName, ulong propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int AddFiletimeProperty(SafeClusterPropertyListHandle clusPropListHandle, string propertyName, ref System.Runtime.InteropServices.ComTypes.FILETIME propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int AddBinaryProperty(SafeClusterPropertyListHandle clusPropListHandle, string propertyName, byte[] data, int dataSize);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int AddExpandSzProperty(SafeClusterPropertyListHandle clusPropListHandle, string propertyName, string propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int AddMultiSzProperty(SafeClusterPropertyListHandle clusPropListHandle, string propertyName, byte[] propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int Reset(SafeClusterPropertyListHandle clusPropListHandle);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetNextProperty(SafeClusterPropertyListHandle clusPropListHandle, ref ClusterPropertyType format, StringBuilder propertyName, ref int propertyNameSize);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetWordProperty(SafeClusterPropertyListHandle clusPropListHandle, out ushort propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetDwordProperty(SafeClusterPropertyListHandle clusPropListHandle, out uint propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetLongProperty(SafeClusterPropertyListHandle clusPropListHandle, out int propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetLong64Property(SafeClusterPropertyListHandle clusPropListHandle, out long propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetBinaryProperty(SafeClusterPropertyListHandle clusPropListHandle, byte[] propertyValue, ref int propertyValueSize);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetFiletimeProperty(SafeClusterPropertyListHandle clusPropListHandle, ref System.Runtime.InteropServices.ComTypes.FILETIME propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetULong64Property(SafeClusterPropertyListHandle clusPropListHandle, out ulong propertyValue);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetStringProperty(SafeClusterPropertyListHandle clusPropListHandle, StringBuilder propertyValue, ref int propertyValueSize);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetMultiSzProperty(SafeClusterPropertyListHandle clusPropListHandle, byte[] propertyValue, ref int propertyValueSize);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetMultiSzProperty(SafeClusterPropertyListHandle clusPropListHandle, char[] propertyValue, ref int propertyValueSize);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetPropertyListBuffer(SafeClusterPropertyListHandle clusPropListHandle, out IntPtr propertyListBuffer, out int propertyListSize);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int SetPropertyToDefaultValue(SafeClusterPropertyListHandle clusPropListHandle, string propertyName, ClusterPropertyType format);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern SafeClusterValueListHandle CreateValueList(IntPtr valueList, int valueListSize);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int DestroyValueList(IntPtr valueListHandle);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ResetValueList(SafeClusterValueListHandle valueListHandle);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetNextValue(SafeClusterValueListHandle valueListHandle, ref CLUSPROP_SYNTAX syntax, IntPtr buffer, ref int bufferSize);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetClusterConnectionInformation(SafeClusterHandle clusterHandle, ClusterConnectionInformationClass code, StringBuilder information, ref int informationLength);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int GetClusterConnectionInformation(SafeClusterHandle clusterHandle, ClusterConnectionInformationClass code, byte[] information, ref int informationLength);

	[DllImport("FailoverClusters.FrameworkSupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ResetCnoPassword(SafeClusterResourceHandle resourceHandle, string dnsName);

	[DllImport("comctl32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern uint TaskDialogIndirect([In] TASKDIALOGCONFIG pTaskConfig, out int pnButton, out int pnRadioButton, [MarshalAs(UnmanagedType.Bool)] out bool pVerificationFlagChecked);

	[DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int StrCmpLogicalW(string x, string y);

	[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

	[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

	[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
	public static extern bool DeleteDC(IntPtr hDC);

	[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
	public static extern bool DeleteObject(IntPtr hObject);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr GetDC(IntPtr hWnd);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

	[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool IsWindowEnabled(IntPtr hwnd);

	[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool IsWindowVisible(IntPtr hwnd);

	[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true)]
	internal static extern IntPtr GetWindow(IntPtr hwnd, GetWindow_Cmd flag);

	[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool AttachThreadInput(int idAttach, int idAttachTo, int fAttach);

	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	internal static extern IntPtr GetForegroundWindow();

	[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool SetForegroundWindow(IntPtr hwnd);

	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	internal static extern IntPtr SetActiveWindow(IntPtr hWnd);

	[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true)]
	internal static extern int GetWindowThreadProcessId(IntPtr hwnd, ref int lpdwProcessId);

	[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true)]
	internal static extern bool AllowSetForegroundWindow(int dwProcessId);

	[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true)]
	public static extern bool FreeLibrary(IntPtr module);

	[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string fileName);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetSystemDirectoryW", SetLastError = true)]
	public static extern int GetSystemDirectory(StringBuilder buffer, int size);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW", SetLastError = true)]
	public static extern IntPtr GetModuleHandle(string moduleName);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "FormatMessageW", SetLastError = true, ThrowOnUnmappableChar = true)]
	public static extern int FormatMessage(FormatMessageFlags flags, IntPtr source, int messageId, uint languageId, [Out] StringBuilder buffer, int size, IntPtr arguments);

	[DllImport("KERNEL32.DLL")]
	public unsafe static extern void CopyMemory(void* dest, void* src, int length);

	[DllImport("Kernel32.dll")]
	internal static extern IntPtr CreateActCtx(ref ACTCTX actctx);

	[DllImport("Kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool ActivateActCtx(IntPtr hActCtx, out IntPtr lpCookie);

	[DllImport("Kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool DeactivateActCtx(uint dwFlags, IntPtr lpCookie);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern IntPtr LocalFree(IntPtr hMem);

	[DllImport("Kernel32.dll", EntryPoint = "RtlZeroMemory")]
	internal static extern void ZeroMemory(IntPtr dest, int size);

	[DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
	internal static extern int NetShareEnum(string serverName, int level, ref IntPtr bufPtr, int prefmaxlen, out int entriesread, out int totalentries, IntPtr resume_handle);

	[DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
	internal static extern int NetApiBufferFree(IntPtr buffer);

	[DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
	internal static extern int NetShareDel(string servername, string netname, int reserved);

	[DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
	internal static extern int NetShareGetInfo(string servername, string netname, int level, ref IntPtr bufptr);

	[DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
	internal static extern int NetShareSetInfo(string servername, string netname, int level, IntPtr bufPtr, out int paramError);

	internal static int HRESULT_FROM_WIN32(int x)
	{
		if (x > 0)
		{
			return (x & 0xFFFF) | (int)(FACILITY_WIN32 << 16) | int.MinValue;
		}
		return x;
	}

	internal static int HRESULT_FROM_WIN32(ErrorCode x)
	{
		if (x > ErrorCode.None)
		{
			return (int)(x & (ErrorCode)65535) | (int)(FACILITY_WIN32 << 16) | int.MinValue;
		}
		return (int)x;
	}

	internal static int HRESULT_FROM_RPC(ErrorCode x)
	{
		if (x > ErrorCode.None)
		{
			return (int)(x & (ErrorCode)65535) | (int)(FACILITY_RPC << 16) | int.MinValue;
		}
		return (int)x;
	}

	public static int ToInt(this ErrorCode error)
	{
		return (int)error;
	}

	public static bool IsEqual(this ErrorCode error, int scCode)
	{
		if (scCode != (int)error && scCode != HRESULT_FROM_WIN32(error))
		{
			return scCode == HRESULT_FROM_RPC(error);
		}
		return true;
	}

	internal static IntPtr Free(IntPtr allocatedMemory)
	{
		if (allocatedMemory == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		Marshal.FreeHGlobal(allocatedMemory);
		return IntPtr.Zero;
	}

	internal static IntPtr Alloc(int bytes)
	{
		return Marshal.AllocHGlobal(bytes);
	}

	internal static IntPtr ReAlloc(IntPtr buffer, int bytes)
	{
		return Marshal.ReAllocHGlobal(buffer, (IntPtr)bytes);
	}

	public static int CLUSCTL_RESOURCE_CODE(int controlCode)
	{
		return (1 << CLUSCTL_OBJECT_SHIFT) | controlCode;
	}

	public static int CLUSCTL_RESOURCE_TYPE_CODE(int controlCode)
	{
		return (2 << CLUSCTL_OBJECT_SHIFT) | controlCode;
	}

	public static int CLUSCTL_GROUP_CODE(int controlCode)
	{
		return (3 << CLUSCTL_OBJECT_SHIFT) | controlCode;
	}

	public static int CLUSCTL_NODE_CODE(int controlCode)
	{
		return (4 << CLUSCTL_OBJECT_SHIFT) | controlCode;
	}

	public static int CLUSCTL_NETWORK_CODE(int controlCode)
	{
		return (5 << CLUSCTL_OBJECT_SHIFT) | controlCode;
	}

	public static int CLUSCTL_NETINTERFACE_CODE(int controlCode)
	{
		return (6 << CLUSCTL_OBJECT_SHIFT) | controlCode;
	}

	public static int CLUSCTL_CLUSTER_CODE(int controlCode)
	{
		return (7 << CLUSCTL_OBJECT_SHIFT) | controlCode;
	}

	public static int KDDSL_EXTERNAL_CODE(int controlCode, int access, int modify)
	{
		return (access << CLUSCTL_ACCESS_SHIFT) | (KDDSL_CLUSTER_BASE + controlCode << CLUSCTL_FUNCTION_SHIFT) | (modify << KDDSL_MODIFY_SHIFT);
	}

	public static int KDDSL_INTERNAL_CODE(int controlCode, int access, int modify)
	{
		return (access << CLUSCTL_ACCESS_SHIFT) | KDDSL_INTERNAL_MASK | (KDDSL_CLUSTER_BASE + controlCode << CLUSCTL_FUNCTION_SHIFT) | (modify << KDDSL_MODIFY_SHIFT);
	}

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern SafeServiceHandle OpenSCManager(string lpMachineName, string lpDatabaseName, int dwDesiredAccess);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern SafeServiceHandle OpenService(SafeServiceHandle hService, string lpServiceName, int dwDesiredAccess);

	[DllImport("advapi32.dll", SetLastError = true)]
	internal static extern IntPtr LockServiceDatabase(IntPtr hSCManager);

	[DllImport("advapi32.dll", SetLastError = true)]
	internal static extern bool CloseServiceHandle(IntPtr hSCObject);

	[DllImport("advapi32.dll", SetLastError = true)]
	internal static extern bool UnlockServiceDatabase(IntPtr hSCManager);

	[DllImport("advapi32.dll", SetLastError = true)]
	internal static extern bool QueryServiceConfig(IntPtr hSCManager, IntPtr buffer, int cbBufSize, ref int pcbBytesNeeded);

	[DllImport("advapi32.dll", SetLastError = true)]
	internal static extern bool ChangeServiceConfig2(SafeServiceHandle hSCManager, int dwInfoLevel, IntPtr buffer);

	[DllImport("advapi32.dll", SetLastError = true)]
	internal static extern bool QueryServiceConfig2(SafeServiceHandle hSCManager, int dwInfoLevel, IntPtr buffer, int cbBufSize, ref int pcbBytesNeeded);

	[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "memcmp")]
	internal static extern int MemoryCompare(byte[] b1, byte[] b2, UIntPtr count);
}


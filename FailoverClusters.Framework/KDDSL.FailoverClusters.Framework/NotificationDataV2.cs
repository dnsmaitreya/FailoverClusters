using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal class NotificationDataV2
{
	private enum CLUSTER_RESOURCE_STATE
	{
		ClusterResourceStateUnknown = -1,
		ClusterResourceInherited = 0,
		ClusterResourceInitializing = 1,
		ClusterResourceOnline = 2,
		ClusterResourceOffline = 3,
		ClusterResourceFailed = 4,
		ClusterResourcePending = 128,
		ClusterResourceOnlinePending = 129,
		ClusterResourceOfflinePending = 130
	}

	private enum CLUSTER_GROUP_STATE
	{
		ClusterGroupStateUnknown = -1,
		ClusterGroupOnline,
		ClusterGroupOffline,
		ClusterGroupFailed,
		ClusterGroupPartialOnline,
		ClusterGroupPending
	}

	private const int DefaultBufferSize = 400;

	private const int DefaultStringSize = 300;

	private const char Delimiter = '\t';

	private NativeMethods.NOTIFY_FILTER_AND_TYPE filterAndType;

	private IntPtr bufferBuilder = NativeMethods.Alloc(400);

	private int bufferSize = 400;

	private int bufferRealSize = 400;

	private StringBuilder objectIdBuilder = new StringBuilder(299);

	private int objectIdSize = 300;

	private StringBuilder parentIdBuilder = new StringBuilder(299);

	private int parentIdSize = 300;

	private StringBuilder nameBuilder = new StringBuilder(299);

	private int nameSize = 300;

	private StringBuilder typeBuilder = new StringBuilder(299);

	private int typeSize = 300;

	public DateTime Timestamp { get; set; }

	public string ObjectType => filterAndType.ObjectType.ToString();

	public ulong FilterFlags => filterAndType.FilterFlags;

	public IntPtr BufferBuilder => bufferBuilder;

	public byte[] Buffer
	{
		get
		{
			byte[] array = new byte[bufferSize];
			Marshal.Copy(bufferBuilder, array, 0, bufferSize);
			return array;
		}
	}

	public string BufferAsString
	{
		get
		{
			if (BufferSize == 0)
			{
				return string.Empty;
			}
			if (BufferSize != 4)
			{
				string text = Encoding.Unicode.GetString(Buffer);
				ClusterIdentityType identityType = ClusterIdentityType.Cluster;
				bool flag = false;
				if (filterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_CLUSTER)
				{
					if (filterAndType.FilterFlags == 128 || filterAndType.FilterFlags == 256)
					{
						identityType = ClusterIdentityType.Cluster;
						flag = true;
					}
				}
				else if (filterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_RESOURCE)
				{
					if (filterAndType.FilterFlags == 1 || filterAndType.FilterFlags == 2)
					{
						identityType = ClusterIdentityType.Cluster;
						flag = true;
					}
				}
				else if (filterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_RESOURCE_TYPE)
				{
					if (filterAndType.FilterFlags == 2 || filterAndType.FilterFlags == 4)
					{
						identityType = ClusterIdentityType.ResourceType;
						flag = true;
					}
				}
				else if (filterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_GROUP)
				{
					if (filterAndType.FilterFlags == 2 || filterAndType.FilterFlags == 4)
					{
						identityType = ClusterIdentityType.Group;
						flag = true;
					}
				}
				else if (filterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NETWORK_INTERFACE)
				{
					if (filterAndType.FilterFlags == 2 || filterAndType.FilterFlags == 4)
					{
						identityType = ClusterIdentityType.NetworkInterface;
						flag = true;
					}
				}
				else if (filterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NETWORK)
				{
					if (filterAndType.FilterFlags == 2 || filterAndType.FilterFlags == 4)
					{
						identityType = ClusterIdentityType.Network;
						flag = true;
					}
				}
				else if (filterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NODE)
				{
					if (filterAndType.FilterFlags == 4 || filterAndType.FilterFlags == 8)
					{
						identityType = ClusterIdentityType.Node;
						flag = true;
					}
				}
				else if (filterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_QUORUM)
				{
					if (filterAndType.FilterFlags == 1 || filterAndType.FilterFlags == 1)
					{
						identityType = ClusterIdentityType.Resource;
						flag = true;
					}
				}
				else if (filterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_SPACEPORT && filterAndType.FilterFlags == 1)
				{
					NativeMethods.CM_NOTIFY_EVENT_DATA cM_NOTIFY_EVENT_DATA = (NativeMethods.CM_NOTIFY_EVENT_DATA)Marshal.PtrToStructure(bufferBuilder, typeof(NativeMethods.CM_NOTIFY_EVENT_DATA));
					NativeMethods.SP_NOTIFICATION_INFO data = cM_NOTIFY_EVENT_DATA.Data;
					return "FilterType:{0}; EventGuid:{1}; PoolId:{2}; Id:{3}; Notification Type:{4}; flags:{5}".FormatInvariantCulture(cM_NOTIFY_EVENT_DATA.FilterType, cM_NOTIFY_EVENT_DATA.EventGuid, data.Id.PoolId, data.Id.Id, data.Type, data.Flags);
				}
				if (flag)
				{
					ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(Guid.Empty, Guid.Empty, identityType);
					ClusApiAdapter.AdapterBase.ParseProperties(clusterPropertyCollection, bufferBuilder, BufferSize, ClusterPropertyKind.Common, readOnly: false);
					text = string.Empty;
					foreach (ClusterProperty item in clusterPropertyCollection)
					{
						text = "{0}|{1}".FormatInvariantCulture(text, item.DisplayName);
						switch (item.PropertyType)
						{
						case ClusterPropertyType.UnsignedInt:
						case ClusterPropertyType.String:
						case ClusterPropertyType.UnsignedInt64:
						case ClusterPropertyType.Int:
						case ClusterPropertyType.ExpandedString:
						case ClusterPropertyType.Int64:
							text = "{0}:{1}".FormatInvariantCulture(text, item.Value.ToString());
							break;
						case ClusterPropertyType.StringCollection:
						{
							string text2 = string.Join(", ", (IEnumerable<string>)item.Value);
							text = "{0}:{1}".FormatInvariantCulture(text, text2);
							break;
						}
						default:
							text = "{0}:???".FormatInvariantCulture(text);
							break;
						}
					}
				}
				else if (filterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_CLUSTER && filterAndType.FilterFlags == 2048)
				{
					text = "Error Membership changed format";
					if (BufferSize >= 9)
					{
						bool flag2 = BitConverter.ToInt32(Buffer, 0) != 0;
						uint num = BitConverter.ToUInt32(Buffer, 4);
						if (BufferSize >= 8 + num)
						{
							text = "HasQuorum {0}: UpNodes: ".FormatInvariantCulture(flag2.ToString());
							List<string> list = new List<string>();
							for (int i = 0; i < num; i++)
							{
								uint num2 = Buffer[8 + i];
								if (num2 != 0)
								{
									list.Add(num2.ToString(CultureInfo.CurrentCulture));
								}
							}
							text = "{0}{1}".FormatInvariantCulture(text, string.Join(", ", list));
						}
					}
				}
				return text;
			}
			int value = BitConverter.ToInt32(Buffer, 0);
			if (filterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_RESOURCE && filterAndType.FilterFlags == 4 && BufferSize == 4)
			{
				return ((CLUSTER_RESOURCE_STATE)Enum.ToObject(typeof(CLUSTER_RESOURCE_STATE), value)).ToString();
			}
			if (filterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_GROUP && filterAndType.FilterFlags == 8 && BufferSize == 4)
			{
				return ((CLUSTER_GROUP_STATE)Enum.ToObject(typeof(CLUSTER_GROUP_STATE), value)).ToString();
			}
			return value.ToString(CultureInfo.CurrentCulture);
		}
	}

	public int BufferSize => bufferSize;

	public string ObjectId => objectIdBuilder.ToString();

	public string ParentId => parentIdBuilder.ToString();

	public string Name => nameBuilder.ToString();

	public int NameSize
	{
		get
		{
			return nameSize;
		}
		set
		{
			nameSize = value;
		}
	}

	public string Type
	{
		get
		{
			string text = typeBuilder.ToString();
			if (text.Trim() == string.Empty)
			{
				text = "<N/A>";
			}
			return text;
		}
	}

	internal NativeMethods.NOTIFY_FILTER_AND_TYPE FilterAndType => filterAndType;

	public NotificationDataV2()
	{
		Timestamp = DateTime.Now;
	}

	~NotificationDataV2()
	{
		if (bufferBuilder != IntPtr.Zero)
		{
			bufferBuilder = NativeMethods.Free(bufferBuilder);
		}
	}

	public void ResetSizes()
	{
		if (bufferSize > bufferRealSize)
		{
			if (bufferBuilder != IntPtr.Zero)
			{
				bufferBuilder = NativeMethods.Free(bufferBuilder);
			}
			bufferBuilder = NativeMethods.Alloc(bufferSize);
			bufferRealSize = bufferSize;
		}
		objectIdSize++;
		parentIdSize++;
		nameSize++;
		typeSize++;
		objectIdBuilder = ((objectIdSize > objectIdBuilder.Capacity) ? new StringBuilder(objectIdSize) : objectIdBuilder);
		parentIdBuilder = ((parentIdSize > parentIdBuilder.Capacity) ? new StringBuilder(parentIdSize) : parentIdBuilder);
		nameBuilder = ((nameSize > nameBuilder.Capacity) ? new StringBuilder(nameSize) : nameBuilder);
		typeBuilder = ((typeSize > typeBuilder.Capacity) ? new StringBuilder(typeSize) : typeBuilder);
	}

	public override string ToString()
	{
		string[] value = new string[9]
		{
			Timestamp.ToString("M/d/yyyy HH:mm:ss.FFFFFFF", CultureInfo.CurrentCulture),
			ObjectType,
			FilterFlags.ToString(CultureInfo.CurrentCulture),
			Name,
			ObjectId,
			ParentId,
			bufferSize.ToString(CultureInfo.CurrentCulture),
			Type,
			BufferAsString
		};
		return string.Join('\t'.ToString(), value);
	}

	internal int GetNext(SafeClusterNotifyPortHandle notificationPort, int p, out int notifyKey)
	{
		bufferSize = bufferRealSize;
		objectIdSize = objectIdBuilder.Capacity;
		parentIdSize = parentIdBuilder.Capacity;
		nameSize = nameBuilder.Capacity;
		typeSize = typeBuilder.Capacity;
		return NativeMethods.GetClusterNotifyV2(notificationPort, out notifyKey, ref filterAndType, bufferBuilder, ref bufferSize, objectIdBuilder, ref objectIdSize, parentIdBuilder, ref parentIdSize, nameBuilder, ref nameSize, typeBuilder, ref typeSize, 500);
	}

	internal bool IsClusterStateNotification()
	{
		NativeMethods.NOTIFY_FILTER_AND_TYPE nOTIFY_FILTER_AND_TYPE = filterAndType;
		if (nOTIFY_FILTER_AND_TYPE.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_CLUSTER)
		{
			return nOTIFY_FILTER_AND_TYPE.FilterFlags == 2;
		}
		return false;
	}

	internal int GetNextV1(IntPtr intPtr, int timeout, out int notificationNotifyKey)
	{
		NativeMethods.CLUSTER_CHANGE filterType = NativeMethods.CLUSTER_CHANGE.CLUSTER_CHANGE_ALL;
		int clusterNotify = NativeMethods.GetClusterNotify(intPtr, out notificationNotifyKey, out filterType, nameBuilder, ref nameSize, timeout);
		filterAndType.FilterFlags = (ulong)filterType;
		bufferSize = 0;
		return clusterNotify;
	}
}


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Common.Services;
using WindowsAPICodePack.Dialogs;

namespace KDDSL.FailoverClusters.Framework;

internal class ClusApiAdapter : RootAdapterBase, IConnectionAdapter, IDisposable
{
	internal class AdapterBase
	{
		protected ClusApiAdapter ClusApiAdapter { get; private set; }

		public AdapterBase(ClusApiAdapter clusApiAdapter)
		{
			ClusApiAdapter = clusApiAdapter;
		}

		public static void ParseProperties(ClusterPropertyCollection propertyCollection, IntPtr propertyList, int propertyListSize, ClusterPropertyKind propertiesKind, bool readOnly)
		{
			string resourceName = null;
			string currentResourceType = null;
			ParseProperties(propertyCollection, propertyList, propertyListSize, propertiesKind, readOnly, ref resourceName, ref currentResourceType);
		}

		public static void ParseProperties(ClusterPropertyCollection propertyCollection, IntPtr propertyList, int propertyListSize, ClusterPropertyKind propertiesKind, bool readOnly, ref string resourceName, ref string currentResourceType)
		{
			if (propertyListSize == 4)
			{
				switch (propertiesKind)
				{
				case ClusterPropertyKind.Common:
					propertyCollection.CommonPropertiesLoaded = true;
					break;
				case ClusterPropertyKind.Private:
					propertyCollection.PrivatePropertiesLoaded = true;
					break;
				}
				return;
			}
			if ((propertyList == IntPtr.Zero) ^ (propertyListSize == 0))
			{
				throw new ClusterPropertyListBufferException();
			}
			SafeClusterPropertyListHandle safeClusterPropertyListHandle = NativeMethods.CreatePropList(propertyList, propertyListSize);
			if (safeClusterPropertyListHandle.IsInvalid)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == NativeMethods.ErrorCode.NoMoreItems.ToInt())
				{
					switch (propertiesKind)
					{
					case ClusterPropertyKind.Common:
						propertyCollection.CommonPropertiesLoaded = true;
						break;
					case ClusterPropertyKind.Private:
						propertyCollection.PrivatePropertiesLoaded = true;
						break;
					}
					return;
				}
				throw new ClusterDefaultException(new Win32Exception(lastWin32Error));
			}
			ClusterPropertyType format = ClusterPropertyType.Unknown;
			int propertyNameSize = 100;
			StringBuilder stringBuilder = new StringBuilder(propertyNameSize);
			try
			{
				Utilities.UnreferencedParameter(NativeMethods.Reset(safeClusterPropertyListHandle));
				while (NativeMethods.GetNextProperty(safeClusterPropertyListHandle, ref format, stringBuilder, ref propertyNameSize) == NativeMethods.ErrorCode.None.ToInt())
				{
					string text = stringBuilder.ToString();
					ClusterProperty clusterProperty = null;
					switch (format)
					{
					case ClusterPropertyType.Binary:
					{
						int propertyValueSize2 = 200;
						byte[] array2 = new byte[propertyValueSize2];
						int binaryProperty = NativeMethods.GetBinaryProperty(safeClusterPropertyListHandle, array2, ref propertyValueSize2);
						if (binaryProperty == NativeMethods.ErrorCode.MoreData.ToInt())
						{
							array2 = new byte[propertyValueSize2];
							binaryProperty = NativeMethods.GetBinaryProperty(safeClusterPropertyListHandle, array2, ref propertyValueSize2);
						}
						if (!NativeMethods.ErrorCode.None.IsEqual(binaryProperty))
						{
							ClusterLog.LogException(new ClusterPropertyGetValueException(resourceName, text, currentResourceType, binaryProperty));
							continue;
						}
						clusterProperty = new ClusterPropertyBinary(text, null, propertiesKind, readOnly, array2);
						break;
					}
					case ClusterPropertyType.UnsignedInt:
					{
						uint propertyValue2;
						int dwordProperty = NativeMethods.GetDwordProperty(safeClusterPropertyListHandle, out propertyValue2);
						if (!NativeMethods.ErrorCode.None.IsEqual(dwordProperty))
						{
							ClusterLog.LogException(new ClusterPropertyGetValueException(resourceName, text, currentResourceType, dwordProperty));
							continue;
						}
						clusterProperty = new ClusterPropertyUInt(text, null, propertiesKind, readOnly, propertyValue2);
						break;
					}
					case ClusterPropertyType.String:
					case ClusterPropertyType.ExpandString:
					case ClusterPropertyType.ExpandedString:
					{
						int propertyValueSize3 = 200;
						StringBuilder stringBuilder2 = new StringBuilder(propertyValueSize3);
						int stringProperty = NativeMethods.GetStringProperty(safeClusterPropertyListHandle, stringBuilder2, ref propertyValueSize3);
						if (stringProperty == NativeMethods.ErrorCode.MoreData.ToInt())
						{
							propertyValueSize3++;
							stringBuilder2 = new StringBuilder(propertyValueSize3);
							stringProperty = NativeMethods.GetStringProperty(safeClusterPropertyListHandle, stringBuilder2, ref propertyValueSize3);
						}
						if (!NativeMethods.ErrorCode.None.IsEqual(stringProperty))
						{
							ClusterLog.LogException(new ClusterPropertyGetValueException(resourceName, text, currentResourceType, stringProperty));
							continue;
						}
						if (text.Equals("name", StringComparison.OrdinalIgnoreCase))
						{
							resourceName = stringBuilder2.ToString();
						}
						if (text.Equals("type", StringComparison.OrdinalIgnoreCase))
						{
							currentResourceType = stringBuilder2.ToString();
						}
						switch (format)
						{
						case ClusterPropertyType.String:
							clusterProperty = new ClusterPropertyString(text, null, propertiesKind, readOnly, stringBuilder2.ToString());
							break;
						case ClusterPropertyType.ExpandString:
							clusterProperty = new ClusterPropertyExpandString(text, null, propertiesKind, readOnly, stringBuilder2.ToString());
							break;
						case ClusterPropertyType.ExpandedString:
							clusterProperty = new ClusterPropertyExpandedString(text, null, propertiesKind, readOnly, stringBuilder2.ToString());
							break;
						}
						break;
					}
					case ClusterPropertyType.StringCollection:
					{
						int propertyValueSize = 200;
						char[] array = new char[propertyValueSize / 2];
						int multiSzProperty = NativeMethods.GetMultiSzProperty(safeClusterPropertyListHandle, array, ref propertyValueSize);
						if (multiSzProperty == NativeMethods.ErrorCode.MoreData.ToInt())
						{
							array = new char[propertyValueSize / 2];
							multiSzProperty = NativeMethods.GetMultiSzProperty(safeClusterPropertyListHandle, array, ref propertyValueSize);
						}
						if (!NativeMethods.ErrorCode.None.IsEqual(multiSzProperty))
						{
							ClusterLog.LogException(new ClusterPropertyGetValueException(resourceName, text, currentResourceType, multiSzProperty));
							continue;
						}
						IList<string> list = new List<string>();
						int num3 = 0;
						int num4 = propertyValueSize / 2;
						while (num3 < num4)
						{
							int i;
							for (i = num3; i < num4 && array[i] != 0; i++)
							{
							}
							if (i < num4)
							{
								if (i - num3 > 0)
								{
									list.Add(new string(array, num3, i - num3));
								}
							}
							else
							{
								list.Add(new string(array, num3, num4 - num3));
							}
							num3 = i + 1;
						}
						clusterProperty = new ClusterPropertyMultipleStrings(text, null, propertiesKind, readOnly, list.ToArray());
						break;
					}
					case ClusterPropertyType.UnsignedInt64:
					{
						ulong propertyValue5;
						int uLong64Property = NativeMethods.GetULong64Property(safeClusterPropertyListHandle, out propertyValue5);
						if (!NativeMethods.ErrorCode.None.IsEqual(uLong64Property))
						{
							ClusterLog.LogException(new ClusterPropertyGetValueException(resourceName, text, currentResourceType, uLong64Property));
							continue;
						}
						clusterProperty = new ClusterPropertyULong(text, null, propertiesKind, readOnly, propertyValue5);
						break;
					}
					case ClusterPropertyType.Int:
					{
						int propertyValue3;
						int longProperty = NativeMethods.GetLongProperty(safeClusterPropertyListHandle, out propertyValue3);
						if (!NativeMethods.ErrorCode.None.IsEqual(longProperty))
						{
							ClusterLog.LogException(new ClusterPropertyGetValueException(resourceName, text, currentResourceType, longProperty));
							continue;
						}
						clusterProperty = new ClusterPropertyInt(text, null, propertiesKind, readOnly, propertyValue3);
						break;
					}
					case ClusterPropertyType.Int64:
					{
						long propertyValue6;
						int long64Property = NativeMethods.GetLong64Property(safeClusterPropertyListHandle, out propertyValue6);
						if (!NativeMethods.ErrorCode.None.IsEqual(long64Property))
						{
							ClusterLog.LogException(new ClusterPropertyGetValueException(resourceName, text, currentResourceType, long64Property));
							continue;
						}
						clusterProperty = new ClusterPropertyLong(text, null, propertiesKind, readOnly, propertyValue6);
						break;
					}
					case ClusterPropertyType.UnsignedShort:
					{
						ushort propertyValue4;
						int wordProperty = NativeMethods.GetWordProperty(safeClusterPropertyListHandle, out propertyValue4);
						if (!NativeMethods.ErrorCode.None.IsEqual(wordProperty))
						{
							ClusterLog.LogException(new ClusterPropertyGetValueException(resourceName, text, currentResourceType, wordProperty));
							continue;
						}
						clusterProperty = new ClusterPropertyLong(text, null, propertiesKind, readOnly, propertyValue4);
						break;
					}
					case ClusterPropertyType.DateTime:
					{
						System.Runtime.InteropServices.ComTypes.FILETIME propertyValue = default(System.Runtime.InteropServices.ComTypes.FILETIME);
						int filetimeProperty = NativeMethods.GetFiletimeProperty(safeClusterPropertyListHandle, ref propertyValue);
						if (!NativeMethods.ErrorCode.None.IsEqual(filetimeProperty))
						{
							ClusterLog.LogException(new ClusterPropertyGetValueException(resourceName, text, currentResourceType, filetimeProperty));
							continue;
						}
						ulong num = (ulong)(uint)propertyValue.dwHighDateTime << 32;
						ulong num2 = (uint)propertyValue.dwLowDateTime;
						long fileTime = (long)(num | num2);
						DateTime value;
						try
						{
							value = DateTime.FromFileTime(fileTime);
						}
						catch (ArgumentOutOfRangeException)
						{
							value = DateTime.MaxValue;
						}
						clusterProperty = new ClusterPropertyDateTime(text, null, propertiesKind, readOnly, value);
						break;
					}
					default:
						ClusterLog.LogWarning("Property format not recognized: " + format);
						continue;
					}
					propertyNameSize = 100;
					if (clusterProperty != null)
					{
						propertyCollection.Add(clusterProperty);
					}
				}
			}
			catch (Exception exception)
			{
				ClusterLog.LogException(exception, "Error parsing the properties in ClusAPIAdapter");
				throw;
			}
			finally
			{
				safeClusterPropertyListHandle.Dispose();
			}
			switch (propertiesKind)
			{
			case ClusterPropertyKind.Common:
				propertyCollection.CommonPropertiesLoaded = true;
				break;
			case ClusterPropertyKind.Private:
				propertyCollection.PrivatePropertiesLoaded = true;
				break;
			}
		}

		protected List<string> ReadMultiSzProp(IntPtr buffer)
		{
			List<string> list = new List<string>();
			int num = Marshal.ReadInt32(buffer);
			if ((num & 0xFFFF) != 5)
			{
				ClusterLog.LogWarning("Unexpected property format parsing a MultiSz cluster property, it should be {0} but it is {1}".FormatCurrentCulture(5.ToString(CultureInfo.CurrentCulture), (num & 0xFFFF).ToString(CultureInfo.CurrentCulture)));
			}
			buffer += 4;
			int num2 = Marshal.ReadInt32(buffer);
			buffer += 4;
			while (num2 != 0)
			{
				string text = Marshal.PtrToStringUni(buffer);
				if (!string.IsNullOrWhiteSpace(text))
				{
					list.Add(text);
				}
				num2 -= (text.Length + 1) * 2;
				buffer += (text.Length + 1) * 2;
			}
			return list;
		}

		protected static void VerifyOperationOccuredWhileUnlocked(int errorCode, string objectName, Guid id)
		{
			bool flag = NativeMethods.ErrorCode.ResourceIsInMaintenanceMode.IsEqual(errorCode);
			bool flag2 = NativeMethods.ErrorCode.ResourceLocked.IsEqual(errorCode);
			if (flag || flag2)
			{
				if (string.IsNullOrWhiteSpace(objectName))
				{
					throw new ClusterResourceLockedException(id, flag);
				}
				throw new ClusterResourceLockedException(objectName, flag);
			}
		}
	}

	internal class ClusterAdapter : AdapterBase, IConnectionAdapterCluster, IDisposable, INotificationHandler
	{
		private readonly ClusApiAdapter clusApiAdapter;

		private readonly PCluster mCluster;

		private readonly List<Guid> coreGroups = new List<Guid>();

		private SafeClusterHandle clusterSafeHandle;

		private IClusterExecutor executor;

		private IResourceExecutor resourceExecutor;

		private const int QuorumLost = 1;

		internal PCluster Cluster => mCluster;

		internal SafeClusterHandle Handle => clusterSafeHandle;

		public IEnumerable<Guid> CoreGroups => coreGroups;

		private IClusterExecutor Executor => executor ?? (executor = ServiceContainer.Container.Resolve<IClusterExecutor>(Array.Empty<object>()));

		private IResourceExecutor ResourceExecutor => resourceExecutor ?? (resourceExecutor = ServiceContainer.Container.Resolve<IResourceExecutor>(Array.Empty<object>()));

		public ClusterAdapter(ClusApiAdapter clusApiAdapter, PCluster cluster)
			: base(clusApiAdapter)
		{
			mCluster = cluster;
			this.clusApiAdapter = clusApiAdapter;
		}

		public Guid Open(string clusterName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess)
		{
			SafeClusterHandle handle = Executor.OpenCluster(clusterName, desiredAccess, out grantedAccess);
			if (desiredAccess == ClusterAccessRights.GenericAll && grantedAccess != ClusterAccessRights.GenericAll)
			{
				throw new ClusterReadOnlyAccessException(clusterName);
			}
			return Open(handle);
		}

		public Guid Open(SafeClusterHandle handle)
		{
			Exceptions.ThrowIfNull(handle, "handle");
			clusterSafeHandle = handle;
			SafeClusterHandle handle2 = Handle;
			coreGroups.Add(GetIdFromCoreGroup(NativeMethods.CLUSREG_NAME_CLUS_AVAIL_STORAGE_GROUP));
			coreGroups.Add(GetIdFromCoreGroup(NativeMethods.CLUSREG_NAME_CLUS_CLUSTER_GROUP));
			mCluster.Id = GetIdFromCoreGroup("ClusterInstanceID");
			Executor.ExecuteOnCommonProperties(handle2, delegate(IntPtr propertyList, int propertyListSize)
			{
				AdapterBase.ParseProperties(mCluster.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
			});
			Executor.ExecuteOnCommonReadOnlyProperties(handle2, delegate(IntPtr propertyList, int propertyListSize)
			{
				AdapterBase.ParseProperties(mCluster.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
			});
			Executor.ExecuteOnPrivateProperties(handle2, delegate(IntPtr propertyList, int propertyListSize)
			{
				AdapterBase.ParseProperties(mCluster.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
			});
			mCluster.LoadedSelection |= 6;
			mCluster.FqdnName = GetFullyQualifiedDomainName();
			return mCluster.Id;
		}

		public string GetFullyQualifiedDomainName()
		{
			string fullyQualifiedDomainName = string.Empty;
			SafeClusterHandle handle = Handle;
			if (handle == null || handle.IsInvalid)
			{
				return fullyQualifiedDomainName;
			}
			Executor.ExecuteOnControlCode(handle, NativeMethods.CLUSCTL_CLUSTER_GET_FQDN, delegate(IntPtr buffer, int bufferSize)
			{
				fullyQualifiedDomainName = Marshal.PtrToStringUni(buffer);
			});
			return fullyQualifiedDomainName;
		}

		public void Dispose()
		{
			Close();
		}

		public void Close()
		{
			clusterSafeHandle = null;
		}

		public void Rename(string newName)
		{
		}

		public void Shutdown()
		{
			SafeClusterHandle handle = Handle;
			Executor.ExecuteOnControlCode(handle, NativeMethods.CLUSCTL_CLUSTER_SHUTDOWN, null);
		}

		public void Destroy(bool deletecComputerObjects)
		{
			SafeClusterHandle handle = Handle;
			Executor.DestroyCluster(handle, ClusterSetupProgressCallbackNative, deletecComputerObjects);
		}

		private bool ClusterSetupProgressCallbackNative(IntPtr callbackArgs, ClusterSetupPhrase setupPhrase, ClusterSetupPhraseType setupPhraseType, ClusterSetupPhraseSeverity setupPhraseSeverity, int percentComplete, string objectName, int status)
		{
			ClusterDestroyProgress clusterDestroyProgress = new ClusterDestroyProgress
			{
				SetupPhrase = setupPhrase,
				SetupPhraseType = setupPhraseType,
				SetupPhraseSeverity = setupPhraseSeverity,
				PercentComplete = percentComplete,
				ObjectName = objectName,
				Status = status
			};
			base.ClusApiAdapter.EnqueueNotification(new ClusterNotification(new ClusterDestroyClusterProgressEventArgs(clusterDestroyProgress, mCluster.Id, null)));
			return true;
		}

		public string GetConnectedToNode()
		{
			SafeClusterHandle handle = Handle;
			if (handle == null || handle.IsInvalid)
			{
				return string.Empty;
			}
			return Executor.GetConnectedNodeName(handle);
		}

		internal Guid GetIdFromCoreGroup(string groupName)
		{
			SafeClusterHandle handle = Handle;
			if (handle == null || handle.IsInvalid)
			{
				return Guid.Empty;
			}
			SafeClusterKeyHandle clusterKey = Executor.GetClusterKey(handle, RegistryRights.QueryValues);
			if (clusterKey.IsInvalid)
			{
				throw new ClusterObjectNotFoundException(groupName, Guid.Empty, typeof(SafeClusterKeyHandle));
			}
			string clusterRegistryValue;
			try
			{
				clusterRegistryValue = Executor.GetClusterRegistryValue(clusterKey, groupName);
			}
			finally
			{
				clusterKey.Dispose();
			}
			return new Guid(clusterRegistryValue);
		}

		public void Load(PCluster cluster, ClusterLoadSelection loadSelection)
		{
			try
			{
				SafeClusterHandle handle = Handle;
				if (handle == null || handle.IsInvalid)
				{
					return;
				}
				if ((loadSelection & ClusterLoadSelection.Basic) == ClusterLoadSelection.Basic && (cluster.LoadedSelection & 1) != 1)
				{
					cluster.ConnectedTo = GetConnectedToNode();
					cluster.LoadedSelection |= 1;
				}
				if ((loadSelection & ClusterLoadSelection.VersionInformation) == ClusterLoadSelection.VersionInformation && (cluster.LoadedSelection & 8) != 8)
				{
					cluster.LoadedSelection |= 8;
					LoadClusterVersionInfo(cluster);
				}
				if ((loadSelection & ClusterLoadSelection.QuorumConfiguration) == ClusterLoadSelection.QuorumConfiguration && (cluster.LoadedSelection & 0x1000) != 4096)
				{
					cluster.LoadedSelection |= 4096;
					cluster.QuorumConfiguration = GetQuorumConfiguration();
				}
				if ((loadSelection & ClusterLoadSelection.CommonProperties) == ClusterLoadSelection.CommonProperties && (cluster.LoadedSelection & 2) != 2)
				{
					cluster.LoadedSelection |= 2;
					Executor.ExecuteOnCommonProperties(handle, delegate(IntPtr propertyList, int propertyListSize)
					{
						AdapterBase.ParseProperties(mCluster.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
					});
					Executor.ExecuteOnCommonReadOnlyProperties(handle, delegate(IntPtr propertyList, int propertyListSize)
					{
						AdapterBase.ParseProperties(mCluster.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
					});
				}
				if ((loadSelection & ClusterLoadSelection.PrivateProperties) == ClusterLoadSelection.PrivateProperties && (cluster.LoadedSelection & 4) != 4)
				{
					cluster.LoadedSelection |= 4;
					Executor.ExecuteOnPrivateProperties(handle, delegate(IntPtr propertyList, int propertyListSize)
					{
						AdapterBase.ParseProperties(mCluster.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
					});
				}
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(cluster.Name, cluster.Id, innerException);
			}
		}

		public void AddVirtualMachine(Guid vmId, string ownerNodeName)
		{
			clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
			{
				wmiAdapter.Cluster.AddVirtualMachine(vmId, ownerNodeName);
			});
		}

		private void LoadClusterVersionInfo(PCluster cluster)
		{
			SafeClusterHandle handle = Handle;
			if (handle != null && !handle.IsInvalid)
			{
				NativeMethods.CLUSTERVERSIONINFO cLUSTERVERSIONINFO = default(NativeMethods.CLUSTERVERSIONINFO);
				cLUSTERVERSIONINFO.dwVersionInfoSize = 284;
				NativeMethods.CLUSTERVERSIONINFO clusterVersionInfo = cLUSTERVERSIONINFO;
				string clusterInformation = Executor.GetClusterInformation(handle, ref clusterVersionInfo);
				cluster.Name = clusterInformation;
				cluster.MajorVersion = clusterVersionInfo.MajorVersion;
				cluster.MinorVersion = clusterVersionInfo.MinorVersion;
				cluster.BuildNumber = clusterVersionInfo.BuildNumber;
				cluster.Version = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", clusterVersionInfo.MajorVersion, clusterVersionInfo.MinorVersion, clusterVersionInfo.BuildNumber);
				cluster.OSVersion = new OSVersion(cluster);
				cluster.VendorId = clusterVersionInfo.szVendorId;
				cluster.CSDVersion = clusterVersionInfo.szCSDVersion;
				cluster.ClusterHighestVersion = clusterVersionInfo.dwClusterHighestVersion;
				cluster.ClusterLowestVersion = clusterVersionInfo.dwClusterLowestVersion;
				cluster.Flags = clusterVersionInfo.dwFlags;
				cluster.Reserved = clusterVersionInfo.dwReserved;
			}
		}

		public void Collect()
		{
		}

		public QuorumConfigurationPrivate GetQuorumConfiguration()
		{
			QuorumData quorumTuple = Executor.GetClusterQuorumConfiguration(Handle);
			ClusterAccessRights clusterAccessRights = Cluster.ClusterAccessRights;
			QuorumConfigurationPrivate quorumConfigurationPrivate = new QuorumConfigurationPrivate
			{
				QuorumResourceId = Guid.Empty,
				QuorumType = ((quorumTuple.QuorumType != 4194304) ? QuorumType.Majority : QuorumType.DiskOnly)
			};
			if (!string.IsNullOrEmpty(quorumTuple.QuorumResourceName))
			{
				Guid id = Guid.Empty;
				ResourceExecutor.ExecuteOnResource(Handle, clusterAccessRights, Guid.Empty, quorumTuple.QuorumResourceName, delegate(SafeClusterResourceHandle resourceHandle)
				{
					id = ResourceExecutor.GetResourceId(resourceHandle, quorumTuple.QuorumResourceName);
				});
				quorumConfigurationPrivate.QuorumResourceId = id;
			}
			return quorumConfigurationPrivate;
		}

		public void SetQuorumConfiguration(QuorumType quorumType, WitnessType witnessType, string quorumWitness, IEnumerable<string> nonVotingNodes)
		{
			throw new NotSupportedException("SetQuorumConfiguration is not implemented by ClusAPI Adapter");
		}

		public FileShareValidationStatus VerifyFileShare(string path)
		{
			throw new NotSupportedException("VerifyFileShare is not implemented by ClusAPI Adapter.");
		}

		public void UpdateFunctionalLevel(bool whatIf)
		{
			throw new NotSupportedException("UpdateFunctionalLevel is not implemented by ClusAPI Adapter.");
		}

		public IEnumerable<PResource> CreateDiskResources(IEnumerable<ClusterDisk> clusterableDisks)
		{
			throw new NotSupportedException("CreateDiskResources is not implemented by ClusAPI Adapter");
		}

		public IEnumerable<ClusterDisk> GetAvailableDisks(Guid poolId)
		{
			throw new NotSupportedException("GetAvailableDisks is not implemented by ClusAPI Adapter");
		}

		public IEnumerable<ClusterDisk> GetAvailableDisks(Guid poolId, bool all)
		{
			throw new NotSupportedException("GetAvailableDisks is not implemented by ClusAPI Adapter");
		}

		public void GetClusterableStoragePools(Action<ClusterableStoragePool> onNext, Action<Exception> onError, Action onCompleted)
		{
			clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
			{
				wmiAdapter.Cluster.GetClusterableStoragePools(onNext, onError, onCompleted);
			});
		}

		public void AddStoragePoolToCluster(ClusterableStoragePool pool, Action<Exception> onError, Action onCompleted)
		{
			clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
			{
				wmiAdapter.Cluster.AddStoragePoolToCluster(pool, onError, onCompleted);
			});
		}

		public bool NotificationArrived(NativeMethods.NOTIFY_FILTER_AND_TYPE filterType, string objectName, string objectId, string parentId, string objectType, IntPtr buffer, int bufferSize)
		{
			if (filterType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_CLUSTER)
			{
				if (filterType.FilterFlags == 512 || filterType.FilterFlags == 1)
				{
					Cluster.Refresh(targeted: false);
					return true;
				}
				if (filterType.FilterFlags == 1024)
				{
					Guid id = clusApiAdapter.clusterAdapter.Cluster.Id;
					string fullyQualifiedDomainName = GetFullyQualifiedDomainName();
					clusApiAdapter.EnqueueNotification(new ClusterNotification(new ClusterFullyQualifiedDomainChangedEventArgs(id, fullyQualifiedDomainName)));
					clusApiAdapter.EnqueueNotification(new ClusterNotification(new ClusterRenamedEventArgs(id, objectName, null)));
					return true;
				}
				if (filterType.FilterFlags == 128)
				{
					if (bufferSize == 0)
					{
						return true;
					}
					Guid id2 = clusApiAdapter.clusterAdapter.Cluster.Id;
					ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(id2, id2, ClusterIdentityType.Cluster)
					{
						Partial = true
					};
					AdapterBase.ParseProperties(clusterPropertyCollection, buffer, bufferSize, ClusterPropertyKind.Common, readOnly: false);
					clusApiAdapter.EnqueueNotification(new ClusterNotification(new ClusterPropertiesEventArgs(id2, objectName, null, null)
					{
						Cluster = clusApiAdapter.clusterAdapter.Cluster,
						Properties = clusterPropertyCollection
					}));
					return true;
				}
			}
			return false;
		}

		public bool WillVoterLossCauseQuorumLoss(QuorumVoterActionCheck voterActionCheck, string id)
		{
			bool willLoseQuorum = false;
			using (NativeMethods.UnmanagedBuffer unmanagedBuffer = NativeMethods.UnmanagedBuffer.Create(id))
			{
				if (unmanagedBuffer.IsMemoryValid)
				{
					Executor.ExecuteOnControlCode(clusterSafeHandle, (voterActionCheck == QuorumVoterActionCheck.Evict) ? NativeMethods.CLUSCTL_CLUSTER_CHECK_VOTER_EVICT : NativeMethods.CLUSCTL_CLUSTER_CHECK_VOTER_DOWN, unmanagedBuffer, delegate(IntPtr buffer, int bufferSize)
					{
						int num = Marshal.ReadInt32(buffer);
						willLoseQuorum = num == 1;
					});
				}
			}
			return willLoseQuorum;
		}
	}

	protected struct ValueListIterator
	{
		public readonly uint IteratorType;

		public readonly IntPtr Buffer;

		public readonly int BufferSize;

		public ValueListIterator(uint iteratorType, IntPtr buffer, int bufferSize)
		{
			IteratorType = iteratorType;
			Buffer = buffer;
			BufferSize = bufferSize;
		}
	}

	private class GroupAdapter : AdapterBase, IConnectionAdapterGroup, INotificationHandler
	{
		private readonly ClusApiAdapter clusApiAdapter;

		private readonly object loadingGroupsLock = new object();

		private const int DefaultEnumItemSize = 200;

		private const int DefaultBulkEnumItemSize = 512;

		private const int PriorityHighStart = 2501;

		private const int PriorityMediumStart = 1501;

		private const int PriorityLowStart = 1;

		public GroupAdapter(ClusApiAdapter clusApiAdapter)
			: base(clusApiAdapter)
		{
			this.clusApiAdapter = clusApiAdapter;
			Init();
		}

		private void Init()
		{
		}

		public IEnumerable<PGroup> GetAll(bool nullElementOnError)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				yield break;
			}
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			SafeClusterGroupEnumHandleEx enumHandle;
			try
			{
				intPtr2 = "GroupType".ToMultiSZPointer(out var size);
				intPtr = "Priority".ToMultiSZPointer(out var size2);
				enumHandle = NativeMethods.ClusterGroupOpenEnumEx(handle, intPtr, size2, intPtr2, size, 0);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
				if (intPtr2 != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr2);
				}
			}
			if (enumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateGroupsException>(Marshal.GetLastWin32Error());
			}
			IntPtr allocatedMemory = IntPtr.Zero;
			try
			{
				int records = NativeMethods.ClusterGroupGetEnumCountEx(enumHandle);
				for (int index = 0; index < records; index++)
				{
					PGroup privateGroup;
					try
					{
						int enumItemSize = 512;
						allocatedMemory = NativeMethods.Alloc(enumItemSize);
						int num = NativeMethods.ClusterGroupEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
						if (num == NativeMethods.ErrorCode.MoreData.ToInt())
						{
							allocatedMemory = NativeMethods.ReAlloc(allocatedMemory, enumItemSize);
							num = NativeMethods.ClusterGroupEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
						}
						if (num != NativeMethods.ErrorCode.None.ToInt())
						{
							throw ExceptionHelper.Build<ClusterIterateGroupsException>(Marshal.GetLastWin32Error());
						}
						NativeMethods.CLUSTER_FILTER_GROUP_ITEM cLUSTER_FILTER_GROUP_ITEM;
						ClusterPropertyCollection clusterPropertyCollection;
						ClusterPropertyCollection clusterPropertyCollection2;
						try
						{
							cLUSTER_FILTER_GROUP_ITEM = (NativeMethods.CLUSTER_FILTER_GROUP_ITEM)Marshal.PtrToStructure(allocatedMemory, typeof(NativeMethods.CLUSTER_FILTER_GROUP_ITEM));
							clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, new Guid(cLUSTER_FILTER_GROUP_ITEM.lpszId), ClusterIdentityType.Group);
							AdapterBase.ParseProperties(clusterPropertyCollection, cLUSTER_FILTER_GROUP_ITEM.pRoProperties, cLUSTER_FILTER_GROUP_ITEM.cbRoProperties, ClusterPropertyKind.Common, readOnly: true);
							clusterPropertyCollection2 = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, new Guid(cLUSTER_FILTER_GROUP_ITEM.lpszId), ClusterIdentityType.Group);
							AdapterBase.ParseProperties(clusterPropertyCollection2, cLUSTER_FILTER_GROUP_ITEM.pProperties, cLUSTER_FILTER_GROUP_ITEM.cbProperties, ClusterPropertyKind.Common, readOnly: false);
						}
						finally
						{
							allocatedMemory = NativeMethods.Free(allocatedMemory);
						}
						GroupType typedValue = (GroupType)((ClusterPropertyUInt)clusterPropertyCollection["GroupType"]).TypedValue;
						privateGroup = PGroup.Constructor(clusApiAdapter.clusterAdapter.Cluster, new Guid(cLUSTER_FILTER_GROUP_ITEM.lpszId), cLUSTER_FILTER_GROUP_ITEM.lpszName, typedValue);
						privateGroup.Priority = UpdateGroupPriority(((ClusterPropertyUInt)clusterPropertyCollection2["Priority"]).TypedValue);
						lock (loadingGroupsLock)
						{
							clusApiAdapter.MappingIdNameGroup.AddOrUpdate(privateGroup.Id, privateGroup.Name, (Guid key, string value) => privateGroup.Name);
							clusApiAdapter.MappingNameIdGroup.AddOrUpdate(privateGroup.Name, privateGroup.Id, (string key, Guid value) => privateGroup.Id);
						}
						privateGroup.Flags = (GroupFlags)cLUSTER_FILTER_GROUP_ITEM.dwFlags;
						privateGroup.GroupState = cLUSTER_FILTER_GROUP_ITEM.state;
						privateGroup.IsCore = (cLUSTER_FILTER_GROUP_ITEM.dwFlags & 1) > 0;
						PNode ownerNode = new PNode(privateGroup.Cluster, clusApiAdapter.nodes.GetNodeIdFromName(cLUSTER_FILTER_GROUP_ITEM.lpszOwnerNode), cLUSTER_FILTER_GROUP_ITEM.lpszOwnerNode);
						privateGroup.OwnerNode = ownerNode;
						privateGroup.LoadedSelection = 1;
					}
					catch (ClusterException exception)
					{
						if (!nullElementOnError)
						{
							throw;
						}
						ClusterLog.LogException(exception, "There was an error when getting group information from the cluster, however is not critical and the process can continue");
						privateGroup = null;
					}
					yield return privateGroup;
				}
			}
			finally
			{
				enumHandle.Dispose();
				NativeMethods.Free(allocatedMemory);
			}
		}

		private static Priority UpdateGroupPriority(uint groupPriority)
		{
			if (groupPriority >= 2501)
			{
				return Priority.High;
			}
			if (groupPriority >= 1501)
			{
				return Priority.Medium;
			}
			if (groupPriority >= 1)
			{
				return Priority.Low;
			}
			if (groupPriority == 0)
			{
				return Priority.NoAutoStart;
			}
			return (Priority)groupPriority;
		}

		public IEnumerable<TResult> Select<TResult>(QueryInfo queryInfo)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				yield break;
			}
			IEnumerable<string> source = queryInfo.ProjectionFields.Select((ClusterObjectMetaDataMember s) => s.Name).Concat(queryInfo.WhereFields.Select((ClusterObjectMetaDataMember s) => s.Name)).Concat(from s in queryInfo.OrderBy.ConvertAll((OrderByItem item) => item.DataMember)
				select s.Name)
				.Distinct();
			List<string> queryFields = source.Where((string s) => s.ToLowerInvariant() != "name" && s.ToLowerInvariant() != "id" && s.ToLowerInvariant() != "grouptype").ToList();
			IntPtr intPtr = IntPtr.Zero;
			SafeClusterGroupEnumHandleEx enumHandle;
			try
			{
				intPtr = "GroupType".ToMultiSZPointer(out var size);
				enumHandle = NativeMethods.ClusterGroupOpenEnumEx(handle, IntPtr.Zero, 0, intPtr, size, 0);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			if (enumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateGroupsException>(Marshal.GetLastWin32Error());
			}
			IntPtr allocatedMemory = IntPtr.Zero;
			try
			{
				int records = NativeMethods.ClusterGroupGetEnumCountEx(enumHandle);
				for (int index = 0; index < records; index++)
				{
					if (queryInfo.IsCancel)
					{
						break;
					}
					int enumItemSize = 200;
					allocatedMemory = NativeMethods.Alloc(enumItemSize);
					int num = NativeMethods.ClusterGroupEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
					if (num == NativeMethods.ErrorCode.MoreData.ToInt())
					{
						allocatedMemory = NativeMethods.ReAlloc(allocatedMemory, enumItemSize);
						num = NativeMethods.ClusterGroupEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
					}
					if (num != NativeMethods.ErrorCode.None.ToInt())
					{
						throw ExceptionHelper.Build<ClusterIterateGroupsException>(Marshal.GetLastWin32Error());
					}
					NativeMethods.CLUSTER_FILTER_GROUP_ITEM cLUSTER_FILTER_GROUP_ITEM;
					ClusterPropertyCollection clusterPropertyCollection;
					try
					{
						cLUSTER_FILTER_GROUP_ITEM = (NativeMethods.CLUSTER_FILTER_GROUP_ITEM)Marshal.PtrToStructure(allocatedMemory, typeof(NativeMethods.CLUSTER_FILTER_GROUP_ITEM));
						clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, new Guid(cLUSTER_FILTER_GROUP_ITEM.lpszId), ClusterIdentityType.Group);
						AdapterBase.ParseProperties(clusterPropertyCollection, cLUSTER_FILTER_GROUP_ITEM.pRoProperties, cLUSTER_FILTER_GROUP_ITEM.cbRoProperties, ClusterPropertyKind.Common, readOnly: true);
					}
					finally
					{
						allocatedMemory = NativeMethods.Free(allocatedMemory);
					}
					GroupType typedValue = (GroupType)((ClusterPropertyUInt)clusterPropertyCollection["GroupType"]).TypedValue;
					PGroup privateGroup = PGroup.Constructor(clusApiAdapter.clusterAdapter.Cluster, new Guid(cLUSTER_FILTER_GROUP_ITEM.lpszId), cLUSTER_FILTER_GROUP_ITEM.lpszName, typedValue);
					lock (loadingGroupsLock)
					{
						clusApiAdapter.MappingIdNameGroup.AddOrUpdate(privateGroup.Id, privateGroup.Name, (Guid key, string value) => privateGroup.Name);
						clusApiAdapter.MappingNameIdGroup.AddOrUpdate(privateGroup.Name, privateGroup.Id, (string key, Guid value) => privateGroup.Id);
					}
					if (queryFields.Any())
					{
						if (queryFields.Count() == 1 && queryFields.ElementAt(0).ToLowerInvariant() == "iscore")
						{
							SetIsCore(privateGroup);
						}
						else
						{
							GroupLoadSelection loadSelection = GroupLoadSelection.None;
							foreach (string item in queryFields)
							{
								switch (item.ToLowerInvariant())
								{
								case "priority":
								case "state":
								case "ownernode":
								case "iscore":
								case "flags":
									loadSelection |= GroupLoadSelection.Basic;
									continue;
								case "preferredowners":
									loadSelection |= GroupLoadSelection.PreferredOwners;
									continue;
								}
								if (item.Equals("commonproperties"))
								{
									loadSelection |= GroupLoadSelection.CommonProperties;
								}
								if (item.Equals("privateproperties"))
								{
									loadSelection |= GroupLoadSelection.PrivateProperties;
								}
							}
							ExecuteOnGroup(privateGroup.Id, privateGroup.Name, delegate(SafeClusterGroupHandle groupHandle)
							{
								if ((loadSelection & GroupLoadSelection.Basic) == GroupLoadSelection.Basic)
								{
									ExecuteOnCommonProperties(groupHandle, privateGroup.Name, delegate(IntPtr propertyList, int propertyListSize)
									{
										AdapterBase.ParseProperties(privateGroup.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
									});
									privateGroup.Priority = UpdateGroupPriority(((ClusterPropertyUInt)privateGroup.Properties["Priority"]).TypedValue);
									SetStateAndNode(privateGroup, groupHandle);
									SetIsCore(privateGroup);
									ExecuteOnControlCode(groupHandle, NativeMethods.CLUSCTL_GROUP_GET_FLAGS, privateGroup.Name, delegate(IntPtr buffer, int bufferSize)
									{
										privateGroup.Flags = (GroupFlags)Marshal.ReadInt32(buffer);
									}, IntPtr.Zero);
									privateGroup.LoadedSelection |= 1;
								}
								if ((loadSelection & GroupLoadSelection.CommonProperties) == GroupLoadSelection.CommonProperties)
								{
									ExecuteOnCommonProperties(groupHandle, privateGroup.Name, delegate(IntPtr propertyList, int propertyListSize)
									{
										AdapterBase.ParseProperties(privateGroup.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
									});
									ExecuteOnReadOnlyCommonProperties(groupHandle, privateGroup.Name, delegate(IntPtr propertyList, int propertyListSize)
									{
										AdapterBase.ParseProperties(privateGroup.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
									});
									privateGroup.LoadedSelection |= 2;
								}
								if ((loadSelection & GroupLoadSelection.PrivateProperties) == GroupLoadSelection.PrivateProperties)
								{
									ExecuteOnPrivateProperties(groupHandle, privateGroup.Name, delegate(IntPtr propertyList, int propertyListSize)
									{
										AdapterBase.ParseProperties(privateGroup.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
									});
									ExecuteOnReadOnlyPrivateProperties(groupHandle, privateGroup.Name, delegate(IntPtr propertyList, int propertyListSize)
									{
										AdapterBase.ParseProperties(privateGroup.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
									});
									privateGroup.LoadedSelection |= 4;
								}
								if ((loadSelection & GroupLoadSelection.PreferredOwners) == GroupLoadSelection.PreferredOwners)
								{
									privateGroup.PreferredOwners = GetPreferredOwnersList(groupHandle);
									privateGroup.LoadedSelection |= 8;
								}
							});
						}
					}
					yield return (TResult)(object)privateGroup;
				}
			}
			finally
			{
				enumHandle.Dispose();
				NativeMethods.Free(allocatedMemory);
			}
		}

		public void Close(Guid id)
		{
		}

		public void Close(string name)
		{
		}

		public void Delete(Guid id, bool force, bool cleanup)
		{
			clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
			{
				wmiAdapter.Group.Delete(id, force, cleanup);
			});
		}

		public PGroup Create(string name, GroupType groupType)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				return null;
			}
			NativeMethods.CLUSTER_CREATE_GROUP_INFO groupInfo = new NativeMethods.CLUSTER_CREATE_GROUP_INFO((int)groupType);
			SafeClusterGroupHandle safeClusterGroupHandle = NativeMethods.CreateClusterGroupEx(handle, name, ref groupInfo);
			Guid id;
			try
			{
				if (safeClusterGroupHandle.IsInvalid)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error == NativeMethods.ErrorCode.ObjectAlreadyExists.ToInt())
					{
						throw new ClusterGroupAlreadyExistException(name);
					}
					throw ExceptionHelper.Build(lastWin32Error);
				}
				id = GetId(safeClusterGroupHandle);
			}
			finally
			{
				safeClusterGroupHandle.Dispose();
			}
			return PGroup.Constructor(clusApiAdapter.clusterAdapter.Cluster, id, name, groupType);
		}

		public void Rename(Guid id, string newName)
		{
			ExecuteOnGroup(id, null, delegate(SafeClusterGroupHandle groupHandle)
			{
				int num = NativeMethods.SetClusterGroupName(groupHandle, newName);
				if (num == NativeMethods.ErrorCode.ErrorAlreadyExists.ToInt())
				{
					throw new ClusterGroupAlreadyExistException(newName);
				}
				if (num != NativeMethods.ErrorCode.None.ToInt() && num != NativeMethods.ErrorCode.IOPending.ToInt())
				{
					throw ExceptionHelper.Build(num);
				}
			});
		}

		public void SetPriority(Guid id, Priority priority)
		{
			ExecuteOnGroup(id, null, delegate(SafeClusterGroupHandle groupHandle)
			{
				ExecuteOnCommonPropertiesSet(clusApiAdapter.MappingIdNameGroup[id], groupHandle, delegate(SafeClusterPropertyListHandle propList)
				{
					int num = NativeMethods.AddDwordProperty(propList, "Priority", (uint)priority);
					if (num != NativeMethods.ErrorCode.None.ToInt() && num != NativeMethods.ErrorCode.IOPending.ToInt())
					{
						throw ExceptionHelper.Build(num);
					}
				});
			});
		}

		public void Online(Guid id, bool overrideLockState = false, bool chooseBestNode = false)
		{
			ExecuteOnGroup(id, null, delegate(SafeClusterGroupHandle groupHandle)
			{
				IntPtr zero = IntPtr.Zero;
				uint num = 0u;
				if (overrideLockState)
				{
					num |= 1u;
				}
				if (overrideLockState)
				{
					num |= 4u;
				}
				int num2 = ((num == 0) ? NativeMethods.OnlineClusterGroupEx(groupHandle, zero, num, IntPtr.Zero, 0) : NativeMethods.OnlineClusterGroup(groupHandle, zero));
				if (!NativeMethods.ErrorCode.None.IsEqual(num2) && !NativeMethods.ErrorCode.IOPending.IsEqual(num2))
				{
					string groupNameFromId = GetGroupNameFromId(id);
					AdapterBase.VerifyOperationOccuredWhileUnlocked(num2, groupNameFromId, id);
					if (NativeMethods.ErrorCode.ResourceIsReplicaVirtualMachine.IsEqual(num2))
					{
						throw new ClusterVirtualMachineStartReplicaException(ExceptionResources.VirtualMachineReplicaStart_Default2, string.Empty, null);
					}
					if (groupNameFromId != null)
					{
						throw new ClusterGroupOnlineException(groupNameFromId, new Win32Exception(num2));
					}
					throw new ClusterGroupOnlineException(id, new Win32Exception(num2));
				}
			});
		}

		public void Offline(Guid id, bool overrideLockState = false)
		{
			ExecuteOnGroup(id, null, delegate(SafeClusterGroupHandle groupHandle)
			{
				int num = (overrideLockState ? NativeMethods.OfflineClusterGroupEx(groupHandle, 1u, IntPtr.Zero, 0) : NativeMethods.OfflineClusterGroup(groupHandle));
				if (num != NativeMethods.ErrorCode.None.ToInt() && num != NativeMethods.ErrorCode.IOPending.ToInt())
				{
					string groupNameFromId = GetGroupNameFromId(id);
					AdapterBase.VerifyOperationOccuredWhileUnlocked(num, groupNameFromId, id);
					if (groupNameFromId != null)
					{
						throw new ClusterGroupOfflineException(groupNameFromId, new Win32Exception(num));
					}
					throw new ClusterGroupOfflineException(id, new Win32Exception(num));
				}
			});
		}

		public void CancelOperation(Guid id)
		{
			ExecuteOnGroup(id, null, delegate(SafeClusterGroupHandle groupHandle)
			{
				int num = NativeMethods.CancelClusterGroupOperation(groupHandle, NativeMethods.NoFlags);
				if (num != NativeMethods.ErrorCode.None.ToInt() && num != NativeMethods.ErrorCode.IOPending.ToInt())
				{
					string groupNameFromId = GetGroupNameFromId(id);
					if (groupNameFromId != null)
					{
						throw new ClusterGroupCancelLiveMigrationException(groupNameFromId, new Win32Exception(num));
					}
					throw new ClusterGroupCancelLiveMigrationException(id, new Win32Exception(num));
				}
			});
		}

		public void Move(Guid id, string nodeName, bool overrideLockState = false)
		{
			ExecuteOnGroup(id, null, delegate(SafeClusterGroupHandle groupHandle)
			{
				clusApiAdapter.nodes.ExecuteOnNode(Guid.Empty, nodeName, delegate(SafeClusterNodeHandle nodeHandle)
				{
					int num = (overrideLockState ? NativeMethods.MoveClusterGroupEx(groupHandle, nodeHandle, 1u, IntPtr.Zero, 0) : NativeMethods.MoveClusterGroup(groupHandle, nodeHandle));
					if (!NativeMethods.ErrorCode.None.IsEqual(num) && !NativeMethods.ErrorCode.IOPending.IsEqual(num))
					{
						string groupNameFromId = GetGroupNameFromId(id);
						AdapterBase.VerifyOperationOccuredWhileUnlocked(num, groupNameFromId, id);
						if (groupNameFromId != null)
						{
							throw new ClusterGroupMoveException(groupNameFromId, new Win32Exception(num));
						}
						throw new ClusterGroupMoveException(id, new Win32Exception(num));
					}
				});
			});
		}

		private void GetMigrationTypePropertyList(VirtualMachineMigrationType migrationType, Action<IntPtr, int> propertyList)
		{
			SafeClusterPropertyListHandle safeClusterPropertyListHandle = NativeMethods.CreatePropList(IntPtr.Zero, 0);
			if (safeClusterPropertyListHandle.IsInvalid)
			{
				throw new ClusterDefaultException(new Win32Exception(Marshal.GetLastWin32Error()));
			}
			Action<SafeClusterPropertyListHandle, string, VirtualMachineMigrationType> action = delegate(SafeClusterPropertyListHandle clusterList, string resourceTypeName, VirtualMachineMigrationType type)
			{
				int num = NativeMethods.AddDwordProperty(clusterList, resourceTypeName, (uint)type);
				if (num != NativeMethods.ErrorCode.None.ToInt())
				{
					throw new ClusterPropertyListBufferException(new Win32Exception(num));
				}
			};
			switch (migrationType)
			{
			case VirtualMachineMigrationType.Live:
				action(safeClusterPropertyListHandle, "Virtual Machine", VirtualMachineMigrationType.Live);
				action(safeClusterPropertyListHandle, "Virtual Machine Configuration", VirtualMachineMigrationType.Live);
				break;
			case VirtualMachineMigrationType.Quick:
				action(safeClusterPropertyListHandle, "Virtual Machine", VirtualMachineMigrationType.Quick);
				action(safeClusterPropertyListHandle, "Virtual Machine Configuration", VirtualMachineMigrationType.Quick);
				break;
			case VirtualMachineMigrationType.Shutdown:
				action(safeClusterPropertyListHandle, "Virtual Machine", VirtualMachineMigrationType.Shutdown);
				action(safeClusterPropertyListHandle, "Virtual Machine Configuration", VirtualMachineMigrationType.Shutdown);
				break;
			case VirtualMachineMigrationType.ShutdownForce:
				action(safeClusterPropertyListHandle, "Virtual Machine", VirtualMachineMigrationType.ShutdownForce);
				action(safeClusterPropertyListHandle, "Virtual Machine Configuration", VirtualMachineMigrationType.ShutdownForce);
				break;
			case VirtualMachineMigrationType.Turnoff:
				action(safeClusterPropertyListHandle, "Virtual Machine", VirtualMachineMigrationType.Turnoff);
				action(safeClusterPropertyListHandle, "Virtual Machine Configuration", VirtualMachineMigrationType.Turnoff);
				break;
			default:
				throw new ArgumentOutOfRangeException("migrationType");
			}
			IntPtr propertyListBuffer;
			int propertyListSize;
			int propertyListBuffer2 = NativeMethods.GetPropertyListBuffer(safeClusterPropertyListHandle, out propertyListBuffer, out propertyListSize);
			if (propertyListBuffer2 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw new ClusterPropertyListBufferException(new Win32Exception(propertyListBuffer2));
			}
			try
			{
				propertyList.SafeCall(propertyListBuffer, propertyListSize);
			}
			finally
			{
				safeClusterPropertyListHandle.Dispose();
			}
		}

		private void MigrateVirtualMachine(PVirtualMachineGroup group, SafeClusterNodeHandle nodeHandle, VirtualMachineMigrationType migrationType, bool overrideLockState = false)
		{
			ExecuteOnGroup(Guid.Empty, group.Name, delegate(SafeClusterGroupHandle groupHandle)
			{
				int flags = ((migrationType == VirtualMachineMigrationType.Live) ? 14 : 0);
				if (overrideLockState)
				{
					flags |= 1;
				}
				GetMigrationTypePropertyList(migrationType, delegate(IntPtr propertyListBuffer, int propertyListBufferSize)
				{
					int num = NativeMethods.MoveClusterGroupEx(groupHandle, nodeHandle, flags, propertyListBuffer, propertyListBufferSize);
					if (!NativeMethods.ErrorCode.None.IsEqual(num) && !NativeMethods.ErrorCode.IOPending.IsEqual(num))
					{
						string groupNameFromId = GetGroupNameFromId(group.Id);
						AdapterBase.VerifyOperationOccuredWhileUnlocked(num, groupNameFromId, group.Id);
						if (groupNameFromId != null)
						{
							throw new ClusterGroupMoveException(groupNameFromId, new Win32Exception(num));
						}
						throw ExceptionHelper.Build(num);
					}
				});
			});
		}

		public IEnumerable<string> GetPreferredOwners(Guid id)
		{
			throw new NotSupportedException("GetPreferredOwners is not supported by ClusApiAdapter");
		}

		public void SetPreferredOwners(Guid id, IEnumerable<string> nodes)
		{
			SafeClusterHandle clusterHandle = clusApiAdapter.clusterAdapter.Handle;
			if (clusterHandle == null)
			{
				return;
			}
			ExecuteOnGroup(id, null, delegate(SafeClusterGroupHandle groupHandle)
			{
				List<SafeClusterNodeHandle> list = new List<SafeClusterNodeHandle>();
				try
				{
					foreach (string node in nodes)
					{
						ClusterAccessRights grantedAccess;
						SafeClusterNodeHandle safeClusterNodeHandle = NativeMethods.OpenClusterNodeEx(clusterHandle, node, clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess);
						int lastWin32Error = Marshal.GetLastWin32Error();
						if (lastWin32Error == NativeMethods.ErrorCode.NodeNotFound.ToInt())
						{
							throw new ClusterObjectNotFoundException(node, id, typeof(Node));
						}
						if (safeClusterNodeHandle.IsInvalid)
						{
							throw ExceptionHelper.ClusterObjectLoadFailedException(node, Guid.Empty, lastWin32Error);
						}
						list.Add(safeClusterNodeHandle);
					}
					IntPtr[] nodes2 = list.ConvertAll((SafeClusterNodeHandle safeHandle) => safeHandle.DangerousGetHandle()).ToArray();
					int num = NativeMethods.SetClusterGroupNodeList(groupHandle, list.Count, nodes2);
					if (num != NativeMethods.ErrorCode.None.ToInt())
					{
						throw new ClusterGroupSetPreferredOwnersException(GetGroupNameFromId(id), new Win32Exception(num));
					}
				}
				finally
				{
					foreach (SafeClusterNodeHandle item in list)
					{
						item.Dispose();
					}
				}
			});
		}

		public PGroup Open(Guid id)
		{
			PGroup privateGroup = null;
			ExecuteOnGroup(id, null, delegate(SafeClusterGroupHandle groupHandle)
			{
				string groupNameFromId = GetGroupNameFromId(id);
				GroupType groupType = GetGroupType(groupHandle);
				privateGroup = PGroup.Constructor(clusApiAdapter.clusterAdapter.Cluster, id, groupNameFromId, groupType);
			});
			return privateGroup;
		}

		public PGroup Open(string groupName)
		{
			PGroup privateGroup = null;
			ExecuteOnGroup(Guid.Empty, groupName, delegate(SafeClusterGroupHandle groupHandle)
			{
				Guid groupIdFromName = GetGroupIdFromName(groupName);
				GroupType groupType = GetGroupType(groupHandle);
				privateGroup = PGroup.Constructor(clusApiAdapter.clusterAdapter.Cluster, groupIdFromName, groupName, groupType);
			});
			return privateGroup;
		}

		public void Load(PGroup group, GroupLoadSelection loadSelection)
		{
			try
			{
				ExecuteOnGroup(group.Id, group.Name, delegate(SafeClusterGroupHandle groupHandle)
				{
					if ((loadSelection & GroupLoadSelection.Basic) == GroupLoadSelection.Basic)
					{
						group.LoadedSelection |= 1;
						SetStateAndNode(group, groupHandle);
						ExecuteOnControlCode(groupHandle, NativeMethods.CLUSCTL_GROUP_GET_FLAGS, group.Name, delegate(IntPtr buffer, int bufferSize)
						{
							group.Flags = (GroupFlags)Marshal.ReadInt32(buffer);
							group.IsCore = (1u & (uint?)group.Flags) == 1;
						}, IntPtr.Zero);
					}
					if (((loadSelection & GroupLoadSelection.CommonProperties) == GroupLoadSelection.CommonProperties && (group.LoadedSelection & 2) != 2) || ((loadSelection & GroupLoadSelection.Basic) == GroupLoadSelection.Basic && (group.LoadedSelection & 1) != 1))
					{
						group.LoadedSelection |= 2;
						ExecuteOnCommonProperties(groupHandle, group.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(group.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
							group.Priority = UpdateGroupPriority((uint)group.Properties["Priority"].Value);
						});
						ExecuteOnReadOnlyCommonProperties(groupHandle, group.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(group.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
						});
					}
					if ((loadSelection & GroupLoadSelection.PrivateProperties) == GroupLoadSelection.PrivateProperties && (group.LoadedSelection & 4) != 4)
					{
						group.LoadedSelection |= 4;
						ExecuteOnPrivateProperties(groupHandle, group.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(group.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
						});
						ExecuteOnReadOnlyPrivateProperties(groupHandle, group.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(group.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: true);
						});
					}
					if ((loadSelection & GroupLoadSelection.PreferredOwners) == GroupLoadSelection.PreferredOwners && (group.LoadedSelection & 8) != 8)
					{
						group.LoadedSelection |= 8;
						group.PreferredOwners = GetPreferredOwnersList(groupHandle);
					}
				});
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(group.Name, group.Id, innerException);
			}
		}

		private void ExecuteOnCommonProperties(SafeClusterGroupHandle groupHandle, string groupName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(groupHandle, NativeMethods.CLUSCTL_GROUP_GET_COMMON_PROPERTIES, groupName, commonPropList);
		}

		private void ExecuteOnPrivateProperties(SafeClusterGroupHandle groupHandle, string groupName, Action<IntPtr, int> commonPropList)
		{
			Utilities.UnreferencedParameter(groupHandle);
			Utilities.UnreferencedParameter(groupName);
			Utilities.UnreferencedParameter(commonPropList);
		}

		private void ExecuteOnReadOnlyCommonProperties(SafeClusterGroupHandle groupHandle, string groupName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(groupHandle, NativeMethods.CLUSCTL_GROUP_GET_RO_COMMON_PROPERTIES, groupName, commonPropList);
		}

		private void ExecuteOnReadOnlyPrivateProperties(SafeClusterGroupHandle groupHandle, string groupName, Action<IntPtr, int> commonPropList)
		{
			Utilities.UnreferencedParameter(groupHandle);
			Utilities.UnreferencedParameter(groupName);
			Utilities.UnreferencedParameter(commonPropList);
		}

		private static void ExecuteOnProperties(SafeClusterGroupHandle groupHandle, int controlCode, string groupName, Action<IntPtr, int> propertyList)
		{
			try
			{
				ExecuteOnControlCode(groupHandle, controlCode, groupName, propertyList, IntPtr.Zero);
			}
			catch (ClusterControlCodeException innerException)
			{
				throw new ClusterGetPropertiesFailedException(innerException);
			}
		}

		public void ExecuteOnGroup(Guid id, string name, Action<SafeClusterGroupHandle> actionOnGroup)
		{
			if (id == Guid.Empty && name == null)
			{
				throw new ArgumentException("id and name cannot be both null");
			}
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				return;
			}
			ClusterAccessRights grantedAccess;
			SafeClusterGroupHandle safeClusterGroupHandle = ((id != Guid.Empty) ? NativeMethods.OpenClusterGroupEx(handle, id.ToString(), clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess) : NativeMethods.OpenClusterGroupEx(handle, name, clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess));
			if (safeClusterGroupHandle.IsInvalid)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == NativeMethods.ErrorCode.GroupNotFound.ToInt())
				{
					throw new ClusterObjectNotFoundException(name, id, typeof(FailoverClusters.Framework.Group));
				}
				throw ExceptionHelper.ClusterObjectLoadFailedException(name, id, lastWin32Error);
			}
			try
			{
				actionOnGroup(safeClusterGroupHandle);
			}
			finally
			{
				safeClusterGroupHandle.Dispose();
			}
		}

		public GroupType GetGroupType(SafeClusterGroupHandle groupHandle)
		{
			SafeClusterKeyHandle clusterGroupKey = NativeMethods.GetClusterGroupKey(groupHandle, RegistryRights.QueryValues);
			if (clusterGroupKey.IsInvalid)
			{
				throw new ClusterRegistryException(new Win32Exception(Marshal.GetLastWin32Error()));
			}
			int data;
			try
			{
				int dataSize = 4;
				int num = NativeMethods.ClusterRegQueryValue(clusterGroupKey, "GroupType", IntPtr.Zero, out data, ref dataSize);
				if (num != NativeMethods.ErrorCode.None.ToInt())
				{
					if (num == NativeMethods.ErrorCode.FileNotFound.ToInt())
					{
						data = 9999;
						return (GroupType)data;
					}
					throw new ClusterRegistryException(new Win32Exception(num));
				}
			}
			finally
			{
				clusterGroupKey.Dispose();
			}
			return (GroupType)data;
		}

		public bool NotificationArrived(NativeMethods.NOTIFY_FILTER_AND_TYPE filterType, string objectName, string objectId, string parentId, string objectType, IntPtr buffer, int bufferSize)
		{
			if (filterType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_CLUSTER && filterType.FilterFlags == 4)
			{
				Guid groupId3 = new Guid(objectId);
				if (bufferSize != 4)
				{
					ClusterLog.LogError("Unexpected buffer size, the buffer size for a Group Added notification should be 4 bytes but it is {0} bytes", bufferSize.ToString(CultureInfo.CurrentCulture));
				}
				GroupType value2 = (GroupType)Marshal.ReadInt32(buffer);
				if (objectName.Length == 0)
				{
					ClusterLog.LogError("Empty group name, the group name in group add notification is empty");
				}
				lock (loadingGroupsLock)
				{
					clusApiAdapter.MappingIdNameGroup.AddOrUpdate(groupId3, objectName, (Guid key, string value) => objectName);
					clusApiAdapter.MappingNameIdGroup.AddOrUpdate(objectName, groupId3, (string key, Guid value) => groupId3);
				}
				ClusterAddedEventArgs payload = new ClusterAddedEventArgs(groupId3, objectName, (int)value2, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster
				};
				clusApiAdapter.EnqueueNotification(new GroupNotification(payload));
				return true;
			}
			if (filterType.ObjectType != NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_GROUP)
			{
				return false;
			}
			NativeMethods.CLUSTER_CHANGE_GROUP_V2 filterFlags = (NativeMethods.CLUSTER_CHANGE_GROUP_V2)filterType.FilterFlags;
			Guid groupId;
			ClusterPropertyCollection clusterPropertyCollection;
			switch (filterFlags)
			{
			case (NativeMethods.CLUSTER_CHANGE_GROUP_V2)0uL:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_DELETED_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_COMMON_PROPERTY_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_DELETED_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_COMMON_PROPERTY_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_PRIVATE_PROPERTY_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_DELETED_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_PRIVATE_PROPERTY_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_PRIVATE_PROPERTY_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_DELETED_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_PRIVATE_PROPERTY_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_STATE_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_DELETED_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_STATE_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_STATE_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_DELETED_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_STATE_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_PRIVATE_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_STATE_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_DELETED_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_PRIVATE_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_STATE_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_PRIVATE_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_STATE_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_DELETED_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_PRIVATE_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_STATE_V2:
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_OWNER_NODE_V2:
			{
				NativeMethods.CLUSTER_CHANGE_GROUP_V2 num = filterFlags - 1;
				if (num <= (NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_DELETED_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_COMMON_PROPERTY_V2))
				{
					switch (num)
					{
					case (NativeMethods.CLUSTER_CHANGE_GROUP_V2)0uL:
					{
						Guid guid2 = new Guid(objectId);
						lock (loadingGroupsLock)
						{
							clusApiAdapter.MappingIdNameGroup.TryRemove(guid2, out var _);
							clusApiAdapter.MappingNameIdGroup.TryRemove(objectName, out var _);
						}
						ClusterRemovedEventArgs payload2 = new ClusterRemovedEventArgs(guid2, objectName, null)
						{
							Cluster = clusApiAdapter.clusterAdapter.Cluster
						};
						clusApiAdapter.EnqueueNotification(new GroupNotification(payload2));
						return true;
					}
					case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_DELETED_V2:
						goto IL_02ea;
					case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_DELETED_V2 | NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_COMMON_PROPERTY_V2:
					{
						Guid guid = new Guid(objectId);
						ClusterPropertyCollection properties = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, guid, ClusterIdentityType.Group);
						try
						{
							ExecuteOnGroup(Guid.Empty, objectName, delegate(SafeClusterGroupHandle groupHandle)
							{
								ExecuteOnPrivateProperties(groupHandle, objectName, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
								});
								ExecuteOnReadOnlyPrivateProperties(groupHandle, objectName, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: true);
								});
							});
						}
						catch (ClusterObjectNotFoundException)
						{
							return true;
						}
						catch (ClusterObjectDeletingException)
						{
							return true;
						}
						clusApiAdapter.EnqueueNotification(new GroupNotification(new ClusterPropertiesEventArgs(guid, objectName, null, null)
						{
							Cluster = clusApiAdapter.clusterAdapter.Cluster,
							Properties = properties
						}));
						return true;
					}
					case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_COMMON_PROPERTY_V2:
						goto end_IL_0150;
					}
				}
				switch (filterFlags)
				{
				case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_STATE_V2:
				{
					Guid id4 = new Guid(objectId);
					GroupState value5 = (GroupState)Marshal.ReadInt32(buffer);
					clusApiAdapter.EnqueueNotification(new GroupNotification(new ClusterGroupStateEventArgs(id4, value5, null)));
					return true;
				}
				case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_OWNER_NODE_V2:
				{
					Guid nodeId = Guid.Empty;
					Guid groupId2 = new Guid(objectId);
					ExecuteOnGroup(groupId2, objectName, delegate(SafeClusterGroupHandle groupHandle)
					{
						GetNode(objectName, groupId2, groupHandle, out nodeId);
					});
					clusApiAdapter.EnqueueNotification(new GroupNotification(new ClusterGroupOwnerNodeEventArgs(groupId2, nodeId, null)));
					return true;
				}
				}
				break;
			}
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_RESOURCE_GAINED_V2:
			{
				Guid id3 = new Guid(objectId);
				string g2 = Marshal.PtrToStringUni(buffer);
				Guid gainedId = new Guid(g2);
				clusApiAdapter.EnqueueNotification(new GroupNotification(new ClusterGainedEventArgs(id3, objectName, gainedId)));
				return true;
			}
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_RESOURCE_LOST_V2:
			{
				Guid id2 = new Guid(objectId);
				string g = Marshal.PtrToStringUni(buffer);
				Guid lostId = new Guid(g);
				clusApiAdapter.EnqueueNotification(new GroupNotification(new ClusterLostEventArgs(id2, objectName, lostId)));
				return true;
			}
			case NativeMethods.CLUSTER_CHANGE_GROUP_V2.CLUSTER_CHANGE_GROUP_PREFERRED_OWNERS_V2:
				{
					Guid id = new Guid(objectId);
					List<Guid> preferredOwnersList = GetPreferredOwnersList(id);
					clusApiAdapter.EnqueueNotification(new GroupNotification(new ClusterGroupPreferredOwnersChangedEventArgs(id, preferredOwnersList, null)));
					return true;
				}
				IL_02ea:
				if (bufferSize == 0)
				{
					return true;
				}
				groupId = new Guid(objectId);
				clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, groupId, ClusterIdentityType.Group)
				{
					Partial = true
				};
				AdapterBase.ParseProperties(clusterPropertyCollection, buffer, bufferSize, ClusterPropertyKind.Common, readOnly: false);
				clusterPropertyCollection.Get("priority", delegate(ClusterPropertyUInt priority)
				{
					clusApiAdapter.EnqueueNotification(new GroupNotification(new ClusterGroupPriorityEventArgs(groupId, UpdateGroupPriority(priority.TypedValue), null)));
				});
				clusterPropertyCollection.Get("Name", delegate(ClusterPropertyString newName)
				{
					clusApiAdapter.EnqueueNotification(new GroupNotification(new ClusterRenamedEventArgs(groupId, newName.TypedValue, null)));
				});
				clusApiAdapter.EnqueueNotification(new GroupNotification(new ClusterPropertiesEventArgs(groupId, objectName, null, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster,
					Properties = clusterPropertyCollection
				}));
				return true;
				end_IL_0150:
				break;
			}
			return false;
		}

		public void Collect()
		{
		}

		private List<Guid> GetPreferredOwnersList(Guid id)
		{
			List<Guid> nodeIds = new List<Guid>();
			ExecuteOnGroup(id, null, delegate(SafeClusterGroupHandle groupHandle)
			{
				nodeIds = GetPreferredOwnersList(groupHandle);
			});
			return nodeIds;
		}

		private List<Guid> GetPreferredOwnersList(SafeClusterGroupHandle groupHandle)
		{
			List<Guid> list = new List<Guid>();
			SafeClusterGroupEnumHandle safeClusterGroupEnumHandle = NativeMethods.ClusterGroupOpenEnum(groupHandle, NativeMethods.GroupEnumType.Node);
			if (safeClusterGroupEnumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateNodeException>(Marshal.GetLastWin32Error());
			}
			try
			{
				int num = NativeMethods.ClusterGroupGetEnumCount(safeClusterGroupEnumHandle);
				for (int i = 0; i < num; i++)
				{
					NativeMethods.GroupEnumType enumType = NativeMethods.GroupEnumType.Node;
					StringBuilder stringBuilder = new StringBuilder(200);
					int resourceNameSize = 200;
					int num2 = NativeMethods.ClusterGroupEnum(safeClusterGroupEnumHandle, i, ref enumType, stringBuilder, ref resourceNameSize);
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (num2 == NativeMethods.ErrorCode.MoreData.ToInt())
					{
						stringBuilder = new StringBuilder(resourceNameSize);
						num2 = NativeMethods.ClusterGroupEnum(safeClusterGroupEnumHandle, i, ref enumType, stringBuilder, ref resourceNameSize);
					}
					if (num2 != NativeMethods.ErrorCode.None.ToInt())
					{
						throw ExceptionHelper.Build<ClusterIterateGroupsException>(lastWin32Error);
					}
					Guid nodeIdFromName = clusApiAdapter.nodes.GetNodeIdFromName(stringBuilder.ToString());
					list.Add(nodeIdFromName);
				}
				return list;
			}
			finally
			{
				safeClusterGroupEnumHandle.Dispose();
			}
		}

		private void SetIsCore(PGroup group)
		{
			if (!group.IsCore.HasValue)
			{
				group.IsCore = clusApiAdapter.clusterAdapter.CoreGroups.Contains(group.Id);
			}
		}

		internal void SetStateAndNode(PGroup group, SafeClusterGroupHandle groupHandle)
		{
			if (group.GroupState.HasValue && group.OwnerNode != null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(200);
			int nodeNameSize = 200;
			int clusterGroupState = NativeMethods.GetClusterGroupState(groupHandle, stringBuilder, ref nodeNameSize);
			if (clusterGroupState == -1)
			{
				clusterGroupState = Marshal.GetLastWin32Error();
				if (NativeMethods.ErrorCode.GroupNotAvailable.IsEqual(clusterGroupState) || NativeMethods.ErrorCode.GroupNotFound.IsEqual(clusterGroupState))
				{
					throw new ClusterObjectNotFoundException(group.Name, new Win32Exception(clusterGroupState));
				}
				throw new Win32Exception(clusterGroupState);
			}
			group.GroupState = (GroupState)clusterGroupState;
			if (group.OwnerNode == null)
			{
				PNode ownerNode = new PNode(group.Cluster, clusApiAdapter.nodes.GetNodeIdFromName(stringBuilder.ToString()), stringBuilder.ToString());
				group.OwnerNode = ownerNode;
			}
		}

		private void GetNode(string groupName, Guid groupId, SafeClusterGroupHandle groupHandle, out Guid nodeId)
		{
			Utilities.UnreferencedParameter(groupId);
			StringBuilder stringBuilder = new StringBuilder(200);
			int nodeNameSize = 200;
			int clusterGroupState = NativeMethods.GetClusterGroupState(groupHandle, stringBuilder, ref nodeNameSize);
			if (clusterGroupState == -1)
			{
				clusterGroupState = Marshal.GetLastWin32Error();
				if (NativeMethods.ErrorCode.GroupNotAvailable.IsEqual(clusterGroupState) || NativeMethods.ErrorCode.GroupNotFound.IsEqual(clusterGroupState))
				{
					throw new ClusterObjectNotFoundException(groupName, new Win32Exception(clusterGroupState));
				}
				throw new Win32Exception(clusterGroupState);
			}
			nodeId = clusApiAdapter.nodes.GetNodeIdFromName(stringBuilder.ToString());
		}

		private void ExecuteOnCommonPropertiesSet(string groupName, SafeClusterGroupHandle groupHandle, Action<SafeClusterPropertyListHandle> commonPropList)
		{
			clusApiAdapter.ExecuteOnPropertiesSet(groupName, groupHandle, NativeMethods.CLUSCTL_GROUP_SET_COMMON_PROPERTIES, commonPropList);
		}

		private void ExecuteOnPrivatePropertiesSet(string groupName, SafeClusterGroupHandle groupHandle, Action<SafeClusterPropertyListHandle> privatePropList)
		{
			clusApiAdapter.ExecuteOnPropertiesSet(groupName, groupHandle, NativeMethods.CLUSCTL_GROUP_SET_PRIVATE_PROPERTIES, privatePropList);
		}

		private static void ExecuteOnControlCode(SafeClusterGroupHandle groupHandle, int controlCode, string groupName, Action<IntPtr, int> controlCodeCallBack, IntPtr inBuffer, int inBufferSize = 0)
		{
			IntPtr intPtr = NativeMethods.Alloc(4096);
			int outBufferSize = 4096;
			int bytesReturned = 0;
			try
			{
				int num = NativeMethods.ClusterGroupControl(groupHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, inBuffer, inBufferSize, intPtr, outBufferSize, ref bytesReturned);
				if (num == NativeMethods.ErrorCode.MoreData.ToInt())
				{
					intPtr = NativeMethods.ReAlloc(intPtr, bytesReturned);
					outBufferSize = bytesReturned;
					num = NativeMethods.ClusterGroupControl(groupHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, inBuffer, inBufferSize, intPtr, outBufferSize, ref bytesReturned);
				}
				if (NativeMethods.ErrorCode.DeletePending.IsEqual(num))
				{
					throw new ClusterObjectDeletingException();
				}
				if (NativeMethods.ErrorCode.GroupNotAvailable.IsEqual(num) || NativeMethods.ErrorCode.GroupNotFound.IsEqual(num))
				{
					throw new ClusterObjectNotFoundException(groupName, new Win32Exception(num));
				}
				if (!NativeMethods.ErrorCode.None.IsEqual(num) && !NativeMethods.ErrorCode.IOPending.IsEqual(num))
				{
					throw new ClusterControlCodeException(controlCode, new Win32Exception(num));
				}
				controlCodeCallBack(intPtr, bytesReturned);
			}
			finally
			{
				NativeMethods.Free(intPtr);
			}
		}

		private string GetGroupName(Guid groupId, SafeClusterGroupHandle groupHandle)
		{
			string groupName = null;
			ExecuteOnReadOnlyCommonProperties(groupHandle, groupId.ToString(), delegate(IntPtr commonPropList, int commonPropListSize)
			{
				int num = NativeMethods.ResUtilFindSzProperty(commonPropList, commonPropListSize, "Name", ref groupName);
				if (num != NativeMethods.ErrorCode.None.ToInt())
				{
					throw new ClusterPropertyNotFoundException(groupId, "Name", typeof(FailoverClusters.Framework.Group), num);
				}
			});
			return groupName;
		}

		internal Guid GetGroupIdFromName(string groupName)
		{
			bool renamed;
			return GetGroupIdFromName(groupName, out renamed);
		}

		private Guid GetGroupIdFromName(string groupName, out bool renamed)
		{
			renamed = false;
			if (!clusApiAdapter.MappingNameIdGroup.TryGetValue(groupName, out var groupGuid))
			{
				lock (loadingGroupsLock)
				{
					if (!clusApiAdapter.MappingNameIdGroup.TryGetValue(groupName, out groupGuid))
					{
						ExecuteOnGroup(Guid.Empty, groupName, delegate(SafeClusterGroupHandle groupHandle)
						{
							groupGuid = GetId(groupHandle);
						});
						if (clusApiAdapter.MappingIdNameGroup.ContainsKey(groupGuid))
						{
							clusApiAdapter.MappingIdNameGroup.TryRemove(groupGuid, out var value2);
							clusApiAdapter.MappingNameIdGroup.TryRemove(value2, out var _);
							renamed = true;
						}
						clusApiAdapter.MappingIdNameGroup.AddOrUpdate(groupGuid, groupName, (Guid key, string value) => groupName);
						clusApiAdapter.MappingNameIdGroup.AddOrUpdate(groupName, groupGuid, (string key, Guid value) => groupGuid);
					}
				}
			}
			return groupGuid;
		}

		private string GetGroupNameFromId(Guid groupId)
		{
			if (!clusApiAdapter.MappingIdNameGroup.TryGetValue(groupId, out var groupName))
			{
				lock (loadingGroupsLock)
				{
					if (!clusApiAdapter.MappingIdNameGroup.TryGetValue(groupId, out groupName))
					{
						ExecuteOnGroup(groupId, null, delegate(SafeClusterGroupHandle groupHandle)
						{
							groupName = GetGroupName(groupId, groupHandle);
						});
						lock (loadingGroupsLock)
						{
							if (clusApiAdapter.MappingIdNameGroup.ContainsKey(groupId))
							{
								clusApiAdapter.MappingIdNameGroup.TryRemove(groupId, out var value2);
								clusApiAdapter.MappingNameIdGroup.TryRemove(value2, out var _);
							}
							clusApiAdapter.MappingIdNameGroup.AddOrUpdate(groupId, groupName, (Guid key, string value) => groupName);
							clusApiAdapter.MappingNameIdGroup.AddOrUpdate(groupName, groupId, (string key, Guid value) => groupId);
						}
					}
				}
			}
			return groupName;
		}

		private Guid GetId(SafeClusterGroupHandle groupHandle)
		{
			Guid guid = Guid.Empty;
			ExecuteOnControlCode(groupHandle, NativeMethods.CLUSCTL_GROUP_GET_ID, guid.ToString(), delegate(IntPtr buffer, int bufferSize)
			{
				string g = Marshal.PtrToStringUni(buffer);
				guid = new Guid(g);
			}, IntPtr.Zero);
			return guid;
		}

		public void MigrateVirtualMachine(PVirtualMachineGroup group, PNode node, VirtualMachineMigrationType migrationType, bool overrideLockState = false)
		{
			if (node != null)
			{
				clusApiAdapter.nodes.ExecuteOnNode(Guid.Empty, node.Name, delegate(SafeClusterNodeHandle nodeHandle)
				{
					MigrateVirtualMachine(group, nodeHandle, migrationType, overrideLockState);
				});
			}
			else
			{
				MigrateVirtualMachine(group, SafeClusterNodeHandle.InvalidHandle, migrationType, overrideLockState);
			}
		}
	}

	private class NetworkAdapter : AdapterBase, IConnectionAdapterNetwork, INotificationHandler
	{
		private readonly ClusApiAdapter clusApiAdapter;

		private readonly object loadingNetworksLock = new object();

		public NetworkAdapter(ClusApiAdapter clusApiAdapter)
			: base(clusApiAdapter)
		{
			this.clusApiAdapter = clusApiAdapter;
			Init();
		}

		private void Init()
		{
		}

		public IEnumerable<PNetwork> GetAll(bool nullElementOnError)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				yield break;
			}
			SafeClusterEnumHandle enumHandle = NativeMethods.ClusterOpenEnumEx(handle, NativeMethods.ClusterEnumType.Network, IntPtr.Zero);
			if (enumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateNetworkException>(Marshal.GetLastWin32Error());
			}
			IntPtr allocatedMemory = IntPtr.Zero;
			try
			{
				int records = NativeMethods.ClusterGetEnumCountEx(enumHandle);
				for (int index = 0; index < records; index++)
				{
					PNetwork pNetwork;
					try
					{
						int enumItemSize = 200;
						allocatedMemory = NativeMethods.Alloc(enumItemSize);
						SetStructVersion1(allocatedMemory);
						int num = NativeMethods.ClusterEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
						if (num == NativeMethods.ErrorCode.MoreData.ToInt())
						{
							allocatedMemory = NativeMethods.ReAlloc(allocatedMemory, enumItemSize);
							SetStructVersion1(allocatedMemory);
							num = NativeMethods.ClusterEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
						}
						if (num != NativeMethods.ErrorCode.None.ToInt())
						{
							throw ExceptionHelper.Build<ClusterIterateNetworksException>(Marshal.GetLastWin32Error());
						}
						NativeMethods.CLUSTER_ENUM_ITEM cLUSTER_ENUM_ITEM = (NativeMethods.CLUSTER_ENUM_ITEM)Marshal.PtrToStructure(allocatedMemory, typeof(NativeMethods.CLUSTER_ENUM_ITEM));
						allocatedMemory = NativeMethods.Free(allocatedMemory);
						Guid networkIdFromName = GetNetworkIdFromName(cLUSTER_ENUM_ITEM.lpszName);
						pNetwork = new PNetwork(clusApiAdapter.clusterAdapter.Cluster, networkIdFromName, cLUSTER_ENUM_ITEM.lpszName);
						lock (loadingNetworksLock)
						{
							clusApiAdapter.MappingIdNameNetwork.TryAdd(pNetwork.Id, pNetwork.Name);
							clusApiAdapter.MappingNameIdNetwork.TryAdd(pNetwork.Name, pNetwork.Id);
						}
					}
					catch (ClusterException exception)
					{
						if (!nullElementOnError)
						{
							throw;
						}
						ClusterLog.LogException(exception, "There was an error when getting network information from the cluster, however is not critical and the process can continue");
						pNetwork = null;
					}
					yield return pNetwork;
				}
			}
			finally
			{
				enumHandle.Dispose();
				NativeMethods.Free(allocatedMemory);
			}
		}

		public IEnumerable<TResult> Select<TResult>(QueryInfo queryInfo)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				yield break;
			}
			List<string> fieldNames = queryInfo.ProjectionFields.Select((ClusterObjectMetaDataMember s) => s.Name).Concat(queryInfo.WhereFields.Select((ClusterObjectMetaDataMember s) => s.Name)).Concat(from s in queryInfo.OrderBy.ConvertAll((OrderByItem item) => item.DataMember)
				select s.Name)
				.Distinct()
				.ToList();
			List<string> queryFields = fieldNames.Where((string s) => s.ToLowerInvariant() != "name" && s.ToLowerInvariant() != "id").ToList();
			SafeClusterEnumHandle enumHandle = NativeMethods.ClusterOpenEnumEx(handle, NativeMethods.ClusterEnumType.Network, IntPtr.Zero);
			if (enumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateNetworkException>(Marshal.GetLastWin32Error());
			}
			IntPtr allocatedMemory = IntPtr.Zero;
			try
			{
				int records = NativeMethods.ClusterGetEnumCountEx(enumHandle);
				for (int index = 0; index < records; index++)
				{
					if (queryInfo.IsCancel)
					{
						break;
					}
					int enumItemSize = 200;
					allocatedMemory = NativeMethods.Alloc(enumItemSize);
					SetStructVersion1(allocatedMemory);
					int num = NativeMethods.ClusterEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
					if (num == NativeMethods.ErrorCode.MoreData.ToInt())
					{
						allocatedMemory = NativeMethods.ReAlloc(allocatedMemory, enumItemSize);
						SetStructVersion1(allocatedMemory);
						num = NativeMethods.ClusterEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
					}
					if (num != NativeMethods.ErrorCode.None.ToInt())
					{
						throw ExceptionHelper.Build<ClusterIterateNetworksException>(Marshal.GetLastWin32Error());
					}
					NativeMethods.CLUSTER_ENUM_ITEM cLUSTER_ENUM_ITEM = (NativeMethods.CLUSTER_ENUM_ITEM)Marshal.PtrToStructure(allocatedMemory, typeof(NativeMethods.CLUSTER_ENUM_ITEM));
					allocatedMemory = NativeMethods.Free(allocatedMemory);
					Guid networkIdFromName = GetNetworkIdFromName(cLUSTER_ENUM_ITEM.lpszName);
					PNetwork privateNetwork = new PNetwork(clusApiAdapter.clusterAdapter.Cluster, networkIdFromName, cLUSTER_ENUM_ITEM.lpszName);
					lock (loadingNetworksLock)
					{
						clusApiAdapter.MappingIdNameNetwork.TryAdd(privateNetwork.Id, privateNetwork.Name);
						clusApiAdapter.MappingNameIdNetwork.TryAdd(privateNetwork.Name, privateNetwork.Id);
					}
					if (queryFields.Any())
					{
						NetworkLoadSelection loadSelection = NetworkLoadSelection.None;
						foreach (string item in fieldNames)
						{
							string text = item.ToLowerInvariant();
							if (text == "state")
							{
								loadSelection |= NetworkLoadSelection.Basic;
								continue;
							}
							if (item.Equals("commonproperties"))
							{
								loadSelection |= NetworkLoadSelection.CommonProperties;
							}
							if (item.Equals("privateproperties"))
							{
								loadSelection |= NetworkLoadSelection.PrivateProperties;
							}
						}
						ExecuteOnNetwork(privateNetwork.Id, privateNetwork.Name, delegate(SafeClusterNetworkHandle networkHandle)
						{
							if ((loadSelection & NetworkLoadSelection.Basic) == NetworkLoadSelection.Basic)
							{
								SetState(privateNetwork, networkHandle);
								privateNetwork.LoadedSelection |= 1;
							}
							if ((loadSelection & NetworkLoadSelection.CommonProperties) == NetworkLoadSelection.CommonProperties)
							{
								ExecuteOnCommonProperties(networkHandle, privateNetwork.Name, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(privateNetwork.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
								});
								ExecuteOnReadOnlyCommonProperties(networkHandle, privateNetwork.Name, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(privateNetwork.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
								});
								privateNetwork.LoadedSelection |= 2;
							}
							if ((loadSelection & NetworkLoadSelection.PrivateProperties) == NetworkLoadSelection.PrivateProperties)
							{
								ExecuteOnPrivateProperties(networkHandle, privateNetwork.Name, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(privateNetwork.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
								});
								ExecuteOnReadOnlyPrivateProperties(networkHandle, privateNetwork.Name, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(privateNetwork.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
								});
								privateNetwork.LoadedSelection |= 4;
							}
						});
					}
					yield return (TResult)(object)privateNetwork;
				}
			}
			finally
			{
				enumHandle.Dispose();
				NativeMethods.Free(allocatedMemory);
			}
		}

		public PNetwork Open(Guid id)
		{
			PNetwork privateNetwork = null;
			ExecuteOnNetwork(id, null, delegate
			{
				string networkNameFromId = GetNetworkNameFromId(id);
				privateNetwork = new PNetwork(clusApiAdapter.clusterAdapter.Cluster, id, networkNameFromId);
			});
			return privateNetwork;
		}

		public PNetwork Open(string networkName)
		{
			PNetwork privateNetwork = null;
			ExecuteOnNetwork(Guid.Empty, networkName, delegate
			{
				Guid networkIdFromName = GetNetworkIdFromName(networkName);
				privateNetwork = new PNetwork(clusApiAdapter.clusterAdapter.Cluster, networkIdFromName, networkName);
			});
			return privateNetwork;
		}

		public void Rename(Guid id, string newName)
		{
			ExecuteOnNetwork(id, string.Empty, delegate(SafeClusterNetworkHandle networkHandle)
			{
				int num = NativeMethods.SetClusterNetworkName(networkHandle, newName);
				if (NativeMethods.ErrorCode.ErrorAlreadyExists.IsEqual(num))
				{
					throw new ClusterNetworkAlreadyExistException(newName);
				}
				if (!NativeMethods.ErrorCode.None.IsEqual(num) && !NativeMethods.ErrorCode.IOPending.IsEqual(num))
				{
					throw ExceptionHelper.Build(num);
				}
			});
		}

		public void Load(PNetwork network, NetworkLoadSelection loadSelection)
		{
			try
			{
				ExecuteOnNetwork(network.Id, network.Name, delegate(SafeClusterNetworkHandle networkHandle)
				{
					if ((loadSelection & NetworkLoadSelection.Basic) == NetworkLoadSelection.Basic && (network.LoadedSelection & 1) != 1)
					{
						network.LoadedSelection |= 1;
						SetState(network, networkHandle);
					}
					if ((loadSelection & NetworkLoadSelection.CommonProperties) == NetworkLoadSelection.CommonProperties && (network.LoadedSelection & 2) != 2)
					{
						network.LoadedSelection |= 2;
						ExecuteOnCommonProperties(networkHandle, network.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(network.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
						});
						ExecuteOnReadOnlyCommonProperties(networkHandle, network.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(network.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
						});
					}
					if ((loadSelection & NetworkLoadSelection.PrivateProperties) == NetworkLoadSelection.PrivateProperties && (network.LoadedSelection & 4) != 4)
					{
						network.LoadedSelection |= 4;
						ExecuteOnPrivateProperties(networkHandle, network.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(network.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
						});
						ExecuteOnReadOnlyPrivateProperties(networkHandle, network.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(network.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: true);
						});
					}
				});
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(network.Name, network.Id, innerException);
			}
		}

		private void SetState(PNetwork network, SafeClusterNetworkHandle networkHandle)
		{
			int clusterNetworkState = NativeMethods.GetClusterNetworkState(networkHandle);
			if (clusterNetworkState == -1)
			{
				clusterNetworkState = Marshal.GetLastWin32Error();
				if (NativeMethods.ErrorCode.NetworkNotAvailable.IsEqual(clusterNetworkState) || NativeMethods.ErrorCode.NetworkNotFound.IsEqual(clusterNetworkState))
				{
					throw new ClusterObjectNotFoundException(network.Name, new Win32Exception(clusterNetworkState));
				}
				throw new Win32Exception(clusterNetworkState);
			}
			network.State = (NetworkState)clusterNetworkState;
		}

		internal void ExecuteOnNetwork(Guid id, string name, Action<SafeClusterNetworkHandle> onNetworkCallBack)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				return;
			}
			if (id == Guid.Empty && name == null)
			{
				onNetworkCallBack(SafeClusterNetworkHandle.InvalidHandle);
				return;
			}
			SafeClusterNetworkHandle safeClusterNetworkHandle;
			ClusterAccessRights grantedAccess;
			if (id != Guid.Empty)
			{
				if (GetNetworkNameFromId(id) == null)
				{
					throw new ClusterObjectNotFoundException(name, id, typeof(Network));
				}
				safeClusterNetworkHandle = NativeMethods.OpenClusterNetworkEx(handle, id.ToString(), clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess);
			}
			else
			{
				safeClusterNetworkHandle = NativeMethods.OpenClusterNetworkEx(handle, name, clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess);
			}
			if (safeClusterNetworkHandle.IsInvalid)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == NativeMethods.ErrorCode.NetworkNotFound.ToInt())
				{
					throw new ClusterObjectNotFoundException(name, id, typeof(Network));
				}
				throw ExceptionHelper.ClusterObjectLoadFailedException(name, id, lastWin32Error);
			}
			try
			{
				onNetworkCallBack(safeClusterNetworkHandle);
			}
			finally
			{
				safeClusterNetworkHandle.Dispose();
			}
		}

		private void ExecuteOnPrivateProperties(SafeClusterNetworkHandle networkHandle, string networkName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(networkHandle, NativeMethods.CLUSCTL_NETWORK_GET_PRIVATE_PROPERTIES, networkName, commonPropList);
		}

		private void ExecuteOnReadOnlyPrivateProperties(SafeClusterNetworkHandle networkHandle, string networkName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(networkHandle, NativeMethods.CLUSCTL_NETWORK_GET_RO_PRIVATE_PROPERTIES, networkName, commonPropList);
		}

		private void ExecuteOnReadOnlyCommonProperties(SafeClusterNetworkHandle networkHandle, string networkName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(networkHandle, NativeMethods.CLUSCTL_NETWORK_GET_RO_COMMON_PROPERTIES, networkName, commonPropList);
		}

		private void ExecuteOnCommonProperties(SafeClusterNetworkHandle networkHandle, string networkName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(networkHandle, NativeMethods.CLUSCTL_NETWORK_GET_COMMON_PROPERTIES, networkName, commonPropList);
		}

		private void ExecuteOnProperties(SafeClusterNetworkHandle networkHandle, int controlCode, string networkName, Action<IntPtr, int> propertyList)
		{
			try
			{
				ExecuteOnControlCode(networkHandle, controlCode, networkName, propertyList);
			}
			catch (ClusterControlCodeException innerException)
			{
				throw new ClusterGetPropertiesFailedException(innerException);
			}
		}

		private string GetNetworkNameFromId(Guid networkId)
		{
			if (!clusApiAdapter.MappingIdNameNetwork.TryGetValue(networkId, out var value))
			{
				throw new ClusterObjectNotFoundException(null, networkId, typeof(Network));
			}
			return value;
		}

		private Guid GetNetworkIdFromName(string networkName)
		{
			if (!clusApiAdapter.MappingNameIdNetwork.TryGetValue(networkName, out var networkGuid))
			{
				lock (loadingNetworksLock)
				{
					if (!clusApiAdapter.MappingNameIdNetwork.TryGetValue(networkName, out networkGuid))
					{
						ExecuteOnNetwork(Guid.Empty, networkName, delegate(SafeClusterNetworkHandle networkHandle)
						{
							networkGuid = GetNetworkId(networkHandle, networkName);
						});
						lock (loadingNetworksLock)
						{
							clusApiAdapter.MappingIdNameNetwork.AddOrUpdate(networkGuid, networkName, (Guid key, string value) => networkName);
							clusApiAdapter.MappingNameIdNetwork.AddOrUpdate(networkName, networkGuid, (string key, Guid value) => networkGuid);
						}
					}
				}
			}
			return networkGuid;
		}

		private Guid GetNetworkId(SafeClusterNetworkHandle networkHandle, string networkName)
		{
			Guid guid = Guid.Empty;
			ExecuteOnControlCode(networkHandle, NativeMethods.CLUSCTL_NETWORK_GET_ID, networkName, delegate(IntPtr buffer, int bufferSize)
			{
				string g = Marshal.PtrToStringUni(buffer);
				guid = new Guid(g);
			});
			return guid;
		}

		private void ExecuteOnControlCode(SafeClusterNetworkHandle networkHandle, int controlCode, string networkName, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null)
		{
			ExecuteOnControlCode(networkHandle, controlCode, networkName, null, controlCodeCallBack, invalidFunctionCallback);
		}

		private void ExecuteOnControlCode(SafeClusterNetworkHandle networkHandle, int controlCode, string networkName, NativeMethods.UnmanagedBuffer inBuffer, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null)
		{
			IntPtr intPtr = NativeMethods.Alloc(4096);
			int outBufferSize = 4096;
			int bytesReturned = 0;
			try
			{
				int num = NativeMethods.ClusterNetworkControl(networkHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, inBuffer?.IntPtr ?? IntPtr.Zero, inBuffer?.Size ?? 0, intPtr, outBufferSize, ref bytesReturned);
				if (NativeMethods.ErrorCode.MoreData.IsEqual(num))
				{
					intPtr = NativeMethods.ReAlloc(intPtr, bytesReturned);
					outBufferSize = bytesReturned;
					num = NativeMethods.ClusterNetworkControl(networkHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, inBuffer?.IntPtr ?? IntPtr.Zero, inBuffer?.Size ?? 0, intPtr, outBufferSize, ref bytesReturned);
				}
				if (NativeMethods.ErrorCode.InvalidFunction.IsEqual(num) && invalidFunctionCallback != null)
				{
					invalidFunctionCallback();
					return;
				}
				if (NativeMethods.ErrorCode.DeletePending.IsEqual(num))
				{
					throw new ClusterObjectDeletingException();
				}
				if (NativeMethods.ErrorCode.NetworkNotAvailable.IsEqual(num) || NativeMethods.ErrorCode.NetworkNotFound.IsEqual(num))
				{
					throw new ClusterObjectNotFoundException(networkName, new Win32Exception(num));
				}
				if (!NativeMethods.ErrorCode.None.IsEqual(num))
				{
					throw new ClusterControlCodeException(controlCode, new Win32Exception(num));
				}
				controlCodeCallBack.SafeCall(intPtr, bytesReturned);
			}
			finally
			{
				NativeMethods.Free(intPtr);
			}
		}

		private unsafe void SetStructVersion1(IntPtr clusterEnumItemV2Group)
		{
			int* ptr = (int*)(void*)clusterEnumItemV2Group;
			*ptr = 1;
		}

		public bool NotificationArrived(NativeMethods.NOTIFY_FILTER_AND_TYPE filterType, string objectName, string objectId, string parentId, string objectType, IntPtr buffer, int bufferSize)
		{
			if (filterType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_CLUSTER && filterType.FilterFlags == 16)
			{
				Guid guid = new Guid(objectId);
				if (objectName.Length == 0)
				{
					ClusterLog.LogError("Empty network name, the network name in network add notification is empty");
				}
				lock (loadingNetworksLock)
				{
					clusApiAdapter.MappingIdNameNetwork.TryAdd(guid, objectName);
					clusApiAdapter.MappingNameIdNetwork.TryAdd(objectName, guid);
				}
				ClusterAddedEventArgs payload = new ClusterAddedEventArgs(guid, objectName, null, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster
				};
				clusApiAdapter.EnqueueNotification(new NetworkNotification(payload));
				return true;
			}
			if (filterType.ObjectType != NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NETWORK)
			{
				return false;
			}
			switch ((NativeMethods.CLUSTER_CHANGE_NETWORK_V2)filterType.FilterFlags)
			{
			case NativeMethods.CLUSTER_CHANGE_NETWORK_V2.CLUSTER_CHANGE_NETWORK_COMMON_PROPERTY_V2:
			{
				if (bufferSize == 0)
				{
					return true;
				}
				Guid networkId = new Guid(objectId);
				ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, networkId, ClusterIdentityType.Network)
				{
					Partial = true
				};
				AdapterBase.ParseProperties(clusterPropertyCollection, buffer, bufferSize, ClusterPropertyKind.Common, readOnly: false);
				clusterPropertyCollection.Get("Name", delegate(ClusterPropertyString newName)
				{
					clusApiAdapter.EnqueueNotification(new NetworkNotification(new ClusterRenamedEventArgs(networkId, newName.TypedValue, null)));
				});
				clusApiAdapter.EnqueueNotification(new NetworkNotification(new ClusterPropertiesEventArgs(networkId, objectName, null, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster,
					Properties = clusterPropertyCollection
				}));
				return true;
			}
			case NativeMethods.CLUSTER_CHANGE_NETWORK_V2.CLUSTER_CHANGE_NETWORK_STATE_V2:
			{
				NetworkState value3 = (NetworkState)Marshal.ReadInt32(buffer);
				Guid id = new Guid(objectId);
				clusApiAdapter.EnqueueNotification(new NetworkNotification(new ClusterNetworkStateEventArgs(id, value3, null)));
				return true;
			}
			case NativeMethods.CLUSTER_CHANGE_NETWORK_V2.CLUSTER_CHANGE_NETWORK_DELETED_V2:
			{
				Guid guid2 = new Guid(objectId);
				lock (loadingNetworksLock)
				{
					clusApiAdapter.MappingIdNameNetwork.TryRemove(guid2, out var _);
					clusApiAdapter.MappingNameIdNetwork.TryRemove(objectName, out var _);
				}
				clusApiAdapter.EnqueueNotification(new NetworkNotification(new ClusterRemovedEventArgs(guid2, objectName, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster
				}));
				return true;
			}
			default:
				return false;
			}
		}

		public void Collect()
		{
		}
	}

	private class NetworkInterfaceAdapter : AdapterBase, IConnectionAdapterNetworkInterface, INotificationHandler
	{
		private readonly ClusApiAdapter clusApiAdapter;

		private readonly object loadingNetworkInterfacesLock = new object();

		public NetworkInterfaceAdapter(ClusApiAdapter clusApiAdapter)
			: base(clusApiAdapter)
		{
			this.clusApiAdapter = clusApiAdapter;
			Init();
		}

		private void Init()
		{
		}

		public IEnumerable<TResult> Select<TResult>(QueryInfo queryInfo)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				yield break;
			}
			List<string> fieldNames = queryInfo.ProjectionFields.Select((ClusterObjectMetaDataMember s) => s.Name).Concat(queryInfo.WhereFields.Select((ClusterObjectMetaDataMember s) => s.Name)).Concat(from s in queryInfo.OrderBy.ConvertAll((OrderByItem item) => item.DataMember)
				select s.Name)
				.Distinct()
				.ToList();
			List<string> queryFields = fieldNames.Where((string s) => s.ToLowerInvariant() != "name" && s.ToLowerInvariant() != "id").ToList();
			SafeClusterEnumHandle enumHandle = NativeMethods.ClusterOpenEnumEx(handle, NativeMethods.ClusterEnumType.NetworkInterface, IntPtr.Zero);
			if (enumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateNetworkInterfaceException>(Marshal.GetLastWin32Error());
			}
			IntPtr allocatedMemory = IntPtr.Zero;
			try
			{
				int records = NativeMethods.ClusterGetEnumCountEx(enumHandle);
				for (int index = 0; index < records; index++)
				{
					if (queryInfo.IsCancel)
					{
						break;
					}
					int enumItemSize = 200;
					allocatedMemory = NativeMethods.Alloc(enumItemSize);
					SetStructVersion1(allocatedMemory);
					int num = NativeMethods.ClusterEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
					if (num == NativeMethods.ErrorCode.MoreData.ToInt())
					{
						allocatedMemory = NativeMethods.ReAlloc(allocatedMemory, enumItemSize);
						SetStructVersion1(allocatedMemory);
						num = NativeMethods.ClusterEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
					}
					if (num != NativeMethods.ErrorCode.None.ToInt())
					{
						throw ExceptionHelper.Build<ClusterIterateNetworkInterfacesException>(Marshal.GetLastWin32Error());
					}
					NativeMethods.CLUSTER_ENUM_ITEM cLUSTER_ENUM_ITEM = (NativeMethods.CLUSTER_ENUM_ITEM)Marshal.PtrToStructure(allocatedMemory, typeof(NativeMethods.CLUSTER_ENUM_ITEM));
					allocatedMemory = NativeMethods.Free(allocatedMemory);
					PNetworkInterface privateNetworkInterface = new PNetworkInterface(id: new Guid(cLUSTER_ENUM_ITEM.lpszId), cluster: clusApiAdapter.clusterAdapter.Cluster, name: cLUSTER_ENUM_ITEM.lpszName);
					lock (loadingNetworkInterfacesLock)
					{
						clusApiAdapter.MappingIdNameNetworkInterface.AddOrUpdate(privateNetworkInterface.Id, privateNetworkInterface.Name, (Guid key, string value) => privateNetworkInterface.Name);
						clusApiAdapter.MappingNameIdNetworkInterface.AddOrUpdate(privateNetworkInterface.Name, privateNetworkInterface.Id, (string key, Guid value) => privateNetworkInterface.Id);
					}
					if (queryFields.Any())
					{
						NetworkInterfaceLoadSelection loadSelection = NetworkInterfaceLoadSelection.None;
						foreach (string item in fieldNames)
						{
							string text = item.ToLowerInvariant();
							if (text == "state")
							{
								loadSelection |= NetworkInterfaceLoadSelection.Basic;
								continue;
							}
							if (item.Equals("commonproperties"))
							{
								loadSelection |= NetworkInterfaceLoadSelection.CommonProperties;
							}
							if (item.Equals("privateproperties"))
							{
								loadSelection |= NetworkInterfaceLoadSelection.PrivateProperties;
							}
						}
						ExecuteOnNetworkInterface(privateNetworkInterface.Id, privateNetworkInterface.Name, delegate(SafeClusterNetworkInterfaceHandle networkInterfaceHandle)
						{
							if ((loadSelection & NetworkInterfaceLoadSelection.Basic) == NetworkInterfaceLoadSelection.Basic)
							{
								SetState(privateNetworkInterface, networkInterfaceHandle);
								privateNetworkInterface.LoadedSelection |= 1;
							}
							if ((loadSelection & NetworkInterfaceLoadSelection.CommonProperties) == NetworkInterfaceLoadSelection.CommonProperties)
							{
								ExecuteOnCommonProperties(networkInterfaceHandle, privateNetworkInterface.Name, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(privateNetworkInterface.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
								});
								ExecuteOnReadOnlyCommonProperties(networkInterfaceHandle, privateNetworkInterface.Name, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(privateNetworkInterface.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
								});
								privateNetworkInterface.LoadedSelection |= 2;
							}
							if ((loadSelection & NetworkInterfaceLoadSelection.PrivateProperties) == NetworkInterfaceLoadSelection.PrivateProperties)
							{
								ExecuteOnPrivateProperties(networkInterfaceHandle, privateNetworkInterface.Name, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(privateNetworkInterface.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
								});
								ExecuteOnReadOnlyPrivateProperties(networkInterfaceHandle, privateNetworkInterface.Name, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(privateNetworkInterface.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
								});
								privateNetworkInterface.LoadedSelection |= 4;
							}
						});
					}
					yield return (TResult)(object)privateNetworkInterface;
				}
			}
			finally
			{
				enumHandle.Dispose();
				NativeMethods.Free(allocatedMemory);
			}
		}

		public PNetworkInterface Open(Guid id)
		{
			PNetworkInterface privateNetworkInterface = null;
			ExecuteOnNetworkInterface(id, null, delegate
			{
				string networkInterfaceNameFromId = GetNetworkInterfaceNameFromId(id);
				privateNetworkInterface = new PNetworkInterface(clusApiAdapter.clusterAdapter.Cluster, id, networkInterfaceNameFromId);
			});
			return privateNetworkInterface;
		}

		public PNetworkInterface Open(string networkInterfaceName)
		{
			PNetworkInterface privateNetworkInterface = null;
			ExecuteOnNetworkInterface(Guid.Empty, networkInterfaceName, delegate
			{
				Guid networkInterfaceIdFromName = GetNetworkInterfaceIdFromName(networkInterfaceName);
				privateNetworkInterface = new PNetworkInterface(clusApiAdapter.clusterAdapter.Cluster, networkInterfaceIdFromName, networkInterfaceName);
			});
			return privateNetworkInterface;
		}

		public void Load(PNetworkInterface networkInterface, NetworkInterfaceLoadSelection loadSelection)
		{
			try
			{
				ExecuteOnNetworkInterface(networkInterface.Id, networkInterface.Name, delegate(SafeClusterNetworkInterfaceHandle networkInterfaceHandle)
				{
					if ((loadSelection & NetworkInterfaceLoadSelection.Basic) == NetworkInterfaceLoadSelection.Basic && (networkInterface.LoadedSelection & 1) != 1)
					{
						networkInterface.LoadedSelection |= 1;
						SetState(networkInterface, networkInterfaceHandle);
					}
					if ((loadSelection & NetworkInterfaceLoadSelection.CommonProperties) == NetworkInterfaceLoadSelection.CommonProperties && (networkInterface.LoadedSelection & 2) != 2)
					{
						networkInterface.LoadedSelection |= 2;
						ExecuteOnCommonProperties(networkInterfaceHandle, networkInterface.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(networkInterface.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
						});
						ExecuteOnReadOnlyCommonProperties(networkInterfaceHandle, networkInterface.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(networkInterface.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
						});
					}
					if ((loadSelection & NetworkInterfaceLoadSelection.PrivateProperties) == NetworkInterfaceLoadSelection.PrivateProperties && (networkInterface.LoadedSelection & 4) != 4)
					{
						networkInterface.LoadedSelection |= 4;
						ExecuteOnPrivateProperties(networkInterfaceHandle, networkInterface.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(networkInterface.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
						});
						ExecuteOnReadOnlyPrivateProperties(networkInterfaceHandle, networkInterface.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(networkInterface.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: true);
						});
					}
				});
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(networkInterface.Name, networkInterface.Id, innerException);
			}
		}

		private void SetState(PNetworkInterface networkInterface, SafeClusterNetworkInterfaceHandle networkInterfaceHandle)
		{
			int clusterNetInterfaceState = NativeMethods.GetClusterNetInterfaceState(networkInterfaceHandle);
			if (clusterNetInterfaceState == -1)
			{
				clusterNetInterfaceState = Marshal.GetLastWin32Error();
				if (NativeMethods.ErrorCode.NetworkInterfaceNotFound.IsEqual(clusterNetInterfaceState))
				{
					throw new ClusterObjectNotFoundException(networkInterface.Name, new Win32Exception(clusterNetInterfaceState));
				}
				throw new Win32Exception(clusterNetInterfaceState);
			}
			networkInterface.State = (NetworkInterfaceState)clusterNetInterfaceState;
		}

		private void ExecuteOnNetworkInterface(Guid id, string name, Action<SafeClusterNetworkInterfaceHandle> onNetworkInterfaceCallBack)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				return;
			}
			if (id == Guid.Empty && name == null)
			{
				onNetworkInterfaceCallBack(SafeClusterNetworkInterfaceHandle.InvalidHandle);
				return;
			}
			SafeClusterNetworkInterfaceHandle safeClusterNetworkInterfaceHandle;
			ClusterAccessRights grantedAccess;
			if (id != Guid.Empty)
			{
				if (GetNetworkInterfaceNameFromId(id) == null)
				{
					throw new ClusterObjectNotFoundException(name, id, typeof(NetworkInterface));
				}
				safeClusterNetworkInterfaceHandle = NativeMethods.OpenClusterNetInterfaceEx(handle, id.ToString(), clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess);
			}
			else
			{
				safeClusterNetworkInterfaceHandle = NativeMethods.OpenClusterNetInterfaceEx(handle, name, clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess);
			}
			if (safeClusterNetworkInterfaceHandle.IsInvalid)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == NativeMethods.ErrorCode.NetworkInterfaceNotFound.ToInt())
				{
					throw new ClusterObjectNotFoundException(name, id, typeof(NetworkInterface));
				}
				throw ExceptionHelper.ClusterObjectLoadFailedException(name, id, lastWin32Error);
			}
			try
			{
				onNetworkInterfaceCallBack(safeClusterNetworkInterfaceHandle);
			}
			finally
			{
				safeClusterNetworkInterfaceHandle.Dispose();
			}
		}

		private void ExecuteOnPrivateProperties(SafeClusterNetworkInterfaceHandle networkInterfaceHandle, string networkInterfaceName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(networkInterfaceHandle, NativeMethods.CLUSCTL_NETINTERFACE_GET_PRIVATE_PROPERTIES, networkInterfaceName, commonPropList);
		}

		private void ExecuteOnReadOnlyPrivateProperties(SafeClusterNetworkInterfaceHandle networkInterfaceHandle, string networkInterfaceName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(networkInterfaceHandle, NativeMethods.CLUSCTL_NETINTERFACE_GET_RO_PRIVATE_PROPERTIES, networkInterfaceName, commonPropList);
		}

		private void ExecuteOnReadOnlyCommonProperties(SafeClusterNetworkInterfaceHandle networkInterfaceHandle, string networkInterfaceName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(networkInterfaceHandle, NativeMethods.CLUSCTL_NETINTERFACE_GET_RO_COMMON_PROPERTIES, networkInterfaceName, commonPropList);
		}

		private void ExecuteOnCommonProperties(SafeClusterNetworkInterfaceHandle networkInterfaceHandle, string networkInterfaceName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(networkInterfaceHandle, NativeMethods.CLUSCTL_NETINTERFACE_GET_COMMON_PROPERTIES, networkInterfaceName, commonPropList);
		}

		private void ExecuteOnProperties(SafeClusterNetworkInterfaceHandle networkInterfaceHandle, int controlCode, string networkInterfaceName, Action<IntPtr, int> propertyList)
		{
			try
			{
				ExecuteOnControlCode(networkInterfaceHandle, controlCode, networkInterfaceName, propertyList);
			}
			catch (ClusterControlCodeException innerException)
			{
				throw new ClusterGetPropertiesFailedException(innerException);
			}
		}

		private string GetNetworkInterfaceNameFromId(Guid networkInterfaceId)
		{
			if (!clusApiAdapter.MappingIdNameNetworkInterface.TryGetValue(networkInterfaceId, out var value))
			{
				throw new ClusterObjectNotFoundException(null, networkInterfaceId, typeof(NetworkInterface));
			}
			return value;
		}

		private Guid GetNetworkInterfaceIdFromName(string networkInterfaceName)
		{
			if (!clusApiAdapter.MappingNameIdNetworkInterface.TryGetValue(networkInterfaceName, out var networkInterfaceGuid))
			{
				lock (loadingNetworkInterfacesLock)
				{
					if (!clusApiAdapter.MappingNameIdNetworkInterface.TryGetValue(networkInterfaceName, out networkInterfaceGuid))
					{
						ExecuteOnNetworkInterface(Guid.Empty, networkInterfaceName, delegate(SafeClusterNetworkInterfaceHandle networkInterfaceHandle)
						{
							networkInterfaceGuid = GetNetworkInterfaceId(networkInterfaceHandle, networkInterfaceName);
						});
						lock (loadingNetworkInterfacesLock)
						{
							clusApiAdapter.MappingIdNameNetworkInterface.AddOrUpdate(networkInterfaceGuid, networkInterfaceName, (Guid key, string value) => networkInterfaceName);
							clusApiAdapter.MappingNameIdNetworkInterface.AddOrUpdate(networkInterfaceName, networkInterfaceGuid, (string key, Guid value) => networkInterfaceGuid);
						}
					}
				}
			}
			return networkInterfaceGuid;
		}

		private Guid GetNetworkInterfaceId(SafeClusterNetworkInterfaceHandle networkInterfaceHandle, string networkInterfaceName)
		{
			Guid guid = Guid.Empty;
			ExecuteOnControlCode(networkInterfaceHandle, NativeMethods.CLUSCTL_NETINTERFACE_GET_ID, networkInterfaceName, delegate(IntPtr buffer, int bufferSize)
			{
				string text = Marshal.PtrToStringUni(buffer);
				if (text == null)
				{
					throw new NullReferenceException("Buffer contains an invalid Id");
				}
				guid = new Guid(text);
			});
			return guid;
		}

		private void ExecuteOnControlCode(SafeClusterNetworkInterfaceHandle networkInterfaceHandle, int controlCode, string networkInterfaceName, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null)
		{
			ExecuteOnControlCode(networkInterfaceHandle, controlCode, networkInterfaceName, null, controlCodeCallBack, invalidFunctionCallback);
		}

		private void ExecuteOnControlCode(SafeClusterNetworkInterfaceHandle networkInterfaceHandle, int controlCode, string networkInterfaceName, NativeMethods.UnmanagedBuffer inBuffer, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null)
		{
			IntPtr intPtr = NativeMethods.Alloc(4096);
			int outBufferSize = 4096;
			int bytesReturned = 0;
			try
			{
				int num = NativeMethods.ClusterNetInterfaceControl(networkInterfaceHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, inBuffer?.IntPtr ?? IntPtr.Zero, inBuffer?.Size ?? 0, intPtr, outBufferSize, ref bytesReturned);
				if (NativeMethods.ErrorCode.MoreData.IsEqual(num))
				{
					intPtr = NativeMethods.ReAlloc(intPtr, bytesReturned);
					outBufferSize = bytesReturned;
					num = NativeMethods.ClusterNetInterfaceControl(networkInterfaceHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, inBuffer?.IntPtr ?? IntPtr.Zero, inBuffer?.Size ?? 0, intPtr, outBufferSize, ref bytesReturned);
				}
				if (NativeMethods.ErrorCode.InvalidFunction.IsEqual(num) && invalidFunctionCallback != null)
				{
					invalidFunctionCallback();
					return;
				}
				if (NativeMethods.ErrorCode.DeletePending.IsEqual(num))
				{
					throw new ClusterObjectDeletingException();
				}
				if (NativeMethods.ErrorCode.NetworkInterfaceNotFound.IsEqual(num))
				{
					throw new ClusterObjectNotFoundException(networkInterfaceName, new Win32Exception(num));
				}
				if (!NativeMethods.ErrorCode.None.IsEqual(num))
				{
					throw new ClusterControlCodeException(controlCode, new Win32Exception(num));
				}
				controlCodeCallBack.SafeCall(intPtr, bytesReturned);
			}
			finally
			{
				NativeMethods.Free(intPtr);
			}
		}

		private unsafe void SetStructVersion1(IntPtr clusterEnumItemV2Group)
		{
			int* ptr = (int*)(void*)clusterEnumItemV2Group;
			*ptr = 1;
		}

		public bool NotificationArrived(NativeMethods.NOTIFY_FILTER_AND_TYPE filterType, string objectName, string objectId, string parentId, string objectType, IntPtr buffer, int bufferSize)
		{
			if (filterType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NODE && filterType.FilterFlags == 1)
			{
				Guid guid = new Guid(objectId);
				if (objectName.Length == 0)
				{
					ClusterLog.LogError("Empty network interface name, the network interface name in network add notification is empty");
				}
				lock (loadingNetworkInterfacesLock)
				{
					clusApiAdapter.MappingIdNameNetworkInterface.TryAdd(guid, objectName);
					clusApiAdapter.MappingNameIdNetworkInterface.TryAdd(objectName, guid);
				}
				ClusterAddedEventArgs payload = new ClusterAddedEventArgs(guid, objectName, null, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster
				};
				clusApiAdapter.EnqueueNotification(new NetworkInterfaceNotification(payload));
				return true;
			}
			if (filterType.ObjectType != NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NETWORK_INTERFACE)
			{
				return false;
			}
			switch ((NativeMethods.CLUSTER_CHANGE_NETINTERFACE_V2)filterType.FilterFlags)
			{
			case NativeMethods.CLUSTER_CHANGE_NETINTERFACE_V2.CLUSTER_CHANGE_NETINTERFACE_COMMON_PROPERTY_V2:
			{
				if (bufferSize == 0)
				{
					return true;
				}
				Guid networkInterfaceId = new Guid(objectId);
				ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, networkInterfaceId, ClusterIdentityType.NetworkInterface)
				{
					Partial = true
				};
				AdapterBase.ParseProperties(clusterPropertyCollection, buffer, bufferSize, ClusterPropertyKind.Common, readOnly: false);
				ClusterProperty clusterProperty = clusterPropertyCollection["Node"];
				if (clusterProperty != null && int.TryParse(clusterProperty.Value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
				{
					string nodeNameFromNodeNumber = base.ClusApiAdapter.nodes.GetNodeNameFromNodeNumber(result);
					clusterProperty.OverrideCurrentValue(nodeNameFromNodeNumber);
				}
				clusterPropertyCollection.Get("Name", delegate(ClusterPropertyString newName)
				{
					clusApiAdapter.EnqueueNotification(new NetworkInterfaceNotification(new ClusterRenamedEventArgs(networkInterfaceId, newName.TypedValue, null)));
				});
				clusApiAdapter.EnqueueNotification(new NetworkInterfaceNotification(new ClusterPropertiesEventArgs(networkInterfaceId, objectName, null, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster,
					Properties = clusterPropertyCollection
				}));
				return true;
			}
			case NativeMethods.CLUSTER_CHANGE_NETINTERFACE_V2.CLUSTER_CHANGE_NETINTERFACE_STATE_V2:
			{
				Guid id = new Guid(objectId);
				NetworkInterfaceState value3 = (NetworkInterfaceState)Marshal.ReadInt32(buffer);
				clusApiAdapter.EnqueueNotification(new NetworkInterfaceNotification(new ClusterNetworkInterfaceStateEventArgs(id, value3, null)));
				return true;
			}
			case NativeMethods.CLUSTER_CHANGE_NETINTERFACE_V2.CLUSTER_CHANGE_NETINTERFACE_DELETED_V2:
			{
				Guid guid2 = new Guid(objectId);
				lock (loadingNetworkInterfacesLock)
				{
					clusApiAdapter.MappingIdNameNetworkInterface.TryRemove(guid2, out var _);
					clusApiAdapter.MappingNameIdNetworkInterface.TryRemove(objectName, out var _);
				}
				clusApiAdapter.EnqueueNotification(new NetworkInterfaceNotification(new ClusterRemovedEventArgs(guid2, objectName, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster
				}));
				return true;
			}
			default:
				return false;
			}
		}

		public void Collect()
		{
		}

		public List<string> GetNodeDnsSuffixes(string nodeName)
		{
			throw new NotSupportedException("GetNodeDnsSuffixes is not implemented for ClusApiAdapter");
		}
	}

	private class NodeAdapter : AdapterBase, IConnectionAdapterNode, INotificationHandler
	{
		private readonly ClusApiAdapter clusApiAdapter;

		private readonly object loadingNodesLock = new object();

		private const uint NodeCleanupTimeoutInMiniSeconds = 300000u;

		public NodeAdapter(ClusApiAdapter clusApiAdapter)
			: base(clusApiAdapter)
		{
			this.clusApiAdapter = clusApiAdapter;
			Init();
		}

		private void Init()
		{
		}

		public IEnumerable<PNode> GetAll(bool nullElementOnError)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				yield break;
			}
			SafeClusterEnumHandle enumHandle = NativeMethods.ClusterOpenEnumEx(handle, NativeMethods.ClusterEnumType.Node, IntPtr.Zero);
			if (enumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateNodeException>(Marshal.GetLastWin32Error());
			}
			IntPtr allocatedMemory = IntPtr.Zero;
			try
			{
				int records = NativeMethods.ClusterGetEnumCountEx(enumHandle);
				for (int index = 0; index < records; index++)
				{
					PNode pNode;
					try
					{
						int enumItemSize = 200;
						allocatedMemory = NativeMethods.Alloc(enumItemSize);
						SetStructVersion1(allocatedMemory);
						int num = NativeMethods.ClusterEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
						if (num == NativeMethods.ErrorCode.MoreData.ToInt())
						{
							allocatedMemory = NativeMethods.ReAlloc(allocatedMemory, enumItemSize);
							SetStructVersion1(allocatedMemory);
							num = NativeMethods.ClusterEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
						}
						if (num != NativeMethods.ErrorCode.None.ToInt())
						{
							throw ExceptionHelper.Build<ClusterIterateNodesException>(Marshal.GetLastWin32Error());
						}
						NativeMethods.CLUSTER_ENUM_ITEM cLUSTER_ENUM_ITEM = (NativeMethods.CLUSTER_ENUM_ITEM)Marshal.PtrToStructure(allocatedMemory, typeof(NativeMethods.CLUSTER_ENUM_ITEM));
						allocatedMemory = NativeMethods.Free(allocatedMemory);
						Guid nodeIdFromName = GetNodeIdFromName(cLUSTER_ENUM_ITEM.lpszName);
						int num2 = int.Parse(cLUSTER_ENUM_ITEM.lpszId, NumberStyles.Integer, CultureInfo.InvariantCulture);
						pNode = new PNode(clusApiAdapter.clusterAdapter.Cluster, nodeIdFromName, cLUSTER_ENUM_ITEM.lpszName);
						lock (loadingNodesLock)
						{
							clusApiAdapter.MappingIdNameNode.TryAdd(pNode.Id, pNode.Name);
							clusApiAdapter.MappingNameIdNode.TryAdd(pNode.Name, pNode.Id);
							clusApiAdapter.MappingNumberNameNode.TryAdd(num2, pNode.Name);
							clusApiAdapter.MappingNameNumberNode.TryAdd(pNode.Name, num2);
						}
						Load(pNode, NodeLoadSelection.All);
					}
					catch (ClusterException exception)
					{
						if (!nullElementOnError)
						{
							throw;
						}
						ClusterLog.LogException(exception, "There was an error when getting node information from the cluster, however is not critical and the process can continue");
						pNode = null;
					}
					yield return pNode;
				}
			}
			finally
			{
				enumHandle.Dispose();
				NativeMethods.Free(allocatedMemory);
			}
		}

		public NodeOperatingSystemInformation GetOperatingSystemInformation(string nodeName)
		{
			NodeOperatingSystemInformation nodeOperatingSystemInformation = null;
			clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
			{
				nodeOperatingSystemInformation = wmiAdapter.Node.GetOperatingSystemInformation(nodeName);
			});
			return nodeOperatingSystemInformation;
		}

		public string GetDomainName(string nodeName)
		{
			string domainName = null;
			clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
			{
				domainName = wmiAdapter.Node.GetDomainName(nodeName);
			});
			return domainName;
		}

		public ServerInformation GetServerInformation(string nodeName)
		{
			ServerInformation serverInformation = null;
			clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
			{
				serverInformation = wmiAdapter.Node.GetServerInformation(nodeName);
			});
			return serverInformation;
		}

		public ProcessorInformation GetProcessorInformation(string nodeName)
		{
			ProcessorInformation processorInformation = null;
			clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
			{
				processorInformation = wmiAdapter.Node.GetProcessorInformation(nodeName);
			});
			return processorInformation;
		}

		public IEnumerable<TResult> Select<TResult>(QueryInfo queryInfo)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				yield break;
			}
			List<string> fieldNames = queryInfo.ProjectionFields.Select((ClusterObjectMetaDataMember s) => s.Name).Concat(queryInfo.WhereFields.Select((ClusterObjectMetaDataMember s) => s.Name)).Concat(from s in queryInfo.OrderBy.ConvertAll((OrderByItem item) => item.DataMember)
				select s.Name)
				.Distinct()
				.ToList();
			List<string> queryFields = fieldNames.Where((string s) => s.ToLowerInvariant() != "nodeName" && s.ToLowerInvariant() != "id").ToList();
			SafeClusterEnumHandle enumHandle = NativeMethods.ClusterOpenEnumEx(handle, NativeMethods.ClusterEnumType.Node, IntPtr.Zero);
			if (enumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateNodeException>(Marshal.GetLastWin32Error());
			}
			IntPtr allocatedMemory = IntPtr.Zero;
			try
			{
				int records = NativeMethods.ClusterGetEnumCountEx(enumHandle);
				for (int index = 0; index < records; index++)
				{
					if (queryInfo.IsCancel)
					{
						break;
					}
					int enumItemSize = 200;
					allocatedMemory = NativeMethods.Alloc(enumItemSize);
					SetStructVersion1(allocatedMemory);
					int num = NativeMethods.ClusterEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
					if (num == NativeMethods.ErrorCode.MoreData.ToInt())
					{
						allocatedMemory = NativeMethods.ReAlloc(allocatedMemory, enumItemSize);
						SetStructVersion1(allocatedMemory);
						num = NativeMethods.ClusterEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
					}
					if (num != NativeMethods.ErrorCode.None.ToInt())
					{
						throw ExceptionHelper.Build<ClusterIterateNodesException>(Marshal.GetLastWin32Error());
					}
					NativeMethods.CLUSTER_ENUM_ITEM cLUSTER_ENUM_ITEM = (NativeMethods.CLUSTER_ENUM_ITEM)Marshal.PtrToStructure(allocatedMemory, typeof(NativeMethods.CLUSTER_ENUM_ITEM));
					allocatedMemory = NativeMethods.Free(allocatedMemory);
					Guid nodeIdFromName = GetNodeIdFromName(cLUSTER_ENUM_ITEM.lpszName);
					int num2 = int.Parse(cLUSTER_ENUM_ITEM.lpszId, NumberStyles.Integer, CultureInfo.InvariantCulture);
					PNode privateNode = new PNode(clusApiAdapter.clusterAdapter.Cluster, nodeIdFromName, cLUSTER_ENUM_ITEM.lpszName);
					lock (loadingNodesLock)
					{
						clusApiAdapter.MappingIdNameNode.TryAdd(privateNode.Id, privateNode.Name);
						clusApiAdapter.MappingNameIdNode.TryAdd(privateNode.Name, privateNode.Id);
						clusApiAdapter.MappingNumberNameNode.TryAdd(num2, privateNode.Name);
						clusApiAdapter.MappingNameNumberNode.TryAdd(privateNode.Name, num2);
					}
					if (queryFields.Any())
					{
						NodeLoadSelection loadSelection = NodeLoadSelection.None;
						foreach (string item in fieldNames)
						{
							string text = item.ToLowerInvariant();
							if (text == "state")
							{
								loadSelection |= NodeLoadSelection.Basic;
								continue;
							}
							if (item.Equals("commonproperties"))
							{
								loadSelection |= NodeLoadSelection.CommonProperties;
							}
							if (item.Equals("privateproperties"))
							{
								loadSelection |= NodeLoadSelection.PrivateProperties;
							}
						}
						ExecuteOnNode(Guid.Empty, privateNode.Name, delegate(SafeClusterNodeHandle nodeHandle)
						{
							if ((loadSelection & NodeLoadSelection.Basic) == NodeLoadSelection.Basic)
							{
								SetState(privateNode, nodeHandle);
								privateNode.LoadedSelection |= 1;
							}
							if ((loadSelection & NodeLoadSelection.CommonProperties) == NodeLoadSelection.CommonProperties)
							{
								ExecuteOnCommonProperties(nodeHandle, privateNode.Name, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(privateNode.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
								});
								ExecuteOnReadOnlyCommonProperties(nodeHandle, privateNode.Name, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(privateNode.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
								});
								privateNode.LoadedSelection |= 2;
							}
							if ((loadSelection & NodeLoadSelection.PrivateProperties) == NodeLoadSelection.PrivateProperties)
							{
								ExecuteOnPrivateProperties(nodeHandle, privateNode.Name, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(privateNode.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
								});
								ExecuteOnReadOnlyPrivateProperties(nodeHandle, privateNode.Name, delegate(IntPtr propertyList, int propertyListSize)
								{
									AdapterBase.ParseProperties(privateNode.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
								});
								privateNode.LoadedSelection |= 4;
							}
						});
					}
					yield return (TResult)(object)privateNode;
				}
			}
			finally
			{
				enumHandle.Dispose();
				NativeMethods.Free(allocatedMemory);
			}
		}

		public bool WillOfflineLoseQuorum(string name)
		{
			throw new NotSupportedException("WillOfflineLoseQuorum is not supported by ClusApiAdapter");
		}

		public bool WillEvictLoseQuorum(string name)
		{
			throw new NotSupportedException("WillOfflineLoseQuorum is not supported by ClusApiAdapter");
		}

		public PNode Open(Guid id)
		{
			PNode privateNode = null;
			ExecuteOnNode(id, null, delegate
			{
				string nodeNameFromId = GetNodeNameFromId(id);
				privateNode = new PNode(clusApiAdapter.clusterAdapter.Cluster, id, nodeNameFromId);
			});
			return privateNode;
		}

		public PNode Open(string nodeName)
		{
			PNode privateNode = null;
			ExecuteOnNode(Guid.Empty, nodeName, delegate
			{
				Guid nodeIdFromName = GetNodeIdFromName(nodeName);
				privateNode = new PNode(clusApiAdapter.clusterAdapter.Cluster, nodeIdFromName, nodeName);
			});
			return privateNode;
		}

		public void Load(PNode node, NodeLoadSelection loadSelection)
		{
			try
			{
				ExecuteOnNode(Guid.Empty, node.Name, delegate(SafeClusterNodeHandle nodeHandle)
				{
					if ((loadSelection & NodeLoadSelection.Basic) == NodeLoadSelection.Basic && (node.LoadedSelection & 1) != 1)
					{
						node.LoadedSelection |= 1;
						SetState(node, nodeHandle);
						ExecuteOnCommonProperties(nodeHandle, node.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(node.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
						});
					}
					if ((loadSelection & NodeLoadSelection.CommonProperties) == NodeLoadSelection.CommonProperties && (node.LoadedSelection & 2) != 2)
					{
						node.LoadedSelection |= 2;
						ExecuteOnCommonProperties(nodeHandle, node.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(node.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
						});
						ExecuteOnReadOnlyCommonProperties(nodeHandle, node.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(node.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
						});
					}
					if ((loadSelection & NodeLoadSelection.PrivateProperties) == NodeLoadSelection.PrivateProperties && (node.LoadedSelection & 4) != 4)
					{
						node.LoadedSelection |= 4;
						ExecuteOnPrivateProperties(nodeHandle, node.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(node.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
						});
						ExecuteOnReadOnlyPrivateProperties(nodeHandle, node.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(node.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: true);
						});
					}
				});
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(node.Name, node.Id, innerException);
			}
		}

		public void Close(Guid id)
		{
		}

		public void Close(string name)
		{
		}

		public void Rename(Guid id, string newName)
		{
		}

		private static Win32Exception FindRpcServerUnavailableException(Exception exception)
		{
			for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
			{
				if (innerException is Win32Exception ex && NativeMethods.ErrorCode.RpcServerUnavailable.IsEqual(ex.NativeErrorCode))
				{
					return ex;
				}
			}
			return null;
		}

		private static void ProcessStartStopException<T>(string nodeName, Exception exception)
		{
			if (FindRpcServerUnavailableException(exception) != null)
			{
				exception = new ClusterServerUnavailableException(nodeName, null);
			}
			if (typeof(T) == typeof(ClusterStopNodeException))
			{
				throw new ClusterStopNodeException(nodeName, exception);
			}
			throw new ClusterStartNodeException(nodeName, exception);
		}

		public void Start(string name)
		{
			try
			{
				using ServiceController serviceController = new ServiceController("ClusSvc", name);
				if (serviceController.Status == ServiceControllerStatus.Stopped)
				{
					serviceController.Start(new string[1] { "/cq" });
				}
			}
			catch (Win32Exception exception)
			{
				ProcessStartStopException<ClusterStartNodeException>(name, exception);
			}
			catch (InvalidOperationException exception2)
			{
				ProcessStartStopException<ClusterStartNodeException>(name, exception2);
			}
		}

		public void Stop(string name)
		{
			try
			{
				using ServiceController serviceController = new ServiceController("ClusSvc", name);
				if (serviceController.Status == ServiceControllerStatus.Running)
				{
					serviceController.Stop();
				}
			}
			catch (Win32Exception exception)
			{
				ProcessStartStopException<ClusterStopNodeException>(name, exception);
			}
			catch (InvalidOperationException exception2)
			{
				ProcessStartStopException<ClusterStopNodeException>(name, exception2);
			}
		}

		public void Pause(string name, NodePauseDrainType drainType, string targetNode)
		{
			ExecuteOnNode(Guid.Empty, name, delegate(SafeClusterNodeHandle nodeHandle)
			{
				int num = NativeMethods.PauseClusterNodeEx(nodeHandle, drainType != NodePauseDrainType.NoDrain, (drainType == NodePauseDrainType.Drain) ? NativeMethods.NodePauseDrainFlag.NodePauseRemainOnPausedNodeOnMoveError : NativeMethods.NodePauseDrainFlag.None, IntPtr.Zero);
				if (num == NativeMethods.ErrorCode.TimeOut.ToInt())
				{
					throw new ClusterPauseNodeTimeoutException(name, new Win32Exception(num));
				}
				if (num != NativeMethods.ErrorCode.None.ToInt())
				{
					throw new ClusterPauseNodeException(name, new Win32Exception(num));
				}
			});
		}

		public void Resume(string name, NodeResumeFailbackType failbackType)
		{
			ExecuteOnNode(Guid.Empty, name, delegate(SafeClusterNodeHandle nodeHandle)
			{
				int num = NativeMethods.ResumeClusterNodeEx(nodeHandle, (int)failbackType, 0u);
				if (num == NativeMethods.ErrorCode.TimeOut.ToInt())
				{
					throw new ClusterResumeNodeTimeoutException(name, new Win32Exception(num));
				}
				if (num != NativeMethods.ErrorCode.None.ToInt())
				{
					throw new ClusterResumeNodeException(name, new Win32Exception(num));
				}
			});
		}

		public void Delete(Guid id)
		{
			ExecuteOnNode(id, null, delegate(SafeClusterNodeHandle nodeHandle)
			{
				string nodeNameFromId = GetNodeNameFromId(id);
				int cleanupStatus;
				int num = NativeMethods.EvictClusterNodeEx(nodeHandle, 300000u, out cleanupStatus);
				if (num == NativeMethods.ErrorCode.TimeOut.ToInt())
				{
					throw new ClusterEvictNodeTimeoutException(nodeNameFromId, new Win32Exception(num));
				}
				if (num == NativeMethods.ErrorCode.ClusterEvictWithoutCleanup.ToInt())
				{
					throw new ClusterCleanupNodeException(nodeNameFromId, new Win32Exception(num));
				}
				if (num != NativeMethods.ErrorCode.None.ToInt())
				{
					throw new ClusterEvictNodeException(nodeNameFromId, new Win32Exception(num), cleanupStatus);
				}
			});
		}

		public PNode Add(string name)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				return null;
			}
			IntPtr callbackArg = IntPtr.Zero;
			NativeMethods.ProgressCallback progressCallback = ProgressCallback;
			if (NativeMethods.AddClusterNode(handle, name, progressCallback, ref callbackArg).IsInvalid)
			{
				throw new ClusterNodeAddedException(name);
			}
			return new PNode(clusApiAdapter.clusterAdapter.Cluster, GetNodeIdFromName(name), name);
		}

		private static bool ProgressCallback(IntPtr callbackArg, ClusterSetupPhrase phase, ClusterSetupPhraseType type, ClusterSetupPhraseSeverity severity, int percent, string objectName, int status)
		{
			return true;
		}

		private unsafe void SetStructVersion1(IntPtr clusterEnumItemV2Group)
		{
			int* ptr = (int*)(void*)clusterEnumItemV2Group;
			*ptr = 1;
		}

		private void SetState(PNode node, SafeClusterNodeHandle nodeHandle)
		{
			int clusterNodeState = NativeMethods.GetClusterNodeState(nodeHandle);
			if (clusterNodeState == -1)
			{
				clusterNodeState = Marshal.GetLastWin32Error();
				if (NativeMethods.ErrorCode.NodeNotAvailable.IsEqual(clusterNodeState) || NativeMethods.ErrorCode.NodeNotFound.IsEqual(clusterNodeState))
				{
					throw new ClusterObjectNotFoundException(node.Name, new Win32Exception(clusterNodeState));
				}
				throw new Win32Exception(clusterNodeState);
			}
			node.State = (NodeState)clusterNodeState;
		}

		private string GetNodeNameFromId(Guid nodeId)
		{
			if (!clusApiAdapter.MappingIdNameNode.TryGetValue(nodeId, out var value))
			{
				throw new ClusterObjectNotFoundException(null, nodeId, typeof(Node));
			}
			return value;
		}

		internal Guid GetNodeIdFromName(string nodeName)
		{
			if (!clusApiAdapter.MappingNameIdNode.TryGetValue(nodeName, out var nodeGuid))
			{
				lock (loadingNodesLock)
				{
					if (!clusApiAdapter.MappingNameIdNode.TryGetValue(nodeName, out nodeGuid))
					{
						ExecuteOnNode(Guid.Empty, nodeName, delegate(SafeClusterNodeHandle nodeHandle)
						{
							ExecuteOnReadOnlyCommonProperties(nodeHandle, nodeName, delegate(IntPtr propList, int propListSize)
							{
								string propertyValue = string.Empty;
								int num = NativeMethods.ResUtilFindSzProperty(propList, propListSize, "NodeInstanceID", ref propertyValue);
								if (num != NativeMethods.ErrorCode.None.ToInt())
								{
									throw new ClusterPropertyNotFoundException(nodeName, "NodeInstanceID", typeof(Node), num);
								}
								nodeGuid = new Guid(propertyValue);
							});
						});
						lock (loadingNodesLock)
						{
							clusApiAdapter.MappingIdNameNode.AddOrUpdate(nodeGuid, nodeName, (Guid key, string value) => nodeName);
							clusApiAdapter.MappingNameIdNode.AddOrUpdate(nodeName, nodeGuid, (string key, Guid value) => nodeGuid);
						}
					}
				}
			}
			return nodeGuid;
		}

		internal string GetNodeNameFromNodeNumber(int nodeNumber)
		{
			if (!clusApiAdapter.MappingNumberNameNode.TryGetValue(nodeNumber, out var value))
			{
				throw new ClusterObjectNotFoundException(nodeNumber.ToString(CultureInfo.InvariantCulture), Guid.Empty, typeof(Node));
			}
			return value;
		}

		private int GetNodeNumberFromNodeName(string nodeName)
		{
			if (!clusApiAdapter.MappingNameNumberNode.TryGetValue(nodeName, out var value))
			{
				throw new ClusterObjectNotFoundException(nodeName, Guid.Empty, typeof(Node));
			}
			return value;
		}

		private void ExecuteOnPrivateProperties(SafeClusterNodeHandle nodeHandle, string nodeName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(nodeHandle, NativeMethods.CLUSCTL_NODE_GET_PRIVATE_PROPERTIES, nodeName, commonPropList);
		}

		private void ExecuteOnReadOnlyPrivateProperties(SafeClusterNodeHandle nodeHandle, string nodeName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(nodeHandle, NativeMethods.CLUSCTL_NODE_GET_RO_PRIVATE_PROPERTIES, nodeName, commonPropList);
		}

		private void ExecuteOnReadOnlyCommonProperties(SafeClusterNodeHandle nodeHandle, string nodeName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(nodeHandle, NativeMethods.CLUSCTL_NODE_GET_RO_COMMON_PROPERTIES, nodeName, commonPropList);
		}

		private void ExecuteOnCommonProperties(SafeClusterNodeHandle nodeHandle, string nodeName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(nodeHandle, NativeMethods.CLUSCTL_NODE_GET_COMMON_PROPERTIES, nodeName, commonPropList);
		}

		private void ExecuteOnProperties(SafeClusterNodeHandle nodeHandle, int controlCode, string nodeName, Action<IntPtr, int> propertyList)
		{
			try
			{
				ExecuteOnControlCode(nodeHandle, controlCode, nodeName, propertyList);
			}
			catch (ClusterControlCodeException innerException)
			{
				throw new ClusterGetPropertiesFailedException(innerException);
			}
		}

		private void ExecuteOnControlCode(SafeClusterNodeHandle nodeHandle, int controlCode, string nodeName, Action<IntPtr, int> controlCodeCallBack)
		{
			IntPtr intPtr = NativeMethods.Alloc(4096);
			int outBufferSize = 4096;
			int bytesReturned = 0;
			try
			{
				int num = NativeMethods.ClusterNodeControl(nodeHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, IntPtr.Zero, 0, intPtr, outBufferSize, ref bytesReturned);
				if (NativeMethods.ErrorCode.MoreData.IsEqual(num))
				{
					intPtr = NativeMethods.ReAlloc(intPtr, bytesReturned);
					outBufferSize = bytesReturned;
					num = NativeMethods.ClusterNodeControl(nodeHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, IntPtr.Zero, 0, intPtr, outBufferSize, ref bytesReturned);
				}
				if (NativeMethods.ErrorCode.NodeNotFound.IsEqual(num))
				{
					throw new ClusterObjectNotFoundException(nodeName, new Win32Exception(num));
				}
				if (!NativeMethods.ErrorCode.None.IsEqual(num))
				{
					throw new ClusterControlCodeException(controlCode, new Win32Exception(num));
				}
				controlCodeCallBack(intPtr, bytesReturned);
			}
			finally
			{
				NativeMethods.Free(intPtr);
			}
		}

		internal void ExecuteOnNode(Guid id, string name, Action<SafeClusterNodeHandle> onNodeCallBack)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				return;
			}
			if (id == Guid.Empty && name == null)
			{
				onNodeCallBack(SafeClusterNodeHandle.InvalidHandle);
				return;
			}
			SafeClusterNodeHandle safeClusterNodeHandle;
			ClusterAccessRights grantedAccess;
			if (id != Guid.Empty)
			{
				string nodeNameFromId = GetNodeNameFromId(id);
				if (nodeNameFromId == null)
				{
					throw new ClusterObjectNotFoundException(name, id, typeof(Node));
				}
				safeClusterNodeHandle = NativeMethods.OpenClusterNodeEx(handle, nodeNameFromId, clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess);
			}
			else
			{
				safeClusterNodeHandle = NativeMethods.OpenClusterNodeEx(handle, name, clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess);
			}
			if (safeClusterNodeHandle.IsInvalid)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == NativeMethods.ErrorCode.NodeNotFound.ToInt())
				{
					throw new ClusterObjectNotFoundException(name, id, typeof(Node));
				}
				throw ExceptionHelper.ClusterObjectLoadFailedException(name, id, lastWin32Error);
			}
			try
			{
				onNodeCallBack(safeClusterNodeHandle);
			}
			finally
			{
				safeClusterNodeHandle.Dispose();
			}
		}

		public bool NotificationArrived(NativeMethods.NOTIFY_FILTER_AND_TYPE filterType, string objectName, string objectId, string parentId, string objectType, IntPtr buffer, int bufferSize)
		{
			if (filterType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_CLUSTER && filterType.FilterFlags == 32)
			{
				Guid nodeId = GetNodeIdFromName(objectName);
				int nodeNumber = int.Parse(objectId, NumberStyles.Integer, CultureInfo.InvariantCulture);
				lock (loadingNodesLock)
				{
					clusApiAdapter.MappingIdNameNode.AddOrUpdate(nodeId, objectName, (Guid key, string value) => objectName);
					clusApiAdapter.MappingNameIdNode.AddOrUpdate(objectName, nodeId, (string key, Guid value) => nodeId);
					clusApiAdapter.MappingNumberNameNode.AddOrUpdate(nodeNumber, objectName, (int key, string value) => objectName);
					clusApiAdapter.MappingNameNumberNode.AddOrUpdate(objectName, nodeNumber, (string key, int value) => nodeNumber);
				}
				ClusterAddedEventArgs payload = new ClusterAddedEventArgs(nodeId, objectName, 0, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster
				};
				clusApiAdapter.EnqueueNotification(new NodeNotification(payload));
				return true;
			}
			if (filterType.ObjectType != NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NODE)
			{
				return false;
			}
			switch ((NativeMethods.CLUSTER_CHANGE_NODE_V2)filterType.FilterFlags)
			{
			case NativeMethods.CLUSTER_CHANGE_NODE_V2.CLUSTER_CHANGE_NODE_DELETED_V2:
			{
				Guid nodeIdFromName3 = GetNodeIdFromName(objectName);
				int nodeNumberFromNodeName = GetNodeNumberFromNodeName(objectName);
				lock (loadingNodesLock)
				{
					clusApiAdapter.MappingIdNameNode.TryRemove(nodeIdFromName3, out var _);
					clusApiAdapter.MappingNameIdNode.TryRemove(objectName, out var _);
					clusApiAdapter.MappingNumberNameNode.TryRemove(nodeNumberFromNodeName, out objectName);
					clusApiAdapter.MappingNameNumberNode.TryRemove(objectName, out var _);
				}
				ClusterRemovedEventArgs payload2 = new ClusterRemovedEventArgs(nodeIdFromName3, objectName, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster
				};
				clusApiAdapter.EnqueueNotification(new NodeNotification(payload2));
				return true;
			}
			case NativeMethods.CLUSTER_CHANGE_NODE_V2.CLUSTER_CHANGE_NODE_STATE_V2:
			{
				Guid nodeIdFromName2 = GetNodeIdFromName(objectName);
				clusApiAdapter.EnqueueNotification(new NodeNotification(new ClusterNodeStateEventArgs(nodeIdFromName2, (NodeState)Marshal.ReadInt32(buffer), null)));
				return true;
			}
			case NativeMethods.CLUSTER_CHANGE_NODE_V2.CLUSTER_CHANGE_NODE_COMMON_PROPERTY_V2:
			{
				if (bufferSize == 0)
				{
					return true;
				}
				Guid nodeIdFromName = GetNodeIdFromName(objectName);
				ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, nodeIdFromName, ClusterIdentityType.Node)
				{
					Partial = true
				};
				AdapterBase.ParseProperties(clusterPropertyCollection, buffer, bufferSize, ClusterPropertyKind.Common, readOnly: false);
				clusApiAdapter.EnqueueNotification(new NodeNotification(new ClusterPropertiesEventArgs(nodeIdFromName, objectName, null, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster,
					Properties = clusterPropertyCollection,
					ObjectType = null,
					PropertyKind = ClusterPropertyKind.Common
				}));
				return true;
			}
			default:
				return false;
			}
		}

		public void Collect()
		{
		}
	}

	private class QuorumAdapter : INotificationHandler
	{
		private readonly ClusApiAdapter clusApiAdapter;

		private PCluster Cluster { get; set; }

		public QuorumAdapter(ClusApiAdapter clusApiAdapter, PCluster cluster)
		{
			this.clusApiAdapter = clusApiAdapter;
			Cluster = cluster;
		}

		public bool NotificationArrived(NativeMethods.NOTIFY_FILTER_AND_TYPE filterType, string objectName, string objectId, string parentId, string objectType, IntPtr buffer, int bufferSize)
		{
			if (filterType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_QUORUM && filterType.FilterFlags == 1)
			{
				QuorumConfigurationPrivate quorumConfiguration = clusApiAdapter.Cluster.GetQuorumConfiguration();
				Cluster.QuorumConfiguration = quorumConfiguration;
				clusApiAdapter.EnqueueNotification(new ClusterNotification(new ClusterQuorumChangedEventArgs(clusApiAdapter.clusterAdapter.Cluster.Id, quorumConfiguration)));
				return true;
			}
			return false;
		}
	}

	private class ResourceAdapter : AdapterBase, IConnectionAdapterResource, INotificationHandler
	{
		private readonly object loadingResourceLock = new object();

		private readonly ConcurrentDictionary<Guid, ClusterPropertiesEventArgs> virtualPrivatePropertiesPayloadCache = new ConcurrentDictionary<Guid, ClusterPropertiesEventArgs>();

		private readonly ClusApiAdapter clusApiAdapter;

		private const int DefaultBulkEnumItemSize = 512;

		public ResourceAdapter(ClusApiAdapter clusApiAdapter)
			: base(clusApiAdapter)
		{
			this.clusApiAdapter = clusApiAdapter;
			Init();
		}

		public void MoveToGroup(Guid resourceId, Guid groupId)
		{
			if (clusApiAdapter.clusterAdapter.Handle != null)
			{
				clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
				{
					wmiAdapter.Resource.MoveToGroup(resourceId, groupId);
				});
			}
		}

		private void Init()
		{
		}

		public IEnumerable<PResource> GetAll(bool nullElementOnError)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				yield break;
			}
			PCluster privateCluster = clusApiAdapter.clusterAdapter.Cluster;
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			SafeClusterResourceEnumExHandle enumHandle;
			try
			{
				intPtr2 = new string[4] { "Name", "LastOperationStatusCode", "ResourceSpecificData1", "ResourceSpecificData2" }.ToMultiSZPointer(out var size);
				intPtr = new string[2] { "Type", "ResourceSpecificStatus" }.ToMultiSZPointer(out var size2);
				enumHandle = NativeMethods.ClusterResourceOpenEnumEx(handle.DangerousGetHandle(), intPtr, size2, intPtr2, size, 0);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
				if (intPtr2 != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr2);
				}
			}
			if (enumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateResourcesException>(Marshal.GetLastWin32Error());
			}
			IntPtr allocatedMemory = IntPtr.Zero;
			try
			{
				int records = NativeMethods.ClusterResourceGetEnumCountEx(enumHandle);
				for (int index = 0; index < records; index++)
				{
					PResource pResource;
					try
					{
						int enumItemSize = 512;
						allocatedMemory = NativeMethods.Alloc(enumItemSize);
						int scCode = NativeMethods.ClusterResourceEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
						if (NativeMethods.ErrorCode.MoreData.IsEqual(scCode))
						{
							allocatedMemory = NativeMethods.ReAlloc(allocatedMemory, enumItemSize);
							scCode = NativeMethods.ClusterResourceEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
						}
						if (!NativeMethods.ErrorCode.None.IsEqual(scCode))
						{
							throw ExceptionHelper.Build<ClusterIterateResourcesException>(Marshal.GetLastWin32Error());
						}
						NativeMethods.CLUSTER_FILTER_RESOURCE_ITEM cLUSTER_FILTER_RESOURCE_ITEM;
						ClusterPropertyCollection clusterPropertyCollection;
						try
						{
							cLUSTER_FILTER_RESOURCE_ITEM = (NativeMethods.CLUSTER_FILTER_RESOURCE_ITEM)Marshal.PtrToStructure(allocatedMemory, typeof(NativeMethods.CLUSTER_FILTER_RESOURCE_ITEM));
							clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, new Guid(cLUSTER_FILTER_RESOURCE_ITEM.lpszOwnerGroupId), ClusterIdentityType.Resource);
							AdapterBase.ParseProperties(clusterPropertyCollection, cLUSTER_FILTER_RESOURCE_ITEM.pProperties, cLUSTER_FILTER_RESOURCE_ITEM.cbProperties, ClusterPropertyKind.Common, readOnly: false);
							AdapterBase.ParseProperties(clusterPropertyCollection, cLUSTER_FILTER_RESOURCE_ITEM.pRoProperties, cLUSTER_FILTER_RESOURCE_ITEM.cbRoProperties, ClusterPropertyKind.Common, readOnly: true);
							clusterPropertyCollection.Partial = true;
							clusterPropertyCollection.CommonPropertiesLoaded = false;
						}
						finally
						{
							allocatedMemory = NativeMethods.Free(allocatedMemory);
						}
						using (ClusterLock clusterLock = privateCluster.CacheManager.Get(new Guid(cLUSTER_FILTER_RESOURCE_ITEM.lpszOwnerGroupId), ClusterIdentityType.Group, LockAccess.Reader))
						{
							PGroup pGroup = null;
							if (clusterLock != null)
							{
								pGroup = (PGroup)clusterLock.Owner;
							}
							if (pGroup == null && !privateCluster.CacheManager.GroupCacheLoaded)
							{
								pGroup = clusApiAdapter.Group.Open(cLUSTER_FILTER_RESOURCE_ITEM.lpszOwnerGroupId);
							}
							if (pGroup == null)
							{
								throw new ClusterObjectNotFoundException(cLUSTER_FILTER_RESOURCE_ITEM.lpszOwnerGroupName, new Guid(cLUSTER_FILTER_RESOURCE_ITEM.lpszOwnerGroupId), typeof(FailoverClusters.Framework.Group));
							}
							PResourceType pResourceType = privateCluster.GetResourceType(((ClusterPropertyString)clusterPropertyCollection["Type"]).TypedValue);
							if (pResourceType == null)
							{
								break;
							}
							if (pGroup.GroupType == GroupType.ClusterSharedVolume && pResourceType.ResourceKind == ResourceKind.PhysicalDisk)
							{
								pResourceType = new PResourceType(clusApiAdapter.clusterAdapter.Cluster, ResourceKind.ClusterFileSystem, pResourceType);
							}
							pResource = PResource.Constructor(privateCluster, new Guid(cLUSTER_FILTER_RESOURCE_ITEM.lpszId), cLUSTER_FILTER_RESOURCE_ITEM.lpszName, pResourceType);
							pResource.OwnerGroup = pGroup;
							pResource.Properties = clusterPropertyCollection;
						}
						pResource.LoadedSelection = 0;
					}
					catch (ClusterException ex)
					{
						if (!nullElementOnError)
						{
							throw;
						}
						if (ex is ClusterClosedException)
						{
							throw;
						}
						ClusterLog.LogException(ex, "There was an error when getting group information from the cluster, however is not critical and the process can continue");
						pResource = null;
					}
					yield return pResource;
				}
			}
			finally
			{
				enumHandle.Dispose();
				NativeMethods.Free(allocatedMemory);
			}
		}

		public IEnumerable<TResult> Select<TResult>(QueryInfo queryInfo)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				yield break;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			Guid guid = Guid.Empty;
			IEnumerable<string> source = queryInfo.ProjectionFields.Select((ClusterObjectMetaDataMember s) => s.Name).Concat(queryInfo.WhereFields.Select((ClusterObjectMetaDataMember s) => s.Name)).Concat(from s in queryInfo.OrderBy.ConvertAll((OrderByItem item) => item.DataMember)
				select s.Name)
				.Distinct();
			List<string> queryFields2 = source.Where((string s) => s.ToLowerInvariant() != "name" && s.ToLowerInvariant() != "id" && s.ToLowerInvariant() != "resourcetype").ToList();
			List<IClusterQueryArgument> whereSyntaxis = queryInfo.WhereSyntaxis;
			string text = null;
			Guid guid2 = Guid.Empty;
			for (int i = 0; i < whereSyntaxis.Count; i++)
			{
				IClusterQueryArgument clusterQueryArgument = whereSyntaxis[i];
				if (clusterQueryArgument is FieldArgument)
				{
					text = clusterQueryArgument.Name;
					switch (text.ToLowerInvariant())
					{
					case "resourceclass":
						if (whereSyntaxis.Count > i + 2)
						{
							OperatorArgument operatorArgument2 = whereSyntaxis[i + 1] as OperatorArgument;
							ValueArgument valueArgument2 = whereSyntaxis[i + 2] as ValueArgument;
							if (operatorArgument2 != null && operatorArgument2.OperatorType == OperatorType.Equal && valueArgument2 != null && (int)valueArgument2.Value == 1)
							{
								flag3 = true;
							}
						}
						break;
					case "poolid":
						if (whereSyntaxis.Count > i + 2)
						{
							OperatorArgument operatorArgument3 = whereSyntaxis[i + 1] as OperatorArgument;
							ValueArgument valueArgument3 = whereSyntaxis[i + 2] as ValueArgument;
							if (operatorArgument3 != null && operatorArgument3.OperatorType == OperatorType.Equal && valueArgument3 != null && valueArgument3.Value is Guid)
							{
								guid = (Guid)valueArgument3.Value;
							}
						}
						break;
					case "resourcekind":
					{
						if (whereSyntaxis.Count <= i + 2)
						{
							break;
						}
						OperatorArgument operatorArgument = whereSyntaxis[i + 1] as OperatorArgument;
						ValueArgument valueArgument = whereSyntaxis[i + 2] as ValueArgument;
						if (operatorArgument != null && operatorArgument.OperatorType == OperatorType.Equal && valueArgument != null && valueArgument.Value is int)
						{
							switch ((ResourceKind)valueArgument.Value)
							{
							case ResourceKind.StoragePool:
								flag4 = true;
								break;
							case ResourceKind.VirtualMachine:
								flag = true;
								break;
							case ResourceKind.ClusterFileSystem:
								flag2 = true;
								break;
							case ResourceKind.PhysicalDisk:
								flag5 = true;
								break;
							}
						}
						break;
					}
					}
				}
				else
				{
					if (!(clusterQueryArgument is GuidArgument))
					{
						continue;
					}
					if (text == null)
					{
						ClusterLog.LogError("Field argument is null in the query '{0}'".FormatCurrentCulture(queryInfo.QueryText));
						throw new NullReferenceException("");
					}
					string text2 = text.ToLowerInvariant();
					if (!(text2 == "ownergroup"))
					{
						if (!(text2 == "type"))
						{
							continue;
						}
						if (flag)
						{
							flag = false;
							continue;
						}
						if (flag4)
						{
							flag4 = false;
							continue;
						}
						OperatorArgument operatorArgument4 = whereSyntaxis[i - 1] as OperatorArgument;
						GuidArgument guidArgument = clusterQueryArgument as GuidArgument;
						if (operatorArgument4 != null)
						{
							if (operatorArgument4.OperatorType == OperatorType.Equal && guidArgument.Value.ToString() == PResourceType.ResourceKindToString(ResourceKind.StoragePool))
							{
								flag4 = true;
							}
							else if (operatorArgument4.OperatorType == OperatorType.Equal && guidArgument.Value.ToString() == PResourceType.ResourceKindToString(ResourceKind.VirtualMachine))
							{
								flag = true;
							}
							else if (operatorArgument4.OperatorType == OperatorType.Equal && guidArgument.Value.ToString() == PResourceType.ResourceKindToString(ResourceKind.ClusterFileSystem))
							{
								flag2 = true;
							}
							else if (operatorArgument4.OperatorType == OperatorType.Equal && guidArgument.Value.ToString() == PResourceType.ResourceKindToString(ResourceKind.PhysicalDisk))
							{
								flag5 = true;
							}
						}
					}
					else
					{
						guid2 = new Guid(((GuidArgument)clusterQueryArgument).Value.ToString());
					}
				}
			}
			if (guid2 != Guid.Empty)
			{
				foreach (TResult item in GetResourcesFromGroup<TResult>(queryInfo, flag, guid2, queryFields2))
				{
					yield return item;
				}
				yield break;
			}
			if (flag2)
			{
				queryFields2 = queryFields2.Union(new string[4] { "name", "id", "resourcetype", "ownergroup" }).Distinct().ToList();
				foreach (Tuple<Guid, string> clusterObject in GetClusterObjects(handle, NativeMethods.ClusterEnumType.ClusterFileSystem))
				{
					ClusterPropertyCollection csvPropertyCollection = null;
					Tuple<Guid, string> csvResourceTouple = clusterObject;
					ExecuteOnResource(Guid.Empty, csvResourceTouple.Item2, delegate(SafeClusterResourceHandle csvResourceHandle)
					{
						csvPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, csvResourceTouple.Item1, ClusterIdentityType.Resource);
						ExecuteOnReadOnlyCommonProperties(csvResourceHandle, csvResourceTouple.Item2, delegate(IntPtr commonPropList, int commonPropListSize)
						{
							AdapterBase.ParseProperties(csvPropertyCollection, commonPropList, commonPropListSize, ClusterPropertyKind.Common, readOnly: true);
						});
					});
					PResource csvResource = CreateResource((string)csvPropertyCollection["name"].Value, queryFields2, SafeClusterGroupHandle.InvalidHandle, null, true);
					clusApiAdapter.groups.ExecuteOnGroup(csvResource.OwnerGroup.Id, null, delegate(SafeClusterGroupHandle csvGroupHandle)
					{
						clusApiAdapter.groups.SetStateAndNode(csvResource.OwnerGroup, csvGroupHandle);
					});
					yield return (TResult)(object)csvResource;
				}
				yield break;
			}
			if (flag3 || flag4 || (flag5 && guid != Guid.Empty))
			{
				foreach (PResource item2 in GetStorage(handle, queryInfo, !flag3 && flag4, guid))
				{
					yield return (TResult)(object)item2;
				}
				yield break;
			}
			foreach (PResource item3 in GetAll(nullElementOnError: false))
			{
				yield return (TResult)(object)item3;
			}
		}

		private IEnumerable<PResource> GetResourcesByResourceType(SafeClusterHandle clusterHandle, QueryInfo queryInfo, ResourceKind resourceKind)
		{
			string resourceType = PResourceType.ResourceKindToString(resourceKind);
			SafeClusterResourceTypeEnumHandle resTypeEnumHandle = NativeMethods.ClusterResourceTypeOpenEnum(clusterHandle, resourceType, NativeMethods.ResourceTypeEnumType.Resource);
			if (resTypeEnumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateResourcesException>(Marshal.GetLastWin32Error());
			}
			try
			{
				int records = NativeMethods.ClusterResourceTypeGetEnumCount(resTypeEnumHandle);
				for (int index = 0; index < records; index++)
				{
					if (queryInfo != null && queryInfo.IsCancel)
					{
						break;
					}
					NativeMethods.ResourceTypeEnumType enumType = NativeMethods.ResourceTypeEnumType.Resource;
					StringBuilder stringBuilder = new StringBuilder(200);
					int resourceTypeNameSize = 200;
					int scCode = NativeMethods.ClusterResourceTypeEnum(resTypeEnumHandle, index, ref enumType, stringBuilder, ref resourceTypeNameSize);
					if (NativeMethods.ErrorCode.MoreData.IsEqual(scCode))
					{
						stringBuilder = new StringBuilder(resourceTypeNameSize);
						scCode = NativeMethods.ClusterResourceTypeEnum(resTypeEnumHandle, index, ref enumType, stringBuilder, ref resourceTypeNameSize);
					}
					if (!NativeMethods.ErrorCode.None.IsEqual(scCode))
					{
						throw ExceptionHelper.Build<ClusterIterateResourcesException>(Marshal.GetLastWin32Error());
					}
					string resourceName = stringBuilder.ToString();
					yield return CreateResource(resourceName, new string[2] { "privateproperties", "commonproperties" }, SafeClusterGroupHandle.InvalidHandle, null, null, resourceType);
				}
			}
			finally
			{
				resTypeEnumHandle.Dispose();
			}
		}

		private IEnumerable<PResource> GetStorage(SafeClusterHandle clusterHandle, QueryInfo queryInfo, bool onlyStoragePool, Guid poolId)
		{
			foreach (PResource item in GetResourcesByResourceType(clusterHandle, queryInfo, onlyStoragePool ? ResourceKind.StoragePool : ResourceKind.PhysicalDisk))
			{
				item.Class = ResourceClass.Storage;
				if (poolId != Guid.Empty)
				{
					ClusterPropertyString clusterPropertyString = (ClusterPropertyString)item.Properties["PoolId"];
					if (clusterPropertyString == null || string.IsNullOrEmpty(clusterPropertyString.TypedValue) || (poolId != new Guid("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF") && poolId != new Guid(clusterPropertyString.TypedValue)))
					{
						continue;
					}
				}
				yield return item;
			}
		}

		private void GetResourceClass(PResource resource, SafeClusterResourceHandle resourceHandle)
		{
			if (resource.ResourceState == ResourceState.Failed && (resource.ResourceType.ResourceKind == ResourceKind.VirtualMachine || resource.ResourceType.ResourceKind == ResourceKind.VirtualMachineConfiguration))
			{
				resource.Class = ResourceClass.Unknown;
				return;
			}
			try
			{
				ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_CLASS_INFO, resource.Name, delegate(IntPtr buffer, int bufferSize)
				{
					ulong num = (ulong)Marshal.ReadInt64(buffer);
					resource.Class = (ResourceClass)(num & 0xFFFF);
					resource.Subclass = (ResourceSubclass)(num >> 32);
					if (resource is PStorageResource pStorageResource)
					{
						pStorageResource.AssignStorageCaps();
					}
				});
			}
			catch (Exception)
			{
				resource.Class = ResourceClass.Unknown;
			}
		}

		private void GetReplicationDiskType(PResource resource)
		{
			if (clusApiAdapter.clusterAdapter.Handle != null)
			{
				PCluster cluster = clusApiAdapter.clusterAdapter.Cluster;
				if (resource is PStorageResource pStorageResource && cluster.CacheManager.ReplicatedResources.TryGetValue(pStorageResource.Id, out var value))
				{
					pStorageResource.ReplicationDiskType = value.Role;
				}
			}
		}

		public void AddDependency(Guid id, Guid dependencyId)
		{
			throw new NotSupportedException("AddDependency is not supported by ClusApiAdapter");
		}

		public void RemoveDependency(Guid resourceId, Guid dependOnResourceId)
		{
			ExecuteOnResource(resourceId, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				ExecuteOnResource(dependOnResourceId, null, delegate(SafeClusterResourceHandle dependOnHandle)
				{
					int scCode = NativeMethods.RemoveClusterResourceDependency(resourceHandle, dependOnHandle);
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (!NativeMethods.ErrorCode.None.IsEqual(scCode))
					{
						throw new ClusterResourceRemoveDependencyException(resourceId, dependOnResourceId, ExceptionHelper.Build(lastWin32Error));
					}
				});
			});
		}

		public void AddRegistryCheckpoint(Guid id, string checkpoint)
		{
			throw new NotSupportedException("AddRegistryCheckpoint is not supported by ClusApiAdapter");
		}

		public void RemoveRegistryCheckpoint(Guid id, string checkpoint)
		{
			throw new NotSupportedException("RemoveRegistryCheckpoint is not supported by ClusApiAdapter");
		}

		public IEnumerable<string> GetRegistryCheckpoints(Guid id)
		{
			throw new NotSupportedException("GetRegistryCheckpoints is not supported by ClusApiAdapter");
		}

		public void AddCryptoCheckpoint(Guid id, string checkpoint)
		{
			throw new NotSupportedException("AddCryptoCheckpoint is not supported by ClusApiAdapter");
		}

		public void RemoveCryptoCheckpoint(Guid id, string checkpoint)
		{
			throw new NotSupportedException("RemoveCryptoCheckpoint is not supported by ClusApiAdapter");
		}

		public IEnumerable<string> GetCryptoCheckpoints(Guid id)
		{
			throw new NotSupportedException("GetCryptoCheckpoints is not supported by ClusApiAdapter");
		}

		public void AddPossibleOwner(Guid id, string node)
		{
			throw new NotSupportedException("AddPossibleOwners is not supported by ClusApiAdapter");
		}

		public void RemovePossibleOwner(Guid id, string node)
		{
			throw new NotSupportedException("RemovePossibleOwners is not supported by ClusApiAdapter");
		}

		public void SetPossibleOwners(Guid id, IEnumerable<Guid> nodes)
		{
			List<Guid> newNodes = new List<Guid>(nodes);
			int returnCode = 0;
			ExecuteOnResource(id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				List<Guid> possibleOwners = GetPossibleOwners(id);
				foreach (Guid currentNode in possibleOwners)
				{
					if (!newNodes.Exists((Guid item) => item == currentNode))
					{
						clusApiAdapter.nodes.ExecuteOnNode(currentNode, null, delegate(SafeClusterNodeHandle nodeHandle)
						{
							returnCode = NativeMethods.RemoveClusterResourceNode(resourceHandle, nodeHandle);
							if (!NativeMethods.ErrorCode.None.IsEqual(returnCode))
							{
								throw new ClusterResourceSetPossibleOwnersException(GetResourceNameFromId(id), new Win32Exception(returnCode));
							}
						});
					}
				}
				foreach (Guid newNode in newNodes)
				{
					if (!possibleOwners.Exists((Guid item) => item == newNode))
					{
						clusApiAdapter.nodes.ExecuteOnNode(newNode, null, delegate(SafeClusterNodeHandle nodeHandle)
						{
							returnCode = NativeMethods.AddClusterResourceNode(resourceHandle, nodeHandle);
							if (!NativeMethods.ErrorCode.None.IsEqual(returnCode))
							{
								throw new ClusterResourceSetPossibleOwnersException(GetResourceNameFromId(id), new Win32Exception(returnCode));
							}
						});
					}
				}
			});
		}

		public PResource Open(Guid id)
		{
			return CreateResource(GetResourceNameFromId(id), null, SafeClusterGroupHandle.InvalidHandle, null, null);
		}

		public PResource Open(string resourceName)
		{
			return CreateResource(resourceName, null, SafeClusterGroupHandle.InvalidHandle, null, null);
		}

		public PResource Create(PGroup group, string name, PResourceType resourceType, bool separateMonitor)
		{
			PResource privateResource = null;
			clusApiAdapter.groups.ExecuteOnGroup(group.Id, group.Name, delegate(SafeClusterGroupHandle groupHandle)
			{
				string name2 = resourceType.Name;
				SafeClusterResourceHandle safeClusterResourceHandle = NativeMethods.CreateClusterResource(groupHandle, name, name2, separateMonitor ? NativeMethods.ClusterResourceCreateFlags.SeparateMonitor : NativeMethods.ClusterResourceCreateFlags.DefaultMonitor);
				Guid id;
				try
				{
					if (safeClusterResourceHandle.IsInvalid)
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						if (NativeMethods.ErrorCode.ObjectAlreadyExists.IsEqual(lastWin32Error))
						{
							throw new ClusterResourceAlreadyExistException(name);
						}
						throw ExceptionHelper.Build(lastWin32Error);
					}
					id = GetId(safeClusterResourceHandle);
				}
				finally
				{
					safeClusterResourceHandle.Dispose();
				}
				privateResource = PResource.Constructor(clusApiAdapter.clusterAdapter.Cluster, id, name, resourceType);
			});
			return privateResource;
		}

		public void Delete(Guid id, bool cleanup)
		{
			clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
			{
				wmiAdapter.Resource.Delete(id, cleanup);
			});
		}

		public void Rename(Guid id, string newName)
		{
			ExecuteOnResource(id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				int num = NativeMethods.SetClusterResourceName(resourceHandle, newName);
				if (NativeMethods.ErrorCode.ErrorAlreadyExists.IsEqual(num))
				{
					throw new ClusterResourceAlreadyExistException(newName);
				}
				if (!NativeMethods.ErrorCode.None.IsEqual(num) && !NativeMethods.ErrorCode.IOPending.IsEqual(num))
				{
					throw ExceptionHelper.Build(num);
				}
			});
		}

		public void Load(PResource resource, ResourceLoadSelection loadSelection)
		{
			SafeClusterHandle clusterHandle = clusApiAdapter.clusterAdapter.Handle;
			if (clusterHandle == null)
			{
				return;
			}
			try
			{
				ExecuteOnResource(resource.Id, resource.Name, delegate(SafeClusterResourceHandle resourceHandle)
				{
					PDistributedNetworkNameResource privateDnnResource = resource as PDistributedNetworkNameResource;
					if ((loadSelection & ResourceLoadSelection.Basic) == ResourceLoadSelection.Basic && (resource.LoadedSelection & 1) != 1)
					{
						SetStateAndGroup(resource, resourceHandle, SafeClusterGroupHandle.InvalidHandle);
						ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_FLAGS, resource.Name, delegate(IntPtr buffer, int bufferSize)
						{
							resource.Flags = (ResourceFlags)Marshal.ReadInt32(buffer);
						});
						ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_CHARACTERISTICS, resource.Name, delegate(IntPtr buffer, int bufferSize)
						{
							resource.Characteristics = (Characteristics)Marshal.ReadInt32(buffer);
						}, delegate
						{
							resource.Characteristics = Characteristics.None;
						});
						GetReplicationDiskType(resource);
						if (privateDnnResource != null)
						{
							privateDnnResource.ClusterNetworkInfos = GetClusterNetworkInfos(clusterHandle);
							loadSelection |= ResourceLoadSelection.PrivateProperties;
						}
						resource.LoadedSelection |= 1;
					}
					if ((loadSelection & ResourceLoadSelection.Storage) == ResourceLoadSelection.Storage && (resource.LoadedSelection & 0x100) != 256)
					{
						if (resource.Class == ResourceClass.Storage)
						{
							resource.OwnerGroup.LoadObject(1);
							LoadDisk(resource);
						}
						resource.LoadedSelection |= 256;
					}
					if ((loadSelection & ResourceLoadSelection.PoolPhysicalDisksInfo) == ResourceLoadSelection.PoolPhysicalDisksInfo && (resource.LoadedSelection & 0x400) != 1024)
					{
						if (resource.ResourceType.ResourceKind == ResourceKind.StoragePool && resource is PStoragePoolResource pStoragePoolResource)
						{
							pStoragePoolResource.PhysicalDisksInfo = GetPoolPhysicalDisksInfo(pStoragePoolResource);
						}
						resource.LoadedSelection |= 1024;
					}
					if ((loadSelection & ResourceLoadSelection.CommonProperties) == ResourceLoadSelection.CommonProperties && (resource.LoadedSelection & 2) != 2)
					{
						ExecuteOnCommonProperties(resourceHandle, resource.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(resource.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
							if (resource.ResourceType.ResourceKind == ResourceKind.Other && resource.Name == null)
							{
								resource.ResourceType.Name = ((ClusterPropertyString)resource.Properties["Type"]).TypedValue;
							}
						});
						ExecuteOnReadOnlyCommonProperties(resourceHandle, resource.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(resource.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
						});
						resource.LoadedSelection |= 2;
					}
					if ((loadSelection & ResourceLoadSelection.PrivateProperties) == ResourceLoadSelection.PrivateProperties && (long)(resource.LoadedSelection & 4) != 4)
					{
						ExecuteOnPrivateProperties(resourceHandle, resource.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(resource.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
						});
						ExecuteOnReadOnlyPrivateProperties(resourceHandle, resource.Name, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(resource.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: true);
						});
						resource.LoadedSelection |= 4;
					}
					if (privateDnnResource != null)
					{
						resource.Properties.Get("InUseNetworks", delegate(ClusterPropertyMultipleStrings inUseNetworks)
						{
							privateDnnResource.ClusterNetworkInfos.RemoveAll((NetworkInfo networkInfo) => !inUseNetworks.TypedValue.Contains(networkInfo.Id.ToString(), StringComparer.OrdinalIgnoreCase));
						});
					}
					if ((loadSelection & ResourceLoadSelection.Dependencies) == ResourceLoadSelection.Dependencies && ((ulong)resource.LoadedSelection & 8uL) != 8)
					{
						resource.Dependencies = GetDependencies(resourceHandle);
						resource.LoadedSelection |= 8;
					}
					if ((loadSelection & ResourceLoadSelection.Dependents) == ResourceLoadSelection.Dependents && (resource.LoadedSelection & 0x10) != 16)
					{
						resource.Dependents = GetDependents(resourceHandle);
						resource.LoadedSelection |= 16;
					}
					if ((loadSelection & ResourceLoadSelection.DependenciesRelation) == ResourceLoadSelection.DependenciesRelation && (resource.LoadedSelection & 0x20) != 32)
					{
						resource.DependencyRelationship = GetDependencyRelationship(resourceHandle, resource.Id);
						resource.LoadedSelection |= 32;
					}
					if ((loadSelection & ResourceLoadSelection.RequiredDependencies) == ResourceLoadSelection.RequiredDependencies && (resource.LoadedSelection & 0x40) != 64)
					{
						resource.RequiredDependencies = GetRequiredDependencies(resourceHandle, resource.Name);
						resource.LoadedSelection |= 64;
					}
					if ((loadSelection & ResourceLoadSelection.PossibleOwners) == ResourceLoadSelection.PossibleOwners)
					{
						resource.PossibleOwners = GetPossibleOwners(resourceHandle);
						resource.LoadedSelection |= 128;
					}
					if ((loadSelection & ResourceLoadSelection.StoragePoolInfo) == ResourceLoadSelection.StoragePoolInfo && (resource.LoadedSelection & 0x200) != 512)
					{
						if (resource is PStorageResource pStorageResource && pStorageResource.GetPoolId() != Guid.Empty)
						{
							LoadPoolInfo(pStorageResource);
						}
						resource.LoadedSelection |= 512;
					}
					if ((loadSelection & ResourceLoadSelection.StorageReplicationInfo) == ResourceLoadSelection.StorageReplicationInfo && (resource.LoadedSelection & 0x800) != 2048)
					{
						if (resource is PStorageResource pStorageResource2 && pStorageResource2.ReplicationDiskType != 0)
						{
							clusApiAdapter.Storage.LoadReplicationInfo(pStorageResource2);
						}
						resource.LoadedSelection |= 2048;
					}
				});
			}
			catch (ClusterException innerException)
			{
				throw new ClusterObjectLoadFailedException(resource.Name, resource.Id, innerException);
			}
		}

		public void Online(Guid id, bool overrideLockState = false, bool chooseBestNode = false)
		{
			ExecuteOnResource(id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				uint num = 0u;
				if (overrideLockState)
				{
					num |= 1u;
				}
				if (chooseBestNode)
				{
					num |= 8u;
				}
				int num2 = ((num != 0) ? NativeMethods.OnlineClusterResourceEx(resourceHandle, num, IntPtr.Zero, 0) : NativeMethods.OnlineClusterResource(resourceHandle));
				if (!NativeMethods.ErrorCode.None.IsEqual(num2) && !NativeMethods.ErrorCode.IOPending.IsEqual(num2))
				{
					string resourceNameFromId = GetResourceNameFromId(id);
					AdapterBase.VerifyOperationOccuredWhileUnlocked(num2, resourceNameFromId, id);
					if (NativeMethods.ErrorCode.ResourceIsReplicaVirtualMachine.IsEqual(num2))
					{
						throw new ClusterVirtualMachineStartReplicaException(resourceNameFromId);
					}
					if (resourceNameFromId != null)
					{
						throw new ClusterResourceOnlineException(resourceNameFromId, new Win32Exception(num2));
					}
					throw new ClusterResourceOnlineException(id, new Win32Exception(num2));
				}
			});
		}

		public void OfflineDependents(Guid id, bool overrideLockState = false)
		{
			ExecuteOnResource(id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				string resourceNameFromId = GetResourceNameFromId(id);
				using NativeMethods.UnmanagedBuffer unmanagedBuffer = NativeMethods.UnmanagedBuffer.Create(overrideLockState ? 1 : 0);
				if (unmanagedBuffer.IsMemoryValid)
				{
					ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_OFFLINE_DEPENDENT_GROUPS, resourceNameFromId, unmanagedBuffer, null);
				}
			});
		}

		public void Offline(Guid id, bool overrideLockState = false)
		{
			ExecuteOnResource(id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				int num = (overrideLockState ? NativeMethods.OfflineClusterResourceEx(resourceHandle, 1u, IntPtr.Zero, 0) : NativeMethods.OfflineClusterResource(resourceHandle));
				if (!NativeMethods.ErrorCode.None.IsEqual(num) && !NativeMethods.ErrorCode.IOPending.IsEqual(num))
				{
					string resourceNameFromId = GetResourceNameFromId(id);
					AdapterBase.VerifyOperationOccuredWhileUnlocked(num, resourceNameFromId, id);
					if (resourceNameFromId != null)
					{
						throw new ClusterResourceOfflineException(resourceNameFromId, new Win32Exception(num));
					}
					throw new ClusterResourceOfflineException(id, new Win32Exception(num));
				}
			});
		}

		public IEnumerable<string> GetPossibleOwners(string name)
		{
			throw new NotSupportedException("GetPossibleOwners is not supported by ClusApiAdapter");
		}

		public string GetType(Guid id, string name)
		{
			string resourceTypeString = string.Empty;
			ExecuteOnResource(id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				ExecuteOnCommonProperties(resourceHandle, name, delegate(IntPtr commonPropList, int commonPropListSize)
				{
					int num = NativeMethods.ResUtilFindSzProperty(commonPropList, commonPropListSize, "Type", ref resourceTypeString);
					if (NativeMethods.ErrorCode.None.Equals(num))
					{
						throw new ClusterPropertyNotFoundException(id, "Type", typeof(Resource), num);
					}
				});
			});
			return resourceTypeString;
		}

		public void Fail(Guid id)
		{
			ExecuteOnResource(id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				int num = NativeMethods.FailClusterResource(resourceHandle);
				if (!NativeMethods.ErrorCode.None.IsEqual(num) && !NativeMethods.ErrorCode.IOPending.IsEqual(num))
				{
					string resourceNameFromId = GetResourceNameFromId(id);
					if (resourceNameFromId != null)
					{
						throw new ClusterResourceFailException(resourceNameFromId, new Win32Exception(num));
					}
					throw new ClusterResourceFailException(id, new Win32Exception(num));
				}
			});
		}

		public void SetDependencyRelationship(Guid id, string relationship)
		{
			ExecuteOnResource(id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				int num = NativeMethods.SetClusterResourceDependencyExpression(resourceHandle, relationship);
				if (!NativeMethods.ErrorCode.None.IsEqual(num) && !NativeMethods.ErrorCode.IOPending.IsEqual(num))
				{
					string resourceNameFromId = GetResourceNameFromId(id);
					if (resourceNameFromId != null)
					{
						throw new ClusterGetDependencyRelationshipException(resourceNameFromId, new Win32Exception(num));
					}
					throw new ClusterGetDependencyRelationshipException(id, new Win32Exception(num));
				}
			});
		}

		public void ExecuteOnResource(Guid id, string name, Action<SafeClusterResourceHandle> actionOnResource)
		{
			if (id == Guid.Empty && name == null)
			{
				throw new ArgumentException("id and name cannot be both null");
			}
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				return;
			}
			ClusterAccessRights grantedAccess;
			SafeClusterResourceHandle safeClusterResourceHandle = ((id != Guid.Empty) ? NativeMethods.OpenClusterResourceEx(handle, id.ToString(), clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess) : NativeMethods.OpenClusterResourceEx(handle, name, clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess));
			if (safeClusterResourceHandle.IsInvalid)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == NativeMethods.ErrorCode.ResourceNotFound.ToInt())
				{
					throw new ClusterObjectNotFoundException(name, id, typeof(Resource));
				}
				throw ExceptionHelper.ClusterObjectLoadFailedException(name, id, lastWin32Error);
			}
			try
			{
				actionOnResource(safeClusterResourceHandle);
			}
			finally
			{
				safeClusterResourceHandle.Dispose();
			}
		}

		public void FetchVirtualPropertiesPayload(ClusterPropertiesEventArgs propertiesPayload)
		{
			Exceptions.ThrowIfNull(propertiesPayload, "propertiesPayload");
			virtualPrivatePropertiesPayloadCache.TryRemove(propertiesPayload.Id, out var _);
			propertiesPayload.VirtualPropertyPayloadStatus = VirtualPropertyPayloadStatus.Fetching;
			try
			{
				ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Fetching priv property for {0}".FormatCurrentCulture(propertiesPayload.Name));
				ExecuteOnResource(Guid.Empty, propertiesPayload.Name, delegate(SafeClusterResourceHandle resourceHandle)
				{
					ExecuteOnPrivateProperties(resourceHandle, propertiesPayload.Name, delegate(IntPtr propertyList, int propertyListSize)
					{
						AdapterBase.ParseProperties(propertiesPayload.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
					});
					ExecuteOnReadOnlyPrivateProperties(resourceHandle, propertiesPayload.Name, delegate(IntPtr propertyList, int propertyListSize)
					{
						AdapterBase.ParseProperties(propertiesPayload.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: true);
					});
					propertiesPayload.VirtualPropertyPayloadStatus = VirtualPropertyPayloadStatus.Success;
				});
			}
			catch (ClusterObjectNotFoundException)
			{
				propertiesPayload.VirtualPropertyPayloadStatus = VirtualPropertyPayloadStatus.Deleted;
				propertiesPayload.Properties.Clear();
			}
			catch (ClusterObjectDeletingException)
			{
				propertiesPayload.VirtualPropertyPayloadStatus = VirtualPropertyPayloadStatus.Deleted;
				propertiesPayload.Properties.Clear();
			}
			catch (ClusterException ex3)
			{
				ClusterLog.LogException(ex3, "There was an error fetching the virtual private property payload for object {0}", propertiesPayload.Name);
				propertiesPayload.VirtualPropertyPayloadStatus = VirtualPropertyPayloadStatus.Error;
				propertiesPayload.Error = ex3;
				propertiesPayload.Properties.Clear();
			}
			catch (Exception ex4)
			{
				ClusterLog.LogException(ex4, "There was an error fetching the virtual private property payload for object {0}", propertiesPayload.Name);
				propertiesPayload.VirtualPropertyPayloadStatus = VirtualPropertyPayloadStatus.Error;
				propertiesPayload.Error = new ClusterDefaultException(ex4);
				propertiesPayload.Properties.Clear();
				throw;
			}
		}

		public bool NotificationArrived(NativeMethods.NOTIFY_FILTER_AND_TYPE filterType, string objectName, string objectId, string parentId, string objectType, IntPtr buffer, int bufferSize)
		{
			if (filterType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_GROUP && filterType.FilterFlags == 64)
			{
				Guid resourceId2 = new Guid(objectId);
				ResourceKind value2 = PResourceType.StringToResourceKind(objectType);
				string text = Marshal.PtrToStringUni(buffer);
				if (text == null)
				{
					throw new ClusterDefaultException("Owner group buffer is null");
				}
				ClusterAddedEventArgs payload = new ClusterAddedEventArgs(parentId: new Guid(text), id: resourceId2, name: objectName, objectType: (int)value2, objectTypeName: objectType, exception: null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster
				};
				clusApiAdapter.EnqueueNotification(new ResourceNotification(payload));
				lock (loadingResourceLock)
				{
					clusApiAdapter.MappingIdNameResource.AddOrUpdate(resourceId2, objectName, (Guid key, string value) => objectName);
					clusApiAdapter.MappingNameIdResource.AddOrUpdate(objectName, resourceId2, (string key, Guid value) => resourceId2);
				}
				try
				{
					ExecuteOnResource(resourceId2, objectName, delegate(SafeClusterResourceHandle resourceHandle)
					{
						RequiredDependencies requiredDependencies = GetRequiredDependencies(resourceHandle, objectName);
						clusApiAdapter.EnqueueNotification(new ResourceNotification(new ClusterRequiredDependenciesEventArgs(resourceId2, requiredDependencies, null)));
					});
				}
				catch (ClusterObjectNotFoundException)
				{
					return true;
				}
				catch (ClusterObjectDeletingException)
				{
					return true;
				}
				return true;
			}
			if (filterType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_SHARED_VOLUME && (filterType.FilterFlags == 2 || filterType.FilterFlags == 4))
			{
				Guid id = new Guid(objectId);
				Guid parentId3 = new Guid(parentId);
				ClusterAddedEventArgs clusterAddedEventArgs = null;
				if (filterType.FilterFlags == 2)
				{
					clusterAddedEventArgs = new ClusterUpsertEventArgs(id, objectName, 65539, "cluster file system", parentId3, null);
				}
				else if (filterType.FilterFlags == 4)
				{
					clusterAddedEventArgs = new ClusterUpsertEventArgs(id, objectName, 3, objectType, parentId3, null);
				}
				if (clusterAddedEventArgs == null)
				{
					return false;
				}
				clusterAddedEventArgs.Cluster = clusApiAdapter.clusterAdapter.Cluster;
				clusApiAdapter.EnqueueNotification(new ResourceNotification(clusterAddedEventArgs));
				return true;
			}
			if (filterType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_SHARED_VOLUME && filterType.FilterFlags == 1)
			{
				Guid id2 = new Guid(objectId);
				ClusterSharedVolumeStateInfo clusterSharedVolumeStateInfo = new ClusterSharedVolumeStateInfo();
				ParseCsvInfoValueList(buffer, bufferSize, clusterSharedVolumeStateInfo);
				clusApiAdapter.EnqueueNotification(new ResourceNotification(new ClusterResourceSharedVolumeStateEventArgs(id2, clusterSharedVolumeStateInfo, null)));
				return true;
			}
			if (filterType.ObjectType != NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_RESOURCE)
			{
				return false;
			}
			NativeMethods.CLUSTER_CHANGE_RESOURCE_V2 filterFlags = (NativeMethods.CLUSTER_CHANGE_RESOURCE_V2)filterType.FilterFlags;
			Guid resourceId;
			ClusterPropertyCollection clusterPropertyCollection;
			switch (filterFlags)
			{
			case (NativeMethods.CLUSTER_CHANGE_RESOURCE_V2)0uL:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_COMMON_PROPERTY_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_PRIVATE_PROPERTY_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_PRIVATE_PROPERTY_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_STATE_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_STATE_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_PRIVATE_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_STATE_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_PRIVATE_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_STATE_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_OWNER_GROUP_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_OWNER_GROUP_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_PRIVATE_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_OWNER_GROUP_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_PRIVATE_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_OWNER_GROUP_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_STATE_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_OWNER_GROUP_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_STATE_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_OWNER_GROUP_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_PRIVATE_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_STATE_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_OWNER_GROUP_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_PRIVATE_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_STATE_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_OWNER_GROUP_V2:
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_DEPENDENCIES_V2:
			{
				NativeMethods.CLUSTER_CHANGE_RESOURCE_V2 num = filterFlags - 1;
				if (num <= (NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_PRIVATE_PROPERTY_V2))
				{
					switch (num)
					{
					case (NativeMethods.CLUSTER_CHANGE_RESOURCE_V2)0uL:
						goto IL_02f5;
					case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_COMMON_PROPERTY_V2:
					{
						Guid guid2 = new Guid(objectId);
						ClusterPropertiesEventArgs orAdd = virtualPrivatePropertiesPayloadCache.GetOrAdd(guid2, new ClusterPropertiesEventArgs(guid2, objectName, null, null)
						{
							Cluster = clusApiAdapter.clusterAdapter.Cluster,
							IsVirtual = true,
							Properties = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, guid2, ClusterIdentityType.Resource),
							ObjectType = (int)PResourceType.StringToResourceKind(objectType),
							PropertyKind = ClusterPropertyKind.Private
						});
						clusApiAdapter.EnqueueNotification(new ResourceNotification(orAdd));
						return true;
					}
					case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_COMMON_PROPERTY_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_PRIVATE_PROPERTY_V2:
					{
						Guid id3 = new Guid(objectId);
						ResourceState value5 = (ResourceState)Marshal.ReadInt32(buffer);
						clusApiAdapter.EnqueueNotification(new ResourceNotification(new ClusterResourceStateEventArgs(id3, value5, null)));
						return true;
					}
					case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_PRIVATE_PROPERTY_V2:
						goto end_IL_0292;
					}
				}
				switch (filterFlags)
				{
				case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_OWNER_GROUP_V2:
				{
					Guid groupId = new Guid(parentId);
					Guid id5 = new Guid(objectId);
					clusApiAdapter.EnqueueNotification(new ResourceNotification(new ClusterResourceOwnerGroupEventArgs(id5, groupId, null)));
					return true;
				}
				case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_DEPENDENCIES_V2:
				{
					string text2 = Marshal.PtrToStringUni(buffer);
					List<Guid> dependencies = (from Match m in new Regex("\\[[-0-9a-fA-F]*\\]").Matches(text2)
						select new Guid(m.Value.Substring(1, 36))).ToList();
					Guid id4 = new Guid(objectId);
					clusApiAdapter.EnqueueNotification(new ResourceNotification(new ClusterDependenciesEventArgs(id4, dependencies, null)));
					clusApiAdapter.EnqueueNotification(new ResourceNotification(new ClusterDependencyRelationshipEventArgs(id4, text2, null)));
					return true;
				}
				}
				break;
			}
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_DEPENDENTS_V2:
			{
				List<Guid> dependants = (from s in ReadMultiSzProp(buffer)
					select new Guid(s)).ToList();
				Guid id7 = new Guid(objectId);
				clusApiAdapter.EnqueueNotification(new ResourceNotification(new ClusterDependentsEventArgs(id7, dependants, null)));
				return true;
			}
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_POSSIBLE_OWNERS_V2:
			{
				Guid id6 = new Guid(objectId);
				List<Guid> possibleOwners = GetPossibleOwners(id6);
				clusApiAdapter.EnqueueNotification(new ResourceNotification(new ClusterResourcePossibleOwnersChangedEventArgs(id6, possibleOwners, null)));
				return true;
			}
			case NativeMethods.CLUSTER_CHANGE_RESOURCE_V2.CLUSTER_CHANGE_RESOURCE_DELETED_V2:
				{
					Guid guid = new Guid(objectId);
					lock (loadingResourceLock)
					{
						clusApiAdapter.MappingIdNameResource.TryRemove(guid, out var _);
						clusApiAdapter.MappingNameIdResource.TryRemove(objectName, out var _);
					}
					clusApiAdapter.EnqueueNotification(new ResourceNotification(new ClusterRemovedEventArgs(guid, objectName, null)
					{
						Cluster = clusApiAdapter.clusterAdapter.Cluster
					}));
					return true;
				}
				IL_02f5:
				if (bufferSize == 0)
				{
					return true;
				}
				resourceId = new Guid(objectId);
				clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, resourceId, ClusterIdentityType.Resource)
				{
					Partial = true
				};
				AdapterBase.ParseProperties(clusterPropertyCollection, buffer, bufferSize, ClusterPropertyKind.Common, readOnly: false);
				clusterPropertyCollection.Get("Name", delegate(ClusterPropertyString newName)
				{
					clusApiAdapter.EnqueueNotification(new ResourceNotification(new ClusterRenamedEventArgs(resourceId, newName.TypedValue, null)));
				});
				clusApiAdapter.EnqueueNotification(new ResourceNotification(new ClusterPropertiesEventArgs(resourceId, objectName, null, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster,
					Properties = clusterPropertyCollection,
					ObjectType = (int)PResourceType.StringToResourceKind(objectType),
					PropertyKind = ClusterPropertyKind.Common
				}));
				return true;
				end_IL_0292:
				break;
			}
			return false;
		}

		public string GetVirtualMachineOwnerGroup(Guid vmId)
		{
			string ownerGroup = string.Empty;
			clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
			{
				ownerGroup = wmiAdapter.Resource.GetVirtualMachineOwnerGroup(vmId);
			});
			return ownerGroup;
		}

		private void VirtualMachineSetOfflineAction(PVirtualMachineResource resource, VirtualMachineOfflineAction offlineAction)
		{
			ExecuteOnResource(Guid.Empty, resource.Name, delegate(SafeClusterResourceHandle resourceHandle)
			{
				using NativeMethods.UnmanagedBuffer unmanagedBuffer = NativeMethods.UnmanagedBuffer.Create((int)offlineAction);
				if (unmanagedBuffer.IsMemoryValid)
				{
					ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_VM_SET_NEXT_OFFLINE_ACTION, resource.Name, unmanagedBuffer, null);
				}
			});
		}

		public void VirtualMachineTurnOff(PVirtualMachineResource resource)
		{
			VirtualMachineSetOfflineAction(resource, VirtualMachineOfflineAction.Turnoff);
			Offline(resource.Id);
		}

		public void VirtualMachineSave(PVirtualMachineResource resource)
		{
			VirtualMachineSetOfflineAction(resource, VirtualMachineOfflineAction.Save);
			Offline(resource.Id);
		}

		public void VirtualMachineShutdown(PVirtualMachineResource resource)
		{
			VirtualMachineSetOfflineAction(resource, VirtualMachineOfflineAction.Shutdown);
			Offline(resource.Id);
		}

		public void VirtualMachineMoveStorage(Guid resourceId, string hostName, VirtualMachineStorageMoveParameters virtualMachineStorageMoveParameters)
		{
			clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
			{
				wmiAdapter.Resource.VirtualMachineMoveStorage(resourceId, hostName, virtualMachineStorageMoveParameters);
			});
		}

		public void VirtualMachineRefreshSettings(Guid resourceId, string hostName)
		{
			clusApiAdapter.ExecuteOnWmi(delegate(WmiAdapter wmiAdapter)
			{
				wmiAdapter.Resource.VirtualMachineRefreshSettings(resourceId, hostName);
			});
		}

		private void LoadPoolInfo(PStorageResource resource)
		{
			if (clusApiAdapter.clusterAdapter.Handle == null)
			{
				return;
			}
			try
			{
				if (!(resource.GetPoolId() != Guid.Empty))
				{
					return;
				}
				ExecuteOnResource(resource.GetPoolId(), resource.Name, delegate(SafeClusterResourceHandle resourceHandle)
				{
					ClusterPropertyCollection properties = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, resource.Id, ClusterIdentityType.Resource);
					ExecuteOnReadOnlyPrivateProperties(resourceHandle, resource.Name, delegate(IntPtr propertyList, int propertyListSize)
					{
						AdapterBase.ParseProperties(properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: true);
					});
					PoolInfo poolInfo = new PoolInfo();
					ClusterPropertyString clusterPropertyString = (ClusterPropertyString)properties["Name"];
					ClusterPropertyString clusterPropertyString2 = (ClusterPropertyString)properties["Description"];
					poolInfo.PoolName = clusterPropertyString.TypedValue;
					poolInfo.PoolDescription = clusterPropertyString2.TypedValue;
					resource.PoolInfo = poolInfo;
				});
			}
			catch (ClusterObjectLoadFailedException exception)
			{
				ClusterLog.LogException(exception, "There was an error loading the pool information for disk {0}".FormatCurrentCulture(resource.Name));
				resource.PoolInfo = null;
				throw;
			}
			catch (ClusterObjectNotFoundException exception2)
			{
				ClusterLog.LogException(exception2, "There was an error loading the pool information for disk {0}".FormatCurrentCulture(resource.Name));
				resource.PoolInfo = null;
				throw;
			}
		}

		private void LoadDisk(PResource resource)
		{
			if (resource.ResourceState != ResourceState.Online)
			{
				resource.DiskInfo = new ClusterDisk();
			}
			else
			{
				if (clusApiAdapter.clusterAdapter.Handle == null)
				{
					return;
				}
				try
				{
					ExecuteOnResource(resource.Id, resource.Name, delegate(SafeClusterResourceHandle resourceHandle)
					{
						ClusterDisk clusterDisk = LoadDiskFromCluster(resourceHandle, resource, includeMountPoints: true);
						if (resource is PCsvVolumeResource)
						{
							ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_STORAGE_GET_SHARED_VOLUME_INFO, resource.Name, delegate(IntPtr buffer, int bufferSize)
							{
								ParseCsvInfoValueList(buffer, bufferSize, clusterDisk);
							});
						}
						resource.DiskInfo = clusterDisk;
					});
				}
				catch (ClusterException exception)
				{
					ClusterLog.LogException(exception, "There was an error loading the disk information for disk {0}".FormatCurrentCulture(resource.Name));
					resource.DiskInfo = new ClusterDisk();
				}
			}
		}

		private List<uint> ParseDirtyList(IntPtr valueList, int valueListSize)
		{
			List<uint> dirtyDiskList = new List<uint>();
			EnumerateValueList(valueList, valueListSize, delegate(ValueListIterator iterator)
			{
				switch (iterator.IteratorType)
				{
				case 65538u:
				{
					NativeMethods.CLUSPROP_DWORD cLUSPROP_DWORD = (NativeMethods.CLUSPROP_DWORD)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_DWORD));
					dirtyDiskList.Add(cLUSPROP_DWORD.dw);
					break;
				}
				default:
					ClusterLog.LogError("Unknown syntax code - {0}", iterator.IteratorType);
					break;
				case 0u:
					break;
				}
			});
			return dirtyDiskList;
		}

		private ClusterDisk ParseClusterDiskValueList(SafeClusterResourceHandle resourceHandle, PResource resource, bool includeMountPoints, IntPtr buffer, int bufferSize)
		{
			List<ClusterDisk> list = ParseClusterableDisks(buffer, bufferSize);
			if (list.Count == 1)
			{
				ClusterDisk clusterDisk = list[0];
				try
				{
					ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_STORAGE_GET_DIRTY, resource.Name, delegate(IntPtr bufferGetDirty, int bufferSizeGetDirty)
					{
						List<uint> list2 = ParseDirtyList(bufferGetDirty, bufferSizeGetDirty);
						foreach (ClusterDiskPartition partition in clusterDisk.Partitions)
						{
							partition.IsDirty = (list2.Contains(partition.PartitionNumber) ? DirtyState.Dirty : DirtyState.NotDirty);
						}
					});
				}
				catch (ClusterException ex)
				{
					Win32Exception firstException = ExceptionHelper.GetFirstException<Win32Exception>(ex);
					if (firstException == null || (!NativeMethods.ErrorCode.InvalidState.IsEqual(firstException.NativeErrorCode) && !NativeMethods.ErrorCode.InvalidFunction.IsEqual(firstException.NativeErrorCode) && !NativeMethods.ErrorCode.FileNotFound.IsEqual(firstException.NativeErrorCode) && !NativeMethods.ErrorCode.InvalidParameter.IsEqual(firstException.NativeErrorCode)))
					{
						throw new ClusterGetDiskIsDirtyException(resource.Name, ex);
					}
				}
				if (includeMountPoints && resource.ResourceState == ResourceState.Online)
				{
					GetMountPoints(resourceHandle, resource, list[0]);
				}
				return clusterDisk;
			}
			throw new ClusterGetDiskException(resource.Name);
		}

		private void SetPartitionName(PResource resource, ClusterDisk clusterDisk)
		{
			string clusterSharedVolumeRoot = string.Empty;
			resource.Cluster.Properties.GetOrNull("SharedVolumesRoot", delegate(ClusterPropertyString propertyValue)
			{
				if (propertyValue != null)
				{
					clusterSharedVolumeRoot = propertyValue.ToString();
				}
				else
				{
					ClusterLog.LogCritical("Cluster Properties do not contain {0}", "SharedVolumesRoot");
				}
			});
			foreach (ClusterDiskPartition item in clusterDisk.Partitions.Where((ClusterDiskPartition _) => _.FileSystem == "CSVFS" && _.MountPoints.Count >= 1))
			{
				string text = string.Empty;
				if (!string.IsNullOrEmpty(clusterSharedVolumeRoot))
				{
					text = item.MountPoints.FirstOrDefault((string mp) => mp.StartsWith(clusterSharedVolumeRoot, StringComparison.OrdinalIgnoreCase));
				}
				item.Name = (string.IsNullOrEmpty(text) ? item.MountPoints[0] : text);
			}
		}

		private void GetMountPoints(SafeClusterResourceHandle resourceHandle, PResource resource, ClusterDisk clusterDisk)
		{
			if (resource.ResourceState != ResourceState.Online)
			{
				return;
			}
			foreach (ClusterDiskPartition partition2 in clusterDisk.Partitions)
			{
				ClusterDiskPartition partition = partition2;
				if (!partition.IsPartitionNumberValid)
				{
					continue;
				}
				using NativeMethods.UnmanagedBuffer inBuffer = NativeMethods.UnmanagedBuffer.Create((int)partition.PartitionNumber);
				try
				{
					ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_STORAGE_GET_MOUNTPOINTS, resource.Name, inBuffer, delegate(IntPtr volumeBuffer, int volumeBufferSize)
					{
						char[] array = new char[volumeBufferSize / 2];
						Marshal.Copy(volumeBuffer, array, 0, array.Length);
						List<string> list = new List<string>();
						int num = 0;
						int num2 = volumeBufferSize / 2;
						while (num < num2)
						{
							int i;
							for (i = num; i < num2 && array[i] != 0; i++)
							{
							}
							if (i < num2)
							{
								if (i - num > 0)
								{
									list.Add(new string(array, num, i - num));
								}
							}
							else
							{
								list.Add(new string(array, num, num2 - num));
							}
							num = i + 1;
						}
						partition.SetMountPoints(list);
						partition.IncludeMountPoints = true;
					});
				}
				catch (ClusterException ex)
				{
					Win32Exception firstException = ExceptionHelper.GetFirstException<Win32Exception>(ex);
					if (firstException != null && NativeMethods.ErrorCode.InvalidFunction.IsEqual(firstException.NativeErrorCode))
					{
						ClusterLog.LogException(ex, "Resource '{0}' does not support mount points.", resource.Name);
						continue;
					}
					if (firstException != null)
					{
						ClusterLog.LogException(ex, "Resource '{0}' is in an invalid state to retrieve mount points.", resource.Name);
						continue;
					}
					throw;
				}
			}
		}

		private List<ClusterDisk> ParseClusterableDisks(IntPtr valueList, int valueListSize)
		{
			List<ClusterDisk> diskList = new List<ClusterDisk>();
			ClusterDisk disk = null;
			EnumerateValueList(valueList, valueListSize, delegate(ValueListIterator iterator)
			{
				switch (iterator.IteratorType)
				{
				case 327682u:
					disk = new ClusterDisk(((NativeMethods.CLUSPROP_DISK_SIGNATURE)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_DISK_SIGNATURE))).dw);
					diskList.Add(disk);
					break;
				case 458754u:
				{
					NativeMethods.CLUSPROP_DISK_NUMBER cLUSPROP_DISK_NUMBER = (NativeMethods.CLUSPROP_DISK_NUMBER)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_DISK_NUMBER));
					disk.DiskNumber = cLUSPROP_DISK_NUMBER.dw;
					break;
				}
				case 524289u:
				{
					NativeMethods.CLUSPROP_PARTITION_INFO partitionInfo = (NativeMethods.CLUSPROP_PARTITION_INFO)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_PARTITION_INFO));
					ClusterDiskPartition item = new ClusterDiskPartition(disk, partitionInfo);
					disk.Partitions.Add(item);
					break;
				}
				case 851969u:
				{
					NativeMethods.CLUSPROP_PARTITION_INFO_EX partitionInfoEx2 = (NativeMethods.CLUSPROP_PARTITION_INFO_EX)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_PARTITION_INFO_EX));
					ClusterDiskPartition item = new ClusterDiskPartition(disk, partitionInfoEx2);
					disk.Partitions.Add(item);
					break;
				}
				case 917505u:
				{
					NativeMethods.CLUSPROP_PARTITION_INFO_EX2 partitionInfoEx = (NativeMethods.CLUSPROP_PARTITION_INFO_EX2)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_PARTITION_INFO_EX2));
					ClusterDiskPartition item = new ClusterDiskPartition(disk, partitionInfoEx);
					disk.Partitions.Add(item);
					break;
				}
				case 720899u:
				{
					Guid diskId = new Guid(((NativeMethods.CLUSPROP_SZ)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_SZ))).sz);
					disk = new ClusterDisk(diskId);
					diskList.Add(disk);
					break;
				}
				case 786438u:
				{
					NativeMethods.CLUSPROP_ULARGE_INTEGER cLUSPROP_ULARGE_INTEGER = (NativeMethods.CLUSPROP_ULARGE_INTEGER)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_ULARGE_INTEGER));
					disk.Size = cLUSPROP_ULARGE_INTEGER.li;
					break;
				}
				default:
					ClusterLog.LogError("Unknown syntax code - {0}", iterator.IteratorType);
					break;
				case 0u:
				case 393218u:
					break;
				}
			});
			return diskList;
		}

		private ClusterDisk LoadDiskFromCluster(SafeClusterResourceHandle resourceHandle, PResource resource, bool includeMountPoints)
		{
			ClusterDisk clusterDisk = null;
			ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_STORAGE_GET_DISK_INFO_EX2, resource.Name, delegate(IntPtr buffer, int bufferSize)
			{
				clusterDisk = ParseClusterDiskValueList(resourceHandle, resource, includeMountPoints, buffer, bufferSize);
			}, delegate
			{
				ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_STORAGE_GET_DISK_INFO_EX, resource.Name, delegate(IntPtr buffer, int bufferSize)
				{
					clusterDisk = ParseClusterDiskValueList(resourceHandle, resource, includeMountPoints, buffer, bufferSize);
				}, delegate
				{
					ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_STORAGE_GET_DISK_INFO, resource.Name, delegate(IntPtr buffer, int bufferSize)
					{
						clusterDisk = ParseClusterDiskValueList(resourceHandle, resource, includeMountPoints, buffer, bufferSize);
					});
				});
			});
			SetPartitionName(resource, clusterDisk);
			return clusterDisk;
		}

		public void SetMaintenanceMode(PStorageResource storageResourcePrivate, bool maintenanceMode)
		{
			ExecuteOnResource(storageResourcePrivate.Id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				NativeMethods.CLUSTER_MAINTENANCE_MODE_INFO obj = default(NativeMethods.CLUSTER_MAINTENANCE_MODE_INFO);
				obj.bInMaintenance = maintenanceMode;
				using NativeMethods.UnmanagedBuffer unmanagedBuffer = NativeMethods.UnmanagedBuffer.Create(obj);
				if (unmanagedBuffer.IsMemoryValid)
				{
					try
					{
						ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_SET_MAINTENANCE_MODE, storageResourcePrivate.Name, unmanagedBuffer, null);
						return;
					}
					catch (ClusterControlCodeException innerException)
					{
						throw new ClusterStorageSetMaintenanceModeException(maintenanceMode ? ExceptionResources.ClusterStorageTurnOnMaintenanceModeException_Text.FormatCurrentCulture(storageResourcePrivate.Name) : ExceptionResources.ClusterStorageTurnOffMaintenanceModeException_Text.FormatCurrentCulture(storageResourcePrivate.Name), innerException);
					}
				}
			});
		}

		public uint GetDiskNumber(PStorageResource storageResourcePrivate, string nodeName)
		{
			uint? diskNumber = null;
			base.ClusApiAdapter.nodes.ExecuteOnNode(Guid.Empty, nodeName, delegate(SafeClusterNodeHandle nodeHandle)
			{
				ExecuteOnResource(storageResourcePrivate.Id, null, delegate(SafeClusterResourceHandle resourceHandle)
				{
					ExecuteOnControlCode(resourceHandle, nodeHandle, NativeMethods.CLUSCTL_RESOURCE_STORAGE_GET_DISK_NUMBER_INFO, storageResourcePrivate.Name, delegate(IntPtr buffer, int bufferSize)
					{
						diskNumber = ((NativeMethods.CLUS_DISK_NUMBER_INFO)Marshal.PtrToStructure(buffer, typeof(NativeMethods.CLUS_DISK_NUMBER_INFO))).diskNumber;
					});
				});
			});
			return diskNumber.Value;
		}

		public CsvVolumeInformation GetCsvVolumeInformation(PCsvVolumeResource csvVolumeResourcePrivate)
		{
			NativeMethods.CLUS_CSV_VOLUME_INFO? csvVolumeInfo = null;
			ExecuteOnResource(csvVolumeResourcePrivate.Id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_STORAGE_GET_SHARED_VOLUME_INFO, csvVolumeResourcePrivate.Name, delegate(IntPtr buffer, int bufferSize)
				{
					csvVolumeInfo = (NativeMethods.CLUS_CSV_VOLUME_INFO)Marshal.PtrToStructure(buffer, typeof(NativeMethods.CLUS_CSV_VOLUME_INFO));
				});
			});
			return new CsvVolumeInformation(csvVolumeInfo.Value);
		}

		public void SetCsvRedirectedAccess(PCsvVolumeResource csvResourcePrivate, Guid deviceId, bool csvRedirectedAccessMode)
		{
			ExecuteOnResource(csvResourcePrivate.Id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				using NativeMethods.UnmanagedBuffer unmanagedBuffer = NativeMethods.UnmanagedBuffer.Create("\\\\?\\Volume{{{0}}}\\".FormatInvariantCulture(deviceId.ToString()));
				using NativeMethods.UnmanagedBuffer unmanagedBuffer2 = new NativeMethods.UnmanagedBuffer(102);
				if (unmanagedBuffer.IsMemoryValid && unmanagedBuffer2.IsMemoryValid)
				{
					try
					{
						string resourceNameFromId = GetResourceNameFromId(csvResourcePrivate.Id);
						ExecuteOnControlCode(resourceHandle, csvRedirectedAccessMode ? NativeMethods.CLUSCTL_RESOURCE_DISABLE_SHARED_VOLUME_DIRECTIO : NativeMethods.CLUSCTL_RESOURCE_ENABLE_SHARED_VOLUME_DIRECTIO, resourceNameFromId, unmanagedBuffer, null);
						return;
					}
					catch (ClusterControlCodeException ex)
					{
						if (ex.InnerException is Win32Exception ex2 && (NativeMethods.ErrorCode.InvalidParameter.IsEqual(ex2.NativeErrorCode) || NativeMethods.ErrorCode.ClusterInvalidRequest.IsEqual(ex2.NativeErrorCode)))
						{
							throw new ClusterDefaultException(ExceptionResources.CsvInMaintenanceModeFailedSettingRedirectedAccess, ExceptionResources.CsvSetRedirectedAccessFailedHeader, ex)
							{
								Icon = TaskDialogStandardIcon.Information
							};
						}
						throw;
					}
				}
			});
		}

		public void AddToClusterSharedVolumes(PStorageResource storageResourcePrivate)
		{
			ExecuteOnResource(storageResourcePrivate.Id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				int num = NativeMethods.AddResourceToClusterSharedVolumes(resourceHandle);
				if (!NativeMethods.ErrorCode.None.IsEqual(num))
				{
					throw ExceptionHelper.Build<ClusterStorageAddToClusterSharedVolumeException>(NativeMethods.ErrorCode.DiskNotCsvCapable.IsEqual(num) ? ExceptionResources.ClusterStorageAddToClusterSharedVolumeDiskNotCsvCapableException_Text : ExceptionResources.ClusterStorageAddToClusterSharedVolumeException_Text.FormatCurrentCulture(storageResourcePrivate.Name), num);
				}
			});
		}

		public void RemoveFromClusterSharedVolumes(PCsvVolumeResource csvVolumeResourcePrivate)
		{
			ExecuteOnResource(csvVolumeResourcePrivate.Id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				int num = NativeMethods.RemoveResourceFromClusterSharedVolumes(resourceHandle);
				if (NativeMethods.ErrorCode.ResourceLocked.IsEqual(num))
				{
					throw new ClusterResourceLockedException(csvVolumeResourcePrivate.Name, maintenanceMode: false);
				}
				if (!NativeMethods.ErrorCode.None.IsEqual(num))
				{
					throw ExceptionHelper.Build<RemoveFromClusterSharedVolumesException>(ExceptionResources.ClusterStorageRemoveFromClusterSharedVolumeException_Text.FormatCurrentCulture(csvVolumeResourcePrivate.Name), num);
				}
			});
		}

		public void NetworkNameRepairActiveDirectoryObject(Guid id)
		{
			ExecuteOnResource(id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_NETNAME_REPAIR_VCO, null, null, null, null);
			});
		}

		public void NetworkNameEnableAdObject(PNetNameResource privateCNOResource)
		{
			List<PNode> clusterNodes = (from node in clusApiAdapter.Node.GetAll(nullElementOnError: false)
				where node.State == NodeState.Up
				select node).ToList();
			string clusterDomainName = GetClusterDomainName(clusterNodes);
			string cnoNetBiosName = string.Empty;
			ExecuteOnResource(privateCNOResource.Id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				ExecuteOnPrivateProperties(resourceHandle, privateCNOResource.Name, delegate(IntPtr privatePropList, int privatePropListSize)
				{
					ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, privateCNOResource.Id, ClusterIdentityType.Resource);
					AdapterBase.ParseProperties(clusterPropertyCollection, privatePropList, privatePropListSize, ClusterPropertyKind.Private, readOnly: false);
					cnoNetBiosName = clusterPropertyCollection["Name"].Value.ToString();
				});
			});
			if (cnoNetBiosName != string.Empty)
			{
				ActiveDirectory.EnableComputerObject(privateCNOResource.Name, cnoNetBiosName, clusterDomainName);
			}
		}

		public void NetworkNameResetCnoPassword(PNetNameResource netNameResourcePrivate)
		{
			ExecuteOnResource(netNameResourcePrivate.Id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				ExecuteOnPrivateProperties(resourceHandle, netNameResourcePrivate.Name, delegate(IntPtr privatePropList, int privatePropListSize)
				{
					ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, netNameResourcePrivate.Id, ClusterIdentityType.Resource);
					AdapterBase.ParseProperties(clusterPropertyCollection, privatePropList, privatePropListSize, ClusterPropertyKind.Private, readOnly: false);
					string dnsName = clusterPropertyCollection["DnsName"].Value.ToString();
					int num = NativeMethods.ResetCnoPassword(resourceHandle, dnsName);
					if (!NativeMethods.ErrorCode.None.IsEqual(num))
					{
						throw ExceptionHelper.Build<ResetCoreNetworkNamePasswordException>(ExceptionResources.NetworkNameResetCnoPasswordException_Text.FormatCurrentCulture(netNameResourcePrivate.Name), num);
					}
				});
			});
		}

		public void NetworkNameRepairReAclDNSRecords(PNetNameResource privateCNOResource)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			List<string> networkDNSNames = new List<string>();
			string cnoNetBiosName = string.Empty;
			foreach (PNetNameResource privateNetNameRes in GetResourcesByResourceType(handle, null, ResourceKind.NetworkName))
			{
				ExecuteOnResource(privateNetNameRes.Id, null, delegate(SafeClusterResourceHandle resourceHandle)
				{
					ExecuteOnPrivateProperties(resourceHandle, privateNetNameRes.Name, delegate(IntPtr privatePropList, int privatePropListSize)
					{
						ClusterPropertyCollection clusterPropertyCollection3 = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, privateNetNameRes.Id, ClusterIdentityType.Resource);
						AdapterBase.ParseProperties(clusterPropertyCollection3, privatePropList, privatePropListSize, ClusterPropertyKind.Private, readOnly: false);
						networkDNSNames.Add(clusterPropertyCollection3["DnsName"].Value.ToString());
					});
				});
			}
			foreach (PNetNameResource privateDistributedNetNameRes in GetResourcesByResourceType(handle, null, ResourceKind.DistributedNetworkName))
			{
				ExecuteOnResource(privateDistributedNetNameRes.Id, null, delegate(SafeClusterResourceHandle resourceHandle)
				{
					ExecuteOnPrivateProperties(resourceHandle, privateDistributedNetNameRes.Name, delegate(IntPtr privatePropList, int privatePropListSize)
					{
						ClusterPropertyCollection clusterPropertyCollection2 = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, privateDistributedNetNameRes.Id, ClusterIdentityType.Resource);
						AdapterBase.ParseProperties(clusterPropertyCollection2, privatePropList, privatePropListSize, ClusterPropertyKind.Private, readOnly: false);
						networkDNSNames.Add(clusterPropertyCollection2["DnsName"].Value.ToString());
					});
				});
			}
			ExecuteOnResource(privateCNOResource.Id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				ExecuteOnPrivateProperties(resourceHandle, privateCNOResource.Name, delegate(IntPtr privatePropList, int privatePropListSize)
				{
					ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, privateCNOResource.Id, ClusterIdentityType.Resource);
					AdapterBase.ParseProperties(clusterPropertyCollection, privatePropList, privatePropListSize, ClusterPropertyKind.Private, readOnly: false);
					cnoNetBiosName = clusterPropertyCollection["Name"].Value.ToString();
				});
			});
			List<PNode> list = (from node in clusApiAdapter.Node.GetAll(nullElementOnError: false)
				where node.State == NodeState.Up
				select node).ToList();
			string clusterDomainName = GetClusterDomainName(list);
			HashSet<string> hashSet = new HashSet<string>();
			foreach (PNode node2 in list)
			{
				try
				{
					List<string> tempSuffixes = null;
					clusApiAdapter.ExecuteOnWmi((WmiAdapter wmiAdapter) => tempSuffixes = wmiAdapter.NetworkInterface.GetNodeDnsSuffixes(node2.Name));
					hashSet.UnionWith(tempSuffixes);
				}
				catch (ClusterException exception)
				{
					ClusterLog.LogException(LogLevel.Info, exception, "Could not get dns suffixes for node {0}", node2.Name);
				}
			}
			hashSet.Add(NetworkHelper.GetDnsSuffixFromFullyQualifiedDomainName(clusApiAdapter.Cluster.GetFullyQualifiedDomainName()));
			IdentityReference computerObjectIdentity = ActiveDirectory.GetComputerObjectIdentity(privateCNOResource.Name, cnoNetBiosName, clusterDomainName);
			foreach (string item in networkDNSNames)
			{
				foreach (string item2 in hashSet)
				{
					DirectoryEntry dnsADObject = ActiveDirectory.GetDnsADObject(clusterDomainName, item2, item, SearchZone.DomainDnsZone);
					if (dnsADObject == null)
					{
						ClusterLog.LogInfo("Could not locate DNS AD object '{0}' in well known location(domainDnsZone), for activeDirectoryDomain '{1}' and dnsDomain '{2}'.", item, clusterDomainName, item2);
						dnsADObject = ActiveDirectory.GetDnsADObject(clusterDomainName, item2, item, SearchZone.System);
						if (dnsADObject == null)
						{
							ClusterLog.LogInfo("Could not locate DNS AD object '{0}' in well known location(system), for activeDirectoryDomain '{1}' and dnsDomain '{2}'.", item, clusterDomainName, item2);
						}
					}
					using (dnsADObject)
					{
						if (dnsADObject == null)
						{
							continue;
						}
						bool flag = true;
						foreach (ActiveDirectoryAccessRule accessRule in dnsADObject.ObjectSecurity.GetAccessRules(includeExplicit: true, includeInherited: true, typeof(SecurityIdentifier)))
						{
							if (accessRule.IdentityReference == computerObjectIdentity && accessRule.ActiveDirectoryRights == ActiveDirectoryRights.GenericAll && accessRule.AccessControlType == AccessControlType.Allow)
							{
								flag = false;
								break;
							}
						}
						try
						{
							if (flag)
							{
								ActiveDirectory.AddAccess(dnsADObject, computerObjectIdentity);
								ClusterLog.LogInfo("Added access to DNS AD object.\nDomainname: '{0}'\nDnssuffix: '{1}'\nDnsname: '{2}'", clusterDomainName, item2, item);
							}
						}
						catch (UnauthorizedAccessException exception2)
						{
							ClusterLog.LogException(LogLevel.Info, exception2, "Error in adding access to DNS AD object.\nDomainname: '{0}'\nDnssuffix: '{1}'\nDnsname: '{2}'", clusterDomainName, item2, item);
						}
					}
				}
			}
		}

		private string GetClusterDomainName(List<PNode> clusterNodes)
		{
			return clusterNodes.Where((PNode node) => node.State == NodeState.Up).Select(delegate(PNode node)
			{
				try
				{
					string tempDomainName2 = null;
					clusApiAdapter.ExecuteOnWmi((WmiAdapter wmiAdapter) => tempDomainName2 = wmiAdapter.Node.GetDomainName(node.Name));
					return tempDomainName2;
				}
				catch (ClusterException exception)
				{
					ClusterLog.LogException(exception);
					return null;
				}
			}).FirstOrDefault((string tempDomainName) => tempDomainName != null);
		}

		private IEnumerable<TResult> GetResourcesFromGroup<TResult>(QueryInfo queryInfo, bool onlyVirtualMachine, Guid groupId, IList<string> queryFields)
		{
			ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Getting resources for group {0}", groupId.ToString());
			PGroup cacheGroup = null;
			using (ClusterLock clusterLock = clusApiAdapter.clusterAdapter.Cluster.CacheManager.Get(groupId, ClusterIdentityType.Group, LockAccess.Reader))
			{
				if (clusterLock != null)
				{
					cacheGroup = (PGroup)clusterLock.Owner;
					ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Group {0} found in cache", groupId.ToString());
				}
			}
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				yield break;
			}
			ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Call OpenClusterGroupEx for {0}", groupId.ToString());
			ClusterAccessRights grantedAccess;
			SafeClusterGroupHandle groupHandle = NativeMethods.OpenClusterGroupEx(handle, groupId.ToString(), clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess);
			if (groupHandle.IsInvalid)
			{
				throw ExceptionHelper.ClusterObjectLoadFailedException(null, groupId, Marshal.GetLastWin32Error());
			}
			GroupType groupType = cacheGroup?.GroupType ?? clusApiAdapter.groups.GetGroupType(groupHandle);
			try
			{
				ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Call OpenClusterGroupEx for {0}", groupId);
				SafeClusterGroupEnumHandle enumHandle = NativeMethods.ClusterGroupOpenEnum(groupHandle, NativeMethods.GroupEnumType.Resource);
				if (enumHandle.IsInvalid)
				{
					throw ExceptionHelper.Build<ClusterEnumerateResourcesException>(Marshal.GetLastWin32Error());
				}
				try
				{
					ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Call to ClusterGroupGetEnumCount for {0}", groupId);
					int records = NativeMethods.ClusterGroupGetEnumCount(enumHandle);
					for (int index = 0; index < records; index++)
					{
						if (queryInfo.IsCancel)
						{
							yield break;
						}
						NativeMethods.GroupEnumType enumType = NativeMethods.GroupEnumType.Resource;
						StringBuilder stringBuilder = new StringBuilder(200);
						int resourceNameSize = 200;
						int scCode = NativeMethods.ClusterGroupEnum(enumHandle, index, ref enumType, stringBuilder, ref resourceNameSize);
						if (NativeMethods.ErrorCode.MoreData.IsEqual(scCode))
						{
							stringBuilder = new StringBuilder(resourceNameSize);
							scCode = NativeMethods.ClusterGroupEnum(enumHandle, index, ref enumType, stringBuilder, ref resourceNameSize);
						}
						if (!NativeMethods.ErrorCode.None.IsEqual(scCode))
						{
							throw ExceptionHelper.Build<ClusterIterateResourcesException>(Marshal.GetLastWin32Error());
						}
						string resourceName = stringBuilder.ToString();
						PResource privateResource = CreateResource(resourceName, queryFields, groupHandle, cacheGroup, groupType == GroupType.ClusterSharedVolume);
						if (onlyVirtualMachine && privateResource.ResourceType.ResourceKind != ResourceKind.VirtualMachine)
						{
							continue;
						}
						if (privateResource.ResourceType.ResourceKind == ResourceKind.VirtualMachineConfiguration)
						{
							List<PResource> extraCsvResources = new List<PResource>();
							ExecuteOnResource(Guid.Empty, resourceName, delegate(SafeClusterResourceHandle resourceHandle)
							{
								ExecuteOnPrivateProperties(resourceHandle, resourceName, delegate(IntPtr privatePropList, int privatePropListSize)
								{
									ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, privateResource.Id, ClusterIdentityType.Resource);
									AdapterBase.ParseProperties(clusterPropertyCollection, privatePropList, privatePropListSize, ClusterPropertyKind.Private, readOnly: false);
									ClusterPropertyMultipleStrings clusterPropertyMultipleStrings = (ClusterPropertyMultipleStrings)clusterPropertyCollection["DependsOnSharedVolumes"];
									if (clusterPropertyMultipleStrings != null)
									{
										foreach (string item in clusterPropertyMultipleStrings.TypedValue)
										{
											string[] array = item.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
											if (array.Length != 0)
											{
												Guid guid = new Guid(array[0]);
												try
												{
													PResource pResource;
													using (ClusterLock clusterLock2 = clusApiAdapter.clusterAdapter.Cluster.CacheManager.Get(guid, ClusterIdentityType.Resource, LockAccess.Reader))
													{
														pResource = ((clusterLock2 == null) ? Open(guid) : ((PResource)clusterLock2.Owner));
													}
													if (pResource is PCsvVolumeResource)
													{
														pResource.LoadObject(1);
														extraCsvResources.Add(pResource);
													}
													else
													{
														ClusterLog.LogError("The dependent CSV '{0}' for the resource {1} was not found in the cluster".FormatCurrentCulture(guid, resourceName));
													}
												}
												catch (ClusterObjectNotFoundException exception)
												{
													ClusterLog.LogException(exception, "The dependent CSV '{0}' for the resource {1} was not found in the cluster".FormatCurrentCulture(guid, resourceName));
												}
											}
										}
									}
								});
							});
							foreach (PResource item2 in extraCsvResources)
							{
								PResource resource = item2;
								clusApiAdapter.groups.ExecuteOnGroup(resource.OwnerGroup.Id, null, delegate(SafeClusterGroupHandle csvGroupHandle)
								{
									clusApiAdapter.groups.SetStateAndNode(resource.OwnerGroup, csvGroupHandle);
								});
								yield return (TResult)(object)item2;
							}
						}
						yield return (TResult)(object)privateResource;
					}
				}
				finally
				{
					enumHandle.Dispose();
				}
			}
			finally
			{
				groupHandle.Dispose();
			}
		}

		private PResource CreateResource(string resourceName, IEnumerable<string> queryFields, SafeClusterGroupHandle groupHandle, PGroup cacheGroup, bool? isCsvResource = false, string resourceTypeString = null)
		{
			ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Creating Resource {0}", resourceName);
			ResourceLoadSelection loadSelection = ResourceLoadSelection.None;
			PCluster privateCluster = clusApiAdapter.clusterAdapter.Cluster;
			if (queryFields != null)
			{
				using IEnumerator<string> enumerator = queryFields.GetEnumerator();
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current.ToLowerInvariant())
					{
					case "state":
					case "ownernode":
					case "ownergroup":
					case "characteristics":
					case "resourceclass":
					case "replicationdisktype":
					case "flags":
						loadSelection |= ResourceLoadSelection.Basic;
						break;
					case "possibleowners":
						loadSelection |= ResourceLoadSelection.PossibleOwners;
						break;
					case "dependencies":
						loadSelection |= ResourceLoadSelection.Dependencies;
						break;
					case "dependenciesrelation":
						loadSelection |= ResourceLoadSelection.DependenciesRelation;
						break;
					case "dependents":
						loadSelection |= ResourceLoadSelection.Dependents;
						break;
					case "requireddependencies":
						loadSelection |= ResourceLoadSelection.RequiredDependencies;
						break;
					case "commonproperties":
						loadSelection |= ResourceLoadSelection.CommonProperties;
						break;
					case "privateproperties":
						loadSelection |= ResourceLoadSelection.PrivateProperties;
						break;
					}
				}
			}
			if (clusApiAdapter.MappingNameIdResource.TryGetValue(resourceName, out var value2))
			{
				using ClusterLock clusterLock = privateCluster.CacheManager.Get(value2, ClusterIdentityType.Resource, LockAccess.Reader);
				if (clusterLock != null)
				{
					PResource pResource = (PResource)clusterLock.Owner;
					if (pResource != null && ((uint)pResource.LoadedSelection & (uint)loadSelection) == (uint)loadSelection && (isCsvResource ^ (pResource.ResourceType.ResourceKind == ResourceKind.ClusterFileSystem)) == false)
					{
						ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Resource {0} found in cache", resourceName);
						return pResource;
					}
				}
			}
			PResource privateResource = null;
			ExecuteOnResource(Guid.Empty, resourceName, delegate(SafeClusterResourceHandle resourceHandle)
			{
				clusApiAdapter.MappingNameIdResource.TryGetValue(resourceName, out var resourceId);
				if (resourceId == Guid.Empty)
				{
					resourceId = GetResourceId(resourceHandle, resourceName);
					lock (loadingResourceLock)
					{
						clusApiAdapter.MappingIdNameResource.AddOrUpdate(resourceId, resourceName, (Guid key, string value) => resourceName);
						clusApiAdapter.MappingNameIdResource.AddOrUpdate(resourceName, resourceId, (string key, Guid value) => resourceId);
					}
				}
				if (resourceTypeString == null)
				{
					ExecuteOnCommonProperties(resourceHandle, resourceName, delegate(IntPtr commonPropList, int commonPropListSize)
					{
						int num = NativeMethods.ResUtilFindSzProperty(commonPropList, commonPropListSize, "Type", ref resourceTypeString);
						if (!NativeMethods.ErrorCode.None.IsEqual(num))
						{
							throw new ClusterPropertyNotFoundException(resourceName, "Type", typeof(Resource), num);
						}
					});
				}
				PResourceType pResourceType = privateCluster.GetResourceType(resourceTypeString);
				if (isCsvResource == true && pResourceType.ResourceKind == ResourceKind.PhysicalDisk)
				{
					ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Convert PDR resource {0} to CSV resource", resourceName);
					pResourceType = new PResourceType(privateCluster, ResourceKind.ClusterFileSystem, pResourceType);
				}
				privateResource = PResource.Constructor(privateCluster, resourceId, resourceName, pResourceType);
				if (!isCsvResource.HasValue && pResourceType.ResourceKind == ResourceKind.PhysicalDisk)
				{
					SetStateAndGroup(privateResource, resourceHandle, SafeClusterGroupHandle.InvalidHandle);
					if (privateResource.OwnerGroup.GroupType == GroupType.ClusterSharedVolume)
					{
						ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Convert PDR resource {0} to CSV resource", resourceName);
						pResourceType = new PResourceType(privateCluster, ResourceKind.ClusterFileSystem, pResourceType);
						privateResource = PResource.Constructor(privateCluster, resourceId, resourceName, pResourceType);
					}
				}
				if (loadSelection != 0)
				{
					if ((loadSelection & ResourceLoadSelection.Basic) == ResourceLoadSelection.Basic)
					{
						if (cacheGroup != null)
						{
							privateResource.OwnerGroup = cacheGroup;
						}
						SetStateAndGroup(privateResource, resourceHandle, groupHandle);
						ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_FLAGS, resourceName, delegate(IntPtr buffer, int bufferSize)
						{
							privateResource.Flags = (ResourceFlags)Marshal.ReadInt32(buffer);
						});
						GetResourceClass(privateResource, resourceHandle);
						GetReplicationDiskType(privateResource);
						ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_CHARACTERISTICS, resourceName, delegate(IntPtr buffer, int bufferSize)
						{
							privateResource.Characteristics = (Characteristics)Marshal.ReadInt32(buffer);
						}, delegate
						{
							privateResource.Characteristics = Characteristics.None;
						});
					}
					if ((loadSelection & ResourceLoadSelection.CommonProperties) == ResourceLoadSelection.CommonProperties)
					{
						ExecuteOnCommonProperties(resourceHandle, resourceName, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(privateResource.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
						});
						ExecuteOnReadOnlyCommonProperties(resourceHandle, resourceName, delegate(IntPtr propertyList, int propertyListSize)
						{
							AdapterBase.ParseProperties(privateResource.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
						});
						privateResource.LoadedSelection |= 2;
					}
					if ((loadSelection & ResourceLoadSelection.PrivateProperties) == ResourceLoadSelection.PrivateProperties)
					{
						try
						{
							ExecuteOnPrivateProperties(resourceHandle, resourceName, delegate(IntPtr propertyList, int propertyListSize)
							{
								AdapterBase.ParseProperties(privateResource.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
							});
							ExecuteOnReadOnlyPrivateProperties(resourceHandle, resourceName, delegate(IntPtr propertyList, int propertyListSize)
							{
								AdapterBase.ParseProperties(privateResource.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: true);
							});
							privateResource.LoadedSelection |= 4;
						}
						catch (ClusterPropertiesNotAvailableException exception)
						{
							ClusterLog.LogException(exception, "Fail to load private properties for resource {0}".FormatCurrentCulture(resourceName));
						}
						catch (ClusterGetPropertiesFailedException exception2)
						{
							ClusterLog.LogException(exception2, "Fail to load private properties for resource {0}".FormatCurrentCulture(resourceName));
						}
					}
					if ((loadSelection & ResourceLoadSelection.PossibleOwners) == ResourceLoadSelection.PossibleOwners)
					{
						privateResource.PossibleOwners = GetPossibleOwners(resourceHandle);
						privateResource.LoadedSelection |= 128;
					}
					if ((loadSelection & ResourceLoadSelection.Dependencies) == ResourceLoadSelection.Dependencies)
					{
						privateResource.Dependencies = GetDependencies(resourceHandle);
						privateResource.LoadedSelection |= 8;
					}
					if ((loadSelection & ResourceLoadSelection.DependenciesRelation) == ResourceLoadSelection.DependenciesRelation)
					{
						privateResource.DependencyRelationship = GetDependencyRelationship(resourceHandle, privateResource.Id);
						privateResource.LoadedSelection |= 32;
					}
					if ((loadSelection & ResourceLoadSelection.Dependents) == ResourceLoadSelection.Dependents)
					{
						privateResource.Dependents = GetDependents(resourceHandle);
						privateResource.LoadedSelection |= 16;
					}
					if ((loadSelection & ResourceLoadSelection.RequiredDependencies) == ResourceLoadSelection.RequiredDependencies)
					{
						privateResource.RequiredDependencies = GetRequiredDependencies(resourceHandle, privateResource.Name);
						privateResource.LoadedSelection |= 64;
					}
				}
			});
			return privateResource;
		}

		private List<Guid> GetPossibleOwners(Guid id)
		{
			List<Guid> nodeIds = new List<Guid>();
			ExecuteOnResource(id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				nodeIds = GetPossibleOwners(resourceHandle);
			});
			return nodeIds;
		}

		private List<Guid> GetPossibleOwners(SafeClusterResourceHandle resourceHandle)
		{
			List<Guid> list = new List<Guid>();
			SafeClusterResourceEnumHandle safeClusterResourceEnumHandle = NativeMethods.ClusterResourceOpenEnum(resourceHandle, NativeMethods.ResourceEnumType.Nodes);
			if (safeClusterResourceEnumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateNodeException>(Marshal.GetLastWin32Error());
			}
			try
			{
				int num = NativeMethods.ClusterResourceGetEnumCount(safeClusterResourceEnumHandle);
				for (int i = 0; i < num; i++)
				{
					NativeMethods.ResourceEnumType enumType = NativeMethods.ResourceEnumType.Nodes;
					StringBuilder stringBuilder = new StringBuilder(200);
					int resourceNameSize = 200;
					int scCode = NativeMethods.ClusterResourceEnum(safeClusterResourceEnumHandle, i, ref enumType, stringBuilder, ref resourceNameSize);
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (NativeMethods.ErrorCode.MoreData.IsEqual(scCode))
					{
						stringBuilder = new StringBuilder(resourceNameSize);
						scCode = NativeMethods.ClusterResourceEnum(safeClusterResourceEnumHandle, i, ref enumType, stringBuilder, ref resourceNameSize);
					}
					if (!NativeMethods.ErrorCode.None.IsEqual(scCode))
					{
						throw ExceptionHelper.Build<ClusterIterateResourcesException>(lastWin32Error);
					}
					Guid nodeIdFromName = clusApiAdapter.nodes.GetNodeIdFromName(stringBuilder.ToString());
					list.Add(nodeIdFromName);
				}
				return list;
			}
			finally
			{
				safeClusterResourceEnumHandle.Dispose();
			}
		}

		private List<NetworkInfo> GetClusterNetworkInfos(SafeClusterHandle clusterHandle)
		{
			List<NetworkInfo> list = new List<NetworkInfo>();
			foreach (Tuple<Guid, string> clusterObject in GetClusterObjects(clusterHandle, NativeMethods.ClusterEnumType.Network))
			{
				list.AddRange(GetNetworkInfos(clusterHandle, clusterObject));
			}
			return list;
		}

		private IEnumerable<Tuple<Guid, string>> GetClusterObjects(SafeClusterHandle clusterHandle, NativeMethods.ClusterEnumType clusterEnumType)
		{
			SafeClusterEnumHandle clusterEnumHandle = NativeMethods.ClusterOpenEnumEx(clusterHandle, clusterEnumType, IntPtr.Zero);
			if (clusterEnumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateObjectsException>(Marshal.GetLastWin32Error());
			}
			IntPtr allocatedMemory = IntPtr.Zero;
			try
			{
				int records = NativeMethods.ClusterGetEnumCountEx(clusterEnumHandle);
				for (int index = 0; index < records; index++)
				{
					int enumItemSize = 200;
					allocatedMemory = NativeMethods.Alloc(enumItemSize);
					SetStructVersion1(allocatedMemory);
					int scCode = NativeMethods.ClusterEnumEx(clusterEnumHandle, index, allocatedMemory, ref enumItemSize);
					if (NativeMethods.ErrorCode.MoreData.IsEqual(scCode))
					{
						allocatedMemory = NativeMethods.ReAlloc(allocatedMemory, enumItemSize);
						SetStructVersion1(allocatedMemory);
						scCode = NativeMethods.ClusterEnumEx(clusterEnumHandle, index, allocatedMemory, ref enumItemSize);
					}
					if (!NativeMethods.ErrorCode.None.IsEqual(scCode))
					{
						throw ExceptionHelper.Build<ClusterIterateObjectsException>(Marshal.GetLastWin32Error());
					}
					NativeMethods.CLUSTER_ENUM_ITEM cLUSTER_ENUM_ITEM = (NativeMethods.CLUSTER_ENUM_ITEM)Marshal.PtrToStructure(allocatedMemory, typeof(NativeMethods.CLUSTER_ENUM_ITEM));
					string lpszName = cLUSTER_ENUM_ITEM.lpszName;
					Guid item = new Guid(cLUSTER_ENUM_ITEM.lpszId);
					allocatedMemory = NativeMethods.Free(allocatedMemory);
					yield return new Tuple<Guid, string>(item, lpszName);
				}
			}
			finally
			{
				clusterEnumHandle.Dispose();
				NativeMethods.Free(allocatedMemory);
			}
		}

		private IEnumerable<NetworkInfo> GetNetworkInfos(SafeClusterHandle clusterHandle, Tuple<Guid, string> networkObject)
		{
			ClusterAccessRights grantedAccess;
			SafeClusterNetworkHandle safeClusterNetworkHandle = NativeMethods.OpenClusterNetworkEx(clusterHandle, networkObject.Item2, clusApiAdapter.clusterAdapter.Cluster.ClusterAccessRights, out grantedAccess);
			if (safeClusterNetworkHandle.IsInvalid)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw ExceptionHelper.ClusterObjectLoadFailedException(networkObject.Item2, Guid.Empty, lastWin32Error);
			}
			try
			{
				List<NetworkInfo> netInfos = new List<NetworkInfo>();
				ClusterPropertyCollection propertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, Guid.Empty, ClusterIdentityType.Network);
				NetworkExecuteOnControlCode(safeClusterNetworkHandle, NativeMethods.CLUSCTL_NETWORK_GET_RO_COMMON_PROPERTIES, delegate(IntPtr commonPropList, int commonPropListSize)
				{
					AdapterBase.ParseProperties(propertyCollection, commonPropList, commonPropListSize, ClusterPropertyKind.Common, readOnly: true);
					Action<string, string> obj = delegate(string ipAddress, string ipAddressPrefix)
					{
						propertyCollection.Get(ipAddress, delegate(ClusterPropertyMultipleStrings ipAddresses)
						{
							propertyCollection.Get(ipAddressPrefix, delegate(ClusterPropertyMultipleStrings ipAddressesPrefixes)
							{
								for (int i = 0; i < ipAddresses.TypedValue.Count; i++)
								{
									if (ipAddresses.TypedValue[i].Length > 0)
									{
										netInfos.Add(new NetworkInfo(networkObject.Item1, networkObject.Item2, ipAddresses.TypedValue[i], ipAddressesPrefixes.TypedValue[i]));
									}
								}
							});
						});
					};
					obj("IPv6Addresses", "IPv6PrefixLengths");
					obj("IPv4Addresses", "IPv4PrefixLengths");
				});
				return netInfos;
			}
			finally
			{
				safeClusterNetworkHandle.Dispose();
			}
		}

		private void NetworkExecuteOnControlCode(SafeClusterNetworkHandle networkHandle, int controlCode, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null)
		{
			IntPtr intPtr = NativeMethods.Alloc(4096);
			int outBufferSize = 4096;
			int bytesReturned = 0;
			try
			{
				int num = NativeMethods.ClusterNetworkControl(networkHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, IntPtr.Zero, 0, intPtr, outBufferSize, ref bytesReturned);
				if (NativeMethods.ErrorCode.MoreData.IsEqual(num))
				{
					intPtr = NativeMethods.ReAlloc(intPtr, bytesReturned);
					outBufferSize = bytesReturned;
					num = NativeMethods.ClusterNetworkControl(networkHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, IntPtr.Zero, 0, intPtr, outBufferSize, ref bytesReturned);
				}
				if (NativeMethods.ErrorCode.InvalidFunction.IsEqual(num) && invalidFunctionCallback != null)
				{
					invalidFunctionCallback();
					return;
				}
				if (!NativeMethods.ErrorCode.None.IsEqual(num) && !NativeMethods.ErrorCode.IOPending.IsEqual(num))
				{
					throw new ClusterControlCodeException(controlCode, new Win32Exception(num));
				}
				controlCodeCallBack(intPtr, bytesReturned);
			}
			finally
			{
				NativeMethods.Free(intPtr);
			}
		}

		private Guid GetResourceId(SafeClusterResourceHandle resourceHandle, string resourceName)
		{
			Guid guid = Guid.Empty;
			ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_ID, resourceName, delegate(IntPtr buffer, int bufferSize)
			{
				string g = Marshal.PtrToStringUni(buffer);
				guid = new Guid(g);
			});
			return guid;
		}

		private string GetResourceName(Guid resourceId, SafeClusterResourceHandle resourceHandle)
		{
			string resourceName = null;
			ExecuteOnReadOnlyCommonProperties(resourceHandle, resourceId.ToString(), delegate(IntPtr commonPropList, int commonPropListSize)
			{
				int num = NativeMethods.ResUtilFindSzProperty(commonPropList, commonPropListSize, "Name", ref resourceName);
				if (!NativeMethods.ErrorCode.None.IsEqual(num))
				{
					throw new ClusterPropertyNotFoundException(resourceId, "Name", typeof(Resource), num);
				}
			});
			return resourceName;
		}

		private Guid GetResourceIdFromName(string resourceName)
		{
			bool renamed;
			return GetResourceIdFromName(resourceName, out renamed);
		}

		private Guid GetResourceIdFromName(string resourceName, out bool renamed)
		{
			renamed = false;
			if (!clusApiAdapter.MappingNameIdResource.TryGetValue(resourceName, out var resourceId))
			{
				lock (loadingResourceLock)
				{
					if (!clusApiAdapter.MappingNameIdResource.TryGetValue(resourceName, out resourceId))
					{
						ExecuteOnResource(Guid.Empty, resourceName, delegate(SafeClusterResourceHandle resourceHandle)
						{
							resourceId = GetResourceId(resourceHandle, resourceName);
						});
						lock (loadingResourceLock)
						{
							if (clusApiAdapter.MappingIdNameResource.ContainsKey(resourceId))
							{
								clusApiAdapter.MappingIdNameResource.TryRemove(resourceId, out var value2);
								clusApiAdapter.MappingNameIdResource.TryRemove(value2, out var _);
								renamed = true;
							}
							clusApiAdapter.MappingIdNameResource.AddOrUpdate(resourceId, resourceName, (Guid key, string value) => resourceName);
							clusApiAdapter.MappingNameIdResource.AddOrUpdate(resourceName, resourceId, (string key, Guid value) => resourceId);
						}
					}
				}
			}
			return resourceId;
		}

		private string GetResourceNameFromId(Guid resourceId)
		{
			if (!clusApiAdapter.MappingIdNameResource.TryGetValue(resourceId, out var resourceName))
			{
				lock (loadingResourceLock)
				{
					if (!clusApiAdapter.MappingIdNameResource.TryGetValue(resourceId, out resourceName))
					{
						ExecuteOnResource(resourceId, null, delegate(SafeClusterResourceHandle resourceHandle)
						{
							resourceName = GetResourceName(resourceId, resourceHandle);
						});
						lock (loadingResourceLock)
						{
							if (clusApiAdapter.MappingIdNameResource.ContainsKey(resourceId))
							{
								clusApiAdapter.MappingIdNameResource.TryRemove(resourceId, out var value2);
								clusApiAdapter.MappingNameIdResource.TryRemove(value2, out var _);
							}
							clusApiAdapter.MappingIdNameResource.AddOrUpdate(resourceId, resourceName, (Guid key, string value) => resourceName);
							clusApiAdapter.MappingNameIdResource.AddOrUpdate(resourceName, resourceId, (string key, Guid value) => resourceId);
						}
					}
				}
			}
			return resourceName;
		}

		private IEnumerable<Guid> GetDependencies(SafeClusterResourceHandle resourceHandle)
		{
			return GetDependentsDependencies(resourceHandle, dependants: false);
		}

		private IEnumerable<Guid> GetDependents(SafeClusterResourceHandle resourceHandle)
		{
			return GetDependentsDependencies(resourceHandle, dependants: true);
		}

		private IEnumerable<Guid> GetDependentsDependencies(SafeClusterResourceHandle resourceHandle, bool dependants)
		{
			List<Guid> list = new List<Guid>();
			SafeClusterResourceEnumHandle safeClusterResourceEnumHandle = NativeMethods.ClusterResourceOpenEnum(resourceHandle, (!dependants) ? NativeMethods.ResourceEnumType.Dependencies : NativeMethods.ResourceEnumType.Dependants);
			if (safeClusterResourceEnumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateResourcesException>(Marshal.GetLastWin32Error());
			}
			try
			{
				int num = NativeMethods.ClusterResourceGetEnumCount(safeClusterResourceEnumHandle);
				for (int i = 0; i < num; i++)
				{
					NativeMethods.ResourceEnumType enumType = NativeMethods.ResourceEnumType.Nodes;
					StringBuilder stringBuilder = new StringBuilder(200);
					int resourceNameSize = 200;
					int scCode = NativeMethods.ClusterResourceEnum(safeClusterResourceEnumHandle, i, ref enumType, stringBuilder, ref resourceNameSize);
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (NativeMethods.ErrorCode.MoreData.IsEqual(scCode))
					{
						stringBuilder = new StringBuilder(resourceNameSize);
						scCode = NativeMethods.ClusterResourceEnum(safeClusterResourceEnumHandle, i, ref enumType, stringBuilder, ref resourceNameSize);
					}
					if (!NativeMethods.ErrorCode.None.IsEqual(scCode))
					{
						throw ExceptionHelper.Build<ClusterIterateResourcesException>(lastWin32Error);
					}
					string resourceName = stringBuilder.ToString();
					Guid resourceIdFromName = GetResourceIdFromName(resourceName);
					list.Add(resourceIdFromName);
				}
				return list;
			}
			finally
			{
				safeClusterResourceEnumHandle.Dispose();
			}
		}

		private string GetDependencyRelationship(SafeClusterResourceHandle resourceHandle, Guid resourceId)
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			int dependencyExpressionSize = 200;
			int clusterResourceDependencyExpression = NativeMethods.GetClusterResourceDependencyExpression(resourceHandle, stringBuilder, ref dependencyExpressionSize);
			if (NativeMethods.ErrorCode.MoreData.IsEqual(clusterResourceDependencyExpression))
			{
				stringBuilder = new StringBuilder(dependencyExpressionSize);
				clusterResourceDependencyExpression = NativeMethods.GetClusterResourceDependencyExpression(resourceHandle, stringBuilder, ref dependencyExpressionSize);
			}
			if (!NativeMethods.ErrorCode.None.IsEqual(clusterResourceDependencyExpression))
			{
				throw new ClusterGetDependencyRelationshipException(resourceId, new Win32Exception(clusterResourceDependencyExpression));
			}
			return stringBuilder.ToString();
		}

		private RequiredDependencies GetRequiredDependencies(SafeClusterResourceHandle resourceHandle, string resourceName)
		{
			List<ResourceClass> resourceClass = new List<ResourceClass>();
			List<string> resourceType = new List<string>();
			ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_REQUIRED_DEPENDENCIES, resourceName, delegate(IntPtr clusterValueList, int clusterValueListSize)
			{
				EnumerateValueList(clusterValueList, clusterValueListSize, delegate(ValueListIterator iterator)
				{
					switch (iterator.IteratorType)
					{
					case 131074u:
					{
						NativeMethods.CLUSPROP_REQUIRED_DEPENDENCY_ResourceClass cLUSPROP_REQUIRED_DEPENDENCY_ResourceClass = (NativeMethods.CLUSPROP_REQUIRED_DEPENDENCY_ResourceClass)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_REQUIRED_DEPENDENCY_ResourceClass));
						resourceClass.Add(cLUSPROP_REQUIRED_DEPENDENCY_ResourceClass.ResClass.rc);
						break;
					}
					case 262147u:
					{
						NativeMethods.CLUSPROP_REQUIRED_DEPENDENCY_ResourceType cLUSPROP_REQUIRED_DEPENDENCY_ResourceType = (NativeMethods.CLUSPROP_REQUIRED_DEPENDENCY_ResourceType)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_REQUIRED_DEPENDENCY_ResourceType));
						resourceType.Add(cLUSPROP_REQUIRED_DEPENDENCY_ResourceType.ResTypeName.sz);
						break;
					}
					}
				});
			});
			return new RequiredDependencies(resourceClass, resourceType);
		}

		private void SetStateAndGroup(PResource resource, SafeClusterResourceHandle resourceHandle, SafeClusterGroupHandle groupHandle)
		{
			if (resource.ResourceState.HasValue && resource.OwnerGroup != null)
			{
				return;
			}
			StringBuilder nodeName = new StringBuilder(200);
			StringBuilder stringBuilder = new StringBuilder(200);
			int nodeNameSize = 200;
			int groupNameSize = 200;
			ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Setting resource owner group and state");
			int clusterResourceState = NativeMethods.GetClusterResourceState(resourceHandle, nodeName, ref nodeNameSize, stringBuilder, ref groupNameSize);
			if (clusterResourceState == -1)
			{
				clusterResourceState = Marshal.GetLastWin32Error();
				if (NativeMethods.ErrorCode.ResourceNotAvailable.IsEqual(clusterResourceState) || NativeMethods.ErrorCode.ResourceNotFound.IsEqual(clusterResourceState))
				{
					throw new ClusterObjectNotFoundException(resource.Name, new Win32Exception(clusterResourceState));
				}
				throw new Win32Exception(clusterResourceState);
			}
			resource.ResourceState = (ResourceState)clusterResourceState;
			if (resource.OwnerGroup == null)
			{
				GroupType groupType = GroupType.Unknown;
				if (groupHandle.IsInvalid)
				{
					clusApiAdapter.groups.ExecuteOnGroup(Guid.Empty, stringBuilder.ToString(), delegate(SafeClusterGroupHandle groupHandleLocal)
					{
						groupType = clusApiAdapter.groups.GetGroupType(groupHandleLocal);
					});
				}
				else
				{
					groupType = clusApiAdapter.groups.GetGroupType(groupHandle);
				}
				PGroup ownerGroup = PGroup.Constructor(resource.Cluster, clusApiAdapter.groups.GetGroupIdFromName(stringBuilder.ToString()), stringBuilder.ToString(), groupType);
				resource.OwnerGroup = ownerGroup;
			}
			else
			{
				ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Owner group found in cache");
			}
		}

		private IEnumerable<PoolPhysicalDiskInfoInternal> GetPoolPhysicalDisksInfo(PStoragePoolResource storagePoolResource)
		{
			List<PoolPhysicalDiskInfoInternal> poolDriveInfos = new List<PoolPhysicalDiskInfoInternal>();
			ExecuteOnResource(storagePoolResource.Id, null, delegate(SafeClusterResourceHandle resourceHandle)
			{
				try
				{
					ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_POOL_GET_DRIVE_INFO, storagePoolResource.Name, null, delegate(IntPtr buffer, int bufferSize)
					{
						int num = Marshal.SizeOf(typeof(NativeMethods.CLUS_POOL_DRIVE_INFO));
						for (int i = 0; i < bufferSize; i += num)
						{
							NativeMethods.CLUS_POOL_DRIVE_INFO poolDriveInfo = (NativeMethods.CLUS_POOL_DRIVE_INFO)Marshal.PtrToStructure(buffer + i, typeof(NativeMethods.CLUS_POOL_DRIVE_INFO));
							poolDriveInfos.Add(new PoolPhysicalDiskInfoInternal(poolDriveInfo));
						}
					});
				}
				catch (ClusterControlCodeException ex)
				{
					ClusterLog.LogException(ex, "Cannot get pool drive info for storage pool.");
					throw new ClusterStoragePoolGetPhysicalDisksException(ExceptionResources.ClusterStorageGetPhysicalDisksException_Text, ex);
				}
			});
			return poolDriveInfos;
		}

		private void ExecuteOnCommonProperties(SafeClusterResourceHandle resourceHandle, string resourceName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_COMMON_PROPERTIES, resourceName, commonPropList);
		}

		private void ExecuteOnReadOnlyCommonProperties(SafeClusterResourceHandle resourceHandle, string resourceName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_RO_COMMON_PROPERTIES, resourceName, commonPropList);
		}

		private void ExecuteOnPrivateProperties(SafeClusterResourceHandle resourceHandle, string resourceName, Action<IntPtr, int> privatePropList)
		{
			ExecuteOnProperties(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_PRIVATE_PROPERTIES, resourceName, privatePropList);
		}

		private void ExecuteOnReadOnlyPrivateProperties(SafeClusterResourceHandle resourceHandle, string resourceName, Action<IntPtr, int> privatePropList)
		{
			ExecuteOnProperties(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_RO_PRIVATE_PROPERTIES, resourceName, privatePropList);
		}

		private void ExecuteOnProperties(SafeClusterResourceHandle resourceHandle, int controlCode, string resourceName, Action<IntPtr, int> propertyList)
		{
			try
			{
				ExecuteOnControlCode(resourceHandle, controlCode, resourceName, propertyList);
			}
			catch (ClusterControlCodeException innerException)
			{
				throw new ClusterGetPropertiesFailedException(innerException);
			}
		}

		private void ExecuteOnControlCode(SafeClusterResourceHandle resourceHandle, SafeClusterNodeHandle nodeHandle, int controlCode, string resourceName, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null)
		{
			ExecuteOnControlCode(resourceHandle, nodeHandle, controlCode, resourceName, null, controlCodeCallBack, invalidFunctionCallback);
		}

		private void ExecuteOnControlCode(SafeClusterResourceHandle resourceHandle, int controlCode, string resourceName, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null)
		{
			ExecuteOnControlCode(resourceHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, resourceName, null, controlCodeCallBack, invalidFunctionCallback);
		}

		private void ExecuteOnControlCode(SafeClusterResourceHandle resourceHandle, int controlCode, string resourceName, NativeMethods.UnmanagedBuffer inBuffer, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null)
		{
			ExecuteOnControlCode(resourceHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, resourceName, inBuffer, controlCodeCallBack, invalidFunctionCallback);
		}

		private void ExecuteOnControlCode(SafeClusterResourceHandle resourceHandle, SafeClusterNodeHandle nodeHandle, int controlCode, string resourceName, NativeMethods.UnmanagedBuffer inBuffer, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null)
		{
			IntPtr intPtr = NativeMethods.Alloc(4096);
			int outBufferSize = 4096;
			int bytesReturned = 0;
			try
			{
				int num = NativeMethods.ClusterResourceControl(resourceHandle, nodeHandle, controlCode, inBuffer?.IntPtr ?? IntPtr.Zero, inBuffer?.Size ?? 0, intPtr, outBufferSize, ref bytesReturned);
				if (NativeMethods.ErrorCode.MoreData.IsEqual(num))
				{
					intPtr = NativeMethods.ReAlloc(intPtr, bytesReturned);
					outBufferSize = bytesReturned;
					num = NativeMethods.ClusterResourceControl(resourceHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, inBuffer?.IntPtr ?? IntPtr.Zero, inBuffer?.Size ?? 0, intPtr, outBufferSize, ref bytesReturned);
				}
				if (NativeMethods.ErrorCode.InvalidFunction.IsEqual(num) && invalidFunctionCallback != null)
				{
					invalidFunctionCallback();
					return;
				}
				if (NativeMethods.ErrorCode.DeletePending.IsEqual(num))
				{
					throw new ClusterObjectDeletingException();
				}
				if (NativeMethods.ErrorCode.ResourceNotAvailable.IsEqual(num) || NativeMethods.ErrorCode.ResourceNotFound.IsEqual(num))
				{
					throw new ClusterObjectNotFoundException(resourceName, new Win32Exception(num));
				}
				if (NativeMethods.ErrorCode.ResourceFailed.IsEqual(num))
				{
					throw new ClusterResourceFailedException();
				}
				if (NativeMethods.ErrorCode.PropertiesNotAvailable.IsEqual(num))
				{
					throw new ClusterPropertiesNotAvailableException();
				}
				if (NativeMethods.ErrorCode.GroupMoving.IsEqual(num))
				{
					throw new ClusterGroupMovingException();
				}
				if (!NativeMethods.ErrorCode.None.IsEqual(num) && !NativeMethods.ErrorCode.IOPending.IsEqual(num))
				{
					throw new ClusterControlCodeException(controlCode, new Win32Exception(num));
				}
				controlCodeCallBack.SafeCall(intPtr, bytesReturned);
			}
			finally
			{
				NativeMethods.Free(intPtr);
			}
		}

		private Guid GetId(SafeClusterResourceHandle resourceHandle)
		{
			Guid guid = Guid.Empty;
			ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_ID, guid.ToString(), delegate(IntPtr buffer, int bufferSize)
			{
				string g = Marshal.PtrToStringUni(buffer);
				guid = new Guid(g);
			});
			return guid;
		}

		private unsafe void SetStructVersion1(IntPtr clusterEnumItemV2Group)
		{
			int* ptr = (int*)(void*)clusterEnumItemV2Group;
			*ptr = 1;
		}

		private void ParseCsvInfoValueList(IntPtr valueList, int valueListSize, object parseParam)
		{
			EnumerateValueList(valueList, valueListSize, delegate(ValueListIterator iterator)
			{
				uint iteratorType = iterator.IteratorType;
				if (iteratorType == 65537)
				{
					if (parseParam is ClusterDisk clusterDisk)
					{
						NativeMethods.CLUSPROP_BINARY_CSV_INFO csvVolumeInfo = (NativeMethods.CLUSPROP_BINARY_CSV_INFO)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_BINARY_CSV_INFO));
						ClusterDiskPartition clusterDiskPartition = clusterDisk.Partitions.FirstOrDefault((ClusterDiskPartition partition) => partition.PartitionNumber == csvVolumeInfo.csvInfo.PartitionNumber);
						if (clusterDiskPartition != null)
						{
							clusterDiskPartition.IsMaintenanceModeOn = csvVolumeInfo.csvInfo.FaultState == NativeMethods.CLUSTER_CSV_VOLUME_FAULT_STATE.VolumeStateInMaintenance;
							clusterDiskPartition.CsvFaultState = (ClusterSharedVolumeFaultState)csvVolumeInfo.csvInfo.FaultState;
						}
					}
					if (parseParam is ClusterSharedVolumeStateInfo clusterSharedVolumeStateInfo)
					{
						if (iterator.BufferSize == Marshal.SizeOf(typeof(NativeMethods.CLUSPROP_BINARY_CSV_STATE_INFO_EX)))
						{
							NativeMethods.CLUSPROP_BINARY_CSV_STATE_INFO_EX cLUSPROP_BINARY_CSV_STATE_INFO_EX = (NativeMethods.CLUSPROP_BINARY_CSV_STATE_INFO_EX)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_BINARY_CSV_STATE_INFO_EX));
							clusterSharedVolumeStateInfo.NodeName = cLUSPROP_BINARY_CSV_STATE_INFO_EX.csvStateInfo.szNodeName;
							clusterSharedVolumeStateInfo.VolumeName = cLUSPROP_BINARY_CSV_STATE_INFO_EX.csvStateInfo.szVolumeName;
							clusterSharedVolumeStateInfo.StateInfo = (ClusterSharedVolumeState)cLUSPROP_BINARY_CSV_STATE_INFO_EX.csvStateInfo.VolumeState;
							clusterSharedVolumeStateInfo.VolumeFriendlyName = cLUSPROP_BINARY_CSV_STATE_INFO_EX.csvStateInfo.szVolumeFriendlyName;
							clusterSharedVolumeStateInfo.RedirectedIoReason = (ClusterSharedRedirectedIoReason)cLUSPROP_BINARY_CSV_STATE_INFO_EX.csvStateInfo.RedirectedIoReason;
							clusterSharedVolumeStateInfo.VolumeRedirectedIoReason = (ClusterSharedVolumeRedirectedIoReason)cLUSPROP_BINARY_CSV_STATE_INFO_EX.csvStateInfo.VolumeRedirectedIoReason;
						}
						else
						{
							NativeMethods.CLUSPROP_BINARY_CSV_STATE_INFO cLUSPROP_BINARY_CSV_STATE_INFO = (NativeMethods.CLUSPROP_BINARY_CSV_STATE_INFO)Marshal.PtrToStructure(iterator.Buffer, typeof(NativeMethods.CLUSPROP_BINARY_CSV_STATE_INFO));
							clusterSharedVolumeStateInfo.NodeName = cLUSPROP_BINARY_CSV_STATE_INFO.csvStateInfo.szNodeName;
							clusterSharedVolumeStateInfo.VolumeName = cLUSPROP_BINARY_CSV_STATE_INFO.csvStateInfo.szVolumeName;
							clusterSharedVolumeStateInfo.StateInfo = (ClusterSharedVolumeState)cLUSPROP_BINARY_CSV_STATE_INFO.csvStateInfo.VolumeState;
							clusterSharedVolumeStateInfo.VolumeFriendlyName = string.Empty;
							clusterSharedVolumeStateInfo.RedirectedIoReason = (ClusterSharedRedirectedIoReason)0uL;
						}
					}
				}
			});
		}

		public void Renew(PCommonIPAddressResource ipAddress)
		{
			throw new NotSupportedException("Renew is not implemented for clusApi Adapter");
		}

		public void Release(PCommonIPAddressResource ipAddress)
		{
			throw new NotSupportedException("Release is not implemented for clusApi Adapter");
		}

		public void Collect()
		{
		}
	}

	private class ResourceTypeAdapter : AdapterBase, IConnectionAdapterResourceType, INotificationHandler
	{
		private readonly ClusApiAdapter clusApiAdapter;

		public ResourceTypeAdapter(ClusApiAdapter clusApiAdapter)
			: base(clusApiAdapter)
		{
			this.clusApiAdapter = clusApiAdapter;
		}

		public IEnumerable<TResult> Select<TResult>(QueryInfo queryInfo)
		{
			IList<string> queryFields = (from s in queryInfo.ProjectionFields.Select((ClusterObjectMetaDataMember s) => s.Name).Concat(queryInfo.WhereFields.Select((ClusterObjectMetaDataMember s) => s.Name)).Concat(from s in queryInfo.OrderBy.ConvertAll((OrderByItem item) => item.DataMember)
					select s.Name)
					.Distinct()
				where s.ToLowerInvariant() != "name" && s.ToLowerInvariant() != "id" && s.ToLowerInvariant() != "resourcetype"
				select s).ToList();
			return from resourceType in GetAll(queryFields, nullElementOnError: false).TakeWhile((PResourceType resourceType) => !queryInfo.IsCancel)
				select (TResult)(object)resourceType;
		}

		public IEnumerable<PResourceType> GetAll(IList<string> queryFields, bool nullElementOnError)
		{
			SafeClusterHandle clusterHandle = clusApiAdapter.clusterAdapter.Handle;
			if (clusterHandle == null)
			{
				yield break;
			}
			SafeClusterEnumHandle enumHandle = NativeMethods.ClusterOpenEnumEx(clusterHandle, NativeMethods.ClusterEnumType.ResourceType, IntPtr.Zero);
			if (enumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateNodeException>(Marshal.GetLastWin32Error());
			}
			IntPtr allocatedMemory = IntPtr.Zero;
			try
			{
				int records = NativeMethods.ClusterGetEnumCountEx(enumHandle);
				for (int index = 0; index < records; index++)
				{
					PResourceType privateResourceType;
					try
					{
						int enumItemSize = 200;
						allocatedMemory = NativeMethods.Alloc(enumItemSize);
						SetStructVersion1(allocatedMemory);
						int num = NativeMethods.ClusterEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
						if (num == NativeMethods.ErrorCode.MoreData.ToInt())
						{
							allocatedMemory = NativeMethods.ReAlloc(allocatedMemory, enumItemSize);
							SetStructVersion1(allocatedMemory);
							num = NativeMethods.ClusterEnumEx(enumHandle, index, allocatedMemory, ref enumItemSize);
						}
						if (num != NativeMethods.ErrorCode.None.ToInt())
						{
							throw ExceptionHelper.Build<ClusterIterateGroupsException>(Marshal.GetLastWin32Error());
						}
						NativeMethods.CLUSTER_ENUM_ITEM cLUSTER_ENUM_ITEM = (NativeMethods.CLUSTER_ENUM_ITEM)Marshal.PtrToStructure(allocatedMemory, typeof(NativeMethods.CLUSTER_ENUM_ITEM));
						allocatedMemory = NativeMethods.Free(allocatedMemory);
						privateResourceType = new PResourceType(clusApiAdapter.clusterAdapter.Cluster, cLUSTER_ENUM_ITEM.lpszName);
						if (queryFields.Any())
						{
							ResourceTypeLoadSelection resourceTypeLoadSelection = ResourceTypeLoadSelection.None;
							foreach (string queryField in queryFields)
							{
								switch (queryField.ToLowerInvariant())
								{
								case "isstorage":
								case "resourceclass":
								case "characteristics":
									resourceTypeLoadSelection |= ResourceTypeLoadSelection.Basic;
									continue;
								case "possibleowners":
									resourceTypeLoadSelection |= ResourceTypeLoadSelection.PossibleOwners;
									continue;
								}
								if (queryField.Equals("commonproperties"))
								{
									resourceTypeLoadSelection |= ResourceTypeLoadSelection.CommonProperties;
								}
								if (queryField.Equals("privateproperties"))
								{
									resourceTypeLoadSelection |= ResourceTypeLoadSelection.PrivateProperties;
								}
							}
							if ((resourceTypeLoadSelection & ResourceTypeLoadSelection.Basic) == ResourceTypeLoadSelection.Basic)
							{
								if (privateResourceType.Name.ToLowerInvariant() == "msmq" || privateResourceType.Name.ToLowerInvariant() == "msmqtriggers")
								{
									privateResourceType.Class = ResourceClass.Unknown;
									privateResourceType.IsStorage = privateResourceType.Class == ResourceClass.Storage;
								}
								else
								{
									ExecuteOnControlCode(privateResourceType.Name, NativeMethods.CLUSCTL_RESOURCE_TYPE_GET_CLASS_INFO, delegate(IntPtr buffer, int bufferSize)
									{
										long num2 = Marshal.ReadInt64(buffer);
										privateResourceType.Class = (ResourceClass)num2;
										privateResourceType.Subclass = (ResourceSubclass)(num2 >> 32);
										privateResourceType.IsStorage = privateResourceType.Class == ResourceClass.Storage;
									});
									ExecuteOnControlCode(privateResourceType.Name, NativeMethods.CLUSCTL_RESOURCE_TYPE_GET_CHARACTERISTICS, delegate(IntPtr buffer, int bufferSize)
									{
										privateResourceType.Characteristics = (Characteristics)Marshal.ReadInt32(buffer);
									});
								}
								privateResourceType.LoadedSelection |= 1;
							}
							if ((resourceTypeLoadSelection & ResourceTypeLoadSelection.PossibleOwners) == ResourceTypeLoadSelection.PossibleOwners)
							{
								privateResourceType.PossibleOwners = GetPossibleOwnersList(clusterHandle, privateResourceType.Name);
								privateResourceType.LoadedSelection |= 128;
							}
						}
					}
					catch (ClusterException exception)
					{
						if (!nullElementOnError)
						{
							throw;
						}
						ClusterLog.LogException(exception, "There was an error when getting resourcetype information from the cluster, however is not critical and the process can continue");
						privateResourceType = null;
					}
					yield return privateResourceType;
				}
			}
			finally
			{
				enumHandle.Dispose();
				NativeMethods.Free(allocatedMemory);
			}
		}

		public IEnumerable<string> GetPossibleOwners(string name)
		{
			throw new NotSupportedException("GetPossibleOwners is not supported by ClusApiAdapter");
		}

		public IEnumerable<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> GetReplicationResources()
		{
			if (clusApiAdapter.clusterAdapter.Handle == null)
			{
				return new List<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK>();
			}
			ClusterLog.LogInfo("Getting all replication resources");
			List<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> replicatedDisks = new List<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK>();
			foreach (PResourceType item in GetAll(new string[1] { "resourceclass" }, nullElementOnError: true))
			{
				if (item == null || item.Subclass != ResourceSubclass.Replication)
				{
					continue;
				}
				try
				{
					ExecuteOnControlCode(item.Name, NativeMethods.CLUSCTL_RESOURCE_TYPE_REPLICATION_GET_REPLICATED_DISKS, delegate(IntPtr buffer, int bufferSize)
					{
						ushort num = (ushort)Marshal.ReadInt16(buffer);
						IntPtr intPtr = IntPtr.Add(buffer, 4);
						int offset = Marshal.SizeOf(typeof(NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK));
						for (int i = 0; i < num; i++)
						{
							NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK sR_RESOURCE_TYPE_REPLICATED_DISK = (NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK)Marshal.PtrToStructure(intPtr, typeof(NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK));
							ClusterLog.LogInfo("Found replication resource with Id '{0}'", sR_RESOURCE_TYPE_REPLICATED_DISK);
							replicatedDisks.Add(sR_RESOURCE_TYPE_REPLICATED_DISK);
							intPtr = IntPtr.Add(intPtr, offset);
						}
					});
				}
				catch (ClusterObjectNotFoundException exception)
				{
					ClusterLog.LogException(LogLevel.Warning, exception, "Resource Type '{0}' not found when getting replication resources".FormatInvariantCulture(item.Name));
				}
				catch (Exception exception2)
				{
					ClusterLog.LogException(LogLevel.Error, exception2, "Resource Type '{0}' not found when getting replication resources".FormatInvariantCulture(item.Name));
				}
			}
			return replicatedDisks;
		}

		public unsafe IEnumerable<ReplicationGroupInfo> GetReplicationGroups()
		{
			if (clusApiAdapter.clusterAdapter.Handle == null)
			{
				return new List<ReplicationGroupInfo>();
			}
			ClusterLog.LogInfo("Getting all groups which contains at least one replicated resource");
			List<ReplicationGroupInfo> replicatedGroups = new List<ReplicationGroupInfo>();
			foreach (PResourceType item in GetAll(new string[1] { "resourceclass" }, nullElementOnError: true))
			{
				if (item == null || item.Subclass != ResourceSubclass.Replication)
				{
					continue;
				}
				try
				{
					ExecuteOnControlCode(item.Name, NativeMethods.CLUSCTL_RESOURCE_TYPE_REPLICATION_GET_RESOURCE_GROUP, delegate(IntPtr valueList, int valueListSize)
					{
						EnumerateValueList(valueList, valueListSize, delegate(ValueListIterator iterator)
						{
							uint iteratorType = iterator.IteratorType;
							if (iteratorType == 65537)
							{
								IntPtr propertyList = IntPtr.Add(iterator.Buffer, sizeof(NativeMethods.CLUSPROP_VALUE));
								Guid id = clusApiAdapter.clusterAdapter.Cluster.Id;
								ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(id, id, ClusterIdentityType.Cluster);
								AdapterBase.ParseProperties(clusterPropertyCollection, propertyList, iterator.BufferSize, ClusterPropertyKind.Common, readOnly: true);
								Guid replicationGroupId = new Guid((string)clusterPropertyCollection["ReplicationGroupId"].Value);
								Guid clusterGroupId = new Guid((string)clusterPropertyCollection["ClusterGroupId"].Value);
								ReplicationGroupRole role = (ReplicationGroupRole)(uint)clusterPropertyCollection["ReplicationClusterGroupType"].Value;
								ReplicationGroupInfo replicationGroupInfo = new ReplicationGroupInfo(clusterGroupId, replicationGroupId, role);
								ClusterLog.LogInfo("Found cluster group with at least one replicated resource '{0}'", replicationGroupInfo);
								replicatedGroups.Add(replicationGroupInfo);
							}
						});
					});
				}
				catch (ClusterObjectNotFoundException exception)
				{
					ClusterLog.LogException(LogLevel.Warning, exception, "Resource Type '{0}' not found when getting replication groups".FormatInvariantCulture(item.Name));
				}
				catch (Exception exception2)
				{
					ClusterLog.LogException(LogLevel.Error, exception2, "Resource Type '{0}' not found when getting replication groups".FormatInvariantCulture(item.Name));
				}
			}
			return replicatedGroups;
		}

		public void Load(PResourceType resourceType, ResourceTypeLoadSelection loadSelection)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				return;
			}
			try
			{
				if ((loadSelection & ResourceTypeLoadSelection.Basic) == ResourceTypeLoadSelection.Basic && (resourceType.LoadedSelection & 1) != 1)
				{
					resourceType.LoadedSelection |= 1;
					if (!resourceType.IsStorage.HasValue)
					{
						if (resourceType.Name.ToLowerInvariant() == "msmq" || resourceType.Name.ToLowerInvariant() == "msmqtriggers")
						{
							resourceType.Class = ResourceClass.Unknown;
							resourceType.IsStorage = resourceType.Class == ResourceClass.Storage;
						}
						else
						{
							ExecuteOnControlCode(resourceType.Name, NativeMethods.CLUSCTL_RESOURCE_TYPE_GET_CLASS_INFO, delegate(IntPtr buffer, int bufferSize)
							{
								ulong num = (ulong)Marshal.ReadInt64(buffer);
								resourceType.Class = (ResourceClass)(num & 0xFFFF);
								resourceType.Subclass = (ResourceSubclass)(num >> 32);
								resourceType.IsStorage = resourceType.Class == ResourceClass.Storage;
							});
							ExecuteOnControlCode(resourceType.Name, NativeMethods.CLUSCTL_RESOURCE_TYPE_GET_CHARACTERISTICS, delegate(IntPtr buffer, int bufferSize)
							{
								resourceType.Characteristics = (Characteristics)Marshal.ReadInt32(buffer);
							});
						}
					}
				}
				if ((loadSelection & ResourceTypeLoadSelection.CommonProperties) == ResourceTypeLoadSelection.CommonProperties && (resourceType.LoadedSelection & 2) != 2)
				{
					resourceType.LoadedSelection |= 2;
					ExecuteOnCommonProperties(resourceType.Name, delegate(IntPtr propertyList, int propertyListSize)
					{
						AdapterBase.ParseProperties(resourceType.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: false);
					});
					ExecuteOnReadOnlyCommonProperties(resourceType.Name, delegate(IntPtr propertyList, int propertyListSize)
					{
						AdapterBase.ParseProperties(resourceType.Properties, propertyList, propertyListSize, ClusterPropertyKind.Common, readOnly: true);
					});
					resourceType.Properties.CommonPropertiesLoaded = true;
				}
				if ((loadSelection & ResourceTypeLoadSelection.PrivateProperties) == ResourceTypeLoadSelection.PrivateProperties && (resourceType.LoadedSelection & 4) != 4)
				{
					resourceType.LoadedSelection |= 4;
					ExecuteOnPrivateProperties(resourceType.Name, delegate(IntPtr propertyList, int propertyListSize)
					{
						AdapterBase.ParseProperties(resourceType.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
					});
					ExecuteOnReadOnlyPrivateProperties(resourceType.Name, delegate(IntPtr propertyList, int propertyListSize)
					{
						AdapterBase.ParseProperties(resourceType.Properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: true);
					});
					resourceType.Properties.PrivatePropertiesLoaded = true;
				}
				if ((loadSelection & ResourceTypeLoadSelection.PossibleOwners) == ResourceTypeLoadSelection.PossibleOwners)
				{
					resourceType.LoadedSelection |= 8;
					resourceType.PossibleOwners = GetPossibleOwnersList(handle, resourceType.Name);
				}
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(resourceType.Name, resourceType.Id, innerException);
			}
		}

		public PResourceType Open(string resourceTypeName)
		{
			if (clusApiAdapter.clusterAdapter.Handle == null)
			{
				return null;
			}
			PResourceType resourceType = new PResourceType(clusApiAdapter.clusterAdapter.Cluster, resourceTypeName);
			ExecuteOnControlCode(resourceTypeName, NativeMethods.CLUSCTL_RESOURCE_TYPE_GET_CLASS_INFO, delegate(IntPtr buffer, int bufferSize)
			{
				ulong num = (ulong)Marshal.ReadInt64(buffer);
				resourceType.Class = (ResourceClass)(num & 0xFFFF);
				resourceType.Subclass = (ResourceSubclass)(num >> 32);
				resourceType.IsStorage = resourceType.Class == ResourceClass.Storage;
			});
			return resourceType;
		}

		private List<Guid> GetPossibleOwnersList(string resourceTypeName)
		{
			Utilities.UnreferencedParameter(resourceTypeName);
			return new List<Guid>();
		}

		private List<Guid> GetPossibleOwnersList(SafeClusterHandle clusterHandle, string resourceTypeName)
		{
			List<Guid> list = new List<Guid>();
			SafeClusterResourceTypeEnumHandle safeClusterResourceTypeEnumHandle = NativeMethods.ClusterResourceTypeOpenEnum(clusterHandle, resourceTypeName, NativeMethods.ResourceTypeEnumType.Node);
			if (safeClusterResourceTypeEnumHandle.IsInvalid)
			{
				throw ExceptionHelper.Build<ClusterEnumerateNodeException>(Marshal.GetLastWin32Error());
			}
			try
			{
				int num = NativeMethods.ClusterResourceTypeGetEnumCount(safeClusterResourceTypeEnumHandle);
				for (int i = 0; i < num; i++)
				{
					NativeMethods.ResourceTypeEnumType enumType = NativeMethods.ResourceTypeEnumType.Node;
					StringBuilder stringBuilder = new StringBuilder(200);
					int resourceTypeNameSize = 200;
					int num2 = NativeMethods.ClusterResourceTypeEnum(safeClusterResourceTypeEnumHandle, i, ref enumType, stringBuilder, ref resourceTypeNameSize);
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (num2 == NativeMethods.ErrorCode.MoreData.ToInt())
					{
						stringBuilder = new StringBuilder(resourceTypeNameSize);
						num2 = NativeMethods.ClusterResourceTypeEnum(safeClusterResourceTypeEnumHandle, i, ref enumType, stringBuilder, ref resourceTypeNameSize);
					}
					if (num2 != NativeMethods.ErrorCode.None.ToInt())
					{
						throw ExceptionHelper.Build<ClusterIterateResourceTypesException>(lastWin32Error);
					}
					Guid nodeIdFromName = clusApiAdapter.nodes.GetNodeIdFromName(stringBuilder.ToString());
					list.Add(nodeIdFromName);
				}
				return list;
			}
			finally
			{
				safeClusterResourceTypeEnumHandle.Dispose();
			}
		}

		private unsafe void SetStructVersion1(IntPtr clusterEnumItemV2Group)
		{
			int* ptr = (int*)(void*)clusterEnumItemV2Group;
			*ptr = 1;
		}

		private void ExecuteOnPrivateProperties(string resourceTypeName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(resourceTypeName, NativeMethods.CLUSCTL_RESOURCE_TYPE_GET_PRIVATE_PROPERTIES, commonPropList);
		}

		private void ExecuteOnReadOnlyPrivateProperties(string resourceTypeName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(resourceTypeName, NativeMethods.CLUSCTL_RESOURCE_TYPE_GET_RO_PRIVATE_PROPERTIES, commonPropList);
		}

		private void ExecuteOnReadOnlyCommonProperties(string resourceTypeName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(resourceTypeName, NativeMethods.CLUSCTL_RESOURCE_TYPE_GET_RO_COMMON_PROPERTIES, commonPropList);
		}

		private void ExecuteOnCommonProperties(string resourceTypeName, Action<IntPtr, int> commonPropList)
		{
			ExecuteOnProperties(resourceTypeName, NativeMethods.CLUSCTL_RESOURCE_TYPE_GET_COMMON_PROPERTIES, commonPropList);
		}

		private void ExecuteOnProperties(string resourceTypeName, int controlCode, Action<IntPtr, int> propertyList)
		{
			try
			{
				ExecuteOnControlCode(resourceTypeName, SafeClusterNodeHandle.InvalidHandle, controlCode, null, propertyList);
			}
			catch (ClusterControlCodeException innerException)
			{
				throw new ClusterGetPropertiesFailedException(innerException);
			}
		}

		private void ExecuteOnControlCode(string resourceTypeName, int controlCode, Action<IntPtr, int> controlCodeCallBack)
		{
			ExecuteOnControlCode(resourceTypeName, SafeClusterNodeHandle.InvalidHandle, controlCode, null, controlCodeCallBack);
		}

		internal void ExecuteOnControlCode(string resourceTypeName, SafeClusterNodeHandle nodeHandle, int controlCode, NativeMethods.UnmanagedBuffer inBuffer, Action<IntPtr, int> controlCodeCallBack)
		{
			SafeClusterHandle handle = clusApiAdapter.clusterAdapter.Handle;
			if (handle == null)
			{
				return;
			}
			IntPtr intPtr = NativeMethods.Alloc(4096);
			int outBufferSize = 4096;
			int bytesReturned = 0;
			try
			{
				int num = NativeMethods.ClusterResourceTypeControl(handle, resourceTypeName, nodeHandle, controlCode, inBuffer?.IntPtr ?? IntPtr.Zero, inBuffer?.Size ?? 0, intPtr, outBufferSize, ref bytesReturned);
				if (NativeMethods.ErrorCode.MoreData.IsEqual(num))
				{
					intPtr = NativeMethods.ReAlloc(intPtr, bytesReturned);
					outBufferSize = bytesReturned;
					num = NativeMethods.ClusterResourceTypeControl(handle, resourceTypeName, SafeClusterNodeHandle.InvalidHandle, controlCode, inBuffer?.IntPtr ?? IntPtr.Zero, inBuffer?.Size ?? 0, intPtr, outBufferSize, ref bytesReturned);
				}
				if (NativeMethods.ErrorCode.ResourceTypeNotFound.IsEqual(num) || NativeMethods.ErrorCode.ResourceTypeNotSupported.IsEqual(num))
				{
					throw new ClusterObjectNotFoundException(resourceTypeName, new Win32Exception(num));
				}
				if (!NativeMethods.ErrorCode.None.IsEqual(num))
				{
					throw new ClusterControlCodeException(controlCode, new Win32Exception(num));
				}
				controlCodeCallBack(intPtr, bytesReturned);
			}
			finally
			{
				NativeMethods.Free(intPtr);
			}
		}

		public bool NotificationArrived(NativeMethods.NOTIFY_FILTER_AND_TYPE filterType, string objectName, string objectId, string parentId, string objectType, IntPtr buffer, int bufferSize)
		{
			if (filterType.ObjectType != NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_RESOURCE_TYPE && filterType.FilterFlags != 64)
			{
				return false;
			}
			if (filterType.FilterFlags == 32)
			{
				PCluster cluster = clusApiAdapter.clusterAdapter.Cluster;
				Guid id = PResourceType.IdFromName(objectName);
				using ClusterLock clusterLock = cluster.CacheManager.Get(id, ClusterIdentityType.ResourceType, LockAccess.Reader);
				PResourceType pResourceType = null;
				if (clusterLock != null)
				{
					pResourceType = (PResourceType)clusterLock.Owner;
				}
				if (pResourceType != null && (pResourceType.Subclass & ResourceSubclass.Replication) == ResourceSubclass.Replication)
				{
					NativeMethods.WVR_EVENT_TYPE wVR_EVENT_TYPE = (NativeMethods.WVR_EVENT_TYPE)Marshal.ReadInt32(buffer + 4);
					ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Received Replication Notification for '{0}' with payloadType '{1}'", objectName, wVR_EVENT_TYPE);
					switch (wVR_EVENT_TYPE)
					{
					case NativeMethods.WVR_EVENT_TYPE.WvrEventTypeReplicationStatusChanged:
					case NativeMethods.WVR_EVENT_TYPE.WvrEventTypeRecoveryStatusChanged:
					{
						NativeMethods.WVR_RESOURCE_TYPE_REPLICA_STATE_NOTIFICATION payload3 = (NativeMethods.WVR_RESOURCE_TYPE_REPLICA_STATE_NOTIFICATION)Marshal.PtrToStructure(buffer, typeof(NativeMethods.WVR_RESOURCE_TYPE_REPLICA_STATE_NOTIFICATION));
						ResourceTypeNotification notification3 = new ResourceTypeNotification(new ClusterResourceTypeReplicationStateEventArgs(pResourceType.Cluster, id, wVR_EVENT_TYPE, payload3, null));
						clusApiAdapter.EnqueueNotification(notification3);
						break;
					}
					case NativeMethods.WVR_EVENT_TYPE.WvrEventTypeGroupDeleted:
					{
						NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_GROUP_NOTIFICATION payload2 = (NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_GROUP_NOTIFICATION)Marshal.PtrToStructure(buffer, typeof(NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_GROUP_NOTIFICATION));
						ResourceTypeNotification notification2 = new ResourceTypeNotification(new ClusterResourceTypeReplicationGroupDeletedEventArgs(pResourceType.Cluster, id, wVR_EVENT_TYPE, payload2, null));
						clusApiAdapter.EnqueueNotification(notification2);
						break;
					}
					case NativeMethods.WVR_EVENT_TYPE.WvrEventTypeGroupCreated:
					case NativeMethods.WVR_EVENT_TYPE.WvrEventTypeGroupModified:
					case NativeMethods.WVR_EVENT_TYPE.WvrEventTypeAddReplica:
					case NativeMethods.WVR_EVENT_TYPE.WvrEventTypeRemoveReplica:
					case NativeMethods.WVR_EVENT_TYPE.WvrEventTypeGroupRoleChanged:
					case NativeMethods.WVR_EVENT_TYPE.WvrEventTypeGroupPartnershipChanged:
					{
						NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_GROUP_MODIFIED_NOTIFICATION payload4 = (NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_GROUP_MODIFIED_NOTIFICATION)Marshal.PtrToStructure(buffer, typeof(NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_GROUP_MODIFIED_NOTIFICATION));
						List<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> list2 = new List<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK>();
						int offset3 = Marshal.SizeOf(typeof(NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_GROUP_MODIFIED_NOTIFICATION)) - Marshal.SizeOf(typeof(NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISKS_RESULT)) - 4;
						IntPtr intPtr2 = IntPtr.Add(buffer, offset3);
						ushort num2 = (ushort)Marshal.ReadInt16(intPtr2);
						intPtr2 = IntPtr.Add(intPtr2, 4);
						int offset4 = Marshal.SizeOf(typeof(NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK));
						for (int j = 0; j < num2; j++)
						{
							NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK item2 = (NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK)Marshal.PtrToStructure(intPtr2, typeof(NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK));
							list2.Add(item2);
							intPtr2 = IntPtr.Add(intPtr2, offset4);
						}
						ResourceTypeNotification notification4 = new ResourceTypeNotification(new ClusterResourceTypeReplicationGroupModifiedEventArgs(pResourceType.Cluster, id, wVR_EVENT_TYPE, payload4, list2, null));
						clusApiAdapter.EnqueueNotification(notification4);
						break;
					}
					case NativeMethods.WVR_EVENT_TYPE.WvrEventTypePartnershipCreated:
					case NativeMethods.WVR_EVENT_TYPE.WvrEventTypePartnershipDestroyed:
					case NativeMethods.WVR_EVENT_TYPE.WvrEventTypeRoleSwitched:
					{
						NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_PARTNERSHIP_NOTIFICATION payload = (NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_PARTNERSHIP_NOTIFICATION)Marshal.PtrToStructure(buffer, typeof(NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_PARTNERSHIP_NOTIFICATION));
						List<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> list = new List<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK>();
						int offset = Marshal.SizeOf(typeof(NativeMethods.WVR_RESOURCE_TYPE_REPLICATION_PARTNERSHIP_NOTIFICATION)) - Marshal.SizeOf(typeof(NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISKS_RESULT));
						IntPtr intPtr = IntPtr.Add(buffer, offset);
						ushort num = (ushort)Marshal.ReadInt16(intPtr);
						intPtr = IntPtr.Add(intPtr, 4);
						int offset2 = Marshal.SizeOf(typeof(NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK));
						for (int i = 0; i < num; i++)
						{
							NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK item = (NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK)Marshal.PtrToStructure(intPtr, typeof(NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK));
							list.Add(item);
							intPtr = IntPtr.Add(intPtr, offset2);
						}
						ResourceTypeNotification notification = new ResourceTypeNotification(new ClusterResourceTypeReplicationPartnershipEventArgs(pResourceType.Cluster, id, wVR_EVENT_TYPE, payload, list, null));
						clusApiAdapter.EnqueueNotification(notification);
						break;
					}
					}
					return true;
				}
			}
			if (filterType.FilterFlags == 64)
			{
				if (objectName.Length == 0)
				{
					ClusterLog.LogError("Empty group name, the group name in group add notification is empty");
				}
				ClusterAddedEventArgs payload5 = new ClusterAddedEventArgs(PResourceType.IdFromName(objectName), objectName, 0, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster
				};
				clusApiAdapter.EnqueueNotification(new ResourceTypeNotification(payload5));
				return true;
			}
			NativeMethods.CLUSTER_CHANGE_RESOURCE_TYPE_V2 filterFlags = (NativeMethods.CLUSTER_CHANGE_RESOURCE_TYPE_V2)filterType.FilterFlags;
			NativeMethods.CLUSTER_CHANGE_RESOURCE_TYPE_V2 num3 = filterFlags - 1;
			if (num3 <= (NativeMethods.CLUSTER_CHANGE_RESOURCE_TYPE_V2.CLUSTER_CHANGE_RESOURCE_TYPE_DELETED_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_TYPE_V2.CLUSTER_CHANGE_RESOURCE_TYPE_COMMON_PROPERTY_V2))
			{
				switch (num3)
				{
				case NativeMethods.CLUSTER_CHANGE_RESOURCE_TYPE_V2.CLUSTER_CHANGE_RESOURCE_TYPE_DELETED_V2 | NativeMethods.CLUSTER_CHANGE_RESOURCE_TYPE_V2.CLUSTER_CHANGE_RESOURCE_TYPE_COMMON_PROPERTY_V2:
					goto IL_03b9;
				case NativeMethods.CLUSTER_CHANGE_RESOURCE_TYPE_V2.CLUSTER_CHANGE_RESOURCE_TYPE_DELETED_V2:
					goto IL_0473;
				case (NativeMethods.CLUSTER_CHANGE_RESOURCE_TYPE_V2)0uL:
					goto IL_0525;
				case NativeMethods.CLUSTER_CHANGE_RESOURCE_TYPE_V2.CLUSTER_CHANGE_RESOURCE_TYPE_COMMON_PROPERTY_V2:
					goto IL_055e;
				}
			}
			if (filterFlags != NativeMethods.CLUSTER_CHANGE_RESOURCE_TYPE_V2.CLUSTER_CHANGE_RESOURCE_TYPE_POSSIBLE_OWNERS_V2)
			{
				goto IL_055e;
			}
			List<Guid> possibleOwnersList = GetPossibleOwnersList(objectName);
			Guid id2 = PResourceType.IdFromName(objectName);
			clusApiAdapter.EnqueueNotification(new ResourceTypeNotification(new ClusterResourceTypePossibleOwnersChangedEventArgs(id2, possibleOwnersList, null)));
			goto IL_0560;
			IL_0560:
			return true;
			IL_03b9:
			Guid guid = PResourceType.IdFromName(objectName);
			ClusterPropertyCollection properties = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, guid, ClusterIdentityType.ResourceType);
			try
			{
				ExecuteOnPrivateProperties(objectName, delegate(IntPtr propertyList, int propertyListSize)
				{
					AdapterBase.ParseProperties(properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
				});
				ExecuteOnReadOnlyPrivateProperties(objectName, delegate(IntPtr propertyList, int propertyListSize)
				{
					AdapterBase.ParseProperties(properties, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: true);
				});
			}
			catch (ClusterObjectNotFoundException)
			{
				return true;
			}
			catch (ClusterObjectDeletingException)
			{
				return true;
			}
			clusApiAdapter.EnqueueNotification(new ResourceTypeNotification(new ClusterPropertiesEventArgs(guid, objectName, null, null)
			{
				Cluster = clusApiAdapter.clusterAdapter.Cluster,
				Properties = properties
			}));
			goto IL_0560;
			IL_0473:
			if (bufferSize != 0)
			{
				Guid guid2 = PResourceType.IdFromName(objectName);
				ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(clusApiAdapter.clusterAdapter.Cluster.Id, guid2, ClusterIdentityType.ResourceType)
				{
					Partial = true
				};
				AdapterBase.ParseProperties(clusterPropertyCollection, buffer, bufferSize, ClusterPropertyKind.Common, readOnly: false);
				clusApiAdapter.EnqueueNotification(new ResourceTypeNotification(new ClusterPropertiesEventArgs(guid2, objectName, null, null)
				{
					Cluster = clusApiAdapter.clusterAdapter.Cluster,
					Properties = clusterPropertyCollection
				}));
			}
			goto IL_0560;
			IL_055e:
			return false;
			IL_0525:
			Guid id3 = PResourceType.IdFromName(objectName);
			clusApiAdapter.EnqueueNotification(new ResourceTypeNotification(new ClusterRemovedEventArgs(id3, objectName, null)
			{
				Cluster = clusApiAdapter.clusterAdapter.Cluster
			}));
			goto IL_0560;
		}

		public PResourceType Create(string name, string displayName, string pathDll)
		{
			throw new NotSupportedException("Create is not supported by ClusApiAdapter");
		}

		public void Delete(string resourceType)
		{
			throw new NotSupportedException("Delete is not supported by ClusApiAdapter");
		}

		public void Collect()
		{
		}
	}

	private class StorageAdapter : AdapterBase, IConnectionAdapterStorage
	{
		private readonly ClusApiAdapter clusApiAdapter;

		public StorageAdapter(ClusApiAdapter clusApiAdapter)
			: base(clusApiAdapter)
		{
			this.clusApiAdapter = clusApiAdapter;
			Init();
		}

		private void Init()
		{
		}

		public IEnumerable<TResult> Select<TResult>(QueryInfo queryInfo)
		{
			Utilities.UnreferencedParameter(queryInfo);
			yield break;
		}

		public void Collect()
		{
		}

		public void SetReplicationLogSize(PStorageResource storageResourcePrivate, long logSize)
		{
			clusApiAdapter.ExecuteOnCim(delegate(CimAdapter cimAdapter)
			{
				cimAdapter.Storage.SetReplicationLogSize(storageResourcePrivate, logSize);
			});
		}

		public void RemoveReplication(PStorageResource storageResource, bool fullCleanUp)
		{
			clusApiAdapter.ExecuteOnCim(delegate(CimAdapter cimAdapter)
			{
				cimAdapter.Storage.RemoveReplication(storageResource, fullCleanUp);
			});
		}

		public IEnumerable<Guid> GetReplicationGroupPartnership(PNode ownerNode, Guid replicationGroupId, ReplicationGroupRole role)
		{
			return clusApiAdapter.ExecuteOnCim((CimAdapter cimAdapter) => cimAdapter.Storage.GetReplicationGroupPartnership(ownerNode, replicationGroupId, role));
		}

		public IEnumerable<string> GetReplicationGroupPartnership(PNode ownerNode, string replicationGroupName, ReplicationGroupRole role)
		{
			return clusApiAdapter.ExecuteOnCim((CimAdapter cimAdapter) => cimAdapter.Storage.GetReplicationGroupPartnership(ownerNode, replicationGroupName, role));
		}

		public void LoadReplicationInfo(PStorageResource resource)
		{
			if (clusApiAdapter.clusterAdapter.Handle == null)
			{
				return;
			}
			ClusterLog.LogInfo("Getting replication information for resource '{0}'", resource.Name);
			PCluster cluster = resource.Cluster;
			try
			{
				List<Guid> list = new List<Guid>();
				if (cluster.CacheManager.ReplicatedResources.TryGetValue(resource.Id, out var replicatedDisk))
				{
					list.AddRange(from replicaResource in cluster.CacheManager.ReplicatedResources.Values
						where replicaResource.ReplicationGroupId == replicatedDisk.ReplicationGroupId
						select replicaResource into replicated
						select replicated.ClusterResourceId);
					foreach (Guid item in list)
					{
						ClusterLog.LogInfo("Found replicated related resource '{0}' for Resource '{1}'", item, resource.Name);
					}
					PGroup ownerGroup = resource.OwnerGroup;
					if (ownerGroup == null)
					{
						resource.LoadObject(1);
						ownerGroup = resource.OwnerGroup;
					}
					PNode ownerNode = ownerGroup.OwnerNode;
					if (ownerNode == null)
					{
						ownerGroup.LoadObject(1);
						ownerNode = ownerGroup.OwnerNode;
					}
					ClusterLog.LogInfo("Getting replication group for cluster group '{0}'", ownerGroup.Name);
					if (cluster.CacheManager.ReplicatedGroups.TryGetValue(ownerGroup.Id, out var value))
					{
						ClusterLog.LogInfo("Replication group for cluster group '{0}' found '{1}' '{2}'", ownerGroup.Name, value.ReplicationGroupId, value.Role);
						ClusterLog.LogInfo("Getting all partnerships for replication group '{0}'", value.ReplicationGroupId);
						foreach (string item2 in clusApiAdapter.Storage.GetReplicationGroupPartnership(ownerNode, replicatedDisk.ReplicationGroupName, value.Role))
						{
							ClusterLog.LogInfo("Replication partnership '{0}' found", item2);
							string partnershipGroupName = item2;
							list.AddRange(from replicaResource in cluster.CacheManager.ReplicatedResources.Values
								where replicaResource.ReplicationGroupName == partnershipGroupName
								select replicaResource into replicated
								select replicated.ClusterResourceId);
							foreach (Guid item3 in list)
							{
								ClusterLog.LogInfo("Found replicated related resource '{0}' in partnership '{1}'", item3, partnershipGroupName);
							}
						}
					}
					else
					{
						ClusterLog.LogWarning("Replication group for cluster group '{0}' not found", ownerGroup.Name);
					}
					clusApiAdapter.ExecuteOnCim(delegate(CimAdapter cimAdapter)
					{
						cimAdapter.Storage.LoadReplicationInfo(resource);
					});
					if (resource.ReplicationInfo != null)
					{
						resource.ReplicationInfo.ReplicationPrivateStorageResources = list;
						LoadReplicationLogInfo(resource);
					}
					else
					{
						ReplicationInfo replicationInfo = new ReplicationInfo(cluster, EnumResources.Unknown, EnumResources.Unknown, ReplicationType.Unknown, ReplicationStatus.Unknown, list, 0L, 0L, 0, 0L, isConsistencyEnabled: false);
						resource.ReplicationInfo = replicationInfo;
					}
				}
				else
				{
					ClusterLog.LogWarning("Resource {0} is not a replicated resource", resource.Name);
				}
			}
			catch (ClusterObjectLoadFailedException exception)
			{
				ClusterLog.LogException(exception, "There was an error loading the replication information for disk {0}".FormatCurrentCulture(resource.Name));
				resource.ReplicationInfo = null;
				resource.ReplicationStatus = new ReplicationStatusInfo[1]
				{
					new ReplicationStatusInfo(Guid.Empty, ReplicationStatus.Unknown)
				};
				throw;
			}
			catch (ClusterObjectNotFoundException exception2)
			{
				ClusterLog.LogException(exception2, "There was an error loading the replication information for disk {0}".FormatCurrentCulture(resource.Name));
				resource.ReplicationInfo = null;
				resource.ReplicationStatus = new ReplicationStatusInfo[1]
				{
					new ReplicationStatusInfo(Guid.Empty, ReplicationStatus.Unknown)
				};
				throw;
			}
		}

		private void LoadReplicationLogInfo(PStorageResource storageResource)
		{
			if (storageResource == null || storageResource.ReplicationDiskType == ReplicationDiskType.None || storageResource.ReplicationDiskType == ReplicationDiskType.Other || storageResource.ReplicationDiskType == ReplicationDiskType.NotInParthership || storageResource.ReplicationDiskType == ReplicationDiskType.LogNotInParthership || storageResource.ResourceState != ResourceState.Online || storageResource.ReplicationInfo == null || storageResource.ReplicationInfo.ReplicationGroupId == Guid.Empty)
			{
				return;
			}
			clusApiAdapter.nodes.ExecuteOnNode(Guid.Empty, storageResource.OwnerGroup.OwnerNode.Name, delegate(SafeClusterNodeHandle nodeHandle)
			{
				using NativeMethods.UnmanagedBuffer unmanagedBuffer = NativeMethods.UnmanagedBuffer.Create(storageResource.ReplicationInfo.ReplicationGroupId);
				if (unmanagedBuffer.IsMemoryValid)
				{
					clusApiAdapter.resourceTypes.ExecuteOnControlCode(PResourceType.ResourceKindToString(ResourceKind.StorageReplica), nodeHandle, NativeMethods.CLUSCTL_RESOURCE_TYPE_REPLICATION_GET_LOG_INFO, unmanagedBuffer, delegate(IntPtr propertyList, int propertyListSize)
					{
						ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(storageResource.Cluster.Id, storageResource.Id, ClusterIdentityType.ResourceType);
						AdapterBase.ParseProperties(clusterPropertyCollection, propertyList, propertyListSize, ClusterPropertyKind.Private, readOnly: false);
						storageResource.ReplicationInfo.ContainerSize = (long)(ulong)clusterPropertyCollection["UnitOfLogSizeChangeInBytes"].Value;
						storageResource.ReplicationInfo.MultiplicationFactor = (int)(uint)clusterPropertyCollection["LogSizeMultiple"].Value;
						storageResource.ReplicationInfo.MinLogSize = (long)(ulong)clusterPropertyCollection["MinimumLogSizeInBytes"].Value;
					});
				}
			});
		}

		public uint? GetDiskNumber(PStorageResource storageResourcePrivate, string uniqueId, string nodeName)
		{
			return clusApiAdapter.ExecuteOnCim((CimAdapter wmiAdapter) => wmiAdapter.Storage.GetDiskNumber(storageResourcePrivate, uniqueId, nodeName));
		}

		public string GetUniqueId(uint diskNumber, string nodeName)
		{
			return clusApiAdapter.ExecuteOnCim((CimAdapter wmiAdapter) => wmiAdapter.Storage.GetUniqueId(diskNumber, nodeName));
		}

		public IEnumerable<T1> Enumerate<T1>(ObservableKeyCollection<T1> collection, ObservableCollectionFilter<T1> filter) where T1 : IKeyQueryable<T1>
		{
			return clusApiAdapter.ExecuteOnCim((CimAdapter wmiAdapter) => wmiAdapter.Storage.Enumerate(collection, filter));
		}

		public IEnumerable<T1> Association<T, T1>(ObservableKeyCollection<T1> collection, T association) where T1 : IKeyQueryable<T1>
		{
			return clusApiAdapter.ExecuteOnCim((CimAdapter wmiAdapter) => wmiAdapter.Storage.Association(collection, association));
		}

		public void Subscribe<T1>(ObservableKeyCollection<T1> collection) where T1 : IKeyQueryable<T1>
		{
			clusApiAdapter.ExecuteOnCim(delegate(CimAdapter wmiAdapter)
			{
				wmiAdapter.Storage.Subscribe(collection);
			});
		}

		public void Unsubscribe<T1>(ObservableKeyCollection<T1> collection) where T1 : IKeyQueryable<T1>
		{
			clusApiAdapter.ExecuteOnCim(delegate(CimAdapter wmiAdapter)
			{
				wmiAdapter.Storage.Subscribe(collection);
			});
		}

		public T1 GetInstance<T1>(string key, string serverName = null) where T1 : IKeyQueryable
		{
			return clusApiAdapter.ExecuteOnCim((CimAdapter wmiAdapter) => wmiAdapter.Storage.GetInstance<T1>(key, serverName));
		}
	}

	private const int EmptyPropertyList = 4;

	protected const int DefaultSmallBufferSize = 200;

	protected const int DefaultBufferSize = 4096;

	private readonly ClusterAdapter clusterAdapter;

	private readonly GroupAdapter groups;

	private readonly NodeAdapter nodes;

	private readonly NetworkAdapter networks;

	private readonly NetworkInterfaceAdapter networkInterfaces;

	private readonly ResourceAdapter resources;

	private readonly StorageAdapter storage;

	private readonly ResourceTypeAdapter resourceTypes;

	private bool isDisposed;

	private readonly List<INotificationHandler> notificationTargets = new List<INotificationHandler>();

	private bool exitNotifications;

	private readonly Queue<Notification> notificationQueue = new Queue<Notification>();

	private readonly object notificationQueueLock = new object();

	private readonly AutoResetEvent notificationReady = new AutoResetEvent(initialState: false);

	private readonly ManualResetEvent isNotificationLoopStopped = new ManualResetEvent(initialState: true);

	private SafeClusterNotifyPortHandle notificationPort;

	private readonly ManualResetEvent notificationsPaused = new ManualResetEvent(initialState: true);

	private CimAdapter cimAdapter;

	private WmiAdapter wmiAdapter;

	public IConnectionAdapterCluster Cluster => clusterAdapter;

	public ClusterAdapterType Adapter => ClusterAdapterType.ClusterApi;

	public IConnectionAdapterGroup Group => groups;

	public IConnectionAdapterResource Resource => resources;

	public IConnectionAdapterNode Node => nodes;

	public IConnectionAdapterNetwork Network => networks;

	public IConnectionAdapterStorage Storage => storage;

	public IConnectionAdapterNetworkInterface NetworkInterface => networkInterfaces;

	public IConnectionAdapterResourceType ResourceType => resourceTypes;

	internal SafeClusterHandle ClusterHandle => clusterAdapter.Handle;

	public ClusApiAdapter(PCluster cluster)
	{
		Exceptions.ThrowIfNull(cluster, "cluster");
		clusterAdapter = new ClusterAdapter(this, cluster);
		nodes = new NodeAdapter(this);
		networks = new NetworkAdapter(this);
		networkInterfaces = new NetworkInterfaceAdapter(this);
		groups = new GroupAdapter(this);
		resources = new ResourceAdapter(this);
		storage = new StorageAdapter(this);
		resourceTypes = new ResourceTypeAdapter(this);
		QuorumAdapter item = new QuorumAdapter(this, cluster);
		notificationTargets.Add(clusterAdapter);
		notificationTargets.Add(nodes);
		notificationTargets.Add(networks);
		notificationTargets.Add(networkInterfaces);
		notificationTargets.Add(resources);
		notificationTargets.Add(groups);
		notificationTargets.Add(resourceTypes);
		notificationTargets.Add(item);
	}

	~ClusApiAdapter()
	{
		Dispose(disposing: false);
		isNotificationLoopStopped.Dispose();
	}

	public void Close()
	{
		Cluster.Close();
		Dispose(disposing: true);
	}

	public IEnumerable<PClusterObject> Select<TInput>(IClusterList<TInput> query) where TInput : ClusterObject
	{
		return Select<PClusterObject>(((ClusterList<TInput>)query).QueryInfo);
	}

	public IEnumerable<TResult> Select<TResult>(QueryInfo queryInfo) where TResult : PClusterObject
	{
		if (typeof(FailoverClusters.Framework.Group).IsAssignableFrom(queryInfo.Source))
		{
			return groups.Select<TResult>(queryInfo);
		}
		if (typeof(Node).IsAssignableFrom(queryInfo.Source))
		{
			return nodes.Select<TResult>(queryInfo);
		}
		if (typeof(Network).IsAssignableFrom(queryInfo.Source))
		{
			return networks.Select<TResult>(queryInfo);
		}
		if (typeof(NetworkInterface).IsAssignableFrom(queryInfo.Source))
		{
			return networkInterfaces.Select<TResult>(queryInfo);
		}
		if (typeof(StorageResource).IsAssignableFrom(queryInfo.Source))
		{
			return storage.Select<TResult>(queryInfo);
		}
		if (typeof(Resource).IsAssignableFrom(queryInfo.Source))
		{
			return resources.Select<TResult>(queryInfo);
		}
		if (typeof(FailoverClusters.Framework.ResourceType).IsAssignableFrom(queryInfo.Source))
		{
			return resourceTypes.Select<TResult>(queryInfo);
		}
		throw new NotSupportedException(ExceptionResources.TypeNotSupportedByLINQ.FormatCurrentCulture(queryInfo.Source));
	}

	public void SubscribeNotifications(Action notificationLostAction, Action<ClusterException> notificationConnectionUnrepairableAction)
	{
		SafeClusterHandle clusterHandle = clusterAdapter.Handle;
		CreateNotificationPort(clusterHandle);
		Worker.Start(delegate
		{
			Thread.CurrentThread.Name = "Fx notification producer - '{0}'".FormatCurrentCulture(clusterAdapter.Cluster.Name);
			isNotificationLoopStopped.Reset();
			NotificationV2 notificationV = new NotificationV2();
			try
			{
				while (!exitNotifications)
				{
					int nextNotification = notificationV.GetNextNotification(notificationPort, 500);
					if (!NativeMethods.ErrorCode.TimeOut.IsEqual(nextNotification))
					{
						if (NativeMethods.ErrorCode.InvalidHandle.IsEqual(nextNotification))
						{
							break;
						}
						if (NativeMethods.ErrorCode.MoreData.IsEqual(nextNotification))
						{
							notificationV.NotificationData.ResetSizes();
						}
						else if (!NativeMethods.ErrorCode.None.IsEqual(nextNotification))
						{
							Win32Exception ex = new Win32Exception(nextNotification);
							ClusterLog.LogException(ex, "Invalid Cluster Notification");
							if (Global.ExtraExceptionData || Global.IsDebug)
							{
								ClusterDialogException.ShowTaskDialogAsync(new ClusterNotificationException(ex.ErrorCode, ex));
							}
							TryReconnect(clusterHandle, notificationLostAction, notificationConnectionUnrepairableAction);
						}
						else
						{
							try
							{
								NotificationDataV2 notificationData = notificationV.NotificationData;
								if (notificationData.FilterAndType.ObjectType == NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_CLUSTER && notificationData.FilterAndType.FilterFlags == 2)
								{
									clusterAdapter.Cluster.SendEventToProxy(new ClusterWrapperEventArgs(EventType.Disconnected, new ClusterDisconnectedEventArgs(clusterAdapter.Cluster.Id, null)));
									break;
								}
								NotificationArrived(notificationData.FilterAndType, notificationData.Name, notificationData.ObjectId, notificationData.ParentId, notificationData.Type, notificationData.BufferBuilder, notificationData.BufferSize);
							}
							catch (ClusterObjectLoadFailedException)
							{
							}
							catch (ClusterObjectNotFoundException)
							{
							}
							catch (ClusterObjectDeletingException)
							{
							}
							catch (ClusterResourceFailedException)
							{
							}
							catch (ClusterException ex6)
							{
								ClusterLog.AdminEvents.WriteFailedProcessNotification(ex6.ToString());
								ClusterLog.LogException(ex6, "There was an error processing notification for object {0} ".FormatCurrentCulture(notificationV.NotificationData.Name));
								if (clusterAdapter.Cluster.DisconnectForRpcError(ex6))
								{
									break;
								}
								if (Global.ExtraExceptionData || Global.IsDebug)
								{
									ClusterDialogException.ShowTaskDialogAsync(new ClusterNotificationException("Exception processing a notification, Check the Log file for more information", ex6));
								}
							}
							catch (Win32Exception ex7)
							{
								ClusterLog.AdminEvents.WriteFailedProcessNotification(ex7.ToString());
								ClusterLog.LogException(ex7, "Invalid Cluster Notification");
								if (clusterAdapter.Cluster.DisconnectForRpcError(new ClusterDefaultException(ex7)))
								{
									break;
								}
								TryReconnect(clusterHandle, notificationLostAction, notificationConnectionUnrepairableAction);
							}
						}
					}
				}
			}
			finally
			{
				isNotificationLoopStopped.Set();
			}
		});
	}

	private void TryReconnect(SafeClusterHandle clusterHandle, Action notificationLostAction, Action<ClusterException> notificationConnectionUnrepairableAction)
	{
		int num = 0;
		Win32Exception ex = null;
		while (++num <= 6)
		{
			ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "There was a Win32Exception processing a notification, trying to reopen notificiation port");
			try
			{
				if (notificationPort != null)
				{
					notificationPort.Dispose();
					notificationPort = null;
				}
			}
			catch (Win32Exception exception)
			{
				ClusterLog.LogException(exception, "Win32Exception thrown by CloseClusterNotifyPort");
				continue;
			}
			try
			{
				CreateNotificationPort(clusterHandle);
			}
			catch (Win32Exception ex2)
			{
				ex = ex2;
				ClusterLog.LogException(ex2, "Exception thrown by CreateNotificationPort");
				Thread.Sleep(3000);
				continue;
			}
			ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Reopen notification port successfully");
			notificationLostAction();
			break;
		}
		if (num > 6)
		{
			UnsubscribeNotifications();
			ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Failed to reopen notification port");
			notificationConnectionUnrepairableAction(new ClusterNotificationException(ex.ErrorCode, ex));
		}
	}

	private void CreateNotificationPort(SafeClusterHandle safeClusterHandle)
	{
		exitNotifications = false;
		NativeMethods.NOTIFY_FILTER_AND_TYPE[] array = new NativeMethods.NOTIFY_FILTER_AND_TYPE[11]
		{
			new NativeMethods.NOTIFY_FILTER_AND_TYPE(NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_CLUSTER, 4095uL),
			new NativeMethods.NOTIFY_FILTER_AND_TYPE(NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_GROUP, 511uL),
			new NativeMethods.NOTIFY_FILTER_AND_TYPE(NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_RESOURCE, 255uL),
			new NativeMethods.NOTIFY_FILTER_AND_TYPE(NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NODE, 23uL),
			new NativeMethods.NOTIFY_FILTER_AND_TYPE(NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NETWORK, 31uL),
			new NativeMethods.NOTIFY_FILTER_AND_TYPE(NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_NETWORK_INTERFACE, 31uL),
			new NativeMethods.NOTIFY_FILTER_AND_TYPE(NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_RESOURCE_TYPE, 47uL),
			new NativeMethods.NOTIFY_FILTER_AND_TYPE(NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_QUORUM, 1uL),
			new NativeMethods.NOTIFY_FILTER_AND_TYPE(NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_SHARED_VOLUME, 7uL),
			new NativeMethods.NOTIFY_FILTER_AND_TYPE(NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_SPACEPORT, 1uL),
			new NativeMethods.NOTIFY_FILTER_AND_TYPE(NativeMethods.CLUSTER_OBJECT_TYPE.CLUSTER_OBJECT_TYPE_UPGRADE, 7uL)
		};
		notificationPort = NativeMethods.CreateClusterNotifyPortV2((IntPtr)(-1), safeClusterHandle, ref array[0], array.Length, IntPtr.Zero);
		if (notificationPort.IsInvalid)
		{
			throw new Win32Exception(Marshal.GetLastWin32Error());
		}
	}

	public void PauseNotifications()
	{
		lock (notificationQueueLock)
		{
			if (!isDisposed)
			{
				notificationsPaused.Reset();
				ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Notification Queue is being paused");
			}
		}
	}

	public void ResumeNotifications()
	{
		lock (notificationQueueLock)
		{
			if (!isDisposed)
			{
				notificationsPaused.Set();
				ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Notification Queue is being resumed");
			}
		}
	}

	public void EnqueueNotification(Notification notification)
	{
		string text = null;
		try
		{
			lock (notificationQueueLock)
			{
				if (isDisposed)
				{
					return;
				}
				if (notificationQueue.Count > 0 && notification is ResourceNotification && notification.Payload is ClusterPropertiesEventArgs && ((ClusterPropertiesEventArgs)notification.Payload).IsVirtual)
				{
					Notification notification2 = notificationQueue.Peek();
					if (notification2 is ResourceNotification && notification2.Payload is ClusterPropertiesEventArgs && ((ClusterPropertiesEventArgs)notification2.Payload).IsVirtual && notification2.Payload.Id == notification.Payload.Id)
					{
						text = ((ClusterPropertiesEventArgs)notification2.Payload).Name;
						return;
					}
				}
				notificationQueue.Enqueue(notification);
				notificationReady.Set();
			}
		}
		finally
		{
			if (text != null)
			{
				ClusterLog.LogVerbose(LogSubcategory.FxAdapterNotification, "Collapsing resource private property for {0}".FormatCurrentCulture(text));
			}
		}
	}

	public void ResetNotifications()
	{
		if (isDisposed)
		{
			return;
		}
		lock (notificationQueueLock)
		{
			notificationQueue.Clear();
			ClusterLog.LogVerbose(LogSubcategory.FxAdapter, "Notification Queue has been cleared because of a reset");
		}
	}

	public Notification DequeueNotification()
	{
		return DequeueNotification(-1);
	}

	public Notification DequeueNotification(int milliSecondsTimeout)
	{
		if (isDisposed)
		{
			return null;
		}
		try
		{
			notificationsPaused.WaitOne(-1);
			if (exitNotifications)
			{
				throw new ClusterNotificationNotStartedException();
			}
			bool flag;
			lock (notificationQueueLock)
			{
				flag = notificationQueue.Count == 0;
			}
			if (flag && !notificationReady.WaitOne(milliSecondsTimeout))
			{
				return null;
			}
			lock (notificationQueueLock)
			{
				if (isDisposed || notificationQueue.Count == 0)
				{
					return null;
				}
				notificationReady.Reset();
				return notificationQueue.Dequeue();
			}
		}
		catch (ObjectDisposedException)
		{
			return null;
		}
	}

	public void UnsubscribeNotifications()
	{
		if (!exitNotifications)
		{
			exitNotifications = true;
			isNotificationLoopStopped.WaitOne(TimeSpan.FromSeconds(30.0));
			notificationsPaused.Set();
			if (notificationPort != null)
			{
				notificationPort.Dispose();
				notificationPort = null;
			}
		}
	}

	private void NotificationArrived(NativeMethods.NOTIFY_FILTER_AND_TYPE filterType, string objectName, string objectId, string parentId, string objectType, IntPtr buffer, int bufferSize)
	{
		notificationTargets.Any((INotificationHandler handler) => handler.NotificationArrived(filterType, objectName, objectId, parentId, objectType, buffer, bufferSize));
	}

	protected void SetToDefaultInPropertyList(SafeClusterPropertyListHandle propertyList, ClusterProperty clusterProperty)
	{
		string propertyName = clusterProperty.RealName ?? clusterProperty.Name;
		int num = NativeMethods.SetPropertyToDefaultValue(propertyList, propertyName, clusterProperty.PropertyType);
		if (num != NativeMethods.ErrorCode.None.ToInt())
		{
			throw ExceptionHelper.Build(num);
		}
	}

	protected void AddInPropertyList(SafeClusterPropertyListHandle propertyList, ClusterProperty clusterProperty)
	{
		string propertyName = clusterProperty.RealName ?? clusterProperty.Name;
		switch (clusterProperty.PropertyType)
		{
		case ClusterPropertyType.Binary:
		{
			ClusterPropertyBinary clusterPropertyBinary = clusterProperty as ClusterPropertyBinary;
			int num4 = NativeMethods.AddBinaryProperty(propertyList, propertyName, clusterPropertyBinary.TypedValue, clusterPropertyBinary.TypedValue.Length);
			if (num4 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw ExceptionHelper.Build(num4);
			}
			break;
		}
		case ClusterPropertyType.UnsignedInt:
		{
			ClusterPropertyUInt clusterPropertyUInt = clusterProperty as ClusterPropertyUInt;
			int num6 = NativeMethods.AddDwordProperty(propertyList, propertyName, clusterPropertyUInt.TypedValue);
			if (num6 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw ExceptionHelper.Build(num6);
			}
			break;
		}
		case ClusterPropertyType.ExpandedString:
		{
			ClusterPropertyExpandedString clusterPropertyExpandedString = clusterProperty as ClusterPropertyExpandedString;
			int num8 = NativeMethods.AddExpandSzProperty(propertyList, propertyName, clusterPropertyExpandedString.TypedValue);
			if (num8 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw ExceptionHelper.Build(num8);
			}
			break;
		}
		case ClusterPropertyType.ExpandString:
		{
			ClusterPropertyExpandString clusterPropertyExpandString = clusterProperty as ClusterPropertyExpandString;
			int num13 = NativeMethods.AddExpandSzProperty(propertyList, propertyName, clusterPropertyExpandString.TypedValue);
			if (num13 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw ExceptionHelper.Build(num13);
			}
			break;
		}
		case ClusterPropertyType.String:
		{
			ClusterPropertyString clusterPropertyString = clusterProperty as ClusterPropertyString;
			int num10 = NativeMethods.AddStringProperty(propertyList, propertyName, clusterPropertyString.TypedValue);
			if (num10 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw ExceptionHelper.Build(num10);
			}
			break;
		}
		case ClusterPropertyType.StringCollection:
		{
			ClusterPropertyMultipleStrings obj = clusterProperty as ClusterPropertyMultipleStrings;
			byte[] array = new byte[obj.TypedValue.Sum((string str) => (str.Length + 1) * 2) + 2];
			int num11 = 0;
			foreach (string item in obj.TypedValue)
			{
				num11 += Encoding.Unicode.GetBytes(item, 0, item.Length, array, num11);
				array[num11++] = 0;
				array[num11++] = 0;
			}
			int num12 = NativeMethods.AddMultiSzProperty(propertyList, propertyName, array);
			if (num12 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw ExceptionHelper.Build(num12);
			}
			break;
		}
		case ClusterPropertyType.UnsignedInt64:
		{
			ClusterPropertyULong clusterPropertyULong = clusterProperty as ClusterPropertyULong;
			int num9 = NativeMethods.AddULong64Property(propertyList, propertyName, clusterPropertyULong.TypedValue);
			if (num9 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw ExceptionHelper.Build(num9);
			}
			break;
		}
		case ClusterPropertyType.Int:
		{
			ClusterPropertyInt clusterPropertyInt = clusterProperty as ClusterPropertyInt;
			int num7 = NativeMethods.AddLongProperty(propertyList, propertyName, clusterPropertyInt.TypedValue);
			if (num7 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw ExceptionHelper.Build(num7);
			}
			break;
		}
		case ClusterPropertyType.Int64:
		{
			ClusterPropertyLong clusterPropertyLong = clusterProperty as ClusterPropertyLong;
			int num5 = NativeMethods.AddLong64Property(propertyList, propertyName, clusterPropertyLong.TypedValue);
			if (num5 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw ExceptionHelper.Build(num5);
			}
			break;
		}
		case ClusterPropertyType.UnsignedShort:
		{
			ClusterPropertyUShort clusterPropertyUShort = clusterProperty as ClusterPropertyUShort;
			int num3 = NativeMethods.AddWordProperty(propertyList, propertyName, clusterPropertyUShort.TypedValue);
			if (num3 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw ExceptionHelper.Build(num3);
			}
			break;
		}
		case ClusterPropertyType.DateTime:
		{
			ClusterPropertyDateTime clusterPropertyDateTime = clusterProperty as ClusterPropertyDateTime;
			try
			{
				long num = clusterPropertyDateTime.TypedValue.ToFileTimeUtc();
				System.Runtime.InteropServices.ComTypes.FILETIME fILETIME = default(System.Runtime.InteropServices.ComTypes.FILETIME);
				fILETIME.dwLowDateTime = (int)(num & 0xFFFFFFFFu);
				fILETIME.dwHighDateTime = (int)(num >> 32);
				System.Runtime.InteropServices.ComTypes.FILETIME propertyValue = fILETIME;
				int num2 = NativeMethods.AddFiletimeProperty(propertyList, propertyName, ref propertyValue);
				if (num2 != NativeMethods.ErrorCode.None.ToInt())
				{
					throw ExceptionHelper.Build(num2);
				}
				break;
			}
			catch (ArgumentOutOfRangeException innerException)
			{
				throw new ClusterSavePropertiesException(clusterPropertyDateTime.Name, innerException);
			}
		}
		case ClusterPropertyType.SecurityDescriptor:
			break;
		}
	}

	public void SaveProperties(PClusterObject clusterObject, ClusterPropertyCollection properties)
	{
		if (properties == null)
		{
			throw new ArgumentNullException("properties");
		}
		Action<SafeHandle, int, int> savePropertiesFx = delegate(SafeHandle clusterObjectHandle, int controlCodeSetCommon, int controlCodeSetPrivate)
		{
			if (properties.Any((ClusterProperty property) => property.PropertyKind == ClusterPropertyKind.Common && (property.IsModified || property.IsDeleted)))
			{
				ExecuteOnPropertiesSet(clusterObject.Name, clusterObjectHandle, controlCodeSetCommon, delegate(SafeClusterPropertyListHandle commonPropList)
				{
					foreach (ClusterProperty property in properties)
					{
						if (property.PropertyKind == ClusterPropertyKind.Common && property.IsModified && !property.IsDeleted)
						{
							AddInPropertyList(commonPropList, property);
						}
						if (property.PropertyKind == ClusterPropertyKind.Common && property.IsDeleted)
						{
							SetToDefaultInPropertyList(commonPropList, property);
						}
					}
				});
			}
			if (properties.Any((ClusterProperty property) => property.PropertyKind == ClusterPropertyKind.Private && (property.IsModified || property.IsDeleted)))
			{
				ExecuteOnPropertiesSet(clusterObject.Name, clusterObjectHandle, controlCodeSetPrivate, delegate(SafeClusterPropertyListHandle privatePropList)
				{
					foreach (ClusterProperty property2 in properties)
					{
						if (property2.PropertyKind == ClusterPropertyKind.Private && property2.IsModified && !property2.IsDeleted)
						{
							AddInPropertyList(privatePropList, property2);
						}
						if (property2.PropertyKind == ClusterPropertyKind.Private && property2.IsDeleted)
						{
							SetToDefaultInPropertyList(privatePropList, property2);
						}
					}
				});
			}
		};
		if (clusterObject is PCluster)
		{
			savePropertiesFx(ClusterHandle, NativeMethods.CLUSCTL_CLUSTER_SET_COMMON_PROPERTIES, NativeMethods.CLUSCTL_CLUSTER_SET_PRIVATE_PROPERTIES);
			return;
		}
		if (clusterObject is PGroup)
		{
			groups.ExecuteOnGroup(clusterObject.Id, clusterObject.Name, delegate(SafeClusterGroupHandle groupHandle)
			{
				savePropertiesFx(groupHandle, NativeMethods.CLUSCTL_GROUP_SET_COMMON_PROPERTIES, NativeMethods.CLUSCTL_GROUP_SET_PRIVATE_PROPERTIES);
			});
			return;
		}
		if (clusterObject is PResource)
		{
			resources.ExecuteOnResource(clusterObject.Id, clusterObject.Name, delegate(SafeClusterResourceHandle resourceHandle)
			{
				savePropertiesFx(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_SET_COMMON_PROPERTIES, NativeMethods.CLUSCTL_RESOURCE_SET_PRIVATE_PROPERTIES);
			});
			return;
		}
		if (clusterObject is PNetwork)
		{
			networks.ExecuteOnNetwork(clusterObject.Id, clusterObject.Name, delegate(SafeClusterNetworkHandle networkHandle)
			{
				savePropertiesFx(networkHandle, NativeMethods.CLUSCTL_NETWORK_SET_COMMON_PROPERTIES, NativeMethods.CLUSCTL_NETWORK_SET_PRIVATE_PROPERTIES);
			});
			return;
		}
		throw new NotSupportedException(ExceptionResources.SavePropertyNotSupportedForTypeX.FormatCurrentCulture(clusterObject.GetType().Name));
	}

	private void ExecuteOnPropertiesSet(string objectName, SafeHandle objectHandle, int controlCode, Action<SafeClusterPropertyListHandle> commonPropList)
	{
		SafeClusterPropertyListHandle safeClusterPropertyListHandle = NativeMethods.CreatePropList(IntPtr.Zero, 0);
		try
		{
			commonPropList(safeClusterPropertyListHandle);
			int propertyListBuffer2 = NativeMethods.GetPropertyListBuffer(safeClusterPropertyListHandle, out var propertyListBuffer, out var propertyListSize);
			if (propertyListBuffer2 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw new ClusterPropertyListBufferException(new Win32Exception(propertyListBuffer2));
			}
			int bytesReturned = 0;
			if (controlCode >> NativeMethods.CLUSCTL_OBJECT_SHIFT == 1)
			{
				propertyListBuffer2 = NativeMethods.ClusterResourceControl((SafeClusterResourceHandle)objectHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, propertyListBuffer, propertyListSize, IntPtr.Zero, 0, ref bytesReturned);
			}
			else if (controlCode >> NativeMethods.CLUSCTL_OBJECT_SHIFT == 2)
			{
				propertyListBuffer2 = NativeMethods.ClusterResourceTypeControl((SafeClusterHandle)objectHandle, objectName, SafeClusterNodeHandle.InvalidHandle, controlCode, propertyListBuffer, propertyListSize, IntPtr.Zero, 0, ref bytesReturned);
			}
			else if (controlCode >> NativeMethods.CLUSCTL_OBJECT_SHIFT == 3)
			{
				propertyListBuffer2 = NativeMethods.ClusterGroupControl((SafeClusterGroupHandle)objectHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, propertyListBuffer, propertyListSize, IntPtr.Zero, 0, ref bytesReturned);
			}
			else if (controlCode >> NativeMethods.CLUSCTL_OBJECT_SHIFT == 4)
			{
				propertyListBuffer2 = NativeMethods.ClusterNodeControl((SafeClusterNodeHandle)objectHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, propertyListBuffer, propertyListSize, IntPtr.Zero, 0, ref bytesReturned);
			}
			else if (controlCode >> NativeMethods.CLUSCTL_OBJECT_SHIFT == 5)
			{
				propertyListBuffer2 = NativeMethods.ClusterNetworkControl((SafeClusterNetworkHandle)objectHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, propertyListBuffer, propertyListSize, IntPtr.Zero, 0, ref bytesReturned);
			}
			else if (controlCode >> NativeMethods.CLUSCTL_OBJECT_SHIFT == 6)
			{
				propertyListBuffer2 = NativeMethods.ClusterNetInterfaceControl((SafeClusterNetworkInterfaceHandle)objectHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, propertyListBuffer, propertyListSize, IntPtr.Zero, 0, ref bytesReturned);
			}
			else
			{
				if (controlCode >> NativeMethods.CLUSCTL_OBJECT_SHIFT != 7)
				{
					throw new NotSupportedException(ExceptionResources.SavePropertyNotSupportedForType);
				}
				propertyListBuffer2 = NativeMethods.ClusterControl((SafeClusterHandle)objectHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, propertyListBuffer, propertyListSize, IntPtr.Zero, 0, ref bytesReturned);
			}
			if (propertyListBuffer2 == NativeMethods.ErrorCode.ResourcePropertiesStored.ToInt())
			{
				throw new ClusterResourcePropertyStoredException(objectName);
			}
			if (propertyListBuffer2 == NativeMethods.ErrorCode.InvalidData.ToInt())
			{
				throw new ClusterInvalidPropertyDataException(objectName);
			}
			if (propertyListBuffer2 != NativeMethods.ErrorCode.None.ToInt())
			{
				throw new ClusterControlCodeException(controlCode, new Win32Exception(propertyListBuffer2));
			}
		}
		finally
		{
			safeClusterPropertyListHandle.Dispose();
		}
	}

	public void ExecuteOnWmi(Action<WmiAdapter> adapterCallback)
	{
		if (wmiAdapter != null && wmiAdapter.Cluster.GetConnectedToNode() != clusterAdapter.GetConnectedToNode())
		{
			wmiAdapter.Collect();
			wmiAdapter.Close();
			wmiAdapter = null;
		}
		if (wmiAdapter == null)
		{
			wmiAdapter = new WmiAdapter(clusterAdapter.Cluster);
		}
		bool flag = false;
		string text = null;
		do
		{
			try
			{
				text = clusterAdapter.GetConnectedToNode();
				wmiAdapter.Cluster.Open(text, clusterAdapter.Cluster.ClusterAccessRights, out var _);
				try
				{
					adapterCallback?.Invoke(wmiAdapter);
				}
				finally
				{
					wmiAdapter.Close();
				}
			}
			catch (ClusterException ex)
			{
				ClusterWmiWin32Exception ex2 = ex.InnerException as ClusterWmiWin32Exception;
				if (!flag && ex2 != null && (NativeMethods.ErrorCode.RpcServerUnavailable.IsEqual(ex2.NativeErrorCode) || NativeMethods.ErrorCode.FileNotFound.IsEqual(ex2.NativeErrorCode)) && text != clusterAdapter.GetConnectedToNode())
				{
					flag = true;
					continue;
				}
				throw;
			}
		}
		while (flag);
	}

	public void ExecuteOnCim(string computerName, Action<CimAdapter> adapterCallback)
	{
		if (cimAdapter == null)
		{
			cimAdapter = new CimAdapter(clusterAdapter.Cluster);
		}
		adapterCallback.SafeCall(cimAdapter);
	}

	public void ExecuteOnCim(Action<CimAdapter> adapterCallback)
	{
		if (cimAdapter != null && cimAdapter.Cluster.GetConnectedToNode() != clusterAdapter.GetConnectedToNode())
		{
			cimAdapter.Collect();
			cimAdapter.Close();
			cimAdapter = null;
		}
		if (cimAdapter == null)
		{
			cimAdapter = new CimAdapter(clusterAdapter.Cluster);
		}
		bool flag = false;
		string text = null;
		do
		{
			try
			{
				text = clusterAdapter.GetConnectedToNode();
				cimAdapter.Cluster.Open(text, clusterAdapter.Cluster.ClusterAccessRights, out var _);
				try
				{
					adapterCallback.SafeCall(cimAdapter);
				}
				finally
				{
					cimAdapter.Close();
				}
			}
			catch (ClusterException ex)
			{
				ClusterWmiWin32Exception ex2 = ex.InnerException as ClusterWmiWin32Exception;
				if (!flag && ex2 != null && (NativeMethods.ErrorCode.RpcServerUnavailable.IsEqual(ex2.NativeErrorCode) || NativeMethods.ErrorCode.FileNotFound.IsEqual(ex2.NativeErrorCode)) && text != clusterAdapter.GetConnectedToNode())
				{
					flag = true;
					continue;
				}
				throw;
			}
		}
		while (flag);
	}

	public T ExecuteOnWmi<T>(Func<WmiAdapter, T> adapterCallback)
	{
		if (wmiAdapter != null && wmiAdapter.Cluster.GetConnectedToNode() != clusterAdapter.GetConnectedToNode())
		{
			wmiAdapter.Collect();
			wmiAdapter.Close();
			wmiAdapter = null;
		}
		if (wmiAdapter == null)
		{
			wmiAdapter = new WmiAdapter(clusterAdapter.Cluster);
		}
		bool flag = false;
		string text = null;
		do
		{
			try
			{
				text = clusterAdapter.GetConnectedToNode();
				wmiAdapter.Cluster.Open(text, clusterAdapter.Cluster.ClusterAccessRights, out var _);
				try
				{
					if (adapterCallback != null)
					{
						return adapterCallback(wmiAdapter);
					}
				}
				finally
				{
					wmiAdapter.Close();
				}
			}
			catch (ClusterException ex)
			{
				ClusterWmiWin32Exception ex2 = ex.InnerException as ClusterWmiWin32Exception;
				if (!flag && ex2 != null && (NativeMethods.ErrorCode.RpcServerUnavailable.IsEqual(ex2.NativeErrorCode) || NativeMethods.ErrorCode.FileNotFound.IsEqual(ex2.NativeErrorCode)) && text != clusterAdapter.GetConnectedToNode())
				{
					flag = true;
					continue;
				}
				throw;
			}
		}
		while (flag);
		return default(T);
	}

	public T ExecuteOnCim<T>(Func<CimAdapter, T> adapterCallback)
	{
		if (cimAdapter != null && cimAdapter.Cluster.GetConnectedToNode() != clusterAdapter.GetConnectedToNode())
		{
			cimAdapter.Collect();
			cimAdapter.Close();
			cimAdapter = null;
		}
		cimAdapter = new CimAdapter(clusterAdapter.Cluster);
		bool flag = false;
		string text = null;
		do
		{
			try
			{
				text = clusterAdapter.GetConnectedToNode();
				cimAdapter.Cluster.Open(text, clusterAdapter.Cluster.ClusterAccessRights, out var _);
			}
			catch (ClusterException ex)
			{
				ClusterWmiWin32Exception ex2 = ex.InnerException as ClusterWmiWin32Exception;
				if (!flag && ex2 != null && (NativeMethods.ErrorCode.RpcServerUnavailable.IsEqual(ex2.NativeErrorCode) || NativeMethods.ErrorCode.FileNotFound.IsEqual(ex2.NativeErrorCode)) && text != clusterAdapter.GetConnectedToNode())
				{
					flag = true;
					continue;
				}
				throw;
			}
		}
		while (flag);
		return adapterCallback.SafeCall(cimAdapter);
	}

	public WmiAdapter ExecuteOnWmi()
	{
		WmiAdapter wmiAdapter = new WmiAdapter(clusterAdapter.Cluster);
		bool flag = false;
		string text = null;
		while (true)
		{
			try
			{
				text = clusterAdapter.GetConnectedToNode();
				wmiAdapter.Cluster.Open(text, clusterAdapter.Cluster.ClusterAccessRights, out var _);
				return wmiAdapter;
			}
			catch (ClusterException ex)
			{
				ClusterWmiWin32Exception ex2 = ex.InnerException as ClusterWmiWin32Exception;
				if (!flag && ex2 != null && (NativeMethods.ErrorCode.RpcServerUnavailable.IsEqual(ex2.ErrorCode) || NativeMethods.ErrorCode.FileNotFound.IsEqual(ex2.ErrorCode)) && text != clusterAdapter.GetConnectedToNode())
				{
					flag = true;
					continue;
				}
				throw;
			}
		}
	}

	public void Collect()
	{
		clusterAdapter.Collect();
		groups.Collect();
		nodes.Collect();
		networks.Collect();
		networkInterfaces.Collect();
		storage.Collect();
		resources.Collect();
		resourceTypes.Collect();
		wmiAdapter?.Collect();
		cimAdapter?.Collect();
	}

	public void PopulateVirtualMachineStateProperties(IEnumerable<PResource> cacheResources)
	{
		SafeClusterHandle handle = clusterAdapter.Handle;
		if (handle == null)
		{
			return;
		}
		Dictionary<Guid, PVirtualMachineResource> dictionary = new Dictionary<Guid, PVirtualMachineResource>();
		SafeClusterKeyHandle clusterKey = NativeMethods.GetClusterKey(handle, RegistryRights.ExecuteKey);
		if (clusterKey.IsInvalid)
		{
			throw new ClusterRegistryException(new Win32Exception(Marshal.GetLastWin32Error()));
		}
		try
		{
			SafeClusterKeyBatchReadHandle registryReadBatch;
			int num = NativeMethods.ClusterRegCreateReadBatch(clusterKey, out registryReadBatch);
			if (!NativeMethods.ErrorCode.None.IsEqual(num))
			{
				ClusterLog.LogError("Error creating ReadBatch command reading Virtual Machine State properties '{0}'".FormatCurrentCulture(num));
				return;
			}
			SafeClusterKeyBatchReplyHandle replyHandle;
			try
			{
				foreach (PResource cacheResource in cacheResources)
				{
					if (cacheResource.ResourceType.ResourceKind == ResourceKind.VirtualMachine)
					{
						num = NativeMethods.ClusterRegReadBatchAddCommand(registryReadBatch, "Resources\\{0}\\Parameters".FormatInvariantCulture(cacheResource.Id), "VmState");
						if (!NativeMethods.ErrorCode.None.IsEqual(num))
						{
							ClusterLog.LogError("Error adding ReadBatch command reading Virtual Machine State properties '{0}'".FormatCurrentCulture(num));
							return;
						}
						dictionary.Add(cacheResource.Id, (PVirtualMachineResource)cacheResource);
					}
				}
			}
			finally
			{
				registryReadBatch.Dispose();
				replyHandle = registryReadBatch.ReplyHandle;
			}
			if (!NativeMethods.ErrorCode.None.IsEqual(num))
			{
				ClusterLog.LogError("Error closing ReadBatch command reading Virtual Machine State properties '{0}'".FormatCurrentCulture(num));
				return;
			}
			IntPtr intPtr = NativeMethods.Alloc(Marshal.SizeOf(typeof(NativeMethods.CLUSTER_READ_BATCH_COMMAND)));
			try
			{
				while (NativeMethods.ClusterRegReadBatchReplyNextCommand(replyHandle, intPtr) == NativeMethods.ErrorCode.None.ToInt())
				{
					NativeMethods.CLUSTER_READ_BATCH_COMMAND cLUSTER_READ_BATCH_COMMAND = (NativeMethods.CLUSTER_READ_BATCH_COMMAND)Marshal.PtrToStructure(intPtr, typeof(NativeMethods.CLUSTER_READ_BATCH_COMMAND));
					if (cLUSTER_READ_BATCH_COMMAND.Command == NativeMethods.CLUSTER_REG_COMMAND.CLUSREG_READ_VALUE)
					{
						Guid key = new Guid(cLUSTER_READ_BATCH_COMMAND.wzSubkeyName.Substring(10, 36));
						uint value = (uint)Marshal.ReadInt32(cLUSTER_READ_BATCH_COMMAND.lpData);
						PVirtualMachineResource pVirtualMachineResource = dictionary[key];
						ClusterPropertyUInt item = new ClusterPropertyUInt("VmState", null, ClusterPropertyKind.Private, readOnly: false, value);
						pVirtualMachineResource.Properties.Add(item);
						pVirtualMachineResource.Properties.Partial = true;
					}
				}
			}
			finally
			{
				NativeMethods.Free(intPtr);
			}
			replyHandle.Dispose();
		}
		finally
		{
			clusterKey.Dispose();
		}
	}

	protected static void EnumerateValueList(IntPtr valueList, int valueListSize, Action<ValueListIterator> iterator)
	{
		SafeClusterValueListHandle safeClusterValueListHandle = NativeMethods.CreateValueList(valueList, valueListSize);
		if (safeClusterValueListHandle.IsInvalid)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == NativeMethods.ErrorCode.NoMoreItems.ToInt())
			{
				return;
			}
			throw new ClusterDefaultException(new Win32Exception(lastWin32Error));
		}
		int num = 4096;
		IntPtr intPtr = NativeMethods.Alloc(num);
		try
		{
			int num2 = NativeMethods.ResetValueList(safeClusterValueListHandle);
			if (num2 != NativeMethods.ErrorCode.None.ToInt() && num2 != NativeMethods.ErrorCode.NoMoreItems.ToInt())
			{
				throw new ClusterPropertyListBufferException(new Win32Exception(num2));
			}
			NativeMethods.CLUSPROP_SYNTAX syntax = default(NativeMethods.CLUSPROP_SYNTAX);
			int bufferSize = num;
			while ((num2 = NativeMethods.GetNextValue(safeClusterValueListHandle, ref syntax, intPtr, ref bufferSize)) == NativeMethods.ErrorCode.None.ToInt() || num2 == NativeMethods.ErrorCode.MoreData.ToInt())
			{
				if (num2 == NativeMethods.ErrorCode.MoreData.ToInt())
				{
					intPtr = NativeMethods.ReAlloc(intPtr, bufferSize);
					num = bufferSize;
				}
				else
				{
					iterator(new ValueListIterator(syntax.dw, intPtr, bufferSize));
					bufferSize = num;
				}
			}
		}
		finally
		{
			NativeMethods.Free(intPtr);
			safeClusterValueListHandle.Dispose();
		}
	}

	public void Dispose(bool disposing)
	{
		lock (notificationQueueLock)
		{
			if (isDisposed)
			{
				return;
			}
			isDisposed = true;
			if (notificationPort != null)
			{
				notificationPort.Dispose();
			}
			notificationReady.Dispose();
			notificationsPaused.Dispose();
		}
		clusterAdapter.Dispose();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}


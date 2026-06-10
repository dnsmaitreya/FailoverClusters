using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal class WmiAdapter : RootAdapterBase, IConnectionAdapter
{
	internal class AdapterBase
	{
		protected WmiAdapter WmiAdapter { get; private set; }

		public AdapterBase(WmiAdapter wmiAdapter)
		{
			WmiAdapter = wmiAdapter;
		}

		public ManagementObject GetSingleObject(Guid guid, string table)
		{
			ObjectGetOptions op = new ObjectGetOptions();
			return GetSingleObject(guid, table, op);
		}

		public ManagementObject GetSingleObject(Guid guid, string table, ObjectGetOptions op)
		{
			ManagementPath path = new ManagementPath("{0}.Name='{1}'".FormatInvariantCulture(table, guid.ToString()));
			return GetSingleObject(path, op);
		}

		public ManagementObject GetSingleObject(string name, string table)
		{
			ObjectGetOptions op = new ObjectGetOptions();
			return GetSingleObject(name, table, op);
		}

		public ManagementObject GetSingleObject(string name, string table, ObjectGetOptions op)
		{
			ManagementPath path = new ManagementPath("{0}.Name='{1}'".FormatInvariantCulture(table, name));
			return GetSingleObject(path, op);
		}

		public ManagementObject GetSingleObject(ManagementPath path)
		{
			ObjectGetOptions op = new ObjectGetOptions();
			return GetSingleObject(path, op);
		}

		private ManagementObject GetSingleObject(ManagementPath path, ObjectGetOptions op)
		{
			return new ManagementObject(WmiAdapter.Scope, path, op);
		}

		public ManagementObject GetSingleObject(ManagementScope scope, string path)
		{
			ObjectGetOptions options = new ObjectGetOptions();
			return new ManagementObject(scope, new ManagementPath(path), options);
		}

		public T ExecuteAndCatchWmiExceptions<T>(Func<T> action, string objectName)
		{
			try
			{
				return action.SafeCall();
			}
			catch (ManagementException exception)
			{
				ClusterDialogException ex = ConvertException(exception, objectName);
				if (ex != null)
				{
					throw ex;
				}
				return default(T);
			}
			catch (COMException exception2)
			{
				ClusterDialogException ex2 = ConvertException(exception2, objectName);
				if (ex2 != null)
				{
					throw ex2;
				}
				throw;
			}
			catch (UnauthorizedAccessException exception3)
			{
				ClusterDialogException ex3 = ConvertException(exception3, objectName);
				if (ex3 != null)
				{
					throw ex3;
				}
				throw;
			}
			catch (FileNotFoundException innerException)
			{
				throw new ClusterDefaultException(innerException);
			}
		}

		public void ExecuteAndCatchWmiExceptions(Action action, string objectName)
		{
			ExecuteAndCatchWmiExceptions((Func<object>)delegate
			{
				action.SafeCall();
				return null;
			}, objectName);
		}

		public ClusterDialogException ConvertException(Exception exception, string objectName = null)
		{
			if (exception is ManagementException ex)
			{
				if (ex.ErrorInformation != null)
				{
					uint? num = null;
					string text = null;
					ClusterWmiErrorType? clusterWmiErrorType = null;
					try
					{
						PropertyData propertyData = ex.ErrorInformation.Properties["StatusCode"];
						if (propertyData.Value != null)
						{
							num = (uint)propertyData.Value;
						}
					}
					catch (ManagementException exception2)
					{
						ClusterLog.LogException(exception2, "There was an error getting the Status Code property");
					}
					try
					{
						PropertyData propertyData = ex.ErrorInformation.Properties["ErrorType"];
						if (propertyData.Value != null)
						{
							clusterWmiErrorType = (ClusterWmiErrorType)(uint)propertyData.Value;
						}
					}
					catch (ManagementException exception3)
					{
						ClusterLog.LogException(exception3, "There was an error getting the Error Type property");
					}
					try
					{
						PropertyData propertyData = ex.ErrorInformation.Properties["Description"];
						if (propertyData.Value != null)
						{
							text = (string)propertyData.Value;
						}
					}
					catch (ManagementException exception4)
					{
						ClusterLog.LogException(exception4, "There was an error getting the Description property");
					}
					if (!num.HasValue)
					{
						return new ClusterDefaultException(new ClusterWmiWin32Exception((int)ex.ErrorCode, ex.Message, ex.StackTrace));
					}
					if (num == NativeMethods.ErrorCode.IOPending.ToInt())
					{
						return null;
					}
					ClusterWmiWin32Exception ex2 = null;
					ex2 = ((clusterWmiErrorType != ClusterWmiErrorType.VirtualMachine) ? ((text != null) ? new ClusterWmiWin32Exception((int)num.Value, text, ex.StackTrace) : new ClusterWmiWin32Exception((int)num.Value, ex.StackTrace)) : new ClusterWmiWin32Exception(text));
					switch ((NativeMethods.ErrorCode)num.Value)
					{
					case NativeMethods.ErrorCode.ErrorAlreadyExists:
					case NativeMethods.ErrorCode.ObjectAlreadyExists:
						if (this is ResourceAdapter)
						{
							throw new ClusterResourceAlreadyExistException(objectName);
						}
						if (this is GroupAdapter)
						{
							throw new ClusterGroupAlreadyExistException(objectName);
						}
						break;
					case NativeMethods.ErrorCode.GroupNotAvailable:
					case NativeMethods.ErrorCode.GroupNotFound:
						throw new ClusterObjectNotFoundException(objectName, Guid.Empty, typeof(PGroup));
					case NativeMethods.ErrorCode.ResourceNotAvailable:
					case NativeMethods.ErrorCode.ResourceNotFound:
						throw new ClusterObjectNotFoundException(objectName, Guid.Empty, typeof(PResource));
					case NativeMethods.ErrorCode.ResourceLocked:
						throw new ClusterResourceLockedException(objectName, maintenanceMode: false);
					}
					return new ClusterDefaultException(ex2);
				}
				return new ClusterDefaultException(new ClusterWmiWin32Exception((int)ex.ErrorCode, ex.Message, ex.StackTrace));
			}
			if (exception is COMException ex3)
			{
				ClusterWmiWin32Exception innerException = new ClusterWmiWin32Exception(ex3.ErrorCode, exception.StackTrace);
				if (NativeMethods.HRESULT_FROM_WIN32(ex3.ErrorCode) == NativeMethods.HRESULT_FROM_WIN32(NativeMethods.ErrorCode.ResourcePropertiesStored))
				{
					throw new ClusterResourcePropertyStoredException(objectName);
				}
				return new ClusterDefaultException(innerException);
			}
			if (exception is UnauthorizedAccessException)
			{
				return new ClusterDefaultException(exception);
			}
			return null;
		}

		public void ParseProperties(ClusterPropertyCollection propertyCollection, PropertyDataCollection wmiPropertyCollection, ClusterPropertyKind propertiesKind)
		{
			string resourceName = null;
			string resourceType = null;
			ParseProperties(propertyCollection, wmiPropertyCollection, propertiesKind, ref resourceName, ref resourceType);
		}

		public static void ParseProperties(ClusterPropertyCollection propertyCollection, PropertyDataCollection wmiPropertyCollection, ClusterPropertyKind propertiesKind, ref string resourceName, ref string resourceType)
		{
			foreach (PropertyData item in wmiPropertyCollection)
			{
				if (item.Name.Equals("name", StringComparison.OrdinalIgnoreCase))
				{
					resourceName = (string)item.Value;
				}
				if (item.Name.Equals("type", StringComparison.OrdinalIgnoreCase))
				{
					resourceType = (string)item.Value;
				}
				bool flag = false;
				bool readOnly = true;
				string name = item.Name;
				foreach (QualifierData qualifier in item.Qualifiers)
				{
					if (qualifier.Name.Equals("propertytype", StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
					}
					if (qualifier.Name.Equals("propertyname", StringComparison.OrdinalIgnoreCase))
					{
						name = (string)item.Qualifiers["propertyname"].Value;
					}
					if (qualifier.Name.Equals("modify", StringComparison.OrdinalIgnoreCase))
					{
						readOnly = !(bool)qualifier.Value;
					}
				}
				if (!flag && propertiesKind != ClusterPropertyKind.Private)
				{
					continue;
				}
				try
				{
					ClusterProperty clusterProperty = null;
					ClusterPropertyType clusterPropertyType = (ClusterPropertyType)item.Qualifiers["PropertyType"].Value;
					switch (clusterPropertyType)
					{
					case ClusterPropertyType.UnsignedInt:
						switch (item.Type)
						{
						case CimType.Boolean:
							clusterProperty = new ClusterPropertyUInt(name, item.Name, propertiesKind, readOnly, ((bool)item.Value) ? 1u : 0u);
							break;
						case CimType.SInt32:
							clusterProperty = new ClusterPropertyUInt(name, item.Name, propertiesKind, readOnly, (uint)(int)item.Value);
							break;
						case CimType.UInt32:
							clusterProperty = new ClusterPropertyUInt(name, item.Name, propertiesKind, readOnly, (uint)item.Value);
							break;
						default:
							ClusterLog.LogError("WMI Property type {0} is not supported".FormatCurrentCulture(item.Type.ToString()));
							break;
						}
						break;
					case ClusterPropertyType.DateTime:
					{
						DateTime value = DateTime.ParseExact(((string)item.Value).Substring(0, 14), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
						clusterProperty = new ClusterPropertyDateTime(name, item.Name, propertiesKind, readOnly, value);
						break;
					}
					case ClusterPropertyType.String:
						clusterProperty = new ClusterPropertyString(name, item.Name, propertiesKind, readOnly, (string)item.Value);
						break;
					case ClusterPropertyType.ExpandString:
						clusterProperty = new ClusterPropertyExpandString(name, item.Name, propertiesKind, readOnly, (string)item.Value);
						break;
					case ClusterPropertyType.ExpandedString:
						clusterProperty = new ClusterPropertyExpandedString(name, item.Name, propertiesKind, readOnly, (string)item.Value);
						break;
					case ClusterPropertyType.StringCollection:
						clusterProperty = new ClusterPropertyMultipleStrings(name, item.Name, propertiesKind, readOnly, (string[])item.Value);
						break;
					case ClusterPropertyType.Binary:
						clusterProperty = new ClusterPropertyBinary(name, item.Name, propertiesKind, readOnly, (byte[])item.Value);
						break;
					default:
						ClusterLog.LogError("WMI Property type {0} is not supported".FormatCurrentCulture(clusterPropertyType.ToString()));
						break;
					}
					if (clusterProperty != null)
					{
						propertyCollection.Add(clusterProperty);
					}
				}
				catch (Exception exception)
				{
					ClusterLog.LogException(exception, "Failed to parse Cluster WMI property");
					throw;
				}
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

		internal void ExecuteMethod(ManagementPath managementPath, string methodName, ObjectGetOptions op = null)
		{
			object[] parameters = new object[0];
			ExecuteMethod(managementPath, methodName, ref parameters, op);
		}

		internal void ExecuteMethod(ManagementPath managementPath, string methodName, ref object[] parameters, ObjectGetOptions op = null)
		{
			ManagementObject managementObject = null;
			if (op == null)
			{
				op = new ObjectGetOptions();
			}
			try
			{
				managementObject = new ManagementObject(WmiAdapter.Scope, managementPath, op);
				managementObject.InvokeMethod(methodName, parameters);
			}
			finally
			{
				managementObject?.Dispose();
			}
		}
	}

	private class ClusterAdapter : AdapterBase, IConnectionAdapterCluster
	{
		private readonly PCluster memberCluster;

		private string connectedTo;

		private const string AvailableStoragePoolRelPathFormat = "mscluster_availablestoragepool.id=\"{0}\"";

		private const string AvailableStoragePoolQuery = "select id,name,healthstatus,description,quorumstatus,totalsize,usage from mscluster_availablestoragepool";

		private const string AvailablePoolsContextKey = "AVAILABLE_POOLS_KEY";

		private DateTime heartBeat = DateTime.Now;

		internal DateTime HeartBeat
		{
			set
			{
				heartBeat = value;
			}
		}

		internal PCluster Cluster => memberCluster;

		public IEnumerable<Guid> CoreGroups => new List<Guid>();

		public ClusterAdapter(WmiAdapter wmiAdapter, PCluster cluster)
			: base(wmiAdapter)
		{
			memberCluster = cluster;
		}

		public Guid Open(string clusterName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess)
		{
			grantedAccess = desiredAccess;
			ExecuteAndCatchWmiExceptions(delegate
			{
				string wmiNamespace = WmiHelper.GetWmiNamespace(clusterName, WmiConnectionType.Cluster);
				base.WmiAdapter.Scope = new ManagementScope(wmiNamespace);
				base.WmiAdapter.Scope.Options.Authentication = AuthenticationLevel.PacketPrivacy;
				base.WmiAdapter.Scope.Options.Impersonation = ImpersonationLevel.Impersonate;
				base.WmiAdapter.Scope.Options.Timeout = new TimeSpan(0, 0, 5);
				base.WmiAdapter.Scope.Connect();
				base.WmiAdapter.clusterMof = new ManagementClass(base.WmiAdapter.Scope, new ManagementPath("mscluster_cluster"), new ObjectGetOptions());
				base.WmiAdapter.groupMof = new ManagementClass(base.WmiAdapter.Scope, new ManagementPath("mscluster_resourcegroup"), new ObjectGetOptions());
				base.WmiAdapter.resourceMof = new ManagementClass(base.WmiAdapter.Scope, new ManagementPath("mscluster_resource"), new ObjectGetOptions());
				base.WmiAdapter.nodeMof = new ManagementClass(base.WmiAdapter.Scope, new ManagementPath("mscluster_node"), new ObjectGetOptions());
				base.WmiAdapter.networkMof = new ManagementClass(base.WmiAdapter.Scope, new ManagementPath("mscluster_network"), new ObjectGetOptions());
				base.WmiAdapter.networkInterfaceMof = new ManagementClass(base.WmiAdapter.Scope, new ManagementPath("mscluster_networkinterface"), new ObjectGetOptions());
				base.WmiAdapter.privateProp = new ManagementClass(base.WmiAdapter.Scope, new ManagementPath("MSCluster_Properties"), new ObjectGetOptions());
				using ManagementObject managementObject = base.WmiAdapter.GetFirstRecord("mscluster_cluster");
				if (!base.WmiAdapter.clusters.memberCluster.Properties.CommonPropertiesLoaded)
				{
					ParseProperties(base.WmiAdapter.clusters.memberCluster.Properties, managementObject.Properties, ClusterPropertyKind.Common);
				}
				if (!base.WmiAdapter.clusters.memberCluster.Properties.PrivatePropertiesLoaded)
				{
					ParseProperties(base.WmiAdapter.clusters.memberCluster.Properties, ((ManagementBaseObject)managementObject["PrivateProperties"]).Properties, ClusterPropertyKind.Private);
				}
			}, clusterName);
			connectedTo = clusterName;
			return memberCluster.Id;
		}

		public Guid Open(SafeClusterHandle handle)
		{
			throw new NotImplementedException("Open using Safehandle is not implemented for Wmi Adapter");
		}

		public void Close()
		{
			base.WmiAdapter.clusterMof.DisposeSafe();
			base.WmiAdapter.groupMof.DisposeSafe();
			base.WmiAdapter.resourceMof.DisposeSafe();
			base.WmiAdapter.nodeMof.DisposeSafe();
			base.WmiAdapter.networkMof.DisposeSafe();
			base.WmiAdapter.networkInterfaceMof.DisposeSafe();
			base.WmiAdapter.privateProp.DisposeSafe();
		}

		public void Load(PCluster cluster, ClusterLoadSelection loadSelection)
		{
		}

		public void Rename(string newName)
		{
		}

		public string GetConnectedToNode()
		{
			return connectedTo;
		}

		public void Collect()
		{
		}

		public void AddVirtualMachine(Guid vmId, string ownerNodeName)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				using ManagementObject managementObject = new ManagementObject(new ManagementScope(WmiHelper.GetWmiNamespace(ownerNodeName, WmiConnectionType.Cluster))
				{
					Options = 
					{
						Authentication = AuthenticationLevel.PacketPrivacy,
						Impersonation = ImpersonationLevel.Impersonate
					}
				}, options: new ObjectGetOptions(), path: new ManagementPath("MSCluster_Cluster.Name=\"" + Cluster.Name + "\""));
				managementObject.InvokeMethod("AddVirtualMachine", new object[1] { vmId.ToString() });
			}, Cluster.Name);
		}

		public IEnumerable<PResource> CreateDiskResources(IEnumerable<ClusterDisk> clusterableDisks)
		{
			throw new NotSupportedException("CreateDiskResources is not implemented by Wmi Adapter");
		}

		public IEnumerable<ClusterDisk> GetAvailableDisks(Guid poolId)
		{
			throw new NotSupportedException("GetAvailableDisks is not implemented by WMI Adapter");
		}

		public IEnumerable<ClusterDisk> GetAvailableDisks(Guid poolId, bool all)
		{
			throw new NotSupportedException("GetAvailableDisks is not implemented by WMI Adapter");
		}

		public void GetClusterableStoragePools(Action<ClusterableStoragePool> onNext, Action<Exception> onError, Action onCompleted)
		{
			try
			{
				ExecuteAndCatchWmiExceptions(delegate
				{
					ManagementOperationObserver managementOperationObserver = new ManagementOperationObserver();
					ObjectQuery query = new ObjectQuery("select id,name,healthstatus,description,quorumstatus,totalsize,usage from mscluster_availablestoragepool");
					Guid poolContextKey = Guid.NewGuid();
					Action<Exception> completed = delegate(Exception mgmtException)
					{
						if (onError != null && mgmtException != null)
						{
							onError.SafeCall(mgmtException);
						}
						else if (onCompleted != null)
						{
							onCompleted.SafeCall();
						}
					};
					Action<ManagementObject> objectArrived = delegate(ManagementObject mgmtObj)
					{
						try
						{
							onNext(ClusterableStoragePoolFactory.CreateClusterableStoragePool(mgmtObj, poolContextKey));
						}
						finally
						{
							mgmtObj.Dispose();
						}
					};
					ManagementObjectSearcher searcher = new ManagementObjectSearcher(base.WmiAdapter.Scope, query);
					try
					{
						searcher.Options.Context.Add("AVAILABLE_POOLS_KEY", poolContextKey.ToString());
						if (base.WmiAdapter.QueriesAreAsync)
						{
							managementOperationObserver.ObjectReady += delegate(object sender, ObjectReadyEventArgs e)
							{
								objectArrived((ManagementObject)e.NewObject);
							};
							managementOperationObserver.Completed += delegate(object sender, CompletedEventArgs e)
							{
								Exception obj2 = null;
								if (e.Status != 0)
								{
									obj2 = new ManagementException("Generic Failure");
									FieldInfo field = typeof(ManagementException).GetField("errorCode", BindingFlags.Instance | BindingFlags.NonPublic);
									FieldInfo field2 = typeof(ManagementException).GetField("errorObject", BindingFlags.Instance | BindingFlags.NonPublic);
									field.SetValue(obj2, e.Status);
									field2.SetValue(obj2, e.StatusObject);
								}
								completed(obj2);
							};
							searcher.Get(managementOperationObserver);
						}
						else
						{
							Worker.Start(delegate
							{
								ExecuteAndCatchWmiExceptions(delegate
								{
									searcher.Options = new EnumerationOptions
									{
										ReturnImmediately = true,
										Rewindable = false,
										BlockSize = 10000
									};
									using (ManagementObjectCollection managementObjectCollection = searcher.Get())
									{
										foreach (ManagementObject item in managementObjectCollection)
										{
											objectArrived(item);
										}
									}
									completed(null);
								}, Cluster.Name);
							}, completed);
						}
					}
					finally
					{
						if (searcher != null)
						{
							((IDisposable)searcher).Dispose();
						}
					}
				}, Cluster.Name);
			}
			catch (ClusterException operationResult)
			{
				onError.SafeCall(operationResult);
			}
		}

		public void AddStoragePoolToCluster(ClusterableStoragePool clusterableStoragePool, Action<Exception> onError, Action onCompleted)
		{
			try
			{
				ObjectGetOptions objectGetOptions = new ObjectGetOptions();
				objectGetOptions.Context.Add("AVAILABLE_POOLS_KEY", clusterableStoragePool.ContextId.ToString());
				ExecuteMethod(new ManagementPath("mscluster_availablestoragepool.id=\"{0}\"".FormatInvariantCulture(clusterableStoragePool.PoolId)), "AddToCluster", objectGetOptions);
				onCompleted?.Invoke();
			}
			catch (Exception ex)
			{
				ClusterDialogException ex2 = ConvertException(ex, Cluster.Name);
				if (ex2 != null)
				{
					if (onError == null)
					{
						throw ex2;
					}
					onError(ex2);
				}
				else
				{
					if (onError == null)
					{
						throw;
					}
					onError(ex);
				}
			}
		}

		public bool WillVoterLossCauseQuorumLoss(QuorumVoterActionCheck voterActionCheck, string id)
		{
			throw new NotImplementedException("WillVoterLossCauseQuorumLoss is not implemented for WmiAdapeter");
		}

		public void Shutdown()
		{
			throw new NotImplementedException("Shutdown is not implemented for WmiAdapeter");
		}

		public QuorumConfigurationPrivate GetQuorumConfiguration()
		{
			throw new NotImplementedException("GetQuorumConfiguration is not implemented for WmiAdapeter");
		}

		public void SetQuorumConfiguration(QuorumType quorumType, WitnessType witnessType, string quorumWitness, IEnumerable<string> nonVotingNodes)
		{
			throw new NotImplementedException("SetQuorumConfiguration is not implemented for WmiAdapeter");
		}

		public FileShareValidationStatus VerifyFileShare(string path)
		{
			throw new NotImplementedException("VerifyFileShare is not implemented for WmiAdapter");
		}

		public void UpdateFunctionalLevel(bool whatIf)
		{
			throw new NotImplementedException("UpdateFunctionalLevel is not implemented for WmiAdapter");
		}

		public void Destroy(bool deletecComputerObjects)
		{
			throw new NotImplementedException("Destroy is not implemented for WmiAdapeter");
		}

		public string GetFullyQualifiedDomainName()
		{
			throw new NotImplementedException("GetFullyQualifiedDomainName is not implemented for WmiAdapter");
		}
	}

	private class GroupAdapter : AdapterBase, IConnectionAdapterGroup
	{
		private readonly object loadingGroupsLock = new object();

		private const string GroupElementatyPayloadQuery = "id,name,grouptype";

		private const string GroupBasicPayloadQuery = ",flags,state,iscore,ownernode,priority";

		private const string GroupCommonPropertiesQuery = ",AntiAffinityClassNames,AutoFailbackType,DefaultOwner,Description,FailbackWindowEnd,FailbackWindowStart,FailoverPeriod,FailoverThreshold,Name,PersistentState,Priority";

		private const string GroupPrivatePropertiesQuery = ",PrivateProperties";

		public object LoadingGroupLock => loadingGroupsLock;

		public string[] PropertiesName
		{
			get
			{
				List<string> list = new List<string>();
				list.AddRange(",AntiAffinityClassNames,AutoFailbackType,DefaultOwner,Description,FailbackWindowEnd,FailbackWindowStart,FailoverPeriod,FailoverThreshold,Name,PersistentState,Priority".Split(','));
				list.AddRange(",PrivateProperties".Split(','));
				return list.ToArray();
			}
		}

		public GroupAdapter(WmiAdapter wmiAdapter)
			: base(wmiAdapter)
		{
			Init();
		}

		private void Init()
		{
		}

		public void Close(Guid id)
		{
		}

		public void Close(string name)
		{
		}

		public void Delete(Guid id, bool force, bool cleanup)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				uint num = (cleanup ? 1u : 0u);
				ExecuteGroupMethod(id, "DestroyGroup", num);
			}, id.ToString());
		}

		public PGroup Create(string name, GroupType groupType)
		{
			object[] parameters = new object[3]
			{
				name,
				(int)groupType,
				string.Empty
			};
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteGroupMethod(Guid.Empty, "CreateGroup", ref parameters);
			}, name);
			return PGroup.Constructor(id: new Guid(parameters[2].ToString()), cluster: base.WmiAdapter.clusters.Cluster, name: name, groupType: groupType);
		}

		public void CancelOperation(Guid id)
		{
			ClusterLog.LogWarning("Cancel operation for a group was called, However Wmi adapter doesn't support this method.");
		}

		public void Rename(Guid id, string newName)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteGroupMethod(id, "Rename", newName);
			}, newName);
		}

		public void SetPriority(Guid id, Priority priority)
		{
			SetGroupProperty(id, "Priority", (uint)priority);
		}

		public void Online(Guid id, bool overrideLockState = false, bool chooseBestNode = false)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteGroupMethod(id, "BringOnline", 0);
			}, id.ToString());
		}

		public void Offline(Guid id, bool overrideLockState = false)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteGroupMethod(id, "TakeOffline", 0);
			}, id.ToString());
		}

		public IEnumerable<string> GetPreferredOwners(Guid id)
		{
			throw new NotSupportedException("GetPreferredOwnerNodes is not supported by Wmi Adapter");
		}

		public void Move(Guid id, string nodeName, bool overrideLockState = false)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteGroupMethod(id, "MoveToNewNode", nodeName);
			}, id.ToString());
		}

		public IEnumerable<PGroup> GetAll(bool nullElementOnError)
		{
			ManagementOperationObserver managementOperationObserver = new ManagementOperationObserver();
			AutoResetEvent doneEvent = new AutoResetEvent(initialState: true);
			Exception lastError = null;
			PGroup returnObject = null;
			AutoResetEvent rdyEvent = new AutoResetEvent(initialState: false);
			Action<Exception> completed = delegate(Exception mgmtException)
			{
				lastError = mgmtException;
				returnObject = null;
				rdyEvent.Set();
			};
			Action<ManagementObject> objectArrived = delegate(ManagementObject mgmtObj)
			{
				try
				{
					doneEvent.WaitOne();
					PGroup pGroup = PGroup.Constructor(base.WmiAdapter.clusters.Cluster, new Guid((string)mgmtObj["Id"]), (string)mgmtObj["Name"], (GroupType)(uint)mgmtObj["grouptype"]);
					pGroup.Flags = (GroupFlags)(uint)mgmtObj["flags"];
					pGroup.GroupState = (GroupState)(uint)mgmtObj["State"];
					pGroup.IsCore = (bool)mgmtObj["IsCore"];
					pGroup.Priority = (Priority)(uint)mgmtObj["Priority"];
					string text = (string)mgmtObj["OwnerNode"];
					PNode ownerNode = new PNode(pGroup.Cluster, base.WmiAdapter.nodes.GetNodeIdFromName(text), text);
					pGroup.OwnerNode = ownerNode;
					pGroup.Properties.Add(new ClusterPropertyString("Name", null, ClusterPropertyKind.Common, readOnly: true, (string)mgmtObj["Name"]));
					pGroup.Properties.Add(new ClusterPropertyString("Description", null, ClusterPropertyKind.Common, readOnly: false, (string)mgmtObj["Description"]));
					pGroup.Properties.Add(new ClusterPropertyUInt("PersistentState", null, ClusterPropertyKind.Common, readOnly: false, ((bool)mgmtObj["PersistentState"]) ? 1u : 0u));
					pGroup.Properties.Add(new ClusterPropertyUInt("FailoverThreshold", null, ClusterPropertyKind.Common, readOnly: false, (uint)mgmtObj["FailoverThreshold"]));
					pGroup.Properties.Add(new ClusterPropertyUInt("FailoverPeriod", null, ClusterPropertyKind.Common, readOnly: false, (uint)mgmtObj["FailoverPeriod"]));
					pGroup.Properties.Add(new ClusterPropertyUInt("AutoFailbackType", null, ClusterPropertyKind.Common, readOnly: false, (uint)mgmtObj["AutoFailbackType"]));
					pGroup.Properties.Add(new ClusterPropertyUInt("FailbackWindowStart", null, ClusterPropertyKind.Common, readOnly: false, (uint)(int)mgmtObj["FailbackWindowStart"]));
					pGroup.Properties.Add(new ClusterPropertyUInt("FailbackWindowEnd", null, ClusterPropertyKind.Common, readOnly: false, (uint)(int)mgmtObj["FailbackWindowEnd"]));
					pGroup.Properties.Add(new ClusterPropertyUInt("Priority", null, ClusterPropertyKind.Common, readOnly: false, (uint)mgmtObj["Priority"]));
					pGroup.Properties.Add(new ClusterPropertyUInt("DefaultOwner", null, ClusterPropertyKind.Common, readOnly: false, (uint)mgmtObj["DefaultOwner"]));
					pGroup.Properties.CommonPropertiesLoaded = true;
					pGroup.LoadedSelection = 1;
					returnObject = pGroup;
					lock (loadingGroupsLock)
					{
						if (!base.WmiAdapter.MappingIdNameGroup.ContainsKey(returnObject.Id))
						{
							base.WmiAdapter.MappingIdNameGroup.TryAdd(returnObject.Id, returnObject.Name);
						}
						if (!base.WmiAdapter.MappingNameIdGroup.ContainsKey(returnObject.Name))
						{
							base.WmiAdapter.MappingNameIdGroup.TryAdd(returnObject.Name, returnObject.Id);
						}
					}
				}
				catch (Exception ex)
				{
					returnObject = null;
					lastError = ex;
				}
				finally
				{
					rdyEvent.Set();
				}
			};
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select");
			stringBuilder.Append(" name");
			stringBuilder.Append(", id");
			stringBuilder.Append(", grouptype");
			stringBuilder.Append(", state");
			stringBuilder.Append(", priority");
			stringBuilder.Append(", ownernode");
			stringBuilder.Append(", iscore");
			stringBuilder.Append(", flags");
			stringBuilder.Append(",AntiAffinityClassNames,AutoFailbackType,DefaultOwner,Description,FailbackWindowEnd,FailbackWindowStart,FailoverPeriod,FailoverThreshold,Name,PersistentState,Priority");
			stringBuilder.Append(" from ");
			stringBuilder.Append("mscluster_resourcegroup");
			ObjectQuery query = new ObjectQuery(stringBuilder.ToString());
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(base.WmiAdapter.Scope, query);
			try
			{
				if (base.WmiAdapter.QueriesAreAsync)
				{
					managementOperationObserver.ObjectReady += delegate(object sender, ObjectReadyEventArgs e)
					{
						objectArrived((ManagementObject)e.NewObject);
					};
					managementOperationObserver.Completed += delegate(object sender, CompletedEventArgs e)
					{
						ManagementException obj2 = null;
						if (e.Status != 0)
						{
							obj2 = new ManagementException("Generic Failure");
							FieldInfo field = typeof(ManagementException).GetField("errorCode", BindingFlags.Instance | BindingFlags.NonPublic);
							FieldInfo field2 = typeof(ManagementException).GetField("errorObject", BindingFlags.Instance | BindingFlags.NonPublic);
							field.SetValue(obj2, e.Status);
							field2.SetValue(obj2, e.StatusObject);
						}
						completed(obj2);
					};
					searcher.Get(managementOperationObserver);
				}
				else
				{
					Worker.Start(delegate
					{
						searcher.Options = new EnumerationOptions
						{
							ReturnImmediately = true,
							Rewindable = false,
							BlockSize = 10000
						};
						using (ManagementObjectCollection managementObjectCollection = searcher.Get())
						{
							foreach (ManagementObject item in managementObjectCollection)
							{
								objectArrived(item);
							}
						}
						completed(null);
					}, completed);
				}
				while (true)
				{
					rdyEvent.WaitOne();
					if (returnObject == null)
					{
						if (lastError == null)
						{
							break;
						}
						if (!nullElementOnError)
						{
							throw lastError;
						}
						ClusterLog.LogException(lastError, "An error has occurred on the group enumeration, however is not critical and the process can continue");
					}
					yield return returnObject;
					doneEvent.Set();
				}
			}
			finally
			{
				if (searcher != null)
				{
					((IDisposable)searcher).Dispose();
				}
			}
		}

		private List<Guid> GetPreferredOwnersList(Guid id)
		{
			object[] parameters = new object[1];
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteGroupMethod(id, "GetPreferredOwners", ref parameters);
			}, id.ToString());
			return new List<Guid>(((Array)parameters[0]).ConvertAll((string nodeName) => base.WmiAdapter.nodes.GetNodeIdFromName(nodeName)));
		}

		public void SetPreferredOwners(Guid id, IEnumerable<string> nodes)
		{
			object[] parameters = new object[1];
			parameters[0] = nodes;
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteGroupMethod(id, "SetPreferredOwners", ref parameters);
			}, id.ToString());
		}

		public PGroup Open(Guid id)
		{
			PGroup pGroup = null;
			using (ManagementObject managementObject = GetSingleObject(id, "mscluster_resourcegroup"))
			{
				GroupType groupType = (GroupType)(uint)managementObject["grouptype"];
				pGroup = PGroup.Constructor(base.WmiAdapter.clusters.Cluster, new Guid((string)managementObject["Id"]), (string)managementObject["Name"], groupType);
				lock (loadingGroupsLock)
				{
					if (!base.WmiAdapter.MappingIdNameGroup.ContainsKey(pGroup.Id))
					{
						base.WmiAdapter.MappingIdNameGroup.TryAdd(pGroup.Id, pGroup.Name);
					}
					if (!base.WmiAdapter.MappingNameIdGroup.ContainsKey(pGroup.Name))
					{
						base.WmiAdapter.MappingNameIdGroup.TryAdd(pGroup.Name, pGroup.Id);
					}
				}
			}
			return pGroup;
		}

		public PGroup Open(string groupName)
		{
			PGroup pGroup = null;
			using (ManagementObject managementObject = GetSingleObject(groupName, "mscluster_resourcegroup"))
			{
				GroupType groupType = (GroupType)(uint)managementObject["grouptype"];
				pGroup = PGroup.Constructor(base.WmiAdapter.clusters.Cluster, new Guid((string)managementObject["Id"]), (string)managementObject["Name"], groupType);
				lock (loadingGroupsLock)
				{
					if (!base.WmiAdapter.MappingIdNameGroup.ContainsKey(pGroup.Id))
					{
						base.WmiAdapter.MappingIdNameGroup.TryAdd(pGroup.Id, pGroup.Name);
					}
					if (!base.WmiAdapter.MappingNameIdGroup.ContainsKey(pGroup.Name))
					{
						base.WmiAdapter.MappingNameIdGroup.TryAdd(pGroup.Name, pGroup.Id);
					}
				}
			}
			return pGroup;
		}

		public void Load(PGroup group, GroupLoadSelection loadSelection)
		{
			if ((loadSelection & GroupLoadSelection.Basic) == GroupLoadSelection.Basic || (loadSelection & GroupLoadSelection.CommonProperties) == GroupLoadSelection.CommonProperties || (loadSelection & GroupLoadSelection.PrivateProperties) == GroupLoadSelection.PrivateProperties)
			{
				using ManagementObject managementObject = GetSingleObject(group.Id, "mscluster_resourcegroup");
				if (managementObject == null)
				{
					throw new ClusterObjectNotFoundException(group.Name, group.Id, typeof(PGroup));
				}
				try
				{
					if (!group.GroupState.HasValue)
					{
						group.GroupState = (GroupState)(uint)managementObject["State"];
					}
					if (!group.Priority.HasValue)
					{
						group.Priority = (Priority)(uint)managementObject["Priority"];
					}
					if (group.OwnerNode == null)
					{
						string text = (string)managementObject["OwnerNode"];
						PNode ownerNode = new PNode(group.Cluster, base.WmiAdapter.nodes.GetNodeIdFromName(text), text);
						group.OwnerNode = ownerNode;
					}
					if (!group.IsCore.HasValue)
					{
						group.IsCore = (bool)managementObject["IsCore"];
					}
					if (!group.Flags.HasValue)
					{
						group.Flags = (GroupFlags)(uint)managementObject["Flags"];
					}
					group.LoadedSelection |= 1;
					ParseProperties(group.Properties, managementObject.Properties, ClusterPropertyKind.Common);
					group.LoadedSelection |= 2;
					ParseProperties(group.Properties, ((ManagementBaseObject)managementObject["PrivateProperties"]).Properties, ClusterPropertyKind.Private);
					group.LoadedSelection |= 4;
				}
				catch (Exception innerException)
				{
					throw new ClusterObjectLoadFailedException(group.Name, group.Id, innerException);
				}
			}
			if ((loadSelection & GroupLoadSelection.PreferredOwners) == GroupLoadSelection.PreferredOwners)
			{
				group.PreferredOwners = GetPreferredOwnersList(group.Id);
				group.LoadedSelection |= 8;
			}
		}

		internal List<string> NormalizeQuery(List<string> fieldsString, out GroupLoadSelection loadSelection)
		{
			loadSelection = GroupLoadSelection.None;
			List<string> list = new List<string>();
			foreach (string item in fieldsString)
			{
				if (",flags,state,iscore,ownernode,priority".Contains("," + item.ToLowerInvariant()))
				{
					loadSelection |= GroupLoadSelection.Basic;
				}
				if (",AntiAffinityClassNames,AutoFailbackType,DefaultOwner,Description,FailbackWindowEnd,FailbackWindowStart,FailoverPeriod,FailoverThreshold,Name,PersistentState,Priority".Contains("," + item.ToLowerInvariant()))
				{
					loadSelection |= GroupLoadSelection.CommonProperties;
				}
				if (item.Equals("privateproperties"))
				{
					loadSelection |= GroupLoadSelection.PrivateProperties;
				}
			}
			list.AddRange("id,name,grouptype".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			if ((loadSelection & GroupLoadSelection.Basic) == GroupLoadSelection.Basic)
			{
				list.AddRange(",flags,state,iscore,ownernode,priority".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			}
			if ((loadSelection & GroupLoadSelection.CommonProperties) == GroupLoadSelection.CommonProperties)
			{
				list.AddRange(",AntiAffinityClassNames,AutoFailbackType,DefaultOwner,Description,FailbackWindowEnd,FailbackWindowStart,FailoverPeriod,FailoverThreshold,Name,PersistentState,Priority".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			}
			if ((loadSelection & GroupLoadSelection.PrivateProperties) == GroupLoadSelection.PrivateProperties)
			{
				list.AddRange(",PrivateProperties".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			}
			return list;
		}

		internal TResult ProcessResult<TResult>(ManagementObject mgmtObj, QueryInfo queryInfo, GroupLoadSelection loadSelection) where TResult : PClusterObject
		{
			PGroup pGroup = PGroup.Constructor(base.WmiAdapter.clusters.Cluster, new Guid((string)mgmtObj["Id"]), (string)mgmtObj["Name"], (GroupType)(uint)mgmtObj["grouptype"]);
			if ((loadSelection & GroupLoadSelection.Basic) == GroupLoadSelection.Basic)
			{
				pGroup.Flags = (GroupFlags)(uint)mgmtObj["flags"];
				pGroup.GroupState = (GroupState)(uint)mgmtObj["state"];
				pGroup.IsCore = (bool)mgmtObj["iscore"];
				Guid id = Guid.Empty;
				string text = (string)mgmtObj["ownernode"];
				lock (base.WmiAdapter.nodes.LoadingNodesLock)
				{
					id = base.WmiAdapter.nodes.GetNodeIdFromName(text);
				}
				PNode pNode = new PNode(base.WmiAdapter.clusters.Cluster, id, text);
				pNode.State = NodeState.Down;
				pGroup.OwnerNode = pNode;
				pGroup.Priority = (Priority)(uint)mgmtObj["Priority"];
				pGroup.LoadedSelection |= 1;
			}
			if ((loadSelection & GroupLoadSelection.CommonProperties) == GroupLoadSelection.CommonProperties)
			{
				ParseProperties(pGroup.Properties, mgmtObj.Properties, ClusterPropertyKind.Common);
			}
			if ((loadSelection & GroupLoadSelection.PrivateProperties) == GroupLoadSelection.PrivateProperties)
			{
				ParseProperties(pGroup.Properties, ((ManagementBaseObject)mgmtObj["PrivateProperties"]).Properties, ClusterPropertyKind.Private);
			}
			if ((loadSelection & GroupLoadSelection.PreferredOwners) == GroupLoadSelection.PreferredOwners)
			{
				pGroup.PreferredOwners = GetPreferredOwnersList(pGroup.Id);
				pGroup.LoadedSelection |= 8;
			}
			return (TResult)(PClusterObject)pGroup;
		}

		internal Guid GetGroupIdFromName(string groupName)
		{
			if (!base.WmiAdapter.MappingNameIdGroup.TryGetValue(groupName, out var value))
			{
				lock (loadingGroupsLock)
				{
					if (!base.WmiAdapter.MappingNameIdGroup.TryGetValue(groupName, out value))
					{
						using ManagementObject managementObject = GetSingleObject(groupName, "mscluster_resourcegroup");
						value = new Guid((string)managementObject["Id"]);
						lock (loadingGroupsLock)
						{
							if (!base.WmiAdapter.MappingIdNameGroup.ContainsKey(value))
							{
								base.WmiAdapter.MappingIdNameGroup.TryAdd(value, groupName);
							}
							if (!base.WmiAdapter.MappingNameIdGroup.ContainsKey(groupName))
							{
								base.WmiAdapter.MappingNameIdGroup.TryAdd(groupName, value);
							}
						}
					}
				}
			}
			return value;
		}

		private void ExecuteGroupMethod(Guid id, string methodName, object parameter)
		{
			object[] parameters = new object[1] { parameter };
			ExecuteGroupMethod(id, methodName, ref parameters);
		}

		private void ExecuteGroupMethod(Guid id, string methodName, ref object[] parameters)
		{
			ObjectGetOptions options = new ObjectGetOptions();
			ManagementObject managementObject = null;
			try
			{
				if (id == Guid.Empty)
				{
					managementObject = new ManagementClass(base.WmiAdapter.Scope, new ManagementPath("MSCluster_ResourceGroup"), options);
				}
				else
				{
					string text = id.ToString();
					managementObject = new ManagementObject(base.WmiAdapter.Scope, new ManagementPath("MSCluster_ResourceGroup.Name=\"" + text + "\""), options);
				}
				managementObject.InvokeMethod(methodName, parameters);
			}
			finally
			{
				managementObject?.Dispose();
			}
		}

		private void SetGroupProperty(Guid id, string propertyName, object propertyValue)
		{
			ObjectGetOptions op = new ObjectGetOptions();
			ExecuteAndCatchWmiExceptions(delegate
			{
				using ManagementObject managementObject = ((id == Guid.Empty) ? new ManagementClass(base.WmiAdapter.Scope, new ManagementPath("MSCluster_ResourceGroup"), op) : new ManagementObject(base.WmiAdapter.Scope, new ManagementPath(string.Concat("MSCluster_ResourceGroup.Name=\"", id, "\"")), op));
				managementObject.Properties[propertyName].Value = propertyValue;
				managementObject.Put();
			}, id.ToString());
		}

		public bool ProcessEvent(ManagementBaseObject newEvent)
		{
			switch (newEvent.ClassPath.ClassName)
			{
			case "MSCluster_EventGroupOwnerNodeChanged":
			{
				string text6 = (string)newEvent["EventObjectId"];
				if (text6 == null)
				{
					return true;
				}
				Guid id2 = new Guid(text6);
				string key = (string)newEvent["EventOwnerNode"];
				Guid value8 = Guid.Empty;
				lock (base.WmiAdapter.nodes.LoadingNodesLock)
				{
					if (!base.WmiAdapter.MappingNameIdNode.TryGetValue(key, out value8))
					{
						return true;
					}
				}
				base.WmiAdapter.EnqueueNotification(new GroupNotification(new ClusterGroupOwnerNodeEventArgs(id2, value8, null)));
				return true;
			}
			case "MSCluster_EventGroupStateChanged":
			{
				string text2 = (string)newEvent["EventObjectId"];
				if (text2 == null)
				{
					return true;
				}
				Guid id = new Guid(text2);
				object obj = newEvent["EventNewState"];
				if (obj == null)
				{
					return true;
				}
				GroupState value = (GroupState)(uint)obj;
				base.WmiAdapter.EnqueueNotification(new GroupNotification(new ClusterGroupStateEventArgs(id, value, null)));
				return true;
			}
			case "MSCluster_EventGroupRenamed":
			{
				string text3 = (string)newEvent["EventObjectName"];
				Guid guid2 = Guid.Empty;
				lock (loadingGroupsLock)
				{
					string text4 = (string)newEvent["EventObjectId"];
					if (text4 == null)
					{
						return true;
					}
					guid2 = new Guid(text4);
					if (!base.WmiAdapter.MappingIdNameGroup.TryGetValue(guid2, out var value2))
					{
						return true;
					}
					base.WmiAdapter.MappingIdNameGroup.TryRemove(guid2, out var _);
					base.WmiAdapter.MappingIdNameGroup.TryAdd(guid2, text3);
					base.WmiAdapter.MappingNameIdGroup.TryRemove(value2, out var _);
					base.WmiAdapter.MappingNameIdGroup.TryAdd(text3, guid2);
				}
				base.WmiAdapter.EnqueueNotification(new GroupNotification(new ClusterRenamedEventArgs(guid2, text3, null)));
				return true;
			}
			case "MSCluster_EventGroupAdded":
			{
				string text7 = (string)newEvent["EventObjectName"];
				Guid guid3 = new Guid((string)newEvent["EventObjectId"]);
				GroupType value9 = (GroupType)(uint)newEvent["EventGroupType"];
				lock (loadingGroupsLock)
				{
					if (!base.WmiAdapter.MappingIdNameGroup.ContainsKey(guid3))
					{
						base.WmiAdapter.MappingIdNameGroup.TryAdd(guid3, text7);
					}
					if (!base.WmiAdapter.MappingNameIdGroup.ContainsKey(text7))
					{
						base.WmiAdapter.MappingNameIdGroup.TryAdd(text7, guid3);
					}
				}
				ClusterAddedEventArgs clusterAddedEventArgs = new ClusterAddedEventArgs(guid3, text7, (int)value9, null);
				clusterAddedEventArgs.Cluster = base.WmiAdapter.clusters.Cluster;
				base.WmiAdapter.EnqueueNotification(new GroupNotification(clusterAddedEventArgs));
				return true;
			}
			case "MSCluster_EventGroupRemoved":
			{
				string text5 = (string)newEvent["EventObjectName"];
				Guid value5 = Guid.Empty;
				lock (loadingGroupsLock)
				{
					if (!base.WmiAdapter.MappingNameIdGroup.TryGetValue(text5, out value5))
					{
						return true;
					}
					base.WmiAdapter.MappingIdNameGroup.TryRemove(value5, out var _);
					base.WmiAdapter.MappingNameIdGroup.TryRemove(text5, out var _);
				}
				ClusterRemovedEventArgs clusterRemovedEventArgs = new ClusterRemovedEventArgs(value5, text5, null);
				clusterRemovedEventArgs.Cluster = base.WmiAdapter.clusters.Cluster;
				base.WmiAdapter.EnqueueNotification(new GroupNotification(clusterRemovedEventArgs));
				return true;
			}
			case "MSCluster_EventGroupPropertyChanged":
			{
				string name = (string)newEvent["EventObjectName"];
				string text = (string)newEvent["EventObjectId"];
				if (text == null)
				{
					return true;
				}
				Guid guid = new Guid(text);
				Priority priority = Priority.Fetching;
				ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(base.WmiAdapter.clusters.Cluster.Id, guid, ClusterIdentityType.Group);
				ParseProperties(clusterPropertyCollection, ((ManagementBaseObject)newEvent["EventProperties"]).Properties, ClusterPropertyKind.Common);
				ParseProperties(clusterPropertyCollection, ((ManagementBaseObject)newEvent["EventPrivateProperties"]).Properties, ClusterPropertyKind.Private);
				ClusterPropertiesEventArgs clusterPropertiesEventArgs = new ClusterPropertiesEventArgs(guid, null, null, null)
				{
					Properties = clusterPropertyCollection
				};
				priority = (Priority)(uint)clusterPropertiesEventArgs.Properties["Priority"].Value;
				clusterPropertiesEventArgs.Name = name;
				clusterPropertiesEventArgs.ObjectType = null;
				clusterPropertiesEventArgs.Cluster = base.WmiAdapter.clusters.Cluster;
				base.WmiAdapter.EnqueueNotification(new GroupNotification(new ClusterGroupPriorityEventArgs(guid, priority, null)));
				base.WmiAdapter.EnqueueNotification(new GroupNotification(clusterPropertiesEventArgs));
				List<Guid> preferredOwnersList = GetPreferredOwnersList(guid);
				base.WmiAdapter.EnqueueNotification(new GroupNotification(new ClusterGroupPreferredOwnersChangedEventArgs(guid, preferredOwnersList, null)));
				return true;
			}
			default:
				return false;
			}
		}

		public void MigrateVirtualMachine(PVirtualMachineGroup group, PNode node, VirtualMachineMigrationType migrationType, bool overrideLockState = false)
		{
			throw new NotImplementedException();
		}

		public void Collect()
		{
		}
	}

	private class NetworkAdapter : AdapterBase, IConnectionAdapterNetwork
	{
		private const string NetworkElementaryPayloadQuery = "id,name";

		private const string NetworkbasicPayloadQuery = "";

		private const string NetworkCommonPropertiesQuery = "";

		private const string NetworkPrivatePropertiesQuery = "";

		public string[] PropertyNames
		{
			get
			{
				List<string> list = new List<string>();
				list.AddRange("".Split(','));
				list.AddRange("".Split(','));
				return list.ToArray();
			}
		}

		public NetworkAdapter(WmiAdapter wmiAdapter)
			: base(wmiAdapter)
		{
			Init();
		}

		public void Init()
		{
		}

		public PNetwork Open(Guid id)
		{
			throw new NotImplementedException();
		}

		public PNetwork Open(string networkName)
		{
			throw new NotImplementedException();
		}

		public void Rename(Guid id, string newName)
		{
			throw new NotImplementedException();
		}

		internal List<string> NormalizeQuery(List<string> fieldsString, out NetworkLoadSelection loadSelection)
		{
			loadSelection = NetworkLoadSelection.None;
			List<string> list = new List<string>();
			foreach (string item in fieldsString)
			{
				if ("".Contains("," + item.ToLowerInvariant()))
				{
					loadSelection |= NetworkLoadSelection.Basic;
				}
			}
			list.AddRange("id,name".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			if ((loadSelection & NetworkLoadSelection.Basic) == NetworkLoadSelection.Basic)
			{
				list.AddRange("".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			}
			return list;
		}

		internal TResult ProcessResult<TResult>(ManagementObject mgmtObj, QueryInfo queryInfo, NetworkLoadSelection loadSelection) where TResult : PClusterObject
		{
			PNetwork pNetwork = new PNetwork(base.WmiAdapter.clusters.Cluster, new Guid((string)mgmtObj["Id"]), (string)mgmtObj["Name"]);
			if ((loadSelection & NetworkLoadSelection.Basic) == NetworkLoadSelection.Basic)
			{
				pNetwork.LoadedSelection |= (int)loadSelection;
			}
			return (TResult)(PClusterObject)pNetwork;
		}

		public void Load(PNetwork network, NetworkLoadSelection loadSelection)
		{
			throw new NotSupportedException("Load Network is not supported on the WMI Adapter");
		}

		public bool ProcessEvent(ManagementBaseObject newEvent)
		{
			return false;
		}

		public void Collect()
		{
		}

		public IEnumerable<PNetwork> GetAll(bool nullElementOnError)
		{
			throw new NotImplementedException("WmiAdapter.NetworkAdapter does not implement GetAll()");
		}
	}

	private class NetworkInterfaceAdapter : AdapterBase, IConnectionAdapterNetworkInterface
	{
		private const string NetworkInterfaceElementaryPayloadQuery = "id,name";

		private const string NetworkInterfacebasicPayloadQuery = "";

		private const string NetworkInterfaceCommonPropertiesQuery = "";

		private const string NetworkInterfacePrivatePropertiesQuery = "";

		private const string NetworkAdapterDnsSuffixQuery = "Select DNSDomain from Win32_NetworkAdapterConfiguration";

		public string[] PropertyNames
		{
			get
			{
				List<string> list = new List<string>();
				list.AddRange("".Split(','));
				list.AddRange("".Split(','));
				return list.ToArray();
			}
		}

		public NetworkInterfaceAdapter(WmiAdapter wmiAdapter)
			: base(wmiAdapter)
		{
			Init();
		}

		public void Init()
		{
		}

		public PNetworkInterface Open(Guid id)
		{
			throw new NotImplementedException();
		}

		public PNetworkInterface Open(string networkInterfaceName)
		{
			throw new NotImplementedException();
		}

		internal List<string> NormalizeQuery(List<string> fieldsString, out NetworkInterfaceLoadSelection loadSelection)
		{
			loadSelection = NetworkInterfaceLoadSelection.None;
			List<string> list = new List<string>();
			foreach (string item in fieldsString)
			{
				if ("".Contains("," + item.ToLowerInvariant()))
				{
					loadSelection |= NetworkInterfaceLoadSelection.Basic;
				}
			}
			list.AddRange("id,name".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			if ((loadSelection & NetworkInterfaceLoadSelection.Basic) == NetworkInterfaceLoadSelection.Basic)
			{
				list.AddRange("".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			}
			return list;
		}

		internal TResult ProcessResult<TResult>(ManagementObject mgmtObj, QueryInfo queryInfo, NetworkInterfaceLoadSelection loadSelection) where TResult : PClusterObject
		{
			PNetworkInterface pNetworkInterface = new PNetworkInterface(base.WmiAdapter.clusters.Cluster, new Guid((string)mgmtObj["Id"]), (string)mgmtObj["Name"]);
			if ((loadSelection & NetworkInterfaceLoadSelection.Basic) == NetworkInterfaceLoadSelection.Basic)
			{
				pNetworkInterface.LoadedSelection |= (int)loadSelection;
			}
			return (TResult)(PClusterObject)pNetworkInterface;
		}

		public void Load(PNetworkInterface network, NetworkInterfaceLoadSelection loadSelection)
		{
			throw new NotSupportedException("Load Network Interface is not supported on the WMI Adapter");
		}

		public bool ProcessEvent(ManagementBaseObject newEvent)
		{
			return false;
		}

		public void Collect()
		{
		}

		public List<string> GetNodeDnsSuffixes(string nodeName)
		{
			List<string> dnsSuffixes = new List<string>();
			ExecuteAndCatchWmiExceptions(delegate
			{
				ManagementScope win32WmiConnection = WmiHelper.GetWin32WmiConnection(nodeName);
				ObjectQuery query = new ObjectQuery("Select DNSDomain from Win32_NetworkAdapterConfiguration");
				using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(win32WmiConnection, query);
				using ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
				foreach (ManagementObject item in managementObjectCollection)
				{
					object obj = item["DNSDomain"];
					if (obj != null)
					{
						dnsSuffixes.Add(obj.ToString());
					}
				}
			}, nodeName);
			return dnsSuffixes;
		}
	}

	private class NodeAdapter : AdapterBase, IConnectionAdapterNode
	{
		private readonly object loadingNodesLock = new object();

		private const string NodeElementaryPayloadQuery = "nodeinstanceid,nodeName";

		private const string NodebasicPayloadQuery = ",state";

		private const string NodeCommonPropertiesQuery = ",NodeName,NodeHighestVersion,NodeLowestVersion,MajorVersion,MinorVersion,BuildNumber,CSDVersion,NodeInstanceID,Description";

		private const string NodePrivatePropertiesQuery = ",PrivateProperties";

		private const string NodeOsInformationQuery = "Select Caption, CSDVersion, FreePhysicalMemory, LastBootUpTime, LocalDateTime, TotalVisibleMemorySize, Version From Win32_OperatingSystem";

		private const string NodeServerInformationQuery = "Select Manufacturer, Model, SystemType From Win32_ComputerSystem";

		private const string NodeDomainQuery = "Select Domain From Win32_ComputerSystem";

		private const string NodeCpuInformationQuery = "Select Caption, Description, Name, LoadPercentage, NumberOfCores, MaxClockSpeed From Win32_Processor";

		public object LoadingNodesLock => loadingNodesLock;

		public string[] PropertiesName
		{
			get
			{
				List<string> list = new List<string>();
				list.AddRange(",NodeName,NodeHighestVersion,NodeLowestVersion,MajorVersion,MinorVersion,BuildNumber,CSDVersion,NodeInstanceID,Description".Split(','));
				list.AddRange(",PrivateProperties".Split(','));
				return list.ToArray();
			}
		}

		public NodeAdapter(WmiAdapter wmiAdapter)
			: base(wmiAdapter)
		{
			Init();
		}

		public void Init()
		{
		}

		public PNode Open(Guid id)
		{
			PNode pNode = null;
			using (ManagementObject managementObject = GetSingleObject(id, "mscluster_node"))
			{
				pNode = new PNode(base.WmiAdapter.clusters.Cluster, new Guid((string)managementObject["Id"]), (string)managementObject["Name"]);
				lock (LoadingNodesLock)
				{
					if (!base.WmiAdapter.MappingIdNameNode.ContainsKey(pNode.Id))
					{
						base.WmiAdapter.MappingIdNameNode.TryAdd(pNode.Id, pNode.Name);
					}
					if (!base.WmiAdapter.MappingNameIdNode.ContainsKey(pNode.Name))
					{
						base.WmiAdapter.MappingNameIdNode.TryAdd(pNode.Name, pNode.Id);
					}
				}
			}
			return pNode;
		}

		public PNode Open(string nodeName)
		{
			PNode pNode = null;
			using (ManagementObject managementObject = GetSingleObject(nodeName, "mscluster_node"))
			{
				pNode = new PNode(base.WmiAdapter.clusters.Cluster, new Guid((string)managementObject["Id"]), (string)managementObject["Name"]);
				lock (LoadingNodesLock)
				{
					if (!base.WmiAdapter.MappingIdNameNode.ContainsKey(pNode.Id))
					{
						base.WmiAdapter.MappingIdNameNode.TryAdd(pNode.Id, pNode.Name);
					}
					if (!base.WmiAdapter.MappingNameIdNode.ContainsKey(pNode.Name))
					{
						base.WmiAdapter.MappingNameIdNode.TryAdd(pNode.Name, pNode.Id);
					}
				}
			}
			return pNode;
		}

		public void Load(PNode node, NodeLoadSelection loadSelection)
		{
			if ((loadSelection & NodeLoadSelection.Basic) != NodeLoadSelection.Basic && (loadSelection & NodeLoadSelection.CommonProperties) != NodeLoadSelection.CommonProperties && (loadSelection & NodeLoadSelection.PrivateProperties) != NodeLoadSelection.PrivateProperties)
			{
				return;
			}
			using ManagementObject managementObject = GetSingleObject(node.Name, "mscluster_node");
			if (managementObject == null)
			{
				throw new ClusterObjectNotFoundException(node.Name, node.Id, typeof(PNode));
			}
			try
			{
				node.State = (NodeState)(uint)managementObject["State"];
				node.LoadedSelection |= 1;
				ParseProperties(node.Properties, managementObject.Properties, ClusterPropertyKind.Common);
				node.LoadedSelection |= 2;
				ParseProperties(node.Properties, ((ManagementBaseObject)managementObject["PrivateProperties"]).Properties, ClusterPropertyKind.Private);
				node.LoadedSelection |= 4;
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

		public bool WillOfflineLoseQuorum(string name)
		{
			throw new NotSupportedException("WillOfflineLoseQuorum is not supported by Wmi Adapter");
		}

		public bool WillEvictLoseQuorum(string name)
		{
			throw new NotSupportedException("WillOfflineLoseQuorum is not supported by Wmi Adapter");
		}

		public void Rename(Guid id, string newName)
		{
			ExecuteNodeMethod(id, "Rename", newName);
		}

		public void Start(string name)
		{
		}

		public void Stop(string name)
		{
		}

		public void Pause(string name, NodePauseDrainType drainType, string targetNode)
		{
		}

		public void Resume(string name, NodeResumeFailbackType failbackType)
		{
		}

		public void Delete(Guid id)
		{
		}

		public PNode Add(string name)
		{
			return null;
		}

		internal Guid GetNodeIdFromName(string nodeName)
		{
			if (!base.WmiAdapter.MappingNameIdNode.TryGetValue(nodeName, out var value))
			{
				lock (LoadingNodesLock)
				{
					if (!base.WmiAdapter.MappingNameIdNode.TryGetValue(nodeName, out value))
					{
						using ManagementObject managementObject = GetSingleObject(nodeName, "mscluster_node");
						value = new Guid((string)managementObject["NodeInstanceId"]);
						lock (LoadingNodesLock)
						{
							if (!base.WmiAdapter.MappingIdNameNode.ContainsKey(value))
							{
								base.WmiAdapter.MappingIdNameNode.TryAdd(value, nodeName);
							}
							if (!base.WmiAdapter.MappingNameIdNode.ContainsKey(nodeName))
							{
								base.WmiAdapter.MappingNameIdNode.TryAdd(nodeName, value);
							}
						}
					}
				}
			}
			return value;
		}

		internal string GetNodeNameFromId(Guid nodeGuid)
		{
			if (!base.WmiAdapter.MappingIdNameNode.TryGetValue(nodeGuid, out var value))
			{
				lock (LoadingNodesLock)
				{
					if (!base.WmiAdapter.MappingIdNameNode.TryGetValue(nodeGuid, out value))
					{
						using ManagementObject managementObject = GetSingleNode(nodeGuid, "mscluster_node");
						value = (string)managementObject["Name"];
						lock (LoadingNodesLock)
						{
							if (!base.WmiAdapter.MappingIdNameNode.ContainsKey(nodeGuid))
							{
								base.WmiAdapter.MappingIdNameNode.TryAdd(nodeGuid, value);
							}
							if (!base.WmiAdapter.MappingNameIdNode.ContainsKey(value))
							{
								base.WmiAdapter.MappingNameIdNode.TryAdd(value, nodeGuid);
							}
						}
					}
				}
			}
			return value;
		}

		public IEnumerable<PNode> GetAll(bool nullElementOnError)
		{
			ManagementOperationObserver managementOperationObserver = new ManagementOperationObserver();
			AutoResetEvent doneEvent = new AutoResetEvent(initialState: true);
			Exception lastError = null;
			PNode returnObject = null;
			AutoResetEvent rdyEvent = new AutoResetEvent(initialState: false);
			Action<Exception> completed = delegate(Exception mgmtException)
			{
				lastError = mgmtException;
				returnObject = null;
				rdyEvent.Set();
			};
			Action<ManagementObject> objectArrived = delegate(ManagementObject mgmtObj)
			{
				try
				{
					doneEvent.WaitOne();
					PNode pNode = new PNode(base.WmiAdapter.clusters.Cluster, new Guid((string)mgmtObj["NodeInstanceId"]), (string)mgmtObj["Name"])
					{
						State = (NodeState)(uint)mgmtObj["state"]
					};
					ParseProperties(pNode.Properties, mgmtObj.Properties, ClusterPropertyKind.Common);
					pNode.Properties.CommonPropertiesLoaded = true;
					ParseProperties(pNode.Properties, ((ManagementBaseObject)mgmtObj["PrivateProperties"]).Properties, ClusterPropertyKind.Private);
					pNode.Properties.PrivatePropertiesLoaded = true;
					pNode.LoadedSelection = 28679;
					returnObject = pNode;
					lock (LoadingNodesLock)
					{
						if (!base.WmiAdapter.MappingIdNameNode.ContainsKey(returnObject.Id))
						{
							base.WmiAdapter.MappingIdNameNode.TryAdd(returnObject.Id, returnObject.Name);
						}
						if (!base.WmiAdapter.MappingNameIdNode.ContainsKey(returnObject.Name))
						{
							base.WmiAdapter.MappingNameIdNode.TryAdd(returnObject.Name, returnObject.Id);
						}
					}
				}
				catch (Exception ex)
				{
					returnObject = null;
					lastError = ex;
				}
				finally
				{
					rdyEvent.Set();
				}
			};
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select *");
			stringBuilder.Append(" from ");
			stringBuilder.Append("mscluster_node");
			ObjectQuery query = new ObjectQuery(stringBuilder.ToString());
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(base.WmiAdapter.Scope, query);
			try
			{
				if (base.WmiAdapter.QueriesAreAsync)
				{
					managementOperationObserver.ObjectReady += delegate(object sender, ObjectReadyEventArgs e)
					{
						objectArrived((ManagementObject)e.NewObject);
					};
					managementOperationObserver.Completed += delegate(object sender, CompletedEventArgs e)
					{
						ManagementException obj2 = null;
						if (e.Status != 0)
						{
							obj2 = new ManagementException("Generic Failure");
							FieldInfo field = typeof(ManagementException).GetField("errorCode", BindingFlags.Instance | BindingFlags.NonPublic);
							FieldInfo field2 = typeof(ManagementException).GetField("errorObject", BindingFlags.Instance | BindingFlags.NonPublic);
							field.SetValue(obj2, e.Status);
							field2.SetValue(obj2, e.StatusObject);
						}
						completed(obj2);
					};
					searcher.Get(managementOperationObserver);
				}
				else
				{
					Worker.Start(delegate
					{
						searcher.Options = new EnumerationOptions
						{
							ReturnImmediately = true,
							Rewindable = false,
							BlockSize = 10000
						};
						using (ManagementObjectCollection managementObjectCollection = searcher.Get())
						{
							foreach (ManagementObject item in managementObjectCollection)
							{
								objectArrived(item);
							}
						}
						completed(null);
					}, completed);
				}
				while (true)
				{
					rdyEvent.WaitOne();
					if (returnObject == null)
					{
						if (lastError == null)
						{
							break;
						}
						if (!nullElementOnError)
						{
							throw lastError;
						}
						ClusterLog.LogException(lastError, "An error has occurred on the node enumeration, however is not critical and the process can continue");
					}
					yield return returnObject;
					doneEvent.Set();
				}
			}
			finally
			{
				if (searcher != null)
				{
					((IDisposable)searcher).Dispose();
				}
			}
		}

		public NodeOperatingSystemInformation GetOperatingSystemInformation(string nodeName)
		{
			NodeOperatingSystemInformation operatingSystemInformation = new NodeOperatingSystemInformation();
			ExecuteAndCatchWmiExceptions(delegate
			{
				ManagementScope win32WmiConnection = WmiHelper.GetWin32WmiConnection(nodeName);
				ObjectQuery query = new ObjectQuery("Select Caption, CSDVersion, FreePhysicalMemory, LastBootUpTime, LocalDateTime, TotalVisibleMemorySize, Version From Win32_OperatingSystem");
				using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(win32WmiConnection, query);
				using ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
				foreach (ManagementObject item in managementObjectCollection)
				{
					ulong? num = (ulong?)item["FreePhysicalMemory"];
					if (num.HasValue)
					{
						operatingSystemInformation.Available = num.Value;
					}
					num = (ulong?)item["TotalVisibleMemorySize"];
					if (num.HasValue)
					{
						operatingSystemInformation.Total = num.Value;
					}
					object obj2 = item["Caption"];
					if (obj2 != null)
					{
						operatingSystemInformation.OsName = obj2.ToString();
					}
					obj2 = item["CSDVersion"];
					if (obj2 != null)
					{
						operatingSystemInformation.CsdVersion = obj2.ToString();
					}
					else
					{
						operatingSystemInformation.CsdVersion = CommonResources.NoServicePack_Text;
					}
					obj2 = item["Version"];
					if (obj2 != null)
					{
						operatingSystemInformation.OsVersion = obj2.ToString();
					}
					obj2 = item["LastBootUpTime"];
					if (obj2 != null)
					{
						operatingSystemInformation.LastBootUptime = ManagementDateTimeConverter.ToDateTime(obj2.ToString());
					}
					obj2 = item["LocalDateTime"];
					if (obj2 != null)
					{
						operatingSystemInformation.LocalDateTime = ManagementDateTimeConverter.ToDateTime(obj2.ToString());
					}
				}
			}, nodeName);
			return operatingSystemInformation;
		}

		public string GetDomainName(string nodeName)
		{
			return ExecuteAndCatchWmiExceptions(delegate
			{
				ManagementScope win32WmiConnection = WmiHelper.GetWin32WmiConnection(nodeName);
				ObjectQuery query = new ObjectQuery("Select Domain From Win32_ComputerSystem");
				using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(win32WmiConnection, query);
				using ManagementObjectCollection source = managementObjectSearcher.Get();
				return (from ManagementObject _ in source
					select (_["Domain"] == null) ? string.Empty : _["Domain"].ToString()).First();
			}, nodeName);
		}

		public ServerInformation GetServerInformation(string nodeName)
		{
			ServerInformation serverInformation = new ServerInformation();
			ExecuteAndCatchWmiExceptions(delegate
			{
				ManagementScope win32WmiConnection = WmiHelper.GetWin32WmiConnection(nodeName);
				ObjectQuery query = new ObjectQuery("Select Manufacturer, Model, SystemType From Win32_ComputerSystem");
				using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(win32WmiConnection, query);
				using ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
				foreach (ManagementObject item in managementObjectCollection)
				{
					object obj2 = item["Manufacturer"];
					if (obj2 != null)
					{
						serverInformation.Manufacturer = obj2.ToString();
					}
					obj2 = item["Model"];
					if (obj2 != null)
					{
						serverInformation.Model = obj2.ToString();
					}
					obj2 = item["SystemType"];
					if (obj2 != null)
					{
						serverInformation.SystemType = obj2.ToString();
					}
				}
			}, nodeName);
			return serverInformation;
		}

		public ProcessorInformation GetProcessorInformation(string nodeName)
		{
			ProcessorInformation processorInformation = new ProcessorInformation();
			ExecuteAndCatchWmiExceptions(delegate
			{
				ManagementScope win32WmiConnection = WmiHelper.GetWin32WmiConnection(nodeName);
				ObjectQuery query = new ObjectQuery("Select Caption, Description, Name, LoadPercentage, NumberOfCores, MaxClockSpeed From Win32_Processor");
				using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(win32WmiConnection, query);
				using ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
				processorInformation.LoadPercentage = 0;
				processorInformation.NumberOfCores = 0u;
				foreach (ManagementObject item in managementObjectCollection)
				{
					object obj2 = item["Caption"];
					if (obj2 != null)
					{
						processorInformation.Caption = obj2.ToString();
					}
					obj2 = item["Description"];
					if (obj2 != null)
					{
						processorInformation.Description = obj2.ToString();
					}
					obj2 = item["Name"];
					if (obj2 != null)
					{
						processorInformation.Name = obj2.ToString();
					}
					ushort? num = (ushort?)item["LoadPercentage"];
					if (num.HasValue)
					{
						processorInformation.LoadPercentage += num.Value;
					}
					uint? num2 = (uint?)item["MaxClockSpeed"];
					if (num2.HasValue)
					{
						processorInformation.MaxClockSpeed = num2.Value;
					}
					num2 = (uint?)item["NumberOfCores"];
					if (num2.HasValue)
					{
						processorInformation.NumberOfCores += num2.Value;
					}
					processorInformation.IsLoaded = true;
				}
				processorInformation.LoadPercentage = (ushort)((float)(int)processorInformation.LoadPercentage / (float)managementObjectCollection.Count);
			}, nodeName);
			return processorInformation;
		}

		internal List<string> NormalizeQuery(List<string> fieldsString, out NodeLoadSelection loadSelection)
		{
			loadSelection = NodeLoadSelection.None;
			List<string> list = new List<string>();
			foreach (string item in fieldsString)
			{
				if (",state".Contains("," + item.ToLowerInvariant()))
				{
					loadSelection |= NodeLoadSelection.Basic;
				}
			}
			list.AddRange("nodeinstanceid,nodeName".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			if ((loadSelection & NodeLoadSelection.Basic) == NodeLoadSelection.Basic)
			{
				list.AddRange(",state".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			}
			return list;
		}

		internal TResult ProcessResult<TResult>(ManagementObject mgmtObj, QueryInfo queryInfo, NodeLoadSelection loadSelection) where TResult : PClusterObject
		{
			PNode pNode = new PNode(base.WmiAdapter.clusters.Cluster, new Guid((string)mgmtObj["NodeInstanceId"]), (string)mgmtObj["Name"]);
			if ((loadSelection & NodeLoadSelection.Basic) == NodeLoadSelection.Basic)
			{
				pNode.State = (NodeState)(uint)mgmtObj["state"];
				pNode.LoadedSelection |= (int)loadSelection;
			}
			return (TResult)(PClusterObject)pNode;
		}

		private ManagementObject GetSingleNode(Guid guid, string table)
		{
			return ExecuteAndCatchWmiExceptions(delegate
			{
				using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(base.WmiAdapter.Scope, new ObjectQuery("select * from {0} where nodeInstanceId='{1}'".FormatInvariantCulture(table, guid.ToString())));
				managementObjectSearcher.Options = new EnumerationOptions
				{
					ReturnImmediately = true,
					Rewindable = false,
					BlockSize = 1
				};
				return managementObjectSearcher.Get().Cast<ManagementObject>().FirstOrDefault();
			}, guid.ToString());
		}

		private ManagementObject GetSingleNode(Guid guid, string table, string fields)
		{
			return ExecuteAndCatchWmiExceptions(delegate
			{
				using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(base.WmiAdapter.Scope, new ObjectQuery("select {0} from {1} where nodeInstanceId='{2}'".FormatInvariantCulture(fields, table, guid.ToString())));
				managementObjectSearcher.Options = new EnumerationOptions
				{
					ReturnImmediately = true,
					Rewindable = false,
					BlockSize = 1
				};
				return managementObjectSearcher.Get().Cast<ManagementObject>().FirstOrDefault();
			}, guid.ToString());
		}

		private ManagementObject GetSingleNode(string name, string table)
		{
			return ExecuteAndCatchWmiExceptions(delegate
			{
				using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(base.WmiAdapter.Scope, new ObjectQuery("select * from {0} where name='{1}'".FormatInvariantCulture(table, name)));
				managementObjectSearcher.Options = new EnumerationOptions
				{
					ReturnImmediately = true,
					Rewindable = false,
					BlockSize = 1
				};
				return managementObjectSearcher.Get().Cast<ManagementObject>().FirstOrDefault();
			}, name);
		}

		private void ExecuteNodeMethod(Guid id, string methodName, object parameter)
		{
			ExecuteNodeMethod(id, methodName, new object[1] { parameter });
		}

		private void ExecuteNodeMethod(Guid id, string methodName, object[] parameters)
		{
			ObjectGetOptions op = new ObjectGetOptions();
			ExecuteAndCatchWmiExceptions(delegate
			{
				string text = null;
				lock (LoadingNodesLock)
				{
					text = base.WmiAdapter.MappingIdNameNode[id];
				}
				using ManagementObject managementObject = new ManagementObject(base.WmiAdapter.Scope, new ManagementPath("MSCluster_Node.Name=\"" + text + "\""), op);
				managementObject.InvokeMethod(methodName, parameters);
			}, id.ToString());
		}

		public bool ProcessEvent(ManagementBaseObject newEvent)
		{
			switch (newEvent.ClassPath.ClassName)
			{
			case "MSCluster_EventNodeStateChanged":
			{
				Guid id = new Guid((string)newEvent["EventObjectId"]);
				NodeState value4 = (NodeState)(uint)newEvent["EventNewState"];
				base.WmiAdapter.EnqueueNotification(new NodeNotification(new ClusterNodeStateEventArgs(id, value4, null)));
				return true;
			}
			case "MSCluster_EventNodeAdded":
			{
				string text2 = (string)newEvent["EventObjectName"];
				Guid guid = new Guid((string)newEvent["EventObjectId"]);
				lock (LoadingNodesLock)
				{
					if (!base.WmiAdapter.MappingIdNameNode.ContainsKey(guid))
					{
						base.WmiAdapter.MappingIdNameNode.TryAdd(guid, text2);
					}
					if (!base.WmiAdapter.MappingNameIdNode.ContainsKey(text2))
					{
						base.WmiAdapter.MappingNameIdNode.TryAdd(text2, guid);
					}
				}
				ClusterAddedEventArgs clusterAddedEventArgs = new ClusterAddedEventArgs(guid, text2, null, null);
				clusterAddedEventArgs.Cluster = base.WmiAdapter.clusters.Cluster;
				base.WmiAdapter.EnqueueNotification(new NodeNotification(clusterAddedEventArgs));
				return true;
			}
			case "MSCluster_EventNodeRemoved":
			{
				string text = (string)newEvent["EventObjectName"];
				Guid value = Guid.Empty;
				lock (LoadingNodesLock)
				{
					if (!base.WmiAdapter.MappingNameIdNode.TryGetValue(text, out value))
					{
						return true;
					}
					base.WmiAdapter.MappingIdNameNode.TryRemove(value, out var _);
					base.WmiAdapter.MappingNameIdNode.TryRemove(text, out var _);
				}
				ClusterRemovedEventArgs clusterRemovedEventArgs = new ClusterRemovedEventArgs(value, text, null);
				clusterRemovedEventArgs.Cluster = base.WmiAdapter.clusters.Cluster;
				base.WmiAdapter.EnqueueNotification(new NodeNotification(clusterRemovedEventArgs));
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

	private class ResourceAdapter : AdapterBase, IConnectionAdapterResource
	{
		private readonly object loadingResourcesLock = new object();

		private const string ResourceElementaryPayloadQuery = "id,name,type";

		private const string ResourceBasicPayloadQuery = ",state,ownernode,ownergroup,characteristics,resourceclass,flags,isClusterSharedVolume";

		private const string ResourceCommonPropertiesQuery = ",DeadlockTimeout,Description,IsAlivePollInterval,LooksAlivePollInterval,MonitorProcessId,Name,PendingTimeout,PersistentState,ResourceSpecificStatus,RestartAction, RestartDelay,RestartPeriod,RestartThreshold,RetryPeriodOnFailure,SeparateMonitor,ObjectType";

		private const string ResourcePrivatePropertiesQuery = ",PrivateProperties";

		private const string VirtualMachineOwnerGroupQuery = "select OwnerGroup from mscluster_resource where type='Virtual Machine' and PrivateProperties.vmid='{0}'";

		public string[] PropertiesName
		{
			get
			{
				List<string> list = new List<string>();
				list.AddRange(",DeadlockTimeout,Description,IsAlivePollInterval,LooksAlivePollInterval,MonitorProcessId,Name,PendingTimeout,PersistentState,ResourceSpecificStatus,RestartAction, RestartDelay,RestartPeriod,RestartThreshold,RetryPeriodOnFailure,SeparateMonitor,ObjectType".Split(','));
				list.AddRange(",PrivateProperties".Split(','));
				return list.ToArray();
			}
		}

		public ResourceAdapter(WmiAdapter wmiAdapter)
			: base(wmiAdapter)
		{
			Init();
		}

		public void Init()
		{
		}

		public void AddDependency(Guid id, Guid dependencyId)
		{
			throw new NotSupportedException("AddDependency is not supported by Wmi Adapter");
		}

		public void RemoveDependency(Guid resourceId, Guid dependOnResourceId)
		{
			throw new NotSupportedException("RemoveDependency is not supported by Wmi Adapter");
		}

		private List<Guid> GetPossibleOwners(Guid id)
		{
			object[] parameters = new object[1];
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceMethod(id, "GetPossibleOwners", ref parameters);
			}, id.ToString());
			return new List<Guid>(((Array)parameters[0]).ConvertAll((string nodeName) => base.WmiAdapter.nodes.GetNodeIdFromName(nodeName)));
		}

		public void AddRegistryCheckpoint(Guid id, string checkpoint)
		{
			throw new NotSupportedException("AddRegistryCheckpoint is not supported by WMI Adapter");
		}

		public void RemoveRegistryCheckpoint(Guid id, string checkpoint)
		{
			throw new NotSupportedException("RemoveRegistryCheckpoint is not supported by WMI Adapter");
		}

		public IEnumerable<string> GetRegistryCheckpoints(Guid id)
		{
			throw new NotSupportedException("GetRegistryCheckpoints is not supported by WMI Adapter");
		}

		public void AddCryptoCheckpoint(Guid id, string checkpoint)
		{
			throw new NotSupportedException("AddCryptoCheckpoint is not supported by WMI Adapter");
		}

		public void RemoveCryptoCheckpoint(Guid id, string checkpoint)
		{
			throw new NotSupportedException("RemoveCryptoCheckpoint is not supported by WMI Adapter");
		}

		public IEnumerable<string> GetCryptoCheckpoints(Guid id)
		{
			throw new NotSupportedException("GetCryptoCheckpoints is not supported by WMI Adapter");
		}

		public void AddPossibleOwner(Guid id, string node)
		{
			throw new NotSupportedException("AddPossibleOwners is not supported by WMI Adapter");
		}

		public void RemovePossibleOwner(Guid id, string node)
		{
			throw new NotSupportedException("RemovePossibleOwners is not supported by WMI Adapter");
		}

		public void SetPossibleOwners(Guid id, IEnumerable<Guid> nodes)
		{
			List<Guid> list = new List<Guid>(nodes);
			List<Guid> possibleOwners = GetPossibleOwners(id);
			foreach (Guid currentNode in possibleOwners)
			{
				if (!list.Exists((Guid item) => item == currentNode))
				{
					object[] parameters2 = new object[1];
					parameters2[0] = base.WmiAdapter.nodes.GetNodeNameFromId(currentNode);
					ExecuteAndCatchWmiExceptions(delegate
					{
						ExecuteResourceMethod(id, "RemovePossibleOwner", ref parameters2);
					}, id.ToString());
				}
			}
			foreach (Guid newNode in list)
			{
				if (!possibleOwners.Exists((Guid item) => item == newNode))
				{
					object[] parameters = new object[1];
					parameters[0] = base.WmiAdapter.nodes.GetNodeNameFromId(newNode);
					ExecuteAndCatchWmiExceptions(delegate
					{
						ExecuteResourceMethod(id, "AddPossibleOwner", ref parameters);
					}, id.ToString());
				}
			}
		}

		public PResource Open(Guid id)
		{
			PResource pResource = null;
			using (ManagementObject managementObject = GetSingleObject(id, "mscluster_resource"))
			{
				string resourceTypeName = (string)managementObject["type"];
				PResourceType resourceType = new PResourceType(base.WmiAdapter.clusters.Cluster, resourceTypeName);
				pResource = PResource.Constructor(base.WmiAdapter.clusters.Cluster, new Guid((string)managementObject["Id"]), (string)managementObject["Name"], resourceType);
				lock (loadingResourcesLock)
				{
					if (!base.WmiAdapter.MappingIdNameResource.ContainsKey(pResource.Id))
					{
						base.WmiAdapter.MappingIdNameResource.TryAdd(pResource.Id, pResource.Name);
					}
					if (!base.WmiAdapter.MappingNameIdResource.ContainsKey(pResource.Name))
					{
						base.WmiAdapter.MappingNameIdResource.TryAdd(pResource.Name, pResource.Id);
					}
				}
			}
			return pResource;
		}

		public PResource Open(string resourceName)
		{
			PResource pResource = null;
			using (ManagementObject managementObject = GetSingleObject(resourceName, "mscluster_resource"))
			{
				string resourceTypeName = (string)managementObject["type"];
				PResourceType resourceType = new PResourceType(base.WmiAdapter.clusters.Cluster, resourceTypeName);
				pResource = PResource.Constructor(base.WmiAdapter.clusters.Cluster, new Guid((string)managementObject["Id"]), (string)managementObject["Name"], resourceType);
				lock (loadingResourcesLock)
				{
					if (!base.WmiAdapter.MappingIdNameResource.ContainsKey(pResource.Id))
					{
						base.WmiAdapter.MappingIdNameResource.TryAdd(pResource.Id, pResource.Name);
					}
					if (!base.WmiAdapter.MappingNameIdResource.ContainsKey(pResource.Name))
					{
						base.WmiAdapter.MappingNameIdResource.TryAdd(pResource.Name, pResource.Id);
					}
				}
			}
			return pResource;
		}

		public void Rename(Guid id, string newName)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceMethod(id, "Rename", newName);
			}, newName);
		}

		public void Load(PResource privateResource, ResourceLoadSelection loadSelection)
		{
			try
			{
				if ((loadSelection & ResourceLoadSelection.Basic) == ResourceLoadSelection.Basic || (loadSelection & ResourceLoadSelection.CommonProperties) == ResourceLoadSelection.CommonProperties || (loadSelection & ResourceLoadSelection.PrivateProperties) == ResourceLoadSelection.PrivateProperties || (loadSelection & ResourceLoadSelection.RequiredDependencies) == ResourceLoadSelection.RequiredDependencies)
				{
					using ManagementObject managementObject = GetSingleObject(privateResource.Id, "mscluster_resource");
					if (managementObject == null)
					{
						throw new ClusterObjectNotFoundException(privateResource.Name, privateResource.Id, typeof(PResource));
					}
					if ((loadSelection & ResourceLoadSelection.Basic) == ResourceLoadSelection.Basic || (loadSelection & ResourceLoadSelection.CommonProperties) == ResourceLoadSelection.CommonProperties || (loadSelection & ResourceLoadSelection.PrivateProperties) == ResourceLoadSelection.PrivateProperties)
					{
						privateResource.Characteristics = (Characteristics)(uint)managementObject["Characteristics"];
						privateResource.Class = (ResourceClass)(uint)managementObject["ResourceClass"];
						privateResource.Flags = (ResourceFlags)(uint)managementObject["Flags"];
						string text = (string)managementObject["OwnerGroup"];
						PGroup ownerGroup = PGroup.Constructor(privateResource.Cluster, base.WmiAdapter.groups.GetGroupIdFromName(text), text, GroupType.Unknown);
						privateResource.OwnerGroup = ownerGroup;
						privateResource.LoadedSelection |= 1;
						ParseProperties(privateResource.Properties, managementObject.Properties, ClusterPropertyKind.Common);
						privateResource.LoadedSelection |= 2;
						string name = (string)managementObject["type"];
						if (privateResource.ResourceType.ResourceKind == ResourceKind.Other && privateResource.ResourceType.Name == null)
						{
							privateResource.ResourceType.Name = name;
						}
						ParseProperties(privateResource.Properties, ((ManagementBaseObject)managementObject["PrivateProperties"]).Properties, ClusterPropertyKind.Private);
						privateResource.LoadedSelection |= 4;
						privateResource.ResourceState = (ResourceState)(uint)managementObject["state"];
					}
					if ((loadSelection & ResourceLoadSelection.RequiredDependencies) == ResourceLoadSelection.RequiredDependencies)
					{
						privateResource.RequiredDependencies = GetRequiredDependencies(managementObject);
						privateResource.LoadedSelection |= 64;
					}
				}
				if ((loadSelection & ResourceLoadSelection.Dependencies) == ResourceLoadSelection.Dependencies)
				{
					privateResource.Dependencies = Dependencies(privateResource.Name);
					privateResource.LoadedSelection |= 8;
				}
				if ((loadSelection & ResourceLoadSelection.DependenciesRelation) == ResourceLoadSelection.DependenciesRelation)
				{
					privateResource.DependencyRelationship = GetDependencyRelationship(privateResource.Id);
					privateResource.LoadedSelection |= 32;
				}
				if ((loadSelection & ResourceLoadSelection.Dependents) == ResourceLoadSelection.Dependents)
				{
					privateResource.Dependents = Dependents(privateResource.Name);
					privateResource.LoadedSelection |= 16;
				}
				if ((loadSelection & ResourceLoadSelection.PossibleOwners) == ResourceLoadSelection.PossibleOwners)
				{
					privateResource.PossibleOwners = GetPossibleOwners(privateResource.Id);
					privateResource.LoadedSelection |= 128;
				}
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(privateResource.Name, privateResource.Id, innerException);
			}
		}

		private RequiredDependencies GetRequiredDependencies(ManagementObject mgmtObj)
		{
			List<ResourceClass> list = null;
			List<string> list2 = null;
			uint[] array = (uint[])mgmtObj["RequiredDependencyClasses"];
			list = ((array != null) ? ((IList)array).ConvertAll((Converter<object, ResourceClass>)((object resourceClass) => (ResourceClass)(uint)resourceClass)) : new List<ResourceClass>());
			string[] array2 = (string[])mgmtObj["RequiredDependencyTypes"];
			list2 = ((array2 != null) ? new List<string>(array2) : new List<string>());
			return new RequiredDependencies(list, list2);
		}

		public void Online(Guid id, bool overrideLockState = false, bool chooseBestNode = false)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceMethod(id, "BringOnline", 0);
			}, id.ToString());
		}

		public void Offline(Guid id, bool overrideLockState = false)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceMethod(id, "TakeOffline", 0);
			}, id.ToString());
		}

		public IEnumerable<string> GetPossibleOwners(string name)
		{
			throw new NotSupportedException("GetPossibleOwners is not supported by WMI adapter");
		}

		public void OfflineDependents(Guid id, bool overrideLockState = false)
		{
		}

		public string GetType(Guid id, string name)
		{
			throw new NotImplementedException();
		}

		public void Fail(Guid id)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceMethod(id, "FailResource", 0);
			}, id.ToString());
		}

		public List<Guid> Dependents(string resourceName)
		{
			return DependentsDependencies(resourceName, dependants: true);
		}

		public List<Guid> Dependencies(string resourceName)
		{
			return DependentsDependencies(resourceName, dependants: false);
		}

		public List<Guid> DependentsDependencies(string resourceName, bool dependants)
		{
			List<Guid> list = new List<Guid>();
			ManagementOperationObserver managementOperationObserver = new ManagementOperationObserver();
			AutoResetEvent doneEvent = new AutoResetEvent(initialState: true);
			Exception lastError = null;
			Guid returnObject = Guid.Empty;
			AutoResetEvent rdyEvent = new AutoResetEvent(initialState: false);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select * from MSCluster_ResourceToDependentResource where ");
			if (dependants)
			{
				stringBuilder.Append("Dependent");
			}
			else
			{
				stringBuilder.Append("Antecedent");
			}
			stringBuilder.Append("=\"");
			stringBuilder.Append("mscluster_resource");
			stringBuilder.Append(".Name='");
			stringBuilder.Append(resourceName);
			stringBuilder.Append("'\"");
			ObjectQuery query = new ObjectQuery(stringBuilder.ToString());
			Action<Exception> completed = delegate(Exception mgmtException)
			{
				lastError = mgmtException;
				returnObject = Guid.Empty;
				rdyEvent.Set();
			};
			Action<ManagementObject> objectArrived = delegate(ManagementObject mgmtObj)
			{
				try
				{
					doneEvent.WaitOne();
					string text = null;
					text = ((!dependants) ? ((string)mgmtObj["Dependent"]) : ((string)mgmtObj["Antecedent"]));
					using ManagementObject managementObject = GetSingleObject(new ManagementPath(text));
					Guid guid = new Guid((string)managementObject["Id"]);
					string text2 = (string)managementObject["Name"];
					returnObject = guid;
					lock (loadingResourcesLock)
					{
						if (!base.WmiAdapter.MappingIdNameResource.ContainsKey(guid))
						{
							base.WmiAdapter.MappingIdNameResource.TryAdd(guid, text2);
						}
						if (!base.WmiAdapter.MappingNameIdResource.ContainsKey(text2))
						{
							base.WmiAdapter.MappingNameIdResource.TryAdd(text2, guid);
						}
					}
				}
				catch (Exception ex)
				{
					returnObject = Guid.Empty;
					lastError = ex;
				}
				finally
				{
					rdyEvent.Set();
				}
			};
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(base.WmiAdapter.Scope, query);
			try
			{
				if (base.WmiAdapter.QueriesAreAsync)
				{
					managementOperationObserver.ObjectReady += delegate(object sender, ObjectReadyEventArgs e)
					{
						objectArrived((ManagementObject)e.NewObject);
					};
					managementOperationObserver.Completed += delegate(object sender, CompletedEventArgs e)
					{
						ManagementException obj2 = null;
						if (e.Status != 0)
						{
							obj2 = new ManagementException("Generic Failure");
							FieldInfo field = typeof(ManagementException).GetField("errorCode", BindingFlags.Instance | BindingFlags.NonPublic);
							FieldInfo field2 = typeof(ManagementException).GetField("errorObject", BindingFlags.Instance | BindingFlags.NonPublic);
							field.SetValue(obj2, e.Status);
							field2.SetValue(obj2, e.StatusObject);
						}
						completed(obj2);
					};
					searcher.Get(managementOperationObserver);
				}
				else
				{
					Worker.Start(delegate
					{
						searcher.Options = new EnumerationOptions
						{
							ReturnImmediately = true,
							Rewindable = false,
							BlockSize = 10000
						};
						using (ManagementObjectCollection managementObjectCollection = searcher.Get())
						{
							foreach (ManagementObject item in managementObjectCollection)
							{
								objectArrived(item);
							}
						}
						completed(null);
					}, completed);
				}
				while (true)
				{
					rdyEvent.WaitOne();
					if (returnObject == Guid.Empty)
					{
						break;
					}
					list.Add(returnObject);
					doneEvent.Set();
				}
				if (lastError != null)
				{
					throw lastError;
				}
				return list;
			}
			finally
			{
				if (searcher != null)
				{
					((IDisposable)searcher).Dispose();
				}
			}
		}

		public string GetDependencyRelationship(ManagementObject mgmtObj)
		{
			object[] retValue = new object[2];
			retValue[1] = true;
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceMethod(mgmtObj, "GetDependencies", ref retValue);
			}, (string)mgmtObj["Name"]);
			return (string)retValue[0];
		}

		public string GetDependencyRelationship(Guid id)
		{
			object[] retValue = new object[2];
			retValue[1] = true;
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceMethod(id, "GetDependencies", ref retValue);
			}, id.ToString());
			return (string)retValue[0];
		}

		public void SetDependencyRelationship(Guid id, string relationship)
		{
			object[] retValue = new object[1];
			retValue[0] = relationship;
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceMethod(id, "SetDependencies", ref retValue);
			}, id.ToString());
		}

		public void FetchVirtualPropertiesPayload(ClusterPropertiesEventArgs propertiesPayload)
		{
		}

		internal List<string> NormalizeQuery(List<string> fieldsString, out ResourceLoadSelection loadSelection)
		{
			loadSelection = ResourceLoadSelection.None;
			List<string> list = new List<string>();
			foreach (string item in fieldsString)
			{
				if (",state,ownernode,ownergroup,characteristics,resourceclass,flags,isClusterSharedVolume".Contains("," + item.ToLowerInvariant()))
				{
					loadSelection |= ResourceLoadSelection.Basic;
				}
				if (",DeadlockTimeout,Description,IsAlivePollInterval,LooksAlivePollInterval,MonitorProcessId,Name,PendingTimeout,PersistentState,ResourceSpecificStatus,RestartAction, RestartDelay,RestartPeriod,RestartThreshold,RetryPeriodOnFailure,SeparateMonitor,ObjectType".Contains("," + item.ToLowerInvariant()))
				{
					loadSelection |= ResourceLoadSelection.CommonProperties;
				}
				if (item.Equals("privateproperties"))
				{
					loadSelection |= ResourceLoadSelection.PrivateProperties;
				}
			}
			list.AddRange("id,name,type".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			if ((loadSelection & ResourceLoadSelection.Basic) == ResourceLoadSelection.Basic)
			{
				list.AddRange(",state,ownernode,ownergroup,characteristics,resourceclass,flags,isClusterSharedVolume".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			}
			if ((loadSelection & ResourceLoadSelection.CommonProperties) == ResourceLoadSelection.CommonProperties)
			{
				list.AddRange(",DeadlockTimeout,Description,IsAlivePollInterval,LooksAlivePollInterval,MonitorProcessId,Name,PendingTimeout,PersistentState,ResourceSpecificStatus,RestartAction, RestartDelay,RestartPeriod,RestartThreshold,RetryPeriodOnFailure,SeparateMonitor,ObjectType".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			}
			if ((loadSelection & ResourceLoadSelection.PrivateProperties) == ResourceLoadSelection.PrivateProperties)
			{
				list.AddRange(",PrivateProperties".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			}
			return list;
		}

		internal TResult ProcessResult<TResult>(ManagementObject mgmtObj, QueryInfo queryInfo, ResourceLoadSelection loadSelection) where TResult : PClusterObject
		{
			string resourceTypeName = (string)mgmtObj["type"];
			bool flag = false;
			if ((loadSelection & ResourceLoadSelection.Basic) == ResourceLoadSelection.Basic)
			{
				flag = (bool)mgmtObj["IsClusterSharedVolume"];
			}
			PResourceType pResourceType = new PResourceType(base.WmiAdapter.clusters.Cluster, resourceTypeName);
			if (flag)
			{
				pResourceType = new PResourceType(base.WmiAdapter.clusters.Cluster, ResourceKind.ClusterFileSystem, pResourceType);
			}
			PResource pResource = PResource.Constructor(base.WmiAdapter.clusters.Cluster, new Guid((string)mgmtObj["Id"]), (string)mgmtObj["Name"], pResourceType);
			if ((loadSelection & ResourceLoadSelection.Basic) == ResourceLoadSelection.Basic)
			{
				pResource.Characteristics = (Characteristics)(uint)mgmtObj["Characteristics"];
				pResource.Class = (ResourceClass)(uint)mgmtObj["ResourceClass"];
				pResource.Flags = (ResourceFlags)(uint)mgmtObj["Flags"];
				string text = (string)mgmtObj["OwnerGroup"];
				PGroup ownerGroup = PGroup.Constructor(pResource.Cluster, base.WmiAdapter.groups.GetGroupIdFromName(text), text, GroupType.Unknown);
				pResource.OwnerGroup = ownerGroup;
				pResource.ResourceState = (ResourceState)(uint)mgmtObj["state"];
			}
			if ((loadSelection & ResourceLoadSelection.CommonProperties) == ResourceLoadSelection.CommonProperties)
			{
				ParseProperties(pResource.Properties, mgmtObj.Properties, ClusterPropertyKind.Common);
			}
			if ((loadSelection & ResourceLoadSelection.PrivateProperties) == ResourceLoadSelection.PrivateProperties)
			{
				ParseProperties(pResource.Properties, ((ManagementBaseObject)mgmtObj["PrivateProperties"]).Properties, ClusterPropertyKind.Private);
			}
			if ((loadSelection & ResourceLoadSelection.Dependencies) == ResourceLoadSelection.Dependencies)
			{
				pResource.Dependencies = Dependencies(pResource.Name);
				pResource.LoadedSelection |= 8;
			}
			if ((loadSelection & ResourceLoadSelection.DependenciesRelation) == ResourceLoadSelection.DependenciesRelation)
			{
				pResource.DependencyRelationship = GetDependencyRelationship(pResource.Id);
				pResource.LoadedSelection |= 32;
			}
			if ((loadSelection & ResourceLoadSelection.Dependents) == ResourceLoadSelection.Dependents)
			{
				pResource.Dependents = Dependents(pResource.Name);
				pResource.LoadedSelection |= 16;
			}
			if ((loadSelection & ResourceLoadSelection.RequiredDependencies) == ResourceLoadSelection.RequiredDependencies)
			{
				pResource.RequiredDependencies = GetRequiredDependencies(mgmtObj);
				pResource.LoadedSelection |= 64;
			}
			if ((loadSelection & ResourceLoadSelection.PossibleOwners) == ResourceLoadSelection.PossibleOwners)
			{
				pResource.PossibleOwners = GetPossibleOwners(pResource.Id);
				pResource.LoadedSelection |= 128;
			}
			return (TResult)(PClusterObject)pResource;
		}

		private void ExecuteResourceMethod(Guid id, string methodName, object parameter)
		{
			object[] parameters = new object[1] { parameter };
			ExecuteResourceMethod(id, methodName, ref parameters);
		}

		private void ExecuteResourceMethod(ManagementObject mgmtObj, string methodName, object parameter)
		{
			object[] parameters = new object[1] { parameter };
			ExecuteResourceMethod(mgmtObj, methodName, ref parameters);
		}

		public void MoveToGroup(Guid resourceId, Guid groupId)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceMethod(resourceId, "MoveToNewGroup", groupId.ToString());
			}, groupId.ToString());
		}

		private void ExecuteResourceMethod(Guid id, string methodName, ref object[] parameters)
		{
			ObjectGetOptions options = new ObjectGetOptions();
			ManagementObject managementObject = null;
			try
			{
				if (id == Guid.Empty)
				{
					managementObject = new ManagementClass(base.WmiAdapter.Scope, new ManagementPath("mscluster_resource"), options);
				}
				else
				{
					string text = id.ToString();
					managementObject = GetSingleObject(base.WmiAdapter.Scope, "MSCluster_Resource.Name=\"" + text + "\"");
				}
				managementObject.InvokeMethod(methodName, parameters);
			}
			finally
			{
				managementObject?.Dispose();
			}
		}

		private void ExecuteResourceMethod(ManagementObject mgmtObj, string methodName, ref object[] parameters)
		{
			mgmtObj.InvokeMethod(methodName, parameters);
		}

		public bool ProcessEvent(ManagementBaseObject newEvent)
		{
			switch (newEvent.ClassPath.ClassName)
			{
			case "MSCluster_EventResourceOwnerGroupChanged":
			{
				_ = (string)newEvent["EventObjectName"];
				string text6 = (string)newEvent["EventObjectId"];
				if (text6 == null)
				{
					return true;
				}
				Guid id2 = new Guid(text6);
				string key = (string)newEvent["EventOwnerGroup"];
				Guid value8 = Guid.Empty;
				lock (base.WmiAdapter.groups.LoadingGroupLock)
				{
					if (!base.WmiAdapter.MappingNameIdGroup.TryGetValue(key, out value8))
					{
						return true;
					}
				}
				base.WmiAdapter.EnqueueNotification(new ResourceNotification(new ClusterResourceOwnerGroupEventArgs(id2, value8, null)));
				return true;
			}
			case "MSCluster_EventResourceStateChanged":
			{
				_ = (string)newEvent["EventObjectName"];
				string text2 = (string)newEvent["EventObjectId"];
				if (text2 == null)
				{
					return true;
				}
				Guid id = new Guid(text2);
				ResourceState value = (ResourceState)(uint)newEvent["EventNewState"];
				base.WmiAdapter.EnqueueNotification(new ResourceNotification(new ClusterResourceStateEventArgs(id, value, null)));
				return true;
			}
			case "MSCluster_EventResourceRenamed":
			{
				string text3 = (string)newEvent["EventObjectName"];
				Guid guid2 = Guid.Empty;
				lock (loadingResourcesLock)
				{
					string text4 = (string)newEvent["EventObjectId"];
					if (text4 == null)
					{
						return true;
					}
					guid2 = new Guid(text4);
					if (!base.WmiAdapter.MappingIdNameResource.TryGetValue(guid2, out var value2))
					{
						return true;
					}
					base.WmiAdapter.MappingIdNameResource.TryRemove(guid2, out var _);
					base.WmiAdapter.MappingIdNameResource.TryAdd(guid2, text3);
					base.WmiAdapter.MappingNameIdResource.TryRemove(value2, out var _);
					base.WmiAdapter.MappingNameIdResource.TryAdd(text3, guid2);
				}
				base.WmiAdapter.EnqueueNotification(new ResourceNotification(new ClusterRenamedEventArgs(guid2, text3, null)));
				return true;
			}
			case "MSCluster_EventResourceAdded":
			{
				string text7 = (string)newEvent["EventObjectName"];
				Guid guid3 = new Guid((string)newEvent["EventObjectId"]);
				PResourceType pResourceType = new PResourceType(base.WmiAdapter.clusters.Cluster, (string)newEvent["EventResourceType"]);
				lock (loadingResourcesLock)
				{
					if (!base.WmiAdapter.MappingIdNameResource.ContainsKey(guid3))
					{
						base.WmiAdapter.MappingIdNameResource.TryAdd(guid3, text7);
					}
					if (!base.WmiAdapter.MappingNameIdResource.ContainsKey(text7))
					{
						base.WmiAdapter.MappingNameIdResource.TryAdd(text7, guid3);
					}
				}
				ClusterAddedEventArgs clusterAddedEventArgs = new ClusterAddedEventArgs(guid3, text7, (int)pResourceType.ResourceKind, null);
				clusterAddedEventArgs.Cluster = base.WmiAdapter.clusters.Cluster;
				base.WmiAdapter.EnqueueNotification(new ResourceNotification(clusterAddedEventArgs));
				return true;
			}
			case "MSCluster_EventResourceRemoved":
			{
				string text5 = (string)newEvent["EventObjectName"];
				Guid value5 = Guid.Empty;
				lock (loadingResourcesLock)
				{
					if (!base.WmiAdapter.MappingNameIdResource.TryGetValue(text5, out value5))
					{
						return true;
					}
					base.WmiAdapter.MappingIdNameResource.TryRemove(value5, out var _);
					base.WmiAdapter.MappingNameIdResource.TryRemove(text5, out var _);
				}
				ClusterRemovedEventArgs clusterRemovedEventArgs = new ClusterRemovedEventArgs(value5, text5, null);
				clusterRemovedEventArgs.Cluster = base.WmiAdapter.clusters.Cluster;
				base.WmiAdapter.EnqueueNotification(new ResourceNotification(clusterRemovedEventArgs));
				return true;
			}
			case "MSCluster_EventResourcePropertyChanged":
			{
				_ = (string)newEvent["EventObjectName"];
				string text = (string)newEvent["EventObjectId"];
				if (text == null)
				{
					return true;
				}
				Guid guid = new Guid(text);
				string resourceName = string.Empty;
				string resourceType = string.Empty;
				ClusterPropertyCollection clusterPropertyCollection = new ClusterPropertyCollection(base.WmiAdapter.clusters.Cluster.Id, guid, ClusterIdentityType.Resource);
				AdapterBase.ParseProperties(clusterPropertyCollection, ((ManagementBaseObject)newEvent["EventProperties"]).Properties, ClusterPropertyKind.Common, ref resourceName, ref resourceType);
				ParseProperties(clusterPropertyCollection, ((ManagementBaseObject)newEvent["EventPrivateProperties"]).Properties, ClusterPropertyKind.Private);
				ClusterPropertiesEventArgs clusterPropertiesEventArgs = new ClusterPropertiesEventArgs(guid, null, null, null)
				{
					Properties = clusterPropertyCollection
				};
				clusterPropertiesEventArgs.Name = resourceName;
				clusterPropertiesEventArgs.ObjectType = (int)PResourceType.StringToResourceKind(resourceType);
				clusterPropertiesEventArgs.Cluster = base.WmiAdapter.clusters.Cluster;
				base.WmiAdapter.EnqueueNotification(new ResourceNotification(clusterPropertiesEventArgs));
				base.WmiAdapter.EnqueueNotification(new ResourceNotification(new ClusterDependentsEventArgs(guid, Dependents(resourceName), null)));
				base.WmiAdapter.EnqueueNotification(new ResourceNotification(new ClusterDependenciesEventArgs(guid, Dependencies(resourceName), null)));
				string dependencyRelationship = GetDependencyRelationship(guid);
				base.WmiAdapter.EnqueueNotification(new ResourceNotification(new ClusterDependencyRelationshipEventArgs(guid, dependencyRelationship, null)));
				List<Guid> possibleOwners = GetPossibleOwners(guid);
				base.WmiAdapter.EnqueueNotification(new ResourceNotification(new ClusterResourcePossibleOwnersChangedEventArgs(guid, possibleOwners, null)));
				return true;
			}
			default:
				return false;
			}
		}

		public void Delete(Guid id, bool cleanup)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				uint num = (cleanup ? 1u : 0u);
				ExecuteResourceMethod(id, "DeleteResource", num);
			}, id.ToString());
		}

		public PResource Create(PGroup privateGroup, string name, PResourceType resourceType, bool separateMonitor)
		{
			object[] parameters = new object[5]
			{
				privateGroup.Name,
				name,
				resourceType.Name,
				separateMonitor,
				string.Empty
			};
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceMethod(Guid.Empty, "CreateResource", ref parameters);
			}, name);
			return PResource.Constructor(id: new Guid(parameters[4].ToString()), cluster: base.WmiAdapter.clusters.Cluster, name: name, resourceType: resourceType);
		}

		public IEnumerable<PResource> GetAll(bool nullElementOnError)
		{
			yield break;
		}

		public void Collect()
		{
		}

		public string GetVirtualMachineOwnerGroup(Guid vmId)
		{
			string result = string.Empty;
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(base.WmiAdapter.Scope, new ObjectQuery("select OwnerGroup from mscluster_resource where type='Virtual Machine' and PrivateProperties.vmid='{0}'".FormatInvariantCulture(vmId.ToString()))))
			{
				ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
				if (managementObjectCollection.Count > 1)
				{
					ClusterLog.LogError("More than one group found for virtual machine");
				}
				else
				{
					foreach (ManagementObject item in managementObjectCollection)
					{
						result = item["OwnerGroup"].ToString();
					}
				}
			}
			return result;
		}

		private void VirtualMachineSetOfflineAction(PVirtualMachineResource resource, VirtualMachineOfflineAction offlineAction)
		{
			byte[] bytes = BitConverter.GetBytes((int)offlineAction);
			object[] parameters = new object[2]
			{
				NativeMethods.CLUSCTL_RESOURCE_VM_SET_NEXT_OFFLINE_ACTION,
				bytes
			};
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceMethod(resource.Id, "ExecuteResourceControl", ref parameters);
			}, resource.Name);
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

		public void VirtualMachineRefreshSettings(Guid resourceId, string hostName)
		{
			ManagementScope clusterScope = WmiHelper.GetClusterWmiConnection(hostName);
			ExecuteAndCatchWmiExceptions(delegate
			{
				string path = "MSCluster_Resource.Name=\"{0}\"".FormatInvariantCulture(resourceId.ToString());
				using ManagementObject managementObject = GetSingleObject(clusterScope, path);
				managementObject.InvokeMethod("UpdateVirtualMachine", null, new InvokeMethodOptions());
			}, resourceId.ToString());
		}

		public void VirtualMachineMoveStorage(Guid resourceId, string hostName, VirtualMachineStorageMoveParameters virtualMachineStorageMoveParameters)
		{
			if (virtualMachineStorageMoveParameters.VirtualHardDisks.Any((VirtualHardDisk vhd) => vhd.DestinationPath == null || vhd.DestinationPoolId == null || vhd.Path == null))
			{
				throw new ArgumentException("VHD's must not contain null paths, pools, or pool ids", "virtualMachineStorageMoveParameters");
			}
			ManagementScope clusterScope = WmiHelper.GetClusterWmiConnection(hostName);
			ExecuteAndCatchWmiExceptions(delegate
			{
				string path = "MSCluster_Resource.Name=\"{0}\"".FormatInvariantCulture(resourceId.ToString("D", CultureInfo.InvariantCulture));
				using ManagementObject managementObject = GetSingleObject(clusterScope, path);
				using ManagementBaseObject managementBaseObject = managementObject.GetMethodParameters("MigrateVirtualMachine");
				managementBaseObject["SnapshotDestinationPath"] = virtualMachineStorageMoveParameters.SnapshotFolder;
				managementBaseObject["ConfigurationDestinationPath"] = virtualMachineStorageMoveParameters.ConfigurationFolder;
				managementBaseObject["SwapFileDestinationPath"] = virtualMachineStorageMoveParameters.PageFileFolder;
				if (virtualMachineStorageMoveParameters.VirtualHardDisks != null && virtualMachineStorageMoveParameters.VirtualHardDisks.Count > 0)
				{
					managementBaseObject["SourcePaths"] = virtualMachineStorageMoveParameters.VirtualHardDisks.Select((VirtualHardDisk vhd) => vhd.Path).ToArray();
					managementBaseObject["DestinationPaths"] = virtualMachineStorageMoveParameters.VirtualHardDisks.Select((VirtualHardDisk vhd) => vhd.DestinationPath).ToArray();
					managementBaseObject["ResourceDestinationPools"] = virtualMachineStorageMoveParameters.VirtualHardDisks.Select((VirtualHardDisk vhd) => vhd.DestinationPoolId).ToArray();
				}
				managementObject.InvokeMethod("MigrateVirtualMachine", managementBaseObject, null);
			}, resourceId.ToString());
		}

		public void NetworkNameRepairActiveDirectoryObject(Guid id)
		{
			throw new NotSupportedException("NetworkNameRepairActiveDirectoryObject is not implemented for WmiAdapter");
		}

		public void NetworkNameResetCnoPassword(PNetNameResource netNameResourcePrivate)
		{
			throw new NotSupportedException("NetworkNameResetCnoPassword is not implemented for WmiAdapter");
		}

		public void NetworkNameEnableAdObject(PNetNameResource netNameResourcePrivate)
		{
			throw new NotImplementedException("NetworkNameEnableADObject is not implemented for MocksAdapeter");
		}

		public void NetworkNameRepairReAclDNSRecords(PNetNameResource privateCNOResource)
		{
			throw new NotSupportedException("NetworkNameRepairReAclDNSRecords is not implemented for WmiAdapter");
		}

		public void AddToClusterSharedVolumes(PStorageResource storageResourcePrivate)
		{
			throw new NotSupportedException("AddResourceToClusterSharedVolumes is not implemented for WmiAdapter");
		}

		public void RemoveFromClusterSharedVolumes(PCsvVolumeResource csvVolumeResourcePrivate)
		{
			throw new NotSupportedException("RemoveResourceFromClusterSharedVolume is not implemented for WmiAdapter");
		}

		public void SetCsvRedirectedAccess(PCsvVolumeResource csvVolumeResourcePrivate, Guid deviceId, bool csvRedirectedAccessMode)
		{
			throw new NotSupportedException("SetRedirectedAccess is not implemented for WmiAdapter");
		}

		public void SetMaintenanceMode(PStorageResource storageResourcePrivate, bool maintenanceMode)
		{
			throw new NotSupportedException("SetMaintenanceMode is not implemented for WmiAdapter");
		}

		public uint GetDiskNumber(PStorageResource storageResourcePrivate, string nodeName)
		{
			throw new NotImplementedException("GetDiskNumber is not implemented for WmiAdapter");
		}

		public CsvVolumeInformation GetCsvVolumeInformation(PCsvVolumeResource csvVolumeResourcePrivate)
		{
			throw new NotImplementedException("GetCsvVolumeInformation not yet implemented on the WmiAdapter");
		}

		public void Renew(PCommonIPAddressResource ipAddress)
		{
			throw new NotSupportedException("Renew is not implemented for Wmi Adapter");
		}

		public void Release(PCommonIPAddressResource ipAddress)
		{
			throw new NotSupportedException("Release is not implemented for Wmi Adapter");
		}
	}

	private static class ClusterableStoragePoolFactory
	{
		public static ClusterableStoragePool CreateClusterableStoragePool(ManagementObject mgmtObject, Guid contextId)
		{
			ClusterableStoragePool clusterableStoragePool = new ClusterableStoragePool(contextId);
			clusterableStoragePool.PoolId = new Guid((string)mgmtObject["Id"]);
			clusterableStoragePool.DisplayName = (string)mgmtObject["Name"];
			object obj = mgmtObject["QuorumStatus"];
			if (obj != null)
			{
				clusterableStoragePool.Quorum = (StoragePoolQuorum)Enum.Parse(typeof(StoragePoolQuorum), obj.ToString());
			}
			object obj2 = mgmtObject["HealthStatus"];
			if (obj2 != null)
			{
				clusterableStoragePool.Health = (StoragePoolHealth)Enum.Parse(typeof(StoragePoolHealth), obj2.ToString());
			}
			object obj3 = mgmtObject["TotalSize"];
			if (obj3 != null)
			{
				clusterableStoragePool.TotalCapacity = (ulong)obj3;
			}
			object obj4 = mgmtObject["Usage"];
			if (obj4 != null)
			{
				clusterableStoragePool.ConsumedCapacity = (ulong)obj4;
			}
			return clusterableStoragePool;
		}
	}

	private class ResourceTypeAdapter : AdapterBase, IConnectionAdapterResourceType
	{
		private readonly object loadingResourceTypesLock = new object();

		private const string ResourceTypeElementaryQuery = "name";

		private const string ResourceTypeBasicPayloadQuery = ",resourceClass";

		private const string ResourceTypeCommonPropertiesQuery = ",DllName,Name,Description,AdminExtensions,LooksAlivePollInterval,IsAlivePollInterval,PendingTimeout,DeadlockTimeout";

		private const string ResourceTypePrivatePropertiesQuery = ",PrivateProperties";

		public string[] PropertiesName
		{
			get
			{
				List<string> list = new List<string>();
				list.AddRange(",DllName,Name,Description,AdminExtensions,LooksAlivePollInterval,IsAlivePollInterval,PendingTimeout,DeadlockTimeout".Split(','));
				list.AddRange(",PrivateProperties".Split(','));
				return list.ToArray();
			}
		}

		public ResourceTypeAdapter(WmiAdapter wmiAdapter)
			: base(wmiAdapter)
		{
			Init();
		}

		public void Init()
		{
		}

		internal List<string> NormalizeQuery(List<string> fieldsString, out ResourceTypeLoadSelection loadSelection)
		{
			loadSelection = ResourceTypeLoadSelection.None;
			List<string> list = new List<string>();
			foreach (string item in fieldsString)
			{
				if (",resourceClass".Contains("," + item.ToLowerInvariant()))
				{
					loadSelection |= ResourceTypeLoadSelection.Basic;
				}
			}
			list.AddRange("name".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			if ((loadSelection & ResourceTypeLoadSelection.Basic) == ResourceTypeLoadSelection.Basic)
			{
				list.AddRange(",resourceClass".Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			}
			return list;
		}

		public IEnumerable<PResourceType> GetAll(IList<string> queryFields, bool nullElementOnError)
		{
			yield break;
		}

		internal TResult ProcessResult<TResult>(ManagementObject mgmtObj, QueryInfo queryInfo, ResourceTypeLoadSelection loadSelection) where TResult : PClusterObject
		{
			string resourceTypeName = (string)mgmtObj["name"];
			PResourceType pResourceType = new PResourceType(base.WmiAdapter.clusters.Cluster, resourceTypeName);
			pResourceType.Name = (string)mgmtObj["name"];
			if ((loadSelection & ResourceTypeLoadSelection.Basic) == ResourceTypeLoadSelection.Basic)
			{
				pResourceType.Class = (ResourceClass)(uint)mgmtObj["resourceclass"];
				pResourceType.IsStorage = pResourceType.Class == ResourceClass.Storage;
				ParseProperties(pResourceType.Properties, mgmtObj.Properties, ClusterPropertyKind.Common);
				ParseProperties(pResourceType.Properties, ((ManagementBaseObject)mgmtObj["PrivateProperties"]).Properties, ClusterPropertyKind.Private);
				pResourceType.LoadedSelection |= 1;
			}
			if ((loadSelection & ResourceTypeLoadSelection.PossibleOwners) == ResourceTypeLoadSelection.PossibleOwners)
			{
				pResourceType.PossibleOwners = GetPossibleOwnersList(mgmtObj);
				pResourceType.LoadedSelection |= 8;
			}
			return (TResult)(PClusterObject)pResourceType;
		}

		public void Load(PResourceType resourceType, ResourceTypeLoadSelection loadSelection)
		{
			if ((loadSelection & ResourceTypeLoadSelection.Basic) == ResourceTypeLoadSelection.Basic || (loadSelection & ResourceTypeLoadSelection.CommonProperties) == ResourceTypeLoadSelection.CommonProperties || (loadSelection & ResourceTypeLoadSelection.PrivateProperties) == ResourceTypeLoadSelection.PrivateProperties)
			{
				using ManagementObject managementObject = GetSingleObject(resourceType.Name, "mscluster_resourcetype");
				if (managementObject == null)
				{
					throw new ClusterObjectNotFoundException(resourceType.Name, resourceType.Id, typeof(PResource));
				}
				try
				{
					resourceType.Class = (ResourceClass)(uint)managementObject["ResourceClass"];
					resourceType.IsStorage = resourceType.Class == ResourceClass.Storage;
					resourceType.LoadedSelection |= 1;
					ParseProperties(resourceType.Properties, managementObject.Properties, ClusterPropertyKind.Common);
					resourceType.LoadedSelection |= 2;
					ParseProperties(resourceType.Properties, ((ManagementBaseObject)managementObject["PrivateProperties"]).Properties, ClusterPropertyKind.Private);
					resourceType.LoadedSelection |= 4;
				}
				catch (Exception innerException)
				{
					throw new ClusterObjectLoadFailedException(resourceType.Name, resourceType.Id, innerException);
				}
			}
			if ((loadSelection & ResourceTypeLoadSelection.PossibleOwners) == ResourceTypeLoadSelection.PossibleOwners)
			{
				resourceType.PossibleOwners = GetPossibleOwnersList(resourceType.Name);
				resourceType.LoadedSelection |= 8;
			}
		}

		public PResourceType Open(string resourceTypeName)
		{
			PResourceType pResourceType = null;
			using (GetSingleObject(resourceTypeName, "mscluster_resourcetype"))
			{
				pResourceType = new PResourceType(base.WmiAdapter.clusters.Cluster, resourceTypeName);
				pResourceType.Name = resourceTypeName;
				return pResourceType;
			}
		}

		public IEnumerable<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> GetReplicationResources()
		{
			return new List<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK>();
		}

		public IEnumerable<ReplicationGroupInfo> GetReplicationGroups()
		{
			return new List<ReplicationGroupInfo>();
		}

		public IEnumerable<string> GetPossibleOwners(string name)
		{
			throw new NotSupportedException("GetPossibleOwners is not supported by WMI adapter");
		}

		private List<Guid> GetPossibleOwnersList(string resourceTypeName)
		{
			object[] parameters = new object[1];
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceTypeMethod(resourceTypeName, "GetPossibleOwners", ref parameters);
			}, resourceTypeName);
			return new List<Guid>(((Array)parameters[0]).ConvertAll((string nodeName) => base.WmiAdapter.nodes.GetNodeIdFromName(nodeName)));
		}

		private List<Guid> GetPossibleOwnersList(ManagementObject mgmtObj)
		{
			object[] parameters = new object[1];
			ExecuteAndCatchWmiExceptions(delegate
			{
				ExecuteResourceTypeMethod(mgmtObj, "GetPossibleOwners", ref parameters);
			}, (string)mgmtObj["Name"]);
			return new List<Guid>(((Array)parameters[0]).ConvertAll((string nodeName) => base.WmiAdapter.nodes.GetNodeIdFromName(nodeName)));
		}

		private void ExecuteResourceTypeMethod(ManagementObject mgmtObj, string methodName, ref object[] parameters)
		{
			mgmtObj.InvokeMethod(methodName, parameters);
		}

		private void ExecuteResourceTypeMethod(string resourceTypeName, string methodName, ref object[] parameters)
		{
			ObjectGetOptions options = new ObjectGetOptions();
			ManagementObject managementObject = null;
			try
			{
				managementObject = ((!string.IsNullOrEmpty(resourceTypeName)) ? new ManagementObject(base.WmiAdapter.Scope, new ManagementPath("MSCluster_ResourceType.Name=\"" + resourceTypeName + "\""), options) : new ManagementClass(base.WmiAdapter.Scope, new ManagementPath("mscluster_resourcetype"), options));
				managementObject.InvokeMethod(methodName, parameters);
			}
			finally
			{
				managementObject?.Dispose();
			}
		}

		public bool ProcessEvent(ManagementBaseObject newEvent)
		{
			string className = newEvent.ClassPath.ClassName;
			if (className == "MSCluster_EventResourceTypePropertyChanged")
			{
				return true;
			}
			return false;
		}

		public PResourceType Create(string name, string displayName, string pathDll)
		{
			throw new NotSupportedException("Create is not supported by Wmi Adapter");
		}

		public void Delete(string resourceType)
		{
			throw new NotSupportedException("Delete is not supported by Wmi Adapter");
		}

		public void Collect()
		{
		}
	}

	private class StorageAdapter : AdapterBase, IConnectionAdapterStorage
	{
		public StorageAdapter(WmiAdapter wmiAdapter)
			: base(wmiAdapter)
		{
			Init();
		}

		private void Init()
		{
		}

		public bool ProcessEvent(ManagementBaseObject newEvent)
		{
			Utilities.UnreferencedParameter(newEvent);
			return false;
		}

		public void Collect()
		{
		}

		public void SetReplicationLogSize(PStorageResource storageResourcePrivate, long logSize)
		{
		}

		public void RemoveReplication(PStorageResource storageResource, bool fullCleanUp)
		{
			throw new NotSupportedException();
		}

		public IEnumerable<Guid> GetReplicationGroupPartnership(PNode ownerNode, Guid replicationGroupId, ReplicationGroupRole role)
		{
			return new List<Guid>();
		}

		public IEnumerable<string> GetReplicationGroupPartnership(PNode ownerNode, string replicationGroupName, ReplicationGroupRole role)
		{
			return new List<string>();
		}

		public void LoadReplicationInfo(PStorageResource resource)
		{
		}

		public uint? GetDiskNumber(PStorageResource storageResourcePrivate, string uniqueId, string nodeName)
		{
			throw new NotSupportedException();
		}

		public string GetUniqueId(uint diskNumber, string nodeName)
		{
			throw new NotSupportedException();
		}

		public IEnumerable<T1> Enumerate<T1>(ObservableKeyCollection<T1> collection, ObservableCollectionFilter<T1> filter) where T1 : IKeyQueryable<T1>
		{
			throw new NotSupportedException();
		}

		public IEnumerable<T1> Association<T, T1>(ObservableKeyCollection<T1> collection, T association) where T1 : IKeyQueryable<T1>
		{
			throw new NotSupportedException();
		}

		public void Subscribe<T1>(ObservableKeyCollection<T1> collection) where T1 : IKeyQueryable<T1>
		{
			throw new NotSupportedException();
		}

		public void Unsubscribe<T1>(ObservableKeyCollection<T1> collection) where T1 : IKeyQueryable<T1>
		{
			throw new NotSupportedException();
		}

		public T1 GetInstance<T1>(string key, string serverName = null) where T1 : IKeyQueryable
		{
			throw new NotSupportedException();
		}
	}

	private const string EnumerateRecords = "select {0} from {1}";

	private const string EnumerateRecordsWhere = "select {0} from {1} where {2}";

	private const string SubscribeNotificationsQuery = "select * from mscluster_eventex";

	private const string SubscribeHeartbeat = "select * from mscluster_eventheartbeat";

	private const string LoadObjectPartial = "select {0} from {1} where Id='{2}'";

	private const string LoadObjectPartialName = "select {0} from {1} where Name='{2}'";

	private const string LoadObjectTotal = "select * from {0}";

	private const string LoadNodeQuery = "select * from {0} where nodeInstanceId='{1}'";

	private const string LoadNodePartial = "select {0} from {1} where nodeInstanceId='{2}'";

	private const string LoadNodeByName = "select * from {0} where name='{1}'";

	private const string TableGroups = "mscluster_resourcegroup";

	private const string TableNodes = "mscluster_node";

	private const string TableNetworks = "mscluster_network";

	private const string TableNetworkInterfaces = "mscluster_networkinterface";

	private const string TableResources = "mscluster_resource";

	private const string TableCluster = "mscluster_cluster";

	private const string TableResourceTypes = "mscluster_resourcetype";

	private const string TablePrivateProperties = "MSCluster_Properties";

	private const int HeartBeatLimit = 10;

	private readonly ClusterAdapter clusters;

	private readonly GroupAdapter groups;

	private readonly NodeAdapter nodes;

	private readonly NetworkAdapter networks;

	private readonly NetworkInterfaceAdapter networkInterfaces;

	private readonly ResourceAdapter resources;

	private readonly StorageAdapter storage;

	private readonly ResourceTypeAdapter resourceTypes;

	private readonly Queue<Notification> notificationQueue = new Queue<Notification>();

	private readonly object notificationLock = new object();

	private readonly AutoResetEvent notificationReady = new AutoResetEvent(initialState: false);

	private readonly ManualResetEvent notificationsPaused = new ManualResetEvent(initialState: true);

	private bool exitNotifications;

	private ManagementEventWatcher notifications;

	private ManagementEventWatcher notificationsHeartBeat;

	private ManagementObject clusterMof;

	private ManagementObject groupMof;

	private ManagementObject resourceMof;

	private ManagementObject nodeMof;

	private ManagementObject networkMof;

	private ManagementObject networkInterfaceMof;

	private ManagementObject privateProp;

	private AdapterBase CommonAdapter { get; set; }

	protected ManagementScope Scope { get; private set; }

	protected bool QueriesAreAsync { get; private set; }

	public IConnectionAdapterCluster Cluster => clusters;

	public ClusterAdapterType Adapter => ClusterAdapterType.Wmi;

	public IConnectionAdapterGroup Group => groups;

	public IConnectionAdapterResource Resource => resources;

	public IConnectionAdapterNode Node => nodes;

	public IConnectionAdapterNetwork Network => networks;

	public IConnectionAdapterStorage Storage => storage;

	public IConnectionAdapterNetworkInterface NetworkInterface => networkInterfaces;

	public IConnectionAdapterResourceType ResourceType => resourceTypes;

	public WmiAdapter(PCluster cluster)
	{
		if (cluster == null)
		{
			throw new ArgumentNullException("cluster");
		}
		QueriesAreAsync = true;
		clusters = new ClusterAdapter(this, cluster);
		nodes = new NodeAdapter(this);
		networks = new NetworkAdapter(this);
		networkInterfaces = new NetworkInterfaceAdapter(this);
		groups = new GroupAdapter(this);
		resources = new ResourceAdapter(this);
		storage = new StorageAdapter(this);
		resourceTypes = new ResourceTypeAdapter(this);
		CommonAdapter = new AdapterBase(this);
	}

	~WmiAdapter()
	{
		notificationReady.Dispose();
		notificationsPaused.Dispose();
	}

	public void Close()
	{
		Cluster.Close();
	}

	public void Collect()
	{
		clusters.Collect();
		groups.Collect();
		nodes.Collect();
		networks.Collect();
		networkInterfaces.Collect();
		resources.Collect();
		storage.Collect();
		resourceTypes.Collect();
	}

	public IEnumerable<PClusterObject> Select<TInput>(IClusterList<TInput> query) where TInput : ClusterObject
	{
		return Select<PClusterObject>(((ClusterList<TInput>)query).QueryInfo);
	}

	public IEnumerable<TResult> Select<TResult>(QueryInfo queryInfo) where TResult : PClusterObject
	{
		ManagementOperationObserver managementOperationObserver = new ManagementOperationObserver();
		AutoResetEvent doneEvent = new AutoResetEvent(initialState: true);
		AutoResetEvent rdyEvent = new AutoResetEvent(initialState: false);
		TResult returnObject = null;
		Exception lastError = null;
		IEnumerable<ClusterObjectMetaDataMember> enumerable = queryInfo.ProjectionFields.Select((ClusterObjectMetaDataMember s) => s).Concat(queryInfo.WhereFields.Select((ClusterObjectMetaDataMember s) => s)).Concat(from s in queryInfo.OrderBy.ConvertAll((OrderByItem item) => item.DataMember)
			select (s))
			.Distinct();
		List<string> list = new List<string>();
		foreach (ClusterObjectMetaDataMember item in enumerable)
		{
			switch (item.MappedName.ToLowerInvariant())
			{
			case "resourceproperties":
				list.AddRange(resources.PropertiesName);
				break;
			case "groupproperties":
				list.AddRange(groups.PropertiesName);
				break;
			case "nodeproperties":
				list.AddRange(nodes.PropertiesName);
				break;
			case "networkproperties":
				list.AddRange(networks.PropertyNames);
				break;
			case "networkinterfaceproperties":
				list.AddRange(networkInterfaces.PropertyNames);
				break;
			case "resourcetypeproperties":
				list.AddRange(resourceTypes.PropertiesName);
				break;
			default:
				list.Add(item.MappedName);
				break;
			}
		}
		list = NormalizeQuery<TResult>(list, queryInfo, out var loadSelection);
		Action<Exception> completed = delegate(Exception mgmtException)
		{
			lastError = mgmtException;
			returnObject = null;
			rdyEvent.Set();
		};
		Action<ManagementObject> objectArrived = delegate(ManagementObject mgmtObj)
		{
			try
			{
				doneEvent.WaitOne();
				if (queryInfo.IsCancel)
				{
					completed(null);
				}
				else
				{
					returnObject = ProcessResult<TResult>(mgmtObj, queryInfo, loadSelection);
				}
			}
			catch (Exception ex)
			{
				returnObject = null;
				lastError = ex;
			}
			finally
			{
				rdyEvent.Set();
			}
		};
		string arg = FormatHelper.FormatColumnNamesInSequence(list.ToArray());
		string table = GetTable(queryInfo);
		if (table == null)
		{
			throw new InvalidOperationException("This query is not supported");
		}
		ObjectQuery query;
		if (queryInfo.WhereLambdaExpression == null)
		{
			query = new ObjectQuery($"select {arg} from {table}");
		}
		else
		{
			StringBuilder stringBuilder = new StringBuilder();
			OperatorArgument operatorArgument = null;
			List<IClusterQueryArgument> whereSyntaxis = queryInfo.WhereSyntaxis;
			string text = null;
			foreach (IClusterQueryArgument item2 in whereSyntaxis)
			{
				if (item2 is StartEndArgument || item2 is FieldArgument)
				{
					stringBuilder.Append(item2.ToString());
					if (item2 is FieldArgument)
					{
						text = item2.Name;
					}
					continue;
				}
				if (item2 is GuidArgument)
				{
					string text2 = text.ToLowerInvariant();
					if (!(text2 == "ownernode"))
					{
						if (text2 == "ownergroup")
						{
							string value = null;
							lock (groups.LoadingGroupLock)
							{
								value = base.MappingIdNameGroup[(Guid)((GuidArgument)item2).Value];
							}
							stringBuilder.Append("'");
							stringBuilder.Append(value);
							stringBuilder.Append("'");
						}
						else
						{
							stringBuilder.Append(item2.ToString());
						}
					}
					else
					{
						string value2 = null;
						lock (nodes.LoadingNodesLock)
						{
							value2 = base.MappingIdNameNode[(Guid)((GuidArgument)item2).Value];
						}
						stringBuilder.Append("'");
						stringBuilder.Append(value2);
						stringBuilder.Append("'");
					}
					continue;
				}
				if (item2 is ValueArgument)
				{
					ValueArgument valueArgument = item2 as ValueArgument;
					if (table == "mscluster_resource" && text != null && text.Equals("type", StringComparison.InvariantCultureIgnoreCase))
					{
						string arg2 = PResourceType.ResourceKindToString((ResourceKind)int.Parse(item2.ToString()));
						stringBuilder.Append($"'{arg2}'");
					}
					else if (operatorArgument != null)
					{
						switch (operatorArgument.OperatorType)
						{
						case OperatorType.Contains:
							stringBuilder.Append($"'%{valueArgument.Value}%'");
							break;
						case OperatorType.StartsWith:
							stringBuilder.Append($"'{valueArgument.Value}%'");
							break;
						case OperatorType.EndsWith:
							stringBuilder.Append($"'%{valueArgument.Value}'");
							break;
						default:
							stringBuilder.Append(item2.ToString());
							break;
						}
					}
					else
					{
						stringBuilder.Append(item2.ToString());
					}
					continue;
				}
				if (item2 is OperatorArgument)
				{
					operatorArgument = item2 as OperatorArgument;
					switch (operatorArgument.OperatorType)
					{
					case OperatorType.And:
						stringBuilder.Append(" and ");
						break;
					case OperatorType.Equal:
						stringBuilder.Append(" = ");
						break;
					case OperatorType.GreaterThan:
						stringBuilder.Append(" > ");
						break;
					case OperatorType.GreaterThanOrEqual:
						stringBuilder.Append(" >= ");
						break;
					case OperatorType.Is:
						stringBuilder.Append(" is ");
						break;
					case OperatorType.IsNot:
						stringBuilder.Append(" is not ");
						break;
					case OperatorType.LessThan:
						stringBuilder.Append(" < ");
						break;
					case OperatorType.LessThanOrEqual:
						stringBuilder.Append(" <= ");
						break;
					case OperatorType.NotEqual:
						stringBuilder.Append(" != ");
						break;
					case OperatorType.Or:
						stringBuilder.Append(" or ");
						break;
					case OperatorType.Contains:
					case OperatorType.StartsWith:
					case OperatorType.EndsWith:
						stringBuilder.Append(" like ");
						break;
					default:
						throw new NotSupportedException("Operator not supported");
					}
					continue;
				}
				throw new NotSupportedException("Argument not supported");
			}
			query = ((stringBuilder.Length != 0) ? new ObjectQuery($"select {arg} from {table} where {stringBuilder.ToString()}") : new ObjectQuery($"select {arg} from {table}"));
		}
		ManagementObjectSearcher searcher = new ManagementObjectSearcher(Scope, query);
		try
		{
			if (QueriesAreAsync)
			{
				managementOperationObserver.ObjectReady += delegate(object sender, ObjectReadyEventArgs e)
				{
					objectArrived((ManagementObject)e.NewObject);
				};
				managementOperationObserver.Completed += delegate(object sender, CompletedEventArgs e)
				{
					ManagementException obj2 = null;
					if (e.Status != 0)
					{
						obj2 = new ManagementException("Generic Failure");
						FieldInfo field = typeof(ManagementException).GetField("errorCode", BindingFlags.Instance | BindingFlags.NonPublic);
						FieldInfo field2 = typeof(ManagementException).GetField("errorObject", BindingFlags.Instance | BindingFlags.NonPublic);
						field.SetValue(obj2, e.Status);
						field2.SetValue(obj2, e.StatusObject);
					}
					completed(obj2);
				};
				searcher.Get(managementOperationObserver);
			}
			else
			{
				Worker.Start(delegate
				{
					searcher.Options = new EnumerationOptions
					{
						ReturnImmediately = true,
						Rewindable = false,
						BlockSize = 500
					};
					using (ManagementObjectCollection managementObjectCollection = searcher.Get())
					{
						foreach (ManagementObject item3 in managementObjectCollection)
						{
							objectArrived(item3);
						}
					}
					completed(null);
				}, completed);
			}
			while (true)
			{
				rdyEvent.WaitOne();
				if (returnObject == null)
				{
					break;
				}
				yield return returnObject;
				doneEvent.Set();
			}
			if (lastError != null)
			{
				throw lastError;
			}
		}
		finally
		{
			if (searcher != null)
			{
				((IDisposable)searcher).Dispose();
			}
		}
	}

	private string GetTable(QueryInfo queryInfo)
	{
		if (typeof(Group).IsAssignableFrom(queryInfo.Source))
		{
			return "mscluster_resourcegroup";
		}
		if (typeof(Resource).IsAssignableFrom(queryInfo.Source))
		{
			return "mscluster_resource";
		}
		if (typeof(Node).IsAssignableFrom(queryInfo.Source))
		{
			return "mscluster_node";
		}
		if (typeof(Network).IsAssignableFrom(queryInfo.Source))
		{
			return "mscluster_network";
		}
		if (typeof(NetworkInterface).IsAssignableFrom(queryInfo.Source))
		{
			return "mscluster_networkinterface";
		}
		if (typeof(ResourceType).IsAssignableFrom(queryInfo.Source))
		{
			return "mscluster_resourcetype";
		}
		return null;
	}

	private TResult ProcessResult<TResult>(ManagementObject mgmtObj, QueryInfo queryInfo, int loadSelection) where TResult : PClusterObject
	{
		if (typeof(Group).IsAssignableFrom(queryInfo.Source))
		{
			return groups.ProcessResult<TResult>(mgmtObj, queryInfo, (GroupLoadSelection)loadSelection);
		}
		if (typeof(Resource).IsAssignableFrom(queryInfo.Source))
		{
			return resources.ProcessResult<TResult>(mgmtObj, queryInfo, (ResourceLoadSelection)loadSelection);
		}
		if (typeof(Node).IsAssignableFrom(queryInfo.Source))
		{
			return nodes.ProcessResult<TResult>(mgmtObj, queryInfo, (NodeLoadSelection)loadSelection);
		}
		if (typeof(Network).IsAssignableFrom(queryInfo.Source))
		{
			return networks.ProcessResult<TResult>(mgmtObj, queryInfo, (NetworkLoadSelection)loadSelection);
		}
		if (typeof(NetworkInterface).IsAssignableFrom(queryInfo.Source))
		{
			return networkInterfaces.ProcessResult<TResult>(mgmtObj, queryInfo, (NetworkInterfaceLoadSelection)loadSelection);
		}
		if (typeof(ResourceType).IsAssignableFrom(queryInfo.Source))
		{
			return resourceTypes.ProcessResult<TResult>(mgmtObj, queryInfo, (ResourceTypeLoadSelection)loadSelection);
		}
		throw new NotSupportedException("Object type not supported");
	}

	private List<string> NormalizeQuery<TResult>(List<string> fieldsString, QueryInfo queryInfo, out int loadSelection) where TResult : PClusterObject
	{
		if (typeof(Group).IsAssignableFrom(queryInfo.Source))
		{
			GroupLoadSelection loadSelection2;
			List<string> result = groups.NormalizeQuery(fieldsString, out loadSelection2);
			loadSelection = (int)loadSelection2;
			return result;
		}
		if (typeof(Resource).IsAssignableFrom(queryInfo.Source))
		{
			ResourceLoadSelection loadSelection3;
			List<string> result2 = resources.NormalizeQuery(fieldsString, out loadSelection3);
			loadSelection = (int)loadSelection3;
			return result2;
		}
		if (typeof(Node).IsAssignableFrom(queryInfo.Source))
		{
			NodeLoadSelection loadSelection4;
			List<string> result3 = nodes.NormalizeQuery(fieldsString, out loadSelection4);
			loadSelection = (int)loadSelection4;
			return result3;
		}
		if (typeof(Network).IsAssignableFrom(queryInfo.Source))
		{
			NetworkLoadSelection loadSelection5;
			List<string> result4 = networks.NormalizeQuery(fieldsString, out loadSelection5);
			loadSelection = (int)loadSelection5;
			return result4;
		}
		if (typeof(NetworkInterface).IsAssignableFrom(queryInfo.Source))
		{
			NetworkInterfaceLoadSelection loadSelection6;
			List<string> result5 = networkInterfaces.NormalizeQuery(fieldsString, out loadSelection6);
			loadSelection = (int)loadSelection6;
			return result5;
		}
		if (typeof(ResourceType).IsAssignableFrom(queryInfo.Source))
		{
			ResourceTypeLoadSelection loadSelection7;
			List<string> result6 = resourceTypes.NormalizeQuery(fieldsString, out loadSelection7);
			loadSelection = (int)loadSelection7;
			return result6;
		}
		throw new NotSupportedException("Object type not supported");
	}

	private ManagementObject GetFirstRecord(string table)
	{
		using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(Scope, new ObjectQuery("select * from {0}".FormatInvariantCulture(table)));
		managementObjectSearcher.Options = new EnumerationOptions
		{
			ReturnImmediately = true,
			Rewindable = false,
			BlockSize = 1
		};
		using (ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = managementObjectSearcher.Get().GetEnumerator())
		{
			if (managementObjectEnumerator.MoveNext())
			{
				return (ManagementObject)managementObjectEnumerator.Current;
			}
		}
		return null;
	}

	public void SubscribeNotifications(Action notificationLostAction, Action<ClusterException> notificationConnectionUnrepairableAction)
	{
		Action action = delegate
		{
			Worker.Start(delegate
			{
				QueriesAreAsync = false;
				notifications.Options = new EventWatcherOptions
				{
					BlockSize = 1,
					Timeout = new TimeSpan(0, 0, 1)
				};
				while (!exitNotifications)
				{
					try
					{
						EventsEventArrived(notifications, notifications.WaitForNextEvent());
					}
					catch (ManagementException ex6)
					{
						if (ex6.ErrorCode != ManagementStatus.Timedout)
						{
							throw CommonAdapter.ConvertException(ex6);
						}
					}
					catch (COMException exception2)
					{
						ClusterDialogException ex7 = CommonAdapter.ConvertException(exception2, clusters.Cluster.Name);
						if (ex7 != null)
						{
							throw ex7;
						}
					}
					catch (UnauthorizedAccessException innerException2)
					{
						throw new ClusterDefaultException(innerException2);
					}
				}
			});
		};
		exitNotifications = false;
		notifications = new ManagementEventWatcher(Scope, new EventQuery("select * from mscluster_eventex"));
		notifications.EventArrived += delegate(object sender, EventArrivedEventArgs eventArrived)
		{
			EventsEventArrived(sender, eventArrived.NewEvent);
		};
		try
		{
			notifications.Start();
		}
		catch (UnauthorizedAccessException)
		{
			action();
		}
		catch (COMException ex2)
		{
			if (ex2.ErrorCode == -2147022986)
			{
				action();
			}
		}
		notificationsHeartBeat = new ManagementEventWatcher(Scope, new EventQuery("select * from mscluster_eventheartbeat"));
		notificationsHeartBeat.EventArrived += delegate(object sender, EventArrivedEventArgs eventArrived)
		{
			EventsEventArrived(sender, eventArrived.NewEvent);
		};
		try
		{
			notificationsHeartBeat.Start();
		}
		catch (UnauthorizedAccessException)
		{
			Worker.Start(delegate
			{
				notificationsHeartBeat.Options = new EventWatcherOptions
				{
					BlockSize = 0,
					Timeout = new TimeSpan(0, 0, 1)
				};
				try
				{
					notificationsHeartBeat.WaitForNextEvent();
				}
				catch (ManagementException ex4)
				{
					if (ex4.ErrorCode != ManagementStatus.Timedout)
					{
						throw CommonAdapter.ConvertException(ex4);
					}
				}
				catch (COMException exception)
				{
					ClusterDialogException ex5 = CommonAdapter.ConvertException(exception, clusters.Cluster.Name);
					if (ex5 != null)
					{
						throw ex5;
					}
				}
				catch (UnauthorizedAccessException innerException)
				{
					throw new ClusterDefaultException(innerException);
				}
			});
		}
	}

	public void UnsubscribeNotifications()
	{
		if (QueriesAreAsync)
		{
			notifications.Stop();
			notificationsHeartBeat.Stop();
		}
		notificationsPaused.Set();
		exitNotifications = true;
		notifications.Dispose();
		notificationsHeartBeat.Dispose();
	}

	private void EventsEventArrived(object sender, ManagementBaseObject newEvent)
	{
		clusters.HeartBeat = DateTime.Now.AddSeconds(10.0);
		try
		{
			if (!groups.ProcessEvent(newEvent) && !resources.ProcessEvent(newEvent) && !nodes.ProcessEvent(newEvent) && !networks.ProcessEvent(newEvent) && !networkInterfaces.ProcessEvent(newEvent) && !storage.ProcessEvent(newEvent) && !resourceTypes.ProcessEvent(newEvent))
			{
				string className = newEvent.ClassPath.ClassName;
				if (!(className == "MSCluster_EventRegistryChange"))
				{
					ClusterLog.LogWarning("Event not found:" + newEvent.ClassPath);
				}
			}
		}
		catch (ClusterObjectNotFoundException)
		{
		}
		catch (Exception exception)
		{
			ClusterLog.LogException(exception, "Error Processing Cluster Notifications");
		}
	}

	public void EnqueueNotification(Notification notification)
	{
		lock (notificationLock)
		{
			notificationQueue.Enqueue(notification);
			notificationReady.Set();
		}
	}

	public void PauseNotifications()
	{
		notificationsPaused.Reset();
	}

	public void ResumeNotifications()
	{
		notificationsPaused.Set();
	}

	public void ResetNotifications()
	{
		lock (notificationLock)
		{
			notificationQueue.Clear();
		}
	}

	public Notification DequeueNotification()
	{
		return DequeueNotification(-1);
	}

	[DebuggerNonUserCode]
	public Notification DequeueNotification(int milliSecondsTimeout)
	{
		notificationsPaused.WaitOne(-1);
		if (exitNotifications)
		{
			throw new InvalidOperationException("A notification can be dequeued when the notification process has not been started");
		}
		if (notificationQueue.Count == 0 && !notificationReady.WaitOne(milliSecondsTimeout))
		{
			return null;
		}
		lock (notificationLock)
		{
			notificationReady.Reset();
			return notificationQueue.Dequeue();
		}
	}

	public void SaveProperties(PClusterObject clusterObject, ClusterPropertyCollection properties)
	{
		ObjectGetOptions objectGetOptions = new ObjectGetOptions();
		objectGetOptions.Context.Add("__GET_EXT_KEYS_ONLY", true);
		objectGetOptions.Context.Add("__GET_EXTENSIONS", true);
		ManagementObject mgmtObj = null;
		if (clusterObject is PGroup)
		{
			mgmtObj = CommonAdapter.GetSingleObject(clusterObject.Name, "mscluster_resourcegroup", objectGetOptions);
		}
		else if (clusterObject is PResource)
		{
			mgmtObj = CommonAdapter.GetSingleObject(clusterObject.Name, "mscluster_resource", objectGetOptions);
		}
		else if (clusterObject is PNode)
		{
			mgmtObj = CommonAdapter.GetSingleObject(clusterObject.Name, "mscluster_node", objectGetOptions);
		}
		else if (clusterObject is PNetwork)
		{
			mgmtObj = CommonAdapter.GetSingleObject(clusterObject.Name, "mscluster_network", objectGetOptions);
		}
		else if (clusterObject is PNetworkInterface)
		{
			mgmtObj = CommonAdapter.GetSingleObject(clusterObject.Name, "mscluster_networkinterface", objectGetOptions);
		}
		else
		{
			if (!(clusterObject is PCluster))
			{
				throw new NotSupportedException("The object type {0} is not supported when saving properties".FormatCurrentCulture(clusterObject.GetType()));
			}
			mgmtObj = CommonAdapter.GetSingleObject(clusterObject.Name, "mscluster_cluster", objectGetOptions);
		}
		ManagementObject mgmtProp = (ManagementObject)privateProp.Clone();
		CommonAdapter.ExecuteAndCatchWmiExceptions(delegate
		{
			try
			{
				foreach (ClusterProperty property in properties)
				{
					object obj = null;
					obj = ((!(property is ClusterPropertyMultipleStrings)) ? property.Value : ((ClusterPropertyMultipleStrings)property).TypedValue.ToArray());
					string text = property.RealName ?? property.Name;
					if (property.PropertyKind == ClusterPropertyKind.Common && (property.IsModified || property.IsDeleted))
					{
						bool flag = false;
						foreach (PropertyData property2 in mgmtObj.Properties)
						{
							if (property2.Name.Equals(text, StringComparison.CurrentCultureIgnoreCase))
							{
								flag = true;
							}
						}
						if (!flag)
						{
							throw new InvalidOperationException("Wmi Interface does not support adding new common properties to the cluster");
						}
						PropertyData propertyData = mgmtObj.Properties[text];
						propertyData.Value = obj;
						propertyData.Qualifiers.Add("Modify", true);
						propertyData.Qualifiers.Add("PropertyType", (int)property.PropertyType);
						if (property.RealName != null && property.Name != property.RealName)
						{
							propertyData.Qualifiers.Add("PropertyName", property.Name);
						}
						if (property.IsDeleted)
						{
							propertyData.Qualifiers.Add("Delete", true);
						}
					}
					else if (property.PropertyKind == ClusterPropertyKind.Private && (property.IsModified || property.IsDeleted))
					{
						mgmtProp.Properties.Add(text, obj);
						PropertyData propertyData2 = mgmtProp.Properties[text];
						propertyData2.Qualifiers.Add("Modify", true);
						propertyData2.Qualifiers.Add("PropertyType", (int)property.PropertyType);
						if (property.RealName != null && property.Name != property.RealName)
						{
							propertyData2.Qualifiers.Add("PropertyName", property.Name);
						}
						if (property.IsDeleted)
						{
							propertyData2.Qualifiers.Add("Delete", true);
						}
					}
				}
				mgmtObj["PrivateProperties"] = mgmtProp;
				PutOptions options = new PutOptions
				{
					Context = { 
					{
						"NO_PROPERTY_VERIFY",
						(object)true
					} }
				};
				mgmtObj.Put(options);
			}
			finally
			{
				if (mgmtObj != null)
				{
					mgmtObj.Dispose();
				}
				if (mgmtProp != null)
				{
					mgmtProp.Dispose();
				}
			}
		}, clusterObject.Name);
	}
}


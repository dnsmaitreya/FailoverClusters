using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UI.Common.Reactive;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Generic;
using Microsoft.Management.Infrastructure.Options;

namespace MS.Internal.FailoverClusters.Framework;

internal class CimAdapter : RootAdapterBase, IConnectionAdapter
{
	internal class AdapterBase
	{
		protected CimAdapter CimAdapter { get; private set; }

		protected CimClass CimCluster => CimAdapter.CimCluster;

		protected CimClass CimGroup => CimAdapter.CimGroup;

		protected CimClass CimResource => CimAdapter.CimResource;

		protected CimClass CimNode => CimAdapter.CimNode;

		protected CimClass CimNetwork => CimAdapter.CimNetwork;

		protected CimClass CimNetworkInterface => CimAdapter.CimNetworkInterface;

		protected CimClass CimResourceType => CimAdapter.CimResourceType;

		protected CimClass CimQuorumSettings => CimAdapter.CimQuorumSettings;

		protected CimClass CimClusterService => CimAdapter.CimClusterService;

		public virtual CimClass IdentityClass => null;

		public virtual string ElementaryPayloadQuery => "elementname,name";

		public virtual string BasicPayloadQuery => string.Empty;

		public virtual string CommonPropertiesQuery => string.Empty;

		public AdapterBase(CimAdapter cimAdapter)
		{
			CimAdapter = cimAdapter;
		}

		public T ExecuteAndCatchWmiExceptions<T>(Func<T> action, string objectName)
		{
			try
			{
				return action.SafeCall();
			}
			catch (CimException exception)
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
			catch (Win32Exception innerException)
			{
				throw new ClusterDefaultException(innerException);
			}
			catch (FileNotFoundException innerException2)
			{
				throw new ClusterDefaultException(innerException2);
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
			if (exception is CimException ex)
			{
				CimInstance errorData = ex.ErrorData;
				if (errorData != null)
				{
					uint? num = null;
					string text = null;
					ClusterWmiErrorType? clusterWmiErrorType = null;
					if (ex.NativeErrorCode == NativeErrorCode.Failed)
					{
						try
						{
							CimProperty cimProperty = errorData.CimInstanceProperties["Error_Code"];
							if (cimProperty.Value != null)
							{
								num = (uint)cimProperty.Value;
							}
						}
						catch (CimException exception2)
						{
							ClusterLog.LogException(exception2, "There was an error getting the Status Code property");
						}
						try
						{
							CimProperty cimProperty = errorData.CimInstanceProperties["ErrorType"];
							if (cimProperty.Value != null)
							{
								clusterWmiErrorType = (ClusterWmiErrorType)(ushort)cimProperty.Value;
							}
						}
						catch (CimException exception3)
						{
							ClusterLog.LogException(exception3, "There was an error getting the Error Type property");
						}
						try
						{
							CimProperty cimProperty = errorData.CimInstanceProperties["Message"];
							if (cimProperty.Value != null && !string.IsNullOrWhiteSpace((string)cimProperty.Value))
							{
								text = (string)cimProperty.Value;
							}
						}
						catch (CimException exception4)
						{
							ClusterLog.LogException(exception4, "There was an error getting the Description property");
						}
						try
						{
							CimProperty cimProperty = errorData.CimInstanceProperties["CIMStatusCodeDescription"];
							if (cimProperty.Value != null)
							{
								text = (string)cimProperty.Value;
							}
						}
						catch (CimException exception5)
						{
							ClusterLog.LogException(exception5, "There was an error getting the Description property");
						}
					}
					if (!num.HasValue)
					{
						return new ClusterDefaultException(new ClusterWmiWin32Exception(ex.HResult, ex.Message, ex.StackTrace));
					}
					if (NativeMethods.ErrorCode.IOPending.IsEqual((int)num.Value))
					{
						return null;
					}
					ClusterWmiWin32Exception innerException = ((clusterWmiErrorType != ClusterWmiErrorType.VirtualMachine) ? ((text != null) ? new ClusterWmiWin32Exception((int)num.Value, text, ex.StackTrace) : new ClusterWmiWin32Exception((int)num.Value, ex.StackTrace)) : new ClusterWmiWin32Exception(text));
					switch ((NativeMethods.ErrorCode)(num & 0xFFFF).Value)
					{
					case NativeMethods.ErrorCode.ResourcePropertiesStored:
						throw new ClusterResourcePropertyStoredException(objectName);
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
					return new ClusterDefaultException(innerException);
				}
				return new ClusterDefaultException(new ClusterWmiWin32Exception(ex.HResult, ex.Message, ex.StackTrace));
			}
			if (exception is COMException ex2)
			{
				ClusterWmiWin32Exception innerException2 = new ClusterWmiWin32Exception(ex2.ErrorCode, exception.StackTrace);
				if (NativeMethods.HRESULT_FROM_WIN32(ex2.ErrorCode) == NativeMethods.HRESULT_FROM_WIN32(NativeMethods.ErrorCode.ResourcePropertiesStored))
				{
					throw new ClusterResourcePropertyStoredException(objectName);
				}
				return new ClusterDefaultException(innerException2);
			}
			if (exception is UnauthorizedAccessException)
			{
				return new ClusterDefaultException(exception);
			}
			return null;
		}

		public void ParseProperties(ClusterPropertyCollection propertyCollection, CimInstance[] cimPropertyCollection, ClusterPropertyKind propertiesKind)
		{
			List<CimProperty> list = new List<CimProperty>();
			foreach (CimInstance obj in cimPropertyCollection)
			{
				string text = (string)obj.CimInstanceProperties["Name"].Value;
				bool flag = (bool)obj.CimInstanceProperties["ReadOnly"].Value;
				object value = obj.CimInstanceProperties["Value"].Value;
				CimFlags cimFlags = CimFlags.None;
				if (obj.CimClass.CimSystemProperties.ClassName == "MSFTCluster_ExpandStringProperty")
				{
					cimFlags |= CimFlags.EnableOverride;
				}
				if (obj.CimClass.CimSystemProperties.ClassName == "MSFTCluster_ExpandedStringProperty")
				{
					text += "_";
					cimFlags |= CimFlags.DisableOverride;
				}
				if (flag)
				{
					cimFlags |= CimFlags.ReadOnly;
				}
				CimProperty item = CimProperty.Create(text, value, cimFlags);
				list.Add(item);
			}
			ParseProperties(propertyCollection, list, propertiesKind);
		}

		public CimInstance[] ParseCimPrivateProperties(ClusterPropertyCollection propertyCollection)
		{
			List<CimInstance> cimInstances = new List<CimInstance>();
			propertyCollection.Where((ClusterProperty property) => property.PropertyKind == ClusterPropertyKind.Private && property.IsModified).ForEach(delegate(ClusterProperty modifiedProperty)
			{
				CimInstance cimInstance = null;
				if (modifiedProperty is ClusterPropertyBinary)
				{
					cimInstance = new CimInstance("MSFTCluster_BinaryProperty", "root\\microsoft\\windows\\cluster");
				}
				else if (modifiedProperty is ClusterPropertyDateTime)
				{
					cimInstance = new CimInstance("MSFTCluster_FiletimeProperty", "root\\microsoft\\windows\\cluster");
				}
				else if (modifiedProperty is ClusterPropertyExpandString)
				{
					cimInstance = new CimInstance("MSFTCluster_ExpandStringProperty", "root\\microsoft\\windows\\cluster");
				}
				else if (modifiedProperty is ClusterPropertyExpandedString)
				{
					cimInstance = new CimInstance("MSFTCluster_ExpandedStringProperty", "root\\microsoft\\windows\\cluster");
				}
				else if (modifiedProperty is ClusterPropertyUShort)
				{
					cimInstance = new CimInstance("MSFTCluster_UInt16Property", "root\\microsoft\\windows\\cluster");
				}
				else if (modifiedProperty is ClusterPropertyInt)
				{
					cimInstance = new CimInstance("MSFTCluster_Int32Property", "root\\microsoft\\windows\\cluster");
				}
				else if (modifiedProperty is ClusterPropertyUInt)
				{
					cimInstance = new CimInstance("MSFTCluster_UInt32Property", "root\\microsoft\\windows\\cluster");
				}
				else if (modifiedProperty is ClusterPropertyLong)
				{
					cimInstance = new CimInstance("MSFTCluster_Int64Property", "root\\microsoft\\windows\\cluster");
				}
				else if (modifiedProperty is ClusterPropertyULong)
				{
					cimInstance = new CimInstance("MSFTCluster_UInt64Property", "root\\microsoft\\windows\\cluster");
				}
				else if (modifiedProperty is ClusterPropertyMultipleStrings)
				{
					cimInstance = new CimInstance("MSFTCluster_StringArrayProperty", "root\\microsoft\\windows\\cluster");
				}
				else if (modifiedProperty is ClusterPropertyString)
				{
					cimInstance = new CimInstance("MSFTCluster_StringProperty", "root\\microsoft\\windows\\cluster");
				}
				if (cimInstance != null)
				{
					CimProperty newItem = CimProperty.Create("Name", modifiedProperty.RealName ?? modifiedProperty.Name, CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified);
					cimInstance.CimInstanceProperties.Add(newItem);
					CimProperty newItem2 = CimProperty.Create("ReadOnly", false, CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified);
					cimInstance.CimInstanceProperties.Add(newItem2);
					if (!modifiedProperty.IsDeleted)
					{
						CimProperty newItem3 = CimProperty.Create("Value", modifiedProperty.Value, CimFlags.Property);
						cimInstance.CimInstanceProperties.Add(newItem3);
					}
					cimInstances.Add(cimInstance);
				}
			});
			return cimInstances.ToArray();
		}

		public void ParseProperties(ClusterPropertyCollection propertyCollection, IEnumerable<CimProperty> cimPropertyCollection, ClusterPropertyKind propertiesKind)
		{
			string resourceName = null;
			string resourceType = null;
			ParseProperties(propertyCollection, cimPropertyCollection, propertiesKind, ref resourceName, ref resourceType);
		}

		public static void ParseProperties(ClusterPropertyCollection propertyCollection, IEnumerable<CimProperty> wmiPropertyCollection, ClusterPropertyKind propertyKind, ref string resourceName, ref string resourceType)
		{
			propertyCollection.Remove(propertyKind);
			foreach (CimProperty item in wmiPropertyCollection)
			{
				if (item.Flags.HasFlag(CimFlags.NullValue))
				{
					continue;
				}
				if (item.Name.Equals("Name", StringComparison.OrdinalIgnoreCase))
				{
					resourceName = (string)item.Value;
				}
				if (item.Name.Equals("type", StringComparison.OrdinalIgnoreCase) && item.Value is string)
				{
					resourceType = (string)item.Value;
				}
				bool num = item.Flags.HasFlag(CimFlags.Property);
				bool readOnly = item.Flags.HasFlag(CimFlags.ReadOnly);
				string name = item.Name.Replace('_', ' ');
				if (!num && propertyKind != ClusterPropertyKind.Private)
				{
					continue;
				}
				try
				{
					ClusterProperty clusterProperty = null;
					switch (item.CimType)
					{
					case CimType.Boolean:
						clusterProperty = new ClusterPropertyUInt(name, item.Name, propertyKind, readOnly, ((bool)(item.Value ?? ((object)0))) ? 1u : 0u);
						break;
					case CimType.SInt32:
						clusterProperty = new ClusterPropertyInt(name, item.Name, propertyKind, readOnly, (int)(item.Value ?? ((object)0)));
						break;
					case CimType.UInt32:
						clusterProperty = new ClusterPropertyUInt(name, item.Name, propertyKind, readOnly, (uint)(item.Value ?? ((object)0u)));
						break;
					case CimType.DateTime:
						clusterProperty = new ClusterPropertyDateTime(name, item.Name, propertyKind, readOnly, (DateTime)(item.Value ?? ((object)DateTime.MinValue)));
						break;
					case CimType.String:
						clusterProperty = ((!item.Flags.HasFlag(CimFlags.EnableOverride)) ? ((!item.Flags.HasFlag(CimFlags.DisableOverride)) ? ((ClusterProperty)new ClusterPropertyString(name, item.Name, propertyKind, readOnly, (string)item.Value)) : ((ClusterProperty)new ClusterPropertyExpandedString(name, item.Name, propertyKind, readOnly, (string)item.Value))) : new ClusterPropertyExpandString(name, item.Name, propertyKind, readOnly, (string)item.Value));
						break;
					case CimType.StringArray:
						clusterProperty = new ClusterPropertyMultipleStrings(name, item.Name, propertyKind, readOnly, (string[])item.Value);
						break;
					case CimType.UInt8Array:
						clusterProperty = new ClusterPropertyBinary(name, item.Name, propertyKind, readOnly, (byte[])item.Value);
						break;
					case CimType.UInt64:
						clusterProperty = new ClusterPropertyULong(name, item.Name, propertyKind, readOnly, (ulong)(item.Value ?? ((object)0uL)));
						break;
					case CimType.Reference:
						ClusterLog.LogError("Cim Property type CimType.Reference is not supported".FormatCurrentCulture(item.ToString()));
						break;
					default:
						ClusterLog.LogError("Cim Property type {0} is not supported".FormatCurrentCulture(item.ToString()));
						break;
					}
					if (clusterProperty != null)
					{
						propertyCollection.Add(clusterProperty);
					}
				}
				catch (Exception exception)
				{
					ClusterLog.LogException(exception, "Failed to parse Cluster Cim property");
					throw;
				}
			}
			switch (propertyKind)
			{
			case ClusterPropertyKind.Common:
				propertyCollection.CommonPropertiesLoaded = true;
				break;
			case ClusterPropertyKind.Private:
				propertyCollection.PrivatePropertiesLoaded = true;
				break;
			}
		}

		public List<string> NormalizeQuery(IEnumerable<string> fieldsString, out int loadSelection)
		{
			loadSelection = 0;
			List<string> list = new List<string>();
			foreach (string item in fieldsString)
			{
				if (BasicPayloadQuery.Contains("," + item.ToLowerInvariant()))
				{
					loadSelection |= 1;
				}
				if (CommonPropertiesQuery.Contains("," + item.ToLowerInvariant()))
				{
					loadSelection |= 2;
				}
			}
			list.AddRange(ElementaryPayloadQuery.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			if ((loadSelection & 1) == 1)
			{
				list.AddRange(BasicPayloadQuery.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			}
			return list;
		}

		public T InvokeMethod<T>(string elementName, string methodName, IEnumerable<CimArgumentNameValue> parameters, Func<CimMethodResult, T> action)
		{
			return GetInstanceIdentity(elementName, (CimInstance cimInstance) => CimAdapter.InvokeMethod(cimInstance, methodName, parameters, action));
		}

		public void InvokeMethod(string elementName, string methodName, IEnumerable<CimArgumentNameValue> parameters)
		{
			GetInstanceIdentity(elementName, delegate(CimInstance cimInstance)
			{
				CimAdapter.InvokeMethod(cimInstance, methodName, parameters, null);
			});
		}

		public void InvokeMethod(CimClass cimClass, string methodName, IEnumerable<CimArgumentNameValue> parameters, Action<CimMethodResult> action, string cimNamespace = "root\\microsoft\\windows\\cluster")
		{
			Func<CimMethodResult, object> action2 = delegate(CimMethodResult instance)
			{
				action.SafeCall(instance);
				return null;
			};
			CimAdapter.InvokeMethod(CimAdapter.Scope, null, cimClass, methodName, parameters, action2, cimNamespace);
		}

		public void InvokeMethod(CimClass cimClass, string methodName, IEnumerable<CimArgumentNameValue> parameters, Action<CimMethodResult> action, CimSession session, string cimNamespace = "root\\microsoft\\windows\\cluster", TimeSpan? timeout = null)
		{
			Func<CimMethodResult, object> action2 = delegate(CimMethodResult instance)
			{
				action.SafeCall(instance);
				return null;
			};
			if (!timeout.HasValue)
			{
				timeout = TimeSpan.FromMinutes(1.0);
			}
			CimAdapter.InvokeMethod(session, null, cimClass, methodName, parameters, action2, cimNamespace, timeout);
		}

		public T InvokeMethod<T>(CimClass cimClass, string methodName, IEnumerable<CimArgumentNameValue> parameters, Func<CimMethodResult, T> action)
		{
			return CimAdapter.InvokeMethod(CimAdapter.Scope, null, cimClass, methodName, parameters, action);
		}

		public void GetInstance(string elementName, Action<CimInstance> action)
		{
			Func<CimInstance, object> action2 = delegate(CimInstance instance)
			{
				action.SafeCall(instance);
				return null;
			};
			GetInstance(elementName, action2);
		}

		public T GetInstance<T>(string elementName, Func<CimInstance, T> action)
		{
			return GetInstance(IdentityClass, elementName, (CimInstance cimInstance) => action.SafeCall(cimInstance));
		}

		public T GetInstance<T>(CimClass cimClass, string keyValue, Func<CimInstance, T> action)
		{
			Exceptions.ThrowIfNull(cimClass, "cimClass");
			Exceptions.ThrowIfNull(keyValue, "keyValue");
			return GetInstanceIdentity(cimClass, keyValue, (CimInstance cimInstance) => CimAdapter.GetInstance(cimInstance, action));
		}

		public void GetInstance(CimClass cimClass, string keyValue, Action<CimInstance> action)
		{
			GetInstanceIdentity(cimClass, keyValue, delegate(CimInstance cimInstance)
			{
				CimAdapter.GetInstance(cimInstance, action);
			});
		}

		public void GetInstanceIdentity(string elementName, Action<CimInstance> action)
		{
			Func<CimInstance, object> action2 = delegate(CimInstance instance)
			{
				action.SafeCall(instance);
				return null;
			};
			GetInstanceIdentity(elementName, action2);
		}

		public T GetInstanceIdentity<T>(string elementName, Func<CimInstance, T> action)
		{
			return GetInstanceIdentity(IdentityClass, elementName, (CimInstance cimInstance) => action.SafeCall(cimInstance));
		}

		public void GetInstanceIdentity(CimClass cimClass, IEnumerable<string> keyValues, Action<IEnumerable<CimInstance>> action)
		{
			Exceptions.ThrowIfNull(cimClass, "cimClass");
			Exceptions.ThrowIfNull(action, "action");
			Func<IEnumerable<CimInstance>, object> action2 = delegate(IEnumerable<CimInstance> instances)
			{
				action(instances);
				return null;
			};
			GetInstanceIdentity(cimClass, keyValues, action2);
		}

		private T GetInstanceIdentity<T>(CimClass cimClass, IEnumerable<string> keyValues, Func<IEnumerable<CimInstance>, T> action)
		{
			Exceptions.ThrowIfNull(cimClass, "cimClass");
			Exceptions.ThrowIfNull(action, "action");
			if (keyValues == null)
			{
				return action(null);
			}
			return ExecuteAndCatchWmiExceptions(delegate
			{
				IList<CimInstance> list = keyValues.Select(delegate(string keyValue)
				{
					CimInstance cimInstance = new CimInstance(cimClass);
					cimInstance.CimInstanceProperties["ElementName"].Value = keyValue;
					ClusterLog.LogInfo("Instance created: {0}", cimInstance.ToString());
					return cimInstance;
				}).ToList();
				try
				{
					return action(list);
				}
				finally
				{
					list.ForEach(delegate(CimInstance instance)
					{
						instance.Dispose();
					});
				}
			}, keyValues.ToString());
		}

		public void GetInstanceIdentity(CimClass cimClass, PClusterObject clusterObject, Action<CimInstance> action)
		{
			Func<CimInstance, object> action2 = delegate(CimInstance instance)
			{
				action.SafeCall(instance);
				return null;
			};
			GetInstanceIdentity(cimClass, clusterObject, action2);
		}

		public T GetInstanceIdentity<T>(CimClass cimClass, PClusterObject clusterObject, Func<CimInstance, T> action)
		{
			Exceptions.ThrowIfNull(clusterObject, "clusterObject");
			if (clusterObject is PNode || clusterObject is PResourceType)
			{
				return GetInstanceIdentity(cimClass, clusterObject.Name, action);
			}
			return GetInstanceIdentity(cimClass, clusterObject.Id.ToString(), action);
		}

		public void GetInstanceIdentity(CimClass cimClass, string keyValue, Action<CimInstance> action)
		{
			Exceptions.ThrowIfNull(cimClass, "cimClass");
			Func<CimInstance, object> action2 = delegate(CimInstance instance)
			{
				action.SafeCall(instance);
				return null;
			};
			GetInstanceIdentity(cimClass, keyValue, action2);
		}

		public T GetInstanceIdentity<T>(CimClass cimClass, string keyValue, Func<CimInstance, T> action)
		{
			Exceptions.ThrowIfNull(cimClass, "cimClass");
			Exceptions.ThrowIfNull(action, "action");
			return ExecuteAndCatchWmiExceptions(delegate
			{
				if (keyValue == null)
				{
					return action(null);
				}
				using CimInstance cimInstance = new CimInstance(cimClass);
				cimInstance.CimInstanceProperties["ElementName"].Value = keyValue;
				ClusterLog.LogInfo("Instance created: {0}", cimInstance.ToString());
				return action(cimInstance);
			}, keyValue);
		}

		public virtual void Collect()
		{
		}
	}

	private class ClusterAdapter : AdapterBase, IConnectionAdapterCluster
	{
		private readonly PCluster memberCluster;

		private string connectedTo;

		internal PCluster Cluster => memberCluster;

		public override CimClass IdentityClass => base.CimCluster;

		public IEnumerable<Guid> CoreGroups
		{
			get
			{
				throw new NotSupportedException("CoreGroups is not implemented for CimAdapter");
			}
		}

		public ClusterAdapter(CimAdapter cimAdapter, PCluster cluster)
			: base(cimAdapter)
		{
			memberCluster = cluster;
		}

		public Guid Open(string clusterName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess)
		{
			try
			{
				ExecuteAndCatchWmiExceptions(delegate
				{
					ClusterLog.LogInfo("Creating CimSession to '{0}'", clusterName);
					using (DComSessionOptions dComSessionOptions = new DComSessionOptions())
					{
						dComSessionOptions.PacketPrivacy = true;
						dComSessionOptions.PacketIntegrity = true;
						ClusterLog.LogInfo("DComSessionOptions:: PacketPrivacy:{0} PacketIntegrity:{1}", dComSessionOptions.PacketPrivacy, dComSessionOptions.PacketIntegrity);
						base.CimAdapter.CreateSession(clusterName, dComSessionOptions);
					}
					base.CimAdapter.TestConnection();
					base.CimAdapter.SetupEnvironment();
				}, clusterName);
				connectedTo = clusterName;
				grantedAccess = desiredAccess;
				return Guid.NewGuid();
			}
			catch (ClusterException innerException)
			{
				throw new ClusterOpenClusterException(innerException);
			}
		}

		public Guid Open(SafeClusterHandle handle)
		{
			throw new NotSupportedException("Open using Safehandle is not implemented for CimAdapter");
		}

		public void Close()
		{
		}

		public void Load(PCluster cluster, ClusterLoadSelection loadSelection)
		{
			if ((loadSelection & ClusterLoadSelection.PrivateProperties) == ClusterLoadSelection.PrivateProperties)
			{
				base.CimAdapter.GetPrivateProperties(cluster);
			}
		}

		public void Rename(string newName)
		{
			throw new NotSupportedException("Rename is not implemented for CimAdapter");
		}

		public string GetConnectedToNode()
		{
			return connectedTo;
		}

		public override void Collect()
		{
		}

		public void AddVirtualMachine(Guid vmId, string ownerNodeName)
		{
			throw new NotSupportedException("AddVirtualMachine is not implemented for CimAdapter");
		}

		public IEnumerable<PResource> CreateDiskResources(IEnumerable<ClusterDisk> clusterableDisks)
		{
			return clusterableDisks.Select((ClusterDisk stackClusterableDisk) => GetInstanceIdentity(base.CimAdapter.CimStorageService, memberCluster, (CimInstance cimInstance) => GetInstanceIdentity(base.CimAdapter.CimDiskInfo, stackClusterableDisk.DiskId, delegate(CimInstance diskInfoInstance)
			{
				CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("DiskInfo", diskInfoInstance);
				CimArgumentNameValue cimArgumentNameValue2 = new CimArgumentNameValue("StorageResourceName", stackClusterableDisk.Name);
				return base.CimAdapter.InvokeMethod(cimInstance, "AddDiskToCluster", new CimArgumentNameValue[2] { cimArgumentNameValue, cimArgumentNameValue2 }, delegate(CimMethodResult returnInstance)
				{
					CimMethodParameter cimMethodParameter = returnInstance.OutParameters["StorageResource"];
					if (cimMethodParameter == null)
					{
						throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("StorageResource"));
					}
					return base.CimAdapter.resources.CreateFromCimInstance((CimInstance)cimMethodParameter.Value, ResourceLoadSelection.Basic | ResourceLoadSelection.CommonProperties);
				});
			})));
		}

		public IEnumerable<ClusterDisk> GetAvailableDisks(Guid poolId)
		{
			return GetAvailableDisks(poolId, all: false);
		}

		public IEnumerable<ClusterDisk> GetAvailableDisks(Guid poolId, bool all)
		{
			CimArgumentNameValue flagsParameter = new CimArgumentNameValue("Flags", 1);
			return GetInstanceIdentity(base.CimAdapter.CimStorageService, memberCluster, (CimInstance cimInstance) => base.CimAdapter.InvokeMethod(cimInstance, "GetAvailableDisks", new CimArgumentNameValue[1] { flagsParameter }, delegate(CimMethodResult returnInstance)
			{
				CimMethodParameter cimMethodParameter = returnInstance.OutParameters["AvailableDisks"];
				if (cimMethodParameter == null)
				{
					return new ClusterDisk[0];
				}
				CimInstance[] array = (CimInstance[])cimMethodParameter.Value;
				if (array.Length == 0)
				{
					return new ClusterDisk[0];
				}
				Dictionary<string, Guid> resourceNameId = base.CimAdapter.resources.GetNameId();
				return ((IList<CimInstance>)array).ConvertAll((Converter<CimInstance, ClusterDisk>)((CimInstance instance) => ParseDisk(instance, resourceNameId, includeMountPoints: false)));
			}));
		}

		private ClusterDisk ParseDisk(CimInstance diskInfoInstance, Dictionary<string, Guid> resourceNameId, bool includeMountPoints)
		{
			if (!diskInfoInstance.CimSystemProperties.ClassName.Equals("MSFTCluster_DiskInfo"))
			{
				throw new InvalidOperationException(ExceptionResources.OutArgumentInvalidReference.FormatCurrentCulture("MSFTCluster_StorageService"));
			}
			CimKeyedCollection<CimProperty> cimInstanceProperties = diskInfoInstance.CimInstanceProperties;
			object value = cimInstanceProperties["GptGuid"].Value;
			ClusterDisk clusterDisk = ((value != null) ? new ClusterDisk(new Guid((string)value)) : new ClusterDisk((uint)cimInstanceProperties["Signature"].Value));
			clusterDisk.DiskNumber = (uint)cimInstanceProperties["Number"].Value;
			clusterDisk.Size = (ulong)cimInstanceProperties["Size"].Value;
			int num = 1;
			string text;
			do
			{
				text = CommonResources.DiskNameFormatter.FormatCurrentCulture(num++);
			}
			while (resourceNameId.ContainsKey(text));
			clusterDisk.Name = text;
			resourceNameId.Add(text, Guid.Empty);
			CimInstance[] array = (CimInstance[])cimInstanceProperties["Volumes"].Value;
			foreach (CimInstance obj in array)
			{
				ClusterDiskPartition clusterDiskPartition = new ClusterDiskPartition(clusterDisk);
				CimKeyedCollection<CimProperty> cimInstanceProperties2 = obj.CimInstanceProperties;
				clusterDiskPartition.Name = (string)cimInstanceProperties2["Path"].Value;
				clusterDiskPartition.DeviceNumber = (uint)cimInstanceProperties2["DeviceNumber"].Value;
				clusterDiskPartition.FileSystem = (string)cimInstanceProperties2["FileSystem"].Value;
				clusterDiskPartition.Flags = (uint)cimInstanceProperties2["Flags"].Value;
				clusterDiskPartition.IsCompressed = (clusterDiskPartition.Flags & 0x8000) != 0;
				clusterDiskPartition.FreeSpace = (uint)cimInstanceProperties2["FreeSpace"].Value;
				clusterDiskPartition.PartitionNumber = (uint)cimInstanceProperties2["PartitionNumber"].Value;
				clusterDiskPartition.SerialNumber = (uint)cimInstanceProperties2["SerialNumber"].Value;
				clusterDiskPartition.Size = (ulong)cimInstanceProperties2["Size"].Value;
				clusterDiskPartition.VolumeGuid = new Guid((string)cimInstanceProperties2["ElementName"].Value);
				clusterDiskPartition.Label = (string)cimInstanceProperties2["VolumeLabel"].Value;
				clusterDiskPartition.IncludeMountPoints = includeMountPoints;
				clusterDiskPartition.IsMaintenanceModeOn = null;
				clusterDiskPartition.CsvFaultState = ClusterSharedVolumeFaultState.NoFaults;
				ClusterPropertyString clusterSharedVolumeRoot = (ClusterPropertyString)memberCluster.Properties["SharedVolumesRoot"];
				if (clusterDiskPartition.FileSystem.Equals("CSVFS", StringComparison.OrdinalIgnoreCase) && clusterDiskPartition.MountPoints.Count >= 1)
				{
					string text2 = string.Empty;
					if (!string.IsNullOrEmpty(clusterSharedVolumeRoot.TypedValue))
					{
						text2 = clusterDiskPartition.MountPoints.FirstOrDefault((string mp) => mp.StartsWith(clusterSharedVolumeRoot.TypedValue, StringComparison.OrdinalIgnoreCase));
					}
					clusterDiskPartition.Name = (string.IsNullOrEmpty(text2) ? clusterDiskPartition.MountPoints[0] : text2);
				}
				clusterDisk.Partitions.Add(clusterDiskPartition);
			}
			return clusterDisk;
		}

		public void GetClusterableStoragePools(Action<ClusterableStoragePool> onNext, Action<Exception> onError, Action onCompleted)
		{
			throw new NotSupportedException("GetClusterableStoragePools is not implemented for CimAdapter");
		}

		public void AddStoragePoolToCluster(ClusterableStoragePool clusterableStoragePool, Action<Exception> onError, Action onCompleted)
		{
			throw new NotSupportedException("AddStoragePoolToCluster is not implemented for CimAdapter");
		}

		public bool WillVoterLossCauseQuorumLoss(QuorumVoterActionCheck voterActionCheck, string id)
		{
			throw new NotSupportedException("WillVoterLossCauseQuorumLoss is not implemented for CimAdapter");
		}

		public void Shutdown()
		{
			throw new NotSupportedException("Shutdown is not implemented for CimAdapter");
		}

		public QuorumConfigurationPrivate GetQuorumConfiguration()
		{
			QuorumConfigurationPrivate quorumConfiguration = new QuorumConfigurationPrivate
			{
				QuorumResourceId = Guid.Empty
			};
			ExecuteAndCatchWmiExceptions(delegate
			{
				base.CimAdapter.GetFirst(base.CimAdapter.CimQuorumSettings, delegate(CimInstance cimInstance)
				{
					string text = (string)cimInstance.CimInstanceProperties["QuorumWitnessResourceName"].Value;
					if (text != null)
					{
						PResource pResource = base.CimAdapter.Resource.Open(text);
						quorumConfiguration.QuorumResourceId = pResource.Id;
					}
					quorumConfiguration.QuorumType = (QuorumType)cimInstance.CimInstanceProperties["QuorumType"].Value;
				});
			}, memberCluster.Name);
			return quorumConfiguration;
		}

		public void SetQuorumConfiguration(QuorumType quorumType, WitnessType witnessType, string quorumWitness, IEnumerable<string> nonVotingNodes)
		{
			GetInstanceIdentity(base.CimNode, nonVotingNodes, delegate(IEnumerable<CimInstance> cimNonVotingNodesInstancess)
			{
				CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("QuorumType", quorumType);
				CimArgumentNameValue cimArgumentNameValue2 = new CimArgumentNameValue("WitnessType", witnessType);
				CimArgumentNameValue cimArgumentNameValue3 = new CimArgumentNameValue("Witness", quorumWitness);
				CimArgumentNameValue cimArgumentNameValue4 = new CimArgumentNameValue("NonVotingNodes", cimNonVotingNodesInstancess);
				base.CimAdapter.InvokeMethod(base.CimQuorumSettings, "SetAdvancedSettings", new CimArgumentNameValue[4] { cimArgumentNameValue, cimArgumentNameValue2, cimArgumentNameValue3, cimArgumentNameValue4 }, null);
			});
		}

		public FileShareValidationStatus VerifyFileShare(string path)
		{
			return base.CimAdapter.InvokeMethod(base.CimQuorumSettings, "VerifyFileShare", new CimArgumentNameValue[1]
			{
				new CimArgumentNameValue("FileSharePath", path)
			}, delegate(CimMethodResult result)
			{
				uint num = (uint)result.OutParameters["Result"].Value;
				if (num == (uint)NativeMethods.HRESULT_FROM_WIN32(NativeMethods.ErrorCode.AccessDenied))
				{
					return FileShareValidationStatus.CnoDeniedAccess;
				}
				return (num == (uint)NativeMethods.HRESULT_FROM_WIN32(NativeMethods.ErrorCode.FileShareResourceConflict)) ? FileShareValidationStatus.ShareServerInCluster : FileShareValidationStatus.Success;
			});
		}

		public void UpdateFunctionalLevel(bool whatIf)
		{
			GetInstanceIdentity(base.CimClusterService, memberCluster, delegate(CimInstance service)
			{
				base.CimAdapter.InvokeMethod(service, "UpdateFunctionalLevel", new CimArgumentNameValue[1]
				{
					new CimArgumentNameValue("WhatIf", whatIf)
				}, null);
			});
		}

		public void Destroy(bool deletecComputerObjects)
		{
			throw new NotSupportedException("Destroy is not implemented for CimAdapter");
		}

		public string GetFullyQualifiedDomainName()
		{
			throw new NotSupportedException("GetFullyQualifiedDomainName is not implemented for CimAdapter");
		}
	}

	internal class CimConstants
	{
		public const string ClusSvcServiceName = "ClusSvc";

		public const string Namespace = "root\\microsoft\\windows\\cluster";

		public const string PropertyName = "Name";

		public const string PropertyReadOnly = "ReadOnly";

		public const string PropertyValue = "Value";

		public const string InstanceKeyName = "ElementName";

		public const string ErrorCodeProperty = "Error_Code";

		public const string ErrorTypeProperty = "ErrorType";

		public const string MessageProperty = "Message";

		public const string StatusCodeDescription = "CIMStatusCodeDescription";

		public const string GroupElementaryPayloadQuery = "elementname,name,type";

		public const string GroupBasicPayloadQuery = ",flags,state,iscore,ownernodename,priority";

		public const string GroupCommonPropertiesQuery = ",AntiAffinityClassNames,AutoFailbackType,DefaultOwner,Description,FailbackWindowEnd,FailbackWindowStart,FailoverPeriod,FailoverThreshold,Name,PersistentState,Priority";

		public const string ResourceElementaryPayloadQuery = "elementname,name,typename";

		public const string ResourceBasicPayloadQuery = ",state,ownernodename,ownergroupname,characteristics,resourceclass,flags";

		public const string ResourceCommonPropertiesQuery = ",DeadlockTimeout,Description,IsAlivePollInterval,LooksAlivePollInterval,MonitorProcessId,Name,PendingTimeout,PersistentState,ResourceSpecificStatus,RestartAction, RestartDelay,RestartPeriod,RestartThreshold,RetryPeriodOnFailure,SeparateMonitor";

		public const string ResourceTypeElementaryPayloadQuery = "name";

		public const string ResourceTypeBasicPayloadQuery = ",resourceClass";

		public const string ResourceTypeCommonPropertiesQuery = ",DllName,Name,Description,AdminExtensions,LooksAlivePollInterval,IsAlivePollInterval,PendingTimeout,DeadlockTimeout";

		public const string EnumerateRecords = "select {0} from {1}";

		public const string EnumerateRecordsWhere = "select {0} from {1} where {2}";

		public const string NodeAllInformationQuery = "Select * from MSFTCluster_Node";

		public const string NodeBasicInformationQuery = "Select Name, NodeInstanceId, State from MSFTCluster_Node";

		public const string GroupBasicInformationQuery = "Select Name, ElementName, Type from MSFTCluster_Group";

		public const string ResourceBasicInformationQuery = "Select Name, ElementName, TypeName from MSFTCluster_Resource";

		public const string ResourceNameIdQuery = "select Name,ElementName, ElementName from MSFTCluster_Resource";

		public const string ResourceTypeBasicInformationQuery = "Select ElementName from MSFTCluster_ResourceType";

		public const string GetQuorumConfigurationQuery = "select QuorumWitnessResourceName from MSFTCluster_QuorumSettings";

		public const string GetDependenciesQuery = "select Dependent from MSFTCluster_ResourceToDependentResource where antecedent = \"MSFTCluster_resource.ElementName='{0}'\"";

		public const string ClusterSharedVolumesRootProperty = "SharedVolumesRoot";

		public const string NodeIdProperty = "ElementName";

		public const string NodeInstanceIdProperty = "NodeInstanceId";

		public const string NodeNameProperty = "Name";

		public const string NodeStateProperty = "State";

		public const string GroupTypeProperty = "Type";

		public const string GroupNameProperty = "Name";

		public const string GroupStateProperty = "State";

		public const string GroupPriorityProperty = "Priority";

		public const string GroupIsCoreProperty = "IsCore";

		public const string GroupFlagsProperty = "Flags";

		public const string GroupIdProperty = "ElementName";

		public const string GroupOwnerNodeNameProperty = "OwnerNodeName";

		public const string NetworkNameProperty = "Name";

		public const string NetworkIdProperty = "ElementName";

		public const string NetworkStateProperty = "State";

		public const string NetworkInterfaceNameProperty = "Name";

		public const string NetworkInterfaceIdProperty = "ElementName";

		public const string NetworkInterfaceStateProperty = "State";

		public const string ResourceTypeProperty = "TypeName";

		public const string ResourceNameProperty = "Name";

		public const string ResourceIdProperty = "ElementName";

		public const string ResourceCharacteristicsProperty = "Characteristics";

		public const string ResourceResourceClassProperty = "ResourceClass";

		public const string ResourceFlagsProperty = "Flags";

		public const string ResourceStateProperty = "State";

		public const string ResourceOwnerNodeNameProperty = "OwnerNodeName";

		public const string ResourceOwnerGroupNameProperty = "OwnerGroupName";

		public const string QuorumResourceNameProperty = "QuorumWitnessResourceName";

		public const string QuorumTypeProperty = "QuorumType";

		public const string ResourceTypeIdProperty = "ElementName";

		public const string ResourceTypeResourceClassProperty = "ResourceClass";

		public const string DiskInfoElementNameProperty = "ElementName";

		public const string DiskInfoGptGuidProperty = "GptGuid";

		public const string DiskInfoIsVirtualProperty = "IsVirtual";

		public const string DiskInfoNumberProperty = "Number";

		public const string DiskInfoScsiBusProperty = "ScsiBus";

		public const string DiskInfoScsiLunProperty = "ScsiLun";

		public const string DiskInfoScsiPortProperty = "ScsiPort";

		public const string DiskInfoScsiTargetIdProperty = "ScsiTargetId";

		public const string DiskInfoSignatureProperty = "Signature";

		public const string DiskInfoSizeProperty = "Size";

		public const string DiskInfoVirtualDiskIdProperty = "VirtualDiskId";

		public const string DiskInfoStoragePoolIdProperty = "StoragePoolId";

		public const string DiskInfoVolumesProperty = "Volumes";

		public const string DiskIsMaintenenceModeProperty = "IsMaintenenceModeEnabled";

		public const string VolumeInfoElementNameProperty = "ElementName";

		public const string VolumeInfoVolumeGuidProperty = "VolumeGuid";

		public const string VolumeInfoFlagsProperty = "Flags";

		public const string VolumeInfoFileSystemProperty = "FileSystem";

		public const string VolumeInfoFileSystemFlagsProperty = "FileSystemFlags";

		public const string VolumeInfoFreeSpaceProperty = "FreeSpace";

		public const string VolumeInfoMaximumComponentLengthProperty = "MaximumComponentLength";

		public const string VolumeInfoPartitionNumberProperty = "PartitionNumber";

		public const string VolumeInfoDeviceNumberProperty = "DeviceNumber";

		public const string VolumeInfoPathProperty = "Path";

		public const string VolumeInfoSerialNumberProperty = "SerialNumber";

		public const string VolumeInfoSizeProperty = "Size";

		public const string VolumeInfoVolumeLabelProperty = "VolumeLabel";

		public const string StorageElementNameProperty = "ElementName";

		public const string StorageNameProperty = "Name";

		public const string StorageIsVirtualDiskProperty = "IsVirtualDisk";

		public const string StorageIsClusterSharedVolumeProperty = "IsClusterSharedVolume";

		public const string StorageDiskTypeProperty = "DiskType";

		public const string StorageDiskSignatureProperty = "DiskSignature";

		public const string StorageDiskIdProperty = "DiskId";

		public const string SharedVolumeInfoElementNameProperty = "ElementName";

		public const string SharedVolumeInfoVolumeOffsetProperty = "VolumeOffset";

		public const string SharedVolumeInfoPartitionNumberProperty = "PartitionNumber";

		public const string SharedVolumeInfoFaultStateProperty = "SharedVolumeHealthState";

		public const string SharedVolumeInfoBackupStateProperty = "BackupState";

		public const string SharedVolumeInfoVolumeGuidProperty = "VolumeGuid";

		public const string SharedVolumeInfoVolumePathProperty = "Path";

		public const string SharedVolumeInfoRootPathProperty = "RootPath";

		public const string AntecedentProperty = "Antecedent";

		public const string DependentProperty = "Dependent";

		public const string GroupComponentProperty = "GroupComponent";

		public const string PartComponentProperty = "PartComponent";

		public const string BinaryPropertyClass = "MSFTCluster_BinaryProperty";

		public const string DateTimePropertyClass = "MSFTCluster_FiletimeProperty";

		public const string ExpandPropertyClass = "MSFTCluster_ExpandStringProperty";

		public const string ExpandedPropertyClass = "MSFTCluster_ExpandedStringProperty";

		public const string UInt16PropertyClass = "MSFTCluster_UInt16Property";

		public const string IntPropertyClass = "MSFTCluster_Int32Property";

		public const string UIntPropertyClass = "MSFTCluster_UInt32Property";

		public const string Int64PropertyClass = "MSFTCluster_Int64Property";

		public const string UInt64PropertyClass = "MSFTCluster_UInt64Property";

		public const string StringArrayPropertyClass = "MSFTCluster_StringArrayProperty";

		public const string StringPropertyClass = "MSFTCluster_StringProperty";

		public const string GroupClass = "MSFTCluster_group";

		public const string NodeClass = "MSFTCluster_node";

		public const string NetworkClass = "MSFTCluster_network";

		public const string NetworkInterfaceClass = "MSFTCluster_NetworkInterface";

		public const string ResourceClass = "MSFTCluster_resource";

		public const string ClusterClass = "MSFTCluster_cluster";

		public const string ClusterServiceClass = "MSFTCluster_ClusterService";

		public const string ResourceTypeClass = "MSFTCluster_resourcetype";

		public const string QuorumSettingsClass = "MSFTCluster_quorumSettings";

		public const string StorageServiceClass = "MSFTCluster_StorageService";

		public const string DiskInfoClass = "MSFTCluster_DiskInfo";

		public const string VolumeInfoClass = "MSFTCluster_VolumeInfo";

		public const string NetworkServiceClass = "MSFTCluster_NetworkService";

		public const string DependenciesAssociatorClass = "MSFTCluster_ResourceToDependentResource";

		public const string GetPropertiesMethod = "GetProperties";

		public const string SetPropertiesMethod = "SaveProperties";

		public const string ResourceOfflineMethod = "Offline";

		public const string ResourceOnlineMethod = "Online";

		public const string ResourceSimulateFailureMethod = "SimulateFailure";

		public const string ResourceSetDependenciesMethod = "SetDependencies";

		public const string ResourceGetPossibleOwnersMethod = "GetPossibleOwners";

		public const string ResourceTypeGetPossibleOwnersMethod = "GetPossibleOwners";

		public const string ResourceTypeDestroyMethod = "Destroy";

		public const string ResourceAddPossibleOwnerMethod = "AddPossibleOwner";

		public const string ResourceRemovePossibleOwnerMethod = "RemovePossibleOwner";

		public const string ResourceGetRegistryCheckpointsMethod = "GetRegistryCheckpoints";

		public const string ResourceGetDependeciesMethod = "GetDependencies";

		public const string ResourceGetCryptoCheckpointsMethod = "GetCryptoCheckpoints";

		public const string ResourceAddRegistryCheckpointMethod = "AddRegistryCheckpoint";

		public const string ResourceRemoveRegistryCheckpointMethod = "RemoveRegistryCheckpoint";

		public const string ResourceAddDependencyMethod = "AddDependency";

		public const string ResourceRemoveDependencyMethod = "RemoveDependency";

		public const string ResourceAddCryptoCheckpointMethod = "AddCryptoCheckpoint";

		public const string ResourceRemoveCryptoCheckpointMethod = "RemoveCryptoCheckpoint";

		public const string ResourceCreateMethod = "Create";

		public const string ResourceDestroyMethod = "Destroy";

		public const string ResourceSetOwnerGroupMethod = "SetOwnerGroup";

		public const string GroupOfflineMethod = "Offline";

		public const string GroupOnlineMethod = "Online";

		public const string GroupDestroyMethod = "Destroy";

		public const string GroupCreateMethod = "Create";

		public const string GroupGetPreferredOwnersMethod = "GetPreferredOwners";

		public const string GroupSetPreferredOwnersMethod = "SetPreferredOwners";

		public const string GroupMoveMethod = "Move";

		public const string NodePauseMethod = "Pause";

		public const string NodeResumeMethod = "Resume";

		public const string NodeWillOfflineLoseQuorumMethod = "WillOfflineNodeLoseQuorum";

		public const string NodeWillEvictLoseQuorumMethod = "WillEvictLoseQuorum";

		public const string StorageGetAvailableDisksMethod = "GetAvailableDisks";

		public const string StorageAddDiskToClusterMethod = "AddDiskToCluster";

		public const string StorageAddToClusterSharedVolumesMethod = "AddToClusterSharedVolumes";

		public const string StorageRemoveFromClusterSharedVolumesMethod = "RemoveFromClusterSharedVolumes";

		public const string StorageGetMountPointsMethod = "GetMountPoints";

		public const string StorageEnableSharedVolumeRedirectedAccess = "EnableSharedVolumeRedirectedAccess";

		public const string StorageDisableSharedVolumeRedirectedAccess = "DisableSharedVolumeRedirectedAccess";

		public const string StorageEnableMaintenanceMode = "EnableMaintenanceMode";

		public const string StorageDisableMaintenanceMode = "DisableMaintenanceMode";

		public const string NetworkServiceRenewMethod = "Renew";

		public const string NetworkServiceReleaseMethod = "Release";

		public const string ResourceTypeCreateMethod = "Create";

		public const string QuorumSetAdvancedSettingsMethod = "SetAdvancedSettings";

		public const string QuorumVerifyFileShareMethod = "VerifyFileShare";

		public const string UpdateFunctionalLevelMethod = "UpdateFunctionalLevel";

		public const string ResourceAddDependencyResourceArgument = "Resource";

		public const string ResourceRemoveDependencyResourceArgument = "Resource";

		public const string ResourceAddRegistryCheckpointArgument = "Checkpoint";

		public const string ResourceRemoveRegistryCheckpointArgument = "Checkpoint";

		public const string ResourceAddCryptoCheckpointArgument = "Checkpoint";

		public const string ResourceRemoveCryptoCheckpointArgument = "Checkpoint";

		public const string ResourceAddPossibleOwnerArgument = "Node";

		public const string ResourceRemovePossibleOwnerArgument = "Node";

		public const string ResourcePreferredOwnersArgument = "Nodes";

		public const string ResourceMoveNodeArgument = "Node";

		public const string ResourceMoveFlagsArgument = "Flags";

		public const string ResourceFlagsArgument = "Flags";

		public const string ResourceSetDependenciesExpressionArgument = "Expression";

		public const string ResourceResourceNameArgument = "ResourceName";

		public const string ResourceAddToSharedVolumeArgument = "StorageResource";

		public const string ResourceRemoveFromSharedVolumeArgument = "StorageResource";

		public const string ResourceResourceArgument = "Resource";

		public const string ResourceResourceTypeArgument = "ResourceType";

		public const string ResourceGroupArgument = "Group";

		public const string ResourceSeparateMonitorArgument = "SeparateMonitor";

		public const string ResourceCleanupArgument = "Cleanup";

		public const string ResourceSetOwnerGroupGroupArgument = "Group";

		public const string ResoruceTypeCreateNameArgument = "Name";

		public const string ResoruceTypeCreateDisplayNameArgument = "DisplayName";

		public const string ResoruceTypeCreateDllArgument = "ResourceTypeDllFilePath";

		public const string GroupFlagsArgument = "Flags";

		public const string GroupCleanupArgument = "Cleanup";

		public const string GroupForceArgument = "Force";

		public const string GroupNameArgument = "Name";

		public const string GroupTypeArgument = "GroupType";

		public const string NodeDrainTypeArgument = "DrainType";

		public const string NodeTargetNodeArgument = "TargetNode";

		public const string NodeFailbackTypeArgument = "FailbackType";

		public const string NodeOfflineLoseQuorumArgument = "Node";

		public const string NodeEvictLoseQuorumArgument = "Node";

		public const string StorageGetAvailableDisksFlagsArgument = "Flags";

		public const string StorageGetMountPointsArgument = "Volume";

		public const string StorageSharedVolumeRedirectedAccessArgument = "SharedVolume";

		public const string StorageAddDiskToClusterDiskInfoArgument = "DiskInfo";

		public const string StorageAddDiskToClusterStorageResourceNameArgument = "StorageResourceName";

		public const string StorageMaintenanceModeArgument = "DiskInfo";

		public const string QuorumQuorumTypeArgument = "QuorumType";

		public const string QuorumWitnessTypeArgument = "WitnessType";

		public const string QuorumWitnessArgument = "Witness";

		public const string QuorumNonVotingNodesArgument = "NonVotingNodes";

		public const string QuorumFileSharePathArgument = "FileSharePath";

		public const string PropertiesMethodResult = "Properties";

		public const string TaskMethodResult = "Task";

		public const string ResourcePossibleOwnersMethodResult = "PossibleOwners";

		public const string ResourceTypePossibleOwnersMethodResult = "PossibleOwners";

		public const string ResourceCreateMethodResult = "Resource";

		public const string ResourceGetRegistryCheckpointsMethodResult = "Checkpoints";

		public const string ResourceGetDependeciesMethodResult = "Expression";

		public const string ResourceGetCryptoCheckpointsMethodResult = "Checkpoints";

		public const string GroupCreateMethodResult = "Group";

		public const string GroupPreferredOwners = "Nodes";

		public const string NodeOfflineLoseQuorumMethodResult = "OfflineLoseQuorum";

		public const string NodeEvictLoseQuorumMethodResult = "EvictLoseQuorum";

		public const string StorageAvailableDisksMethodResult = "AvailableDisks";

		public const string StorageAddDiskToClusterMethodResult = "StorageResource";

		public const string StorageGetMountPointsMethodResult = "MountPoints";

		public const string ResourceTypeCreateMethodResult = "ResourceType";
	}

	private class GroupAdapter : AdapterBase, IConnectionAdapterGroup
	{
		public override CimClass IdentityClass => base.CimGroup;

		public override string ElementaryPayloadQuery => "elementname,name,type";

		public override string BasicPayloadQuery => ",flags,state,iscore,ownernodename,priority";

		public override string CommonPropertiesQuery => ",AntiAffinityClassNames,AutoFailbackType,DefaultOwner,Description,FailbackWindowEnd,FailbackWindowStart,FailoverPeriod,FailoverThreshold,Name,PersistentState,Priority";

		public GroupAdapter(CimAdapter cimAdapter)
			: base(cimAdapter)
		{
		}

		public PGroup Open(Guid id)
		{
			return GetInstance(id.ToString(), (CimInstance instance) => CreateFromCimInstance(instance, GroupLoadSelection.Basic | GroupLoadSelection.CommonProperties));
		}

		public PGroup Open(string name)
		{
			return GetInstance(name, (CimInstance instance) => CreateFromCimInstance(instance, GroupLoadSelection.Basic | GroupLoadSelection.CommonProperties));
		}

		public void Close(Guid id)
		{
		}

		public void Close(string name)
		{
		}

		public void Delete(Guid id, bool force, bool cleanup)
		{
			GetInstanceIdentity(id.ToString(), delegate(CimInstance cimInstance)
			{
				CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Cleanup", cleanup);
				CimArgumentNameValue cimArgumentNameValue2 = new CimArgumentNameValue("Force", force);
				base.CimAdapter.InvokeMethod(cimInstance, "Destroy", new CimArgumentNameValue[2] { cimArgumentNameValue, cimArgumentNameValue2 }, null);
			});
		}

		public PGroup Create(string name, GroupType groupType)
		{
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Name", name);
			CimArgumentNameValue cimArgumentNameValue2 = new CimArgumentNameValue("GroupType", (uint)groupType);
			return InvokeMethod(base.CimAdapter.CimGroup, "Create", new CimArgumentNameValue[2] { cimArgumentNameValue, cimArgumentNameValue2 }, delegate(CimMethodResult returnInstance)
			{
				CimInstance cimInstance = (CimInstance)(returnInstance.OutParameters["Group"] ?? throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("Group"))).Value;
				return Open((string)cimInstance.CimInstanceProperties["ElementName"].Value);
			});
		}

		public void CancelOperation(Guid id)
		{
			ClusterLog.LogWarning("Cancel operation for a group was called, However Wmi adapter doesn't support this method.");
		}

		public void Rename(Guid id, string newName)
		{
			GetInstanceIdentity(id.ToString(), delegate(CimInstance cimInstance)
			{
				cimInstance.CimInstanceProperties["Name"].Value = newName;
				base.CimAdapter.ModifyInstance(cimInstance);
			});
		}

		public void SetPriority(Guid id, Priority priority)
		{
			throw new NotSupportedException("SetPriority is not implemented for CimAdapter");
		}

		public void Online(Guid id, bool overrideLockState = false, bool chooseBestNode = false)
		{
			InvokeMethod(id.ToString(), "Online", null);
		}

		public void Offline(Guid id, bool overrideLockState = false)
		{
			uint num = (overrideLockState ? 1u : 0u);
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Flags", num);
			InvokeMethod(id.ToString(), "Offline", new CimArgumentNameValue[1] { cimArgumentNameValue });
		}

		public IEnumerable<string> GetPreferredOwners(Guid id)
		{
			return InvokeMethod(id.ToString(), "GetPreferredOwners", null, (CimMethodResult returnInstance) => ((CimInstance[])(returnInstance.OutParameters["Nodes"] ?? throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("Nodes"))).Value).Select((CimInstance instance) => (string)instance.CimInstanceProperties["Name"].Value));
		}

		public void SetPreferredOwners(Guid id, IEnumerable<string> nodes)
		{
			GetInstanceIdentity(id.ToString(), delegate(CimInstance cimInstance)
			{
				GetInstanceIdentity(base.CimNode, nodes, delegate(IEnumerable<CimInstance> nodeInstances)
				{
					CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Nodes", nodeInstances.ToArray());
					base.CimAdapter.InvokeMethod(cimInstance, "SetPreferredOwners", new CimArgumentNameValue[1] { cimArgumentNameValue }, null);
				});
			});
		}

		public void Move(Guid id, string nodeName, bool overrideLockState = false)
		{
			CimArgumentNameValue overrideLockParameter = new CimArgumentNameValue("Flags", overrideLockState ? 1u : 0u);
			GetInstanceIdentity(base.CimGroup, id.ToString(), delegate(CimInstance groupInstance)
			{
				if (nodeName != null)
				{
					GetInstanceIdentity(base.CimNode, nodeName, delegate(CimInstance nodeInstance)
					{
						CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Node", nodeInstance);
						base.CimAdapter.InvokeMethod(groupInstance, "Move", new CimArgumentNameValue[2] { cimArgumentNameValue, overrideLockParameter });
					});
				}
				else
				{
					base.CimAdapter.InvokeMethod(groupInstance, "Move", new CimArgumentNameValue[1] { overrideLockParameter });
				}
			});
		}

		public IEnumerable<PGroup> GetAll(bool nullElementOnError)
		{
			return from instance in base.CimAdapter.ExecuteOptimalQuery("Select Name, ElementName, Type from MSFTCluster_Group", nullElementOnError)
				select CreateFromCimInstance(instance, GroupLoadSelection.None);
		}

		internal PGroup CreateFromCimInstance(CimInstance instance, GroupLoadSelection loadSelection)
		{
			if (instance == null)
			{
				return null;
			}
			GroupType groupType = (GroupType)(uint)instance.CimInstanceProperties["Type"].Value;
			string name = (string)instance.CimInstanceProperties["Name"].Value;
			PGroup pGroup = PGroup.Constructor(id: new Guid((string)instance.CimInstanceProperties["ElementName"].Value), cluster: base.CimAdapter.clusters.Cluster, name: name, groupType: groupType);
			PopulateFromCimInstance(instance, pGroup, loadSelection);
			return pGroup;
		}

		private void PopulateFromCimInstance(CimInstance instance, PGroup group, GroupLoadSelection loadSelection)
		{
			Exceptions.ThrowIfNull(instance, "instance");
			Exceptions.ThrowIfNull(group, "group");
			ExecuteAndCatchWmiExceptions(delegate
			{
				if ((loadSelection & GroupLoadSelection.CommonProperties) == GroupLoadSelection.CommonProperties || (loadSelection & GroupLoadSelection.Basic) == GroupLoadSelection.Basic)
				{
					ParseProperties(group.Properties, instance.CimInstanceProperties, ClusterPropertyKind.Common);
					group.GroupState = (GroupState)(uint)instance.CimInstanceProperties["State"].Value;
					group.Priority = (Priority)(uint)instance.CimInstanceProperties["Priority"].Value;
					string name = (string)instance.CimInstanceProperties["OwnerNodeName"].Value;
					PNode ownerNode = base.CimAdapter.nodes.Open(name);
					group.OwnerNode = ownerNode;
					group.IsCore = (bool)instance.CimInstanceProperties["IsCore"].Value;
					group.Flags = (GroupFlags)(uint)instance.CimInstanceProperties["Flags"].Value;
					group.LoadedSelection |= 3;
				}
			}, group.Name);
		}

		public void Load(PGroup group, GroupLoadSelection loadSelection)
		{
			try
			{
				ExecuteAndCatchWmiExceptions(delegate
				{
					if ((loadSelection & GroupLoadSelection.CommonProperties) == GroupLoadSelection.CommonProperties || (loadSelection & GroupLoadSelection.Basic) == GroupLoadSelection.Basic)
					{
						GetInstance(base.CimGroup, group.Id.ToString(), delegate(CimInstance instance)
						{
							PopulateFromCimInstance(instance, group, loadSelection);
						});
					}
					if ((loadSelection & GroupLoadSelection.PrivateProperties) == GroupLoadSelection.PrivateProperties)
					{
						base.CimAdapter.GetPrivateProperties(group);
					}
				}, group.Name);
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(group.Name, group.Id, innerException);
			}
		}

		public void MigrateVirtualMachine(PVirtualMachineGroup group, PNode node, VirtualMachineMigrationType migrationType, bool overrideLockState = false)
		{
			throw new NotSupportedException("MigrateVirtualMachine is not implemented for CimAdapter");
		}

		public override void Collect()
		{
		}
	}

	private class NetworkAdapter : AdapterBase, IConnectionAdapterNetwork
	{
		public override CimClass IdentityClass => base.CimNetwork;

		public NetworkAdapter(CimAdapter cimAdapter)
			: base(cimAdapter)
		{
		}

		public PNetwork Open(Guid id)
		{
			return GetInstance(id.ToString(), (CimInstance instance) => CreateFromCimInstance(instance, NetworkLoadSelection.Basic | NetworkLoadSelection.CommonProperties));
		}

		public PNetwork Open(string name)
		{
			return GetInstance(name, (CimInstance instance) => CreateFromCimInstance(instance, NetworkLoadSelection.Basic | NetworkLoadSelection.CommonProperties));
		}

		public void Rename(Guid id, string newName)
		{
			GetInstanceIdentity(id.ToString(), delegate(CimInstance cimInstance)
			{
				cimInstance.CimInstanceProperties["Name"].Value = newName;
				base.CimAdapter.ModifyInstance(cimInstance);
			});
		}

		internal PNetwork CreateFromCimInstance(CimInstance instance, NetworkLoadSelection loadSelection)
		{
			if (instance == null)
			{
				return null;
			}
			Guid id = new Guid((string)instance.CimInstanceProperties["ElementName"].Value);
			string name = (string)instance.CimInstanceProperties["Name"].Value;
			PNetwork pNetwork = new PNetwork(base.CimAdapter.clusters.Cluster, id, name);
			PopulateFromCimInstance(instance, pNetwork, loadSelection);
			return pNetwork;
		}

		private void PopulateFromCimInstance(CimInstance instance, PNetwork network, NetworkLoadSelection loadSelection)
		{
			Exceptions.ThrowIfNull(instance, "instance");
			Exceptions.ThrowIfNull(network, "network");
			ExecuteAndCatchWmiExceptions(delegate
			{
				if ((loadSelection & NetworkLoadSelection.CommonProperties) == NetworkLoadSelection.CommonProperties || (loadSelection & NetworkLoadSelection.Basic) == NetworkLoadSelection.Basic)
				{
					ParseProperties(network.Properties, instance.CimInstanceProperties, ClusterPropertyKind.Common);
					network.State = (NetworkState)(uint)instance.CimInstanceProperties["State"].Value;
					network.LoadedSelection |= 3;
				}
			}, network.Name);
		}

		public void Load(PNetwork network, NetworkLoadSelection loadSelection)
		{
			try
			{
				ExecuteAndCatchWmiExceptions(delegate
				{
					if ((loadSelection & NetworkLoadSelection.CommonProperties) == NetworkLoadSelection.CommonProperties || (loadSelection & NetworkLoadSelection.Basic) == NetworkLoadSelection.Basic)
					{
						GetInstance(base.CimNetwork, network.Id.ToString(), delegate(CimInstance instance)
						{
							PopulateFromCimInstance(instance, network, loadSelection);
						});
					}
					if ((loadSelection & NetworkLoadSelection.PrivateProperties) == NetworkLoadSelection.PrivateProperties)
					{
						base.CimAdapter.GetPrivateProperties(network);
					}
				}, network.Name);
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(network.Name, network.Id, innerException);
			}
		}

		public override void Collect()
		{
		}

		public IEnumerable<PNetwork> GetAll(bool nullElementOnError)
		{
			throw new NotImplementedException("CimAdapter.NetworkAdapter does not implement GetAll()");
		}
	}

	private class NetworkInterfaceAdapter : AdapterBase, IConnectionAdapterNetworkInterface
	{
		public override CimClass IdentityClass => base.CimNetworkInterface;

		public NetworkInterfaceAdapter(CimAdapter cimAdapter)
			: base(cimAdapter)
		{
		}

		public PNetworkInterface Open(Guid id)
		{
			return GetInstance(id.ToString(), (CimInstance instance) => CreateFromCimInstance(instance, NetworkInterfaceLoadSelection.Basic | NetworkInterfaceLoadSelection.CommonProperties));
		}

		public PNetworkInterface Open(string name)
		{
			return GetInstance(name, (CimInstance instance) => CreateFromCimInstance(instance, NetworkInterfaceLoadSelection.Basic | NetworkInterfaceLoadSelection.CommonProperties));
		}

		internal PNetworkInterface CreateFromCimInstance(CimInstance instance, NetworkInterfaceLoadSelection loadSelection)
		{
			if (instance == null)
			{
				return null;
			}
			Guid id = new Guid((string)instance.CimInstanceProperties["ElementName"].Value);
			string name = (string)instance.CimInstanceProperties["Name"].Value;
			PNetworkInterface pNetworkInterface = new PNetworkInterface(base.CimAdapter.clusters.Cluster, id, name);
			PopulateFromCimInstance(instance, pNetworkInterface, loadSelection);
			return pNetworkInterface;
		}

		private void PopulateFromCimInstance(CimInstance instance, PNetworkInterface networkInterface, NetworkInterfaceLoadSelection loadSelection)
		{
			Exceptions.ThrowIfNull(instance, "instance");
			Exceptions.ThrowIfNull(networkInterface, "networkInterface");
			ExecuteAndCatchWmiExceptions(delegate
			{
				if ((loadSelection & NetworkInterfaceLoadSelection.CommonProperties) == NetworkInterfaceLoadSelection.CommonProperties || (loadSelection & NetworkInterfaceLoadSelection.Basic) == NetworkInterfaceLoadSelection.Basic)
				{
					ParseProperties(networkInterface.Properties, instance.CimInstanceProperties, ClusterPropertyKind.Common);
					networkInterface.State = (NetworkInterfaceState)(uint)instance.CimInstanceProperties["State"].Value;
					networkInterface.LoadedSelection |= 3;
				}
			}, networkInterface.Name);
		}

		public void Load(PNetworkInterface networkInterface, NetworkInterfaceLoadSelection loadSelection)
		{
			try
			{
				ExecuteAndCatchWmiExceptions(delegate
				{
					if ((loadSelection & NetworkInterfaceLoadSelection.CommonProperties) == NetworkInterfaceLoadSelection.CommonProperties || (loadSelection & NetworkInterfaceLoadSelection.Basic) == NetworkInterfaceLoadSelection.Basic)
					{
						GetInstance(base.CimNetworkInterface, networkInterface.Id.ToString(), delegate(CimInstance instance)
						{
							PopulateFromCimInstance(instance, networkInterface, loadSelection);
						});
					}
					if ((loadSelection & NetworkInterfaceLoadSelection.PrivateProperties) == NetworkInterfaceLoadSelection.PrivateProperties)
					{
						base.CimAdapter.GetPrivateProperties(networkInterface);
					}
				}, networkInterface.Name);
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(networkInterface.Name, networkInterface.Id, innerException);
			}
		}

		public override void Collect()
		{
		}

		public List<string> GetNodeDnsSuffixes(string nodeName)
		{
			throw new NotSupportedException("GetNodeDnsSuffixes is not implemented for CimAdapter");
		}
	}

	private class NodeAdapter : AdapterBase, IConnectionAdapterNode
	{
		public override CimClass IdentityClass => base.CimNode;

		public NodeAdapter(CimAdapter cimAdapter)
			: base(cimAdapter)
		{
		}

		public PNode Open(Guid id)
		{
			throw new NotSupportedException("Open is not implemented for CimAdapeter");
		}

		public PNode Open(string name)
		{
			return GetInstance(name, (CimInstance instance) => CreateFromCimInstance(instance, NodeLoadSelection.Basic | NodeLoadSelection.CommonProperties));
		}

		public void Close(Guid id)
		{
		}

		public void Close(string name)
		{
		}

		public void Rename(Guid id, string newName)
		{
			throw new NotSupportedException("Rename is not implemented for CimAdapter");
		}

		public void Start(string name)
		{
			ExecuteAndCatchWmiExceptions(delegate
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
					ProcessStartStopException<ClusterStopNodeException>(name, exception);
				}
				catch (InvalidOperationException exception2)
				{
					ProcessStartStopException<ClusterStopNodeException>(name, exception2);
				}
			}, name);
		}

		public void Stop(string name)
		{
			ExecuteAndCatchWmiExceptions(delegate
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
			}, name);
		}

		public void Pause(string name, NodePauseDrainType drainType, string targetNode)
		{
			GetInstanceIdentity(name, delegate(CimInstance cimInstance)
			{
				List<CimArgumentNameValue> arguments = new List<CimArgumentNameValue>();
				uint num = 0u;
				switch (drainType)
				{
				case NodePauseDrainType.NoDrain:
					num = 0u;
					break;
				case NodePauseDrainType.Drain:
					num = 1u;
					break;
				case NodePauseDrainType.ForceDrain:
					num = 2u;
					break;
				}
				CimArgumentNameValue item = new CimArgumentNameValue("DrainType", num);
				arguments.Add(item);
				GetInstanceIdentity(base.CimNode, targetNode, delegate(CimInstance targetNodeInstance)
				{
					if (targetNodeInstance != null)
					{
						CimArgumentNameValue item2 = new CimArgumentNameValue("TargetNode", targetNodeInstance);
						arguments.Add(item2);
					}
					base.CimAdapter.InvokeMethod(cimInstance, "Pause", arguments, delegate
					{
					});
				});
			});
		}

		public void Resume(string name, NodeResumeFailbackType failbackType)
		{
			GetInstanceIdentity(name, delegate(CimInstance cimInstance)
			{
				uint num = 0u;
				switch (failbackType)
				{
				case NodeResumeFailbackType.DoNotFailbackGroups:
					num = 0u;
					break;
				case NodeResumeFailbackType.FailbackGroupsImmediately:
					num = 1u;
					break;
				case NodeResumeFailbackType.FailbackGroupsPerPolicy:
					num = 2u;
					break;
				}
				CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("FailbackType", num);
				base.CimAdapter.InvokeMethod(cimInstance, "Resume", new CimArgumentNameValue[1] { cimArgumentNameValue }, delegate
				{
				});
			});
		}

		public bool WillOfflineLoseQuorum(string name)
		{
			return GetInstanceIdentity(base.CimAdapter.CimQuorumSettings, base.CimAdapter.clusters.Cluster.Id.ToString(), (CimInstance quorumSettingsInstance) => GetInstanceIdentity(name, delegate(CimInstance nodeInstance)
			{
				CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Node", nodeInstance);
				return base.CimAdapter.InvokeMethod(quorumSettingsInstance, "WillOfflineNodeLoseQuorum", new CimArgumentNameValue[1] { cimArgumentNameValue }, (CimMethodResult returnInstance) => (bool)(returnInstance.OutParameters["OfflineLoseQuorum"] ?? throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("OfflineLoseQuorum"))).Value);
			}));
		}

		public bool WillEvictLoseQuorum(string name)
		{
			return GetInstanceIdentity(base.CimAdapter.CimQuorumSettings, base.CimAdapter.clusters.Cluster.Id.ToString(), (CimInstance quorumSettingsInstance) => GetInstanceIdentity(name, delegate(CimInstance nodeInstance)
			{
				CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Node", nodeInstance);
				return base.CimAdapter.InvokeMethod(quorumSettingsInstance, "WillEvictLoseQuorum", new CimArgumentNameValue[1] { cimArgumentNameValue }, (CimMethodResult returnInstance) => (bool)(returnInstance.OutParameters["EvictLoseQuorum"] ?? throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("EvictLoseQuorum"))).Value);
			}));
		}

		public void Delete(Guid id)
		{
		}

		public PNode Add(string name)
		{
			return null;
		}

		public IEnumerable<PNode> GetAll(bool nullElementOnError)
		{
			return from instance in base.CimAdapter.ExecuteOptimalQuery("Select Name, NodeInstanceId, State from MSFTCluster_Node", nullElementOnError)
				select CreateFromCimInstance(instance, NodeLoadSelection.None);
		}

		internal PNode CreateFromCimInstance(CimInstance instance, NodeLoadSelection loadSelection)
		{
			if (instance == null)
			{
				return null;
			}
			string name = (string)instance.CimInstanceProperties["Name"].Value;
			PNode pNode = new PNode(id: new Guid((string)instance.CimInstanceProperties["NodeInstanceId"].Value), cluster: base.CimAdapter.clusters.Cluster, name: name);
			PopulateFromCimInstance(instance, pNode, loadSelection);
			return pNode;
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

		private void PopulateFromCimInstance(CimInstance instance, PNode node, NodeLoadSelection loadSelection)
		{
			Exceptions.ThrowIfNull(node, "node");
			Exceptions.ThrowIfNull(instance, "instance");
			ExecuteAndCatchWmiExceptions(delegate
			{
				if ((loadSelection & NodeLoadSelection.CommonProperties) == NodeLoadSelection.CommonProperties || (loadSelection & NodeLoadSelection.Basic) == NodeLoadSelection.Basic)
				{
					ParseProperties(node.Properties, instance.CimInstanceProperties, ClusterPropertyKind.Common);
					node.Properties.CommonPropertiesLoaded = true;
					node.State = (NodeState)(uint)instance.CimInstanceProperties["State"].Value;
					node.LoadedSelection |= 3;
				}
			}, node.Name);
		}

		public void Load(PNode node, NodeLoadSelection loadSelection)
		{
			try
			{
				ExecuteAndCatchWmiExceptions(delegate
				{
					if ((loadSelection & NodeLoadSelection.CommonProperties) == NodeLoadSelection.CommonProperties || (loadSelection & NodeLoadSelection.Basic) == NodeLoadSelection.Basic)
					{
						GetInstance(base.CimNode, node.Name, delegate(CimInstance instance)
						{
							PopulateFromCimInstance(instance, node, loadSelection);
						});
					}
					if ((loadSelection & NodeLoadSelection.PrivateProperties) == NodeLoadSelection.PrivateProperties)
					{
						base.CimAdapter.GetPrivateProperties(node);
					}
				}, node.Name);
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(node.Name, node.Id, innerException);
			}
		}

		public NodeOperatingSystemInformation GetOperatingSystemInformation(string nodeName)
		{
			throw new NotSupportedException("NodeOperatingSystemInformation is not implemented for CimAdapter");
		}

		public string GetDomainName(string nodeName)
		{
			throw new NotSupportedException("GetDomainName is not implemented for CimAdapter");
		}

		public ServerInformation GetServerInformation(string nodeName)
		{
			throw new NotSupportedException("GetServerInformation is not implemented for CimAdapter");
		}

		public ProcessorInformation GetProcessorInformation(string nodeName)
		{
			throw new NotSupportedException("ProcessorInformation is not implemented for CimAdapter");
		}

		public override void Collect()
		{
		}
	}

	private class ResourceAdapter : AdapterBase, IConnectionAdapterResource
	{
		public override CimClass IdentityClass => base.CimResource;

		public override string ElementaryPayloadQuery => "elementname,name,typename";

		public override string BasicPayloadQuery => ",state,ownernodename,ownergroupname,characteristics,resourceclass,flags";

		public override string CommonPropertiesQuery => ",DeadlockTimeout,Description,IsAlivePollInterval,LooksAlivePollInterval,MonitorProcessId,Name,PendingTimeout,PersistentState,ResourceSpecificStatus,RestartAction, RestartDelay,RestartPeriod,RestartThreshold,RetryPeriodOnFailure,SeparateMonitor";

		public ResourceAdapter(CimAdapter cimAdapter)
			: base(cimAdapter)
		{
		}

		public PResource Open(Guid id)
		{
			return GetInstance(id.ToString(), (CimInstance instance) => CreateFromCimInstance(instance, ResourceLoadSelection.Basic | ResourceLoadSelection.CommonProperties));
		}

		public PResource Open(string name)
		{
			return GetInstance(name, (CimInstance instance) => CreateFromCimInstance(instance, ResourceLoadSelection.Basic | ResourceLoadSelection.CommonProperties));
		}

		public Dictionary<string, Guid> GetNameId()
		{
			Dictionary<string, Guid> dictionary = new Dictionary<string, Guid>();
			foreach (CimInstance item in base.CimAdapter.ExecuteOptimalQuery("select Name,ElementName, ElementName from MSFTCluster_Resource", nullElementOnError: false))
			{
				string key = (string)item.CimInstanceProperties["Name"].Value;
				Guid value = new Guid((string)item.CimInstanceProperties["ElementName"].Value);
				dictionary.Add(key, value);
			}
			return dictionary;
		}

		public void AddDependency(Guid resourceId, Guid dependencyId)
		{
			GetInstance(resourceId.ToString(), delegate(CimInstance cimInstance)
			{
				GetInstanceIdentity(base.CimAdapter.CimResource, dependencyId.ToString(), delegate(CimInstance dependencyInstance)
				{
					CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Resource", dependencyInstance);
					base.CimAdapter.InvokeMethod(cimInstance, "AddDependency", new CimArgumentNameValue[1] { cimArgumentNameValue }, null);
				});
			});
		}

		public void RemoveDependency(Guid resourceId, Guid dependencyId)
		{
			GetInstance(resourceId.ToString(), delegate(CimInstance cimInstance)
			{
				GetInstanceIdentity(base.CimAdapter.CimResource, dependencyId.ToString(), delegate(CimInstance dependencyInstance)
				{
					CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Resource", dependencyInstance);
					base.CimAdapter.InvokeMethod(cimInstance, "RemoveDependency", new CimArgumentNameValue[1] { cimArgumentNameValue }, null);
				});
			});
		}

		public void AddRegistryCheckpoint(Guid id, string checkpoint)
		{
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Checkpoint", checkpoint);
			InvokeMethod(id.ToString(), "AddRegistryCheckpoint", new CimArgumentNameValue[1] { cimArgumentNameValue });
		}

		public void RemoveRegistryCheckpoint(Guid id, string checkpoint)
		{
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Checkpoint", checkpoint);
			InvokeMethod(id.ToString(), "RemoveRegistryCheckpoint", new CimArgumentNameValue[1] { cimArgumentNameValue });
		}

		public IEnumerable<string> GetRegistryCheckpoints(Guid id)
		{
			return InvokeMethod(id.ToString(), "GetRegistryCheckpoints", null, (CimMethodResult returnInstance) => (string[])(returnInstance.OutParameters["Checkpoints"] ?? throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("Checkpoints"))).Value);
		}

		public void AddCryptoCheckpoint(Guid id, string checkpoint)
		{
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Checkpoint", checkpoint);
			InvokeMethod(id.ToString(), "AddCryptoCheckpoint", new CimArgumentNameValue[1] { cimArgumentNameValue });
		}

		public void RemoveCryptoCheckpoint(Guid id, string checkpoint)
		{
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Checkpoint", checkpoint);
			InvokeMethod(id.ToString(), "RemoveCryptoCheckpoint", new CimArgumentNameValue[1] { cimArgumentNameValue });
		}

		public IEnumerable<string> GetCryptoCheckpoints(Guid id)
		{
			return InvokeMethod(id.ToString(), "GetCryptoCheckpoints", null, (CimMethodResult returnInstance) => (string[])(returnInstance.OutParameters["Checkpoints"] ?? throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("Checkpoints"))).Value);
		}

		public void AddPossibleOwner(Guid id, string node)
		{
			GetInstanceIdentity(id.ToString(), delegate(CimInstance cimInstance)
			{
				GetInstanceIdentity(base.CimNode, node, delegate(CimInstance nodeInstance)
				{
					CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Node", nodeInstance);
					base.CimAdapter.InvokeMethod(cimInstance, "AddPossibleOwner", new CimArgumentNameValue[1] { cimArgumentNameValue }, null);
				});
			});
		}

		public void RemovePossibleOwner(Guid id, string node)
		{
			GetInstanceIdentity(id.ToString(), delegate(CimInstance cimInstance)
			{
				GetInstanceIdentity(base.CimNode, node, delegate(CimInstance nodeInstance)
				{
					CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Node", nodeInstance);
					base.CimAdapter.InvokeMethod(cimInstance, "RemovePossibleOwner", new CimArgumentNameValue[1] { cimArgumentNameValue }, null);
				});
			});
		}

		public void SetPossibleOwners(Guid id, IEnumerable<Guid> nodes)
		{
			throw new NotSupportedException("SetPossibleOwners is not implemented for CimAdapter");
		}

		public void Rename(Guid id, string newName)
		{
			GetInstanceIdentity(id.ToString(), delegate(CimInstance cimInstance)
			{
				cimInstance.CimInstanceProperties["Name"].Value = newName;
				base.CimAdapter.ModifyInstance(cimInstance);
			});
		}

		public void Online(Guid id, bool overrideLockState = false, bool chooseBestNode = false)
		{
			InvokeMethod(id.ToString(), "Online", null);
		}

		public void Offline(Guid id, bool overrideLockState = false)
		{
			uint num = (overrideLockState ? 1u : 0u);
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Flags", num);
			InvokeMethod(id.ToString(), "Offline", new CimArgumentNameValue[1] { cimArgumentNameValue });
		}

		public IEnumerable<string> GetPossibleOwners(string name)
		{
			return InvokeMethod(name, "GetPossibleOwners", null, (CimMethodResult returnInstance) => ((CimInstance[])(returnInstance.OutParameters["PossibleOwners"] ?? throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("PossibleOwners"))).Value).Select((CimInstance instance) => (string)instance.CimInstanceProperties["Name"].Value));
		}

		public void OfflineDependents(Guid id, bool overrideLockState = false)
		{
			throw new NotSupportedException("OfflineDependents is not implemented for CimAdapter");
		}

		public string GetType(Guid id, string name)
		{
			throw new NotSupportedException("GetType is not implemented for CimAdapter");
		}

		public void Fail(Guid id)
		{
			InvokeMethod(id.ToString(), "SimulateFailure", null);
		}

		public void SetDependencyRelationship(Guid id, string relationship)
		{
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Expression", relationship);
			InvokeMethod(id.ToString(), "SetDependencies", new CimArgumentNameValue[1] { cimArgumentNameValue });
		}

		public void FetchVirtualPropertiesPayload(ClusterPropertiesEventArgs propertiesPayload)
		{
			throw new NotSupportedException("SetDependencyRelationship is not implemented for CimAdapter");
		}

		internal PResource CreateFromCimInstance(CimInstance instance, ResourceLoadSelection loadSelection)
		{
			if (instance == null)
			{
				return null;
			}
			string text = (string)instance.CimInstanceProperties["TypeName"].Value;
			string name = (string)instance.CimInstanceProperties["Name"].Value;
			Guid id = new Guid((string)instance.CimInstanceProperties["ElementName"].Value);
			PResourceType pResourceType = new PResourceType(base.CimAdapter.clusters.Cluster, text);
			if (pResourceType.ResourceKind == ResourceKind.Other && pResourceType.Name == null)
			{
				pResourceType.Name = text;
			}
			PGroup pGroup = null;
			if (pResourceType.ResourceKind == ResourceKind.PhysicalDisk)
			{
				pGroup = GetOwnerGroupFromInstance(instance);
				if (pGroup.GroupType == GroupType.ClusterSharedVolume)
				{
					pResourceType = new PResourceType(base.CimAdapter.clusters.Cluster, ResourceKind.ClusterFileSystem, pResourceType);
				}
			}
			PResource pResource = PResource.Constructor(base.CimAdapter.clusters.Cluster, id, name, pResourceType);
			PopulateFromCimInstance(instance, pResource, loadSelection, pGroup);
			return pResource;
		}

		private void PopulateFromCimInstance(CimInstance instance, PResource resource, ResourceLoadSelection loadSelection, PGroup ownerGroup)
		{
			Exceptions.ThrowIfNull(instance, "instance");
			Exceptions.ThrowIfNull(resource, "resource");
			ExecuteAndCatchWmiExceptions(delegate
			{
				if ((loadSelection & ResourceLoadSelection.CommonProperties) == ResourceLoadSelection.CommonProperties || (loadSelection & ResourceLoadSelection.Basic) == ResourceLoadSelection.Basic)
				{
					ParseProperties(resource.Properties, instance.CimInstanceProperties, ClusterPropertyKind.Common);
					resource.Characteristics = (Characteristics)(uint)instance.CimInstanceProperties["Characteristics"].Value;
					resource.Class = (ResourceClass)(uint)instance.CimInstanceProperties["ResourceClass"].Value;
					resource.Flags = (ResourceFlags)(uint)instance.CimInstanceProperties["Flags"].Value;
					resource.ResourceState = (ResourceState)(uint)instance.CimInstanceProperties["State"].Value;
					if (ownerGroup == null)
					{
						ownerGroup = GetOwnerGroupFromInstance(instance);
					}
					resource.OwnerGroup = ownerGroup;
					resource.LoadedSelection |= 3;
				}
			}, resource.Name);
		}

		private PGroup GetOwnerGroupFromInstance(CimInstance instance)
		{
			string text = (string)instance.CimInstanceProperties["OwnerGroupName"].Value;
			if (text == null)
			{
				Guid id = new Guid((string)instance.CimInstanceProperties["ElementName"].Value);
				return Open(id).OwnerGroup;
			}
			return base.CimAdapter.Group.Open(text);
		}

		public void Load(PResource resource, ResourceLoadSelection loadSelection)
		{
			try
			{
				ExecuteAndCatchWmiExceptions(delegate
				{
					if ((loadSelection & ResourceLoadSelection.CommonProperties) == ResourceLoadSelection.CommonProperties || (loadSelection & ResourceLoadSelection.Basic) == ResourceLoadSelection.Basic)
					{
						GetInstance(base.CimResource, resource.Id.ToString(), delegate(CimInstance instance)
						{
							PopulateFromCimInstance(instance, resource, loadSelection, null);
						});
					}
					if ((loadSelection & ResourceLoadSelection.PrivateProperties) == ResourceLoadSelection.PrivateProperties)
					{
						base.CimAdapter.GetPrivateProperties(resource);
					}
					if (loadSelection.HasFlag(ResourceLoadSelection.Storage) && (resource.LoadedSelection & 0x100) != 256)
					{
						if (resource.Class == ResourceClass.Storage)
						{
							if (resource.OwnerGroup.LoadedSelection == 1)
							{
								resource.OwnerGroup.LoadObject(1);
							}
							LoadDisk(resource);
						}
						resource.LoadedSelection |= 256;
					}
					if ((loadSelection & ResourceLoadSelection.Dependencies) == ResourceLoadSelection.Dependencies)
					{
						resource.Dependencies = GetDependencies(resource.Id);
						resource.LoadedSelection |= 8;
					}
					if ((loadSelection & ResourceLoadSelection.DependenciesRelation) == ResourceLoadSelection.DependenciesRelation)
					{
						resource.DependencyRelationship = GetDependencyRelationship(resource.Id);
						resource.LoadedSelection |= 32;
					}
				}, resource.Name);
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(resource.Name, resource.Id, innerException);
			}
		}

		private IEnumerable<Guid> GetDependencies(Guid resourceId)
		{
			return ExecuteAndCatchWmiExceptions(() => from dependencyInstance in base.CimAdapter.ExecuteOptimalQuery("select Dependent from MSFTCluster_ResourceToDependentResource where antecedent = \"MSFTCluster_resource.ElementName='{0}'\"".FormatInvariantCulture(resourceId.ToString()))
				select (CimInstance)dependencyInstance.CimInstanceProperties["Dependent"].Value into returnInstance
				select new Guid((string)returnInstance.CimInstanceProperties["ElementName"].Value), resourceId.ToString()).ToList();
		}

		private string GetDependencyRelationship(Guid resourceId)
		{
			return InvokeMethod(resourceId.ToString(), "GetDependencies", null, (CimMethodResult returnInstance) => (string)(returnInstance.OutParameters["Expression"] ?? throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("Expression"))).Value);
		}

		private void LoadDisk(PResource resource)
		{
			if (resource.ResourceState != ResourceState.Online)
			{
				resource.DiskInfo = new ClusterDisk();
				return;
			}
			GetInstance(base.CimAdapter.CimDiskInfo, resource.Id.ToString(), delegate(CimInstance cimInstance)
			{
				resource.DiskInfo = ParseDisk(cimInstance, resource, null, includeMountPoints: true);
			});
		}

		private ClusterDisk ParseDisk(CimInstance diskInfoInstance, PResource resource, Dictionary<string, Guid> resourceNameId, bool includeMountPoints)
		{
			if (!diskInfoInstance.CimSystemProperties.ClassName.Equals("MSFTCluster_DiskInfo"))
			{
				throw new InvalidOperationException(ExceptionResources.OutArgumentInvalidReference.FormatCurrentCulture("MSFTCluster_StorageService"));
			}
			CimKeyedCollection<CimProperty> cimInstanceProperties = diskInfoInstance.CimInstanceProperties;
			object value = cimInstanceProperties["GptGuid"].Value;
			ClusterDisk clusterDisk = ((value != null) ? new ClusterDisk(new Guid((string)value)) : new ClusterDisk((uint)cimInstanceProperties["Signature"].Value));
			clusterDisk.DiskNumber = (uint)cimInstanceProperties["Number"].Value;
			clusterDisk.Size = (ulong)cimInstanceProperties["Size"].Value;
			if (resource != null)
			{
				clusterDisk.Name = resource.Name;
			}
			else
			{
				int num = 1;
				string text;
				do
				{
					text = CommonResources.DiskNameFormatter.FormatCurrentCulture(num++);
				}
				while (resourceNameId.ContainsKey(text));
				clusterDisk.Name = text;
				resourceNameId.Add(text, Guid.Empty);
			}
			CimInstance[] array = (CimInstance[])cimInstanceProperties["Volumes"].Value;
			foreach (CimInstance obj in array)
			{
				ClusterDiskPartition diskPartition = new ClusterDiskPartition(clusterDisk);
				CimKeyedCollection<CimProperty> cimInstanceProperties2 = obj.CimInstanceProperties;
				diskPartition.Name = (string)cimInstanceProperties2["Path"].Value;
				diskPartition.DeviceNumber = (uint)cimInstanceProperties2["DeviceNumber"].Value;
				diskPartition.FileSystem = (string)cimInstanceProperties2["FileSystem"].Value;
				diskPartition.Flags = (uint)cimInstanceProperties2["Flags"].Value;
				diskPartition.IsCompressed = (diskPartition.Flags & 0x8000) != 0;
				diskPartition.FreeSpace = (ulong)cimInstanceProperties2["FreeSpace"].Value;
				diskPartition.PartitionNumber = (uint)cimInstanceProperties2["PartitionNumber"].Value;
				diskPartition.SerialNumber = (uint)cimInstanceProperties2["SerialNumber"].Value;
				diskPartition.Size = (ulong)cimInstanceProperties2["Size"].Value;
				diskPartition.VolumeGuid = new Guid((string)cimInstanceProperties2["VolumeGuid"].Value);
				diskPartition.Label = (string)cimInstanceProperties2["VolumeLabel"].Value;
				diskPartition.IsMaintenanceModeOn = (bool)diskInfoInstance.CimInstanceProperties["IsMaintenenceModeEnabled"].Value;
				diskPartition.IncludeMountPoints = includeMountPoints;
				if (includeMountPoints && resource != null)
				{
					GetInstanceIdentity(base.CimAdapter.CimStorageService, base.CimAdapter.clusters.Cluster.Id.ToString(), delegate(CimInstance storageServiceInstance)
					{
						GetInstanceIdentity(base.CimAdapter.CimVolumeInfo, resource.Id.ToString(), delegate(CimInstance volumeInfoInstance)
						{
							volumeInfoInstance.CimInstanceProperties["VolumeGuid"].Value = diskPartition.VolumeGuid;
							CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Volume", volumeInfoInstance);
							base.CimAdapter.InvokeMethod(storageServiceInstance, "GetMountPoints", new CimArgumentNameValue[1] { cimArgumentNameValue }, delegate(CimMethodResult mountPointsInstance)
							{
								string[] mountPoints = (string[])(mountPointsInstance.OutParameters["MountPoints"] ?? throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("MountPoints"))).Value;
								diskPartition.SetMountPoints(mountPoints);
							});
						});
					});
				}
				if (resource is PCsvVolumeResource && diskPartition.IsMaintenanceModeOn == false)
				{
					ClusterSharedVolumeInfo clusterSharedVolumeInfo = new ClusterSharedVolumeInfo(diskPartition);
					clusterSharedVolumeInfo.VolumeOffset = (ulong)cimInstanceProperties2["VolumeOffset"].Value;
					clusterSharedVolumeInfo.PartitionNumber = (uint)cimInstanceProperties2["PartitionNumber"].Value;
					ClusterSharedVolumeFaultState faultState = (diskPartition.CsvFaultState = (ClusterSharedVolumeFaultState)(uint)cimInstanceProperties2["SharedVolumeHealthState"].Value);
					clusterSharedVolumeInfo.FaultState = faultState;
					clusterSharedVolumeInfo.BackupState = 0u;
					clusterSharedVolumeInfo.VolumeGuid = (string)cimInstanceProperties2["VolumeGuid"].Value;
					string sharedVolumesRootPath = base.CimAdapter.clusters.Cluster.SharedVolumesRootPath;
					string friendlyVolumeName = (diskPartition.Name = (string)cimInstanceProperties2["Path"].Value);
					clusterSharedVolumeInfo.FriendlyVolumeName = friendlyVolumeName;
					clusterSharedVolumeInfo.RootPath = sharedVolumesRootPath;
					if (clusterSharedVolumeInfo.FriendlyVolumeName.Length > sharedVolumesRootPath.Length)
					{
						clusterSharedVolumeInfo.VolumeName = clusterSharedVolumeInfo.FriendlyVolumeName.Substring(sharedVolumesRootPath.Length + 1);
					}
					diskPartition.SharedVolumeInfo = clusterSharedVolumeInfo;
				}
				clusterDisk.Partitions.Add(diskPartition);
			}
			return clusterDisk;
		}

		public void MoveToGroup(Guid resourceId, Guid groupId)
		{
			GetInstanceIdentity(resourceId.ToString(), delegate(CimInstance cimInstance)
			{
				GetInstanceIdentity(base.CimGroup, groupId.ToString(), delegate(CimInstance groupInstance)
				{
					CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Group", groupInstance);
					base.CimAdapter.InvokeMethod(cimInstance, "SetOwnerGroup", new CimArgumentNameValue[1] { cimArgumentNameValue });
				});
			});
		}

		public void Delete(Guid id, bool cleanup)
		{
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Cleanup", cleanup);
			InvokeMethod(id.ToString(), "Destroy", new CimArgumentNameValue[1] { cimArgumentNameValue });
		}

		public PResource Create(PGroup privateGroup, string name, PResourceType resourceType, bool separateMonitor)
		{
			return GetInstanceIdentity(base.CimResourceType, resourceType, (CimInstance resourceTypeInstance) => GetInstanceIdentity(base.CimGroup, privateGroup, delegate(CimInstance groupInstance)
			{
				CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("ResourceName", name);
				CimArgumentNameValue cimArgumentNameValue2 = new CimArgumentNameValue("ResourceType", resourceTypeInstance);
				CimArgumentNameValue cimArgumentNameValue3 = new CimArgumentNameValue("SeparateMonitor", separateMonitor);
				CimArgumentNameValue cimArgumentNameValue4 = new CimArgumentNameValue("Group", groupInstance);
				return base.CimAdapter.InvokeMethod(base.CimResource, "Create", new CimArgumentNameValue[4] { cimArgumentNameValue, cimArgumentNameValue2, cimArgumentNameValue3, cimArgumentNameValue4 }, delegate(CimMethodResult returnInstance)
				{
					CimInstance cimInstance = (CimInstance)(returnInstance.OutParameters["Resource"] ?? throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("Resource"))).Value;
					return Open((string)cimInstance.CimInstanceProperties["ElementName"].Value);
				});
			}));
		}

		public IEnumerable<PResource> GetAll(bool nullElementOnError)
		{
			return from instance in base.CimAdapter.ExecuteOptimalQuery("Select Name, ElementName, TypeName from MSFTCluster_Resource", nullElementOnError)
				select CreateFromCimInstance(instance, ResourceLoadSelection.None);
		}

		public string GetVirtualMachineOwnerGroup(Guid vmId)
		{
			throw new NotSupportedException("GetVirtualMachineOwnerGroup is not implemented for CimAdapter");
		}

		public void VirtualMachineTurnOff(PVirtualMachineResource resource)
		{
			Offline(resource.Id);
		}

		public void VirtualMachineSave(PVirtualMachineResource resource)
		{
			Offline(resource.Id);
		}

		public void VirtualMachineShutdown(PVirtualMachineResource resource)
		{
			Offline(resource.Id);
		}

		public void VirtualMachineRefreshSettings(Guid resourceId, string hostName)
		{
			throw new NotSupportedException("VirtualMachineRefreshSettings is not implemented for CimAdapter");
		}

		public void VirtualMachineMoveStorage(Guid resourceId, string hostName, VirtualMachineStorageMoveParameters virtualMachineStorageMoveParameters)
		{
			throw new NotSupportedException("VirtualMachineMoveStorage is not implemented for CimAdapter");
		}

		public void NetworkNameRepairActiveDirectoryObject(Guid id)
		{
			throw new NotSupportedException("NetworkNameRepairActiveDirectoryObject is not implemented for CimAdapter");
		}

		public void NetworkNameResetCnoPassword(PNetNameResource netNameResourcePrivate)
		{
			throw new NotSupportedException("NetworkNameResetCnoPassword is not implemented for CimAdapter");
		}

		public void NetworkNameEnableAdObject(PNetNameResource netNameResourcePrivate)
		{
			throw new NotSupportedException("NetworkNameEnableADObject is not implemented for CimAdapter");
		}

		public void NetworkNameRepairReAclDNSRecords(PNetNameResource privateCNOResource)
		{
			throw new NotSupportedException("NetworkNameRepairReAclDNSRecords is not implemented for CimAdapter");
		}

		public void AddToClusterSharedVolumes(PStorageResource storageResource)
		{
			GetInstanceIdentity(base.CimAdapter.CimStorageService, storageResource.Cluster.Id.ToString(), delegate(CimInstance storageServiceInstance)
			{
				GetInstanceIdentity(storageResource.Id.ToString(), delegate(CimInstance resourceInstance)
				{
					CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("StorageResource", resourceInstance);
					base.CimAdapter.InvokeMethod(storageServiceInstance, "AddToClusterSharedVolumes", new CimArgumentNameValue[1] { cimArgumentNameValue }, null);
				});
			});
		}

		public void RemoveFromClusterSharedVolumes(PCsvVolumeResource storageResource)
		{
			GetInstanceIdentity(base.CimAdapter.CimStorageService, storageResource.Cluster.Id.ToString(), delegate(CimInstance storageServiceInstance)
			{
				GetInstanceIdentity(storageResource.Id.ToString(), delegate(CimInstance resourceInstance)
				{
					CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("StorageResource", resourceInstance);
					base.CimAdapter.InvokeMethod(storageServiceInstance, "RemoveFromClusterSharedVolumes", new CimArgumentNameValue[1] { cimArgumentNameValue }, null);
				});
			});
		}

		public void SetCsvRedirectedAccess(PCsvVolumeResource csvVolumeResourcePrivate, Guid deviceId, bool csvRedirectedAccessMode)
		{
			GetInstanceIdentity(base.CimAdapter.CimStorageService, base.CimAdapter.clusters.Cluster.Id.ToString(), delegate(CimInstance storageServiceInstance)
			{
				GetInstanceIdentity(base.CimAdapter.CimVolumeInfo, csvVolumeResourcePrivate.Id.ToString(), delegate(CimInstance volumeInfoInstance)
				{
					volumeInfoInstance.CimInstanceProperties["VolumeGuid"].Value = deviceId.ToString();
					CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("SharedVolume", volumeInfoInstance);
					base.CimAdapter.InvokeMethod(storageServiceInstance, csvRedirectedAccessMode ? "EnableSharedVolumeRedirectedAccess" : "DisableSharedVolumeRedirectedAccess", new CimArgumentNameValue[1] { cimArgumentNameValue }, null);
				});
			});
		}

		public void SetMaintenanceMode(PStorageResource storageResource, bool maintenanceMode)
		{
			GetInstanceIdentity(base.CimAdapter.CimStorageService, storageResource.Cluster.Id.ToString(), delegate(CimInstance storageServiceInstance)
			{
				GetInstanceIdentity(base.CimAdapter.CimResource, storageResource.Id.ToString(), delegate(CimInstance resourceInstance)
				{
					CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("DiskInfo", resourceInstance);
					base.CimAdapter.InvokeMethod(storageServiceInstance, maintenanceMode ? "EnableMaintenanceMode" : "DisableMaintenanceMode", new CimArgumentNameValue[1] { cimArgumentNameValue });
				});
			});
		}

		public uint GetDiskNumber(PStorageResource storageResourcePrivate, string nodeName)
		{
			throw new NotImplementedException("GetDiskNumber not yet implemented on the CimAdapter");
		}

		public CsvVolumeInformation GetCsvVolumeInformation(PCsvVolumeResource csvVolumeResourcePrivate)
		{
			throw new NotImplementedException("GetCsvVolumeInformation not yet implemented on the CimAdapter");
		}

		public void Renew(PCommonIPAddressResource ipAddress)
		{
			GetInstanceIdentity(base.CimAdapter.CimNetworkService, ipAddress.Cluster.Id.ToString(), delegate(CimInstance networkServiceInstance)
			{
				GetInstanceIdentity(base.CimAdapter.CimResource, ipAddress.Id.ToString(), delegate(CimInstance resourceInstance)
				{
					CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Resource", resourceInstance);
					base.CimAdapter.InvokeMethod(networkServiceInstance, "Renew", new CimArgumentNameValue[1] { cimArgumentNameValue });
				});
			});
		}

		public void Release(PCommonIPAddressResource ipAddress)
		{
			GetInstanceIdentity(base.CimAdapter.CimNetworkService, ipAddress.Cluster.Id.ToString(), delegate(CimInstance networkServiceInstance)
			{
				GetInstanceIdentity(base.CimAdapter.CimResource, ipAddress.Id.ToString(), delegate(CimInstance resourceInstance)
				{
					CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Resource", resourceInstance);
					base.CimAdapter.InvokeMethod(networkServiceInstance, "Release", new CimArgumentNameValue[1] { cimArgumentNameValue });
				});
			});
		}

		public override void Collect()
		{
		}
	}

	private class StorageAdapter : AdapterBase, IConnectionAdapterStorage
	{
		private class StorageEnclosureToPhysicallyConnectedStorageNode
		{
			private CimSession Session { get; set; }

			private CimInstance Instance { get; set; }

			public IEnumerable<MSFT_StorageNode> StorageNode
			{
				get
				{
					CimOperationOptions cimOperationOptions = new CimOperationOptions();
					cimOperationOptions.SetCustomOption("PhysicallyConnected", optionValue: true, mustComply: true);
					if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageNodeToStorageEnclosure", "MSFT_StorageNode", "StorageEnclosure", "StorageNode", cimOperationOptions) != null)
					{
						return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageNodeToStorageEnclosure", "MSFT_StorageNode", "StorageEnclosure", "StorageNode", cimOperationOptions)
							select new MSFT_StorageNode(Session, i)).ToArray();
					}
					return null;
				}
			}

			private StorageEnclosureToPhysicallyConnectedStorageNode(CimSession session, CimInstance instance)
			{
				Session = session;
				Instance = instance;
			}

			public static StorageEnclosureToPhysicallyConnectedStorageNode CreateReference(CimSession session, string objectId)
			{
				CimInstance cimInstance = new CimInstance("MSFT_StorageEnclosure", "root/microsoft/windows/storage");
				cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", objectId, CimFlags.Key));
				return new StorageEnclosureToPhysicallyConnectedStorageNode(session, cimInstance);
			}
		}

		private class SubscriptionTask
		{
			private class MyObserver : IObserver<MSFT_StorageEvent>
			{
				private readonly ConcurrentQueue<MSFT_StorageEvent> eventsQueue;

				private MSFT_StorageEvent lastQueuedEvent;

				private readonly AutoResetEvent resetEvent;

				public bool Completed { get; private set; }

				public MyObserver(ConcurrentQueue<MSFT_StorageEvent> queue, AutoResetEvent eventArrived)
				{
					eventsQueue = queue;
					resetEvent = eventArrived;
				}

				public void OnCompleted()
				{
					Completed = true;
					resetEvent.Set();
				}

				public void OnError(Exception error)
				{
					ClusterLog.LogException(error);
				}

				public void OnNext(MSFT_StorageEvent value)
				{
					if (eventsQueue.Count <= 0 || lastQueuedEvent == null || !(lastQueuedEvent.SourceClassName == value.SourceClassName) || !(lastQueuedEvent.SourceNamespace == value.SourceNamespace) || !(lastQueuedEvent.SourceObjectId == value.SourceObjectId) || !(lastQueuedEvent.SourceServer == value.SourceServer))
					{
						lastQueuedEvent = value;
						eventsQueue.Enqueue(value);
						resetEvent.Set();
						_ = eventsQueue.Count % 10;
					}
				}
			}

			private class ObservableGenericCollection
			{
				private WeakReferenceEx observableKeyCollectionWeek;

				public Type ContainedType { get; private set; }

				public object ObservableKeyCollection
				{
					get
					{
						if (observableKeyCollectionWeek != null)
						{
							return observableKeyCollectionWeek.Target;
						}
						return null;
					}
				}

				public void SetCollection<T>(ObservableKeyCollection<T> collection) where T : IKeyQueryable<T>
				{
					Exceptions.ThrowIfNull(collection, "collection");
					ContainedType = collection.GetType();
					observableKeyCollectionWeek = new WeakReferenceEx(collection);
				}
			}

			private readonly ConcurrentDictionary<string, ConcurrentDictionary<ObservableGenericCollection, ObservableGenericCollection>> subscriptionsDictionary = new ConcurrentDictionary<string, ConcurrentDictionary<ObservableGenericCollection, ObservableGenericCollection>>();

			private readonly CimSession session;

			private readonly MyObserver observer;

			private IDisposable disposableObserver;

			private readonly ConcurrentQueue<MSFT_StorageEvent> eventsQueue = new ConcurrentQueue<MSFT_StorageEvent>();

			private readonly AutoResetEvent eventArrived = new AutoResetEvent(initialState: false);

			private static int subscriptionServerCount;

			private static int subscriptionCollectionCount;

			private bool Started { get; set; }

			public SubscriptionTask(CimSession cimSession)
			{
				session = cimSession;
				observer = new MyObserver(eventsQueue, eventArrived);
			}

			private void Start()
			{
				lock (this)
				{
					if (Started)
					{
						return;
					}
					Started = true;
				}
				Subscribe();
				Task.Run(delegate
				{
					ProcessEvents();
				});
			}

			public bool Collect()
			{
				int num = 0;
				foreach (KeyValuePair<string, ConcurrentDictionary<ObservableGenericCollection, ObservableGenericCollection>> item in subscriptionsDictionary)
				{
					ConcurrentDictionary<ObservableGenericCollection, ObservableGenericCollection> value = item.Value;
					foreach (ObservableGenericCollection key in value.Keys)
					{
						if (key.ObservableKeyCollection == null)
						{
							value.TryRemove(key, out var _);
						}
						else
						{
							num++;
						}
					}
				}
				if (num != 0)
				{
					return false;
				}
				if (disposableObserver != null)
				{
					disposableObserver.Dispose();
					disposableObserver = null;
				}
				eventArrived.Set();
				return true;
			}

			private void Subscribe()
			{
				disposableObserver = MSFT_StorageEvent.Subscribe(session, observer);
			}

			private void ProcessEvents()
			{
				while (!observer.Completed && disposableObserver != null)
				{
					if (!eventArrived.WaitOne())
					{
						continue;
					}
					while (eventsQueue.Count > 0)
					{
						if (eventsQueue.TryDequeue(out var result))
						{
							ProcessEvent(result);
						}
					}
				}
			}

			private void ProcessEvent(MSFT_StorageEvent storageEvent)
			{
				if (!subscriptionsDictionary.TryGetValue(storageEvent.SourceClassName, out var value) || !SubscriptionsEventConvertion.TryGetValue(storageEvent.SourceClassName, out var value2))
				{
					return;
				}
				foreach (ObservableGenericCollection key in value.Keys)
				{
					object observableKeyCollection = key.ObservableKeyCollection;
					if (observableKeyCollection != null)
					{
						SubscriptionOperation arg;
						switch (storageEvent.Instance.CimClass.CimSystemProperties.ClassName)
						{
						case "MSFT_StorageArrivalEvent":
							arg = SubscriptionOperation.Add;
							break;
						case "MSFT_StorageDepartureEvent":
							arg = SubscriptionOperation.Delete;
							break;
						case "MSFT_StorageModificationEvent":
							arg = SubscriptionOperation.Modify;
							break;
						default:
							continue;
						}
						value2(arg, observableKeyCollection, session, storageEvent);
					}
				}
			}

			public void Subscribe<T>(ObservableKeyCollection<T> collection) where T : IKeyQueryable<T>
			{
				if (!SubscriptionsMappingClass.TryGetValue(collection.ItemType, out var value))
				{
					ClusterLog.LogError("Failed to find a subscription mapping class for type {0}".FormatInvariantCulture(typeof(T).Name));
					return;
				}
				foreach (string item in value)
				{
					ConcurrentDictionary<ObservableGenericCollection, ObservableGenericCollection> orAdd = subscriptionsDictionary.GetOrAdd(item, new ConcurrentDictionary<ObservableGenericCollection, ObservableGenericCollection>());
					ObservableGenericCollection observableGenericCollection = new ObservableGenericCollection();
					observableGenericCollection.SetCollection(collection);
					orAdd.TryAdd(observableGenericCollection, observableGenericCollection);
				}
				Start();
			}
		}

		private readonly CimAdapter cimApiAdapter;

		private const string EventSpacesPhysicalDisk = "SPACES_PhysicalDisk";

		private const string EventSpacesVirtualDisk = "SPACES_VirtualDisk";

		private const string EventSpacesEnclosure = "SPACES_StorageEnclosure";

		private const string EventSpacesStorageNode = "SPACES_StorageNode";

		private const string EventSpacesDisk = "SPACES_Disk";

		private const string ReplicaNameSpace = "root\\Microsoft\\Windows\\StorageReplica";

		private const string ReplicaAdminTaskClass = "MSFT_WvrAdminTasks";

		private const string ReplicaPartnershipQueryFromSource = "Select * from MSFT_WvrReplicationPartnership where SourceRGName='{0}' and SourceComputerName='{1}'";

		private const string ReplicaPartnershipQueryFromDestination = "Select * from MSFT_WvrReplicationPartnership where DestinationRGName='{0}' and DestinationComputerName='{1}'";

		private const string ReplicaGroupQueryId = "Select * from MSFT_WvrReplicationGroup where Id='{0}'";

		private const string ReplicaGroupQueryName = "Select * from MSFT_WvrReplicationGroup where Name='{0}'";

		private const string ReplicaRemovePartnershipMethod = "RemovePartnership";

		private const string ReplicaSetLogSizeMethod = "SetGroupModifyConfig";

		private const string ReplicaRemoveSyncPair = "SetGroupRemoveVolumes";

		private const string ReplicationGroupNameField = "Name";

		private const string ReplicationGroupDescriptionField = "Description";

		private const string ReplicationGroupIdField = "Id";

		private const string ReplicationComputerNameField = "ComputerName";

		private const string ReplicationTypeField = "ReplicationMode";

		private const string ReplicationPartitionIdField = "PartitionId";

		private const string ReplicationStatusField = "ReplicationStatus";

		private const string ReplicationIsWriteConsistency = "IsWriteConsistency";

		private const string ReplicationLogSizeField = "LogSizeInBytes";

		private const string ReplicasField = "Replicas";

		private const string ReplicationFullCleanupParam = "FullCleanup";

		private const string ReplicationSourceComputerNameField = "SourceComputerName";

		private const string ReplicationSourceRgNameField = "SourceRGName";

		private const string ReplicationDestinationComputerNameField = "DestinationComputerName";

		private const string ReplicationDestinationRgNameField = "DestinationRGName";

		private const string ReplicationDeleteReplicationGroupMethod = "WvrDeleteReplicationGroup";

		private const string ReplicationGroupName2Field = "ReplicationGroupName";

		private const string ReplicaRemoveVolumeNameField = "RemoveVolumeName";

		private static readonly ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, Cluster, MSFT_StorageSubSystem, IEnumerable<IKeyQueryable>>> Enumerations;

		private static readonly ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, string, Cluster, MSFT_StorageSubSystem, IEnumerable<IKeyQueryable>>> Queries;

		private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, string, Cluster, IEnumerable<IKeyQueryable>>>> Associations;

		private static readonly ConcurrentDictionary<SubscriptionTask, SubscriptionTask> Subscriptions;

		private static readonly ConcurrentDictionary<ObservableCollectionItem, IEnumerable<string>> SubscriptionsMappingClass;

		private static readonly ConcurrentDictionary<string, Action<SubscriptionOperation, object, CimSession, MSFT_StorageEvent>> SubscriptionsEventConvertion;

		private static readonly ConcurrentDictionary<Type, Func<CimSession, string, Cluster, object>> Instances;

		static StorageAdapter()
		{
			Enumerations = new ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, Cluster, MSFT_StorageSubSystem, IEnumerable<IKeyQueryable>>>();
			Queries = new ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, string, Cluster, MSFT_StorageSubSystem, IEnumerable<IKeyQueryable>>>();
			Associations = new ConcurrentDictionary<Type, ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, string, Cluster, IEnumerable<IKeyQueryable>>>>();
			Subscriptions = new ConcurrentDictionary<SubscriptionTask, SubscriptionTask>();
			SubscriptionsMappingClass = new ConcurrentDictionary<ObservableCollectionItem, IEnumerable<string>>();
			SubscriptionsEventConvertion = new ConcurrentDictionary<string, Action<SubscriptionOperation, object, CimSession, MSFT_StorageEvent>>();
			Instances = new ConcurrentDictionary<Type, Func<CimSession, string, Cluster, object>>();
			Instances.TryAdd(typeof(Enclosure), (CimSession session, string key, Cluster cluster) => MSFT_StorageEnclosure.GetInstance(session, key).ToEnclosure(cluster));
			Instances.TryAdd(typeof(PhysicalDiskInfo), (CimSession session, string key, Cluster cluster) => MSFT_PhysicalDisk.GetInstance(session, key).ToPhysicalDiskInfo(cluster));
			Instances.TryAdd(typeof(VirtualDiskInfo), (CimSession session, string key, Cluster cluster) => MSFT_VirtualDisk.GetInstance(session, key).ToVirtualDiskInfo(cluster));
			Instances.TryAdd(typeof(StorageNode), (CimSession session, string key, Cluster cluster) => MSFT_StorageNode.GetInstance(session, key).ToStorageNode(cluster));
			Instances.TryAdd(typeof(MsftDiskInfo), (CimSession session, string key, Cluster cluster) => MSFT_Disk.GetInstance(session, key).ToMsftDiskInfo(cluster));
			Instances.TryAdd(typeof(MsftVolumeInfo), (CimSession session, string key, Cluster cluster) => MSFT_Volume.GetInstance(session, key).ToMsftVolumeInfo(cluster));
			Instances.TryAdd(typeof(MsftPartitionInfo), (CimSession session, string key, Cluster cluster) => MSFT_Partition.GetInstance(session, key).ToMsftDiskPartitionInfo(cluster));
			Enumerations.TryAdd(ObservableCollectionItem.Enclosures, (CimSession session, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => from msftEnclosure in MSFT_StorageEnclosure.Enumerate(session)
				select msftEnclosure.ToEnclosure(cluster));
			Enumerations.TryAdd(ObservableCollectionItem.PhysicalDisk, (CimSession session, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => storageSubsystem.MSFT_StorageSubSystemToPhysicalDisk.PhysicalDisk.Select((MSFT_PhysicalDisk msftPhysicalDisk) => msftPhysicalDisk.ToPhysicalDiskInfo(cluster)));
			Enumerations.TryAdd(ObservableCollectionItem.VirtualDisk, (CimSession session, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => storageSubsystem.MSFT_StorageSubSystemToVirtualDisk.VirtualDisk.Select((MSFT_VirtualDisk msftVirtualDisk) => msftVirtualDisk.ToVirtualDiskInfo(cluster)));
			Enumerations.TryAdd(ObservableCollectionItem.StorageNode, (CimSession session, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => from msftStorageNode in MSFT_StorageNode.Enumerate(session)
				select msftStorageNode.ToStorageNode(cluster));
			Enumerations.TryAdd(ObservableCollectionItem.Disk, (CimSession session, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => from msftDisk in MSFT_Disk.Enumerate(session)
				select msftDisk.ToMsftDiskInfo(cluster));
			Enumerations.TryAdd(ObservableCollectionItem.DiskPartition, (CimSession session, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => from msftPartition in MSFT_Partition.Enumerate(session)
				select msftPartition.ToMsftDiskPartitionInfo(cluster));
			Enumerations.TryAdd(ObservableCollectionItem.VolumeInfo, (CimSession session, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => from msftVolume in MSFT_Volume.Enumerate(session)
				select msftVolume.ToMsftVolumeInfo(cluster));
			Enumerations.TryAdd(ObservableCollectionItem.DiskAndPhysicalDisk, (CimSession session, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => ((IEnumerable<DiskInfoBase>)(from disk in MSFT_Disk.Enumerate(session)
				where disk.IsClustered == true
				select disk into msftDisk
				select msftDisk.ToMsftDiskInfo(cluster))).Concat((IEnumerable<DiskInfoBase>)storageSubsystem.MSFT_StorageSubSystemToPhysicalDisk.PhysicalDisk.Select((MSFT_PhysicalDisk msftPhysicalDisk) => msftPhysicalDisk.ToPhysicalDiskInfo(cluster))));
			Queries.TryAdd(ObservableCollectionItem.Enclosures, (CimSession session, string whereFilter, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => from msftEnclosure in MSFT_StorageEnclosure.Query(session, whereFilter)
				select msftEnclosure.ToEnclosure(cluster));
			Queries.TryAdd(ObservableCollectionItem.PhysicalDisk, (CimSession session, string whereFilter, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => from msftPhysicalDisk in MSFT_PhysicalDisk.Query(session, whereFilter)
				select msftPhysicalDisk.ToPhysicalDiskInfo(cluster));
			Queries.TryAdd(ObservableCollectionItem.VirtualDisk, (CimSession session, string whereFilter, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => from msftVirtualDisk in MSFT_VirtualDisk.Query(session, whereFilter)
				select msftVirtualDisk.ToVirtualDiskInfo(cluster));
			Queries.TryAdd(ObservableCollectionItem.StorageNode, (CimSession session, string whereFilter, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => from msftStorageNode in MSFT_StorageNode.Query(session, whereFilter)
				select msftStorageNode.ToStorageNode(cluster));
			Queries.TryAdd(ObservableCollectionItem.Disk, (CimSession session, string whereFilter, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => from msftDisk in MSFT_Disk.Query(session, whereFilter)
				select msftDisk.ToMsftDiskInfo(cluster));
			Queries.TryAdd(ObservableCollectionItem.DiskPartition, (CimSession session, string whereFilter, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => from msftPartition in MSFT_Partition.Query(session, whereFilter)
				select msftPartition.ToMsftDiskPartitionInfo(cluster));
			Queries.TryAdd(ObservableCollectionItem.VolumeInfo, (CimSession session, string whereFilter, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => from msftVolume in MSFT_Volume.Query(session, whereFilter)
				select msftVolume.ToMsftVolumeInfo(cluster));
			Queries.TryAdd(ObservableCollectionItem.DiskAndPhysicalDisk, (CimSession session, string whereFilter, Cluster cluster, MSFT_StorageSubSystem storageSubsystem) => ((IEnumerable<DiskInfoBase>)(from msftDisk in MSFT_Disk.Query(session, whereFilter)
				select msftDisk.ToMsftDiskInfo(cluster))).Concat((IEnumerable<DiskInfoBase>)(from msftPhysicalDisk in MSFT_PhysicalDisk.Query(session, whereFilter)
				select msftPhysicalDisk.ToPhysicalDiskInfo(cluster))));
			Associations.GetOrAdd(typeof(Enclosure), delegate
			{
				ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, string, Cluster, IEnumerable<IKeyQueryable>>> concurrentDictionary4 = new ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, string, Cluster, IEnumerable<IKeyQueryable>>>();
				concurrentDictionary4.TryAdd(ObservableCollectionItem.PhysicalDisk, (CimSession session, string objectId, Cluster cluster) => MSFT_StorageEnclosure.CreateReference(session, objectId).MSFT_StorageEnclosureToPhysicalDisk.PhysicalDisk.Select((MSFT_PhysicalDisk msftPhysicalDisk) => msftPhysicalDisk.ToPhysicalDiskInfo(cluster)));
				concurrentDictionary4.TryAdd(ObservableCollectionItem.StorageNode, (CimSession session, string objectId, Cluster cluster) => MSFT_StorageEnclosure.CreateReference(session, objectId).MSFT_StorageNodeToStorageEnclosure.StorageNode.Select((MSFT_StorageNode msftStorageNode) => msftStorageNode.ToStorageNode(cluster)));
				concurrentDictionary4.TryAdd(ObservableCollectionItem.PhysicallyConnectedStorageNode, (CimSession session, string objectId, Cluster cluster) => StorageEnclosureToPhysicallyConnectedStorageNode.CreateReference(session, objectId).StorageNode.Select((MSFT_StorageNode msftStorageNode) => msftStorageNode.ToStorageNode(cluster)));
				return concurrentDictionary4;
			});
			Associations.GetOrAdd(typeof(Node), delegate
			{
				ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, string, Cluster, IEnumerable<IKeyQueryable>>> concurrentDictionary3 = new ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, string, Cluster, IEnumerable<IKeyQueryable>>>();
				concurrentDictionary3.TryAdd(ObservableCollectionItem.PhysicalDisk, (CimSession session, string objectId, Cluster cluster) => MSFT_StorageNode.CreateReference(session, objectId).MSFT_StorageNodeToPhysicalDisk.PhysicalDisk.Select((MSFT_PhysicalDisk msftPhysicalDisk) => msftPhysicalDisk.ToPhysicalDiskInfo(cluster)));
				concurrentDictionary3.TryAdd(ObservableCollectionItem.Enclosures, (CimSession session, string objectId, Cluster cluster) => MSFT_StorageNode.CreateReference(session, objectId).MSFT_StorageNodeToStorageEnclosure.StorageEnclosure.Select((MSFT_StorageEnclosure msftEnclosure) => msftEnclosure.ToEnclosure(cluster)));
				return concurrentDictionary3;
			});
			Associations.GetOrAdd(typeof(MsftDiskInfo), delegate
			{
				ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, string, Cluster, IEnumerable<IKeyQueryable>>> concurrentDictionary2 = new ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, string, Cluster, IEnumerable<IKeyQueryable>>>();
				concurrentDictionary2.TryAdd(ObservableCollectionItem.DiskPartition, (CimSession session, string key, Cluster cluster) => MSFT_Disk.CreateReference(session, key).MSFT_DiskToPartition.Partition.Select((MSFT_Partition msftPartitionInfo) => msftPartitionInfo.ToMsftDiskPartitionInfo(cluster)));
				return concurrentDictionary2;
			});
			Associations.GetOrAdd(typeof(MsftPartitionInfo), delegate
			{
				ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, string, Cluster, IEnumerable<IKeyQueryable>>> concurrentDictionary = new ConcurrentDictionary<ObservableCollectionItem, Func<CimSession, string, Cluster, IEnumerable<IKeyQueryable>>>();
				concurrentDictionary.TryAdd(ObservableCollectionItem.VolumeInfo, delegate(CimSession session, string key, Cluster cluster)
				{
					MsftPartitionInfo.ParseKey(key, out var diskId, out var _);
					return MSFT_Partition.CreateReference(session, diskId).MSFT_PartitionToVolume.Volume.Select((MSFT_Volume msftVolumeInfo) => msftVolumeInfo.ToMsftVolumeInfo(cluster));
				});
				return concurrentDictionary;
			});
			SubscriptionsMappingClass.TryAdd(ObservableCollectionItem.PhysicalDisk, new string[1] { "SPACES_PhysicalDisk" });
			SubscriptionsEventConvertion.TryAdd("SPACES_PhysicalDisk", delegate(SubscriptionOperation operation, object collection, CimSession session, MSFT_StorageEvent storageEvent)
			{
				ObservableKeyCollection<PhysicalDiskInfo> observableKeyCollection5 = (ObservableKeyCollection<PhysicalDiskInfo>)collection;
				PhysicalDiskInfo item5 = GetPhysicalDiskInstance(operation, session, storageEvent.SourceObjectId).ToPhysicalDiskInfo(observableKeyCollection5.Cluster);
				observableKeyCollection5.OnNext(operation, item5);
			});
			SubscriptionsMappingClass.TryAdd(ObservableCollectionItem.VirtualDisk, new string[1] { "SPACES_VirtualDisk" });
			SubscriptionsEventConvertion.TryAdd("SPACES_VirtualDisk", delegate(SubscriptionOperation operation, object collection, CimSession session, MSFT_StorageEvent storageEvent)
			{
				ObservableKeyCollection<VirtualDiskInfo> observableKeyCollection4 = (ObservableKeyCollection<VirtualDiskInfo>)collection;
				VirtualDiskInfo item4 = GetVirtualDiskInstance(operation, session, storageEvent.SourceObjectId).ToVirtualDiskInfo(observableKeyCollection4.Cluster);
				observableKeyCollection4.OnNext(operation, item4);
			});
			SubscriptionsMappingClass.TryAdd(ObservableCollectionItem.Enclosures, new string[1] { "SPACES_StorageEnclosure" });
			SubscriptionsEventConvertion.TryAdd("SPACES_StorageEnclosure", delegate(SubscriptionOperation operation, object collection, CimSession session, MSFT_StorageEvent storageEvent)
			{
				ObservableKeyCollection<Enclosure> observableKeyCollection3 = (ObservableKeyCollection<Enclosure>)collection;
				Enclosure item3 = GetEnclosureInstance(operation, session, storageEvent.SourceObjectId).ToEnclosure(observableKeyCollection3.Cluster);
				observableKeyCollection3.OnNext(operation, item3);
			});
			SubscriptionsMappingClass.TryAdd(ObservableCollectionItem.StorageNode, new string[1] { "SPACES_StorageNode" });
			SubscriptionsEventConvertion.TryAdd("SPACES_StorageNode", delegate(SubscriptionOperation operation, object collection, CimSession session, MSFT_StorageEvent storageEvent)
			{
				ObservableKeyCollection<StorageNode> observableKeyCollection2 = (ObservableKeyCollection<StorageNode>)collection;
				StorageNode item2 = GetStorageNodeInstance(operation, session, storageEvent.SourceObjectId).ToStorageNode(observableKeyCollection2.Cluster);
				observableKeyCollection2.OnNext(operation, item2);
			});
			SubscriptionsMappingClass.TryAdd(ObservableCollectionItem.Disk, new string[1] { "SPACES_Disk" });
			SubscriptionsEventConvertion.TryAdd("SPACES_Disk", delegate(SubscriptionOperation operation, object collection, CimSession session, MSFT_StorageEvent storageEvent)
			{
				ObservableKeyCollection<MsftDiskInfo> observableKeyCollection = (ObservableKeyCollection<MsftDiskInfo>)collection;
				MsftDiskInfo item = GetDiskInstance(operation, session, storageEvent.SourceObjectId).ToMsftDiskInfo(observableKeyCollection.Cluster);
				observableKeyCollection.OnNext(operation, item);
			});
		}

		public StorageAdapter(CimAdapter cimAdapter)
			: base(cimAdapter)
		{
			cimApiAdapter = cimAdapter;
		}

		public void SetReplicationLogSize(PStorageResource storageResource, long logSize)
		{
			ExecuteAndCatchWmiExceptions(delegate
			{
				Connect(null, delegate(CimSession remoteSession)
				{
					SetReplicationGroupLogSize(remoteSession, storageResource.Cluster.Name, storageResource.ReplicationInfo.ReplicationGroupName, logSize);
					ReplicationInfo replicationInfo = new ReplicationInfo(storageResource.Cluster, storageResource.ReplicationInfo.ReplicationGroupName, storageResource.ReplicationInfo.Description, storageResource.ReplicationInfo.ReplicationType, storageResource.ReplicationInfo.ReplicationStatus, storageResource.ReplicationInfo.ReplicationPrivateStorageResources, logSize, storageResource.ReplicationInfo.ContainerSize, storageResource.ReplicationInfo.MultiplicationFactor, storageResource.ReplicationInfo.MinLogSize, storageResource.ReplicationInfo.IsConsistencyEnabled);
					storageResource.ReplicationInfo = replicationInfo;
				});
			}, storageResource.Name);
		}

		private void SetReplicationGroupLogSize(CimSession session, string computerName, string replicationGroup, long logSize)
		{
			ClusterLog.LogInfo("Setting replication log size for replication group '{0}'", replicationGroup);
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("ComputerName", computerName);
			CimArgumentNameValue cimArgumentNameValue2 = new CimArgumentNameValue("Name", replicationGroup);
			CimArgumentNameValue cimArgumentNameValue3 = new CimArgumentNameValue("LogSizeInBytes", (ulong)logSize);
			InvokeMethod(GetReplicaAdminClass(session), "SetGroupModifyConfig", new CimArgumentNameValue[3] { cimArgumentNameValue, cimArgumentNameValue2, cimArgumentNameValue3 }, delegate(CimMethodResult result)
			{
				CimMethodParameter returnValue = result.ReturnValue;
				if ((uint)returnValue.Value != 0)
				{
					ClusterWmiWin32Exception ex = new ClusterWmiWin32Exception((int)(uint)returnValue.Value, null);
					ClusterLog.LogException(ex, "There was an error setting replication log size");
					throw ex;
				}
			}, session, "root\\Microsoft\\Windows\\StorageReplica", TimeSpan.FromMinutes(5.0));
		}

		public void RemoveReplication(PStorageResource storageResource, bool fullCleanUp)
		{
			PCluster privateCluster = storageResource.Cluster;
			if (!privateCluster.CacheManager.ReplicatedResources.TryGetValue(storageResource.Id, out var replicatedDisk))
			{
				throw new ClusterErrorException("clustername");
			}
			ExecuteAndCatchWmiExceptions(delegate
			{
				Connect(null, delegate(CimSession localSession)
				{
					Connect(storageResource.OwnerGroup.OwnerNode.Fqdn, delegate(CimSession remoteSession)
					{
						GetReplicationGroup(remoteSession, storageResource, delegate(CimInstance replicationGroupInstance)
						{
							bool num = privateCluster.CacheManager.ReplicatedResources.Values.Count((NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK _) => _.ReplicationGroupId == replicatedDisk.ReplicationGroupId && _.Role == ReplicationDiskType.Source) == 1;
							string sourceReplicationGroupName = (string)replicationGroupInstance.CimInstanceProperties["Name"].Value;
							string sourceComputerName = (string)replicationGroupInstance.CimInstanceProperties["ComputerName"].Value;
							if (num)
							{
								string destinationReplicationGroupName;
								GetReplicationPartnership(remoteSession, replicationGroupInstance, ReplicationGroupRole.Primary, delegate(CimInstance cimPartnership)
								{
									destinationReplicationGroupName = (string)cimPartnership.CimInstanceProperties["DestinationRGName"].Value;
									string value2 = (string)cimPartnership.CimInstanceProperties["DestinationComputerName"].Value;
									CimArgumentNameValue cimArgumentNameValue4 = new CimArgumentNameValue("DestinationComputerName", value2);
									CimArgumentNameValue cimArgumentNameValue5 = new CimArgumentNameValue("DestinationRGName", destinationReplicationGroupName);
									CimArgumentNameValue cimArgumentNameValue6 = new CimArgumentNameValue("SourceComputerName", sourceComputerName);
									CimArgumentNameValue cimArgumentNameValue7 = new CimArgumentNameValue("SourceRGName", sourceReplicationGroupName);
									InvokeMethod(GetReplicaAdminClass(localSession), "RemovePartnership", new CimArgumentNameValue[4] { cimArgumentNameValue4, cimArgumentNameValue5, cimArgumentNameValue6, cimArgumentNameValue7 }, delegate(CimMethodResult result)
									{
										CimMethodParameter returnValue2 = result.ReturnValue;
										if ((uint)returnValue2.Value != 0)
										{
											ClusterWmiWin32Exception ex2 = new ClusterWmiWin32Exception((int)(uint)returnValue2.Value, null);
											ClusterLog.LogException(ex2, "There was an error removing replication");
											throw ex2;
										}
										try
										{
											DeleteReplicationGroup(remoteSession, destinationReplicationGroupName);
										}
										catch (Exception exception)
										{
											ClusterLog.LogException(exception, "Failed to delete destination replication group");
										}
										try
										{
											DeleteReplicationGroup(remoteSession, sourceReplicationGroupName);
										}
										catch (Exception exception2)
										{
											ClusterLog.LogException(exception2, "Failed to delete destination replication group");
										}
									}, localSession, "root\\Microsoft\\Windows\\StorageReplica", TimeSpan.FromMinutes(5.0));
								});
							}
							else
							{
								string[] value = storageResource.DiskInfo.Partitions.Select((ClusterDiskPartition _) => (!_.IsCsvFs) ? ("\\\\?\\Volume" + _.VolumeGuid.ToString("B")) : _.Name).ToArray();
								CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("ComputerName", storageResource.OwnerGroup.OwnerNode.Name);
								CimArgumentNameValue cimArgumentNameValue2 = new CimArgumentNameValue("Name", sourceReplicationGroupName);
								CimArgumentNameValue cimArgumentNameValue3 = new CimArgumentNameValue("RemoveVolumeName", value);
								InvokeMethod(GetReplicaAdminClass(localSession), "SetGroupRemoveVolumes", new CimArgumentNameValue[3] { cimArgumentNameValue, cimArgumentNameValue2, cimArgumentNameValue3 }, delegate(CimMethodResult result)
								{
									CimMethodParameter returnValue = result.ReturnValue;
									if ((uint)returnValue.Value != 0)
									{
										ClusterWmiWin32Exception ex = new ClusterWmiWin32Exception((int)(uint)returnValue.Value, null);
										ClusterLog.LogException(ex, "There was an error removing replication");
										throw ex;
									}
								}, localSession, "root\\Microsoft\\Windows\\StorageReplica", TimeSpan.FromMinutes(5.0));
							}
						});
					});
				});
			}, storageResource.Name);
		}

		public IEnumerable<Guid> GetReplicationGroupPartnership(PNode ownerNode, Guid replicationGroupId, ReplicationGroupRole role)
		{
			List<Guid> replicationGroups = new List<Guid>();
			ExecuteAndCatchWmiExceptions(delegate
			{
				Connect(ownerNode.Fqdn, delegate(CimSession remoteSession)
				{
					GetReplicationGroup(remoteSession, replicationGroupId, delegate(CimInstance replicationGroup)
					{
						GetReplicationPartnership(remoteSession, replicationGroup, role, delegate(CimInstance partnerShipInstance)
						{
							string replicationGroup2 = ((role == ReplicationGroupRole.Primary) ? ((string)partnerShipInstance.CimInstanceProperties["DestinationRGName"].Value) : ((string)partnerShipInstance.CimInstanceProperties["SourceRGName"].Value));
							GetReplicationGroup(remoteSession, replicationGroup2, delegate(CimInstance replicationGroupPartner)
							{
								string g = (string)replicationGroupPartner.CimInstanceProperties["Id"].Value;
								Guid item = new Guid(g);
								replicationGroups.Add(item);
							});
						});
					});
				});
			}, ownerNode.Name);
			return replicationGroups;
		}

		public IEnumerable<string> GetReplicationGroupPartnership(PNode ownerNode, string replicationGroupName, ReplicationGroupRole role)
		{
			List<string> replicationGroups = new List<string>();
			ExecuteAndCatchWmiExceptions(delegate
			{
				Connect(ownerNode.Fqdn, delegate(CimSession remoteSession)
				{
					GetReplicationPartnership(remoteSession, replicationGroupName, ownerNode.Cluster.Name, role, delegate(CimInstance partnerShipInstance)
					{
						string item = ((role == ReplicationGroupRole.Primary) ? ((string)partnerShipInstance.CimInstanceProperties["DestinationRGName"].Value) : ((string)partnerShipInstance.CimInstanceProperties["SourceRGName"].Value));
						replicationGroups.Add(item);
					});
				});
			}, ownerNode.Name);
			return replicationGroups;
		}

		public void LoadReplicationInfo(PStorageResource resource)
		{
			PCluster cluster = resource.Cluster;
			PGroup ownerGroup = resource.OwnerGroup;
			PNode privateNode = ownerGroup.OwnerNode;
			if (!cluster.CacheManager.ReplicatedResources.TryGetValue(resource.Id, out var replicatedDisk))
			{
				return;
			}
			ExecuteAndCatchWmiExceptions(delegate
			{
				Connect(privateNode.Fqdn, delegate(CimSession remoteSession)
				{
					GetReplicationGroup(remoteSession, replicatedDisk.ReplicationGroupId, delegate(CimInstance replicationGroup)
					{
						string text = (string)replicationGroup.CimInstanceProperties["Name"].Value;
						string description = (string)replicationGroup.CimInstanceProperties["Description"].Value;
						ReplicationType replicationType = (ReplicationType)(uint)replicationGroup.CimInstanceProperties["ReplicationMode"].Value;
						CimProperty cimProperty = replicationGroup.CimInstanceProperties["ReplicationStatus"];
						uint num = (uint)((cimProperty != null && cimProperty.Value != null) ? cimProperty.Value : ((object)0u));
						bool isConsistencyEnabled = (bool)replicationGroup.CimInstanceProperties["IsWriteConsistency"].Value;
						long logSize = (long)(ulong)replicationGroup.CimInstanceProperties["LogSizeInBytes"].Value;
						ReplicationStatus replicationStatus = (ReplicationStatus)num;
						CimInstance[] obj = (CimInstance[])replicationGroup.CimInstanceProperties["Replicas"].Value;
						List<ReplicationStatusInfo> list = new List<ReplicationStatusInfo>();
						CimInstance[] array = obj;
						foreach (CimInstance cimInstance in array)
						{
							if (cimInstance.CimInstanceProperties["ReplicationStatus"] != null && cimInstance.CimInstanceProperties["ReplicationStatus"].Value != null)
							{
								Guid partitionId = new Guid((string)cimInstance.CimInstanceProperties["PartitionId"].Value);
								uint replicationStatus2 = (uint)cimInstance.CimInstanceProperties["ReplicationStatus"].Value;
								list.Add(new ReplicationStatusInfo(partitionId, (ReplicationStatus)replicationStatus2));
							}
						}
						ClusterLog.LogInfo("Getting replication group information for group '{0}'", text);
						ReplicationInfo replicationInfo = new ReplicationInfo(cluster, text, description, replicationType, replicationStatus, new List<Guid>(), logSize, 0L, 0, 0L, isConsistencyEnabled);
						resource.ReplicationInfo = replicationInfo;
						resource.ReplicationStatus = list;
					});
				});
			}, privateNode.Name);
		}

		private void DeleteReplicationGroup(CimSession session, string replicationGroupName)
		{
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("ReplicationGroupName", replicationGroupName);
			CimArgumentNameValue cimArgumentNameValue2 = new CimArgumentNameValue("FullCleanup", true);
			ClusterLog.LogInfo("Deleting replication group '{0}'", cimArgumentNameValue);
			InvokeMethod(GetReplicaAdminClass(session), "WvrDeleteReplicationGroup", new CimArgumentNameValue[2] { cimArgumentNameValue, cimArgumentNameValue2 }, delegate(CimMethodResult result)
			{
				CimMethodParameter returnValue = result.ReturnValue;
				if ((uint)returnValue.Value != 0)
				{
					ClusterWmiWin32Exception ex = new ClusterWmiWin32Exception((int)(uint)returnValue.Value, null);
					ClusterLog.LogException(ex, "There was an error deleting the replication group");
					throw ex;
				}
			}, session, "root\\Microsoft\\Windows\\StorageReplica", TimeSpan.FromMinutes(5.0));
		}

		private void GetReplicationGroup(CimSession session, PStorageResource storageResource, Action<CimInstance> action)
		{
			PCluster cluster = storageResource.Cluster;
			ClusterLog.LogInfo("Getting replication group for resource '{0}' from the cache", storageResource.Name);
			if (!cluster.CacheManager.ReplicatedResources.TryGetValue(storageResource.Id, out var value))
			{
				ClusterLog.LogError("Replication group for resource '{0}' not found in the cache", storageResource.Name);
			}
			else
			{
				GetReplicationGroup(session, value.ReplicationGroupName, action);
			}
		}

		private IEnumerable<CimInstance> ExecuteReplicationQuery(CimSession session, string query)
		{
			Exceptions.ThrowIfNull(query, "query");
			Exceptions.ThrowIfNull(session, "session");
			ClusterLog.LogInfo("Executing Query in replication namespace to execute query {{0}}", "root\\Microsoft\\Windows\\StorageReplica", query);
			using TraceWmiQuery queryTracer = ClusterLog.StartTraceQuery(Microsoft.FailoverClusters.UI.Common.TraceSource.Framework, query);
			int instances = 0;
			using (CimOperationOptions options = new CimOperationOptions())
			{
				options.Timeout = TimeSpan.FromSeconds(60.0);
				foreach (CimInstance item in session.QueryInstances("root\\Microsoft\\Windows\\StorageReplica", "WQL", query, options))
				{
					instances++;
					yield return item;
				}
			}
			queryTracer.Instances = instances;
			queryTracer.Tag = "Sync Query";
		}

		private void GetReplicationGroup(CimSession session, string replicationGroup, Action<CimInstance> action)
		{
			ClusterLog.LogInfo("Getting replication group '{0}'", replicationGroup);
			using (CimInstance cimInstance = ExecuteReplicationQuery(session, "Select * from MSFT_WvrReplicationGroup where Name='{0}'".FormatInvariantCulture(replicationGroup)).FirstOrDefault())
			{
				if (cimInstance != null)
				{
					action.SafeCall(cimInstance);
					return;
				}
			}
			ClusterLog.LogWarning("Replication group '{0}' not found", replicationGroup);
		}

		private void GetReplicationGroup(CimSession session, Guid replicationId, Action<CimInstance> action)
		{
			ClusterLog.LogInfo("Getting replication group with Id '{0}'", replicationId);
			using (CimInstance cimInstance = ExecuteReplicationQuery(session, "Select * from MSFT_WvrReplicationGroup where Id='{0}'".FormatInvariantCulture(replicationId.ToString())).FirstOrDefault())
			{
				if (cimInstance != null)
				{
					action.SafeCall(cimInstance);
					return;
				}
			}
			ClusterLog.LogWarning("Replication group with Id '{0}' not found", replicationId);
		}

		private bool GetReplicationPartnership(CimSession session, CimInstance replicationGroup, ReplicationGroupRole role, Action<CimInstance> action)
		{
			string text = (string)replicationGroup.CimInstanceProperties["Name"].Value;
			string text2 = (string)replicationGroup.CimInstanceProperties["ComputerName"].Value;
			ClusterLog.LogInfo("Getting replication partnerships for replication group '{0}'", text);
			int num = 0;
			foreach (CimInstance item in ExecuteReplicationQuery(session, (role == ReplicationGroupRole.Primary) ? "Select * from MSFT_WvrReplicationPartnership where SourceRGName='{0}' and SourceComputerName='{1}'".FormatInvariantCulture(text, text2) : "Select * from MSFT_WvrReplicationPartnership where DestinationRGName='{0}' and DestinationComputerName='{1}'".FormatInvariantCulture(text, text2)))
			{
				num++;
				string text3 = (string)item.CimInstanceProperties["DestinationRGName"].Value;
				ClusterLog.LogInfo("Found replication partnership for replication group '{0}'", text3);
				action.SafeCall(item);
			}
			if (num == 0)
			{
				ClusterLog.LogWarning("There are no partnership for replication group '{0}'", text);
			}
			return num > 0;
		}

		private bool GetReplicationPartnership(CimSession session, string replicationGroupName, string computerName, ReplicationGroupRole role, Action<CimInstance> action)
		{
			ClusterLog.LogInfo("Getting replication partnerships for replication group '{0}'", replicationGroupName);
			int num = 0;
			foreach (CimInstance item in ExecuteReplicationQuery(session, (role == ReplicationGroupRole.Primary) ? "Select * from MSFT_WvrReplicationPartnership where SourceRGName='{0}' and SourceComputerName='{1}'".FormatInvariantCulture(replicationGroupName, computerName) : "Select * from MSFT_WvrReplicationPartnership where DestinationRGName='{0}' and DestinationComputerName='{1}'".FormatInvariantCulture(replicationGroupName, computerName)))
			{
				num++;
				string text = (string)item.CimInstanceProperties["DestinationRGName"].Value;
				ClusterLog.LogInfo("Found replication partnership for replication group '{0}'", text);
				action.SafeCall(item);
			}
			if (num == 0)
			{
				ClusterLog.LogWarning("There are no partnership for replication group '{0}'", replicationGroupName);
			}
			return num > 0;
		}

		private CimClass GetReplicaAdminClass(CimSession session)
		{
			return session.GetClass("root\\Microsoft\\Windows\\StorageReplica", "MSFT_WvrAdminTasks");
		}

		private void Connect(string computerName, Action<CimSession> action)
		{
			ClusterLog.LogInfo("Connecting to {0}", computerName ?? "localhost");
			using CimSession cimSession = CimSession.Create(computerName);
			if (!cimSession.TestConnection(out var _, out var exception))
			{
				ClusterLog.LogException(exception, "There was an error connecting to CIM on machine {0}".FormatInvariantCulture(computerName ?? "localhost"));
				throw exception;
			}
			action.SafeCall(cimSession);
		}

		public uint? GetDiskNumber(PStorageResource storageResourcePrivate, string uniqueId, string nodeName)
		{
			using CimSession session = CimSession.Create(nodeName);
			return MSFT_Disk.Enumerate(session).FirstOrDefault((MSFT_Disk d) => d.UniqueId == uniqueId)?.Number;
		}

		public string GetUniqueId(uint diskNumber, string nodeName)
		{
			using CimSession session = CimSession.Create(nodeName);
			MSFT_Disk mSFT_Disk = MSFT_Disk.Enumerate(session).FirstOrDefault((MSFT_Disk d) => d.Number == diskNumber);
			return (mSFT_Disk != null) ? mSFT_Disk.UniqueId : string.Empty;
		}

		public T1 GetInstance<T1>(string key, string serverName = null) where T1 : IKeyQueryable
		{
			if (Instances.TryGetValue(typeof(T1), out var value))
			{
				if (serverName == null)
				{
					return (T1)value(base.CimAdapter.Scope, key, base.CimAdapter.clusters.Cluster.GetProxy());
				}
				using CimSession arg = CimSession.Create(serverName);
				return (T1)value(arg, key, base.CimAdapter.clusters.Cluster.GetProxy());
			}
			return default(T1);
		}

		public IEnumerable<T1> Enumerate<T1>(ObservableKeyCollection<T1> collection, ObservableCollectionFilter<T1> filter) where T1 : IKeyQueryable<T1>
		{
			MSFT_StorageSubSystem clusterStorageSubSystem = GetClusterStorageSubSystem(base.CimAdapter.Scope);
			IEnumerable<T1> enumerable;
			if (filter == null || string.IsNullOrWhiteSpace(filter.FilterQuery))
			{
				if (!Enumerations.TryGetValue(collection.ItemType, out var value))
				{
					throw new ArgumentException("Enumeration for type {0} is not supported".FormatInvariantCulture(typeof(T1).Name));
				}
				enumerable = (IEnumerable<T1>)value(base.CimAdapter.Scope, collection.Cluster, clusterStorageSubSystem);
			}
			else
			{
				if (!Queries.TryGetValue(collection.ItemType, out var value2))
				{
					throw new ArgumentException("Query for type {0} is not supported".FormatInvariantCulture(typeof(T1).Name));
				}
				enumerable = (IEnumerable<T1>)value2(base.CimAdapter.Scope, filter.FilterQuery, collection.Cluster, clusterStorageSubSystem);
			}
			foreach (T1 item in enumerable)
			{
				if (filter == null || filter.FilterFx == null || filter.FilterFx(item))
				{
					yield return item;
				}
			}
		}

		public IEnumerable<T1> Association<T, T1>(ObservableKeyCollection<T1> collection, T association) where T1 : IKeyQueryable<T1>
		{
			IEnumerable<T1> enumerable = null;
			Node node;
			string arg;
			if ((node = association as Node) != null)
			{
				arg = GetStorageNode(base.CimAdapter.Scope, node).ObjectId;
			}
			else
			{
				if (!((object)association is IKeyQueryable keyQueryable))
				{
					throw new ArgumentException("Association type {0} it doesn't have a keyable property".FormatInvariantCulture(typeof(T).Name));
				}
				arg = keyQueryable.Key;
			}
			if (Associations.TryGetValue(typeof(T), out var value) && value.TryGetValue(collection.ItemType, out var value2))
			{
				enumerable = (IEnumerable<T1>)value2(base.CimAdapter.Scope, arg, collection.Cluster);
			}
			if (enumerable == null)
			{
				throw new NotSupportedException("Association {0} not supported to {1}".FormatCurrentCulture(typeof(T).Name, typeof(T1)));
			}
			foreach (T1 item in enumerable)
			{
				yield return item;
			}
		}

		public void Subscribe<T1>(ObservableKeyCollection<T1> collection) where T1 : IKeyQueryable<T1>
		{
			SubscriptionTask subscriptionTask = new SubscriptionTask(base.CimAdapter.Scope);
			Subscriptions.TryAdd(subscriptionTask, subscriptionTask);
			subscriptionTask.Subscribe(collection);
		}

		public void Unsubscribe<T1>(ObservableKeyCollection<T1> collection) where T1 : IKeyQueryable<T1>
		{
		}

		private MSFT_StorageNode GetStorageNode(CimSession session, Node node)
		{
			string fqdnName = base.CimAdapter.clusters.Cluster.FqdnName;
			MSFT_StorageSubSystem clusterStorageSubSystem = GetClusterStorageSubSystem(session);
			string nodeName = base.CimAdapter.ReplaceClusterNameWithNodeName(node.Name);
			MSFT_StorageNode mSFT_StorageNode = clusterStorageSubSystem.MSFT_StorageSubSystemToStorageNode.StorageNode.FirstOrDefault((MSFT_StorageNode enumNode) => enumNode.Name.Equals(nodeName, StringComparison.CurrentCultureIgnoreCase));
			if (mSFT_StorageNode == null)
			{
				ClusterLog.LogWarning("Cluster subsystem to storage node association did not return any record for cluster {0} on node {1}", fqdnName, nodeName);
				throw new InvalidOperationException("Cluster subsystem to storage node association did not return any record for cluster {0} on node {1}".FormatCurrentCulture(fqdnName, nodeName));
			}
			return mSFT_StorageNode;
		}

		private MSFT_StorageSubSystem GetClusterStorageSubSystem(CimSession session)
		{
			string fqdnName = base.CimAdapter.clusters.Cluster.FqdnName;
			MSFT_StorageSubSystem mSFT_StorageSubSystem = MSFT_StorageSubSystem.Query(session, "name = '{0}'".FormatInvariantCulture(fqdnName)).FirstOrDefault();
			if (mSFT_StorageSubSystem == null)
			{
				string text = base.CimAdapter.ReplaceClusterNameWithNodeName(base.CimAdapter.clusters.Cluster.ConnectedTo);
				ClusterLog.LogWarning("Cluster subsystem was not found on SM-API for cluster {0} on node {1}", fqdnName, text);
				throw new InvalidOperationException("Cluster subsystem was not found on SM-API for cluster {0} on node {1}".FormatCurrentCulture(fqdnName, text));
			}
			return mSFT_StorageSubSystem;
		}

		private static MSFT_PhysicalDisk GetPhysicalDiskInstance(SubscriptionOperation operation, CimSession session, string objectId)
		{
			if (operation == SubscriptionOperation.Delete)
			{
				return MSFT_PhysicalDisk.CreateReference(session, objectId);
			}
			return MSFT_PhysicalDisk.GetInstance(session, objectId);
		}

		private static MSFT_VirtualDisk GetVirtualDiskInstance(SubscriptionOperation operation, CimSession session, string objectId)
		{
			if (operation == SubscriptionOperation.Delete)
			{
				return MSFT_VirtualDisk.CreateReference(session, objectId);
			}
			return MSFT_VirtualDisk.GetInstance(session, objectId);
		}

		private static MSFT_StorageEnclosure GetEnclosureInstance(SubscriptionOperation operation, CimSession session, string objectId)
		{
			if (operation == SubscriptionOperation.Delete)
			{
				return MSFT_StorageEnclosure.CreateReference(session, objectId);
			}
			return MSFT_StorageEnclosure.GetInstance(session, objectId);
		}

		private static MSFT_StorageNode GetStorageNodeInstance(SubscriptionOperation operation, CimSession session, string objectId)
		{
			if (operation == SubscriptionOperation.Delete)
			{
				return MSFT_StorageNode.CreateReference(session, objectId);
			}
			return MSFT_StorageNode.GetInstance(session, objectId);
		}

		private static MSFT_Disk GetDiskInstance(SubscriptionOperation operation, CimSession session, string objectId)
		{
			if (operation == SubscriptionOperation.Delete)
			{
				return MSFT_Disk.CreateReference(session, objectId);
			}
			return MSFT_Disk.GetInstance(session, objectId);
		}

		public override void Collect()
		{
			foreach (SubscriptionTask key in Subscriptions.Keys)
			{
				if (key.Collect())
				{
					Subscriptions.TryRemove(key, out var _);
				}
			}
		}
	}

	private class ResourceTypeAdapter : AdapterBase, IConnectionAdapterResourceType
	{
		public override CimClass IdentityClass => base.CimResourceType;

		public override string ElementaryPayloadQuery => "name";

		public override string BasicPayloadQuery => ",resourceClass";

		public override string CommonPropertiesQuery => ",DllName,Name,Description,AdminExtensions,LooksAlivePollInterval,IsAlivePollInterval,PendingTimeout,DeadlockTimeout";

		public ResourceTypeAdapter(CimAdapter cimAdapter)
			: base(cimAdapter)
		{
		}

		public PResourceType Open(string name)
		{
			return GetInstance(name, (CimInstance instance) => CreateFromCimInstance(instance, ResourceTypeLoadSelection.Basic | ResourceTypeLoadSelection.CommonProperties));
		}

		public IEnumerable<PResourceType> GetAll(IList<string> queryFields, bool nullElementOnError)
		{
			return from instance in base.CimAdapter.ExecuteOptimalQuery("Select ElementName from MSFTCluster_ResourceType", nullElementOnError)
				select CreateFromCimInstance(instance, ResourceTypeLoadSelection.None);
		}

		internal PResourceType CreateFromCimInstance(CimInstance instance, ResourceTypeLoadSelection loadSelection)
		{
			if (instance == null)
			{
				return null;
			}
			string resourceTypeName = (string)instance.CimInstanceProperties["ElementName"].Value;
			PResourceType pResourceType = new PResourceType(base.CimAdapter.clusters.Cluster, resourceTypeName);
			PopulateFromCimInstance(instance, pResourceType, loadSelection);
			return pResourceType;
		}

		private void PopulateFromCimInstance(CimInstance instance, PResourceType resourceType, ResourceTypeLoadSelection loadSelection)
		{
			Exceptions.ThrowIfNull(instance, "instance");
			Exceptions.ThrowIfNull(resourceType, "resourceType");
			ExecuteAndCatchWmiExceptions(delegate
			{
				if ((loadSelection & ResourceTypeLoadSelection.CommonProperties) == ResourceTypeLoadSelection.CommonProperties || (loadSelection & ResourceTypeLoadSelection.Basic) == ResourceTypeLoadSelection.Basic)
				{
					ParseProperties(resourceType.Properties, instance.CimInstanceProperties, ClusterPropertyKind.Common);
					object value = instance.CimInstanceProperties["ResourceClass"].Value;
					if (value != null)
					{
						resourceType.Class = (ResourceClass)(uint)value;
						resourceType.IsStorage = resourceType.Class == ResourceClass.Storage;
					}
					resourceType.LoadedSelection |= 3;
				}
			}, resourceType.Name);
		}

		public void Load(PResourceType resourceType, ResourceTypeLoadSelection loadSelection)
		{
			try
			{
				ExecuteAndCatchWmiExceptions(delegate
				{
					if ((loadSelection & ResourceTypeLoadSelection.CommonProperties) == ResourceTypeLoadSelection.CommonProperties || (loadSelection & ResourceTypeLoadSelection.Basic) == ResourceTypeLoadSelection.Basic)
					{
						GetInstance(base.CimResourceType, resourceType.Name, delegate(CimInstance instance)
						{
							PopulateFromCimInstance(instance, resourceType, loadSelection);
						});
					}
					if ((loadSelection & ResourceTypeLoadSelection.PrivateProperties) == ResourceTypeLoadSelection.PrivateProperties)
					{
						base.CimAdapter.GetPrivateProperties(resourceType);
					}
				}, resourceType.Name);
			}
			catch (Exception innerException)
			{
				throw new ClusterObjectLoadFailedException(resourceType.Name, resourceType.Id, innerException);
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
			return InvokeMethod(name, "GetPossibleOwners", null, (CimMethodResult returnInstance) => ((CimInstance[])(returnInstance.OutParameters["PossibleOwners"] ?? throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("PossibleOwners"))).Value).Select((CimInstance instance) => (string)instance.CimInstanceProperties["Name"].Value));
		}

		public PResourceType Create(string name, string displayName, string pathDll)
		{
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Name", name);
			CimArgumentNameValue cimArgumentNameValue2 = new CimArgumentNameValue("DisplayName", displayName);
			CimArgumentNameValue cimArgumentNameValue3 = new CimArgumentNameValue("ResourceTypeDllFilePath", pathDll);
			return InvokeMethod(base.CimAdapter.CimResourceType, "Create", new CimArgumentNameValue[3] { cimArgumentNameValue, cimArgumentNameValue2, cimArgumentNameValue3 }, delegate(CimMethodResult returnInstance)
			{
				CimInstance cimInstance = (CimInstance)(returnInstance.OutParameters["ResourceType"] ?? throw new InvalidOperationException(ExceptionResources.OutArgumentNullReference.FormatCurrentCulture("ResourceType"))).Value;
				return Open((string)cimInstance.CimInstanceProperties["ElementName"].Value);
			});
		}

		public void Delete(string resourceType)
		{
			InvokeMethod(resourceType, "Destroy", null);
		}

		public override void Collect()
		{
		}
	}

	private readonly ClusterAdapter clusters;

	private readonly GroupAdapter groups;

	private readonly NodeAdapter nodes;

	private readonly NetworkAdapter networks;

	private readonly NetworkInterfaceAdapter networkInterfaces;

	private readonly ResourceAdapter resources;

	private readonly StorageAdapter storage;

	private readonly ResourceTypeAdapter resourceTypes;

	private readonly ConcurrentDictionary<ClusterIdentityType, Func<CimInstance, int, PClusterObject>> processResult = new ConcurrentDictionary<ClusterIdentityType, Func<CimInstance, int, PClusterObject>>();

	private readonly ConcurrentDictionary<ClusterIdentityType, AdapterBase> adapters = new ConcurrentDictionary<ClusterIdentityType, AdapterBase>();

	private readonly ConcurrentDictionary<ClusterIdentityType, CimClass> cimClasses = new ConcurrentDictionary<ClusterIdentityType, CimClass>();

	private static readonly ConcurrentDictionary<OperatorType, string> OperatorsMap;

	protected CimClass CimCluster { get; private set; }

	protected CimClass CimClusterService { get; private set; }

	protected CimClass CimGroup { get; private set; }

	protected CimClass CimResource { get; private set; }

	protected CimClass CimNode { get; private set; }

	protected CimClass CimNetwork { get; private set; }

	protected CimClass CimNetworkInterface { get; private set; }

	protected CimClass CimResourceType { get; private set; }

	protected CimClass CimQuorumSettings { get; private set; }

	protected CimClass CimStorageService { get; private set; }

	protected CimClass CimDiskInfo { get; private set; }

	protected CimClass CimVolumeInfo { get; private set; }

	protected CimClass CimNetworkService { get; private set; }

	private AdapterBase CommonAdapter { get; set; }

	private CimSession Scope { get; set; }

	protected bool QueriesAreAsync { get; private set; }

	public IConnectionAdapterCluster Cluster => clusters;

	public ClusterAdapterType Adapter => ClusterAdapterType.Cim;

	public IConnectionAdapterGroup Group => groups;

	public IConnectionAdapterResource Resource => resources;

	public IConnectionAdapterNode Node => nodes;

	public IConnectionAdapterNetwork Network => networks;

	public IConnectionAdapterStorage Storage => storage;

	public IConnectionAdapterNetworkInterface NetworkInterface => networkInterfaces;

	public IConnectionAdapterResourceType ResourceType => resourceTypes;

	static CimAdapter()
	{
		OperatorsMap = new ConcurrentDictionary<OperatorType, string>();
		OperatorsMap.TryAdd(OperatorType.And, " and ");
		OperatorsMap.TryAdd(OperatorType.Equal, " = ");
		OperatorsMap.TryAdd(OperatorType.GreaterThan, " > ");
		OperatorsMap.TryAdd(OperatorType.GreaterThanOrEqual, " >= ");
		OperatorsMap.TryAdd(OperatorType.Is, " is ");
		OperatorsMap.TryAdd(OperatorType.IsNot, " is not ");
		OperatorsMap.TryAdd(OperatorType.LessThan, " < ");
		OperatorsMap.TryAdd(OperatorType.LessThanOrEqual, " <= ");
		OperatorsMap.TryAdd(OperatorType.NotEqual, " != ");
		OperatorsMap.TryAdd(OperatorType.Or, " or ");
		OperatorsMap.TryAdd(OperatorType.Contains, " like ");
		OperatorsMap.TryAdd(OperatorType.StartsWith, " like ");
		OperatorsMap.TryAdd(OperatorType.EndsWith, " like ");
	}

	public CimAdapter(PCluster cluster)
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
		processResult.TryAdd(ClusterIdentityType.Group, (CimInstance cimInstance, int loadSelection) => groups.CreateFromCimInstance(cimInstance, (GroupLoadSelection)loadSelection));
		processResult.TryAdd(ClusterIdentityType.Resource, (CimInstance cimInstance, int loadSelection) => resources.CreateFromCimInstance(cimInstance, (ResourceLoadSelection)loadSelection));
		processResult.TryAdd(ClusterIdentityType.Network, (CimInstance cimInstance, int loadSelection) => networks.CreateFromCimInstance(cimInstance, (NetworkLoadSelection)loadSelection));
		processResult.TryAdd(ClusterIdentityType.NetworkInterface, (CimInstance cimInstance, int loadSelection) => networkInterfaces.CreateFromCimInstance(cimInstance, (NetworkInterfaceLoadSelection)loadSelection));
		processResult.TryAdd(ClusterIdentityType.Node, (CimInstance cimInstance, int loadSelection) => nodes.CreateFromCimInstance(cimInstance, (NodeLoadSelection)loadSelection));
		processResult.TryAdd(ClusterIdentityType.ResourceType, (CimInstance cimInstance, int loadSelection) => resourceTypes.CreateFromCimInstance(cimInstance, (ResourceTypeLoadSelection)loadSelection));
		adapters.TryAdd(ClusterIdentityType.Cluster, clusters);
		adapters.TryAdd(ClusterIdentityType.Group, groups);
		adapters.TryAdd(ClusterIdentityType.Resource, resources);
		adapters.TryAdd(ClusterIdentityType.Network, networks);
		adapters.TryAdd(ClusterIdentityType.NetworkInterface, networkInterfaces);
		adapters.TryAdd(ClusterIdentityType.Node, nodes);
		adapters.TryAdd(ClusterIdentityType.ResourceType, resourceTypes);
		adapters.TryAdd(ClusterIdentityType.Storage, storage);
	}

	public void Close()
	{
		Cluster.Close();
	}

	public void Collect()
	{
		adapters.Values.ForEach(delegate(AdapterBase adapter)
		{
			adapter.Collect();
		});
	}

	public void SetupEnvironment()
	{
		using CimOperationOptions cimOperationOptions = new CimOperationOptions();
		cimOperationOptions.Timeout = TimeSpan.FromSeconds(15.0);
	}

	public IEnumerable<PClusterObject> Select<TInput>(IClusterList<TInput> query) where TInput : ClusterObject
	{
		return Select<PClusterObject>(((ClusterList<TInput>)query).QueryInfo);
	}

	public IEnumerable<TResult> Select<TResult>(QueryInfo queryInfo) where TResult : PClusterObject
	{
		bool onlyCsvs = false;
		int loadSelection;
		string text = FormatHelper.FormatColumnNamesInSequence(adapters[queryInfo.IdentityType].NormalizeQuery(GetQueryFields(queryInfo), out loadSelection).ToArray());
		string className = cimClasses[queryInfo.IdentityType].CimSystemProperties.ClassName;
		string query;
		if (queryInfo.WhereLambdaExpression == null)
		{
			query = "select {0} from {1}".FormatCurrentCulture(text, className);
		}
		else
		{
			StringBuilder stringBuilder = new StringBuilder();
			OperatorArgument operatorArgument = null;
			List<IClusterQueryArgument> whereSyntaxis = queryInfo.WhereSyntaxis;
			string fieldArgument = string.Empty;
			foreach (IClusterQueryArgument item in whereSyntaxis)
			{
				if (item is StartEndArgument || item is FieldArgument)
				{
					if (!AppendFieldArgument(item, stringBuilder, ref fieldArgument))
					{
						stringBuilder.Append(item);
					}
				}
				else if (!AppendGuidArgument(item, stringBuilder, fieldArgument) && !AppendValueArgument(item, stringBuilder, fieldArgument, operatorArgument, className, out onlyCsvs) && (operatorArgument = AppendOperatorArgument(item, stringBuilder)) == null)
				{
					throw new NotSupportedException("Argument not supported");
				}
			}
			query = ((stringBuilder.Length == 0) ? "select {0} from {1}".FormatCurrentCulture(text, className) : "select {0} from {1} where {2}".FormatCurrentCulture(text, className, stringBuilder));
		}
		return ExecuteQuery<TResult>(query, queryInfo.IdentityType, onlyCsvs, loadSelection);
	}

	private bool AppendFieldArgument(IClusterQueryArgument argument, StringBuilder whereExpression, ref string fieldArgument)
	{
		if (argument is FieldArgument)
		{
			fieldArgument = argument.Name;
			switch (fieldArgument.ToLowerInvariant())
			{
			case "type":
				whereExpression.Append("typename");
				break;
			case "ownergroup":
				whereExpression.Append("OwnerGroupName");
				break;
			case "resourcekind":
				whereExpression.Append("TypeName");
				break;
			default:
				whereExpression.Append(argument);
				break;
			}
			return true;
		}
		return false;
	}

	private bool AppendGuidArgument(IClusterQueryArgument argument, StringBuilder whereExpression, string fieldArgument)
	{
		if (!(argument is GuidArgument guidArgument))
		{
			return false;
		}
		string text = fieldArgument.ToLowerInvariant();
		if (text == "ownernode" || text == "ownergroup")
		{
			whereExpression.Append("'");
			whereExpression.Append(guidArgument.Name);
			whereExpression.Append("'");
		}
		else
		{
			whereExpression.Append(argument);
		}
		return true;
	}

	private bool AppendValueArgument(IClusterQueryArgument argument, StringBuilder whereExpression, string fieldArgument, OperatorArgument operatorArgument, string cimClass, out bool onlyCsvs)
	{
		onlyCsvs = false;
		if (!(argument is ValueArgument valueArgument))
		{
			return false;
		}
		if (cimClass.Equals("MSFTCluster_resource", StringComparison.OrdinalIgnoreCase) && fieldArgument.Equals("resourcekind", StringComparison.OrdinalIgnoreCase))
		{
			ResourceKind resourceKind = (ResourceKind)int.Parse(argument.ToString(), NumberFormatInfo.InvariantInfo);
			if (resourceKind.HasFlag(ResourceKind.ClusterFileSystem))
			{
				resourceKind = ResourceKind.PhysicalDisk;
				onlyCsvs = true;
			}
			string arg = PResourceType.ResourceKindToString(resourceKind);
			whereExpression.Append("'{0}'".FormatCurrentCulture(arg));
		}
		else if (operatorArgument != null)
		{
			switch (operatorArgument.OperatorType)
			{
			case OperatorType.Contains:
				whereExpression.Append("'%{0}%'".FormatInvariantCulture(valueArgument.Value));
				break;
			case OperatorType.StartsWith:
				whereExpression.Append("'{0}%'".FormatInvariantCulture(valueArgument.Value));
				break;
			case OperatorType.EndsWith:
				whereExpression.Append("'%{0}'".FormatInvariantCulture(valueArgument.Value));
				break;
			default:
				whereExpression.Append(argument);
				break;
			}
		}
		else
		{
			whereExpression.Append(argument);
		}
		return true;
	}

	private OperatorArgument AppendOperatorArgument(IClusterQueryArgument argument, StringBuilder whereExpression)
	{
		if (!(argument is OperatorArgument operatorArgument))
		{
			return null;
		}
		if (!OperatorsMap.TryGetValue(operatorArgument.OperatorType, out var value))
		{
			throw new NotSupportedException("Operator not supported");
		}
		whereExpression.Append(value);
		return operatorArgument;
	}

	private IEnumerable<TResult> ExecuteQuery<TResult>(string query, ClusterIdentityType identityType, bool onlyCsvs, int loadSelection) where TResult : PClusterObject
	{
		foreach (CimInstance item in ExecuteOptimalQuery(query, nullElementOnError: false))
		{
			PClusterObject pClusterObject = processResult[identityType](item, loadSelection);
			if (!onlyCsvs || ((PResource)pClusterObject).ResourceType.ResourceKind == ResourceKind.ClusterFileSystem)
			{
				yield return (TResult)pClusterObject;
			}
		}
	}

	private static IEnumerable<ClusterObjectMetaDataMember> GetQueryMembers(QueryInfo queryInfo)
	{
		return queryInfo.ProjectionFields.Select((ClusterObjectMetaDataMember s) => s).Concat(queryInfo.WhereFields.Select((ClusterObjectMetaDataMember s) => s)).Concat(from s in queryInfo.OrderBy.ConvertAll((OrderByItem item) => item.DataMember)
			select (s))
			.Distinct();
	}

	private static IEnumerable<string> GetQueryFields(QueryInfo queryInfo)
	{
		foreach (ClusterObjectMetaDataMember queryMember in GetQueryMembers(queryInfo))
		{
			string text = queryMember.MappedName.ToLowerInvariant();
			if (text == "id")
			{
				yield return "ElementName";
				continue;
			}
			if (queryMember.Source.IsAssignableFrom(typeof(Group)))
			{
				text = queryMember.MappedName.ToLowerInvariant();
				if (text == "grouptype")
				{
					yield return "Type";
					continue;
				}
			}
			yield return queryMember.MappedName;
		}
	}

	[DebuggerNonUserCode]
	public Notification DequeueNotification(int milliSecondsTimeout)
	{
		throw new NotSupportedException("DequeueNotification is not implemented for CimAdapeter");
	}

	public void SaveProperties(PClusterObject clusterObject, ClusterPropertyCollection properties)
	{
		List<ClusterProperty> list = properties.Where((ClusterProperty property) => property.PropertyKind == ClusterPropertyKind.Common && property.IsModified).ToList();
		List<ClusterProperty> list2 = properties.Where((ClusterProperty property) => property.PropertyKind == ClusterPropertyKind.Private && property.IsModified).ToList();
		if (list.Count > 0)
		{
			SaveCommonProperties(clusterObject, list);
		}
		if (list2.Count > 0)
		{
			SavePrivateProperties(clusterObject, properties);
		}
	}

	private void SavePrivateProperties(PClusterObject clusterObject, ClusterPropertyCollection properties)
	{
		CommonAdapter.GetInstanceIdentity(cimClasses[clusterObject.IdentityType], clusterObject, delegate(CimInstance cimInstance)
		{
			CimInstance[] value = CommonAdapter.ParseCimPrivateProperties(properties);
			CimArgumentNameValue cimArgumentNameValue = new CimArgumentNameValue("Properties", value);
			InvokeMethod(cimInstance, "SaveProperties", new CimArgumentNameValue[1] { cimArgumentNameValue }, delegate(CimMethodResult returnInstance)
			{
				_ = returnInstance.OutParameters["Properties"];
			});
		});
	}

	private void SaveCommonProperties(PClusterObject clusterObject, List<ClusterProperty> commonProperties)
	{
		CommonAdapter.GetInstanceIdentity(cimClasses[clusterObject.IdentityType], clusterObject, delegate(CimInstance cimInstance)
		{
			commonProperties.ForEach(delegate(ClusterProperty modifiedProperty)
			{
				cimInstance.CimInstanceProperties[modifiedProperty.RealName ?? modifiedProperty.Name].Value = modifiedProperty.Value;
			});
			ModifyInstance(cimInstance);
		});
	}

	internal void GetPrivateProperties(PClusterObject clusterObject)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		CimClass cimClass = cimClasses[clusterObject.IdentityType];
		CommonAdapter.GetInstanceIdentity(cimClass, clusterObject, delegate(CimInstance instance)
		{
			InvokeMethod(instance, "GetProperties", null, delegate(CimMethodResult returnInstance)
			{
				CimMethodParameter cimMethodParameter = returnInstance.OutParameters["Properties"];
				if (cimMethodParameter != null)
				{
					if (!(cimMethodParameter.Value is CimInstance[] cimPropertyCollection))
					{
						ClusterLog.LogError("The method invocation 'GetProperties' failed to return right return type");
						return;
					}
					CommonAdapter.ParseProperties(clusterObject.Properties, cimPropertyCollection, ClusterPropertyKind.Private);
				}
				clusterObject.LoadedSelection |= 4;
			});
		});
	}

	public void GetFirst(CimClass cimClass, Action<CimInstance> action)
	{
		Exceptions.ThrowIfNull(cimClass, "cimClass");
		Exceptions.ThrowIfNull(action, "action");
		using TraceWmiEnumeration traceWmiEnumeration = ClusterLog.StartTraceEnumeration(Microsoft.FailoverClusters.UI.Common.TraceSource.Framework, cimClass.CimSystemProperties.ClassName);
		using CimInstance obj = Scope.EnumerateInstances(cimClass.CimSystemProperties.Namespace, cimClass.CimSystemProperties.ClassName).First();
		traceWmiEnumeration.Instances = 1;
		action(obj);
	}

	public IEnumerable<CimInstance> EnumerateOptimalAssociatedInstances(CimInstance sourceInstance, string associationClassName, string resultClassName, AssociationType associationType)
	{
		return Scope.EnumerateAssociatedInstances("root\\microsoft\\windows\\cluster", sourceInstance, associationClassName, resultClassName, (associationType == AssociationType.CimDependency) ? "Antecedent" : "GroupComponent", (associationType == AssociationType.CimDependency) ? "Dependent" : "PartComponent");
	}

	public IEnumerable<CimInstance> EnumerateOptimalReferencingInstances(CimInstance sourceInstance, string associationClassName, AssociationType associationType)
	{
		return Scope.EnumerateReferencingInstances("root\\microsoft\\windows\\cluster", sourceInstance, associationClassName, (associationType == AssociationType.CimDependency) ? "Antecedent" : "GroupComponent");
	}

	public IEnumerable<CimInstance> ExecuteOptimalQuery(string query, bool nullElementOnError = true, string cimNamespace = "root\\microsoft\\windows\\cluster")
	{
		if (QueriesAreAsync)
		{
			return ExecuteQueryAsync(query, nullElementOnError, cimNamespace);
		}
		try
		{
			return ExecuteQuery(query, nullElementOnError, cimNamespace);
		}
		catch (Exception)
		{
			if (!nullElementOnError)
			{
				throw;
			}
		}
		return null;
	}

	public IEnumerable<CimInstance> ExecuteQuery(string query, bool nullElementOnError = true, string cimNamespace = "root\\microsoft\\windows\\cluster")
	{
		Exceptions.ThrowIfNull(query, "query");
		ClusterLog.LogInfo("Executing Query in namespace {0} going to execute query {{1}}", "root\\microsoft\\windows\\cluster", query);
		using TraceWmiQuery queryTracer = ClusterLog.StartTraceQuery(Microsoft.FailoverClusters.UI.Common.TraceSource.Framework, query);
		int instances = 0;
		using (CimOperationOptions options = new CimOperationOptions())
		{
			options.Timeout = TimeSpan.FromSeconds(5.0);
			foreach (CimInstance item in Scope.QueryInstances(cimNamespace, "WQL", query, options))
			{
				instances++;
				yield return item;
			}
		}
		queryTracer.Instances = instances;
		queryTracer.Tag = "Sync Query";
	}

	public IEnumerable<CimInstance> ExecuteQueryAsync(string query, bool nullElementOnError = true, string cimNamespace = "root\\microsoft\\windows\\cluster")
	{
		Exceptions.ThrowIfNull(query, "query");
		ClusterLog.LogInfo("Executing Query in namespace {0} going to execute query {{1}}", "root\\microsoft\\windows\\cluster", query);
		using TraceWmiQuery queryTracer = ClusterLog.StartTraceQuery(Microsoft.FailoverClusters.UI.Common.TraceSource.Framework, query);
		int instances = 0;
		using (CimOperationOptions options = new CimOperationOptions())
		{
			options.Timeout = TimeSpan.FromSeconds(5.0);
			CimAsyncMultipleResults<CimInstance> results = Scope.QueryInstancesAsync(cimNamespace, "WQL", query, options);
			foreach (CimInstance item in AsyncResultToEnumerable(results, nullElementOnError))
			{
				if (item != null)
				{
					instances++;
					yield return item;
				}
			}
		}
		queryTracer.Instances = instances;
		queryTracer.Tag = "ASync Query";
	}

	public IEnumerable<CimInstance> AsyncResultToEnumerable(CimAsyncMultipleResults<CimInstance> results, bool nullElementOnError = true)
	{
		AutoResetEvent rdyEvent = new AutoResetEvent(initialState: false);
		AutoResetEvent doneEvent = new AutoResetEvent(initialState: true);
		CimInstance returnObject = null;
		Exception lastError = null;
		AnonymousObserver<CimInstance> queryResult = new AnonymousObserver<CimInstance>(delegate(CimInstance next)
		{
			try
			{
				doneEvent.WaitOne();
				returnObject = next;
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
		}, delegate(Exception error)
		{
			doneEvent.WaitOne();
			returnObject = null;
			lastError = error;
			rdyEvent.Set();
		}, delegate
		{
			doneEvent.WaitOne();
			returnObject = null;
			rdyEvent.Set();
		});
		results.Subscribe(queryResult);
		try
		{
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
			queryResult.Dispose();
		}
	}

	private void TestConnection()
	{
		using (ClusterLog.StartTraceInvokeMethod(Microsoft.FailoverClusters.UI.Common.TraceSource.Framework, isStaticInvoke: false, Scope.ComputerName, "TestConnection", null))
		{
			Scope.TestConnection();
		}
	}

	private void CreateSession(string clusterName, CimSessionOptions options)
	{
		string relativePath = ReplaceClusterNameWithNodeName(clusterName);
		using (ClusterLog.StartTraceInvokeMethod(Microsoft.FailoverClusters.UI.Common.TraceSource.Framework, isStaticInvoke: false, relativePath, "CreateSession", null))
		{
			Scope = CimSession.Create(clusterName, options);
		}
	}

	protected string ReplaceClusterNameWithNodeName(string nodeName)
	{
		string[] array = clusters.Cluster.FqdnName.Split('.');
		array[0] = nodeName;
		return string.Join(".", array);
	}

	private CimInstance ModifyInstance(CimInstance cimInstance)
	{
		string propertyName = string.Join(", ", cimInstance.CimInstanceProperties.Where((CimProperty property) => property.Value != null));
		using (ClusterLog.StartTraceModifyInstance(Microsoft.FailoverClusters.UI.Common.TraceSource.Framework, cimInstance.GetRelativePath(), propertyName))
		{
			return Scope.ModifyInstance(cimInstance);
		}
	}

	private T GetInstance<T>(CimInstance identityInstance, Func<CimInstance, T> action)
	{
		ClusterLog.LogInfo("In {0} going to look for {1}", identityInstance.CimSystemProperties.Namespace, identityInstance.ToString());
		using (ClusterLog.StartTraceGetInstance(Microsoft.FailoverClusters.UI.Common.TraceSource.Framework, identityInstance.GetRelativePath()))
		{
			using CimInstance arg = Scope.GetInstance(identityInstance.CimSystemProperties.Namespace, identityInstance);
			return action(arg);
		}
	}

	public void GetInstance(CimInstance identityInstance, Action<CimInstance> action)
	{
		ClusterLog.LogInfo("In {0} going to look for {1}", identityInstance.CimSystemProperties.Namespace, identityInstance.ToString());
		CimOperationOptions options = new CimOperationOptions
		{
			Timeout = TimeSpan.FromSeconds(30.0)
		};
		using (ClusterLog.StartTraceGetInstance(Microsoft.FailoverClusters.UI.Common.TraceSource.Framework, identityInstance.GetRelativePath()))
		{
			using CimInstance obj = Scope.GetInstance(identityInstance.CimSystemProperties.Namespace, identityInstance, options);
			action(obj);
		}
	}

	public void InvokeMethod(CimInstance cimInstance, string methodName, IEnumerable<CimArgumentNameValue> parameters)
	{
		InvokeMethod<object>(Scope, cimInstance, null, methodName, parameters, null);
	}

	public void InvokeMethod(CimInstance cimInstance, string methodName, IEnumerable<CimArgumentNameValue> parameters, Action<CimMethodResult> action)
	{
		Func<CimMethodResult, object> action2 = delegate(CimMethodResult methodResult)
		{
			action.SafeCall(methodResult);
			return null;
		};
		InvokeMethod(cimInstance, methodName, parameters, action2);
	}

	public T InvokeMethod<T>(CimInstance cimInstance, string methodName, IEnumerable<CimArgumentNameValue> parameters, Func<CimMethodResult, T> action)
	{
		return InvokeMethod(Scope, cimInstance, null, methodName, parameters, action);
	}

	public void InvokeMethod(CimClass cimClass, string methodName, IEnumerable<CimArgumentNameValue> parameters, Action<CimMethodResult> action)
	{
		Func<CimMethodResult, object> action2 = delegate(CimMethodResult methodResult)
		{
			action.SafeCall(methodResult);
			return null;
		};
		InvokeMethod(cimClass, methodName, parameters, action2);
	}

	public T InvokeMethod<T>(CimClass cimClass, string methodName, IEnumerable<CimArgumentNameValue> parameters, Func<CimMethodResult, T> action)
	{
		return InvokeMethod(Scope, null, cimClass, methodName, parameters, action);
	}

	private T InvokeMethod<T>(CimSession session, CimInstance cimInstance, CimClass cimClass, string methodName, IEnumerable<CimArgumentNameValue> parameters, Func<CimMethodResult, T> action, string cimNamespace = "root\\microsoft\\windows\\cluster", TimeSpan? timeout = null)
	{
		Exceptions.ThrowIfNull(methodName, "methodName");
		if (!timeout.HasValue)
		{
			timeout = TimeSpan.FromMinutes(1.0);
		}
		if (cimClass == null && cimInstance == null)
		{
			throw new ArgumentException("Both cimClass and cimInstance are null.", "cimClass");
		}
		return CommonAdapter.ExecuteAndCatchWmiExceptions(delegate
		{
			CimClass cimClass2 = ((cimInstance != null) ? cimInstance.CimClass : cimClass);
			CimReadOnlyKeyedCollection<CimMethodParameterDeclaration> parameters2 = (cimClass2.CimClassMethods[methodName] ?? throw new MissingMethodException(cimClass2.CimSystemProperties.ClassName, methodName)).Parameters;
			using CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
			if (parameters != null)
			{
				foreach (CimArgumentNameValue parameter in parameters)
				{
					CimMethodParameterDeclaration cimMethodParameterDeclaration = parameters2[parameter.Name];
					switch (cimMethodParameterDeclaration.CimType)
					{
					case CimType.Reference:
					{
						CimInstance value2 = (CimInstance)parameter.Value;
						cimMethodParametersCollection.Add(CimMethodParameter.Create(parameter.Name, value2, cimMethodParameterDeclaration.CimType, CimFlags.Parameter));
						break;
					}
					case CimType.ReferenceArray:
					{
						CimInstance[] array = null;
						object value = parameter.Value;
						if (value != null && value.GetType().IsArray)
						{
							object[] obj = (object[])value;
							array = new CimInstance[obj.Length];
							int num = 0;
							object[] array2 = obj;
							foreach (object obj2 in array2)
							{
								array[num++] = (CimInstance)obj2;
							}
						}
						else if (value is IEnumerable<CimInstance>)
						{
							array = ((IEnumerable<CimInstance>)value).ToArray();
						}
						cimMethodParametersCollection.Add(CimMethodParameter.Create(parameter.Name, array, cimMethodParameterDeclaration.CimType, CimFlags.Parameter));
						break;
					}
					default:
						cimMethodParametersCollection.Add(CimMethodParameter.Create(parameter.Name, parameter.Value, cimMethodParameterDeclaration.CimType, CimFlags.Parameter));
						break;
					}
				}
			}
			ClusterLog.LogInfo("Invoking method {0} on CimInstance {1}".FormatCurrentCulture(methodName, cimInstance));
			Func<CimMethodParametersCollection, CimMethodResult> func = delegate(CimMethodParametersCollection parametersFx)
			{
				CimOperationOptions options = new CimOperationOptions
				{
					Timeout = timeout.Value
				};
				if (cimInstance != null)
				{
					return session.InvokeMethod(cimNamespace, cimInstance, methodName, parametersFx, options);
				}
				return (cimClass != null) ? session.InvokeMethod(cimNamespace, cimClass.CimSystemProperties.ClassName, methodName, parametersFx, options) : null;
			};
			string relativePath = null;
			if (cimClass != null)
			{
				relativePath = cimClass.CimSystemProperties.ClassName;
			}
			else if (cimInstance != null)
			{
				relativePath = cimInstance.GetRelativePath();
			}
			string[] parameterNames = cimMethodParametersCollection.Select((CimMethodParameter parameter) => parameter.Name).ToArray();
			using TraceWmiInvokeMethod traceWmiInvokeMethod = ClusterLog.StartTraceInvokeMethod(Microsoft.FailoverClusters.UI.Common.TraceSource.Framework, cimClass != null, relativePath, methodName, parameterNames);
			if (cimInstance != null)
			{
				traceWmiInvokeMethod.Tag = string.Join(", ", cimInstance.CimInstanceProperties.Where((CimProperty property) => property.Value != null && (property.Flags & CimFlags.Key) == CimFlags.Key));
			}
			using CimMethodResult cimMethodResult = func(cimMethodParametersCollection);
			int num2 = (int)(uint)cimMethodResult.ReturnValue.Value;
			if (!NativeMethods.ErrorCode.None.IsEqual(num2) && !NativeMethods.ErrorCode.IOPending.IsEqual(num2))
			{
				ClusterLog.LogError("Return value is not 0. Actual value is: {0,10} - 0x{1,-10:X} :: {2}".FormatCurrentCulture(num2, num2, num2));
				throw new ClusterWmiWin32Exception(num2, null);
			}
			return action.SafeCall(cimMethodResult);
		}, methodName);
	}

	public void SubscribeNotifications(Action notificationLostAction, Action<ClusterException> notificationConnectionUnrepairableAction)
	{
		throw new NotSupportedException("SubscribeNotifications is not implemented for CimAdapeter");
	}

	public void UnsubscribeNotifications()
	{
		throw new NotSupportedException("UnsubscribeNotifications is not implemented for CimAdapeter");
	}

	public void EnqueueNotification(Notification notification)
	{
	}

	public void PauseNotifications()
	{
		throw new NotSupportedException("PauseNotifications is not implemented for CimAdapeter");
	}

	public void ResumeNotifications()
	{
		throw new NotSupportedException("ResumeNotifications is not implemented for CimAdapeter");
	}

	public void ResetNotifications()
	{
		throw new NotSupportedException("ResetNotifications is not implemented for CimAdapeter");
	}

	public Notification DequeueNotification()
	{
		throw new NotSupportedException("DequeueNotification is not implemented for CimAdapeter");
	}
}

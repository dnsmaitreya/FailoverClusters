using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using Win32;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.PowerShell;

[CLSCompliant(false)]
public abstract class ClusterVMMonitoredItem
{
	internal const string ProductName = "Failover Clustering";

	internal const string FeatureName = "VM Monitoring";

	internal const string FeatureEnabledValue = "VMMonitoringConfigured";

	internal const string ServiceTypeString = "Service";

	internal const string EventTypeString = "Event";

	internal const string ApplicationTypeString = "Application";

	public string VMName { get; set; }

	public string Name { get; set; }

	protected Guid Id { get; set; }

	protected string ItemType { get; set; }

	protected RegistryKey RootKey { get; set; }

	protected RegistryKey KvpKey { get; set; }

	protected RegistryKey ItemKey { get; set; }

	internal ClusterVMMonitoredItem(string itemType, string name, Guid id, string vmName)
	{
		ItemType = itemType;
		Name = name;
		Id = id;
		VMName = vmName;
	}

	public override string ToString()
	{
		return Name;
	}

	internal static void ConnectToTaskScheduler(string computerName, out NativeMethods.ITaskService taskService, out NativeMethods.ITaskFolder taskFolder, CancellationToken cancellationToken)
	{
		if (!new Win32Adapter().IsVirtualMachine(computerName))
		{
			throw new ClusterVirtualMachineNotAVirtualMachineException(ExceptionResources.NotAVm, computerName);
		}
		cancellationToken.ThrowIfCancellationRequested();
		Type typeFromProgID = Type.GetTypeFromProgID("Schedule.Service", throwOnError: true);
		taskService = (NativeMethods.ITaskService)Activator.CreateInstance(typeFromProgID);
		taskService.Connect(computerName, null, null, null);
		cancellationToken.ThrowIfCancellationRequested();
		try
		{
			taskService.GetFolder("\\Microsoft\\Windows").CreateFolder("Failover Clustering", null);
		}
		catch (COMException ex)
		{
			if ((ex.ErrorCode & 0xFFFF) != 183)
			{
				throw;
			}
		}
		try
		{
			taskService.GetFolder("\\Microsoft\\Windows\\Failover Clustering").CreateFolder("VM Monitoring", null);
		}
		catch (COMException ex2)
		{
			if ((ex2.ErrorCode & 0xFFFF) != 183)
			{
				throw;
			}
		}
		taskFolder = taskService.GetFolder("\\Microsoft\\Windows\\Failover Clustering\\VM Monitoring");
	}

	private static RegistryKey CreateRootKey(string computerName)
	{
		return ((RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName, RegistryView.Registry64).OpenSubKey("Software\\Microsoft", writable: true) ?? throw new ClusterDialogException(ExceptionResources.FailedToCreateRegistryKey.FormatCurrentCulture(computerName))).CreateSubKey("Failover Clustering") ?? throw new ClusterDialogException(ExceptionResources.FailedToCreateRegistryKey.FormatCurrentCulture(computerName))).CreateSubKey("VM Monitoring") ?? throw new ClusterDialogException(ExceptionResources.FailedToCreateRegistryKey.FormatCurrentCulture(computerName));
	}

	private static RegistryKey OpenRootKey(string computerName, bool writable)
	{
		return ((RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName, RegistryView.Registry64).OpenSubKey("Software\\Microsoft", writable) ?? throw new ClusterDialogException(ExceptionResources.FailedToOpenRegistryKey.FormatCurrentCulture(computerName))).OpenSubKey("Failover Clustering", writable) ?? throw new ClusterDialogException(ExceptionResources.FailedToOpenRegistryKey.FormatCurrentCulture(computerName))).OpenSubKey("VM Monitoring", writable) ?? throw new ClusterDialogException(ExceptionResources.FailedToOpenRegistryKey.FormatCurrentCulture(computerName));
	}

	internal static IEnumerable<ClusterVMMonitoredItem> GetMonitoredItems(string computerName, CancellationToken cancellationToken)
	{
		RegistryKey rootKey;
		string[] subKeyNames;
		try
		{
			rootKey = OpenRootKey(computerName, writable: false);
			subKeyNames = rootKey.GetSubKeyNames();
		}
		catch (ClusterDialogException)
		{
			return new List<ClusterVMMonitoredItem>();
		}
		cancellationToken.ThrowIfCancellationRequested();
		return from itemKeyName in subKeyNames
			let itemKey = rootKey.OpenSubKey(itemKeyName)
			select ReadFromRegistry(computerName, itemKeyName, cancellationToken);
	}

	internal static bool IsMonitoringSupported(OSProductType? guestOSProductType, int? guestOSMajorVersion, int? guestOSMinorVersion)
	{
		if (guestOSProductType.HasValue && guestOSMajorVersion.HasValue && guestOSMinorVersion.HasValue && (guestOSProductType == OSProductType.Server || guestOSProductType == OSProductType.DomainController))
		{
			return guestOSMajorVersion * 10 + guestOSMinorVersion >= 62;
		}
		return false;
	}

	internal static ClusterVMMonitoredItem ReadFromRegistry(string computerName, string name, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		RegistryKey registryKey = OpenRootKey(computerName, writable: true).OpenSubKey(name);
		if (registryKey == null)
		{
			throw new ApplicationException(ExceptionResources.VmMonitoredItemNotFound.FormatCurrentCulture(name, computerName));
		}
		cancellationToken.ThrowIfCancellationRequested();
		string text = registryKey.GetValue("Type") as string;
		if (registryKey.GetValue("Id") is string g)
		{
			switch (text)
			{
			case "Service":
				return new ClusterVMMonitoredService(name, new Guid(g), computerName);
			case "Event":
			{
				string eventLog = registryKey.GetValue("EventLog") as string;
				string eventSource = registryKey.GetValue("EventSource") as string;
				int eventId = (int)registryKey.GetValue("EventId");
				return new ClusterVMMonitoredEvent(name, new Guid(g), computerName, eventLog, eventSource, eventId);
			}
			case "Application":
				return new ClusterVMMonitoredApplication(name, new Guid(g), computerName);
			default:
				throw new ApplicationException(ExceptionResources.UnknownItemType.FormatCurrentCulture(text));
			case null:
				break;
			}
		}
		return null;
	}

	internal void CreateItemKey(string computerName)
	{
		RootKey = CreateRootKey(computerName);
		KvpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName, RegistryView.Registry64).OpenSubKey("Software\\Microsoft\\Virtual Machine\\Guest", writable: true);
		if (RootKey.GetSubKeyNames().Length == 0 && KvpKey != null)
		{
			KvpKey.SetValue("VMMonitoringConfigured", 1);
		}
		ItemKey = RootKey.CreateSubKey(Name);
		if (ItemKey == null)
		{
			throw new ClusterDialogException(ExceptionResources.FailedToCreateRegistryKey.FormatCurrentCulture(computerName));
		}
		ItemKey.SetValue("Id", Id.ToString());
		ItemKey.SetValue("Type", ItemType);
	}

	internal void DeleteItemKey(string computerName, NativeMethods.ITaskService taskService)
	{
		OpenRegistryKey(computerName);
		RootKey.DeleteSubKey(Name, throwOnMissingSubKey: true);
		if (RootKey.GetSubKeyNames().Length == 0)
		{
			if (KvpKey != null)
			{
				KvpKey.SetValue("VMMonitoringConfigured", 0);
			}
			RootKey.Close();
			RegistryKey registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName, RegistryView.Registry64).OpenSubKey("Software\\Microsoft", writable: true);
			if (registryKey == null)
			{
				throw new ApplicationException(ExceptionResources.FailedToOpenRegistryKey.FormatCurrentCulture(computerName));
			}
			RegistryKey registryKey2 = registryKey.OpenSubKey("Failover Clustering", writable: true);
			if (registryKey2 == null)
			{
				throw new ApplicationException(ExceptionResources.FailedToOpenRegistryKey.FormatCurrentCulture(computerName));
			}
			if (registryKey2.ValueCount == 0 && registryKey2.GetSubKeyNames().Length == 0)
			{
				registryKey2.DeleteSubKey("VM Monitoring");
			}
			registryKey2.Close();
			if (registryKey.ValueCount == 0 && registryKey.GetSubKeyNames().Length == 0)
			{
				registryKey.DeleteSubKey("Failover Clustering");
			}
		}
	}

	private void OpenRegistryKey(string computerName)
	{
		RootKey = OpenRootKey(computerName, writable: true);
		KvpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName, RegistryView.Registry64).OpenSubKey("Software\\Microsoft\\Virtual Machine\\Guest", writable: true);
	}

	internal void CreateTask(NativeMethods.ITaskService taskService, NativeMethods.ITaskFolder taskFolder, string eventQuery, string taskDescription, string taskName, string serviceName, string serviceGuid, string criticalStateValue)
	{
		NativeMethods.ITaskDefinition taskDefinition = taskService.NewTask(0);
		taskDefinition.RegistrationInfo.Description = taskDescription;
		taskDefinition.RegistrationInfo.Author = "Corporation";
		taskDefinition.Principal.LogonType = NativeMethods.TaskLogonType.TaskLogonServiceAccount;
		taskDefinition.Principal.RunLevel = NativeMethods.TaskRunlevelType.TaskRunlevelHighest;
		NativeMethods.IEventTrigger obj = (taskDefinition.Triggers.Create(NativeMethods.TaskTriggerType.TaskTriggerEvent) as NativeMethods.IEventTrigger) ?? throw new ApplicationException("Failed to create task trigger.");
		string text = "\"";
		string text2 = serviceGuid + "_" + criticalStateValue + ".js";
		string text3 = "%WINDIR%\\Temp\\" + text2;
		obj.Subscription = eventQuery;
		NativeMethods.IExecAction obj2 = (taskDefinition.Actions.Create(NativeMethods.TaskActionType.TaskActionExec) as NativeMethods.IExecAction) ?? throw new ApplicationException("Failed to create task action.");
		obj2.Path = "cmd.exe";
		obj2.Arguments = "/s /c " + text + "echo var hs = new ActiveXObject(" + text + "HyperV.AppHealthMonitor" + text + "); hs.SetApplicationState(" + text + serviceGuid + text + ", " + text + serviceName + text + ", " + criticalStateValue + "); " + text + "  > " + text3;
		NativeMethods.IExecAction obj3 = (taskDefinition.Actions.Create(NativeMethods.TaskActionType.TaskActionExec) as NativeMethods.IExecAction) ?? throw new ApplicationException("Failed to create task action.");
		obj3.Path = "cscript.exe";
		obj3.Arguments = text3 + " //B";
		NativeMethods.IRegisteredTask pRegisteredTask;
		uint num = taskFolder.RegisterTaskDefinition(taskName, taskDefinition, NativeMethods.TaskCreation.TaskCreateOrUpdate, "NT AUTHORITY\\System", null, NativeMethods.TaskLogonType.TaskLogonServiceAccount, null, out pRegisteredTask);
		if (num != 0)
		{
			throw new COMException(ExceptionResources.ClusterVMMonitoredItem_CreateTaskFailed.FormatCurrentCulture(num), (int)num);
		}
	}

	internal abstract void RegisterItem(NativeMethods.ITaskService taskService, NativeMethods.ITaskFolder taskFolder, string computerName, bool overideSettings = false);

	internal abstract void UnregisterItem(NativeMethods.ITaskService taskService, NativeMethods.ITaskFolder taskFolder, string computerName);
}


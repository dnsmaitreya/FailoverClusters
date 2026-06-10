using System;
using System.Globalization;
using System.IO;
using FailoverClusters.UI.Common;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.PowerShell;

[CLSCompliant(false)]
public class ClusterVMMonitoredApplication : ClusterVMMonitoredItem
{
	private bool isFullPath;

	internal ClusterVMMonitoredApplication(string name, Guid id, string vmName)
		: base("Application", name, id, vmName)
	{
	}

	internal override void RegisterItem(NativeMethods.ITaskService taskService, NativeMethods.ITaskFolder taskFolder, string computerName, bool unused = false)
	{
		if (!base.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
		{
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, ExceptionResources.ApplicationMustEndWithExe));
		}
		isFullPath = File.Exists(base.Name);
		if (!isFullPath && base.Name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
		{
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, ExceptionResources.InvalidApplicationName, base.Name));
		}
		CreateItemKey(computerName);
		string eventQuery = "<QueryList><Query Id='0' Path='Application'><Select Path='Application'>*[System[Provider[@Name='Application Error'] and (EventID=1000)] and (EventData[Data[1] = '" + base.Name + "' ] or EventData[Data[11] = '" + base.Name + "' ])]</Select></Query></QueryList>";
		try
		{
			CreateTask(taskService, taskFolder, eventQuery, CommonResources.ClusterVMMonitoredApplication_TaskDescription.FormatCurrentCulture(base.Name), base.Name + CommonResources.NameSuffixApplication, base.Name, base.Id.ToString(), "1");
		}
		catch (Exception)
		{
			DeleteItemKey(computerName, taskService);
			throw;
		}
	}

	internal override void UnregisterItem(NativeMethods.ITaskService taskService, NativeMethods.ITaskFolder taskFolder, string computerName)
	{
		Exception ex = null;
		try
		{
			taskFolder.DeleteTask(base.Name + CommonResources.NameSuffixApplication, 0);
		}
		catch (Exception ex2)
		{
			ex = ex2;
		}
		DeleteItemKey(computerName, taskService);
		if (ex != null)
		{
			throw ex;
		}
	}
}


using System;
using System.Globalization;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.PowerShell;

[CLSCompliant(false)]
public class ClusterVMMonitoredEvent : ClusterVMMonitoredItem
{
	public string EventLog { get; set; }

	public string EventSource { get; set; }

	public int EventId { get; set; }

	public ClusterVMMonitoredEvent(string name, Guid id, string vmName, string eventLog, string eventSource, int eventId)
		: base("Event", name, id, vmName)
	{
		EventLog = eventLog;
		EventSource = eventSource;
		EventId = eventId;
	}

	public ClusterVMMonitoredEvent(Guid id, string vmName, string eventLog, string eventSource, int eventId)
		: base("Event", FormatName(eventLog, eventSource, eventId), id, vmName)
	{
		EventLog = eventLog;
		EventSource = eventSource;
		EventId = eventId;
	}

	internal static string FormatName(string eventLog, string eventSource, int eventId)
	{
		string text = eventLog + "," + eventSource + "," + eventId.ToString(CultureInfo.InvariantCulture);
		if (text.Contains("\\"))
		{
			text = text.Replace("\\", "+");
		}
		if (text.Contains("/"))
		{
			text = text.Replace("/", "+");
		}
		return text;
	}

	internal override void RegisterItem(NativeMethods.ITaskService taskService, NativeMethods.ITaskFolder taskFolder, string computerName, bool unused = false)
	{
		CreateItemKey(computerName);
		base.ItemKey.SetValue("EventLog", EventLog);
		base.ItemKey.SetValue("EventSource", EventSource);
		base.ItemKey.SetValue("EventId", EventId);
		string eventQuery = "<QueryList><Query Id='0' Path='" + EventLog + "'><Select Path='" + EventLog + "'>*[System[Provider[@Name='" + EventSource + "'] and EventID=" + EventId + "]]</Select></Query></QueryList>";
		try
		{
			CreateTask(taskService, taskFolder, eventQuery, CommonResources.ClusterVMMonitoredEvent_TaskDescription, base.Name + CommonResources.NameSuffixEvent, base.Name, base.Id.ToString(), "1");
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
			taskFolder.DeleteTask(base.Name + CommonResources.NameSuffixEvent, 0);
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

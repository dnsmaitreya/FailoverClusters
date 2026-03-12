using System.Collections.Generic;
using System.Management;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class VirtualMachineResourcePool
{
	public string PoolId { get; private set; }

	public IEnumerable<VirtualMachineResourcePool> ChildPools { get; set; }

	public string Caption { get; private set; }

	public IEnumerable<string> HostResources { get; private set; }

	internal VirtualMachineResourcePool(ManagementObject poolManagementObject, ManagementObject poolRasd, string hostName)
	{
		Exceptions.ThrowIfNull(poolManagementObject, "poolManagementObject");
		Exceptions.ThrowIfNull(poolRasd, "poolRasd");
		PoolId = (string)poolManagementObject["PoolId"];
		Caption = (string)poolRasd.GetPropertyValue("Caption");
		string[] obj = (string[])poolRasd["HostResource"];
		List<string> list = new List<string>();
		string[] array = obj;
		foreach (string text in array)
		{
			if (text.Length > 1 && text[1] == ':')
			{
				list.Add("\\\\{0}\\{1}".FormatCurrentCulture(hostName, text.Replace(':', '$')));
			}
			else
			{
				list.Add(text);
			}
		}
		HostResources = list;
	}
}

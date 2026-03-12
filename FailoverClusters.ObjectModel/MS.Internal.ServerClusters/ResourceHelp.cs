using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace MS.Internal.ServerClusters;

public sealed class ResourceHelp
{
	private ResourceHelp()
	{
	}

	private static string GenerateScopedResourceName(ClusterGroup group, string baseName)
	{
		ClusterResource clusterResource = group.TryFindGroupNetworkName();
		return string.Format(arg1: (clusterResource == null) ? group.Name : clusterResource.Name, provider: Thread.CurrentThread.CurrentCulture, format: "{0}-{1}", arg0: baseName);
	}

	public static string GenerateNetNameResourceName(Cluster cluster, string netName)
	{
		return GenerateResourceName(cluster, null, netName, null, isNetworkName: false);
	}

	public static string GenerateIPAddressResourceName(Cluster cluster, string netName, string ipAddress)
	{
		string resourceName_IPAddress_Text = Resources.ResourceName_IPAddress_Text;
		return GenerateResourceName(cluster, null, resourceName_IPAddress_Text, ipAddress, isNetworkName: false);
	}

	public static string GenerateResourceName(Cluster cluster, string baseName, [MarshalAs(UnmanagedType.U1)] bool isNetworkName)
	{
		if (string.IsNullOrEmpty(baseName))
		{
			throw new ArgumentException(Resources.Argument_NullOrEmpty_Text, "baseName");
		}
		return GenerateResourceName(cluster, null, baseName, null, isNetworkName);
	}

	public static string GenerateResourceName(Cluster cluster, string baseName)
	{
		if (string.IsNullOrEmpty(baseName))
		{
			throw new ArgumentException(Resources.Argument_NullOrEmpty_Text, "baseName");
		}
		return GenerateResourceName(cluster, null, baseName, null, isNetworkName: false);
	}

	public static string GenerateResourceName(ClusterGroup group, string prefixName, ClusterResourceType resourceType)
	{
		if (group == null)
		{
			throw new ArgumentNullException("group");
		}
		if (resourceType == null)
		{
			throw new ArgumentNullException("resourceType");
		}
		string strA = "Distributed Transaction Coordinator";
		if (0 == string.Compare(strA, resourceType.Name, StringComparison.OrdinalIgnoreCase))
		{
			string typeName = GenerateScopedResourceName(group, Resources.MSDTCBaseResourceName_Text);
			return GenerateResourceName(group.Cluster, null, typeName, null, isNetworkName: false);
		}
		string strA2 = "MSMQ";
		if (0 == string.Compare(strA2, resourceType.Name, StringComparison.OrdinalIgnoreCase))
		{
			string typeName2 = GenerateScopedResourceName(group, Resources.MSMQBaseResourceName_Text);
			return GenerateResourceName(group.Cluster, null, typeName2, null, isNetworkName: false);
		}
		string strA3 = "MSMQTriggers";
		if (0 == string.Compare(strA3, resourceType.Name, StringComparison.OrdinalIgnoreCase))
		{
			string typeName3 = GenerateScopedResourceName(group, Resources.MSMQTriggersBaseResourceName_Text);
			return GenerateResourceName(group.Cluster, null, typeName3, null, isNetworkName: false);
		}
		string displayName = resourceType.DisplayName;
		return GenerateResourceName(group.Cluster, prefixName, displayName, null, isNetworkName: false);
	}

	public static string GenerateResourceName(Cluster cluster, string prefixName, string typeName, string suffixName, [MarshalAs(UnmanagedType.U1)] bool isNetworkName)
	{
		if (string.IsNullOrEmpty(typeName))
		{
			throw new ArgumentException(Resources.Argument_NullOrEmpty_Text, "typeName");
		}
		Dictionary<Guid, string> uniqueResourceNames = cluster.GetUniqueResourceNames();
		string text = typeName;
		if (prefixName != null)
		{
			text = string.Format(Thread.CurrentThread.CurrentCulture, "{0} {1}", prefixName, typeName);
		}
		if (suffixName != null)
		{
			text = string.Format(Thread.CurrentThread.CurrentCulture, "{0} {1}", text, suffixName);
		}
		string text2 = text;
		int num = 2;
		if (uniqueResourceNames.ContainsValue(text.ToLower(CultureInfo.CurrentCulture)))
		{
			do
			{
				if (isNetworkName)
				{
					int num2 = num;
					num++;
					text2 = string.Format(Thread.CurrentThread.CurrentCulture, "{0}{1}", text, num2);
				}
				else
				{
					int num3 = num;
					num++;
					text2 = string.Format(Thread.CurrentThread.CurrentCulture, "{0} ({1})", text, num3);
				}
			}
			while (uniqueResourceNames.ContainsValue(text2.ToLower(CultureInfo.CurrentCulture)));
		}
		return text2;
	}

	public static string GenerateResourceName(Cluster cluster, string prefixName, string typeName, string suffixName)
	{
		return GenerateResourceName(cluster, prefixName, typeName, suffixName, isNetworkName: false);
	}

	public static string GetResourceString(string resourceName)
	{
		return Resources.ResourceManager.GetString(resourceName, Resources.Culture);
	}
}

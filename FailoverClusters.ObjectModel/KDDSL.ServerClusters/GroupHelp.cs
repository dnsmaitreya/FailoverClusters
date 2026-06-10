using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace KDDSL.ServerClusters;

public sealed class GroupHelp
{
	private GroupHelp()
	{
	}

	public static string GenerateGroupName(Cluster cluster, string baseName)
	{
		//Discarded unreachable code: IL_0082, IL_0084
		if (string.IsNullOrEmpty(baseName))
		{
			throw new ArgumentException(Resources.Argument_NullOrEmpty_Text, "baseName");
		}
		try
		{
			Dictionary<Guid, string> uniqueGroupNames = cluster.GetUniqueGroupNames();
			string text = baseName;
			int num = 2;
			while (uniqueGroupNames.ContainsValue(text.ToLower(CultureInfo.CurrentCulture)))
			{
				int num2 = num;
				num++;
				text = string.Format(Thread.CurrentThread.CurrentCulture, "{0} ({1})", baseName, num2);
			}
			return text;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.GroupHelp_GenerateGroupName_Fail_Text,
				cluster.Name
			});
		}
	}

	public static string GenerateVirtualServerName(Cluster cluster, string nameBase, string nameSuffix, ClusterGroup group, ClusterNetworkNameSet nameSet)
	{
		IDictionary<string, string> netBiosNetworkNames = NetworkHelp.GetNetBiosNetworkNames(NetworkHelp.GetClusterNetworkNames(cluster, group, nameSet));
		int num = 0;
		nameBase = global::_003CModule_003E.MS_002EInternal_002EServerClusters_002ERemoveInvalidDnsChars(nameBase);
		nameSuffix = global::_003CModule_003E.MS_002EInternal_002EServerClusters_002ERemoveInvalidDnsChars(nameSuffix);
		string text;
		while (true)
		{
			text = ((num != 0) ? string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", nameBase, nameSuffix, num) : string.Format(CultureInfo.InvariantCulture, "{0}{1}", nameBase, nameSuffix));
			if (!NetworkHelp.IsNetBiosName(text))
			{
				nameBase = nameBase.Substring(0, nameBase.Length - 1);
			}
			if (NetworkHelp.IsNetBiosName(text))
			{
				num++;
				if (!netBiosNetworkNames.ContainsKey(text))
				{
					break;
				}
			}
		}
		return text;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsGroupNameInUse(Cluster cluster, string groupName)
	{
		//Discarded unreachable code: IL_005c, IL_005e
		if (string.IsNullOrEmpty(groupName))
		{
			throw new ArgumentException(Resources.Argument_NullOrEmpty_Text, "groupName");
		}
		try
		{
			if (cluster.GetUniqueGroupNames().ContainsValue(groupName.ToLower(CultureInfo.CurrentCulture)))
			{
				return true;
			}
			return false;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.GroupHelp_IsGroupNameInUse_Fail_Text,
				cluster.Name,
				groupName
			});
		}
	}
}

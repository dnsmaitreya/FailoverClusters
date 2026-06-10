using System.DirectoryServices;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Principal;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public static class ActiveDirectory
{
	public static DirectoryEntry GetDnsADObject(string domainName, string dnsDomain, string networkName, SearchZone searchZone)
	{
		DirectoryEntry result = null;
		string[] value = domainName.Split('.');
		string text = string.Format(CultureInfo.InvariantCulture, "DC={0}", string.Join(",DC=", value));
		string path = string.Empty;
		switch (searchZone)
		{
		case SearchZone.DomainDnsZone:
			path = "LDAP://DC=" + networkName + ",DC=" + dnsDomain + ",CN=MicrosoftDNS,DC=DomainDnsZones," + text;
			break;
		case SearchZone.System:
			path = "LDAP://DC=" + networkName + ",DC=" + dnsDomain + ",CN=MicrosoftDNS,CN=System," + text;
			break;
		}
		if (DirectoryEntry.Exists(path))
		{
			result = new DirectoryEntry(path);
		}
		return result;
	}

	public static IdentityReference GetComputerObjectIdentity(string resourceName, string networkName, string activeDirectoryDomain)
	{
		IdentityReference identityReference = null;
		using DirectorySearcher directorySearcher = BindToDomain(activeDirectoryDomain);
		using DirectoryEntry directoryEntry = FindMachine(directorySearcher, networkName);
		if (directoryEntry == null)
		{
			throw new ClusterNetNameRepairActiveDirectoryObjectException(resourceName, ExceptionResources.ClusterNetNameRepairActiveDirectoryObject_NotFound_Text.FormatCurrentCulture(networkName), null);
		}
		return new SecurityIdentifier((byte[])directoryEntry.Properties["objectSid"][0], 0);
	}

	public static void EnableComputerObject(string resourceName, string objectNetBiosName, string activeDirectoryDomain)
	{
		using DirectorySearcher directorySearcher = BindToDomain(activeDirectoryDomain);
		using DirectoryEntry directoryEntry = FindMachine(directorySearcher, objectNetBiosName);
		if (directoryEntry == null)
		{
			throw new ClusterNetNameRepairActiveDirectoryObjectException(resourceName, ExceptionResources.ClusterNetNameRepairActiveDirectoryObject_NotFound_Text.FormatCurrentCulture(objectNetBiosName), null);
		}
		int num = (int)directoryEntry.Properties["userAccountControl"].Value;
		if ((num & 2) == 2)
		{
			directoryEntry.Properties["userAccountControl"].Value = num & -3;
			directoryEntry.CommitChanges();
			ClusterLog.LogInfo("Enabled computer object '{0}'", objectNetBiosName);
		}
	}

	public static void AddAccess(DirectoryEntry entry, IdentityReference identity)
	{
		entry.Options.SecurityMasks = SecurityMasks.Dacl;
		ActiveDirectorySecurity objectSecurity = entry.ObjectSecurity;
		ActiveDirectoryAccessRule rule = (ActiveDirectoryAccessRule)objectSecurity.AccessRuleFactory(identity, -1, isInherited: false, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow);
		objectSecurity.AddAccessRule(rule);
		entry.CommitChanges();
	}

	private static DirectorySearcher BindToDomain(string domainName)
	{
		string path = ((domainName != null) ? string.Format(CultureInfo.InvariantCulture, "LDAP://{0}", domainName) : string.Empty);
		return new DirectorySearcher(new DirectoryEntry(path));
	}

	private static DirectoryEntry FindMachine(DirectorySearcher directorySearcher, string machineName)
	{
		directorySearcher.SearchScope = SearchScope.Subtree;
		string filter = string.Format(CultureInfo.InvariantCulture, "(&(objectCategory=computer)(samAccountName={0}$))", machineName);
		directorySearcher.Filter = filter;
		return directorySearcher.FindOne()?.GetDirectoryEntry();
	}
}


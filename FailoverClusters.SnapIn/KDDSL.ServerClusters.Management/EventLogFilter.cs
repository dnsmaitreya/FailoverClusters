using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;

namespace KDDSL.ServerClusters.Management;

internal class EventLogFilter
{
	internal static readonly ReadOnlyCollection<string> ProvidersForSystemChannel = new ReadOnlyCollection<string>(new List<string>
	{
		EventLog.ClusterProvider,
		EventLog.ClusterAwareUpdatingProvider,
		EventLog.ClusterAwareUpdatingManagementProvider
	});

	internal List<string> Nodes;

	internal List<string> Channels;

	internal IList<string> Providers;

	internal EventLevel Level;

	internal List<EventIdRange> EventIds;

	internal DateTime From;

	internal DateTime To;

	internal bool addLogClearToSystemChannel;

	internal object tag;

	private string clusterObject;

	private string clusterObjectType;

	private Guid[] replicationGroupIds;

	internal string ClusterNode
	{
		set
		{
			SetClusterObject(value, "NodeName");
		}
	}

	internal Guid[] ReplicationGroupIds
	{
		set
		{
			replicationGroupIds = value;
		}
	}

	internal string ClusterGroup
	{
		set
		{
			SetClusterObject(value, "ResourceGroup");
		}
	}

	internal string ClusterResource
	{
		set
		{
			SetClusterObject(value, "ResourceName");
		}
	}

	internal string ClusterNetwork
	{
		set
		{
			SetClusterObject(value, "NetworkName");
		}
	}

	internal string ClusterNetworkInterface
	{
		set
		{
			SetClusterObject(value, "InterfaceName");
		}
	}

	internal bool AddLogClearToSystemChannel
	{
		set
		{
			addLogClearToSystemChannel = value;
		}
	}

	internal object Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	internal EventLogFilter()
	{
		Nodes = new List<string>();
		Channels = new List<string>();
		Providers = new List<string>();
		Level = (EventLevel)0;
		EventIds = new List<EventIdRange>();
		From = DateTime.MinValue;
		To = DateTime.MaxValue;
		clusterObject = null;
		clusterObjectType = null;
	}

	private void SetClusterObject(string objectValue, string objectType)
	{
		clusterObject = objectValue;
		clusterObjectType = objectType;
	}

	public string GetQuery()
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlNode xmlNode = xmlDocument.AppendChild(xmlDocument.CreateElement("QueryList"));
		XmlElement xmlElement = xmlDocument.CreateElement("Query");
		xmlElement.SetAttribute("Id", "0");
		xmlElement.SetAttribute("Path", EventLog.SystemChannel);
		xmlNode.AppendChild(xmlElement);
		foreach (string channel in Channels)
		{
			string selectFilter = GetSelectFilter(channel);
			if (selectFilter.Length > 0)
			{
				XmlElement xmlElement2 = xmlDocument.CreateElement("Select");
				xmlElement2.SetAttribute("Path", channel);
				xmlElement2.InnerText = selectFilter;
				xmlElement.AppendChild(xmlElement2);
			}
		}
		string suppressFilter = GetSuppressFilter();
		if (suppressFilter.Length > 0)
		{
			foreach (string channel2 in Channels)
			{
				XmlElement xmlElement3 = xmlDocument.CreateElement("Suppress");
				xmlElement3.SetAttribute("Path", channel2);
				xmlElement3.InnerText = suppressFilter;
				xmlElement.AppendChild(xmlElement3);
			}
		}
		return xmlDocument.InnerXml;
	}

	private string GetSelectFilter(string channel)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetSystemFilter(channel));
		AppendSubExpression(stringBuilder, "and", GetEventDataFilter());
		if (stringBuilder.Length == 0)
		{
			return "*";
		}
		return stringBuilder.ToString();
	}

	private string GetSuppressFilter()
	{
		string eventIdsFilter = GetEventIdsFilter(suppress: true);
		if (eventIdsFilter.Length == 0)
		{
			return string.Empty;
		}
		return string.Format(CultureInfo.InvariantCulture, "*[System[{0}]]", eventIdsFilter);
	}

	private string GetSystemFilter(string channel)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (channel.Equals(EventLog.SystemChannel, StringComparison.OrdinalIgnoreCase))
		{
			if (addLogClearToSystemChannel)
			{
				stringBuilder.Append(GetLogClearEventFilter());
				AppendSubExpression(stringBuilder, "or", GetProviderFilter());
			}
			else
			{
				stringBuilder.Append(GetProviderFilter());
			}
		}
		AppendSubExpression(stringBuilder, "and", GetLevelFilter());
		AppendSubExpression(stringBuilder, "and", GetEventIdsFilter(suppress: false));
		AppendSubExpression(stringBuilder, "and", GetTimeCreatedFilter());
		if (stringBuilder.Length == 0)
		{
			return string.Empty;
		}
		return string.Format(CultureInfo.InvariantCulture, "*[System[{0}]]", stringBuilder);
	}

	private string GetLogClearEventFilter()
	{
		return string.Format(CultureInfo.InvariantCulture, "EventID={0}", 104);
	}

	private string GetProviderFilter()
	{
		if (Providers == null || Providers.Count == 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (Providers.Count > 1)
		{
			stringBuilder.Append("(");
		}
		stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "Provider[@Name='{0}']", Providers[0]));
		for (int i = 1; i < Providers.Count; i++)
		{
			stringBuilder.Append(" or ");
			stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "Provider[@Name='{0}']", Providers[i]));
		}
		if (Providers.Count > 1)
		{
			stringBuilder.Append(")");
		}
		return stringBuilder.ToString();
	}

	private string GetLevelFilter()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (Level != (EventLevel.Critical | EventLevel.Error | EventLevel.Warning | EventLevel.Informational | EventLevel.Verbose))
		{
			if ((Level & EventLevel.Critical) != 0)
			{
				stringBuilder.Append("Level=1");
			}
			if ((Level & EventLevel.Error) != 0)
			{
				AppendSubExpression(stringBuilder, "or", "Level=2");
			}
			if ((Level & EventLevel.Warning) != 0)
			{
				AppendSubExpression(stringBuilder, "or", "Level=3");
			}
			if ((Level & EventLevel.Informational) != 0)
			{
				AppendSubExpression(stringBuilder, "or", "Level=4");
			}
			if ((Level & EventLevel.Verbose) != 0)
			{
				AppendSubExpression(stringBuilder, "or", "Level=5");
			}
		}
		if (stringBuilder.Length == 0)
		{
			return string.Empty;
		}
		return string.Format(CultureInfo.InvariantCulture, "({0})", stringBuilder);
	}

	private string GetEventIdsFilter(bool suppress)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (EventIdRange eventId in EventIds)
		{
			if (eventId.Suppress == suppress)
			{
				if (eventId.LowerLimit == eventId.UpperLimit)
				{
					AppendSubExpression(stringBuilder, "or", string.Format(CultureInfo.InvariantCulture, "EventID={0}", eventId.LowerLimit));
				}
				else
				{
					AppendSubExpression(stringBuilder, "or", string.Format(CultureInfo.InvariantCulture, "(EventID>={0} and EventID<={1})", eventId.LowerLimit, eventId.UpperLimit));
				}
			}
		}
		if (stringBuilder.Length == 0)
		{
			return string.Empty;
		}
		return string.Format(CultureInfo.InvariantCulture, "({0})", stringBuilder);
	}

	private string GetTimeCreatedFilter()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (From != DateTime.MinValue)
		{
			stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "@SystemTime>='{0}'", ToIso8601(From)));
		}
		if (To != DateTime.MaxValue)
		{
			AppendSubExpression(stringBuilder, "and", string.Format(CultureInfo.InvariantCulture, "@SystemTime<='{0}'", ToIso8601(To)));
		}
		if (stringBuilder.Length == 0)
		{
			return string.Empty;
		}
		return string.Format(CultureInfo.InvariantCulture, "TimeCreated[{0}]", stringBuilder);
	}

	private string GetEventDataFilter()
	{
		if (clusterObject == null)
		{
			return string.Empty;
		}
		if (replicationGroupIds == null || replicationGroupIds.Length == 0)
		{
			return string.Format(CultureInfo.InvariantCulture, "*/EventData/Data[@Name=\"{0}\"]=\"{1}\"", clusterObjectType, clusterObject);
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "(*/EventData/Data[@Name=\"{0}\"]=\"{1}\"", clusterObjectType, clusterObject));
		Guid[] array = replicationGroupIds;
		for (int i = 0; i < array.Length; i++)
		{
			Guid guid = array[i];
			stringBuilder.Append(" or ");
			stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "*/EventData/Data[@Name=\"SourceReplicationGroupId\"]=\"{0}\"", guid.ToString("B")));
			stringBuilder.Append(" or ");
			stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "*/EventData/Data[@Name=\"TargetReplicationGroupId\"]=\"{0}\"", guid.ToString("B")));
			stringBuilder.Append(" or ");
			stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "*/EventData/Data[@Name=\"ReplicationGroupId\"]=\"{0}\"", guid.ToString("B")));
		}
		stringBuilder.Append(")");
		return stringBuilder.ToString();
	}

	private StringBuilder AppendSubExpression(StringBuilder a, string op, string b)
	{
		if (b.Length > 0)
		{
			if (a.Length > 0)
			{
				a.Append(' ').Append(op).Append(' ');
			}
			a.Append(b);
		}
		return a;
	}

	private string ToIso8601(DateTime time)
	{
		return time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
	}
}

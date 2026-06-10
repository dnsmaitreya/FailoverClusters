using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;

namespace KDDSL.ServerClusters;

[Serializable]
public class AzureException : ClusterBaseException
{
	private string m_azError;

	public string AzureErrorCode
	{
		get
		{
			return m_azError;
		}
		set
		{
			m_azError = value;
		}
	}

	public AzureException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		AzureErrorCode = info.GetString("AzureErrorCode");
	}

	public AzureException(XmlDocument xml)
		: base(xml.SelectSingleNode("//Error/Message/text()").Value)
	{
		AzureErrorCode = xml.SelectSingleNode("//Error/Code/text()").Value;
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("AzureErrorCode", AzureErrorCode);
	}
}

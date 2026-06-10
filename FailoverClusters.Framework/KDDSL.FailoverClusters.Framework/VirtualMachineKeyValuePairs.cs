using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Xml.XPath;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal class VirtualMachineKeyValuePairs
{
	internal enum RequestedInformation
	{
		CSDVersion,
		FullyQualifiedDomainName,
		IntegrationServicesVersion,
		NetworkAddressIPv4,
		NetworkAddressIPv6,
		OSBuildNumber,
		OSEditionId,
		OSName,
		OSMajorVersion,
		OSMinorVersion,
		OSPlatformId,
		OSVersion,
		ProcessorArchitecture,
		ProductType,
		RDPAddressIPv4,
		RDPAddressIPv6,
		ServicePackMajor,
		ServicePackMinor,
		SuiteMask
	}

	private static class KeyValuePairNames
	{
		public const string CSDVersion = "CSDVersion";

		public const string FullyQualifiedDomainName = "FullyQualifiedDomainName";

		public const string IntegrationServicesVersion = "IntegrationServicesVersion";

		public const string NetworkAddressIPv4 = "NetworkAddressIPv4";

		public const string NetworkAddressIPv6 = "NetworkAddressIPv6";

		public const string OSBuildNumber = "OSBuildNumber";

		public const string OSEditionId = "OSEditionId";

		public const string OSName = "OSName";

		public const string OSMajorVersion = "OSMajorVersion";

		public const string OSMinorVersion = "OSMinorVersion";

		public const string OSPlatformId = "OSPlatformId";

		public const string OSVersion = "OSVersion";

		public const string ProcessorArchitecture = "ProcessorArchitecture";

		public const string ProductType = "ProductType";

		public const string RDPAddressIPv4 = "RDPAddressIPv4";

		public const string RDPAddressIPv6 = "RDPAddressIPv6";

		public const string ServicePackMajor = "ServicePackMajor";

		public const string ServicePackMinor = "ServicePackMinor";

		public const string SuiteMask = "SuiteMask";
	}

	private string csdVersion;

	private string fullyQualifiedDomainName;

	private string integrationServicesVersion;

	private string networkAddressIPv4;

	private string networkAddressIPv6;

	private string operatingSystemBuildNumber;

	private string operatingSystemEditionId;

	private string operatingSystemName;

	private string operatingSystemMajorVersion;

	private string operatingSystemMinorVersion;

	private string operatingSystemPlatformId;

	private string operatingSystemVersion;

	private string processorArchitecture;

	private string productType;

	private string rdpAddressIPv4;

	private string rdpAddressIPv6;

	private string servicePackMajor;

	private string servicePackMinor;

	private string suiteMask;

	private VirtualMachineIntegrationServicesStatus? integrationServicesStatus;

	internal static readonly uint[] VMKeyValuePairsBasic = new uint[7] { 7u, 1u, 2u, 13u, 8u, 9u, 5u };

	private static readonly uint[] VMKeyValuePairsSummary = new uint[8] { 7u, 1u, 2u, 3u, 4u, 0u, 16u, 17u };

	private static readonly uint[] VMKeyValuePairsFull = new uint[19]
	{
		0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u, 8u, 9u,
		10u, 11u, 12u, 13u, 14u, 15u, 16u, 17u, 18u
	};

	public VirtualMachineIntegrationServicesStatus? IntegrationServicesStatus => integrationServicesStatus;

	public string CSDVersion => csdVersion;

	public string FullyQualifiedDomainName => fullyQualifiedDomainName;

	public string IntegrationServicesVersion
	{
		get
		{
			if (string.IsNullOrEmpty(integrationServicesVersion))
			{
				return CommonResources.VirtualMachineIntegrationServicesNotInstalled;
			}
			return integrationServicesVersion;
		}
	}

	public string NetworkAddressIPv4 => networkAddressIPv4;

	public string NetworkAddressIPv6 => networkAddressIPv6;

	public string OSBuildNumber => operatingSystemBuildNumber;

	public string OSEditionId => operatingSystemEditionId;

	public string OSName => operatingSystemName;

	public string OSMajorVersion => operatingSystemMajorVersion;

	public string OSMinorVersion => operatingSystemMinorVersion;

	public string OSPlatformId => operatingSystemPlatformId;

	public string OSVersion => operatingSystemVersion;

	public string ProcessorArchitecture => processorArchitecture;

	public string ProductType => productType;

	public string RDPAddressIPv4 => rdpAddressIPv4;

	public string RDPAddressIPv6 => rdpAddressIPv6;

	public string ServicePackMajor => servicePackMajor;

	public string ServicePackMinor => servicePackMinor;

	public string SuiteMask => suiteMask;

	public VirtualMachineKeyValuePairs(ManagementObject kvpExchangeComponent, uint[] requestedInformation)
	{
		Initialize(kvpExchangeComponent, requestedInformation);
	}

	internal static uint[] GetRequestedInformationArray(VirtualMachineKeyValuePairRequest requestedInformation)
	{
		uint[] array = null;
		return requestedInformation switch
		{
			VirtualMachineKeyValuePairRequest.Full => VMKeyValuePairsFull, 
			VirtualMachineKeyValuePairRequest.Basic => VMKeyValuePairsBasic, 
			VirtualMachineKeyValuePairRequest.Summary => VMKeyValuePairsSummary, 
			_ => throw new ArgumentOutOfRangeException("requestedInformation"), 
		};
	}

	private void Initialize(ManagementObject kvpExchangeComponent, uint[] requestedInformation)
	{
		string[] array = (string[])kvpExchangeComponent["GuestIntrinsicExchangeItems"];
		if (array != null)
		{
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				XPathNavigator navigator = new XPathDocument(new StringReader(array2[i])).CreateNavigator();
				foreach (uint requestedProperty in requestedInformation)
				{
					LoadProperty(navigator, requestedProperty);
				}
			}
		}
		integrationServicesStatus = GetIntegrationServicesStatus(requestedInformation);
	}

	private VirtualMachineIntegrationServicesStatus? GetIntegrationServicesStatus(uint[] requestedInformation)
	{
		VirtualMachineIntegrationServicesStatus? result = VirtualMachineIntegrationServicesStatus.Unknown;
		if (new List<uint>(requestedInformation).Contains(2u))
		{
			result = ((!string.IsNullOrEmpty(integrationServicesVersion)) ? new VirtualMachineIntegrationServicesStatus?(VirtualMachineIntegrationServicesStatus.Installed) : new VirtualMachineIntegrationServicesStatus?(VirtualMachineIntegrationServicesStatus.NotInstalled));
		}
		return result;
	}

	private void LoadProperty(XPathNavigator navigator, uint requestedProperty)
	{
		switch ((RequestedInformation)requestedProperty)
		{
		case RequestedInformation.FullyQualifiedDomainName:
			GetStringProperty(navigator, "FullyQualifiedDomainName", ref fullyQualifiedDomainName);
			break;
		case RequestedInformation.OSName:
			GetStringProperty(navigator, "OSName", ref operatingSystemName);
			break;
		case RequestedInformation.IntegrationServicesVersion:
			GetStringProperty(navigator, "IntegrationServicesVersion", ref integrationServicesVersion);
			break;
		case RequestedInformation.CSDVersion:
			GetStringProperty(navigator, "CSDVersion", ref csdVersion);
			break;
		case RequestedInformation.NetworkAddressIPv4:
			GetStringProperty(navigator, "NetworkAddressIPv4", ref networkAddressIPv4);
			break;
		case RequestedInformation.NetworkAddressIPv6:
			GetStringProperty(navigator, "NetworkAddressIPv6", ref networkAddressIPv6);
			break;
		case RequestedInformation.OSBuildNumber:
			GetStringProperty(navigator, "OSBuildNumber", ref operatingSystemBuildNumber);
			break;
		case RequestedInformation.OSEditionId:
			GetStringProperty(navigator, "OSEditionId", ref operatingSystemEditionId);
			break;
		case RequestedInformation.OSMajorVersion:
			GetStringProperty(navigator, "OSMajorVersion", ref operatingSystemMajorVersion);
			break;
		case RequestedInformation.OSMinorVersion:
			GetStringProperty(navigator, "OSMinorVersion", ref operatingSystemMinorVersion);
			break;
		case RequestedInformation.OSPlatformId:
			GetStringProperty(navigator, "OSPlatformId", ref operatingSystemPlatformId);
			break;
		case RequestedInformation.OSVersion:
			GetStringProperty(navigator, "OSVersion", ref operatingSystemVersion);
			break;
		case RequestedInformation.ProcessorArchitecture:
			GetStringProperty(navigator, "ProcessorArchitecture", ref processorArchitecture);
			break;
		case RequestedInformation.ProductType:
			GetStringProperty(navigator, "ProductType", ref productType);
			break;
		case RequestedInformation.RDPAddressIPv4:
			GetStringProperty(navigator, "RDPAddressIPv4", ref rdpAddressIPv4);
			break;
		case RequestedInformation.RDPAddressIPv6:
			GetStringProperty(navigator, "RDPAddressIPv6", ref rdpAddressIPv6);
			break;
		case RequestedInformation.ServicePackMajor:
			GetStringProperty(navigator, "ServicePackMajor", ref servicePackMajor);
			break;
		case RequestedInformation.ServicePackMinor:
			GetStringProperty(navigator, "ServicePackMinor", ref servicePackMinor);
			break;
		case RequestedInformation.SuiteMask:
			GetStringProperty(navigator, "SuiteMask", ref suiteMask);
			break;
		default:
			throw new ArgumentOutOfRangeException("requestedProperty");
		}
	}

	private void GetStringProperty(XPathNavigator navigator, string propertyName, ref string propertyValue)
	{
		navigator = navigator.SelectSingleNode("/INSTANCE/PROPERTY[@NAME='Name']/VALUE[child::text() = '" + propertyName + "']");
		if (navigator != null)
		{
			navigator = navigator.SelectSingleNode("/INSTANCE/PROPERTY[@NAME='Data']/VALUE/child::text()");
			if (navigator != null)
			{
				propertyValue = navigator.Value;
			}
		}
	}
}


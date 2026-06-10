using System;

namespace FailoverClusters.Framework;

public class ClusterResourceVirtualMachineGuestSummaryEventArgs : ClusterEventArgs
{
	public string GuestOperatingSystem { get; internal set; }

	public string GuestComputerName { get; internal set; }

	public string IntegrationServicesVersion { get; internal set; }

	public VirtualMachineIntegrationServicesStatus? IntegrationServicesStatus { get; internal set; }

	public OSProductType GuestOsProductType { get; internal set; }

	public int GuestOsMajorVersion { get; internal set; }

	public int GuestOsMinorVersion { get; internal set; }

	public int GuestOsBuildNumber { get; internal set; }

	public string Version { get; internal set; }

	public ClusterResourceVirtualMachineGuestSummaryEventArgs(Guid id, string guestOperatingSystem, string guestComputerName, string integrationServicesVersion, VirtualMachineIntegrationServicesStatus? integrationServicesStatus, ClusterException exception, OSProductType guestOSProductType, int guestOSMajorVersion, int guestOSMinorVersion, int guestOSBuildNumber, string version)
		: base(id, exception)
	{
		GuestOperatingSystem = guestOperatingSystem;
		GuestComputerName = guestComputerName;
		IntegrationServicesVersion = integrationServicesVersion;
		IntegrationServicesStatus = integrationServicesStatus;
		GuestOsProductType = guestOSProductType;
		GuestOsMajorVersion = guestOSMajorVersion;
		GuestOsMinorVersion = guestOSMinorVersion;
		GuestOsBuildNumber = guestOSBuildNumber;
		Version = version;
	}
}


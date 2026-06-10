using System;
using System.Management;
using FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters.Management;

internal class ServerInfo
{
	private string serverName;

	private bool dataLoaded;

	private string osName;

	private string systemManufacturer;

	private string systemModel;

	private string systemType;

	private uint numProcessors;

	private uint processorSpeed;

	private ulong totalPhysicalMemory;

	private ulong totalVirtualMemory;

	private ulong pageFileSpace;

	public string OSName => osName;

	public string SystemManufacturer => systemManufacturer;

	public string SystemModel => systemModel;

	public string SystemType => systemType;

	public uint NumberOfProcessors => numProcessors;

	public uint ProcessorSpeed => processorSpeed;

	public ulong TotalPhysicalMemory => totalPhysicalMemory;

	public ulong TotalVirtualMemory => totalVirtualMemory;

	public ulong PageFileSpace => pageFileSpace;

	public ServerInfo(string serverName)
	{
		this.serverName = serverName;
	}

	public void LoadServerInformation()
	{
		if (dataLoaded)
		{
			return;
		}
		try
		{
			if (NetworkHelper.CanPing(serverName))
			{
				WmiHelp wmiHelp = WmiHelp.Create(serverName, (WmiNamespace)0);
				LoadOperatingSystemInfo(wmiHelp);
				LoadComputerSystemInfo(wmiHelp);
				LoadMemoryInfo(wmiHelp);
				LoadProcessorInfo(wmiHelp);
				dataLoaded = true;
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.LoadServerInfoFailed_Text,
				serverName
			});
		}
	}

	private void LoadOperatingSystemInfo(WmiHelp wmiHelp)
	{
		ManagementObject onlyElement = WmiHelp.GetOnlyElement(wmiHelp.ExecuteQuery("SELECT Caption FROM Win32_OperatingSystem"));
		osName = (string)onlyElement["Caption"];
	}

	private void LoadComputerSystemInfo(WmiHelp wmiHelp)
	{
		ManagementObject onlyElement = WmiHelp.GetOnlyElement(wmiHelp.ExecuteQuery("SELECT Manufacturer, Model, SystemType, NumberOfProcessors FROM Win32_ComputerSystem"));
		systemManufacturer = WmiHelp.GetString((ManagementBaseObject)onlyElement, "Manufacturer");
		systemModel = WmiHelp.GetString((ManagementBaseObject)onlyElement, "Model");
		systemType = WmiHelp.GetString((ManagementBaseObject)onlyElement, "SystemType");
		numProcessors = WmiHelp.GetUInt32((ManagementBaseObject)onlyElement, "NumberOfProcessors");
	}

	private void LoadMemoryInfo(WmiHelp wmiHelp)
	{
		ManagementObject onlyElement = WmiHelp.GetOnlyElement(wmiHelp.ExecuteQuery("SELECT TotalVisibleMemorySize, TotalVirtualMemorySize, SizeStoredInPagingFiles FROM CIM_OperatingSystem"));
		totalPhysicalMemory = WmiHelp.GetUInt64((ManagementBaseObject)onlyElement, "TotalVisibleMemorySize");
		totalVirtualMemory = WmiHelp.GetUInt64((ManagementBaseObject)onlyElement, "TotalVirtualMemorySize");
		pageFileSpace = WmiHelp.GetUInt64((ManagementBaseObject)onlyElement, "SizeStoredInPagingFiles");
	}

	private void LoadProcessorInfo(WmiHelp wmiHelp)
	{
		ManagementObject firstElement = WmiHelp.GetFirstElement(wmiHelp.ExecuteQuery("SELECT MaxClockSpeed FROM Win32_Processor"));
		processorSpeed = WmiHelp.GetUInt32((ManagementBaseObject)firstElement, "MaxClockSpeed");
	}
}


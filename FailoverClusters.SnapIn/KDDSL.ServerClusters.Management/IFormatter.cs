namespace KDDSL.ServerClusters.Management;

internal interface IFormatter
{
	string Format(EventLogEvent e);
}

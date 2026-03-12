namespace MS.Internal.ServerClusters.Management;

internal interface IFormatter
{
	string Format(EventLogEvent e);
}

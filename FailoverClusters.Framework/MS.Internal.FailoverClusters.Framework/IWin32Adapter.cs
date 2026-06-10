namespace MS.Internal.FailoverClusters.Framework;

public interface IWin32Adapter
{
	bool IsVirtualMachine(string hostName);
}

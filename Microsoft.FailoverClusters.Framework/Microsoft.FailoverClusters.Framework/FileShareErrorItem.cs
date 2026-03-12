using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class FileShareErrorItem : IFileShareDataItem
{
	public Exception Exception { get; private set; }

	public CimObservableErrorType ErrorType { get; private set; }

	public FileShareProtocol Protocol { get; private set; }

	public string NetName { get; private set; }

	public string CimClassName { get; private set; }

	public FileShareErrorItem(CimObservableErrorType errorType, FileShareProtocol protocol, string className, string netName, Exception exception = null)
	{
		Exception = exception;
		ErrorType = errorType;
		Protocol = protocol;
		CimClassName = className;
		NetName = netName;
	}
}

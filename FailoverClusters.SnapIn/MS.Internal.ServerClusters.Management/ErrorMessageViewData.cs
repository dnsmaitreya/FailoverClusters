using System;

namespace MS.Internal.ServerClusters.Management;

internal class ErrorMessageViewData
{
	internal readonly Exception Exception;

	internal readonly string Message;

	internal ErrorMessageViewData(Exception e, string message)
	{
		Exception = e;
		Message = message;
	}
}

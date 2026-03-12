using System;

namespace Microsoft.FailoverClusters.Framework;

public class ActionResult<T>
{
	public T Result { get; private set; }

	public Exception Error { get; private set; }

	public ActionResult(T resultObject, Exception error)
	{
		Error = error;
		Result = resultObject;
	}
}

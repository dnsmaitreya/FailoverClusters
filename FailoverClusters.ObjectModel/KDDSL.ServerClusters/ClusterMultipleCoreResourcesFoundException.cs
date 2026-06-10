using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterMultipleCoreResourcesFoundException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_MultipleCoreResourcesFound_Text;

	private static int errorCode = 0;

	public override string Header
	{
		get
		{
			object[] args = new object[0];
			return string.Format(CultureInfo.InvariantCulture, Resources.Exception_MultipleCoreResourcesFound_Header_Text, args);
		}
	}

	public override string Caption => Resources.TaskDialog_WindowTitle_MultipleCoreResourcesFound_Text;

	public override int NumericError => errorCode;

	public ClusterMultipleCoreResourcesFoundException(string clusterName, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName), inner)
	{
		base.HResult = errorCode;
	}

	public ClusterMultipleCoreResourcesFoundException(string clusterName)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName))
	{
		base.HResult = errorCode;
	}

	public ClusterMultipleCoreResourcesFoundException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		base.HResult = errorCode;
	}

	protected ClusterMultipleCoreResourcesFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

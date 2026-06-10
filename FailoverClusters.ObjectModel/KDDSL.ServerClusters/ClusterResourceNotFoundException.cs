using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterResourceNotFoundException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_ResourceNotFound_Text;

	private static int errorCode = -2147019890;

	public override string Header
	{
		get
		{
			object[] args = new object[0];
			return string.Format(CultureInfo.InvariantCulture, Resources.Exception_ResourceNotFound_Header_Text, args);
		}
	}

	public override string Caption => Resources.TaskDialog_WindowTitle_ResourceNotFound_Text;

	public override int NumericError => errorCode;

	public ClusterResourceNotFoundException(string clusterName, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName), inner)
	{
		base.HResult = errorCode;
	}

	public ClusterResourceNotFoundException(string clusterName)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName))
	{
		base.HResult = errorCode;
	}

	public ClusterResourceNotFoundException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		base.HResult = errorCode;
	}

	protected ClusterResourceNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

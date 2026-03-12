using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace MS.Internal.ServerClusters;

[Serializable]
public class ClusterEndpointNotRegisteredException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_EndpointNotRegistered_Text;

	private static int errorCode = -2147023143;

	private string parameter;

	public override string Header => string.Format(CultureInfo.InvariantCulture, Resources.Exception_EndpointNotRegistered_Header_Text, parameter);

	public override string Caption => Resources.TaskDialog_WindowTitle_EndpointNotRegistered_Text;

	public override int NumericError => errorCode;

	public ClusterEndpointNotRegisteredException(string parameter, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, parameter), inner)
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	public ClusterEndpointNotRegisteredException(string parameter)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, parameter))
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	public ClusterEndpointNotRegisteredException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		parameter = string.Empty;
		base.HResult = errorCode;
	}

	protected ClusterEndpointNotRegisteredException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		parameter = info.GetString("parameter");
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("parameter", parameter);
	}
}

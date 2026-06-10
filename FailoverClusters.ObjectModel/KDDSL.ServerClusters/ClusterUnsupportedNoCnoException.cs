using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterUnsupportedNoCnoException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_UnsupportedNoCno_Text;

	private static int errorCode = ((global::_003CModule_003E.MS_002EInternal_002EServerClusters_002EERROR_NO_ADMIN_ACCESS_POINT_1 > 0) ? ((global::_003CModule_003E.MS_002EInternal_002EServerClusters_002EERROR_NO_ADMIN_ACCESS_POINT_1 & 0xFFFF) | -2147024896) : global::_003CModule_003E.MS_002EInternal_002EServerClusters_002EERROR_NO_ADMIN_ACCESS_POINT_1);

	private string parameter;

	public override string Header
	{
		get
		{
			object[] args = new object[0];
			return string.Format(CultureInfo.InvariantCulture, Resources.Exception_UnsupportedNoCno_Header_Text, args);
		}
	}

	public override string Caption => Resources.TaskDialog_WindowTitle_UnsupportedNoCno_Text;

	public ClusterUnsupportedNoCnoException(string parameter, Exception inner)
		: base(parameter, inner)
	{
		base.HResult = errorCode;
	}

	public ClusterUnsupportedNoCnoException(string parameter)
		: base(parameter)
	{
		base.HResult = errorCode;
	}

	public ClusterUnsupportedNoCnoException()
		: base(Resources.Exception_UnsupportedNoCno_Text)
	{
		base.HResult = errorCode;
	}

	protected ClusterUnsupportedNoCnoException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
	}
}

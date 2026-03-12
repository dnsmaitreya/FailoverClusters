using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MS.Internal.ServerClusters;

[Serializable]
public class ClusterUnsupportedRoleNoCnoException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_UnsupportedRoleNoCno_Text;

	private static int errorCode = ((global::_003CModule_003E.MS_002EInternal_002EServerClusters_002EERROR_NO_ADMIN_ACCESS_POINT_1 > 0) ? ((global::_003CModule_003E.MS_002EInternal_002EServerClusters_002EERROR_NO_ADMIN_ACCESS_POINT_1 & 0xFFFF) | -2147024896) : global::_003CModule_003E.MS_002EInternal_002EServerClusters_002EERROR_NO_ADMIN_ACCESS_POINT_1);

	private string parameter;

	public override string Header
	{
		get
		{
			object[] args = new object[0];
			return string.Format(CultureInfo.InvariantCulture, Resources.Exception_UnsupportedRoleNoCno_Header_Text, args);
		}
	}

	public override string Caption => Resources.TaskDialog_WindowTitle_UnsupportedNoCno_Text;

	public ClusterUnsupportedRoleNoCnoException(string parameter, Exception inner)
		: base(parameter, inner)
	{
		base.HResult = errorCode;
	}

	public ClusterUnsupportedRoleNoCnoException(string parameter)
		: base(parameter)
	{
		base.HResult = errorCode;
	}

	public ClusterUnsupportedRoleNoCnoException()
		: base(Resources.Exception_UnsupportedRoleNoCno_Text)
	{
		base.HResult = errorCode;
	}

	protected ClusterUnsupportedRoleNoCnoException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
	}
}

using System;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public class ControlCodesEventArgs : EventArgs
{
	private bool m_isResponse;

	private ValueType m_requestTime;

	private string m_controlCodeGroup;

	private string m_controlCodeOwnerName;

	private int m_handle;

	private ClusterNode m_node;

	private int m_controlCode;

	private string m_controlCodeAlias;

	private int m_statusCode;

	private string m_callStack;

	public string CallStack => m_callStack;

	public int StatusCode => m_statusCode;

	public ClusterNode Node => m_node;

	public string ControlCodeAlias => m_controlCodeAlias;

	public int ControlCode => m_controlCode;

	public int Handle => m_handle;

	public string ControlCodeOwnerName => m_controlCodeOwnerName;

	public string ControlCodeGroup => m_controlCodeGroup;

	public ValueType RequestTime => m_requestTime;

	public bool IsResponse
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_isResponse;
		}
	}

	public ControlCodesEventArgs([MarshalAs(UnmanagedType.U1)] bool isResponse, ValueType requestTime, string controlCodeGroup, string controlCodeOwnerName, int handle, ClusterNode node, int controlCode, string controlCodeAlias, int statusCode, string callStack)
	{
		m_isResponse = isResponse;
		m_requestTime = requestTime;
		m_controlCodeGroup = controlCodeGroup;
		m_controlCodeOwnerName = controlCodeOwnerName;
		m_handle = handle;
		m_node = node;
		m_controlCode = controlCode;
		m_controlCodeAlias = controlCodeAlias;
		m_statusCode = statusCode;
		m_callStack = callStack;
	}
}

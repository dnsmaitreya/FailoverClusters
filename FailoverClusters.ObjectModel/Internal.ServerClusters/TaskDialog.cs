using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters;

public class TaskDialog : CommonDialog
{
	private class CallBackObjectData
	{
		public unsafe _TASKDIALOGCONFIG* DialogConfig;

		public string DiagnoseMessage;
	}

	private int m_bverificationChecked;

	private DialogResult m_result;

	private unsafe _TASKDIALOGCONFIG* m_pConfig;

	private GCHandle gcHandle;

	private CallBackObjectData callBackObjectdata;

	private bool hasOwner;

	private bool visible;

	public bool VerificationChecked
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_bverificationChecked == 1;
		}
	}

	public bool Visible
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return visible;
		}
	}

	public bool HasOwner
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return hasOwner;
		}
		[param: MarshalAs(UnmanagedType.U1)]
		set
		{
			hasOwner = value;
		}
	}

	public bool IsCanceled
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_result == DialogResult.Cancel;
		}
	}

	public TaskDialog(ITaskDialogResource taskDialogResource)
	{
		try
		{
			Initialize(taskDialogResource.Caption, taskDialogResource.Header, taskDialogResource.Content, taskDialogResource.Footer, taskDialogResource.Details, taskDialogResource.Diagnose, new ConfirmationMessage(Resources.OK_Text), null, taskDialogResource.Icon, TaskDialogButtons.Ok, MessageBoxDefaultButton.Button2, allowCancel: false, showVerification: false, null, 250, isITaskDialogResource: true);
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(disposing: true);
			throw;
		}
	}

	public TaskDialog(string caption, string message, ConfirmationMessage confirmationMessage, ConfirmationMessage cancelMessage, TaskDialogIcon icon, MessageBoxDefaultButton defaultButton)
	{
		try
		{
			Initialize(caption, message, null, null, null, null, confirmationMessage, cancelMessage, icon, TaskDialogButtons.YesNo, defaultButton, allowCancel: false, showVerification: false, null, -1, isITaskDialogResource: false);
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(disposing: true);
			throw;
		}
	}

	public TaskDialog(string caption, string message, ConfirmationMessage confirmationMessage, TaskDialogIcon icon)
	{
		try
		{
			Initialize(caption, message, null, null, null, null, confirmationMessage, null, icon, TaskDialogButtons.Ok, MessageBoxDefaultButton.Button1, allowCancel: false, showVerification: false, null, -1, isITaskDialogResource: false);
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(disposing: true);
			throw;
		}
	}

	public TaskDialog(string message, ConfirmationMessage confirmationMessage, ConfirmationMessage cancelMessage, ConfirmationStrength strength, string verificationText)
	{
		try
		{
			TaskDialogIcon icon = ((strength != 0) ? TaskDialogIcon.Warning : TaskDialogIcon.Information);
			Initialize(Resources.TaskDialog_WindowTitle_Text, message, null, null, null, null, confirmationMessage, cancelMessage, icon, TaskDialogButtons.YesNo, MessageBoxDefaultButton.Button2, allowCancel: true, showVerification: true, verificationText, -1, isITaskDialogResource: false);
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(disposing: true);
			throw;
		}
	}

	public TaskDialog(string message, ConfirmationMessage confirmationMessage, ConfirmationMessage cancelMessage, ConfirmationStrength strength)
	{
		try
		{
			byte showVerification;
			TaskDialogIcon icon;
			if (strength == ConfirmationStrength.Normal)
			{
				showVerification = 1;
				icon = TaskDialogIcon.Information;
			}
			else
			{
				showVerification = 0;
				icon = TaskDialogIcon.Warning;
			}
			Initialize(Resources.TaskDialog_WindowTitle_Text, message, null, null, null, null, confirmationMessage, cancelMessage, icon, TaskDialogButtons.YesNo, MessageBoxDefaultButton.Button2, allowCancel: true, showVerification != 0, null, -1, isITaskDialogResource: false);
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(disposing: true);
			throw;
		}
	}

	private unsafe void _007ETaskDialog()
	{
		//IL_004f: Expected I, but got I8
		//IL_0077: Expected I, but got I8
		//IL_00a2: Expected I, but got I8
		//IL_00bd: Expected I, but got I8
		//IL_00d5: Expected I, but got I8
		//IL_00fc: Expected I, but got I8
		//IL_0126: Expected I, but got I8
		_TASKDIALOGCONFIG* pConfig = m_pConfig;
		if (pConfig == null)
		{
			return;
		}
		if (System.Runtime.CompilerServices.Unsafe.ReadUnaligned<long>((void*)((long)(nint)pConfig + 64L)) != 0L)
		{
			uint num = 0u;
			if (0u < (uint)System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>((void*)((long)(nint)pConfig + 60L)))
			{
				do
				{
					ulong num2 = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>((void*)((long)num * 12L + System.Runtime.CompilerServices.Unsafe.ReadUnaligned<long>((void*)((long)(nint)m_pConfig + 64L)) + 4));
					if (num2 != 0L)
					{
						InteropHelp.FreeWstr((ushort*)num2);
					}
					num++;
				}
				while (num < (uint)System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>((void*)((long)(nint)m_pConfig + 60L)));
			}
			global::_003CModule_003E.delete_005B_005D((void*)System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>((void*)((long)(nint)m_pConfig + 64L)));
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 64L), 0L);
		}
		ulong num3 = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>((void*)((long)(nint)m_pConfig + 92L));
		if (num3 != 0L)
		{
			InteropHelp.FreeWstr((ushort*)num3);
		}
		ulong num4 = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>((void*)((long)(nint)m_pConfig + 28L));
		if (num4 != 0L)
		{
			InteropHelp.FreeWstr((ushort*)num4);
		}
		ulong num5 = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>((void*)((long)(nint)m_pConfig + 44L));
		if (num5 != 0L)
		{
			InteropHelp.FreeWstr((ushort*)num5);
		}
		pConfig = m_pConfig;
		if (((uint)System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>((void*)((long)(nint)pConfig + 20L)) & 2u) != 0)
		{
			ulong num6 = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>((void*)((long)(nint)pConfig + 36L));
			if (num6 != 0L && global::_003CModule_003E.DestroyIcon((HICON__*)num6) != 0)
			{
				DebugLog.LogError("Error closing the icon handle for the taskdialog");
			}
		}
		gcHandle.Free();
		global::_003CModule_003E.delete(m_pConfig);
		m_pConfig = null;
	}

	public override void Reset()
	{
		m_bverificationChecked = 0;
		m_result = DialogResult.None;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public override bool RunDialog(IntPtr hwndOwner)
	{
		IntPtr parent = ((!hasOwner) ? IntPtr.Zero : hwndOwner);
		DialogResult dialogResult = (m_result = InternalShowDialog(parent));
		int num = ((dialogResult == DialogResult.Yes || dialogResult == DialogResult.OK) ? 1 : 0);
		return (byte)num != 0;
	}

	private unsafe void Initialize(string caption, string header, string content, string footer, string details, string diagnoseMessage, ConfirmationMessage confirmationMessage, ConfirmationMessage cancelMessage, TaskDialogIcon icon, TaskDialogButtons buttons, MessageBoxDefaultButton defaultButton, [MarshalAs(UnmanagedType.U1)] bool allowCancel, [MarshalAs(UnmanagedType.U1)] bool showVerification, string verificationText, int taskDialogWidth, [MarshalAs(UnmanagedType.U1)] bool isITaskDialogResource)
	{
		//IL_0009: Expected I8, but got I
		//IL_0056: Expected I4, but got I8
		//IL_00c7: Expected I8, but got I
		//IL_00fa: Expected I, but got I8
		//IL_0115: Expected I, but got I8
		//IL_0136: Expected I, but got I8
		//IL_0178: Expected I4, but got I8
		//IL_0187: Expected I8, but got I
		//IL_01b7: Expected I8, but got I
		//IL_01d0: Expected I, but got I8
		//IL_01ee: Expected I8, but got I
		//IL_0200: Expected I, but got I8
		//IL_021d: Expected I8, but got I
		//IL_0330: Expected I, but got I8
		//IL_0330: Expected I, but got I8
		//IL_0334: Expected I8, but got I
		//IL_03ad: Expected I8, but got I
		//IL_03c4: Expected I8, but got I
		//IL_03d8: Expected I8, but got I
		//IL_03f5: Expected I8, but got I
		//IL_0409: Expected I8, but got I
		//IL_042b: Expected I8, but got I
		//IL_047f: Expected I8, but got I
		//IL_0496: Expected I8, but got I
		//IL_04d8: Expected I, but got I8
		//IL_04d8: Expected I, but got I8
		//IL_04e9: Expected I, but got I8
		//IL_051a: Expected I, but got I8
		long num = (nint)stackalloc byte[global::_003CModule_003E.__CxxQueryExceptionSize()];
		hasOwner = true;
		if (confirmationMessage == null)
		{
			throw new ArgumentNullException("confirmationMessage");
		}
		if (buttons != 0 && cancelMessage == null)
		{
			throw new ArgumentNullException("cancelMessage");
		}
		try
		{
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(m_pConfig = (_TASKDIALOGCONFIG*)global::_003CModule_003E.@new(160uL), 0, 160);
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned(m_pConfig, 160);
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 20L), 4105);
			if (!isITaskDialogResource)
			{
				*(int*)((ulong)(nint)m_pConfig + 20uL) |= 16;
			}
			if (details != null)
			{
				*(int*)((ulong)(nint)m_pConfig + 20uL) |= 64;
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 100L), (long)(nint)InteropHelp.StringToWstr(details));
			}
			if (taskDialogWidth >= 0)
			{
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 156L), taskDialogWidth);
			}
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 12L), 0L);
			_TASKDIALOGCONFIG* ptr = (_TASKDIALOGCONFIG*)((ulong)(nint)m_pConfig + 60uL);
			(*(int*)ptr)++;
			if (cancelMessage != null)
			{
				ptr = (_TASKDIALOGCONFIG*)((ulong)(nint)m_pConfig + 60uL);
				(*(int*)ptr)++;
			}
			if (diagnoseMessage != null)
			{
				ptr = (_TASKDIALOGCONFIG*)((ulong)(nint)m_pConfig + 60uL);
				(*(int*)ptr)++;
			}
			_TASKDIALOG_BUTTON* ptr2 = (_TASKDIALOG_BUTTON*)global::_003CModule_003E.new_005B_005D((ulong)(uint)System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>((void*)((long)(nint)m_pConfig + 60L)) * 12uL);
			_TASKDIALOG_BUTTON* ptr3 = ptr2;
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ptr2, 0, (long)(uint)System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>((void*)((long)(nint)m_pConfig + 60L)) * 12L);
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 64L), (long)(nint)ptr2);
			if (ptr2 != null)
			{
				int num2 = -1;
				if (diagnoseMessage != null)
				{
					num2 = 0;
					System.Runtime.CompilerServices.Unsafe.WriteUnaligned(ptr2, 1024);
					System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)ptr2 + 4L), (long)(nint)InteropHelp.StringToWstr(Resources.Diagnose_Text));
				}
				int num3 = ((buttons != TaskDialogButtons.YesNo) ? 1 : 6);
				num2++;
				_TASKDIALOG_BUTTON* ptr4 = (_TASKDIALOG_BUTTON*)((long)num2 * 12L + (nint)ptr2);
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned(ptr4, num3);
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)ptr4 + 4L), (long)(nint)InteropHelp.StringToWstr(CombineMessage(confirmationMessage)));
				if (cancelMessage != null)
				{
					num2++;
					_TASKDIALOG_BUTTON* ptr5 = (_TASKDIALOG_BUTTON*)((long)num2 * 12L + (nint)ptr2);
					System.Runtime.CompilerServices.Unsafe.WriteUnaligned(ptr5, 7);
					System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)ptr5 + 4L), (long)(nint)InteropHelp.StringToWstr(CombineMessage(cancelMessage)));
				}
			}
			switch (defaultButton)
			{
			case MessageBoxDefaultButton.Button3:
			{
				_TASKDIALOGCONFIG* pConfig2 = m_pConfig;
				uint num5 = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<uint>((void*)((long)(nint)pConfig2 + 60L));
				if (num5 > 2)
				{
					System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)pConfig2 + 72L), System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>((void*)((long)(nint)ptr2 + 24L)));
				}
				else
				{
					System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)pConfig2 + 72L), System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>((void*)((long)num5 * 12L + (nint)ptr2)));
				}
				break;
			}
			case MessageBoxDefaultButton.Button2:
			{
				_TASKDIALOGCONFIG* pConfig = m_pConfig;
				uint num4 = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<uint>((void*)((long)(nint)pConfig + 60L));
				if (num4 > 1)
				{
					System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)pConfig + 72L), System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>((void*)((long)(nint)ptr2 + 12L)));
				}
				else
				{
					System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)pConfig + 72L), System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>((void*)((long)num4 * 12L + (nint)ptr2)));
				}
				break;
			}
			case MessageBoxDefaultButton.Button1:
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 72L), System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>(ptr2));
				break;
			}
			switch (icon)
			{
			case TaskDialogIcon.Error:
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 36L), 65534L);
				break;
			case TaskDialogIcon.Question:
				*(int*)((ulong)(nint)m_pConfig + 20uL) |= 2;
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 36L), (long)(nint)global::_003CModule_003E.LoadIconW(null, (ushort*)32514));
				break;
			case TaskDialogIcon.Information:
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 36L), 65533L);
				break;
			case TaskDialogIcon.Warning:
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 36L), 65535L);
				break;
			}
			if (allowCancel)
			{
				*(int*)((ulong)(nint)m_pConfig + 20uL) |= 8;
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 24L), 8);
			}
			if (showVerification)
			{
				if (string.IsNullOrEmpty(verificationText))
				{
					System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 92L), (long)(nint)InteropHelp.StringToWstr(Resources.TaskDialog_Verification_Text));
				}
				else
				{
					System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 92L), (long)(nint)InteropHelp.StringToWstr(verificationText));
				}
			}
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 28L), (long)(nint)InteropHelp.StringToWstr(caption));
			if (content != null)
			{
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 52L), (long)(nint)InteropHelp.StringToWstr(content));
			}
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 44L), (long)(nint)InteropHelp.StringToWstr(header));
			if (footer != null)
			{
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 132L), (long)(nint)InteropHelp.StringToWstr(footer));
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 124L), 65533L);
			}
			GetTaskDialogDelegate getTaskDialogDelegate = GetTaskDialogCallBack;
			GCHandle gCHandle = GCHandle.Alloc(getTaskDialogDelegate);
			gcHandle = gCHandle;
			IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate(getTaskDialogDelegate);
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 140L), (long)(nint)functionPointerForDelegate.ToPointer());
			_TASKDIALOGCONFIG* pConfig3 = m_pConfig;
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)pConfig3 + 148L), (long)(nint)pConfig3);
			(callBackObjectdata = new CallBackObjectData()).DialogConfig = m_pConfig;
			callBackObjectdata.DiagnoseMessage = diagnoseMessage;
		}
		catch when (((Func<bool>)delegate
		{
			// Could not convert BlockContainer to single expression
			uint exceptionCode = (uint)Marshal.GetExceptionCode();
			return (byte)global::_003CModule_003E.__CxxExceptionFilter((void*)Marshal.GetExceptionPointers(), null, 0, null) != 0;
		}).Invoke())
		{
			uint num6 = 0u;
			global::_003CModule_003E.__CxxRegisterExceptionObject((void*)Marshal.GetExceptionPointers(), (void*)num);
			try
			{
				try
				{
					((IDisposable)this).Dispose();
					global::_003CModule_003E._CxxThrowException(null, null);
				}
				catch when (((Func<bool>)delegate
				{
					// Could not convert BlockContainer to single expression
					num6 = (uint)global::_003CModule_003E.__CxxDetectRethrow((void*)Marshal.GetExceptionPointers());
					return (byte)num6 != 0;
				}).Invoke())
				{
				}
				if (num6 != 0)
				{
					throw;
				}
			}
			finally
			{
				global::_003CModule_003E.__CxxUnregisterExceptionObject((void*)num, (int)num6);
			}
		}
		m_result = DialogResult.None;
		m_bverificationChecked = 0;
	}

	private unsafe DialogResult InternalShowDialog(IntPtr parent)
	{
		//IL_0023: Expected I8, but got I
		//IL_004c: Expected I, but got I8
		DialogResult result = DialogResult.None;
		if (parent != IntPtr.Zero)
		{
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 4L), (long)(nint)parent.ToPointer());
		}
		else
		{
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)m_pConfig + 4L), 0L);
		}
		visible = true;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out int num);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out int bverificationChecked);
		int num2 = global::_003CModule_003E.TaskDialogIndirect(m_pConfig, &num, null, &bverificationChecked);
		visible = false;
		if (num2 >= 0)
		{
			m_bverificationChecked = bverificationChecked;
			result = ((num == 2) ? DialogResult.Cancel : ((num == 6) ? DialogResult.Yes : ((num == 7) ? DialogResult.No : DialogResult.None)));
		}
		return result;
	}

	private string CombineMessage(ConfirmationMessage message)
	{
		if (message.MessageSubtext != null)
		{
			return string.Format(CultureInfo.CurrentCulture, "{0}\n{1}", message.MessageText, message.MessageSubtext);
		}
		return message.MessageText;
	}

	private unsafe int GetTaskDialogCallBack(int windowHandle, int notification, int highParameter, int lowParameter, IntPtr ptrData)
	{
		//IL_0028: Expected I, but got I8
		switch (notification)
		{
		case 3:
		{
			Process process2 = new Process();
			process2.StartInfo.UseShellExecute = true;
			process2.StartInfo.FileName = InteropHelp.WstrToString((ushort*)lowParameter);
			process2.Start();
			goto default;
		}
		case 2:
			if (highParameter == 1024)
			{
				string tempFileName = Path.GetTempFileName();
				File.WriteAllText(tempFileName, callBackObjectdata.DiagnoseMessage);
				Process process = new Process();
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.FileName = "notepad.exe";
				process.StartInfo.Arguments = tempFileName;
				process.Start();
				return 1;
			}
			goto default;
		default:
			return 0;
		}
	}

	[HandleProcessCorruptedStateExceptions]
	protected override void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			try
			{
				_007ETaskDialog();
				return;
			}
			finally
			{
				base.Dispose(disposing: true);
			}
		}
		base.Dispose(disposing: false);
	}
}

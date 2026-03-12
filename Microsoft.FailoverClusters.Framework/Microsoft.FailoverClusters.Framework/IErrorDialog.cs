using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

public interface IErrorDialog
{
	TaskDialogStandardButtons Buttons { get; }

	string Caption { get; }

	string Header { get; }

	string Content { get; }

	string Footer { get; }

	string Details { get; }

	string Diagnose { get; }

	TaskDialogStandardIcon Icon { get; }
}

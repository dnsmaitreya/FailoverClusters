namespace MS.Internal.ServerClusters;

public interface ITaskDialogResource
{
	string Diagnose { get; }

	TaskDialogIcon Icon { get; }

	string Details { get; }

	string Footer { get; }

	string Content { get; }

	string Header { get; }

	string Caption { get; }
}

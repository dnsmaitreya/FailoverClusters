namespace FailoverClusters.SnapIn;

internal interface IUiActionProducer
{
	void Enqueue(CommandsToActionsContainer commandsToActionsContainer);
}


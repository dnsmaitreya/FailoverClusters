using System;

namespace KDDSL.ServerClusters;

public class DeletingEventArgs : EventArgs
{
	private DeletingStage m_stage;

	public DeletingStage Stage => m_stage;

	public DeletingEventArgs(DeletingStage stage)
	{
		m_stage = stage;
	}
}

using System.Collections.ObjectModel;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal interface ISharesContainer
{
	ReadOnlyObservableCollection<FileShare> Shares { get; }
}


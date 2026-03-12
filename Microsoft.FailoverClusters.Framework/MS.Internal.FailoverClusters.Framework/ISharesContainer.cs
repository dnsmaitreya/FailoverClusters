using System.Collections.ObjectModel;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal interface ISharesContainer
{
	ReadOnlyObservableCollection<FileShare> Shares { get; }
}

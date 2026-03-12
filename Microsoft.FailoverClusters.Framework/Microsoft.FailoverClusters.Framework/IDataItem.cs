using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Microsoft.FailoverClusters.Framework;

public interface IDataItem
{
	string DisplayName { get; }

	IEnumerable<ICommand> Commands { get; }

	Guid Id { get; }
}

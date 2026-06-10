using System.Collections.Generic;

namespace MS.Internal.ServerClusters.Management;

internal delegate void AsyncBatchReady<T>(ICollection<T> items);

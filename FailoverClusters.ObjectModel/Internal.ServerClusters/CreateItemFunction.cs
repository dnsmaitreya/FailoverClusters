using System;

namespace MS.Internal.ServerClusters;

internal delegate T CreateItemFunction<T>(string name, Guid id);

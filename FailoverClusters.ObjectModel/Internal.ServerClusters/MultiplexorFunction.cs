namespace MS.Internal.ServerClusters;

public delegate TOutputData MultiplexorFunction<TInputData, TMultiplexedData, TOutputData>(TInputData inputData, TMultiplexedData multiplexedData);

namespace Microsoft.FailoverClusters.Framework;

public class ElapsedTime
{
	public long ElapsedMilliseconds => 0L;

	public void Start()
	{
	}

	public long StopAndOutput(string message)
	{
		return 0L;
	}

	public static ElapsedTime CreateAndStart()
	{
		ElapsedTime elapsedTime = new ElapsedTime();
		elapsedTime.Start();
		return elapsedTime;
	}
}

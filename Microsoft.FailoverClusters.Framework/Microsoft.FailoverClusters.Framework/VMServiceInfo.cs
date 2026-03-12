using System.ComponentModel;

namespace Microsoft.FailoverClusters.Framework;

public class VMServiceInfo : INotifyPropertyChanged
{
	private bool isMonitored;

	public string Name { get; set; }

	public string DisplayName { get; set; }

	public string Description { get; set; }

	public string Status { get; set; }

	public bool IsMonitored
	{
		get
		{
			return isMonitored;
		}
		set
		{
			if (isMonitored != value)
			{
				isMonitored = value;
				NotifiedPropertyChanged("IsMonitored");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	private void NotifiedPropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

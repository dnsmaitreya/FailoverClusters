using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace MS.Internal.ServerClusters.Management;

internal class CriticalEventsFilterDialog : SnapinForm
{
	private INotifyUser notifyUser;

	private Cluster cluster;

	private IContainer components;

	private Label instructionsLabel1;

	private Button okButton;

	private Button cancelButton;

	private Label eventLogLabel;

	private Label levelLabel;

	private CheckBox criticalCheckBox;

	private CheckBox errorCheckBox;

	private CheckBox warningCheckBox;

	private CheckBox informationalCheckBox;

	private CheckBox verboseCheckBox;

	private Label nodesLabel;

	private CheckedListBox nodesListBox;

	private Label eventIdsLabel;

	private TextBox eventIdsTextBox;

	private Label eventIdsInstructionsLabel;

	private Label fromLabel;

	private ComboBox fromComboBox;

	private DateTimePicker fromDatePicker;

	private DateTimePicker fromTimePicker;

	private DateTimePicker toTimePicker;

	private DateTimePicker toDatePicker;

	private ComboBox toComboBox;

	private Label toLabel;

	private Button resetButton;

	private CheckBox filterSystemLogCheckBox;

	private CheckedListBox channelsListBox;

	internal CriticalEventsFilterDialog(Cluster cluster, ICollection<string> nodes, ICollection<EventChannelName> channels)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		InitializeComponent();
		notifyUser = (INotifyUser)new MessageBoxNotifyUser((IWin32Window)this);
		this.cluster = cluster;
		nodesListBox.BeginUpdate();
		foreach (string node in nodes)
		{
			nodesListBox.Items.Add(node, isChecked: false);
		}
		nodesListBox.EndUpdate();
		channelsListBox.BeginUpdate();
		CheckedListBox.ObjectCollection items = channelsListBox.Items;
		object[] items2 = channels.ToArray();
		items.AddRange(items2);
		channelsListBox.EndUpdate();
		fromComboBox.Items.Add(Resources.FirstEvent_Text);
		fromComboBox.Items.Add(Resources.EventsOn_Text);
		toComboBox.Items.Add(Resources.LastEvent_Text);
		toComboBox.Items.Add(Resources.EventsOn_Text);
		SetDefaultQuery();
	}

	internal void AddNode(string node)
	{
		if (!nodesListBox.Items.Contains(node))
		{
			nodesListBox.Items.Add(node, isChecked: false);
		}
	}

	internal void RemoveNode(string node)
	{
		if (nodesListBox.Items.Contains(node))
		{
			nodesListBox.Items.Remove(node);
		}
	}

	private void ResetDialog()
	{
		for (int i = 0; i < nodesListBox.Items.Count; i++)
		{
			nodesListBox.SetItemChecked(i, value: false);
		}
		foreach (int checkedIndex in channelsListBox.CheckedIndices)
		{
			channelsListBox.SetItemChecked(checkedIndex, value: false);
		}
		filterSystemLogCheckBox.Checked = false;
		criticalCheckBox.Checked = false;
		errorCheckBox.Checked = false;
		warningCheckBox.Checked = false;
		informationalCheckBox.Checked = false;
		verboseCheckBox.Checked = false;
		eventIdsTextBox.Text = string.Empty;
		SelectComboBoxItem(fromComboBox, Resources.EventsOn_Text);
		SelectComboBoxItem(toComboBox, Resources.EventsOn_Text);
		DateTimePicker dateTimePicker = fromDatePicker;
		DateTime value = (fromTimePicker.Value = DateTime.Now);
		dateTimePicker.Value = value;
		DateTimePicker dateTimePicker2 = toDatePicker;
		value = (toTimePicker.Value = DateTime.Now);
		dateTimePicker2.Value = value;
	}

	private static int SelectComboBoxItem(ComboBox comboBox, string item)
	{
		for (int i = 0; i < comboBox.Items.Count; i++)
		{
			if (string.Compare((string)comboBox.Items[i], item, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return comboBox.SelectedIndex = i;
			}
		}
		return -1;
	}

	private static void CheckListBoxItem(CheckedListBox listBox, string item)
	{
		for (int i = 0; i < listBox.Items.Count; i++)
		{
			if (string.Compare((string)listBox.Items[i], item, StringComparison.OrdinalIgnoreCase) == 0)
			{
				listBox.SetItemChecked(i, value: true);
				break;
			}
		}
	}

	private void CheckChannel(string item)
	{
		for (int i = 0; i < channelsListBox.Items.Count; i++)
		{
			if (string.Compare(((EventChannelName)channelsListBox.Items[i]).PathName, item, StringComparison.OrdinalIgnoreCase) == 0)
			{
				channelsListBox.SetItemChecked(i, value: true);
				break;
			}
		}
	}

	private static void CheckListBoxItems(CheckedListBox listBox)
	{
		for (int i = 0; i < listBox.Items.Count; i++)
		{
			listBox.SetItemChecked(i, value: true);
		}
	}

	private void SetDefaultQuery()
	{
		ResetDialog();
		CheckListBoxItems(nodesListBox);
		CheckChannel(EventLog.ClusterChannelOperational);
		CheckChannel(EventLog.SystemChannel);
		CheckChannel(EventLog.ClusterAwareUpdatingChannelAdmin);
		CheckChannel(EventLog.ClusterAwareUpdatingManagementChannelAdmin);
		filterSystemLogCheckBox.Checked = true;
		criticalCheckBox.Checked = true;
		errorCheckBox.Checked = true;
		warningCheckBox.Checked = true;
		SelectComboBoxItem(fromComboBox, Resources.EventsOn_Text);
		DateTimePicker dateTimePicker = fromDatePicker;
		DateTime value = (fromTimePicker.Value = ClusterAdministrator.GetClusterEventsStartTime(cluster, useWaitDialog: true));
		dateTimePicker.Value = value;
		SelectComboBoxItem(toComboBox, Resources.LastEvent_Text);
		DateTimePicker dateTimePicker2 = toDatePicker;
		value = (toTimePicker.Value = DateTime.Now);
		dateTimePicker2.Value = value;
	}

	internal void SaveQuery(string file)
	{
		using XmlTextWriter xmlTextWriter = new XmlTextWriter(file, Encoding.ASCII);
		xmlTextWriter.Formatting = Formatting.Indented;
		xmlTextWriter.Indentation = 4;
		ToXmlDocument().WriteTo(xmlTextWriter);
	}

	private XmlDocument ToXmlDocument()
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlNode xmlNode = xmlDocument.AppendChild(xmlDocument.CreateElement("failoverClusters")).AppendChild(xmlDocument.CreateElement("eventQuery"));
		foreach (string checkedItem in nodesListBox.CheckedItems)
		{
			AppendChild(xmlNode, "node", checkedItem);
		}
		foreach (EventChannelName checkedItem2 in channelsListBox.CheckedItems)
		{
			AppendChild(xmlNode, "channel", checkedItem2.PathName);
		}
		if (criticalCheckBox.Checked)
		{
			AppendChild(xmlNode, "level", "critical");
		}
		if (errorCheckBox.Checked)
		{
			AppendChild(xmlNode, "level", "error");
		}
		if (warningCheckBox.Checked)
		{
			AppendChild(xmlNode, "level", "warning");
		}
		if (informationalCheckBox.Checked)
		{
			AppendChild(xmlNode, "level", "informational");
		}
		if (verboseCheckBox.Checked)
		{
			AppendChild(xmlNode, "level", "verbose");
		}
		foreach (EventIdRange eventId in GetEventIds())
		{
			XmlElement xmlElement = xmlDocument.CreateElement("id");
			xmlElement.SetAttribute("lower", eventId.LowerLimit.ToString(CultureInfo.CurrentCulture));
			xmlElement.SetAttribute("upper", eventId.UpperLimit.ToString(CultureInfo.CurrentCulture));
			xmlElement.SetAttribute("suppress", eventId.Suppress.ToString(CultureInfo.CurrentCulture));
			xmlNode.AppendChild(xmlElement);
		}
		if (fromDatePicker.Enabled)
		{
			AppendChild(xmlNode, "from", CreateDateTime(fromDatePicker.Value, fromTimePicker.Value).ToUniversalTime().ToString("u", CultureInfo.InvariantCulture));
		}
		if (toDatePicker.Enabled)
		{
			AppendChild(xmlNode, "to", CreateDateTime(toDatePicker.Value, toTimePicker.Value).ToUniversalTime().ToString("u", CultureInfo.InvariantCulture));
		}
		AppendChild(xmlNode, "filterSystemLog", filterSystemLogCheckBox.Checked.ToString(CultureInfo.InvariantCulture));
		return xmlDocument;
	}

	private static void AppendChild(XmlNode node, string name, string value)
	{
		XmlElement xmlElement = node.OwnerDocument.CreateElement(name);
		xmlElement.SetAttribute("value", value);
		node.AppendChild(xmlElement);
	}

	private static string GetAttributeValue(XmlNode node, string name)
	{
		XmlAttribute xmlAttribute = node.Attributes[name];
		if (xmlAttribute == null)
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[2]
			{
				Resources.EventFilterAttributeFailed_Text,
				name
			});
		}
		return xmlAttribute.Value;
	}

	internal void OpenQuery(string file)
	{
		FileStream fileStream = null;
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			fileStream = new FileStream(file, FileMode.Open);
			xmlDocument.Load(fileStream);
			FromXmlDocument(xmlDocument);
		}
		finally
		{
			fileStream?.Close();
		}
	}

	private void FromXmlDocument(XmlDocument document)
	{
		XmlElement xmlElement = document.DocumentElement["eventQuery"];
		if (xmlElement == null)
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.EventFilterQueryNotFound_Text });
		}
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		List<EventIdRange> list4 = new List<EventIdRange>();
		DateTime from = DateTime.MinValue;
		DateTime to = DateTime.MaxValue;
		bool @checked = false;
		foreach (XmlNode childNode in xmlElement.ChildNodes)
		{
			string name = childNode.Name;
			if (name != null)
			{
				switch (name.Length)
				{
				case 4:
					switch (name[0])
					{
					case 'n':
						if (!(name == "node"))
						{
							break;
						}
						list.Add(GetAttributeValue(childNode, "value"));
						continue;
					case 'f':
						if (!(name == "from"))
						{
							break;
						}
						from = ParseUniversalDateTime(GetAttributeValue(childNode, "value")).ToLocalTime();
						continue;
					}
					break;
				case 2:
					switch (name[0])
					{
					case 'i':
					{
						if (!(name == "id"))
						{
							break;
						}
						EventIdRange eventIdRange = new EventIdRange();
						eventIdRange.LowerLimit = ParseEventId(GetAttributeValue(childNode, "lower"));
						eventIdRange.UpperLimit = ParseEventId(GetAttributeValue(childNode, "upper"));
						eventIdRange.Suppress = ParseBoolean(GetAttributeValue(childNode, "suppress"));
						if (eventIdRange.LowerLimit > eventIdRange.UpperLimit)
						{
							throw ExceptionHelp.Build<ClusterInputValidationException>(new string[3]
							{
								Resources.EventFilterIdOutOfRange_Text,
								eventIdRange.LowerLimit.ToString(CultureInfo.CurrentCulture),
								eventIdRange.UpperLimit.ToString(CultureInfo.CurrentCulture)
							});
						}
						list4.Add(eventIdRange);
						continue;
					}
					case 't':
						if (!(name == "to"))
						{
							break;
						}
						to = ParseUniversalDateTime(GetAttributeValue(childNode, "value")).ToLocalTime();
						continue;
					}
					break;
				case 7:
					if (!(name == "channel"))
					{
						break;
					}
					list2.Add(GetAttributeValue(childNode, "value"));
					continue;
				case 15:
					if (!(name == "filterSystemLog"))
					{
						break;
					}
					@checked = ParseBoolean(GetAttributeValue(childNode, "value"));
					continue;
				case 5:
				{
					if (!(name == "level"))
					{
						break;
					}
					string attributeValue = GetAttributeValue(childNode, "value");
					switch (attributeValue)
					{
					case "critical":
					case "error":
					case "warning":
					case "informational":
					case "verbose":
						list3.Add(attributeValue);
						break;
					default:
						throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.EventFilterLevelInvalid_Text });
					}
					continue;
				}
				}
			}
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[2]
			{
				Resources.EventFilterInvalidElement_Text,
				childNode.Name
			});
		}
		ResetDialog();
		foreach (string item in list)
		{
			CheckListBoxItem(nodesListBox, item);
		}
		foreach (string item2 in list2)
		{
			CheckChannel(item2);
		}
		filterSystemLogCheckBox.Checked = @checked;
		using (List<string>.Enumerator enumerator2 = list3.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				switch (enumerator2.Current)
				{
				case "critical":
					criticalCheckBox.Checked = true;
					break;
				case "error":
					errorCheckBox.Checked = true;
					break;
				case "warning":
					warningCheckBox.Checked = true;
					break;
				case "informational":
					informationalCheckBox.Checked = true;
					break;
				case "verbose":
					verboseCheckBox.Checked = true;
					break;
				}
			}
		}
		SetEventIds(list4);
		SetDateRange(from, to);
	}

	private void SetEventIds(List<EventIdRange> eventIds)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (EventIdRange eventId in eventIds)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(",");
			}
			if (eventId.Suppress)
			{
				stringBuilder.Append("-");
			}
			if (eventId.LowerLimit == eventId.UpperLimit)
			{
				stringBuilder.Append(eventId.LowerLimit.ToString(CultureInfo.CurrentCulture));
			}
			else
			{
				stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0}-{1}", eventId.LowerLimit, eventId.UpperLimit);
			}
		}
		eventIdsTextBox.Text = stringBuilder.ToString();
	}

	private void SetDateRange(DateTime from, DateTime to)
	{
		DateTime value;
		if (from == DateTime.MinValue)
		{
			SelectComboBoxItem(fromComboBox, Resources.FirstEvent_Text);
		}
		else
		{
			SelectComboBoxItem(fromComboBox, Resources.EventsOn_Text);
			DateTimePicker dateTimePicker = fromDatePicker;
			value = (fromTimePicker.Value = from);
			dateTimePicker.Value = value;
		}
		if (to == DateTime.MaxValue)
		{
			SelectComboBoxItem(toComboBox, Resources.LastEvent_Text);
			return;
		}
		SelectComboBoxItem(toComboBox, Resources.EventsOn_Text);
		DateTimePicker dateTimePicker2 = toDatePicker;
		value = (toTimePicker.Value = to);
		dateTimePicker2.Value = value;
	}

	internal EventLogFilter GetEventFilter()
	{
		List<string> list = new List<string>();
		foreach (EventChannelName checkedItem in channelsListBox.CheckedItems)
		{
			list.Add(checkedItem.PathName);
		}
		EventLevel eventLevel = (EventLevel)0;
		if (criticalCheckBox.Checked)
		{
			eventLevel |= EventLevel.Critical;
		}
		if (errorCheckBox.Checked)
		{
			eventLevel |= EventLevel.Error;
		}
		if (warningCheckBox.Checked)
		{
			eventLevel |= EventLevel.Warning;
		}
		if (informationalCheckBox.Checked)
		{
			eventLevel |= EventLevel.Informational;
		}
		if (verboseCheckBox.Checked)
		{
			eventLevel |= EventLevel.Verbose;
		}
		List<EventIdRange> eventIds = GetEventIds();
		DateTime from = (fromDatePicker.Enabled ? CreateDateTime(fromDatePicker.Value, fromTimePicker.Value) : DateTime.MinValue);
		DateTime to = (toDatePicker.Enabled ? CreateDateTime(toDatePicker.Value, toTimePicker.Value) : DateTime.MaxValue);
		EventLogFilter eventLogFilter = new EventLogFilter();
		foreach (string checkedItem2 in nodesListBox.CheckedItems)
		{
			eventLogFilter.Nodes.Add(checkedItem2);
		}
		foreach (string item in list)
		{
			eventLogFilter.Channels.Add(item);
		}
		eventLogFilter.Level = eventLevel;
		eventLogFilter.EventIds.AddRange(eventIds);
		eventLogFilter.From = from;
		eventLogFilter.To = to;
		if (filterSystemLogCheckBox.Enabled && filterSystemLogCheckBox.Checked)
		{
			eventLogFilter.Providers = EventLogFilter.ProvidersForSystemChannel;
		}
		return eventLogFilter;
	}

	private void ValidateEventFilter(EventLogFilter filter)
	{
		if (filter.Channels.Count == 0)
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.EventFilterNoChanel_Text });
		}
		if (filter.Providers != null && filter.Providers.Count > 0)
		{
			if (filter.Providers != EventLogFilter.ProvidersForSystemChannel)
			{
				throw ExceptionHelp.Build<ClusterInputValidationException>(new string[2]
				{
					Resources.EventFilterClusterProviderNotSpecified_Text,
					EventLog.ClusterProvider
				});
			}
			if (!filter.Channels.Any((string channel) => string.Compare(channel, EventLog.SystemChannel, StringComparison.OrdinalIgnoreCase) == 0))
			{
				throw ExceptionHelp.Build<ClusterInputValidationException>(new string[2]
				{
					Resources.EventFilterSystemChanelNotIncluded_Text,
					EventLog.SystemChannel
				});
			}
		}
	}

	internal void SetEventFilter(EventLogFilter filter)
	{
		ValidateEventFilter(filter);
		ResetDialog();
		if (filter.Nodes.Count > 0)
		{
			foreach (string node in filter.Nodes)
			{
				CheckListBoxItem(nodesListBox, node);
			}
		}
		else
		{
			CheckListBoxItems(nodesListBox);
		}
		foreach (string channel in filter.Channels)
		{
			if (string.Compare(channel, EventLog.SystemChannel, StringComparison.OrdinalIgnoreCase) == 0 && filter.Providers != null && filter.Providers.Count > 0)
			{
				filterSystemLogCheckBox.Checked = true;
			}
			CheckChannel(channel);
		}
		if ((filter.Level & EventLevel.Critical) != 0)
		{
			criticalCheckBox.Checked = true;
		}
		if ((filter.Level & EventLevel.Error) != 0)
		{
			errorCheckBox.Checked = true;
		}
		if ((filter.Level & EventLevel.Warning) != 0)
		{
			warningCheckBox.Checked = true;
		}
		if ((filter.Level & EventLevel.Informational) != 0)
		{
			informationalCheckBox.Checked = true;
		}
		if ((filter.Level & EventLevel.Verbose) != 0)
		{
			verboseCheckBox.Checked = true;
		}
		SetEventIds(filter.EventIds);
		SetDateRange(filter.From, filter.To);
	}

	private List<EventIdRange> GetEventIds()
	{
		List<EventIdRange> list = new List<EventIdRange>();
		if (eventIdsTextBox.Text.Length > 0)
		{
			string[] array = eventIdsTextBox.Text.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (text.Length == 0)
				{
					throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.InvalidEventIdFilter_Text });
				}
				EventIdRange eventIdRange = new EventIdRange();
				if (text[0] == '-')
				{
					if (text.Length == 1)
					{
						throw ExceptionHelp.Build<ClusterInputValidationException>(new string[2]
						{
							Resources.InvalidEventId_Text,
							text
						});
					}
					eventIdRange.Suppress = true;
					text = text.Substring(1);
				}
				string[] array2 = text.Split('-');
				switch (array2.Length)
				{
				case 1:
					eventIdRange.LowerLimit = (eventIdRange.UpperLimit = ParseEventId(array2[0]));
					break;
				case 2:
					eventIdRange.LowerLimit = ParseEventId(array2[0]);
					eventIdRange.UpperLimit = ParseEventId(array2[1]);
					break;
				default:
					throw ExceptionHelp.Build<ClusterInputValidationException>(new string[2]
					{
						Resources.InvalidEventIdRange_Text,
						text
					});
				}
				if (eventIdRange.UpperLimit < eventIdRange.LowerLimit)
				{
					throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.InvalidEventIdLimit_Text });
				}
				list.Add(eventIdRange);
			}
		}
		return list;
	}

	private int ParseEventId(string id)
	{
		try
		{
			return int.Parse(id, CultureInfo.CurrentCulture);
		}
		catch (FormatException)
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[2]
			{
				Resources.InvalidEventId_Text,
				id
			});
		}
		catch (OverflowException)
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[4]
			{
				Resources.EventIdOverflow_Text,
				id,
				int.MinValue.ToString(CultureInfo.CurrentCulture),
				int.MaxValue.ToString(CultureInfo.CurrentCulture)
			});
		}
	}

	private bool ParseBoolean(string value)
	{
		try
		{
			return bool.Parse(value);
		}
		catch (FormatException)
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.QueryFileFormatInvalid_Text });
		}
	}

	private DateTime ParseUniversalDateTime(string value)
	{
		try
		{
			return DateTime.ParseExact(value, "u", CultureInfo.InvariantCulture);
		}
		catch (FormatException)
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.QueryFileFormatInvalid_Text });
		}
	}

	private DateTime CreateDateTime(DateTime date, DateTime time)
	{
		return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
	}

	private void ValidateForm()
	{
		if (nodesListBox.CheckedItems.Count == 0)
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.SelectNode_Text });
		}
		if (channelsListBox.CheckedItems.Count == 0)
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.SelectLog_Text });
		}
		if (!criticalCheckBox.Checked && !errorCheckBox.Checked && !warningCheckBox.Checked && !informationalCheckBox.Checked && !verboseCheckBox.Checked)
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.SelectEventLevel_Text });
		}
		GetEventIds();
		if (fromDatePicker.Enabled && toDatePicker.Enabled && CreateDateTime(fromDatePicker.Value, fromTimePicker.Value) >= CreateDateTime(toDatePicker.Value, toTimePicker.Value))
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.InvalidDateRange_Text });
		}
	}

	private void OkButtonClick(object sender, EventArgs e)
	{
		try
		{
			ValidateForm();
		}
		catch (ClusterInputValidationException ex)
		{
			notifyUser.ShowError((Exception)ex);
			return;
		}
		((Form)this).DialogResult = DialogResult.OK;
	}

	private void CancelButtonClick(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.Cancel;
	}

	private void FromComboBoxSelectedIndexChanged(object sender, EventArgs e)
	{
		DateTimePicker dateTimePicker = fromDatePicker;
		bool enabled = (fromTimePicker.Enabled = string.Compare(fromComboBox.SelectedItem.ToString(), Resources.EventsOn_Text, StringComparison.OrdinalIgnoreCase) == 0);
		dateTimePicker.Enabled = enabled;
	}

	private void ToComboBoxSelectedIndexChanged(object sender, EventArgs e)
	{
		DateTimePicker dateTimePicker = toDatePicker;
		bool enabled = (toTimePicker.Enabled = string.Compare(toComboBox.SelectedItem.ToString(), Resources.EventsOn_Text, StringComparison.OrdinalIgnoreCase) == 0);
		dateTimePicker.Enabled = enabled;
	}

	private void ResetButtonClick(object sender, EventArgs e)
	{
		SetDefaultQuery();
	}

	private void OnChannelsListBoxItemCheck(object sender, ItemCheckEventArgs e)
	{
		if (((EventChannelName)channelsListBox.Items[e.Index]).PathName.Equals(EventLog.SystemChannel, StringComparison.OrdinalIgnoreCase))
		{
			filterSystemLogCheckBox.Enabled = e.NewValue == CheckState.Checked;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CriticalEventsFilterDialog));
		instructionsLabel1 = new Label();
		okButton = new Button();
		cancelButton = new Button();
		eventLogLabel = new Label();
		levelLabel = new Label();
		criticalCheckBox = new CheckBox();
		errorCheckBox = new CheckBox();
		warningCheckBox = new CheckBox();
		informationalCheckBox = new CheckBox();
		verboseCheckBox = new CheckBox();
		nodesLabel = new Label();
		nodesListBox = new CheckedListBox();
		eventIdsLabel = new Label();
		eventIdsTextBox = new TextBox();
		eventIdsInstructionsLabel = new Label();
		fromLabel = new Label();
		fromComboBox = new ComboBox();
		fromDatePicker = new DateTimePicker();
		fromTimePicker = new DateTimePicker();
		toTimePicker = new DateTimePicker();
		toDatePicker = new DateTimePicker();
		toComboBox = new ComboBox();
		toLabel = new Label();
		filterSystemLogCheckBox = new CheckBox();
		resetButton = new Button();
		channelsListBox = new CheckedListBox();
		((Control)this).SuspendLayout();
		instructionsLabel1.AutoEllipsis = true;
		componentResourceManager.ApplyResources(instructionsLabel1, "instructionsLabel1");
		instructionsLabel1.Name = "instructionsLabel1";
		componentResourceManager.ApplyResources(okButton, "okButton");
		okButton.Name = "okButton";
		okButton.Click += OkButtonClick;
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.Name = "cancelButton";
		cancelButton.Click += CancelButtonClick;
		eventLogLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(eventLogLabel, "eventLogLabel");
		eventLogLabel.Name = "eventLogLabel";
		componentResourceManager.ApplyResources(levelLabel, "levelLabel");
		levelLabel.AutoEllipsis = true;
		levelLabel.Name = "levelLabel";
		componentResourceManager.ApplyResources(criticalCheckBox, "criticalCheckBox");
		criticalCheckBox.AutoEllipsis = true;
		criticalCheckBox.Name = "criticalCheckBox";
		criticalCheckBox.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(errorCheckBox, "errorCheckBox");
		errorCheckBox.AutoEllipsis = true;
		errorCheckBox.Name = "errorCheckBox";
		errorCheckBox.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(warningCheckBox, "warningCheckBox");
		warningCheckBox.AutoEllipsis = true;
		warningCheckBox.Name = "warningCheckBox";
		warningCheckBox.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(informationalCheckBox, "informationalCheckBox");
		informationalCheckBox.AutoEllipsis = true;
		informationalCheckBox.Name = "informationalCheckBox";
		informationalCheckBox.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(verboseCheckBox, "verboseCheckBox");
		verboseCheckBox.AutoEllipsis = true;
		verboseCheckBox.Name = "verboseCheckBox";
		verboseCheckBox.UseVisualStyleBackColor = true;
		nodesLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(nodesLabel, "nodesLabel");
		nodesLabel.Name = "nodesLabel";
		componentResourceManager.ApplyResources(nodesListBox, "nodesListBox");
		nodesListBox.CheckOnClick = true;
		nodesListBox.FormattingEnabled = true;
		nodesListBox.Name = "nodesListBox";
		nodesListBox.Sorted = true;
		componentResourceManager.ApplyResources(eventIdsLabel, "eventIdsLabel");
		eventIdsLabel.AutoEllipsis = true;
		eventIdsLabel.Name = "eventIdsLabel";
		componentResourceManager.ApplyResources(eventIdsTextBox, "eventIdsTextBox");
		eventIdsTextBox.Name = "eventIdsTextBox";
		eventIdsInstructionsLabel.AccessibleRole = AccessibleRole.None;
		componentResourceManager.ApplyResources(eventIdsInstructionsLabel, "eventIdsInstructionsLabel");
		eventIdsInstructionsLabel.Name = "eventIdsInstructionsLabel";
		componentResourceManager.ApplyResources(fromLabel, "fromLabel");
		fromLabel.AutoEllipsis = true;
		fromLabel.Name = "fromLabel";
		componentResourceManager.ApplyResources(fromComboBox, "fromComboBox");
		fromComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		fromComboBox.Name = "fromComboBox";
		fromComboBox.SelectedIndexChanged += FromComboBoxSelectedIndexChanged;
		componentResourceManager.ApplyResources(fromDatePicker, "fromDatePicker");
		fromDatePicker.Format = DateTimePickerFormat.Short;
		fromDatePicker.Name = "fromDatePicker";
		componentResourceManager.ApplyResources(fromTimePicker, "fromTimePicker");
		fromTimePicker.Format = DateTimePickerFormat.Time;
		fromTimePicker.Name = "fromTimePicker";
		fromTimePicker.ShowUpDown = true;
		componentResourceManager.ApplyResources(toTimePicker, "toTimePicker");
		toTimePicker.Format = DateTimePickerFormat.Time;
		toTimePicker.Name = "toTimePicker";
		toTimePicker.ShowUpDown = true;
		componentResourceManager.ApplyResources(toDatePicker, "toDatePicker");
		toDatePicker.Format = DateTimePickerFormat.Short;
		toDatePicker.Name = "toDatePicker";
		componentResourceManager.ApplyResources(toComboBox, "toComboBox");
		toComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		toComboBox.Name = "toComboBox";
		toComboBox.SelectedIndexChanged += ToComboBoxSelectedIndexChanged;
		componentResourceManager.ApplyResources(toLabel, "toLabel");
		toLabel.AutoEllipsis = true;
		toLabel.Name = "toLabel";
		componentResourceManager.ApplyResources(filterSystemLogCheckBox, "filterSystemLogCheckBox");
		filterSystemLogCheckBox.AutoEllipsis = true;
		filterSystemLogCheckBox.Name = "filterSystemLogCheckBox";
		filterSystemLogCheckBox.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(resetButton, "resetButton");
		resetButton.Name = "resetButton";
		resetButton.Click += ResetButtonClick;
		componentResourceManager.ApplyResources(channelsListBox, "channelsListBox");
		channelsListBox.CheckOnClick = true;
		channelsListBox.FormattingEnabled = true;
		channelsListBox.Name = "channelsListBox";
		channelsListBox.Sorted = true;
		channelsListBox.ItemCheck += OnChannelsListBoxItemCheck;
		((Form)this).AcceptButton = okButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add(channelsListBox);
		((Control)this).Controls.Add(resetButton);
		((Control)this).Controls.Add(filterSystemLogCheckBox);
		((Control)this).Controls.Add(okButton);
		((Control)this).Controls.Add(toTimePicker);
		((Control)this).Controls.Add(cancelButton);
		((Control)this).Controls.Add(instructionsLabel1);
		((Control)this).Controls.Add(toDatePicker);
		((Control)this).Controls.Add(eventIdsLabel);
		((Control)this).Controls.Add(eventLogLabel);
		((Control)this).Controls.Add(nodesListBox);
		((Control)this).Controls.Add(toComboBox);
		((Control)this).Controls.Add(eventIdsTextBox);
		((Control)this).Controls.Add(levelLabel);
		((Control)this).Controls.Add(nodesLabel);
		((Control)this).Controls.Add(toLabel);
		((Control)this).Controls.Add(eventIdsInstructionsLabel);
		((Control)this).Controls.Add(criticalCheckBox);
		((Control)this).Controls.Add(verboseCheckBox);
		((Control)this).Controls.Add(fromTimePicker);
		((Control)this).Controls.Add(fromLabel);
		((Control)this).Controls.Add(errorCheckBox);
		((Control)this).Controls.Add(informationalCheckBox);
		((Control)this).Controls.Add(fromDatePicker);
		((Control)this).Controls.Add(fromComboBox);
		((Control)this).Controls.Add(warningCheckBox);
		((Control)this).Name = "CriticalEventsFilterDialog";
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}

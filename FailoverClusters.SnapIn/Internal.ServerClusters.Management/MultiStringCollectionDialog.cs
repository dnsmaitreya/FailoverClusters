using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class MultiStringCollectionDialog : SnapinForm
{
	private enum Direction
	{
		Up,
		Down
	}

	private IContainer components;

	private Button okButton;

	private Button cancelButton;

	private Button addButton;

	private Button deleteButton;

	private Button upButton;

	private Button downButton;

	private ListBox stringListBox;

	private TextBox addEditTextBox;

	private Label instructionsLabel;

	public StringCollection StringCollection
	{
		get
		{
			StringCollection stringCollection = new StringCollection();
			foreach (string item in stringListBox.Items)
			{
				if (!string.IsNullOrEmpty(item))
				{
					stringCollection.Add(item);
				}
			}
			return stringCollection;
		}
		set
		{
			stringListBox.Items.Clear();
			StringEnumerator enumerator = value.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					stringListBox.Items.Add(current);
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
	}

	public MultiStringCollectionDialog()
	{
		InitializeComponent();
	}

	private void addButton_Click(object sender, EventArgs e)
	{
		string text = addEditTextBox.Text.Trim();
		addEditTextBox.Text = string.Empty;
		addEditTextBox.AcceptsReturn = true;
		addEditTextBox.Focus();
		if (text.Length > 0)
		{
			int selectedIndex = stringListBox.SelectedIndex;
			if (selectedIndex < 0)
			{
				stringListBox.Items.Add(text);
				return;
			}
			stringListBox.Items.RemoveAt(selectedIndex);
			stringListBox.Items.Insert(selectedIndex, text);
		}
	}

	private void deleteButton_Click(object sender, EventArgs e)
	{
		stringListBox.Items.RemoveAt(stringListBox.SelectedIndex);
		addEditTextBox.Focus();
	}

	private void upButton_Click(object sender, EventArgs e)
	{
		MoveSelectedString(Direction.Up);
	}

	private void downButton_Click(object sender, EventArgs e)
	{
		MoveSelectedString(Direction.Down);
	}

	private void MoveSelectedString(Direction direction)
	{
		int selectedIndex = stringListBox.SelectedIndex;
		string item = (string)stringListBox.SelectedItem;
		int num = ((direction != 0) ? (selectedIndex + 1) : (selectedIndex - 1));
		stringListBox.Items.RemoveAt(selectedIndex);
		stringListBox.Items.Insert(num, item);
		stringListBox.SelectedIndex = num;
	}

	private void stringListBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		int selectedIndex = stringListBox.SelectedIndex;
		if (selectedIndex < 0)
		{
			deleteButton.Enabled = false;
			upButton.Enabled = false;
			downButton.Enabled = false;
			addEditTextBox.Text = string.Empty;
			addEditTextBox.AcceptsReturn = false;
			addButton.Text = Resources.AddButton_Text;
			return;
		}
		deleteButton.Enabled = true;
		if (selectedIndex > 0)
		{
			upButton.Enabled = true;
		}
		if (selectedIndex < stringListBox.Items.Count - 1)
		{
			downButton.Enabled = true;
		}
		addEditTextBox.Text = (string)stringListBox.SelectedItem;
		addEditTextBox.AcceptsReturn = true;
		addButton.Text = Resources.ApplyButton_Text;
	}

	private void addEditTextBox_TextChanged(object sender, EventArgs e)
	{
		if (addEditTextBox.Text.Length > 0)
		{
			addButton.Enabled = true;
		}
		else
		{
			addButton.Enabled = false;
		}
	}

	private void MultiStringCollectionForm_Load(object sender, EventArgs e)
	{
		addButton.Enabled = false;
		deleteButton.Enabled = false;
		upButton.Enabled = false;
		downButton.Enabled = false;
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MultiStringCollectionDialog));
		okButton = new Button();
		cancelButton = new Button();
		addButton = new Button();
		deleteButton = new Button();
		upButton = new Button();
		downButton = new Button();
		stringListBox = new ListBox();
		addEditTextBox = new TextBox();
		instructionsLabel = new Label();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(okButton, "okButton");
		okButton.DialogResult = DialogResult.OK;
		okButton.Name = "okButton";
		okButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.Name = "cancelButton";
		cancelButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(addButton, "addButton");
		addButton.Name = "addButton";
		addButton.UseVisualStyleBackColor = true;
		addButton.Click += addButton_Click;
		componentResourceManager.ApplyResources(deleteButton, "deleteButton");
		deleteButton.Name = "deleteButton";
		deleteButton.UseVisualStyleBackColor = true;
		deleteButton.Click += deleteButton_Click;
		componentResourceManager.ApplyResources(upButton, "upButton");
		upButton.Name = "upButton";
		upButton.UseVisualStyleBackColor = true;
		upButton.Click += upButton_Click;
		componentResourceManager.ApplyResources(downButton, "downButton");
		downButton.Name = "downButton";
		downButton.UseVisualStyleBackColor = true;
		downButton.Click += downButton_Click;
		componentResourceManager.ApplyResources(stringListBox, "stringListBox");
		stringListBox.FormattingEnabled = true;
		stringListBox.Name = "stringListBox";
		stringListBox.SelectedIndexChanged += stringListBox_SelectedIndexChanged;
		componentResourceManager.ApplyResources(addEditTextBox, "addEditTextBox");
		addEditTextBox.Name = "addEditTextBox";
		addEditTextBox.TextChanged += addEditTextBox_TextChanged;
		componentResourceManager.ApplyResources(instructionsLabel, "instructionsLabel");
		instructionsLabel.Name = "instructionsLabel";
		((Form)this).AcceptButton = okButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add(instructionsLabel);
		((Control)this).Controls.Add(addEditTextBox);
		((Control)this).Controls.Add(stringListBox);
		((Control)this).Controls.Add(downButton);
		((Control)this).Controls.Add(upButton);
		((Control)this).Controls.Add(deleteButton);
		((Control)this).Controls.Add(addButton);
		((Control)this).Controls.Add(cancelButton);
		((Control)this).Controls.Add(okButton);
		((Control)this).Name = "MultiStringCollectionDialog";
		((Form)this).Load += MultiStringCollectionForm_Load;
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}

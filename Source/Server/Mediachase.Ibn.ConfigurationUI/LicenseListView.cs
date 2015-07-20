using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;
using Mediachase.Ibn.Configuration;
using System.Windows.Forms;
using System.Globalization;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents License List View.
	/// </summary>
	public class LicenseListView : MmcListView
	{
		#region Const
		#endregion

		#region Properties
		protected LicenseScopeNode LicenseScopeNode { get { return (LicenseScopeNode)this.ScopeNode; } }

		protected SyncAction CopyToClipboardAction { get; private set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="LicenseListView"/> class.
		/// </summary>
		public LicenseListView()
		{
			this.CopyToClipboardAction = new SyncAction(SnapInResources.CompanySettingsListView_Action_CopyToClipboard_Name,
	SnapInResources.CompanySettingsListView_Action_CopyToClipboard_Description);
			this.CopyToClipboardAction.ImageIndex = 19;

		}
		#endregion

		#region Methods
		/// <summary>
		/// Allows snap-in code to perform custom view initialization.
		/// </summary>
		/// <param name="status">The status object to update.</param>
		protected override void OnInitialize(AsyncStatus status)
		{
			base.OnInitialize(status);

			// Create a set of columns for use in the list view
			// Define the default column title
			this.Columns[0].Title = SnapInResources.LicenseListView_Column_Name;
			this.Columns[0].SetWidth(200);

			// Add detail column
			this.Columns.Add(new MmcListViewColumn(SnapInResources.LicenseListView_Column_Value, 300));

			// Set to show all columns
			this.Mode = MmcListViewMode.Report;  // default (set for clarity)

			this.LicenseScopeNode.LicenseListView = this;

			// Set to show refresh as an option
			LoadLicenseProperties();
		}

		/// <summary>
		/// Loads the license properties.
		/// </summary>
		private void LoadLicenseProperties()
		{
			IConfigurator configurator = this.LicenseScopeNode.Configurator;

			try
			{
				foreach (IConfigurationParameter param in configurator.ListLicenseProperties())
				{
					string value = param.Value;

					// Reformat DateTime string from InvariantCulture to CurrentUICulture.
					if (param.Type == "DateTime" && !string.IsNullOrEmpty(value))
						value = DateTime.Parse(value, CultureInfo.InvariantCulture).ToString(CultureInfo.CurrentUICulture);

					AddLicensePropertyNode(param.Name, value);
				}
			}
			catch (LicenseExpiredException)
			{
				AddLicensePropertyNode("ErrorMessage", SnapInResources.License_Expired);
				AddLicensePropertyNode("ErrorInfo", string.Format(SnapInResources.License_Expired_Text, Mediachase.Ibn.IbnConst.FullProductName));
			}
			catch (Exception ex)
			{
				AddLicensePropertyNode("ErrorMessage", ex.Message);
				AddLicensePropertyNode("ErrorInfo", ex.ToString());
			}
		}

		/// <summary>
		/// Adds the license property node.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		private void AddLicensePropertyNode(string key, string value)
		{
			AddLicensePropertyNode(key, value, 18);
		}

		/// <summary>
		/// Adds the result node.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		private void AddLicensePropertyNode(string key, string value, int imageIndex)
		{
			ResultNode node = new ResultNode();
			node.DisplayName = key;
			node.ImageIndex = imageIndex;
			node.SubItemDisplayNames.Add(value ?? "");

			this.ResultNodes.Add(node);
		}

		/// <summary>
		/// Refreshes this instance.
		/// </summary>
		public void Refresh()
		{
			this.ResultNodes.Clear();

			LoadLicenseProperties();
		}

		/// <summary>
		/// Called when a selection changes. The snap-in should override this method to read the updated <see cref="P:Microsoft.ManagementConsole.MmcListView.SelectedNodes"></see> property and update <see cref="T:Microsoft.ManagementConsole.SelectionData"></see> class accordingly.
		/// </summary>
		/// <param name="status">The object that holds the status information.</param>
		protected override void OnSelectionChanged(SyncStatus status)
		{
			if (this.SelectedNodes.Count == 0)
			{
				// No items are selected; clear selection data and associated actions.
				SelectionData.Clear();
				SelectionData.ActionsPaneItems.Clear();
			}
			else
			{
				StringBuilder selectedItems = new StringBuilder();

				foreach (ResultNode node in this.SelectedNodes)
				{
					selectedItems.Append(node.DisplayName);
					selectedItems.Append(" ");
					selectedItems.Append(string.Join(" ", node.SubItemDisplayNames.ToArray()));
					selectedItems.AppendLine();
				}

				SelectionData.Update(selectedItems.ToString(), this.SelectedNodes.Count > 1, null, null);

				SelectionData.ActionsPaneItems.Clear();
				SelectionData.ActionsPaneItems.Add(this.CopyToClipboardAction);
			}
		}

		/// <summary>
		/// Handles the execution of a selection-dependent action that runs synchronous to MMC.
		/// </summary>
		/// <param name="action">The executed action.</param>
		/// <param name="status">The object that holds the status information.</param>
		protected override void OnSyncSelectionAction(SyncAction action, SyncStatus status)
		{
			if (action == this.CopyToClipboardAction)
			{
				OnCopyToClipboard(status);
			}

			base.OnSyncAction(action, status);
		}

		/// <summary>
		/// Called when [copy to clipboard].
		/// </summary>
		/// <param name="status">The status.</param>
		private void OnCopyToClipboard(SyncStatus status)
		{
			string seletectedElements = (string)SelectionData.SelectionObject;

			Clipboard.SetText(seletectedElements);
		}
		#endregion

		
	}
}

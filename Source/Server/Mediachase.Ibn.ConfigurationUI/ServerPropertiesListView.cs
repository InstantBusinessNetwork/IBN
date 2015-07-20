using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;
using Mediachase.Ibn.Configuration;
using System.Windows.Forms;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents Server Properties List View.
	/// </summary>
	public class ServerPropertiesListView : MmcListView
	{
		#region Const
		#endregion

		#region Properties
		protected ServerPropertiesScopeNode ServerPropertiesScopeNode { get { return (ServerPropertiesScopeNode)this.ScopeNode; } }

		protected SyncAction CopyToClipboardAction { get; private set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ServerListView"/> class.
		/// </summary>
		public ServerPropertiesListView()
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
			this.Columns[0].Title = SnapInResources.ServerListView_Column_Name;
			this.Columns[0].SetWidth(200);

			// Add detail column
			this.Columns.Add(new MmcListViewColumn(SnapInResources.ServerListView_Column_Value, 300));

			// Set to show all columns
			this.Mode = MmcListViewMode.Report;  // default (set for clarity)

			this.ServerPropertiesScopeNode.ServerPropertiesListView = this;

			// Set to show refresh as an option
			LoadServerProperties();
		}

		/// <summary>
		/// Loads the license properties.
		/// </summary>
		private void LoadServerProperties()
		{
			IConfigurator configurator = this.ServerPropertiesScopeNode.Configurator;

			try
			{
				foreach (IConfigurationParameter param in configurator.ListServerProperties())
				{
					AddServerPropertyNode(param.Name, param.Value);
				}
			}
			catch (Exception ex)
			{
				AddServerPropertyNode("ErrorMessage", ex.Message);
				AddServerPropertyNode("ErrorInfo", ex.ToString());
			}
		}

		/// <summary>
		/// Adds the license property node.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		private void AddServerPropertyNode(string key, string value)
		{
			AddServerPropertyNode(key, value, 18);
		}

		/// <summary>
		/// Adds the result node.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		private void AddServerPropertyNode(string key, string value, int imageIndex)
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

			LoadServerProperties();
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

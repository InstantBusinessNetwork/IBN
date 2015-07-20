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
	/// Represents Company Statistics View.
	/// </summary>
	public class CompanySettingsListView : MmcListView
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CompanyStatisticsView"/> class.
		/// </summary>
		public CompanySettingsListView()
		{
			this.CopyToClipboardAction = new SyncAction(SnapInResources.CompanySettingsListView_Action_CopyToClipboard_Name,
				SnapInResources.CompanySettingsListView_Action_CopyToClipboard_Description);
			this.CopyToClipboardAction.ImageIndex = 19;
		}
		#endregion

		#region Properties
		protected CompanyScopeNode CompanyScopeNode { get { return (CompanyScopeNode)this.ScopeNode; } }

		/// <summary>
		/// Gets or sets the copy to clipboard action.
		/// </summary>
		/// <value>The copy to clipboard action.</value>
		protected SyncAction CopyToClipboardAction { get; private set; }
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
			this.Columns[0].Title = SnapInResources.CompanySettingsListView_Column_Key;
			this.Columns[0].SetWidth(200);

			// Add detail column
			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanySettingsListView_Column_Value, 300));

			// Set to show all columns
			this.Mode = MmcListViewMode.Report;  // default (set for clarity)

			//this.SelectionData.EnabledStandardVerbs = StandardVerbs.Copy;// | StandardVerbs.Paste;
			this.CompanyScopeNode.CompanySettingsListView = this;

			LoadCompanySettings();
		}

		/// <summary>
		/// Loads the company settings.
		/// </summary>
		private void LoadCompanySettings()
		{
			IConfigurator configurator = this.CompanyScopeNode.GetCompanyConfigurator();

			ICompanyInfo companyInfo = configurator.GetCompanyInfo((string)this.CompanyScopeNode.Tag);

			// Read Company Info
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_Status, companyInfo.IsActive ? SnapInResources.Company_ActiveStatusName : SnapInResources.Company_InactiveStatusName);
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_Ssl, companyInfo.Scheme);
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_PrimaryDomain, companyInfo.Host);
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_Port, companyInfo.Port);
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_Build, companyInfo.CodeVersion.ToString(CultureInfo.CurrentUICulture));
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_DatabaseName, companyInfo.Database);
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_DatabaseVersion, companyInfo.DatabaseVersion.ToString(CultureInfo.CurrentUICulture));
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_CodePath, companyInfo.CodePath);
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_SiteId, companyInfo.SiteId.ToString(CultureInfo.CurrentUICulture));
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_IMPool, companyInfo.IMPool);
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_PortalPool, companyInfo.PortalPool);
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_DatabaseState, companyInfo.DatabaseState.ToString(CultureInfo.CurrentUICulture));
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_DefaultLanguage, companyInfo.DefaultLanguage.FriendlyName);
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_DatabaseSize, companyInfo.DatabaseSize.ToString(CultureInfo.CurrentUICulture));
			AddCompanySettingNode(SnapInResources.CompanyListView_Column_UsersCount, companyInfo.InternalUsersCount.ToString(CultureInfo.CurrentUICulture));
			
			// Load 
			foreach (IConfigurationParameter param in this.CompanyScopeNode.GetCompanyConfigurator().ListCompanyProperties(companyInfo.Id))
			{
				AddCompanySettingNode(param.Name, param.Value, 18);
			}
		}

		/// <summary>
		/// Adds the company setting node.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		private void AddCompanySettingNode(string key, string value)
		{
			AddCompanySettingNode(key, value, 17);
		}

		/// <summary>
		/// Adds the result node.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		private void AddCompanySettingNode(string key, string value, int imageIndex)
		{
			ResultNode node = new ResultNode();
			node.DisplayName = key;
			node.ImageIndex = imageIndex;
			node.SubItemDisplayNames.Add(value??"");

			this.ResultNodes.Add(node);
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
			//    WritableSharedData writableSharedData = new WritableSharedData();

			//    WritableSharedDataItem writableSharedDataItem = new WritableSharedDataItem(DataFormats.UnicodeText, true);
			//    writableSharedDataItem.SetData(Encoding.Unicode.GetBytes("asdasdas asd adasd asdas das d"));
			//    writableSharedData.Add(writableSharedDataItem);

			//    WritableSharedDataItem writableSharedDataItem2 = new WritableSharedDataItem(DataFormats.Text, true);
			//    writableSharedDataItem2.SetData(Encoding.ASCII.GetBytes("asdasdas asd adasd asdas das d"));
			//    writableSharedData.Add(writableSharedDataItem2);

				//WritableSharedDataItem writableSharedDataItem3 = new WritableSharedDataItem(DataFormats.Html, false);
				//writableSharedDataItem3.SetData(Encoding.ASCII.GetBytes("asdasdas asd adasd asdas das d"));
				//writableSharedData.Add(writableSharedDataItem3);

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

		protected override bool OnPaste(SharedData data, DragAndDropVerb pasteType, SyncStatus status)
		{
			//data.Add(new SharedDataItem(DataFormats.UnicodeText));
			//data.Add(new SharedDataItem(DataFormats.Text));
			//data.Add(new SharedDataItem(DataFormats.Html));

			////string displayName1 = Encoding.Unicode.GetString(data.GetItem(DataFormats.UnicodeText).GetData());
			////string displayName2 = Encoding.ASCII.GetString(data.GetItem(DataFormats.Text).GetData());
			//string displayName3 = Encoding.ASCII.GetString(data.GetItem(DataFormats.Html).GetData());


			return base.OnPaste(data, pasteType, status);
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

		public void Refresh()
		{
			this.ResultNodes.Clear();

			LoadCompanySettings();
		}
		#endregion
	}
}

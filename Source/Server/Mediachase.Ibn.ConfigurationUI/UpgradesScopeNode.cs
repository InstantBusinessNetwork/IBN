using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;
using System.Windows.Forms;
using Mediachase.Ibn.ConfigurationUI.Updates;
using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents upgrades scope node.
	/// </summary>
	public class UpgradesScopeNode : ConfigurationScopeNode
	{
		#region Const
		#endregion

		#region Properties
		protected SyncAction UpdateCheckAction { get; set; }
		protected IConfigurator Configurator { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="UpgradesScopeNode"/> class.
		/// </summary>
		public UpgradesScopeNode(IConfigurator configurator)
		{
			this.Configurator = configurator;

			this.DataElementType = DataElementType.Upgrades;

			this.DisplayName = SnapInResources.ScopeNodeDispayName_Upgrades;
			this.DataElementType = DataElementType.Upgrades;
			this.Tag = string.Empty;
			this.ImageIndex = 6;
			this.SelectedImageIndex = 6;

			this.UpdateCheckAction = new SyncAction(SnapInResources.UpgradesScopeNode_Action_UpdateCheck_Name,
				SnapInResources.UpgradesScopeNode_Action_UpdateCheck_Description, 6);

#if RADIUS
			this.UpdateCheckAction.Enabled = false;
			this.UpdateCheckAction.ImageIndex = 30;
#endif

			this.ActionsPaneItems.Add(this.UpdateCheckAction); 
		}
		#endregion

		#region Methods
		protected override void OnSyncAction(SyncAction action, SyncStatus status)
		{
			if (action == this.UpdateCheckAction)
			{
				try
				{
					OnCheckForUpdate(status);
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
		}

		private void OnCheckForUpdate(SyncStatus status)
		{
			UpdateCheckForm form = new UpdateCheckForm(this.Configurator);
			this.SnapIn.Console.ShowDialog(form);
		}
		#endregion

		
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;
using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents License Scope Node.
	/// </summary>
	public class LicenseScopeNode : ConfigurationScopeNode
	{
		#region Const
		#endregion

		#region Properties
		public IConfigurator Configurator { get; private set; }

		public bool IsLoaded { get; set; }

		public LicenseListView LicenseListView { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="LicenseScopeNode"/> class.
		/// </summary>
		/// <param name="configurator">The configurator.</param>
		public LicenseScopeNode(IConfigurator configurator):base(true)
		{
			this.Configurator = configurator;

			// Init View Params and Tag
			this.DataElementType = DataElementType.License;
			this.DisplayName = SnapInResources.ScopeNodeDispayName_License;
			this.Tag = string.Empty;
			this.ImageIndex = 20;
			this.SelectedImageIndex = 20;

			// Init Right Panel
			// Create a message view for the root node.
			MmcListViewDescription lvd = new MmcListViewDescription();
			lvd.DisplayName = SnapInResources.ScopeNodeDispayName_License;
			lvd.ViewType = typeof(LicenseListView);
			//lvd.Options = MmcListViewOptions.ExcludeScopeNodes;

			this.ViewDescriptions.Add(lvd);
			this.ViewDescriptions.DefaultIndex = 0;

			// Init Actions
			this.EnabledStandardVerbs = StandardVerbs.Refresh;
		}
		#endregion

		#region Methods
		protected override void OnRefresh(AsyncStatus status)
		{
			base.OnRefresh(status);

			try
			{
				// Refresh CompanySettingsListView
				if (LicenseListView != null)
					LicenseListView.Refresh();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex, "LicenseScopeNode.Refresh");
			}

		}
		#endregion

		
	}
}

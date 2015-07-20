using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;
using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents ServerProperties Scope Node.
	/// </summary>
	public class ServerPropertiesScopeNode : ConfigurationScopeNode
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the configurator.
		/// </summary>
		/// <value>The configurator.</value>
		public IConfigurator Configurator { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is loaded.
		/// </summary>
		/// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
		public bool IsLoaded { get; set; }

		/// <summary>
		/// Gets or sets the server properties list view.
		/// </summary>
		/// <value>The server properties list view.</value>
		public ServerPropertiesListView ServerPropertiesListView { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ServerPropertiesScopeNode"/> class.
		/// </summary>
		/// <param name="configurator">The configurator.</param>
		public ServerPropertiesScopeNode(IConfigurator configurator):base(true)
		{
			this.Configurator = configurator;

			// Init View Params and Tag
			this.DataElementType = DataElementType.ServerProperties;
			this.DisplayName = SnapInResources.ScopeNodeDispayName_ServerProperties;
			this.Tag = string.Empty;
			this.ImageIndex = 21;
			this.SelectedImageIndex = 21;

			// Init Right Panel
			// Create a message view for the root node.
			MmcListViewDescription lvd = new MmcListViewDescription();
			lvd.DisplayName = SnapInResources.ScopeNodeDispayName_License;
			lvd.ViewType = typeof(ServerPropertiesListView);
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
				if (ServerPropertiesListView != null)
					ServerPropertiesListView.Refresh();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex, "ServerProperties.Refresh");
			}

		}
		#endregion

		
	}
}

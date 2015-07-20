using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;
using Mediachase.Ibn.Configuration;
using System.Data;
using System.Windows.Forms;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents Server Scope Node.
	/// </summary>
	public class ServerScopeNode : ConfigurationScopeNode
	{
		#region Const
		#endregion

		#region Properties
		public bool IsLoaded { get; set; }
		public IConfigurator Configurator { get; private set; }
		public ServerFormView ServerFormView { get; set; }
		
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ServerScopeNode"/> class.
		/// </summary>
		public ServerScopeNode(IConfigurator configurator)
		{
			this.EnabledStandardVerbs = StandardVerbs.Refresh;
			this.Configurator = configurator;

			// Initialize View ViewDescriptions
			// Create a form view for the root node.
			FormViewDescription fvd = new FormViewDescription();
			fvd.DisplayName = "(localhost) Home";
			fvd.ViewType = typeof(ServerFormView);
			fvd.ControlType = typeof(ServerFeaturesControl);

			// Attach the view to the root node.
			this.ViewDescriptions.Add(fvd);
			this.ViewDescriptions.DefaultIndex = 0;

		}
		#endregion

		#region Methods
		/// <summary>
		/// Called when a scope node is expanded. This method allows the derived classes to provide custom behavior.
		/// </summary>
		/// <param name="status">The object that holds the status information.</param>
		protected override void OnExpand(AsyncStatus status)
		{
			if (!IsLoaded)
			{
				Refresh();
				this.IsLoaded = true;
			}
			
			base.OnExpand(status);
		}

		/// <summary>
		/// This method is called when the node has to be expanded synchronously with respect to MMC.  MMC expands nodes to select the last node that was selected when the console file was saved.
		/// </summary>
		/// <param name="status">The object that holds the status information.</param>
		/// <returns>
		/// True indicates that the node was expanded completely; otherwise, false.
		/// </returns>
		protected override bool OnExpandFromLoad(SyncStatus status)
		{
			if (!IsLoaded)
			{
				Refresh();
				this.IsLoaded = true;
			}

			return base.OnExpandFromLoad(status);
		}

		/// <summary>
		/// Called when the standard verb <see cref="F:Microsoft.ManagementConsole.StandardVerbs.Refresh"></see> is triggered.
		/// </summary>
		/// <param name="status">The object  that holds the status information.</param>
		protected override void OnRefresh(AsyncStatus status)
		{
			Refresh();
			
			base.OnRefresh(status);
		}

		/// <summary>
		/// Refreshes this instance.
		/// </summary>
		internal void Refresh()
		{
			string serverElementId = (string)this.Tag;

			// Refresh View
			if (this.ServerFormView != null &&
				this.ServerFormView.ServerFeaturesControl != null)
			{
				this.ServerFormView.ServerFeaturesControl.LoadDataFromConfigurator();
			}

			// LoadServerChildScopeNodes
			this.Children.Clear();

			LoadServerChildScopeNodes(this);
		}

		/// <summary>
		/// Loads the server child scope nodes.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		/// <param name="serverScopeNode">The server scope node.</param>
		private void LoadServerChildScopeNodes(ScopeNode serverScopeNode)
		{
			// Companies
			CompaniesScopeNode companiesScopeNode = new CompaniesScopeNode(this.Configurator);
			serverScopeNode.Children.Add(companiesScopeNode);

			// License
			LicenseScopeNode licenseScopeNode = new LicenseScopeNode(this.Configurator);
			serverScopeNode.Children.Add(licenseScopeNode);

			// Updates
			UpgradesScopeNode upgradesScopeNode = new UpgradesScopeNode(this.Configurator);
			serverScopeNode.Children.Add(upgradesScopeNode);


			foreach (int version in GetUpgradeElements(this.Configurator))
			{
				UpgradeScopeNode upgradeScopeNode = new UpgradeScopeNode(this.Configurator, version);
				upgradesScopeNode.Children.Add(upgradeScopeNode);
			}

			// Properties
			ServerPropertiesScopeNode serverPropertiesScopeNode = new ServerPropertiesScopeNode(this.Configurator);
			serverScopeNode.Children.Add(serverPropertiesScopeNode);

		}



		/// <summary>
		/// Gets the upgrades name list.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		/// <returns></returns>
		private int[] GetUpgradeElements(IConfigurator serverConfigurator)
		{
			return serverConfigurator.ListUpdates();
		}

		#endregion

		
	}
}

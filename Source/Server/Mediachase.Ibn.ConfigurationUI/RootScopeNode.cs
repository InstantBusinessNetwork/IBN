using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;
using Mediachase.Ibn.Configuration;
using System.Net;
using System.Security.Principal;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents root scope node.
	/// </summary>
	public class RootScopeNode : ConfigurationScopeNode
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="RootScopeNode"/> class.
		/// </summary>
		public RootScopeNode()
		{
			// Init View Params and Tag
			this.DataElementType = DataElementType.Companies;
			this.DisplayName = IbnConst.ProductFamily;//SnapInResources.ScopeNodeDispayName_Root;
			this.Tag = string.Empty;
			this.ImageIndex = 1;
			this.SelectedImageIndex = 1;

			// Init Right Panel
			// Create a message view for the root node.
			MmcListViewDescription lvd = new MmcListViewDescription();
			lvd.DisplayName = IbnConst.ProductFamily;//SnapInResources.ScopeNodeDispayName_Root;

			lvd.ViewType = typeof(ServerListView);
			//lvd.Options = MmcListViewOptions.ExcludeScopeNodes;

			this.ViewDescriptions.Add(lvd);
			this.ViewDescriptions.DefaultIndex = 0;

			// Init Actions
			this.EnabledStandardVerbs = StandardVerbs.Refresh;
		}
		#endregion

		#region Properties
		public bool IsLoaded { get; set; }
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

		private void Refresh()
		{
			// Remove Old Child Nodes
			this.Children.Clear();

			// TODO: Read Server List

			// Create Server Scope Node, Load IConfigurator and invoke LoadServerChildScopeNodes
			ServerScopeNode serverScopeNode = new ServerScopeNode(Configurator.Create());
			this.Children.Add(serverScopeNode);

			// 
			DataElement serverElement = new DataElement(DataElementType.Server,
					"DB44AFD2-CC7B-4ee1-9C81-64422D7BA76C",
					Dns.GetHostName().ToUpperInvariant() + " (" + WindowsIdentity.GetCurrent().Name + ")");

			serverScopeNode.DataElementType = DataElementType.Server;
			serverScopeNode.Tag = serverElement.Id;
			serverScopeNode.DisplayName = serverElement.DisplayName;
			serverScopeNode.ImageIndex = 2;
			serverScopeNode.SelectedImageIndex = 2;

			// TODO: Read Server Information and Init SubItemDisplayNames
			// IP
			serverScopeNode.SubItemDisplayNames.Add("127.0.0.1");
		}
		#endregion

		
	}
}

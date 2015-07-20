using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Configuration;
using Microsoft.ManagementConsole;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents upgrade scope node.
	/// </summary>
	public class UpgradeScopeNode: ConfigurationScopeNode
	{
		#region Const
		#endregion

		#region Properties
		public IConfigurator Configurator { get; set; }
		public int Version { get; set; }
		public UpdateFormView UpdateFormView { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="UpgradeScopeNode"/> class.
		/// </summary>
		public UpgradeScopeNode(IConfigurator configurator, int version):base(true)
		{
			this.DataElementType = DataElementType.Upgrade;
			this.Configurator = configurator;
			this.Version = version;

			this.DisplayName = version.ToString();
			this.Tag = version;

			this.ImageIndex = 7;
			this.SelectedImageIndex = 7;

			// Initialize View ViewDescriptions
			// Create a form view for the root node.
			FormViewDescription fvd = new FormViewDescription();
			fvd.DisplayName = string.Format(SnapInResources.UpdateFormView_FrindlyName_Format, version);
			fvd.ViewType = typeof(UpdateFormView);
			fvd.ControlType = typeof(UpdateInfoControl);

			// Attach the view to the root node.
			this.ViewDescriptions.Add(fvd);
			this.ViewDescriptions.DefaultIndex = 0;
		}
		#endregion

		#region Methods
		#endregion
		
	}
}

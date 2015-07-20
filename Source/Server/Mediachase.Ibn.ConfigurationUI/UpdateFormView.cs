using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;
using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents Update Form View.
	/// </summary>
	public class UpdateFormView : FormView
	{
		#region Const
		#endregion

		#region Properties

		public UpdateInfoControl UpdateInfoControl { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateFormView"/> class.
		/// </summary>
		public UpdateFormView()
		{
		}
		#endregion

		#region Methods
		protected override void OnInitialize(AsyncStatus status)
		{
			// handle any basic stuff
			base.OnInitialize(status);

			this.UpdateInfoControl = (UpdateInfoControl)this.Control;
			((UpgradeScopeNode)this.ScopeNode).UpdateFormView = this;
		}
		#endregion
	}
}

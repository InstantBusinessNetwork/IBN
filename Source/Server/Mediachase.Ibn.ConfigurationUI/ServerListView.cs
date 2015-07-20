using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents server list view.
	/// </summary>
	public class ServerListView : MmcListView
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ServerListView"/> class.
		/// </summary>
		public ServerListView()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		protected override void OnInitialize(AsyncStatus status)
		{
			base.OnInitialize(status);

			// Create a set of columns for use in the list view
			// Define the default column title
			this.Columns[0].Title = SnapInResources.ServerListView_Column_Name;
			this.Columns[0].SetWidth(200);

			// Add detail column
			this.Columns.Add(new MmcListViewColumn(SnapInResources.ServerListView_Column_IP, 100));

			// Set to show all columns
			this.Mode = MmcListViewMode.Report;  // default (set for clarity)

			// Set to show refresh as an option
		}
		#endregion
	}
}

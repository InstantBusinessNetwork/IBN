using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;
using Mediachase.Ibn.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading;
using Mediachase.Ibn.ConfigurationUI.Updates;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents ibn configuration manger mmc-snapin.
	/// </summary>
	[SnapInSettings("{D8F2A8F5-658E-4FAC-B51C-86B9BAC84A0B}",
	 DisplayName = IbnConst.ProductFamily,
	 Description = IbnConst.ProductFamily + " Portals Configuration Manager provides basic configuration management for " + IbnConst.ProductFamilyShort + " portals.",
	 Vendor = IbnConst.CompanyName)]
	public class IbnConfigurationMangerSnapIn: SnapIn
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="IbnConfigurationMangerSnapIn"/> class.
		/// </summary>
		public IbnConfigurationMangerSnapIn()
		{
#if DEBUG
			// TURN ON RUSSIAN LANGUAGE
			//Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU", false);
			//Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU", false);

#endif
			//
			//SafeIbnConfig file1 = new SafeIbnConfig(@"D:\Program Files\Mediachase\IBN Server 4.8\ibn.config", System.IO.FileAccess.Read);
			//SafeIbnConfig file2 = new SafeIbnConfig(@"D:\Program Files\Mediachase\IBN Server 4.8\ibn.config", System.IO.FileAccess.Read);

			//SafeIbnConfig file3 = new SafeIbnConfig(@"D:\Program Files\Mediachase\IBN Server 4.8\ibn.config", System.IO.FileAccess.ReadWrite);

			//SafeIbnConfig file4 = new SafeIbnConfig(@"D:\Program Files\Mediachase\IBN Server 4.8\ibn.config", System.IO.FileAccess.Read);

			// Assign Large and Small Icon 
			LoadImageLists();

			// Create Root
			this.RootNode = new RootScopeNode();
		}

		#endregion

		#region Properties
		#endregion

		#region Methods
		/// <summary>
		/// Loads the image lists.
		/// </summary>
		private void LoadImageLists()
		{
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_Empty, Color.Magenta); // 0
#if RADIUS
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_RootRS, Color.Magenta); // 1
#else
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_Root, Color.Magenta); // 1
#endif
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_Server, Color.Magenta); // 2
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_Companies, Color.Magenta); // 3
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_Company, Color.Magenta); // 4
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_DomainAliases, Color.Magenta); // 5
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_Upgrades, Color.Magenta); // 6
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_Upgrade, Color.Magenta); // 7
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_NewCompany, Color.Magenta); // 8

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CompanyDelete, Color.Magenta); // 9

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CompanyStart, Color.Magenta); // 10
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CompanyStop, Color.Magenta); // 11

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CompanyChangeDomain, Color.Magenta); // 12

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CompanyStartDisable, Color.Magenta); // 13
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CompanyStopDisable, Color.Magenta); // 14
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CompanyInactive, Color.Magenta); // 15

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_Browse, Color.Magenta); // 16

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CompanyInfoSetting, Color.Magenta); // 17
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CompanyInfoSetting2, Color.Magenta); // 18
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CopyToClipboard, Color.Magenta); // 19

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_License, Color.Magenta); // 20

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_ServerProperties, Color.Magenta); // 21

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_Asp, Color.Magenta); // 22
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_AspCreate, Color.Magenta); // 23
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_AspDelete, Color.Magenta); // 24

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CompanyWarning, Color.Magenta); // 25
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CompanyInactiveWarning, Color.Magenta); // 26

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_SchedulerStart, Color.Magenta); // 27
			this.SmallImages.Add(SnapInResources.ScopeNodeImage_SchedulerStop, Color.Magenta); // 28

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_CompanyPoolEdit, Color.Magenta); // 29

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_UpgradesDisable, Color.Magenta); // 30

			this.SmallImages.Add(SnapInResources.ScopeNodeImage_NewCompanyForDatabase, Color.Magenta); // 31
		}

		#endregion

		
	}
}

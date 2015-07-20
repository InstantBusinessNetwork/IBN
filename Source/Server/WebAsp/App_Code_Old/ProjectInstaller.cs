using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace Mediachase.Ibn.WebAsp.App_Code_Old
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();
		}
	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Mediachase.Ibn.ConfigurationUI
{
	public partial class LogoControl : UserControl
	{
		public LogoControl()
		{
			InitializeComponent();

#if RADIUS
			this.pictureBoxLogo.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ServerFormViewImage_LogoRS;
#else
			this.pictureBoxLogo.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ServerFormViewImage_Logo;
#endif
		}
	}
}

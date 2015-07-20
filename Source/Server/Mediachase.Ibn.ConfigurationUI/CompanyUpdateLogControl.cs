using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Mediachase.Ibn.ConfigurationUI
{
	public partial class CompanyUpdateLogControl : UserControl
	{
		protected string Log { get; set; }

		protected string FileName { get; set; }

		public CompanyUpdateLogControl()
		{
			InitializeComponent();
		}

		public void Init(string companyName, string companyId, string log)
		{
			labelCompanyName.Text = companyName;
			labelCompanyId.Text = companyId;
			this.Log = log;

		}

		private void linkDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (string.IsNullOrEmpty(FileName))
			{
				// Save Log to Temporary File Storage and
				FileName = Path.GetTempFileName() + ".txt";

				using (StreamWriter writer = new StreamWriter(FileName))
				{
					writer.Write(this.Log);
					writer.Flush();
					writer.Close();
				}
			}

			Process.Start(FileName);
		}
	}
}

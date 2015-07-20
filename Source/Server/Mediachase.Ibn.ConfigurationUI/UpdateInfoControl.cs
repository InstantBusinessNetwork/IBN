using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.ManagementConsole;
using Mediachase.Ibn.Configuration;
using System.IO;
using System.Web;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents update info control.
	/// </summary>
	public partial class UpdateInfoControl : UserControl, IFormViewControl
	{
		public UpdateFormView ParentFormView { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateInfoControl"/> class.
		/// </summary>
		public UpdateInfoControl()
		{
			InitializeComponent();
			this.Dock = DockStyle.Fill;
		}


		/// <summary>
		/// Uses the associated Windows Forms view to initialize the control that implements the interface.
		/// </summary>
		/// <param name="view">The associated <see cref="T:Microsoft.ManagementConsole.FormView"></see> value.</param>
		void IFormViewControl.Initialize(FormView view)
		{
			this.ParentFormView = (UpdateFormView)view;

			InitializeUpdateInfoBlock();
		}


		/// <summary>
		/// Gets the update info.
		/// </summary>
		/// <returns></returns>
		protected IUpdateInfo[] GetUpdateInfo()
		{
			UpgradeScopeNode scopeNode = ((UpgradeScopeNode)this.ParentFormView.ScopeNode);
			return scopeNode.Configurator.GetUpdateInfo(scopeNode.Version);
		}

		/// <summary>
		/// Initializes the update info block.
		/// </summary>
		private void InitializeUpdateInfoBlock()
		{
			IUpdateInfo[] infoList = GetUpdateInfo();

			if (infoList.Length > 0)
			{
				textBoxVersion.Text = infoList[0].Version.ToString();
				textBoxReleaseDate.Text = infoList[0].ReleaseDate.ToShortDateString();

				if (infoList[0].RequiresCommonComponentsUpdate || infoList[0].UpdatesCommonComponents)
				{
					textBoxRestrictions.Text = string.Empty;

					if (infoList[0].RequiresCommonComponentsUpdate)
					{
						textBoxRestrictions.Text += SnapInResources.UpdateInfo_Restrictions_UpdatesCommonComponents;
						if (infoList[0].UpdatesCommonComponents)
							textBoxRestrictions.Text += ", ";
					}

					if (infoList[0].UpdatesCommonComponents)
					{
						textBoxRestrictions.Text += SnapInResources.UpdateInfo_Restrictions_UpdatesCommonComponents;
					}
				}
				else
					textBoxRestrictions.Text = SnapInResources.UpdateInfo_Restrictions_None;
			}

			// Create Information list
			StringBuilder infoHtml = new StringBuilder();

			infoHtml.AppendFormat(@"<html>{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"<head>{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"	<style type=""text/css"">{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		.styleHeader{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		{{{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			color: #000000;{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			font-family: Arial;{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			font-size: 8pt;{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		}}{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		.styleChanges{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		{{{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			font-family: Arial;{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			font-size: 8pt;{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		}}{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		.styleWarnings{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		{{{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			font-family: Arial;{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			font-size: 8pt;{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		}}{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		.styleWarningsHeader{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		{{{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			color: #FF5050;{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			font-family: Arial;{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			font-size: 8pt;			{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		}}{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		.styleChangesHeader{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		{{{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			color: #00CC00;{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			font-family: Arial;{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"			font-size: 8pt;			{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"		}}{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"	</style>{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"</head>{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"<body>{0}", Environment.NewLine);

			//bool bFirstItem = true;

			foreach (IUpdateInfo info in infoList)
			{
				infoHtml.AppendFormat(@"	<span class=""styleHeader"">{0}", Environment.NewLine);
				infoHtml.AppendFormat(@"	<b>{3}</b>{1} ({2}){0}", Environment.NewLine, info.Version, info.ReleaseDate.ToShortDateString(), SnapInResources.UpdateInfo_VersionHtmlField);
				infoHtml.AppendFormat(@"	<br/>{0}", Environment.NewLine);

				#region Render Restrictions Section
				if (info.RequiresCommonComponentsUpdate || info.UpdatesCommonComponents)
				{
					infoHtml.AppendFormat(@"	<b>{0}</b>", SnapInResources.UpdateInfo_RestrictionsHtmlField);
					if (info.RequiresCommonComponentsUpdate)
					{
						infoHtml.AppendFormat(SnapInResources.UpdateInfo_Restrictions_UpdatesCommonComponents);
						if (info.UpdatesCommonComponents)
							infoHtml.AppendFormat(@", ");
					}

					if (info.UpdatesCommonComponents)
					{
						infoHtml.AppendFormat(SnapInResources.UpdateInfo_Restrictions_RequiresCommonComponentsUpdate);
					}
					infoHtml.AppendLine();
				} 
				#endregion

				infoHtml.AppendFormat(@"	</span>{0}", Environment.NewLine);
				infoHtml.AppendFormat(@"	<br/>{0}", Environment.NewLine);

				#region Render Warnings Section
				if (!string.IsNullOrEmpty(info.Warnings))
				{
					infoHtml.AppendFormat(@"	<span class=""styleWarningsHeader"">{0}", Environment.NewLine);
					infoHtml.AppendFormat(@"	<b>{1}</b>{0}", Environment.NewLine, SnapInResources.UpdateInfo_WarningsHtmlField);
					infoHtml.AppendFormat(@"	</span>{0}", Environment.NewLine);
					infoHtml.AppendFormat(@"	<div style=""width=100%;"">{0}", Environment.NewLine);


					StringReader reader = new StringReader(info.Warnings);

					string warningLine = reader.ReadLine();
					while (warningLine != null)
					{
						warningLine = warningLine.Trim();

						if (!string.IsNullOrEmpty(warningLine))
							infoHtml.AppendFormat(@"		<span class=""styleWarnings"">{1}</span><br/>{0}", Environment.NewLine, HttpUtility.HtmlEncode(warningLine));

						warningLine = reader.ReadLine();
					}

					infoHtml.AppendFormat(@"	</div>{0}", Environment.NewLine);
					infoHtml.AppendFormat(@"	<br/>	{0}", Environment.NewLine);
				} 
				#endregion

				#region Render Changes Section
				if (!string.IsNullOrEmpty(info.Changes))
				{
					infoHtml.AppendFormat(@"	<span class=""styleChangesHeader"">{0}", Environment.NewLine);
					infoHtml.AppendFormat(@"	<b>{1}</b>{0}", Environment.NewLine, SnapInResources.UpdateInfo_ChangesHtmlField);
					infoHtml.AppendFormat(@"	</span>{0}", Environment.NewLine);

					infoHtml.AppendFormat(@"	<div style=""width=100%;"">{0}", Environment.NewLine);

					StringReader reader = new StringReader(info.Changes);

					string changesLine = reader.ReadLine();
					while (changesLine != null)
					{
						changesLine = changesLine.Trim();

						if (!string.IsNullOrEmpty(changesLine))
							infoHtml.AppendFormat(@"		<span class=""styleChanges"">{1}</span><br/>{0}", Environment.NewLine, changesLine);

						changesLine = reader.ReadLine();
					}

					infoHtml.AppendFormat(@"	</div>{0}", Environment.NewLine);
					infoHtml.AppendFormat(@"	<span class=""styleHeader"">{0}", Environment.NewLine);
					infoHtml.AppendFormat(@"	<hr />{0}", Environment.NewLine);
				} 
				#endregion


				//bFirstItem = false;
			}

			infoHtml.AppendFormat(@"</body>{0}", Environment.NewLine);
			infoHtml.AppendFormat(@"</html>{0}", Environment.NewLine);

			webBrowserUpdateInfo.DocumentText = infoHtml.ToString();
		}
	}
}

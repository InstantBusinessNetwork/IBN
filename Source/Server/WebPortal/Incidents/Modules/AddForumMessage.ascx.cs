namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Collections;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.FileUploader.Web;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for AddForumMessage.
	/// </summary>
	public partial class AddForumMessage : System.Web.UI.UserControl, IPageTemplateTitle
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(AddForumMessage).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentEdit", typeof(AddForumMessage).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strAddDoc", typeof(AddForumMessage).Assembly);

		#region IncidentId
		protected int IncidentId
		{
			get
			{
				if (Request["IncidentId"] != null)
					return int.Parse(Request["IncidentId"]);
				else
					return -1;
			}
		}
		#endregion

		#region AddGuid
		protected string AddGuid
		{
			get
			{
				string sGuid = "";
				if (Request.QueryString["guid"] != null)
					sGuid += "&guid=" + Request.QueryString["guid"];
				if (Request.QueryString["send"] != null)
					sGuid += "&send=" + Request.QueryString["send"]; ;
				return sGuid;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
				BindToolbar();
			imbSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			imbCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");
			Ajax.Utility.RegisterTypeForAjax(typeof(AddForumMessage));
		}

		#region BindToolbar
		private void BindToolbar()
		{
			imbSave.Text = LocRM.GetString("tSend");
			imbCancel.Text = LocRM2.GetString("tbSaveCancel");
			secHeader.Title = LocRM.GetString("tbAddMess");
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region Ajax
		[Ajax.AjaxMethod()]
		public ArrayList GetProgressInfo(string FormId)
		{
			ArrayList values = new ArrayList();

			Guid progressUid = new Guid(FormId);
			UploadProgressInfo upi = UploadProgress.Provider.GetInfo(progressUid);

			if (upi == null)
			{
				values.Add("-1");
				values.Add(LocRM3.GetString("tWaitForUploading"));
			}
			else
			{
				if (upi.Result == UploadResult.Succeeded)
				{
					if (upi.BytesTotal != upi.BytesReceived)
					{
						values.Add("-2");
						values.Add(LocRM3.GetString("tUploadFailed"));
					}
					else
					{
						values.Add("-3");
						values.Add(LocRM3.GetString("tUploadSuccess"));
					}
				}
				else
				{
					// 0
					values.Add(Util.CommonHelper.ByteSizeToStr(upi.BytesReceived));
					// 1
					values.Add(Util.CommonHelper.ByteSizeToStr(upi.BytesTotal));
					// 2
					values.Add(upi.EstimatedTime.ToString().Substring(0, 8));
					// 3
					values.Add((upi.TimeRemaining.ToString().Substring(0, 8)).StartsWith("-") ? "00:00:00" : upi.TimeRemaining.ToString().Substring(0, 8));

					// 4
					int percents = (int)((float)upi.BytesReceived / (float)upi.BytesTotal * 100);
					values.Add(percents.ToString());

					// 5
					string sFName = upi.CurrentFileName;
					if (sFName.LastIndexOf("\\") >= 0)
						sFName = sFName.Substring(sFName.LastIndexOf("\\") + 1);
					values.Add(LocRM3.GetString("tInProgress") + " " + sFName);
				}
			}
			return values;
		}
		#endregion

		#region IPageTemplateTitle Members
		public string Modify(string oldValue)
		{
			return LocRM.GetString("tbAddMess");
		}
		#endregion
	}
}

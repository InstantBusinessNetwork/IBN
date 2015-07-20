namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.Ibn.Assignments;
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Core.Business;
	using Mediachase.Ibn.Data;

	/// <summary>
	///		Summary description for Unsubscribe.
	/// </summary>
	public partial class Unsubscribe : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strPageTitles", typeof(Unsubscribe).Assembly);

		#region EventTypeId
		private int EventTypeId
		{
			get
			{
				return int.Parse(Request["EventTypeId"]);
			}
		}
		#endregion

		#region ObjectId
		private int? ObjectId
		{
			get
			{
				int? retval = null;
				if (!String.IsNullOrEmpty(Request["ObjectId"]))
					retval = int.Parse(Request["ObjectId"], CultureInfo.InvariantCulture);
				return retval;
			}
		}
		#endregion

		#region ObjectUid
		private Guid? ObjectUid
		{
			get
			{
				Guid? retval = null;
				if (!String.IsNullOrEmpty(Request["ObjectUid"]))
					retval = new Guid(Request["ObjectUid"]);
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			trResults.Visible = false;
			ApplyLocalization();

			if (!IsPostBack)
				BindValues();
		}

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
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			secHeader.Title = LocRM.GetString("tUnsubscribe");
			btnSave.Text = LocRM.GetString("Submit");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			lnkNotification.Text = LocRM.GetString("NotificationLink");
			lnkNotificationForObject.Text = LocRM.GetString("NotificationForObjectLink");
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			string Title = "";
			int ObjectTypeId = (int)ObjectTypes.UNDEFINED;
			///  EventTypeId, ParentId, ObjectTypeId, RelObjectTypeId, Title, IsActive
			using (IDataReader reader = SystemEvents.GetSystemEventType(EventTypeId))
			{
				if (reader.Read())
				{
					Title = reader["Title"].ToString();
					ObjectTypeId = (int)reader["ObjectTypeId"];
				}
			}

			lblNotification.Text = SystemEvents.GetSystemEventName(Title);

			lblObjectType.Text = Util.CommonHelper.GetObjectTypeName(ObjectTypeId);
			if (ObjectId.HasValue)
			{
				lblObject.Text = Util.CommonHelper.GetObjectLinkAndTitle(ObjectTypeId, ObjectId.Value);
			}
			else if (ObjectUid.HasValue && ObjectTypeId == (int)ObjectTypes.Assignment)
			{
				AssignmentEntity entity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, (PrimaryKeyId)ObjectUid.Value);
				if (entity != null && entity.OwnerDocumentId.HasValue)
				{
					string link = Mediachase.Ibn.Web.UI.CHelper.GetAssignmentLink(ObjectUid.Value.ToString(), (int)ObjectTypes.Document, entity.OwnerDocumentId.Value, this.Page);
					lblObject.Text = Util.CommonHelper.GetHtmlLink(link, entity.Subject);
				}
			}

			rblType.Items.Add(new ListItem(LocRM.GetString("Unsubscribe"), "1"));
			if (ObjectId.HasValue)
				rblType.Items.Add(new ListItem(LocRM.GetString("UnsubscribeForObject"), "2"));
			rblType.Items[0].Selected = true;

			lnkNotification.NavigateUrl = String.Format("~/Directory/UserView.aspx?UserId={0}&Tab=3", Security.CurrentUser.UserID);

			if (ObjectId.HasValue)
				lnkNotificationForObject.NavigateUrl = String.Format("~/Directory/SystemNotificationForObject.aspx?ObjectId={0}&ObjectTypeId={1}", ObjectId.Value, ObjectTypeId);
			else
				lnkNotificationForObject.Visible = false;
		}
		#endregion

		#region btnSave_ServerClick
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			if (rblType.SelectedValue == "1")
				SystemEvents.UnsubscribePersonal(EventTypeId);
			else if (ObjectId.HasValue)
				SystemEvents.UnsubscribePersonalForObject(EventTypeId, ObjectId.Value);

			trResults.Visible = true;
			trUnsubscribe.Visible = false;
		}
		#endregion
	}
}

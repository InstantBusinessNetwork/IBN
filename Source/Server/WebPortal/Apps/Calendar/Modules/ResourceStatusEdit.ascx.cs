using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Events;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Web.UI.Calendar.Modules
{
	public partial class ResourceStatusEdit : MCDataBoundControl
	{
		#region _resId
		private PrimaryKeyId _resId
		{
			get
			{
				if (ViewState["_CurrentResourcsId"] != null)
					return (PrimaryKeyId)ViewState["_CurrentResourcsId"];
				else
					return PrimaryKeyId.Empty;
			}
			set
			{
				ViewState["_CurrentResourcsId"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			btnAccept.ServerClick += new EventHandler(btnAccept_ServerClick);
			btnTentative.ServerClick += new EventHandler(btnTentative_ServerClick);
			btnDecline.ServerClick += new EventHandler(btnDecline_ServerClick);

			BindButtons();
		}

		#region BindButtons
		private void BindButtons()
		{
			btnAccept.CustomImage = this.Page.ResolveUrl("~/layouts/images/accept_green.gif");
			btnTentative.CustomImage = this.Page.ResolveUrl("~/images/IbnFramework/help_vio.png");
			btnDecline.CustomImage = this.Page.ResolveUrl("~/layouts/images/deny.gif");
			btnAccept.Text = GetGlobalResourceObject("IbnFramework.Calendar", "Accept").ToString();
			btnTentative.Text = GetGlobalResourceObject("IbnFramework.Calendar", "Tentative").ToString();
			btnDecline.Text = GetGlobalResourceObject("IbnFramework.Calendar", "Decline").ToString();
			secHeader.Title = GetGlobalResourceObject("IbnFramework.Calendar", "UserStatus").ToString();
		}
		#endregion

		#region DataBind
		public override void DataBind()
		{
			if (DataItem != null)
			{
				CalendarEventEntity ceo = (CalendarEventEntity)DataItem;
				FilterElementCollection fec = new FilterElementCollection();
				FilterElement fe = FilterElement.EqualElement("EventId", ((VirtualEventId)ceo.PrimaryKeyId).RealEventId);
				fec.Add(fe);
				fe = FilterElement.EqualElement("PrincipalId", Mediachase.IBN.Business.Security.CurrentUser.UserID);
				fec.Add(fe);

				EntityObject[] list = BusinessManager.List(CalendarEventResourceEntity.ClassName, fec.ToArray());
				if (list.Length > 0)
				{
					CalendarEventResourceEntity cero = (CalendarEventResourceEntity)list[0];
					_resId = cero.PrimaryKeyId.Value;
					if (cero.Status.HasValue)
					{
						if (cero.Status.Value == (int)eResourceStatus.Accepted)
							btnAccept.Disabled = true;
						if (cero.Status.Value == (int)eResourceStatus.Tentative)
							btnTentative.Disabled = true;
						if (cero.Status.Value == (int)eResourceStatus.Declined)
							btnDecline.Disabled = true;
					}
				}
			}
			base.DataBind();
		}
		#endregion

		#region UpdateStatus
		void btnDecline_ServerClick(object sender, EventArgs e)
		{
			CommonPartStatusUpdate(eResourceStatus.Declined);
		}

		void btnTentative_ServerClick(object sender, EventArgs e)
		{
			CommonPartStatusUpdate(eResourceStatus.Tentative);
		}

		void btnAccept_ServerClick(object sender, EventArgs e)
		{
			CommonPartStatusUpdate(eResourceStatus.Accepted);
		}

		private void CommonPartStatusUpdate(eResourceStatus status)
		{
			CalendarEventResourceEntity cero = BusinessManager.Load(CalendarEventResourceEntity.ClassName, _resId) as CalendarEventResourceEntity;
			if (cero != null)
			{
				Mediachase.Ibn.Events.Request.ChangeTrackingRequest req = new Mediachase.Ibn.Events.Request.ChangeTrackingRequest(cero);
				req.Status = status;
				BusinessManager.Execute(req);
				CHelper.RequireDataBind();
			}
		}
		#endregion

		#region CheckVisibility
		public override bool CheckVisibility(object dataItem)
		{
			if (dataItem != null)
			{
				CalendarEventEntity ceo = (CalendarEventEntity)dataItem;
				FilterElementCollection fec = new FilterElementCollection();
				FilterElement fe = FilterElement.EqualElement("EventId", ((VirtualEventId)ceo.PrimaryKeyId).RealEventId);
				fec.Add(fe);
				fe = FilterElement.EqualElement("PrincipalId", Mediachase.IBN.Business.Security.CurrentUser.UserID);
				fec.Add(fe);

				EntityObject[] list = BusinessManager.List(CalendarEventResourceEntity.ClassName, fec.ToArray());
				if (list.Length == 0)
					return false;
			}
			return base.CheckVisibility(dataItem);
		}
		#endregion
	}
}
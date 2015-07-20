using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Business.Customization;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine
{
	#region class : ColumnInfo
	public class ColumnInfo : IEqualityComparer<ColumnInfo>
	{
		public string Id;
		public int Width;

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj == this)
				return true;

			if (obj is ColumnInfo)
			{
				return String.Equals(((ColumnInfo)obj).Id, this.Id, StringComparison.InvariantCultureIgnoreCase);
			}

			return base.Equals(obj);

		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region .ctor
		public ColumnInfo()
		{
		}

		public ColumnInfo(string id)
			: this()
		{
			this.Id = id;
		}

		public ColumnInfo(string id, int width)
			: this(id)
		{
			this.Width = width;
		}
		#endregion

		#region IEqualityComparer<ColumnInfo> Members

		public bool Equals(ColumnInfo x, ColumnInfo y)
		{
			return String.Equals(x.Id, y.Id);
		}

		public int GetHashCode(ColumnInfo obj)
		{
			return base.GetHashCode();
		}

		#endregion
	}
	#endregion

	#region class : CpInfoItem
	public class CpInfoItem
	{
		public string Id;
		public string Collapsed;
		public string InstanseUid;
	}
	#endregion

	#region class : CpInfo
	public class CpInfo
	{
		public string Id;
		public List<CpInfoItem> Items;
	}
	#endregion

	#region class: LayoutContextKey
	[Serializable]
	public class LayoutContextKey
	{
		#region prop: PageUid
		private Guid pageUid;

		/// <summary>
		/// Gets or sets the page uid.
		/// </summary>
		/// <value>The page uid.</value>
		public Guid PageUid
		{
			get { return pageUid; }
			set { pageUid = value; }
		}
		#endregion

		#region prop: IsAdmin
		private bool isAdmin;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is admin.
		/// </summary>
		/// <value><c>true</c> if this instance is admin; otherwise, <c>false</c>.</value>
		public bool IsAdmin
		{
			get { return isAdmin; }
			set { isAdmin = value; }
		}
		#endregion

		#region prop: ProfileId
		private int? profileId = null;

		public int? ProfileId
		{
			get { return profileId; }
			set { profileId = value; }
		}
		#endregion

		#region prop: UserId
		private int? userId = null;

		public int? UserId
		{
			get { return userId; }
			set { userId = value; }
		}
		#endregion

		#region .ctor
		public LayoutContextKey()
		{
		}

		public LayoutContextKey(Guid pageUid)
			: this()
		{
			this.PageUid = pageUid;
		}

		public LayoutContextKey(Guid pageUid, bool isAdmin)
			: this(pageUid)
		{
			this.IsAdmin = isAdmin;
		}

		public LayoutContextKey(Guid pageUid, bool isAdmin, int? profileId, int? userId)
			: this(pageUid, isAdmin)
		{
			this.ProfileId = profileId;
			this.UserId = userId;
		}
		#endregion
	}
	#endregion

	public class IbnControlPlaceManager : CompositeDataBoundControl
	{
		#region GetCurrent
		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <returns></returns>
		public static IbnControlPlaceManager GetCurrent(Page _page)
		{
			IbnControlPlaceManager retVal = null;
			retVal = GetActionManagerFromCollection(_page.Controls);
			return retVal;
		}

		private static IbnControlPlaceManager GetActionManagerFromCollection(ControlCollection coll)
		{
			IbnControlPlaceManager retVal = null;
			foreach (Control c in coll)
			{
				if (c is IbnControlPlaceManager)
				{
					retVal = (IbnControlPlaceManager)c;
					break;

				}
				else
				{
					retVal = GetActionManagerFromCollection(c.Controls);
					if (retVal != null)
						break;
				}
			}
			return retVal;
		}
		#endregion

		public IbnControlPlaceManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region prop: PageUid
		/// <summary>
		/// Gets or sets the page uid.
		/// </summary>
		/// <value>The page uid.</value>
		public Guid PageUid
		{
			get
			{
				if (ViewState["_PageUid"] != null)
					return (Guid)ViewState["_PageUid"];

				return Guid.Empty;
			}
			set
			{
				ViewState["_PageUid"] = value;
			}
		}
		#endregion

		#region prop: IsAdmin
		/// <summary>
		/// Gets or sets a value indicating whether this instance is admin.
		/// </summary>
		/// <value><c>true</c> if this instance is admin; otherwise, <c>false</c>.</value>
		public bool IsAdmin
		{
			get
			{
				if (ViewState["_IsAdmin"] != null)
					return Convert.ToBoolean(ViewState["_IsAdmin"].ToString(), CultureInfo.InvariantCulture);

				return false;
			}
			set
			{
				ViewState["_IsAdmin"] = value;
			}
		}
		#endregion

		#region prop: ControlPlaces
		private ArrayList controlPlaces;
		public ArrayList ControlPlaces
		{
			get
			{
				if (controlPlaces == null)
					controlPlaces = new ArrayList();

				return controlPlaces;
			}
		}
		#endregion

		#region prop: JsonItems
		private string jsonItems = string.Empty;
		public string JsonItems
		{
			get
			{
				return jsonItems;
			}
			set
			{
				jsonItems = value;
			}
		}
		#endregion

		#region FillJsonItems
		/// <summary>
		/// Fills the json items.
		/// </summary>
		private void FillJsonItems()
		{
			this.JsonItems = string.Empty;

			foreach (Control ctrl in this.ControlPlaces)
			{
				if (ctrl is IbnControlPlace)
				{
					this.JsonItems += ((IbnControlPlace)ctrl).GetItemsJson() + ",";
				}
			}

			if (this.JsonItems.Length > 0)
				this.JsonItems = this.JsonItems.Remove(this.JsonItems.Length - 1);
		}
		#endregion

		#region ProfileId
		protected int? ProfileId
		{
			get
			{
				int? retval = null;
				HttpRequest request = HttpContext.Current.Request;
				if (String.Compare(request["ClassName"], CustomizationProfileEntity.ClassName, true) == 0
					&& !String.IsNullOrEmpty(request["ObjectId"]))
					retval = int.Parse(request["ObjectId"]);
				return retval;
			}
		}
		#endregion

		#region UserId
		protected int? UserId
		{
			get
			{
				int? retval = null;

				if (!this.IsAdmin)
				{
					return Mediachase.IBN.Business.Security.UserID;
				}
				else
				{
					HttpRequest request = HttpContext.Current.Request;
					if (String.Compare(request["ClassName"], "Principal", true) == 0
						&& !String.IsNullOrEmpty(request["ObjectId"]))
						retval = int.Parse(request["ObjectId"]);
				}
				return retval;
			}
		}
		#endregion

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.EnsureChildControls();
			this.EnsureDataBound();
		}

		protected override void OnPreRender(EventArgs e)
		{
			//this.ViewState["__dvsKeyTest"] = "__dvsKeyTest111";
			FillJsonItems();
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute("id", this.ClientID);
			writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
			writer.RenderBeginTag("div");

			base.RenderChildren(writer);

			writer.RenderEndTag();

			writer.AddAttribute("id", "clearLayout");
			writer.AddAttribute("clear", "both");
			writer.RenderBeginTag("div");
			writer.RenderEndTag();
		}

		#region GetIdsForControlPlace
		/// <summary>
		/// Gets the ids for control place.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		String GetIdsForControlPlace(string id)
		{
			String retVal = String.Empty;
			string userSettings = string.Empty;
			
			// using if getting old settings from database(controls without instanceUid)
			bool _updateUidFlag = false;

			CustomPageEntity page = null;
			if (!IsAdmin)
			{
				page = CustomPageManager.GetCustomPage(PageUid, null, Mediachase.IBN.Business.Security.CurrentUser.UserID);
			}
			else
			{
				page = CustomPageManager.GetCustomPage(PageUid, ProfileId, UserId);
			}
			userSettings = page.JsonData;

			List<CpInfo> list = UtilHelper.JsonDeserialize<List<CpInfo>>(userSettings);

			foreach (CpInfo cpInfo in list)
			{
				if (cpInfo.Id == id)
				{
					for (int i = 0; i < cpInfo.Items.Count; i++)
					{
						if (String.IsNullOrEmpty(cpInfo.Items[i].InstanseUid))
						{
							cpInfo.Items[i].InstanseUid = Guid.NewGuid().ToString("N");
							_updateUidFlag = true;
						}
						retVal += String.Format("{0}^{1}^{2}:", cpInfo.Items[i].Id, cpInfo.Items[i].Collapsed, cpInfo.Items[i].InstanseUid); //cpInfo.Items[i].Id + ":";
					}
				}
			}

			#region Update InstanceUid to database
			if (_updateUidFlag)
			{
				string userData = UtilHelper.JsonSerialize(list);

				if (!IsAdmin)
				{
					CustomPageManager.UpdateCustomPage(PageUid, userData, page.TemplateId, null, Mediachase.IBN.Business.Security.CurrentUser.UserID);
				}
				else
				{
					CustomPageManager.UpdateCustomPage(PageUid, userData, page.TemplateId, ProfileId, UserId);
				}
			} 
			#endregion


			if (retVal.Length > 0)
				retVal = retVal.TrimEnd(':');

			return retVal;
		}
		#endregion

		#region CreateChildControls
		/// <summary>
		/// When overridden in an abstract class, creates the control hierarchy that is used to render the composite data-bound control based on the values from the specified data source.
		/// </summary>
		/// <param name="dataSource">An <see cref="T:System.Collections.IEnumerable"></see> that contains the values to bind to the control.</param>
		/// <param name="dataBinding">true to indicate that the <see cref="M:System.Web.UI.WebControls.CompositeDataBoundControl.CreateChildControls(System.Collections.IEnumerable,System.Boolean)"></see> is called during data binding; otherwise, false.</param>
		/// <returns>
		/// The number of items created by the <see cref="M:System.Web.UI.WebControls.CompositeDataBoundControl.CreateChildControls(System.Collections.IEnumerable,System.Boolean)"></see>.
		/// </returns>
		protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
		{
			HttpContext.Current.Items[ControlProperties._pageUidKey] = this.PageUid;
			HttpContext.Current.Items[ControlProperties._profileUidKey] = this.ProfileId;
			HttpContext.Current.Items[ControlProperties._userUidKey] = this.UserId;

			if (dataBinding)
			{
				ViewState["__TemplateUid"] = dataSource.ToString();

				this.BindData(dataSource.ToString());

				foreach (Control ctrl in this.ControlPlaces)
				{
					if (ctrl is IbnControlPlace)
					{
						//((IbnControlPlace)ctrl).DataSource = IbnFactory.GetIdsForControlPlace(ctrl.ID);
						((IbnControlPlace)ctrl).DataSource = GetIdsForControlPlace(((IbnControlPlace)ctrl).ControlPlaceId).Split(':');

						//if (!this.Page.IsPostBack)
						((IbnControlPlace)ctrl).DataBind();
					}
				}
			}
			else
			{
				if (ViewState["__TemplateUid"] == null)
					throw new ArgumentNullException("__TemplateUid @ IbnControlPlaceManager");

				string _uid = ViewState["__TemplateUid"].ToString();

				this.BindData(_uid);
			}

			//2. Init DataSource foreach PlaceHolderWrapper


			return 0;
		}
		#endregion

		#region BindData
		/// <summary>
		/// Binds the data.
		/// </summary>
		/// <param name="uid">The uid.</param>
		private void BindData(string uid)
		{
			if (uid == null)
				throw new ArgumentNullException("uid");

			WorkspaceTemplateInfo wti = WorkspaceTemplateFactory.GetTemplateInfo(uid);

			if (wti == null)
				throw new ArgumentException(string.Format("Cant find Template with uid: {0}", uid));

			List<ColumnInfo> list = UtilHelper.JsonDeserialize<List<ColumnInfo>>(wti.ColumnInfo);

			foreach (ColumnInfo ci in list)
			{
				IbnControlPlace icp = new IbnControlPlace();
				icp.WidthPercentage = ci.Width;
				icp.ID = ci.Id;
				icp.ControlPlaceId = ci.Id;
				icp.PageUid = this.PageUid;
				icp.IsAdmin = this.IsAdmin;
				this.Controls.Add(icp);
			}
		}
		#endregion
	}
}

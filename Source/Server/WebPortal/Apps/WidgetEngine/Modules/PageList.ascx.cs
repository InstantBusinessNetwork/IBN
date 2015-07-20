using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Business.Customization;
using Mediachase.IBN.Business.WidgetEngine;


namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules
{
	public partial class PageList : MCDataBoundControl
	{
		#region ClassName
		protected string ClassName
		{
			get
			{
				return Request["ClassName"];
			}
		}
		#endregion

		#region ObjectId
		protected int? ObjectId
		{
			get
			{
				try
				{
					
					return int.Parse(Request["ObjectId"]);
				}
				catch
				{
					return null;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			RegisterStyle();
		}

		public override void DataBind()
		{
			ctrlToolbar.GridId = ctrlGrid.GridClientContainerId;
			ctrlGrid.GridFilters = GetFilters();
		}

		#region GetFilters
		private FilterElementCollection GetFilters()
		{
			FilterElementCollection coll = new FilterElementCollection();
			if (!String.IsNullOrEmpty(ClassName))
			{
				if (ObjectId == null)
					throw new ArgumentNullException("QueryString:ObjectId");

				//Profile level
				if (String.Compare(ClassName, CustomizationProfileEntity.ClassName, true) == 0)
				{
					// ProfileId is null OR ProfileId = value
					OrBlockFilterElement orBlock = new OrBlockFilterElement();
					orBlock.ChildElements.Add(FilterElement.IsNullElement(CustomPageEntity.FieldProfileId));
					orBlock.ChildElements.Add(FilterElement.EqualElement(CustomPageEntity.FieldProfileId, ObjectId));

					coll.Add(orBlock);
					coll.Add(FilterElement.IsNullElement(CustomPageEntity.FieldUserId));
				}

				//User level
				if (String.Compare(ClassName, "Principal", true) == 0)
				{
					int profileId = ProfileManager.GetProfileIdByUser();

					// ProfileId is null AND UserId is null
					AndBlockFilterElement andBlock1 = new AndBlockFilterElement();
					andBlock1.ChildElements.Add(FilterElement.IsNullElement(CustomPageEntity.FieldProfileId));
					andBlock1.ChildElements.Add(FilterElement.IsNullElement(CustomPageEntity.FieldUserId));

					// ProfileId = value AND UserId is null
					AndBlockFilterElement andBlock2 = new AndBlockFilterElement();
					andBlock2.ChildElements.Add(FilterElement.EqualElement(CustomPageEntity.FieldProfileId, profileId));
					andBlock2.ChildElements.Add(FilterElement.IsNullElement(CustomPageEntity.FieldUserId));

					// ProfileId is null AND UserId = value
					AndBlockFilterElement andBlock3 = new AndBlockFilterElement();
					andBlock3.ChildElements.Add(FilterElement.IsNullElement(CustomPageEntity.FieldProfileId));
					andBlock3.ChildElements.Add(FilterElement.EqualElement(CustomPageEntity.FieldUserId, ObjectId));

					OrBlockFilterElement orBlock = new OrBlockFilterElement();
					orBlock.ChildElements.Add(andBlock1);
					orBlock.ChildElements.Add(andBlock2);
					orBlock.ChildElements.Add(andBlock3);

					coll.Add(orBlock);
				}
			}
			else
			{
				//Site level
				coll.Add(FilterElement.IsNullElement(CustomPageEntity.FieldProfileId));
				coll.Add(FilterElement.IsNullElement(CustomPageEntity.FieldUserId));

			}
			return coll;
		} 
		#endregion

		#region RegisterStyle
		/// <summary>
		/// Registers the style.
		/// </summary>
		void RegisterStyle()
		{
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("<link rel='stylesheet' type='text/css' href='{0}' />",
					McScriptLoader.Current.GetScriptUrl("~/styles/IbnFramework/grid.css", this.Page)));
		} 
		#endregion
	}
}
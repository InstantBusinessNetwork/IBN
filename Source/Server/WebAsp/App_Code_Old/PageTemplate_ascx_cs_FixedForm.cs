//====================================================================
// This file is generated as part of Web project conversion.
// The extra class 'FixedForm' in the code behind file in 'Modules\PageTemplate.ascx.cs' is moved to this file.
//====================================================================
using System;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Configuration;


namespace Mediachase.Ibn.WebAsp.Modules
{
	public class FixedForm: System.Web.UI.HtmlControls.HtmlForm
	{
		public override string Name
		{
			get
			{
				return UniqueID;
			}

			set
			{
			}
		}

		public override string UniqueID
		{
			get
			{
				return base.UniqueID.Replace(":","_");
			}
		}
		
		public FixedForm()
		{
		}
	}
}
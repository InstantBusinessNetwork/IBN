using System;
using System.Resources;
using System.Text;

using ComponentArt.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Public
{
	/// <summary>
	/// Summary description for TermsOfUseRUS.
	/// </summary>
	public partial class TermsOfUseRUS : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Home.Resources.strWorkspace", typeof(TermsOfUseRUS).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/tabStyle.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/public.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			if (!IsPostBack)
			{
				TabStripTab tbt = new TabStripTab();
				tbt.Text = LocRM.GetString("tTrial");
				TabStrip1.Tabs.Add(tbt);
				tbt = new TabStripTab();
				tbt.Text = LocRM.GetString("tCommerce");
				TabStrip1.Tabs.Add(tbt);

				lblTrialVersion.InnerHtml = @"<p>В настоящий момент Вы используете триал версию программного продукта Instant Business Network, разработки ООО «Медиачеис».</p>
								<p>Компания «Медиачеис» не несет ответственности за информацию и материалы, размещаемые Вами в системе Instant Business Network 4.7.</p>
								<p>Компания «Медиачеис» не гарантирует полную работоспособность триал версии системы и не предоставляет технические консультации пользователям триал версии Instant Business Network 4.7</p>
								<p>По истечении срока действия триал версии Вы обязаны удалить программный продукт либо приобрести коммерческую версию Instant Business Network 4.7.</p>
								<p>По вопросам приобретения коммерческой версии Instant Business Network 4.7 обращайтесь:</p>
								<p>По телефонам:</p>
								<ul><li>+7 (495) 648 61 62 (Москва)</li><li>+7 (4012) 36 85 98 (Калининград)</li></ul>
								<p>По электронной почте:<br/>
								<a href='mailto:sales@mediachase.ru'>sales@mediachase.ru</a></p>
								<p>Адрес веб сайта компании:<br/>
								<a href='http://www.pmbox.ru' target='_blank'>www.mediachase.ru</a></p>";

				lblBillableVersion.InnerHtml = @"<p>В настоящий момент Вы используете коммерческую версию программного продукта Instant Business Network, разработки ООО «Медиачеис».</p>
								<p>Использование коммерческой версии данного продукта без подписания лицензионного соглашения противоречит законодательству Российской Федерации.</p>
								<p>Текст лицензионного соглашения вы можете загрузить с сайта компании или запросить по приведенным ниже контактам.</p>
								<p>По всем вопросам, связанным с использованием Instant Business Network 4.7, обращайтесь:</p>
								<p>По телефонам:</p>
								<ul><li>+7 (495) 648 61 62 (Москва)</li><li>+7 (4012) 36 85 98 (Калининград)</li></ul>
								<p>По электронной почте:<br/>
								<a href='mailto:sales@mediachase.ru'>sales@mediachase.ru</a><br/>
								<a href='mailto:info@mediachase.ru'>info@mediachase.ru</a></p>
								<p>Адрес веб сайта компании:<br/>
								<a href='http://www.pmbox.ru' target='_blank'>www.mediachase.ru</a></p>";
			}
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}

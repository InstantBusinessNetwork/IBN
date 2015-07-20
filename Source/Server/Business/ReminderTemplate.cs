using System.Data;
using System.Resources;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for ReminderTemplate.
	/// </summary>
	public class ReminderTemplate
	{
		private static ResourceManager rmTemplates = new ResourceManager(AlertTemplate.sReminderTemplates, typeof(ReminderTemplate).Assembly);
		private static ResourceManager rmTypes = new ResourceManager("Mediachase.IBN.Business.Resources.ReminderTypes", typeof(ReminderTemplate).Assembly);

		internal static ResourceManager TemplatesRM{ get{ return rmTemplates; } }
		internal static ResourceManager TypesRM{ get{ return rmTypes; } }

		#region * Fields *
		public string Subject;
		public string Body;
		public string Locale;

		internal int LanguageId;
		internal DateTypes DateType;
		#endregion

		#region * Constructors *
		public ReminderTemplate(DateTypes dateType, string locale)
		{
			Locale = locale;
			DateType = dateType;

			using(IDataReader reader = Common.GetListLanguages())
			{
				while(reader.Read())
				{
					if(Locale == reader["Locale"].ToString())
					{
						LanguageId = (int)reader["LanguageId"];
						break;
					}
				}
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ReminderTemplate"/> class.
		/// </summary>
		/// <param name="dateType">Type of the date.</param>
		/// <param name="objecTypeId">The objec type id.</param>
		/// <param name="languageId">The language id.</param>
		public ReminderTemplate(DateTypes dateType, int languageId)
		{
			LanguageId = languageId;
			DateType = dateType;
			//ObjecTypeId = objecTypeId;

			using(IDataReader reader = Common.GetListLanguages())
			{
				while(reader.Read())
				{
					if((int)reader["LanguageId"] == languageId)
					{
						Locale = reader["Locale"].ToString();
						break;
					}
				}
			}
		}
		#endregion

		#region Load()
		public bool Load()
		{
			return Load(true);
		}

		public bool Load(bool getFromDb)
		{
			return AlertTemplate.GetTemplate(AlertTemplateTypes.Reminder, Locale, DateType.ToString(), getFromDb, out Subject, out Body);
		}
		#endregion
	}
}

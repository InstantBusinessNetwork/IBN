using System;
using System.Collections;
using Mediachase.IBN.Database.SpreadSheet;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for BusinessScore.
	/// </summary>
	public class BusinessScore
	{
		private BusinessScoreRow _srcRow = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="BusinessScore"/> class.
		/// </summary>
		/// <param name="row">The row.</param>
		private BusinessScore(BusinessScoreRow row)
		{
			_srcRow = row;
		}

		#region Public Properties
		
		/// <summary>
		/// Gets the business score id.
		/// </summary>
		/// <value>The business score id.</value>
		public virtual int BusinessScoreId
	    
		{
			get
			{
				return _srcRow.BusinessScoreId;
			}
			
		}
		
		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>The key.</value>
		public virtual string Key
	    
		{
			get
			{
				return _srcRow.Key;
			}
			
			set
			{
				_srcRow.Key = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public virtual string Name
	    
		{
			get
			{
				return _srcRow.Name;
			}
			
			set
			{
				_srcRow.Name = value;
			}	
			
		}
		
		#endregion


		/// <summary>
		/// Deletes the specified business score id.
		/// </summary>
		/// <param name="BusinessScoreId">The business score id.</param>
		public static int Create(string Key, string Name)
		{
			BusinessScoreRow row = new BusinessScoreRow();

			row.Key = Key;
			row.Name = Name;

			row.Update();
			return row.PrimaryKeyId;
		}

		public static void Update(BusinessScore item)
		{
			item._srcRow.Update();
		}

		/// <summary>
		/// Loads the specified business score id.
		/// </summary>
		/// <param name="BusinessScoreId">The business score id.</param>
		/// <returns></returns>
		public static BusinessScore Load(int BusinessScoreId)
		{
			return new BusinessScore(new BusinessScoreRow(BusinessScoreId));
		}

		/// <summary>
		/// Lists this instance.
		/// </summary>
		/// <returns></returns>
		public static BusinessScore[] List()
		{
			ArrayList retVal = new ArrayList();

			foreach(BusinessScoreRow row in BusinessScoreRow.List())
			{
				retVal.Add(new BusinessScore(row));
			}

			return (BusinessScore[])retVal.ToArray(typeof(BusinessScore));
		}

		/// <summary>
		/// Deletes the specified business score id.
		/// </summary>
		/// <param name="BusinessScoreId">The business score id.</param>
		public static void Delete(int BusinessScoreId)
		{
			BusinessScoreRow.Delete(BusinessScoreId); 
		}
	}
}

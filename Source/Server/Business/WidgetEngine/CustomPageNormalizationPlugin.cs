using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using System.Collections;

namespace Mediachase.IBN.Business.WidgetEngine
{
	/// <summary>
	/// Represents custom page normalization plugin.
	/// </summary>
	public sealed class CustomPageNormalizationPlugin: IPlugin
	{
		#region Const
		public const string FieldNameCreatedLevel = "CustomPageNormalization_CreatedLevel";
		public const string FieldNameModifiedLevel = "CustomPageNormalization_ModifiedLevel";

		#endregion

		#region IPlugin Members

		public void Execute(BusinessContext context)
		{
			if (context.Request.Parameters.Contains("CustomPageNormalizationPlugin") && !(bool)context.Request.Parameters["CustomPageNormalizationPlugin"].Value)
				return;

			ListResponse response = context.Response as ListResponse;

			if (response != null)
			{
				List<EntityObject> items = new List<EntityObject>(response.EntityObjects);

				if (items.Count > 0)
				{
					items.Sort(CustomPageEntitySort);
				}

				Hashtable uidHash = new Hashtable();

				List<EntityObject> resultItem = new List<EntityObject>();

				foreach (CustomPageEntity item in items)
				{
					if (uidHash.ContainsKey(item.Uid))
					{
						CustomPageEntity topElement = (CustomPageEntity)uidHash[item.Uid];

						topElement[FieldNameCreatedLevel] = GetLevel(item);
					}
					else
					{
						resultItem.Add(item);

						uidHash.Add(item.Uid, item);

						item[FieldNameCreatedLevel] = GetLevel(item);
						item[FieldNameModifiedLevel] = GetLevel(item);
					}
				}

				response.EntityObjects = resultItem.ToArray();
			}
		}

		/// <summary>
		/// Gets the level.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		private CustomPageLevel GetLevel(CustomPageEntity item)
		{
			if (item.UserId.HasValue)
				return CustomPageLevel.User;

			if (item.ProfileId.HasValue)
				return CustomPageLevel.Profile;

			if (item.Properties.Contains(LocalDiskEntityObjectPlugin.IsLoadDiskEntityPropertyName))
				return CustomPageLevel.Disk;

			return CustomPageLevel.Global;
		}

		#endregion

		#region Test
		void Test()
		{
			List<int?> list = new List<int?>();

			list.Add(6);
			list.Add(null);
			list.Add(1);
			list.Add(26);
			list.Add(5);

			list.Sort(CustomPageEntitySort2);
		}

		private int CustomPageEntitySort2(int? x, int? y)
		{
			if (x == null && y == null)
				return 0;

			if (x == null)
				return 1;
			if (y == null)
				return -1;

			return -1 * ((IComparable)x).CompareTo(y);
		} 
		#endregion

		private int CustomPageEntitySort(EntityObject x, EntityObject y)
		{
			CustomPageEntity xPage = x as CustomPageEntity;
			CustomPageEntity yPage = y as CustomPageEntity;

			// Global
			if (xPage == null && yPage==null)
				return 0;
			if (xPage == null)
				return 1;
			if (yPage == null)
				return -1;

			// User
			if (xPage.UserId == null && yPage.UserId == null)
			{
				// Profile
				if (xPage.ProfileId == null && yPage.ProfileId == null)
				{
					// Global
					if (xPage.Properties.Contains(LocalDiskEntityObjectPlugin.IsLoadDiskEntityPropertyName))
						return 1;
					if (yPage.Properties.Contains(LocalDiskEntityObjectPlugin.IsLoadDiskEntityPropertyName))
						return -1;
				}
				if (xPage.ProfileId == null)
					return 1;
				if (yPage.ProfileId == null)
					return -1;
			}

			if (xPage.UserId == null)
				return 1;
			if (yPage.UserId == null)
				return -1;


			return 0;
		}
	}
}

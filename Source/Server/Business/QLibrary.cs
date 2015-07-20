using System;
using System.Data;
using System.Resources;

using Mediachase.SQLQueryCreator;

namespace Mediachase.IBN.Business.Reports
{
	/// <summary>
	/// Summary description for QLibrary.
	/// </summary>
	public class QLibrary : QObject
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.IBN.Business.Resources.QObjectsResource", typeof(QCalendarEntries).Assembly);

		public QLibrary()
		{
		}

		public override FilterCondition[] GetWhereExtensions()
		{
			return new FilterCondition[]{new SimpleFilterCondition(this.Fields["AssetVersionId"],"(SELECT MAX(VersionId) FROM ASSET_VERSIONS GROUP BY AssetId)",SimpleFilterType.In),
			new SimpleFilterCondition(this.Fields["AssetId"],"(SELECT AssetId FROM ASSET_CONTAINERS WHERE ObjectTypeId=10 AND IsLink=0)",SimpleFilterType.In)};
		}

		protected override void LoadScheme()
		{
			OwnerTable = "ASSETS";

			//key
			this.Fields.Add(new QField("AssetId", LocRM.GetString("AssetId"), "AssetId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, true));

			//VersionId 1
			this.Fields.Add(new QField("AssetVersionId", LocRM.GetString("AssetVersionId"), "VersionId", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "ASSET_VERSIONS", "AssetId", "AssetId")));

			//CreatedDate	1
			this.Fields.Add(new QField("AssetCreationDate", LocRM.GetString("AssetCreationDate"), "CreatedDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, true));

			//TODO:ModifiedDate       1
			this.Fields.Add(new QField("AssetModifiedDate", LocRM.GetString("AssetModifiedDate"), "CreatedDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "ASSET_VERSIONS", "AssetId", "AssetId")));

			//CreatorId	1
			this.Fields.Add(new QField("AssetCreator", LocRM.GetString("AssetCreator"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));
			this.Fields.Add(new QField("AssetCreatorId", LocRM.GetString("AssetCreatorId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["AssetCreatorId"], this.Fields["AssetCreator"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT CreatorId FROM ASSETS) ORDER BY [Value]"));

			//Title                        1
			this.Fields.Add(new QField("AssetTitle", LocRM.GetString("AssetTitle"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation(this.OwnerTable, "ASSET_VERSIONS", "AssetId", "AssetId")));

			//Type		
			this.Fields.Add(new QField("AssetType", LocRM.GetString("AssetType"),
				"(CASE {0}.IsInternal WHEN 1 THEN N'" + LocRM.GetString("AssetType_File") + "' WHEN 0 THEN N'" + LocRM.GetString("AssetType_Link") + "' END)",
				DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "ASSET_VERSIONS", "AssetId", "AssetId"),
				true));

			this.Fields.Add(new QField("AssetTypeId", "AssetTypeId",
				"(CASE {0}.IsInternal WHEN 1 THEN N'" + LocRM.GetString("AssetType_File") + "' WHEN 0 THEN N'" + LocRM.GetString("AssetType_Link") + "' END)",
				DbType.String, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "ASSET_VERSIONS", "AssetId", "AssetId"),
				true));

			this.Dictionary.Add(new QDictionary(this.Fields["AssetTypeId"], this.Fields["AssetType"],
				"SELECT N'" + LocRM.GetString("AssetType_File") + "' As Id, N'" + LocRM.GetString("AssetType_File") + "' As Value" +
				" UNION " +
				"SELECT N'" + LocRM.GetString("AssetType_Link") + "' As Id, N'" + LocRM.GetString("AssetType_Link") + "' As Value"
				));

			//FileName
			this.Fields.Add(new QField("AssetFileName", LocRM.GetString("AssetFileName"),
				"(CASE {0}.IsInternal WHEN 1 THEN {0}.FileName WHEN 0 THEN NULL END)",
				DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "ASSET_VERSIONS", "AssetId", "AssetId")));

			//Size	
			this.Fields.Add(new QField("AssetSize", LocRM.GetString("AssetSize"),
				"(CASE {0}.IsInternal WHEN 1 THEN {0}.[Size] WHEN 0 THEN NULL END)",
				DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "ASSET_VERSIONS", "AssetId", "AssetId")));

			//URL
			this.Fields.Add(new QField("AssetURL", LocRM.GetString("AssetURL"),
				"(CASE {0}.IsInternal WHEN 1 THEN NULL WHEN 0 THEN {0}.URL END)",
				DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "ASSET_VERSIONS", "AssetId", "AssetId")));

			//ParentFolder
			this.Fields.Add(new QField("AssetParentFolder", LocRM.GetString("AssetParentFolder"), "Name", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"ASSET_CONTAINERS","AssetId","AssetId", 
						new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.Folder).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("ASSET_CONTAINERS","FOLDERS","ObjectId","FolderId")
				}));

			//ParentFolderId
			this.Fields.Add(new QField("AssetParentFolderId", LocRM.GetString("AssetParentFolderId"), "FolderId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"ASSET_CONTAINERS","AssetId","AssetId", 
					new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.Folder).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("ASSET_CONTAINERS","FOLDERS","ObjectId","FolderId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["AssetParentFolderId"], this.Fields["AssetParentFolder"], "SELECT FolderId as Id, Name as Value FROM FOLDERS"));

			//ContentType
			this.Fields.Add(new QField("AssetContentType", LocRM.GetString("AssetContentType"),
				"ContentTypeString",
				DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"ASSET_VERSIONS","AssetId","AssetId"),
					new QFieldJoinRelation("ASSET_VERSIONS","CONTENT_TYPES","ContentTypeId","ContentTypeId")
				}));

			this.Fields.Add(new QField("AssetContentTypeId", "AssetContentTypeId",
				"ContentTypeId",
				DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"ASSET_VERSIONS","AssetId","AssetId"),
					new QFieldJoinRelation("ASSET_VERSIONS","CONTENT_TYPES","ContentTypeId","ContentTypeId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["AssetContentTypeId"], this.Fields["AssetContentType"], "SELECT ContentTypeId as Id, FriendlyName as Value FROM CONTENT_TYPES"));

			QMetaLoader.LoadMetaField(this, "AssetsEx");
		}
	}
}

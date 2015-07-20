if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_MetaClassMetaFieldRelation_MetaClass]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[MetaClassMetaFieldRelation] DROP CONSTRAINT FK_MetaClassMetaFieldRelation_MetaClass
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_MetaField_MetaDataType]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[MetaField] DROP CONSTRAINT FK_MetaField_MetaDataType
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_MetaClassMetaFieldRelation_MetaField]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[MetaClassMetaFieldRelation] DROP CONSTRAINT FK_MetaClassMetaFieldRelation_MetaField
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_MetaDictionary_MetaField]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[MetaDictionary] DROP CONSTRAINT FK_MetaDictionary_MetaField
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_MetaFileValue_MetaKey]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[MetaFileValue] DROP CONSTRAINT FK_MetaFileValue_MetaKey
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_MetaMultivalueDictionary_MetaKey]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[MetaMultivalueDictionary] DROP CONSTRAINT FK_MetaMultivalueDictionary_MetaKey
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_AddMetaAttribute]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_AddMetaAttribute]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_AddMetaDictionary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_AddMetaDictionary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_AddMetaField]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_AddMetaField]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_AddMetaFieldToMetaClass]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_AddMetaFieldToMetaClass]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_AddMetaStringDictionary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_AddMetaStringDictionary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_AddMultivalueDictionary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_AddMultivalueDictionary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_CheckReplaceUser]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_CheckReplaceUser]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_ClearMetaAttribute]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_ClearMetaAttribute]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_ClearMultivalueDictionary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_ClearMultivalueDictionary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_ClearStringDictionary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_ClearStringDictionary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_CreateMetaClass]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_CreateMetaClass]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_CreateMetaClassHistoryTrigger]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_CreateMetaClassHistoryTrigger]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_CreateMetaClassProcedure]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_CreateMetaClassProcedure]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_CreateMetaClassProcedureAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_CreateMetaClassProcedureAll]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_DeleteDContrainByTableAndField]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_DeleteDContrainByTableAndField]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_DeleteMetaAttribute]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_DeleteMetaAttribute]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_DeleteMetaClass]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_DeleteMetaClass]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_DeleteMetaClassProcedure]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_DeleteMetaClassProcedure]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_DeleteMetaDictionary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_DeleteMetaDictionary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_DeleteMetaField]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_DeleteMetaField]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_DeleteMetaFieldFromMetaClass]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_DeleteMetaFieldFromMetaClass]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_DeleteMetaFile]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_DeleteMetaFile]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_DeleteMetaKeyObjects]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_DeleteMetaKeyObjects]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_DeleteMetaObjectValue]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_DeleteMetaObjectValue]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_DeleteMetaRule]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_DeleteMetaRule]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_FullTextQueriesActivate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_FullTextQueriesActivate]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_FullTextQueriesAddAllFields]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_FullTextQueriesAddAllFields]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_FullTextQueriesDeactivate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_FullTextQueriesDeactivate]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_FullTextQueriesDeleteAllFields]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_FullTextQueriesDeleteAllFields]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_FullTextQueriesEnable]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_FullTextQueriesEnable]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_FullTextQueriesFieldUpdate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_FullTextQueriesFieldUpdate]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_GetMetaKey]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_GetMetaKey]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_GetMetaKeyInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_GetMetaKeyInfo]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_GetUniqueFieldName]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_GetUniqueFieldName]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadChildMetaClassList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadChildMetaClassList]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaAttributes]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaAttributes]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaClassById]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaClassById]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaClassByName]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaClassByName]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaClassByNamespace]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaClassByNamespace]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaClassList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaClassList]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaClassListByMetaField]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaClassListByMetaField]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaDictionary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaDictionary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaField]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaField]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaFieldByName]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaFieldByName]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaFieldByNamespace]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaFieldByNamespace]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaFieldList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaFieldList]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaFieldListByMetaClassId]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaFieldListByMetaClassId]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaFieldWeight]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaFieldWeight]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaFile]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaFile]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaFileList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaFileList]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaObjectValue]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaObjectValue]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaRuleById]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaRuleById]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaRuleByMetaClassId]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaRuleByMetaClassId]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaStringDictionary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaStringDictionary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMetaTypeList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMetaTypeList]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_LoadMultivalueDictionary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_LoadMultivalueDictionary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_MetaFieldAllowSearch]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_MetaFieldAllowSearch]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_MetaFieldSaveHistory]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_MetaFieldSaveHistory]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_ReplaceUser]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_ReplaceUser]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_UpdateMetaClass]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_UpdateMetaClass]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_UpdateMetaDictionary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_UpdateMetaDictionary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_UpdateMetaField]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_UpdateMetaField]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_UpdateMetaFieldEnabled]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_UpdateMetaFieldEnabled]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_UpdateMetaFieldWeight]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_UpdateMetaFieldWeight]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_UpdateMetaFile]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_UpdateMetaFile]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_UpdateMetaObjectValue]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_UpdateMetaObjectValue]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_UpdateMetaRule]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_UpdateMetaRule]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mdpsp_sys_UpdateMetaSqlScriptTemplate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[mdpsp_sys_UpdateMetaSqlScriptTemplate]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MetaRule]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MetaRule]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MetaAttribute]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MetaAttribute]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MetaClass]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MetaClass]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MetaClassMetaFieldRelation]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MetaClassMetaFieldRelation]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MetaDataType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MetaDataType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MetaDictionary]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MetaDictionary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MetaField]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MetaField]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MetaFileValue]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MetaFileValue]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MetaKey]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MetaKey]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MetaMultivalueDictionary]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MetaMultivalueDictionary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MetaObjectValue]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MetaObjectValue]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MetaStringDictionaryValue]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MetaStringDictionaryValue]
GO

CREATE TABLE [dbo].[MetaRule] (
	[RuleId] [int] IDENTITY (1, 1) NOT NULL ,
	[MetaClassId] [int] NOT NULL ,
	[Data] [image] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[MetaAttribute] (
	[AttrOwnerId] [int] NOT NULL ,
	[AttrOwnerType] [int] NOT NULL ,
	[Key] [nvarchar] (256) NOT NULL ,
	[Value] [ntext] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[MetaClass] (
	[MetaClassId] [int] IDENTITY (1, 1) NOT NULL ,
	[Namespace] [nvarchar] (1024) NOT NULL ,
	[Name] [nvarchar] (256) NOT NULL ,
	[FriendlyName] [nvarchar] (256) NOT NULL ,
	[IsSystem] [bit] NOT NULL ,
	[IsAbstract] [bit] NOT NULL ,
	[ParentClassId] [int] NOT NULL ,
	[TableName] [nvarchar] (256) NOT NULL ,
	[PrimaryKeyName] [nvarchar] (256) NOT NULL ,
	[Description] [ntext] NULL ,
	[FieldListChangedSqlScript] [ntext] NULL ,
	[Tag] [image] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[MetaClassMetaFieldRelation] (
	[MetaClassId] [int] NOT NULL ,
	[MetaFieldId] [int] NOT NULL ,
	[Weight] [int] NOT NULL ,
	[Enabled] [bit] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[MetaDataType] (
	[DataTypeId] [int] NOT NULL ,
	[Name] [nvarchar] (256) NOT NULL ,
	[FriendlyName] [nvarchar] (256) NOT NULL ,
	[Description] [ntext] NULL ,
	[Length] [int] NOT NULL ,
	[SqlName] [nvarchar] (256) NOT NULL ,
	[AllowNulls] [bit] NOT NULL ,
	[Variable] [bit] NOT NULL ,
	[IsSQLCommonType] [bit] NOT NULL ,
	[DefaultValue] [nvarchar] (256) NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[MetaDictionary] (
	[MetaDictionaryId] [int] IDENTITY (1, 1) NOT NULL ,
	[MetaFieldId] [int] NOT NULL ,
	[Value] [nvarchar] (2048) NOT NULL ,
	[Tag] [image] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[MetaField] (
	[MetaFieldId] [int] IDENTITY (1, 1) NOT NULL ,
	[Name] [nvarchar] (256) NOT NULL ,
	[Namespace] [nvarchar] (1024) NOT NULL ,
	[SystemMetaClassId] [int] NOT NULL ,
	[FriendlyName] [nvarchar] (256) NOT NULL ,
	[Description] [ntext] NULL ,
	[DataTypeId] [int] NOT NULL ,
	[Length] [int] NOT NULL ,
	[AllowNulls] [bit] NOT NULL ,
	[SaveHistory] [bit] NOT NULL ,
	[MultiLanguageValue] [bit] NOT NULL ,
	[AllowSearch] [bit] NOT NULL ,
	[Tag] [image] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[MetaFileValue] (
	[MetaKey] [int] NOT NULL ,
	[FileName] [nvarchar] (256) NULL ,
	[ContentType] [nvarchar] (50) NULL ,
	[Data] [image] NULL ,
	[Size] [int] NOT NULL ,
	[CreationTime] [datetime] NOT NULL ,
	[LastWriteTime] [datetime] NOT NULL ,
	[LastReadTime] [datetime] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[MetaKey] (
	[MetaKey] [int] IDENTITY (1, 1) NOT NULL ,
	[MetaObjectId] [int] NOT NULL ,
	[MetaClassId] [int] NOT NULL ,
	[MetaFieldId] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[MetaMultivalueDictionary] (
	[MetaKey] [int] NOT NULL ,
	[MetaDictionaryId] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[MetaObjectValue] (
	[MetaKey] [int] NOT NULL ,
	[MetaClassId] [int] NOT NULL ,
	[MetaObjectId] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[MetaStringDictionaryValue] (
	[MetaDictionaryId] [int] IDENTITY (1, 1) NOT NULL,
	[MetaKey] [int] NOT NULL ,
	[Key] [nvarchar] (512) NOT NULL ,
	[Value] [ntext] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[MetaRule] WITH NOCHECK ADD 
	CONSTRAINT [PK_MetaRule] PRIMARY KEY  CLUSTERED 
	(
		[RuleId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaAttribute] WITH NOCHECK ADD 
	CONSTRAINT [PK_MetaAttribute] PRIMARY KEY  CLUSTERED 
	(
		[AttrOwnerId],
		[AttrOwnerType],
		[Key]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaClass] WITH NOCHECK ADD 
	CONSTRAINT [PK_MetaClass] PRIMARY KEY  CLUSTERED 
	(
		[MetaClassId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaClassMetaFieldRelation] WITH NOCHECK ADD 
	CONSTRAINT [PK_MetaClassMetaFieldRelation] PRIMARY KEY  CLUSTERED 
	(
		[MetaClassId],
		[MetaFieldId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaDataType] WITH NOCHECK ADD 
	CONSTRAINT [PK_MetaDataType] PRIMARY KEY  CLUSTERED 
	(
		[DataTypeId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaDictionary] WITH NOCHECK ADD 
	CONSTRAINT [PK_MetaDictionary] PRIMARY KEY  CLUSTERED 
	(
		[MetaDictionaryId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaField] WITH NOCHECK ADD 
	CONSTRAINT [PK_MetaField] PRIMARY KEY  CLUSTERED 
	(
		[MetaFieldId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaFileValue] WITH NOCHECK ADD 
	CONSTRAINT [PK_MetaFileValue] PRIMARY KEY  CLUSTERED 
	(
		[MetaKey]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaKey] WITH NOCHECK ADD 
	CONSTRAINT [PK_MetaKey] PRIMARY KEY  CLUSTERED 
	(
		[MetaKey]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaMultivalueDictionary] WITH NOCHECK ADD 
	CONSTRAINT [PK_MetaMultivalueDictionary] PRIMARY KEY  CLUSTERED 
	(
		[MetaKey],
		[MetaDictionaryId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaObjectValue] WITH NOCHECK ADD 
	CONSTRAINT [PK_MetaObjectValue] PRIMARY KEY  CLUSTERED 
	(
		[MetaKey]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaStringDictionaryValue] WITH NOCHECK ADD 
	CONSTRAINT [PK_MetaStringDictionaryValue] PRIMARY KEY  CLUSTERED 
	(
		[MetaDictionaryId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaClass] ADD 
	CONSTRAINT [DF_MetaClass_Namespace] DEFAULT (N'') FOR [Namespace],
	CONSTRAINT [DF_MetaClass_FriendlyName] DEFAULT (N'') FOR [FriendlyName],
	CONSTRAINT [DF_MetaClass_IsSystem] DEFAULT (0) FOR [IsSystem],
	CONSTRAINT [DF_MetaClass_IsAbstract] DEFAULT (0) FOR [IsAbstract],
	CONSTRAINT [DF_MetaClass_ParentClassId] DEFAULT (0) FOR [ParentClassId],
	CONSTRAINT [DF_MetaClass_PrimaryKeyName] DEFAULT (N'') FOR [PrimaryKeyName],
	CONSTRAINT [IX_MetaClass] UNIQUE  NONCLUSTERED 
	(
		[Name]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaClassMetaFieldRelation] ADD 
	CONSTRAINT [DF_MetaClassMetaFieldRelation_Weight] DEFAULT (0) FOR [Weight],
	CONSTRAINT [DF_MetaClassMetaFieldRelation_Enabled] DEFAULT (1) FOR [Enabled]
GO

ALTER TABLE [dbo].[MetaDataType] ADD 
	CONSTRAINT [DF_MetaDataType_Length] DEFAULT (0) FOR [Length],
	CONSTRAINT [DF_MetaDataType_AllowNulls] DEFAULT (1) FOR [AllowNulls],
	CONSTRAINT [DF_MetaDataType_Variable] DEFAULT (0) FOR [Variable],
	CONSTRAINT [DF_MetaDataType_IsSQLCommonType] DEFAULT (1) FOR [IsSQLCommonType],
	CONSTRAINT [DF_MetaDataType_DefaultValue] DEFAULT (N'') FOR [DefaultValue]
GO

ALTER TABLE [dbo].[MetaDictionary] ADD 
	CONSTRAINT [DF_MetaDictionary_Value] DEFAULT (N'') FOR [Value]
GO

ALTER TABLE [dbo].[MetaField] ADD 
	CONSTRAINT [DF_MetaField_Namespace] DEFAULT ('') FOR [Namespace],
	CONSTRAINT [DF_MetaField_SystemMetaClassId] DEFAULT (0) FOR [SystemMetaClassId],
	CONSTRAINT [DF_MetaField_Length] DEFAULT (0) FOR [Length],
	CONSTRAINT [DF_MetaField_AllowNulls] DEFAULT (1) FOR [AllowNulls],
	CONSTRAINT [DF_MetaField_SaveHistory] DEFAULT (0) FOR [SaveHistory],
	CONSTRAINT [DF_MetaField_MultiLanguageValue] DEFAULT (0) FOR [MultiLanguageValue],
	CONSTRAINT [IX_MetaField] UNIQUE  NONCLUSTERED 
	(
		[Name],
		[SystemMetaClassId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MetaFileValue] ADD 
	CONSTRAINT [DF_MetaFileValue_Size] DEFAULT (0) FOR [Size],
	CONSTRAINT [DF_MetaFileValue_CreationTime] DEFAULT (getdate()) FOR [CreationTime],
	CONSTRAINT [DF_MetaFileValue_LastWriteTime] DEFAULT (getdate()) FOR [LastWriteTime],
	CONSTRAINT [DF_MetaFileValue_LastReadTime] DEFAULT (getdate()) FOR [LastReadTime]
GO

ALTER TABLE [dbo].[MetaClassMetaFieldRelation] ADD 
	CONSTRAINT [FK_MetaClassMetaFieldRelation_MetaClass] FOREIGN KEY 
	(
		[MetaClassId]
	) REFERENCES [dbo].[MetaClass] (
		[MetaClassId]
	),
	CONSTRAINT [FK_MetaClassMetaFieldRelation_MetaField] FOREIGN KEY 
	(
		[MetaFieldId]
	) REFERENCES [dbo].[MetaField] (
		[MetaFieldId]
	)
GO

ALTER TABLE [dbo].[MetaDictionary] ADD 
	CONSTRAINT [FK_MetaDictionary_MetaField] FOREIGN KEY 
	(
		[MetaFieldId]
	) REFERENCES [dbo].[MetaField] (
		[MetaFieldId]
	)
GO

ALTER TABLE [dbo].[MetaField] ADD 
	CONSTRAINT [FK_MetaField_MetaDataType] FOREIGN KEY 
	(
		[DataTypeId]
	) REFERENCES [dbo].[MetaDataType] (
		[DataTypeId]
	)
GO

ALTER TABLE [dbo].[MetaFileValue] ADD 
	CONSTRAINT [FK_MetaFileValue_MetaKey] FOREIGN KEY 
	(
		[MetaKey]
	) REFERENCES [dbo].[MetaKey] (
		[MetaKey]
	)
GO

ALTER TABLE [dbo].[MetaMultivalueDictionary] ADD 
	CONSTRAINT [FK_MetaMultivalueDictionary_MetaKey] FOREIGN KEY 
	(
		[MetaKey]
	) REFERENCES [dbo].[MetaKey] (
		[MetaKey]
	)
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_AddMetaAttribute
	@AttrOwnerId		INT,
	@AttrOwnerType	INT,
	@Key			NVARCHAR(256),
	@Value			NTEXT
AS
	IF ( (SELECT COUNT(*) FROM MetaAttribute WHERE AttrOwnerId = @AttrOwnerId AND AttrOwnerType = @AttrOwnerType AND [Key] = @Key) = 0)
	BEGIN
		-- Insert
		INSERT INTO MetaAttribute (AttrOwnerId, AttrOwnerType, [Key], [Value] ) VALUES (@AttrOwnerId, @AttrOwnerType, @Key, @Value)
	END
	ELSE
	BEGIN
		-- Update
		UPDATE MetaAttribute SET [Value] = @Value  WHERE AttrOwnerId = @AttrOwnerId AND AttrOwnerType = @AttrOwnerType AND [Key] = @Key
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_AddMetaDictionary
	@MetaFieldId	INT,
	@Value		NVARCHAR(2048),
	@Retval	INT OUT	
AS
	SET NOCOUNT ON
	SET @Retval = -1

BEGIN TRAN

	INSERT INTO MetaDictionary (MetaFieldId, [Value]) VALUES (@MetaFieldId, @Value)

	IF @@ERROR <> 0 GOTO ERR

	SET @Retval = @@IDENTITY

	COMMIT TRAN
RETURN

ERR:
	SET @Retval = -1
	ROLLBACK TRAN
RETURN


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_AddMetaField 
	@Namespace 		NVARCHAR(1024) = N'Mediachase.MetaDataPlus.User',
	@Name		NVARCHAR(256),
	@FriendlyName	NVARCHAR(256),
	@Description	NTEXT,
	@DataTypeId	INT,
	@Length	INT,
	@AllowNulls	BIT,
	@SaveHistory	BIT,
	@MultiLanguageValue BIT, 
	@AllowSearch	BIT,
	@Retval 	INT OUTPUT
AS
	-- Step 0. Prepare
	SET NOCOUNT ON
	SET @Retval = -1

BEGIN TRAN
	DECLARE @DataTypeVariable	INT
	DECLARE @DataTypeLength	INT

	SELECT @DataTypeVariable = Variable, @DataTypeLength = Length FROM MetaDataType WHERE DataTypeId = @DataTypeId

	IF (@Length <= 0 OR @Length > @DataTypeLength )
		SET @Length = @DataTypeLength
		
	-- Step 2. Insert a record in to MetaField table.
	INSERT INTO [MetaField]  ([Namespace], [Name], [FriendlyName], [Description], [DataTypeId], [Length], [AllowNulls],  [SaveHistory], [MultiLanguageValue], [AllowSearch])
		VALUES(@Namespace, @Name,  @FriendlyName, @Description, @DataTypeId, @Length, @AllowNulls, @SaveHistory,@MultiLanguageValue, @AllowSearch)

	IF @@ERROR <> 0 GOTO ERR

	SET @Retval = @@IDENTITY

	COMMIT TRAN
RETURN

ERR:
	SET @Retval = -1
	ROLLBACK TRAN
RETURN


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_AddMetaFieldToMetaClass 
	@MetaClassId	INT,
	@MetaFieldId	INT,
	@Weight	INT
AS
	-- Step 0. Prepare
	SET NOCOUNT ON

	DECLARE @IsAbstractClass	BIT
	SELECT @IsAbstractClass = IsAbstract FROM MetaClass WHERE MetaClassId = @MetaClassId

BEGIN TRAN
	IF NOT EXISTS( SELECT * FROM MetaClass WHERE MetaClassId = @MetaClassId AND IsSystem = 0)
	BEGIN
		RAISERROR ('Wrong @MetaClassId. The class is system or not existed.', 16,1)
		GOTO ERR
	END

	IF NOT EXISTS( SELECT * FROM MetaField WHERE MetaFieldId = @MetaFieldId AND SystemMetaClassId = 0)
	BEGIN
		RAISERROR ('Wrong @MetaFieldId. The field is system or not existed.', 16,1)
		GOTO ERR
	END

	IF @IsAbstractClass = 0
	BEGIN
		-- Step 1. Insert a new column.
		DECLARE @Name		NVARCHAR(256)
		DECLARE @DataTypeId	INT
		DECLARE @Length		INT
		DECLARE @AllowNulls		BIT
		DECLARE @SaveHistory	BIT
		DECLARE @MultiLanguageValue BIT
		DECLARE @AllowSearch	BIT
	
		SELECT @Name = Name, @DataTypeId = DataTypeId,  @Length = Length, @AllowNulls = AllowNulls, 
			@SaveHistory = SaveHistory, @MultiLanguageValue = MultiLanguageValue, @AllowSearch = AllowSearch
		FROM MetaField WHERE MetaFieldId = @MetaFieldId AND SystemMetaClassId = 0 
	
		-- Step 1-1. Create a new column query.
	
		DECLARE @MetaClassTableName NVARCHAR(256)
		DECLARE @SqlDataTypeName NVARCHAR(256)
		DECLARE @IsVariableDataType BIT	
		DECLARE @DefaultValue	NVARCHAR(50)
	
		SELECT @MetaClassTableName = TableName FROM MetaClass WHERE MetaClassId = @MetaClassId
	
		IF @@ERROR<> 0 GOTO ERR
		
		SELECT @SqlDataTypeName = SqlName,  @IsVariableDataType = Variable, @DefaultValue = DefaultValue FROM MetaDataType WHERE DataTypeId= @DataTypeId
	
		IF @@ERROR<> 0 GOTO ERR
	
		DECLARE @ExecLine 		NVARCHAR(1024)
		DECLARE @ExecLineHistory 	NVARCHAR(1024)
		
		SET @ExecLine = 'ALTER TABLE [dbo].['+@MetaClassTableName+'] ADD ['+@Name+'] ' + @SqlDataTypeName
		SET @ExecLineHistory = 'ALTER TABLE [dbo].['+@MetaClassTableName+'_History] ADD ['+@Name+'] ' + @SqlDataTypeName
	
		IF @IsVariableDataType = 1 
		BEGIN
			SET @ExecLine = @ExecLine + ' (' + STR(@Length) + ')'
			SET @ExecLineHistory = @ExecLineHistory + ' (' + STR(@Length) + ')'
		END
	
		SET @ExecLineHistory = @ExecLineHistory + ' NULL'
	
		IF @AllowNulls = 1 
		BEGIN
			SET @ExecLine = @ExecLine + ' NULL'
			--SET @ExecLineHistory = @ExecLineHistory + ' NULL'
		END
		ELSE
			BEGIN
				SET @ExecLine = @ExecLine + ' NOT NULL DEFAULT ' + @DefaultValue 
				--SET @ExecLineHistory = @ExecLineHistory + ' NOT NULL DEFAULT ' + @DefaultValue
	
				--IF @IsVariableDataType = 1 
				--BEGIN
					--SET @ExecLine = @ExecLine + ' (' + STR(@Length) + ')'
					--SET @ExecLineHistory = @ExecLineHistory + ' (' + STR(@Length) + ')'
				--END
	
				SET @ExecLine = @ExecLine  +'  WITH VALUES' 
				--SET @ExecLineHistory = @ExecLineHistory  +'  WITH VALUES' 
			END
	
		--PRINT (@ExecLine)
		--PRINT (@ExecLineHistory)
	
		-- Step 1-2. Create a new column.
		EXEC (@ExecLine)
	
		IF @@ERROR<> 0 GOTO ERR
	
		-- Step 1-2. Create a new history column.
		EXEC (@ExecLineHistory)
	
		IF @@ERROR <> 0 GOTO ERR
	END

	-- Step 1-3. Create or Modify the Trigger 	mdpt_@MetaClassTableName_History

	-- Step 1-4. Create or Modify the View 	mdpv_@MetaClassTableName

	-- Step 2. Insert a record in to MetaClassMetaFieldRelation table.
	INSERT INTO [MetaClassMetaFieldRelation] (MetaClassId, MetaFieldId, Weight) VALUES(@MetaClassId, @MetaFieldId, @Weight)

	IF @@ERROR <> 0 GOTO ERR

	IF @IsAbstractClass = 0
	BEGIN
		EXEC mdpsp_sys_CreateMetaClassProcedure @MetaClassId
	
		IF @@ERROR <> 0 GOTO ERR
	
		EXEC mdpsp_sys_CreateMetaClassHistoryTrigger @MetaClassId
	
		IF @@ERROR <> 0 GOTO ERR
	END

	--EXEC mdpsp_sys_FullTextQueriesFieldUpdate @MetaClassId, @MetaFieldId,1
	
	--IF @@ERROR <> 0 GOTO ERR

	COMMIT TRAN

	IF @IsAbstractClass = 0
	BEGIN
		-- execute outside transaction
		EXEC mdpsp_sys_FullTextQueriesFieldUpdate @MetaClassId, @MetaFieldId,1
	END
RETURN

ERR:
	ROLLBACK TRAN
RETURN

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_AddMetaStringDictionary 
	@MetaKey	INT,
	@Key		NVARCHAR(512),
	@Value		NTEXT
AS
	-- Step 0. Prepare
	SET NOCOUNT ON

BEGIN TRAN

	INSERT INTO MetaStringDictionaryValue (MetaKey, [Key], [Value]) VALUES (@MetaKey, @Key, @Value)

COMMIT TRAN
RETURN

ERR:
ROLLBACK TRAN
RETURN


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_AddMultivalueDictionary 
	@MetaKey	INT,
	@MetaDictionaryId	INT
AS
	-- Step 0. Prepare
	SET NOCOUNT ON

BEGIN TRAN

	INSERT INTO MetaMultivalueDictionary (MetaKey, MetaDictionaryId) VALUES (@MetaKey, @MetaDictionaryId)

COMMIT TRAN
RETURN

ERR:
ROLLBACK TRAN
RETURN


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].mdpsp_sys_CheckReplaceUser 
	@OldUserId AS INT,
	@Retval AS INT OUTPUT
AS
	SET NOCOUNT ON

	SET @Retval = 0

	DECLARE classall_cursor CURSOR FOR
		SELECT MetaClassId, TableName FROM MetaClass WHERE IsSystem =0 AND IsAbstract = 0

	DECLARE @MetaClassId	INT
	DECLARE @TableName		NVARCHAR(255)

	OPEN classall_cursor	
	FETCH NEXT FROM classall_cursor INTO @MetaClassId, @TableName

	DECLARE @SQLString NVARCHAR(500)

	WHILE(@@FETCH_STATUS = 0 AND @Retval = 0)
	BEGIN

		SET @SQLString  = N'IF EXISTS(SELECT TOP 1 * FROM ' + @TableName  + ' WHERE CreatorId = @OldUserId) SELECT 1'
		EXEC sp_executesql @SQLString, N'@OldUserId AS INT', @OldUserId = @OldUserId
		IF @@ROWCOUNT <> 0 
		BEGIN
			SET @Retval = 1
			BREAK
		END

		SET @SQLString  = N'IF EXISTS(SELECT TOP 1 * FROM ' + @TableName  + ' WHERE ModifierId = @OldUserId) SELECT 1'
		EXEC sp_executesql @SQLString, N'@OldUserId AS INT', @OldUserId = @OldUserId
		IF @@ROWCOUNT <> 0 
		BEGIN
			SET @Retval = 1
			BREAK
		END

		SET @SQLString  = N'IF EXISTS(SELECT TOP 1 * FROM ' + @TableName  + '_History WHERE ModifierId = @OldUserId) SELECT 1'
		EXEC sp_executesql @SQLString, N'@OldUserId AS INT', @OldUserId = @OldUserId
		IF @@ROWCOUNT <> 0 
		BEGIN
			SET @Retval = 1
			BREAK
		END

		FETCH NEXT FROM classall_cursor INTO @MetaClassId, @TableName
	END
	
	CLOSE classall_cursor
	DEALLOCATE classall_cursor
RETURN
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_ClearMetaAttribute 
	@AttrOwnerId		INT,
	@AttrOwnerType	INT
AS
	DELETE FROM MetaAttribute WHERE AttrOwnerId = @AttrOwnerId AND AttrOwnerType = @AttrOwnerType


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_ClearMultivalueDictionary 
	@MetaKey	INT
AS
	DELETE FROM MetaMultivalueDictionary WHERE MetaKey = @MetaKey


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_ClearStringDictionary
	@MetaKey	INT
AS
	DELETE FROM MetaStringDictionaryValue WHERE MetaKey = @MetaKey


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_CreateMetaClass  
	@Namespace 		NVARCHAR(1024),
	@Name 		NVARCHAR(256),
	@FriendlyName		NVARCHAR(256),
	@TableName 		NVARCHAR(256),
	@ParentClassId 		INT,
	@IsSystem		BIT,
	@IsAbstract		BIT	=	0,
	@Description 		NTEXT,
	@Retval 		INT OUTPUT
AS
BEGIN
	-- Step 0. Prepare
	SET NOCOUNT ON
	SET @Retval = -1

BEGIN TRAN
	-- Step 1. Insert a new record in to the MetaClass table
	INSERT INTO [MetaClass] ([Namespace],[Name], [FriendlyName],[Description], [TableName], [ParentClassId], [IsSystem], [IsAbstract])
		VALUES (@Namespace, @Name, @FriendlyName, @Description, @TableName, @ParentClassId, @IsSystem, @IsAbstract)

	IF @@ERROR <> 0 GOTO ERR

	SET @Retval = @@IDENTITY

	IF @IsSystem = 1
	BEGIN
		IF NOT EXISTS(SELECT * FROM SYSOBJECTS WHERE [NAME] = @TableName AND [type] = 'U') 
		BEGIN
			RAISERROR ('Wrong System TableName.', 16,1 )
			GOTO ERR
		END

		-- Step 3-2. Insert a new record in to the MetaField table
		INSERT INTO [MetaField]  ([Namespace], [Name], [FriendlyName], [SystemMetaClassId], [DataTypeId], [Length], [AllowNulls],  [SaveHistory], [MultiLanguageValue], [AllowSearch])
			 SELECT @Namespace+ N'.' + @Name, SC .[name] , SC .[name] , @Retval ,MDT .[DataTypeId], SC .[length], SC .[isnullable], 0, 0, 0  FROM SYSCOLUMNS AS SC  
				INNER JOIN SYSOBJECTS SO ON SO.[ID] = SC.ID 
				INNER JOIN SYSTYPES ST ON ST.[xtype] = SC .[xtype]
				INNER JOIN MetaDataType MDT ON MDT.[Name] = ST .[name]
			WHERE SO.[ID]  = object_id( @TableName) and OBJECTPROPERTY( SO.[ID], N'IsTable') = 1 and ST.name<>'sysname'
	
		IF @@ERROR<> 0 GOTO ERR

		-- Step 3-2. Insert a new record in to the MetaClassMetaFieldRelation table
		INSERT INTO [MetaClassMetaFieldRelation]  (MetaClassId, MetaFieldId)
			SELECT @Retval, MetaFieldId FROM MetaField WHERE [SystemMetaClassId] = @Retval
	END
	ELSE
	BEGIN
		IF @IsAbstract = 0
		BEGIN
			-- Step 2. Create the @TableName table.
			EXEC('CREATE TABLE [dbo].[' + @TableName  + '] ([ObjectId] [int] NOT NULL , [CreatorId] [int]  , [Created] [datetime], [ModifierId] [int]  , [Modified] [datetime] ) ON [PRIMARY]')
		
			IF @@ERROR <> 0 GOTO ERR
		
			EXEC('ALTER TABLE [dbo].[' + @TableName  + '] WITH NOCHECK ADD CONSTRAINT [PK_' + @TableName  + '] PRIMARY KEY  CLUSTERED ([ObjectId])  ON [PRIMARY]') 
		
			IF @@ERROR <> 0 GOTO ERR
		
			-- Step 2-1. Create the @TableName_History table
			EXEC('CREATE TABLE [dbo].[' + @TableName  + '_History] ([Id] [int] IDENTITY (1, 1)  NOT NULL, [ObjectId] [int] NOT NULL , [ModifierId] [int]  ,	[Modified] [datetime] ) ON [PRIMARY]')
		
			IF @@ERROR <> 0 GOTO ERR
		
			EXEC('ALTER TABLE [dbo].[' + @TableName  + '_History] WITH NOCHECK ADD CONSTRAINT [PK_' + @TableName  + '_History] PRIMARY KEY  CLUSTERED ([Id])  ON [PRIMARY]') 
		
			IF @@ERROR<> 0 GOTO ERR
	
			IF EXISTS(SELECT * FROM MetaClass WHERE MetaClassId = @ParentClassId /* AND @IsSystem = 1 */ )
			BEGIN
				-- Step 3-2. Insert a new record in to the MetaClassMetaFieldRelation table
				INSERT INTO [MetaClassMetaFieldRelation]  (MetaClassId, MetaFieldId)
					SELECT @Retval, MetaFieldId FROM MetaField WHERE [SystemMetaClassId] = @ParentClassId
			END
			
			IF @@ERROR<> 0 GOTO ERR
	
			EXEC mdpsp_sys_CreateMetaClassProcedure @Retval
			IF @@ERROR <> 0 GOTO ERR
		END
	END

	-- Update PK Value
	DECLARE @PrimaryKeyName	NVARCHAR(256)
	SELECT @PrimaryKeyName = name FROM Sysobjects WHERE OBJECTPROPERTY(id, N'IsPrimaryKey') = 1 and parent_obj = OBJECT_ID(@TableName) and OBJECTPROPERTY(parent_obj, N'IsUserTable') = 1

	IF @PrimaryKeyName IS NOT NULL
		UPDATE [MetaClass] SET PrimaryKeyName = @PrimaryKeyName WHERE MetaClassId = @Retval

	COMMIT TRAN
RETURN 

ERR:
	ROLLBACK TRAN
	SET @Retval = -1
RETURN
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].mdpsp_sys_CreateMetaClassHistoryTrigger
	@MetaClassId	INT
AS
	SET NOCOUNT ON

BEGIN TRAN
	IF NOT EXISTS( SELECT * FROM MetaClass WHERE MetaClassId = @MetaClassId AND IsSystem = 0)
	BEGIN
		RAISERROR ('Wrong @MetaClassId. The class is system or not existed.', 16,1)
		GOTO ERR
	END

	-- Step 1. Create SQL Code
	--PRINT 'Step 1. Create SQL Code'

	DECLARE	@MetaClassTable			NVARCHAR(256)
	DECLARE	@MetaClassHistoryTrigger		NVARCHAR(256)

	SELECT @MetaClassTable = TableName FROM MetaClass WHERE MetaClassId = @MetaClassId

	SET @MetaClassHistoryTrigger 		= 'mdptr_avto_' +@MetaClassTable +'_History' 

	-- Step 2. Drop operation
	--PRINT'Step 2. Drop operation'

	if EXISTS (SELECT name FROM sysobjects WHERE name = @MetaClassHistoryTrigger AND type = 'TR')
		EXEC('DROP TRIGGER ' + @MetaClassHistoryTrigger)

	IF @@ERROR <> 0 GOTO ERR

	DECLARE	@MetaClassFieldList		NVARCHAR(4000)
	DECLARE	@MetaClassFieldListWithI	NVARCHAR(4000)
	DECLARE	@MetaClassFieldListCmpr	NVARCHAR(4000)
	DECLARE	@MetaClassFieldListIF		NVARCHAR(4000)

	SET @MetaClassFieldList = 'ObjectId, ModifierId, Modified'
	SET @MetaClassFieldListWithI = 'D.ObjectId, D.ModifierId, D.Modified'
	SET @MetaClassFieldListCmpr = '1<>1'
	SET @MetaClassFieldListIF = '1<>1'

	DECLARE field_cursor CURSOR FOR 
		SELECT MF.[Name] FROM MetaField MF 
			INNER JOIN MetaClassMetaFieldRelation MCFR ON MCFR.MetaFieldId = MF.MetaFieldId
		WHERE MCFR.MetaClassId = @MetaClassId AND MF.SystemMetaClassId = 0 AND MF.SaveHistory = 1 ORDER BY MCFR.Weight	

	DECLARE @Name 	NVARCHAR(256)

	OPEN field_cursor	
	FETCH NEXT FROM field_cursor INTO @Name 

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @MetaClassFieldList = @MetaClassFieldList + ', [' + @Name + ']'
		SET @MetaClassFieldListWithI = @MetaClassFieldListWithI + ', D.[' + @Name + ']'
		SET @MetaClassFieldListCmpr = @MetaClassFieldListCmpr + ' OR I.[' + @Name + '] <> D.[' + @Name + ']'
		SET @MetaClassFieldListIF = @MetaClassFieldListIF + ' OR UPDATE([' + @Name + '])'
	FETCH NEXT FROM field_cursor INTO @Name
	END

	CLOSE field_cursor
	DEALLOCATE field_cursor

--	IF @MetaClassFieldListCmpr<>'1<>1'
--	BEGIN
		SET QUOTED_IDENTIFIER OFF 
		SET ANSI_NULLS OFF 
	
		--PRINT('CREATE TRIGGER ' + @MetaClassHistoryTrigger + ' ON ' + @MetaClassTable + ' FOR INSERT, UPDATE AS' + ' DECLARE @rows INT ' +  
--			'SELECT @rows = @@rowcount IF @rows = 0 RETURN '  + 
--			'IF ' + @MetaClassFieldListIF +' BEGIN INSERT INTO ' +@MetaClassTable+ '_History (' + @MetaClassFieldList + ')  SELECT ' + @MetaClassFieldListWithI + ' FROM DELETED D JOIN INSERTED I ON (D.ObjectId = I.ObjectId) WHERE ' +  @MetaClassFieldListCmpr + ' END')


		EXEC('CREATE TRIGGER ' + @MetaClassHistoryTrigger + ' ON ' + @MetaClassTable + ' FOR INSERT, UPDATE AS' + ' DECLARE @rows INT ' +  
			'SELECT @rows = @@rowcount IF @rows = 0 RETURN '  + 
			'IF ' + @MetaClassFieldListIF +' BEGIN INSERT INTO ' +@MetaClassTable+ '_History (' + @MetaClassFieldList + ')  SELECT ' + @MetaClassFieldListWithI + ' FROM DELETED D JOIN INSERTED I ON (D.ObjectId = I.ObjectId) WHERE ' +  @MetaClassFieldListCmpr + ' END')

		IF @@ERROR <> 0 GOTO ERR
	
		SET QUOTED_IDENTIFIER OFF 
		SET ANSI_NULLS ON 
	
--	END

	COMMIT TRAN
	--PRINT('COMMIT TRAN')
RETURN

ERR:
	ROLLBACK TRAN
	--PRINT('ROLLBACK TRAN')
RETURN

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].mdpsp_sys_CreateMetaClassProcedure 
	@MetaClassId	INT
AS
	SET NOCOUNT ON

BEGIN TRAN
	IF NOT EXISTS( SELECT * FROM MetaClass WHERE MetaClassId = @MetaClassId)
	BEGIN
		RAISERROR ('Wrong @MetaClassId. The class is system or not existed.', 16,1)
		GOTO ERR
	END

	-- Step 1. Create SQL Code
	--PRINT'Step 1. Create SQL Code'

	DECLARE	@MetaClassTable			NVARCHAR(256)
	DECLARE	@MetaClassGetSpName			NVARCHAR(256)
	DECLARE	@MetaClassUpdateSpName		NVARCHAR(256)
	DECLARE	@MetaClassDeleteSpName		NVARCHAR(256)
	DECLARE	@MetaClassListSpName			NVARCHAR(256)
	DECLARE	@MetaClassHistorySpName		NVARCHAR(256)

	SELECT @MetaClassTable = TableName FROM MetaClass WHERE MetaClassId = @MetaClassId

	SET @MetaClassGetSpName 		= 'mdpsp_avto_' +@MetaClassTable +'_Get' 
	SET @MetaClassUpdateSpName 	= 'mdpsp_avto_' +@MetaClassTable +'_Update'
	SET @MetaClassDeleteSpName 		= 'mdpsp_avto_' +@MetaClassTable +'_Delete'
	SET @MetaClassListSpName 		= 'mdpsp_avto_' +@MetaClassTable +'_List'
	SET @MetaClassHistorySpName 	= 'mdpsp_avto_' +@MetaClassTable +'_History'

	-- Step 2. Drop operation
	--PRINT'Step 2. Drop operation'

	if exists (select * from dbo.sysobjects where id = object_id(@MetaClassGetSpName) and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		EXEC('drop procedure ' + @MetaClassGetSpName)
	IF @@ERROR <> 0 GOTO ERR

	if exists (select * from dbo.sysobjects where id = object_id(@MetaClassUpdateSpName) and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		EXEC('drop procedure ' + @MetaClassUpdateSpName)
	IF @@ERROR <> 0 GOTO ERR

	if exists (select * from dbo.sysobjects where id = object_id(@MetaClassDeleteSpName) and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		EXEC('drop procedure ' + @MetaClassDeleteSpName)
	IF @@ERROR <> 0 GOTO ERR

	if exists (select * from dbo.sysobjects where id = object_id(@MetaClassListSpName) and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		EXEC('drop procedure ' + @MetaClassListSpName)
	IF @@ERROR <> 0 GOTO ERR

	if exists (select * from dbo.sysobjects where id = object_id(@MetaClassHistorySpName) and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		EXEC('drop procedure ' + @MetaClassHistorySpName)
	IF @@ERROR <> 0 GOTO ERR

	-- Step 3. Create Procedure operation
	--PRINT'Step 3. ALTER Procedure operation'

	DECLARE	@MetaClassFieldList		NVARCHAR(4000)
	DECLARE	@MetaClassFieldListWithAt	NVARCHAR(4000)
	DECLARE	@MetaClassFieldListSet		NVARCHAR(4000)
	DECLARE	@MetaClassFieldListInsert	NVARCHAR(4000)

	SET @MetaClassFieldList = 'ObjectId, CreatorId, Created, ModifierId, Modified'
	SET @MetaClassFieldListWithAt = '@ObjectId INT, @CreatorId INT, @Created DATETIME, @ModifierId INT, @Modified DATETIME, @Retval INT OUT'
	SET @MetaClassFieldListSet = 'CreatorId = @CreatorId, Created = @Created, ModifierId = @ModifierId, Modified = @Modified'
	SET @MetaClassFieldListInsert = '@ObjectId, @CreatorId, @Created, @ModifierId, @Modified' 

	DECLARE field_cursor CURSOR FOR 
		SELECT MF.[Name], DT.SqlName, DT.Variable, MF.Length FROM MetaField MF 
			INNER JOIN MetaDataType DT ON DT.DataTypeId = MF.DataTypeId
			INNER JOIN MetaClassMetaFieldRelation MCFR ON MCFR.MetaFieldId = MF.MetaFieldId
		WHERE MCFR.MetaClassId = @MetaClassId AND MF.SystemMetaClassId = 0 ORDER BY MCFR.Weight	

	DECLARE @Name 	NVARCHAR(256)
	DECLARE @SqlName 	NVARCHAR(256)
	DECLARE @Variable 	BIT
	DECLARE @Length 	INT

	OPEN field_cursor	
	FETCH NEXT FROM field_cursor INTO @Name, @SqlName, @Variable, @Length

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @MetaClassFieldList = @MetaClassFieldList + ', '
		SET @MetaClassFieldListWithAt = @MetaClassFieldListWithAt + ', '
		SET @MetaClassFieldListSet = @MetaClassFieldListSet + ', '
		SET @MetaClassFieldListInsert = @MetaClassFieldListInsert + ', '
	
		SET @MetaClassFieldList = @MetaClassFieldList + '[' + @Name + ']'
		SET @MetaClassFieldListSet = @MetaClassFieldListSet + '[' +@Name + '] = @' + @Name 
		SET @MetaClassFieldListInsert = @MetaClassFieldListInsert + '@' + @Name 
	
		IF @Variable = 0
			SET @MetaClassFieldListWithAt = @MetaClassFieldListWithAt + '@' + @Name + ' ' + @SqlName 
		ELSE
			SET @MetaClassFieldListWithAt = @MetaClassFieldListWithAt + '@' + @Name + ' ' + @SqlName + '(' + STR(@Length) + ') ' 

	FETCH NEXT FROM field_cursor INTO @Name, @SqlName, @Variable, @Length
	END

	CLOSE field_cursor
	DEALLOCATE field_cursor

	SET QUOTED_IDENTIFIER OFF 
	SET ANSI_NULLS OFF 

	--PRINT 'CREATE PROCEDURE [dbo].[' + @MetaClassGetSpName + '] '  + '@ObjectId INT AS SELECT ' + @MetaClassFieldList + ' FROM ' +@MetaClassTable + ' WHERE ObjectId = @ObjectId'
	EXEC('CREATE PROCEDURE [dbo].[' + @MetaClassGetSpName + '] '  + '@ObjectId INT AS SELECT ' + @MetaClassFieldList + ' FROM ' +@MetaClassTable + ' WHERE ObjectId = @ObjectId')

	IF @@ERROR <> 0 GOTO ERR

/*	PRINT('CREATE PROCEDURE [dbo].[' + @MetaClassUpdateSpName + '] '  + @MetaClassFieldListWithAt + 
		' AS ' +
		' SET NOCOUNT ON ' +
		' BEGIN TRAN IF @ObjectId = -1 BEGIN SELECT @ObjectId = MAX(ObjectId)+1 FROM ' + @MetaClassTable + ' IF (@ObjectId IS NULL)  SET @ObjectId = 1 END ' +
		' SET @Retval = @ObjectId ' +
                           ' IF EXISTS(SELECT * FROM '+ @MetaClassTable +' WHERE ObjectId = @ObjectId  ) UPDATE ' + @MetaClassTable + ' SET ' + @MetaClassFieldListSet + ' WHERE ObjectId = @ObjectId' +
		' ELSE INSERT INTO ' + @MetaClassTable + ' ('+ @MetaClassFieldList + ') VALUES (' + @MetaClassFieldListInsert + ')' +
		' IF @@ERROR <> 0 GOTO ERR COMMIT TRAN RETURN ERR: ROLLBACK TRAN RETURN '
	)*/
	EXEC('CREATE PROCEDURE [dbo].[' + @MetaClassUpdateSpName + '] '  + @MetaClassFieldListWithAt + 
		' AS ' +
		' SET NOCOUNT ON ' +
		' BEGIN TRAN IF @ObjectId = -1 BEGIN SELECT @ObjectId = MAX(ObjectId)+1 FROM ' + @MetaClassTable + ' IF (@ObjectId IS NULL)  SET @ObjectId = 1 END ' +
		' SET @Retval = @ObjectId ' +
                           ' IF EXISTS(SELECT * FROM '+ @MetaClassTable +' WHERE ObjectId = @ObjectId  ) UPDATE ' + @MetaClassTable + ' SET ' + @MetaClassFieldListSet + ' WHERE ObjectId = @ObjectId' +
		' ELSE INSERT INTO ' + @MetaClassTable + ' ('+ @MetaClassFieldList + ') VALUES (' + @MetaClassFieldListInsert + ')' +
		' IF @@ERROR <> 0 GOTO ERR COMMIT TRAN RETURN ERR: ROLLBACK TRAN RETURN '
	)

	IF @@ERROR <> 0 GOTO ERR

	DECLARE @MetaClassIdSTR NVARCHAR(10)
	SET @MetaClassIdSTR = CAST( @MetaClassId AS NVARCHAR(10) )

	--PRINT('CREATE PROCEDURE [dbo].[' + @MetaClassDeleteSpName + ']  @ObjectId INT AS DELETE FROM '  + @MetaClassTable + ' WHERE ObjectId = @ObjectId')
	EXEC ('CREATE PROCEDURE [dbo].[' + @MetaClassDeleteSpName + ']  @ObjectId INT AS DELETE FROM '  + @MetaClassTable + ' WHERE ObjectId = @ObjectId DELETE FROM ' + @MetaClassTable +'_History WHERE ObjectId = @ObjectId ' +
		' EXEC mdpsp_sys_DeleteMetaKeyObjects '+@MetaClassIdSTR+', -1, @ObjectId ')

	IF @@ERROR <> 0 GOTO ERR

	--PRINT('CREATE PROCEDURE [dbo].[' + @MetaClassListSpName + '] '  + '@ObjectId INT AS SELECT ' + @MetaClassFieldList + ' FROM ' +@MetaClassTable )
	EXEC('CREATE PROCEDURE [dbo].[' + @MetaClassListSpName + '] '  + ' AS SELECT ' + @MetaClassFieldList + ' FROM ' +@MetaClassTable )
	IF @@ERROR <> 0 GOTO ERR

	EXEC('CREATE PROCEDURE [dbo].[' + @MetaClassHistorySpName + '] '  + ' @ObjectId INT  AS SELECT * FROM ' +@MetaClassTable + '_History WHERE ObjectId = @ObjectId' )
	IF @@ERROR <> 0 GOTO ERR

	SET QUOTED_IDENTIFIER OFF 
	SET ANSI_NULLS ON 

	COMMIT TRAN
	--PRINT('COMMIT TRAN')
RETURN

ERR:
	ROLLBACK TRAN
	--PRINT('ROLLBACK TRAN')
RETURN
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_CreateMetaClassProcedureAll 
AS
SET NOCOUNT ON
BEGIN TRAN
	DECLARE classall_cursor CURSOR FOR
		SELECT MetaClassId FROM MetaClass WHERE IsSystem =0 AND IsAbstract = 0

	DECLARE @MetaClassId	INT

	OPEN classall_cursor	
	FETCH NEXT FROM classall_cursor INTO @MetaClassId

	WHILE @@FETCH_STATUS = 0
	BEGIN
		--PRINT @MetaClassId
		EXEC  mdpsp_sys_CreateMetaClassProcedure @MetaClassId
		IF @@ERROR <> 0 GOTO ERR

	FETCH NEXT FROM classall_cursor INTO @MetaClassId
	END
	
	CLOSE classall_cursor
	DEALLOCATE classall_cursor

	COMMIT TRAN
RETURN

ERR:
	CLOSE classall_cursor
	DEALLOCATE classall_cursor

	ROLLBACK TRAN
RETURN

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_DeleteDContrainByTableAndField 
	@TableName	NVARCHAR(256),
	@FieldName	NVARCHAR(256)
AS
	SET NOCOUNT ON

	DECLARE @DConstrainName NVARCHAR(256)

	DECLARE DConstrainCursor CURSOR FOR 
		SELECT SO.Name FROM sysobjects SO
			LEFT JOIN syscolumns SC ON SC.ID = SO.parent_obj
			LEFT JOIN sysobjects ST ON SC.ID = ST.id
		WHERE (SC.Name = @FieldName AND ST.Name = @TableName AND ST.type='U' AND SO.type = 'D') 

	OPEN DConstrainCursor

	FETCH NEXT FROM DConstrainCursor  INTO @DConstrainName

	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC('ALTER TABLE [dbo].[' + @TableName +'] DROP  CONSTRAINT '+ @DConstrainName)
		--
		FETCH NEXT FROM DConstrainCursor  INTO @DConstrainName
	END

	CLOSE DConstrainCursor
	DEALLOCATE DConstrainCursor


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_DeleteMetaAttribute 
	@AttrOwnerId		INT,
	@AttrOwnerType	INT,
	@Key			NVARCHAR(512)
AS
	DELETE FROM MetaAttribute WHERE AttrOwnerId = @AttrOwnerId AND AttrOwnerType = @AttrOwnerType AND [Key] = @Key


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].mdpsp_sys_DeleteMetaClass 
	@MetaClassId	INT
AS
	
	-- Step 0. Prepare
	SET NOCOUNT ON

	BEGIN TRAN

	DECLARE @MetaFieldOwnerTable	NVARCHAR(256)

	-- Check Childs Table
	IF EXISTS(SELECT *  FROM MetaClass MC WHERE ParentClassId = @MetaClassId)
	BEGIN
		RAISERROR ('The class have childs.', 16, 1)
		GOTO ERR
	END
	
	-- Step 1. Find a TableName
	IF EXISTS(SELECT *  FROM MetaClass MC WHERE MetaClassId = @MetaClassId)
	BEGIN
		IF EXISTS(SELECT *  FROM MetaClass MC WHERE MetaClassId = @MetaClassId AND IsSystem = 0 AND IsAbstract = 0)
		BEGIN
			SELECT @MetaFieldOwnerTable = TableName  FROM MetaClass MC WHERE MetaClassId = @MetaClassId AND IsSystem = 0 AND IsAbstract = 0
			
			IF @@ERROR <> 0 GOTO ERR

			EXEC mdpsp_sys_DeleteMetaClassProcedure @MetaClassId

			IF @@ERROR <> 0 GOTO ERR
	
			-- Step 2. Delete Table
			EXEC('DROP TABLE [dbo].[' + @MetaFieldOwnerTable + ']')
			
			IF @@ERROR <> 0 GOTO ERR
	
			EXEC('DROP TABLE [dbo].[' + @MetaFieldOwnerTable + '_History]')
	
			IF @@ERROR <> 0 GOTO ERR

			-- Delete Meta Dictionary Relations
			--DELETE FROM MetaMultivalueDictionary  WHERE MetaKey IN 
			--	(SELECT MK.MetaKey FROM MetaKey MK WHERE MK.MetaClassId = @MetaClassId)
		
			-- IF @@ERROR <> 0 GOTO ERR
		
			-- Delete Meta File
			--DELETE FROM MetaFileValue  WHERE MetaKey IN 
			--	(SELECT MK.MetaKey FROM MetaKey MK WHERE MK.MetaClassId = @MetaClassId)
		
			-- IF @@ERROR <> 0 GOTO ERR
		
			-- Delete Meta Key
			--DELETE FROM MetaKey WHERE MetaClassId = @MetaClassId
		
			EXEC mdpsp_sys_DeleteMetaKeyObjects @MetaClassId
			 IF @@ERROR <> 0 GOTO ERR

			-- Delete Meta Attribute
			EXEC mdpsp_sys_ClearMetaAttribute @MetaClassId, 1
		
			 IF @@ERROR <> 0 GOTO ERR
	
			-- Step 3. Delete MetaField Relations
			DELETE FROM MetaClassMetaFieldRelation WHERE MetaClassId = @MetaClassId
	
			IF @@ERROR <> 0 GOTO ERR
	
			-- Step 3. Delete MetaClass
			DELETE FROM MetaClass WHERE MetaClassId = @MetaClassId
	
			IF @@ERROR <> 0 GOTO ERR
		END
		ELSE
		BEGIN
			-- Delete Meta Attribute
			EXEC mdpsp_sys_ClearMetaAttribute @MetaClassId, 1
		
			 IF @@ERROR <> 0 GOTO ERR

			-- Step 3. Delete MetaField Relations
			DELETE FROM MetaClassMetaFieldRelation WHERE MetaClassId = @MetaClassId

			IF @@ERROR <> 0 GOTO ERR

			-- Step 3. Delete MetaField
			DELETE FROM MetaField WHERE SystemMetaClassId = @MetaClassId
	
			IF @@ERROR <> 0 GOTO ERR
	
			-- Step 3. Delete MetaClass
			DELETE FROM MetaClass WHERE MetaClassId = @MetaClassId
	
			IF @@ERROR <> 0 GOTO ERR
			
		END
	END
	ELSE 
	BEGIN
		RAISERROR ('Wrong @MetaClassId.', 16, 1)
		GOTO ERR
	END

	COMMIT TRAN
	RETURN

ERR:
	ROLLBACK TRAN
	RETURN
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].mdpsp_sys_DeleteMetaClassProcedure 
	@MetaClassId	INT
AS
	SET NOCOUNT ON

BEGIN TRAN
	IF NOT EXISTS( SELECT * FROM MetaClass WHERE MetaClassId = @MetaClassId)
	BEGIN
		RAISERROR ('Wrong @MetaClassId. The class is system or not existed.', 16,1)
		GOTO ERR
	END

	-- Step 1. Create SQL Code
	PRINT'Step 1. Create SQL Code'

	DECLARE	@MetaClassTable			NVARCHAR(256)
	DECLARE	@MetaClassGetSpName			NVARCHAR(256)
	DECLARE	@MetaClassUpdateSpName		NVARCHAR(256)
	DECLARE	@MetaClassDeleteSpName		NVARCHAR(256)
	DECLARE	@MetaClassListSpName		NVARCHAR(256)
	DECLARE	@MetaClassHistorySpName		NVARCHAR(256)

	SELECT @MetaClassTable = TableName FROM MetaClass WHERE MetaClassId = @MetaClassId

	SET @MetaClassGetSpName 		= 'mdpsp_avto_' +@MetaClassTable +'_Get' 
	SET @MetaClassUpdateSpName 	= 'mdpsp_avto_' +@MetaClassTable +'_Update'
	SET @MetaClassDeleteSpName 	= 'mdpsp_avto_' +@MetaClassTable +'_Delete'
	SET @MetaClassListSpName 	= 'mdpsp_avto_' +@MetaClassTable +'_List'
	SET @MetaClassHistorySpName 	= 'mdpsp_avto_' +@MetaClassTable +'_History'

	-- Step 2. Drop operation
	PRINT'Step 2. Drop operation'

	if exists (select * from dbo.sysobjects where id = object_id(@MetaClassUpdateSpName) and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		EXEC('drop procedure ' + @MetaClassUpdateSpName)
	IF @@ERROR <> 0 GOTO ERR

	if exists (select * from dbo.sysobjects where id = object_id(@MetaClassGetSpName) and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		EXEC('drop procedure ' + @MetaClassGetSpName)
	IF @@ERROR <> 0 GOTO ERR

	if exists (select * from dbo.sysobjects where id = object_id(@MetaClassDeleteSpName) and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		EXEC('drop procedure ' + @MetaClassDeleteSpName)
	IF @@ERROR <> 0 GOTO ERR

	if exists (select * from dbo.sysobjects where id = object_id(@MetaClassListSpName) and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		EXEC('drop procedure ' + @MetaClassListSpName)
	IF @@ERROR <> 0 GOTO ERR

	if exists (select * from dbo.sysobjects where id = object_id(@MetaClassHistorySpName) and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		EXEC('drop procedure ' + @MetaClassHistorySpName)
	IF @@ERROR <> 0 GOTO ERR


	COMMIT TRAN
	--PRINT('COMMIT TRAN')
RETURN

ERR:
	ROLLBACK TRAN
	--PRINT('ROLLBACK TRAN')
RETURN
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_DeleteMetaDictionary
	@MetaDictionaryId	INT
AS
BEGIN
	-- Step 0. Prepare
	SET NOCOUNT ON

	BEGIN TRAN

	IF NOT EXISTS(SELECT * FROM MetaDictionary WHERE MetaDictionaryId = @MetaDictionaryId)
	BEGIN
		RAISERROR ('Wrong @MetaDictionaryId.', 16, 1)
		GOTO ERR
	END

	DELETE FROM MetaDictionary WHERE MetaDictionaryId = @MetaDictionaryId

	IF @@ERROR <> 0 GOTO ERR

	COMMIT TRAN
	RETURN
ERR:
	ROLLBACK TRAN
	RETURN @@Error
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_DeleteMetaField 
	@MetaFieldId	INT
AS
BEGIN
	-- Step 0. Prepare
	SET NOCOUNT ON

	BEGIN TRAN

	IF NOT EXISTS(SELECT * FROM MetaClassMetaFieldRelation WHERE MetaFieldId = @MetaFieldId)
	BEGIN
		-- Step 5. Delete Dictionary Record
		DELETE FROM MetaDictionary WHERE MetaFieldId = @MetaFieldId
	
		IF @@ERROR <> 0 GOTO ERR

		-- Step 5. Delete Field Info Record
		DELETE FROM MetaField WHERE MetaFieldId = @MetaFieldId
	
		IF @@ERROR <> 0 GOTO ERR

		EXEC mdpsp_sys_ClearMetaAttribute @MetaFieldId, 2

		IF @@ERROR <> 0 GOTO ERR
	END
	ELSE 
	BEGIN
		RAISERROR ('The MetaClass have got a link to @MetaFieldId.', 16, 1)
		GOTO ERR
	END

	COMMIT TRAN
	RETURN
ERR:
	ROLLBACK TRAN
	RETURN @@Error
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE [dbo].mdpsp_sys_DeleteMetaFieldFromMetaClass
	@MetaClassId	INT,
	@MetaFieldId	INT
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM MetaClassMetaFieldRelation WHERE MetaFieldId = @MetaFieldId AND MetaClassId = @MetaClassId)
	BEGIN
		--RAISERROR ('Wrong @MetaFieldId and @MetaClassId.', 16, 1)
		-- GOTO ERR
		RETURN
	END

	-- Step 0. Prepare
	SET NOCOUNT ON

	DECLARE @MetaFieldName		NVARCHAR(256)
	DECLARE @MetaFieldOwnerTable	NVARCHAR(256)
	DECLARE @IsAbstractClass		BIT
	
	-- Step 1. Find a Field Name
	-- Step 2. Find a TableName
	IF NOT EXISTS(SELECT * FROM MetaField MF WHERE MetaFieldId = @MetaFieldId AND SystemMetaClassId = 0 )
	BEGIN
		RAISERROR ('Wrong @MetaFieldId.', 16, 1)
		GOTO ERR
	END

	SELECT @MetaFieldName = MF.[Name] FROM MetaField MF WHERE MetaFieldId = @MetaFieldId AND SystemMetaClassId = 0

	IF NOT EXISTS(SELECT * FROM MetaClass MC WHERE MetaClassId = @MetaClassId AND IsSystem = 0)
	BEGIN
		RAISERROR ('Wrong @MetaClassId.', 16, 1)
		GOTO ERR
	END

	SELECT @MetaFieldOwnerTable = MC.TableName, @IsAbstractClass = MC.IsAbstract FROM MetaClass MC WHERE MetaClassId = @MetaClassId AND IsSystem = 0

	 IF @@ERROR <> 0 GOTO ERR

	IF @IsAbstractClass = 0
	BEGIN
		-- need to remove full text indexes before removing item
		EXEC mdpsp_sys_FullTextQueriesFieldUpdate @MetaClassId, @MetaFieldId, 0
	END
	
	BEGIN TRAN

	IF @IsAbstractClass = 0
	BEGIN
		EXEC mdpsp_sys_DeleteMetaKeyObjects @MetaClassId, @MetaFieldId
		 IF @@ERROR <> 0 GOTO ERR

		-- Delete Meta Dictionary Relations
		--DELETE FROM MetaMultivalueDictionary  WHERE MetaKey IN 
		--	(SELECT MK.MetaKey FROM MetaKey MK WHERE MK.MetaFieldId = @MetaFieldId AND MK.MetaClassId = @MetaClassId)
	
		-- IF @@ERROR <> 0 GOTO ERR
	
		-- Delete Meta File
		--DELETE FROM MetaFileValue  WHERE MetaKey IN 
		--	(SELECT MK.MetaKey FROM MetaKey MK WHERE MK.MetaFieldId = @MetaFieldId AND MK.MetaClassId = @MetaClassId)
	
		-- IF @@ERROR <> 0 GOTO ERR
	
		--DELETE FROM MetaKey WHERE MetaFieldId = @MetaFieldId AND MetaClassId = @MetaClassId
	
		-- IF @@ERROR <> 0 GOTO ERR
		
		-- Step 3. Delete Constrains
		EXEC mdpsp_sys_DeleteDContrainByTableAndField @MetaFieldOwnerTable, @MetaFieldName
	
		IF @@ERROR <> 0 GOTO ERR
	
		-- Step 4. Delete Field
		EXEC ('ALTER TABLE ['+@MetaFieldOwnerTable+'] DROP COLUMN [' + @MetaFieldName + ']')
	
		IF @@ERROR <> 0 GOTO ERR
	
		SET @MetaFieldOwnerTable = @MetaFieldOwnerTable + '_History'
	
		-- Step 3. Delete History Constrains
		EXEC mdpsp_sys_DeleteDContrainByTableAndField @MetaFieldOwnerTable, @MetaFieldName
	
		IF @@ERROR <> 0 GOTO ERR
	
		-- Step 4. Delete History Field
		EXEC ('ALTER TABLE ['+@MetaFieldOwnerTable+'] DROP COLUMN [' + @MetaFieldName + ']')
	
		IF @@ERROR <> 0 GOTO ERR
	END

	-- Step 5. Delete Field Info Record
	DELETE FROM MetaClassMetaFieldRelation WHERE MetaFieldId = @MetaFieldId AND MetaClassId = @MetaClassId
	IF @@ERROR <> 0 GOTO ERR

	IF @IsAbstractClass = 0
	BEGIN
		EXEC mdpsp_sys_CreateMetaClassProcedure @MetaClassId
	
		IF @@ERROR <> 0 GOTO ERR
	
		EXEC mdpsp_sys_CreateMetaClassHistoryTrigger @MetaClassId
	
		IF @@ERROR <> 0 GOTO ERR
	
		--EXEC mdpsp_sys_FullTextQueriesFieldUpdate @MetaClassId, @MetaFieldId, 0
		
		--IF @@ERROR <> 0 GOTO ERR
	END

	COMMIT TRAN
	RETURN
ERR:
	ROLLBACK TRAN

	-- readd indexes if error occured
	IF @IsAbstractClass = 0
	BEGIN
		EXEC mdpsp_sys_FullTextQueriesFieldUpdate @MetaClassId, @MetaFieldId, 1
	END

	RETURN @@Error
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_DeleteMetaFile 
	@MetaKey	INT
AS
	SET NOCOUNT ON
	DELETE FROM MetaFileValue WHERE MetaKey = @MetaKey


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].mdpsp_sys_DeleteMetaKeyObjects
	@MetaClassId	INT,
	@MetaFieldId	INT	=	-1,
	@MetaObjectId	INT	=	-1
AS
	-- Delete MetaObjectValue
	DELETE FROM MetaObjectValue  WHERE MetaKey IN 
		(SELECT MK.MetaKey FROM MetaKey MK WHERE 
			(@MetaObjectId = MK.MetaObjectId OR @MetaObjectId = -1)  AND 
			(@MetaClassId = MK.MetaClassId OR @MetaClassId = -1) AND
			(@MetaFieldId = MK.MetaFieldId  OR @MetaFieldId = -1)
		)

	 IF @@ERROR <> 0 GOTO ERR

	-- Delete MetaStringDictionaryValue
	DELETE FROM MetaStringDictionaryValue  WHERE MetaKey IN 
		(SELECT MK.MetaKey FROM MetaKey MK WHERE 
			(@MetaObjectId = MK.MetaObjectId OR @MetaObjectId = -1)  AND 
			(@MetaClassId = MK.MetaClassId OR @MetaClassId = -1) AND
			(@MetaFieldId = MK.MetaFieldId  OR @MetaFieldId = -1)
		)

	 IF @@ERROR <> 0 GOTO ERR

	-- Delete MetaMultivalueDictionary
	DELETE FROM MetaMultivalueDictionary  WHERE MetaKey IN 
		(SELECT MK.MetaKey FROM MetaKey MK WHERE 
			(@MetaObjectId = MK.MetaObjectId OR @MetaObjectId = -1)  AND 
			(@MetaClassId = MK.MetaClassId OR @MetaClassId = -1) AND
			(@MetaFieldId = MK.MetaFieldId  OR @MetaFieldId = -1)
		)
		
	 IF @@ERROR <> 0 GOTO ERR
		
	-- Delete Meta File
	DELETE FROM MetaFileValue  WHERE MetaKey IN 
		(SELECT MK.MetaKey FROM MetaKey MK WHERE 
			(@MetaObjectId = MK.MetaObjectId OR @MetaObjectId = -1)  AND 
			(@MetaClassId = MK.MetaClassId OR @MetaClassId = -1) AND
			(@MetaFieldId = MK.MetaFieldId  OR @MetaFieldId = -1)
		)

	 IF @@ERROR <> 0 GOTO ERR

	-- Clear Meta Key
	DELETE FROM MetaKey  WHERE 
		(@MetaObjectId = MetaObjectId OR @MetaObjectId = -1)  AND 
		(@MetaClassId = MetaClassId OR @MetaClassId = -1) AND
		(@MetaFieldId = MetaFieldId  OR @MetaFieldId = -1)

ERR:
	RETURN
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_DeleteMetaObjectValue 
	@MetaKey	INT
AS
	SET NOCOUNT ON
	DELETE FROM MetaObjectValue WHERE MetaKey = @MetaKey


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].[mdpsp_sys_DeleteMetaRule]
	@RuleId	INT
AS
	SET NOCOUNT ON

	DELETE FROM MetaRule WHERE RuleId=@RuleId

	SET NOCOUNT OFF
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_FullTextQueriesActivate
AS
	-- Step 1. Use SQL Server Service Manager to verify the full-text service, Microsoft Search, is running. 

	-- Step 3. If not enabled (which is the default for newly created databases), enable the pubs database for full-text processing. Using pubs, execute this stored procedure: 
	EXEC sp_fulltext_database  'enable'
	-- Step 4. Create a full-text catalog named MetaDataFullTextQueriesCatalog, opting for the default directory, by executing this stored procedure in the pubs database:  
	EXEC sp_fulltext_catalog 'MetaDataFullTextQueriesCatalog', 'create'    

	EXEC sp_fulltext_catalog  'MetaDataFullTextQueriesCatalog',  'start_full'


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_FullTextQueriesAddAllFields 
AS
	DECLARE field_w_search CURSOR FOR
		SELECT MCMFR.MetaClassId, MCMFR.MetaFieldId FROM MetaClassMetaFieldRelation MCMFR
			INNER JOIN MetaField MF ON MCMFR.MetaFieldId = MF.MetaFieldId
		WHERE MF.AllowSearch = 1 AND (MF.SystemMetaClassId = 0 OR MF.SystemMetaClassId = MCMFR.MetaClassId)

	DECLARE @MetaClassId INT
	DECLARE @MetaFieldId  INT
	
	OPEN field_w_search	
	FETCH NEXT FROM field_w_search INTO @MetaClassId, @MetaFieldId

	WHILE @@FETCH_STATUS = 0
	BEGIN

		EXEC mdpsp_sys_FullTextQueriesFieldUpdate @MetaClassId, @MetaFieldId, 1

	FETCH NEXT FROM field_w_search INTO @MetaClassId, @MetaFieldId
	END

	CLOSE field_w_search
	DEALLOCATE field_w_search


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_FullTextQueriesDeactivate
AS
	DECLARE TableInFulltextCatalog	CURSOR FOR
		--SELECT TableName FROM MetaClass WHERE IsSystem = 0 AND OBJECTPROPERTY (OBJECT_ID(TableName), 'TableFulltextCatalogId') <> 0
		SELECT SO.[Name] as TableName FROM dbo.sysobjects SO INNER JOIN sysfulltextcatalogs SFTC ON SFTC.ftcatid = SO.ftcatid where OBJECTPROPERTY(id, N'IsTable') = 1 AND SFTC.[Name] = 'MetaDataFullTextQueriesCatalog' 

	DECLARE @TableName 	NVARCHAR(256)

	OPEN TableInFulltextCatalog	

	FETCH NEXT FROM TableInFulltextCatalog INTO @TableName 
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--PRINT('sp_fulltext_table Drop = ' + @TableName)
		EXEC sp_fulltext_table @TableName, 'Drop'

	FETCH NEXT FROM TableInFulltextCatalog INTO @TableName
	END

	CLOSE  TableInFulltextCatalog
	DEALLOCATE TableInFulltextCatalog

	-- This action fails if this catalog contains indexes for one or more tables.
	EXEC sp_fulltext_catalog 'MetaDataFullTextQueriesCatalog', 'drop'    

	-- disable
	EXEC sp_fulltext_database  'disable'


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_FullTextQueriesDeleteAllFields 
AS
	DECLARE field_w_search CURSOR FOR
		SELECT MCMFR.MetaClassId, MCMFR.MetaFieldId FROM MetaClassMetaFieldRelation MCMFR
			INNER JOIN MetaField MF ON MCMFR.MetaFieldId = MF.MetaFieldId
		WHERE MF.AllowSearch = 1

	DECLARE @MetaClassId INT
	DECLARE @MetaFieldId  INT
	
	OPEN field_w_search	
	FETCH NEXT FROM field_w_search INTO @MetaClassId, @MetaFieldId

	WHILE @@FETCH_STATUS = 0
	BEGIN

		EXEC mdpsp_sys_FullTextQueriesFieldUpdate @MetaClassId, @MetaFieldId, 0

	FETCH NEXT FROM field_w_search INTO @MetaClassId, @MetaFieldId
	END

	CLOSE field_w_search
	DEALLOCATE field_w_search


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].mdpsp_sys_FullTextQueriesEnable 
AS
	SELECT CASE FULLTEXTSERVICEPROPERTY( 'IsFullTextInstalled' )
	WHEN 1 THEN 
		CASE DatabaseProperty (DB_NAME(DB_ID()),  'IsFulltextEnabled')
		WHEN 1 THEN 1
		ELSE 0
		END
	ELSE 0
	END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].mdpsp_sys_FullTextQueriesFieldUpdate
	@MetaClassId 	INT,
	@MetaFieldId	INT,
	@Add		BIT
AS
	DECLARE 	@IsFulltextEnabled INT

	--SELECT @IsFulltextEnabled = DatabaseProperty (DB_NAME(DB_ID()),  'IsFulltextEnabled' )
	SELECT @IsFulltextEnabled = (CASE FULLTEXTSERVICEPROPERTY( 'IsFullTextInstalled' ) WHEN 1 THEN CASE DatabaseProperty (DB_NAME(DB_ID()),  'IsFulltextEnabled') WHEN 1 THEN 1 ELSE 0 END ELSE 0 END)

	IF @IsFulltextEnabled = 0
		RETURN

	DECLARE @TableName		NVARCHAR(256)
	SELECT @TableName	= TableName FROM MetaClass WHERE MetaClassId = @MetaClassId --AND IsSystem = 0

	DECLARE @FieldName		NVARCHAR(256)
	SELECT @FieldName	= MF.Name FROM MetaField MF
		INNER JOIN MetaDataType MDT ON MDT.DataTypeId = MF.DataTypeId
	WHERE MF.MetaFieldId = @MetaFieldId AND MDT.SqlName IN (N'char', N'nchar', N'varchar', N'nvarchar', N'text', N'ntext', N'image')

	IF (@TableName IS NULL) OR (@FieldName IS NULL)
		RETURN
	
	DECLARE @TableFulltextCatalogId	INT

	SELECT @TableFulltextCatalogId = OBJECTPROPERTY (OBJECT_ID(TableName), 'TableFulltextCatalogId') FROM MetaClass WHERE MetaClassId = @MetaClassId

	IF @TableFulltextCatalogId = 0 
	BEGIN
		DECLARE	@PK_Table NVARCHAR(256)

		SELECT 	@PK_Table = PrimaryKeyName FROM MetaClass WHERE MetaClassId = @MetaClassId

		EXEC sp_fulltext_table  @TableName, 'create', 'MetaDataFullTextQueriesCatalog', @PK_Table

		-- Step 2. Add new meta field
		IF @Add = 1
			EXEC sp_fulltext_column @TableName,  @FieldName,  'add'
	
	END
	ELSE
	BEGIN
		-- Step 1. Deactivate the table so that the column can be added to the full-text index or the column can be removed by executing this stored procedure: 
		EXEC sp_fulltext_table  @TableName,  'deactivate'

		-- Step 2. Add new meta field
		IF @Add = 1
			EXEC sp_fulltext_column @TableName,  @FieldName,  'add'
		ELSE
			EXEC sp_fulltext_column @TableName,  @FieldName,  'drop'
	END

	-- Calculate Count of Searchable Fileds.
	DECLARE @SFCount	INT	
	SELECT @SFCount = SUM(COLUMNPROPERTY ( OBJECT_ID(@TableName), [name],  'IsFullTextIndexed' )) FROM MetaField 

	-- Step 4. Reactivate the table using this stored procedure: 
	IF @SFCount = 0
	BEGIN
		--PRINT('Drop Table: ' + @TableName)
		EXEC sp_fulltext_table  @TableName,  'drop'
	END
	ELSE
	BEGIN
		--PRINT('Activate Table: ' + @TableName)
		EXEC sp_fulltext_table @TableName,  'activate'
	END

	-- Step 5. Start an incremental population of the full-text catalog by executing this stored procedure:
	EXEC sp_fulltext_catalog  'MetaDataFullTextQueriesCatalog',  'start_full'
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_GetMetaKey 
	@MetaObjectId	INT,
	@MetaClassId	INT,
	@MetaFieldId	INT,
	@Retval	INT	OUT
AS
	SET NOCOUNT ON

	IF EXISTS(SELECT * FROM MetaKey WHERE MetaObjectId = @MetaObjectId AND MetaClassId = @MetaClassId AND MetaFieldId = @MetaFieldId)
	BEGIN
		SELECT @RetVal = MetaKey FROM MetaKey WHERE MetaObjectId = @MetaObjectId AND MetaClassId = @MetaClassId AND MetaFieldId = @MetaFieldId
	END
	ELSE
	BEGIN
		INSERT INTO MetaKey (MetaObjectId, MetaClassId, MetaFieldId) VALUES (@MetaObjectId, @MetaClassId, @MetaFieldId)
		SET @Retval = @@IDENTITY
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].mdpsp_sys_GetMetaKeyInfo
	@MetaKey	INT
AS
	SET NOCOUNT ON
	SELECT * FROM MetaKey WHERE MetaKey = @MetaKey
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE [dbo].mdpsp_sys_GetUniqueFieldName 
	@Name		NVARCHAR(256),
	@UniqueName 	NVARCHAR(256) OUT
AS
	SET NOCOUNT OFF

	DECLARE	@Index		INT

	SET @UniqueName 	= @Name
	SET @Index		= (SELECT COUNT(*) FROM MetaField WHERE SystemMetaClassId = 0 AND Name LIKE @Name + '[0123456789]%')

	WHILE (SELECT COUNT(*) FROM MetaField WHERE SystemMetaClassId = 0 AND Name=@UniqueName) <> 0
	BEGIN
		SET @UniqueName = @Name + CAST(@Index  AS NVARCHAR(32))
		SET @Index = @Index + 1
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_LoadChildMetaClassList
	@MetaClassId	INT
AS
	SELECT MetaClassId, Namespace,Name, [FriendlyName], IsSystem, IsAbstract, ParentClassId, TableName, Description, FieldListChangedSqlScript, Tag 
	FROM MetaClass WHERE ParentClassId = @MetaClassId

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaAttributes 
	@AttrOwnerId		INT,
	@AttrOwnerType	INT
AS
	SELECT [Key], [Value] FROM MetaAttribute WHERE AttrOwnerId = @AttrOwnerId AND AttrOwnerType = @AttrOwnerType


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaClassById 
	@MetaClassId	INT
AS
	SELECT MetaClassId, Namespace, Name, FriendlyName, IsSystem, IsAbstract, ParentClassId, TableName, Description, FieldListChangedSqlScript, Tag
	FROM MetaClass WHERE MetaClassId = @MetaClassId

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaClassByName
	@Name		NVARCHAR(256)
AS
	SELECT MetaClassId, Namespace, Name, FriendlyName, IsSystem, IsAbstract,ParentClassId, TableName, Description, FieldListChangedSqlScript, Tag
	FROM MetaClass WHERE Name = @Name

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaClassByNamespace
	@Namespace		NVARCHAR(1024),
	@Deep			BIT
AS
	IF @Deep = 1
		SELECT MetaClassId, Namespace, Name, FriendlyName, IsSystem, IsAbstract, ParentClassId, TableName, Description, FieldListChangedSqlScript, Tag
		FROM MetaClass WHERE  Namespace = @Namespace OR Namespace LIKE (@Namespace + '.%')
	ELSE
		SELECT MetaClassId, Namespace, Name, FriendlyName, IsSystem, IsAbstract, ParentClassId, TableName, Description, FieldListChangedSqlScript, Tag
		FROM MetaClass WHERE Namespace = @Namespace

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaClassList
AS

SELECT C.MetaClassId, C.Namespace, C.[Name], C.FriendlyName, C.IsSystem, C.IsAbstract, C.ParentClassId, C.TableName, C.[Description], C.FieldListChangedSqlScript, C.Tag, 
	P.[Name] AS ParentName, P.TableName AS ParentTableName, P.FriendlyName AS ParentFriendlyName
  FROM MetaClass C
	LEFT JOIN MetaClass P ON (C.ParentClassId = P.MetaClassId)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaClassListByMetaField
	@MetaFieldId	INT	
AS
	SELECT MC.MetaClassId, MC.Namespace, MC.Name, MC.FriendlyName, MC.IsSystem,  MC.IsAbstract, MC.ParentClassId, MC.TableName, MC.Description, MC.FieldListChangedSqlScript, MC.Tag
	FROM MetaClass MC
	INNER JOIN MetaClassMetaFieldRelation MCFR ON MCFR.MetaClassId = MC.MetaClassId
	WHERE MCFR.MetaFieldId = @MetaFieldId

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaDictionary
	@MetaFieldId	INT
AS
	SELECT MetaDictionaryId, MetaFieldId, [Value] FROM MetaDictionary WHERE MetaFieldId = @MetaFieldId


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaField
	@MetaFieldId	INT
AS	
	SELECT [MetaFieldId] , [Namespace], [Name], [FriendlyName], [Description], [SystemMetaClassId], [DataTypeId],[Length],[AllowNulls],[SaveHistory],[MultiLanguageValue], [AllowSearch], [Tag]
	FROM MetaField WHERE MetaFieldId = @MetaFieldId


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaFieldByName
	@Name		NVARCHAR(256)
AS	
	SELECT [MetaFieldId] ,  [Namespace], [Name], [FriendlyName], [Description], [SystemMetaClassId], [DataTypeId],[Length],[AllowNulls],[SaveHistory],[MultiLanguageValue], [AllowSearch], [Tag]
	FROM MetaField WHERE  [Name] = @Name	AND SystemMetaClassId = 0


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaFieldByNamespace
	@Namespace		NVARCHAR(1024),
	@Deep			BIT
AS
	IF @Deep = 1 
		SELECT [MetaFieldId] ,  [Namespace], [Name], [FriendlyName], [Description], [SystemMetaClassId], [DataTypeId],[Length],[AllowNulls],[SaveHistory],[MultiLanguageValue], [AllowSearch], [Tag]
		FROM MetaField WHERE Namespace = @Namespace OR Namespace LIKE (@Namespace + '.%')
	ELSE
		SELECT [MetaFieldId] ,  [Namespace], [Name], [FriendlyName], [Description], [SystemMetaClassId], [DataTypeId],[Length],[AllowNulls],[SaveHistory],[MultiLanguageValue], [AllowSearch], [Tag]
		FROM MetaField WHERE Namespace = @Namespace


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaFieldList
AS	
	SELECT [MetaFieldId] ,  [Namespace], [Name], [FriendlyName], [Description], [SystemMetaClassId], [DataTypeId],[Length],[AllowNulls],[SaveHistory],[MultiLanguageValue], [AllowSearch], [Tag]
	FROM MetaField


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaFieldListByMetaClassId
	@MetaClassId	INT
AS	
	SELECT MF.[MetaFieldId] ,  MF.[Namespace], MF.[Name], MF.[FriendlyName], MF.[Description], MF.[SystemMetaClassId] , MF.[DataTypeId], MF.[Length], MF.[AllowNulls], MF.[SaveHistory], MF.[MultiLanguageValue], MF.[AllowSearch] , MF.Tag, MCFR.[Weight], MCFR.[Enabled]
		FROM MetaField MF
	INNER JOIN MetaClassMetaFieldRelation MCFR ON MCFR.MetaFieldId = MF.MetaFieldId
	WHERE MCFR.MetaClassId = @MetaClassId
	ORDER BY MCFR.[Weight]


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaFieldWeight
	@MetaClassId	INT,
	@MetaFieldId	INT
AS	
	IF NOT EXISTS(	SELECT * FROM MetaClassMetaFieldRelation WHERE MetaClassId = @MetaClassId AND MetaFieldId = @MetaFieldId)
		RAISERROR ('Wrong @MetaClassId or @MetaFieldId.', 16,1)

	SELECT Weight, Enabled FROM MetaClassMetaFieldRelation WHERE MetaClassId = @MetaClassId AND MetaFieldId = @MetaFieldId


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaFile 
	@MetaKey	INT
AS
	SELECT MetaKey, [FileName], ContentType, Data, CreationTime, LastWriteTime, LastReadTime FROM MetaFileValue WHERE MetaKey = @MetaKey


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaFileList
 AS
	SELECT MetaKey, [FileName], ContentType, Data, CreationTime, LastWriteTime, LastReadTime FROM MetaFileValue
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaObjectValue
	@MetaKey	INT
AS
	SELECT MetaKey, MetaClassId, MetaObjectId  FROM MetaObjectValue WHERE MetaKey = @MetaKey


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].[mdpsp_sys_LoadMetaRuleById] 
	@RuleId	INT
AS
	SELECT RuleId, MetaClassId, Data FROM MetaRule
	WHERE RuleId = @RuleId
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].[mdpsp_sys_LoadMetaRuleByMetaClassId] 
	@MetaClassId	INT
AS
	SELECT RuleId, MetaClassId, Data FROM MetaRule
	WHERE MetaClassId = @MetaClassId
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaStringDictionary
	@MetaKey	INT
AS
	SELECT MetaKey, [Key],[Value] FROM MetaStringDictionaryValue WHERE MetaKey = @MetaKey


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaType
	@MetaTypeId	INT
AS
	SELECT DataTypeId, Name, FriendlyName, Description, Length, SqlName, AllowNulls, Variable, IsSQLCommonType FROM MetaDataType WHERE
		DataTypeId = @MetaTypeId


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_LoadMetaTypeList	
AS
	SELECT DataTypeId, [Name], FriendlyName, [Description], Length, SqlName, AllowNulls, Variable, IsSQLCommonType FROM MetaDataType


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_LoadMultivalueDictionary 
	@MetaKey	INT
AS
	
	SELECT MD.MetaDictionaryId, MD.MetaFieldId, MD.[Value] FROM MetaDictionary MD
		INNER JOIN MetaMultivalueDictionary  MVD ON MVD.MetaDictionaryId = MD.MetaDictionaryId 
		WHERE MVD.MetaKey = @MetaKey


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_MetaFieldAllowSearch
	@MetaFieldId 	INT,
	@AllowSearch	BIT
AS
	SET NOCOUNT ON

	--BEGIN TRAN

	IF NOT EXISTS( SELECT * FROM MetaField WHERE MetaFieldId = @MetaFieldId)
	BEGIN
		RAISERROR ('Wrong @MetaFieldId. The field is system or not existed.', 16,1)
		GOTO ERR
	END

	UPDATE MetaField SET AllowSearch = @AllowSearch WHERE MetaFieldId = @MetaFieldId

	--COMMIT TRAN
	
-- Fult Text Query Addon 

	DECLARE class_w_search CURSOR FOR
		SELECT MCMFR.MetaClassId FROM MetaClassMetaFieldRelation MCMFR
			INNER JOIN MetaField MF ON MF.MetaFieldId = MCMFR.MetaFieldId
			INNER JOIN MetaClass MC ON MC.MetaClassId = MCMFR.MetaClassId
		WHERE MCMFR.MetaFieldId = @MetaFieldId AND (MC.IsSystem = 1 OR MF.SystemMetaClassId = 0 )

	DECLARE @MetaClassId INT
	
	OPEN class_w_search	
	FETCH NEXT FROM class_w_search INTO @MetaClassId

	WHILE @@FETCH_STATUS = 0
	BEGIN

		EXEC mdpsp_sys_FullTextQueriesFieldUpdate @MetaClassId, @MetaFieldId, @AllowSearch

		FETCH NEXT FROM class_w_search INTO @MetaClassId
	END

	CLOSE class_w_search
	DEALLOCATE class_w_search

	-- End Fult Text Query Addon 

RETURN

ERR:
	ROLLBACK TRAN
RETURN
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_MetaFieldSaveHistory
	@MetaFieldId 	INT,
	@SaveHistory	BIT
AS
	SET NOCOUNT ON

	BEGIN TRAN

	IF NOT EXISTS( SELECT * FROM MetaField WHERE MetaFieldId = @MetaFieldId  AND SystemMetaClassId = 0)
	BEGIN
		RAISERROR ('Wrong @MetaFieldId. The field is system or not existed.', 16,1)
		GOTO ERR
	END

	UPDATE MetaField SET SaveHistory = @SaveHistory WHERE MetaFieldId = @MetaFieldId

	IF @@ERROR <> 0 GOTO ERR

	DECLARE class_cursor CURSOR FOR 
		SELECT MetaClassId  FROM MetaClassMetaFieldRelation WHERE MetaFieldId =   @MetaFieldId

	DECLARE @MetaClassId INT

	OPEN class_cursor	
	FETCH NEXT FROM class_cursor INTO @MetaClassId 

	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC mdpsp_sys_CreateMetaClassHistoryTrigger @MetaClassId
		IF @@ERROR <> 0 GOTO ERR
	FETCH NEXT FROM class_cursor INTO @MetaClassId
	END
		
	CLOSE class_cursor
	DEALLOCATE class_cursor

	COMMIT TRAN
RETURN

ERR:
	ROLLBACK TRAN
RETURN


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].mdpsp_sys_ReplaceUser 
	@OldUserId AS INT,
	@NewUserId AS INT
AS
SET NOCOUNT ON
BEGIN TRAN
	DECLARE classall_cursor CURSOR FOR
		SELECT MetaClassId, TableName FROM MetaClass WHERE IsSystem =0 AND IsAbstract = 0

	DECLARE @MetaClassId	INT
	DECLARE @TableName		NVARCHAR(255)

	OPEN classall_cursor	
	FETCH NEXT FROM classall_cursor INTO @MetaClassId, @TableName

	DECLARE @SQLString NVARCHAR(500)

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @SQLString  = N'UPDATE ' + @TableName  + ' SET CreatorId = @NewUserId WHERE CreatorId = @OldUserId'
		EXEC sp_executesql @SQLString, N'@OldUserId AS INT, 	@NewUserId AS INT', @OldUserId = @OldUserId, @NewUserId = @NewUserId
		IF @@ERROR <> 0 GOTO ERR

		SET @SQLString  = N'UPDATE ' + @TableName  + ' SET ModifierId = @NewUserId WHERE ModifierId = @OldUserId'
		EXEC sp_executesql @SQLString, N'@OldUserId AS INT, 	@NewUserId AS INT', @OldUserId = @OldUserId, @NewUserId = @NewUserId
		IF @@ERROR <> 0 GOTO ERR

		SET @SQLString  = N'UPDATE ' + @TableName  + '_History SET ModifierId = @NewUserId WHERE ModifierId = @OldUserId'
		EXEC sp_executesql @SQLString, N'@OldUserId AS INT, 	@NewUserId AS INT', @OldUserId = @OldUserId, @NewUserId = @NewUserId
		IF @@ERROR <> 0 GOTO ERR

	FETCH NEXT FROM classall_cursor INTO @MetaClassId, @TableName
	END
	
	CLOSE classall_cursor
	DEALLOCATE classall_cursor

	COMMIT TRAN
RETURN

ERR:
	CLOSE classall_cursor
	DEALLOCATE classall_cursor

	ROLLBACK TRAN
RETURN
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_UpdateMetaClass
	@MetaClassId 	INT,
	@Namespace		NVARCHAR(1024),
	@Name			NVARCHAR(256),
	@FriendlyName		NVARCHAR(256),
	@Description		NTEXT,
	@Tag			IMAGE
AS
	UPDATE MetaClass SET Namespace = @Namespace, Name = @Name, FriendlyName = @FriendlyName, Description = @Description, Tag = @Tag WHERE MetaClassId = @MetaClassId


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_UpdateMetaDictionary
	@MetaDictionaryId	INT,
	@Value			NVARCHAR(1024)
AS
	SET NOCOUNT ON

BEGIN TRAN
	
	IF NOT EXISTS(SELECT * FROM MetaDictionary WHERE MetaDictionaryId = @MetaDictionaryId )
	BEGIN
		RAISERROR('Wrong @MetaDictionaryId.',16,1)
		GOTO ERR
	END

	UPDATE MetaDictionary SET [Value] = @Value WHERE MetaDictionaryId = @MetaDictionaryId

	IF @@ERROR <> 0 GOTO ERR

	COMMIT TRAN
RETURN

ERR:
	ROLLBACK TRAN
RETURN


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_UpdateMetaField
	@MetaFieldId 	INT,
	@Namespace 	NVARCHAR(1024) = N'Mediachase.MetaDataPlus.User',
	@FriendlyName	NVARCHAR(256),
	@Description	NTEXT,
	@Tag		IMAGE
AS
	UPDATE MetaField SET Namespace = @Namespace, FriendlyName = @FriendlyName, Description = @Description, Tag = @Tag WHERE MetaFieldId = @MetaFieldId


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_UpdateMetaFieldEnabled
	@MetaClassId	INT,
	@MetaFieldId	INT,
	@Enabled	BIT
AS	
	IF NOT EXISTS(	SELECT * FROM MetaClassMetaFieldRelation WHERE MetaClassId = @MetaClassId AND MetaFieldId = @MetaFieldId)
		RAISERROR ('Wrong @MetaClassId or @MetaFieldId.', 16,1)
	ELSE
		UPDATE MetaClassMetaFieldRelation SET  Enabled = @Enabled WHERE MetaClassId = @MetaClassId AND MetaFieldId = @MetaFieldId


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_UpdateMetaFieldWeight
	@MetaClassId	INT,
	@MetaFieldId	INT,
	@Weight	INT
AS	
	IF NOT EXISTS(	SELECT * FROM MetaClassMetaFieldRelation WHERE MetaClassId = @MetaClassId AND MetaFieldId = @MetaFieldId)
		RAISERROR ('Wrong @MetaClassId or @MetaFieldId.', 16,1)
	ELSE
		UPDATE MetaClassMetaFieldRelation SET  Weight = @Weight WHERE MetaClassId = @MetaClassId AND MetaFieldId = @MetaFieldId


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_UpdateMetaFile
	@MetaKey	INT,
	@FileName	NVARCHAR(256),
	@ContentType	NVARCHAR(256),
	@Data		image,
	@Size		INT,
	@CreationTime	DATETIME,
	@LastWriteTime 	DATETIME,
	@LastReadTime	DATETIME
AS
	SET NOCOUNT ON

	IF (EXISTS(SELECT * FROM MetaFileValue WHERE MetaKey = @MetaKey) )
		UPDATE MetaFileValue SET [FileName] = @FileName, ContentType = @ContentType, Data = @Data, [Size] = @Size, 
			CreationTime = @CreationTime, LastWriteTime = @LastWriteTime, LastReadTime = @LastReadTime WHERE MetaKey = @MetaKey
	ELSE
		INSERT INTO MetaFileValue (MetaKey,FileName,ContentType,Data, Size,CreationTime, LastWriteTime, LastReadTime )
			VALUES (@MetaKey,@FileName,@ContentType,@Data, @Size,@CreationTime, @LastWriteTime, @LastReadTime)


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE [dbo].mdpsp_sys_UpdateMetaObjectValue
	@MetaKey	INT,
	@MetaClassId	INT,
	@MetaObjectId	INT
AS
	SET NOCOUNT ON

	IF (EXISTS(SELECT * FROM MetaObjectValue WHERE MetaKey = @MetaKey) )
		UPDATE MetaObjectValue SET MetaClassId = @MetaClassId, MetaObjectId = @MetaObjectId WHERE MetaKey = @MetaKey
	ELSE
		INSERT INTO MetaObjectValue (MetaKey,MetaClassId,MetaObjectId)
			VALUES (@MetaKey,@MetaClassId,@MetaObjectId)


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].[mdpsp_sys_UpdateMetaRule] 
	@RuleId	INT,
	@MetaClassId	INT,
	@Data		IMAGE,
	@RetVal	INT	OUTPUT
AS
	SET NOCOUNT ON

	IF ((SELECT COUNT(*) FROM MetaRule WHERE RuleId=@RuleId) = 0)
	BEGIN
		INSERT INTO MetaRule(MetaClassId, Data) VALUES (@MetaClassId, @Data)

		IF @@ERROR <> 0 SET @RetVal = -1
		ELSE SET @RetVal = @@IDENTITY
	END
	ELSE
	BEGIN
		UPDATE MetaRule SET Data=@Data WHERE RuleId=@RuleId
		SET @RetVal = @RuleId
	END

	SET NOCOUNT OFF
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE PROCEDURE [dbo].mdpsp_sys_UpdateMetaSqlScriptTemplate
	@MetaClassId 	INT,
	@FieldListChanged	NTEXT
AS
	UPDATE MetaClass SET FieldListChangedSqlScript = @FieldListChanged WHERE MetaClassId = @MetaClassId


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


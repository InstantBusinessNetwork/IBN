IF NOT EXISTS (SELECT * FROM [DatabaseVersion])
	INSERT INTO [DatabaseVersion] ([Major], [Minor], [Build]) VALUES ({MajorVersion},{MinorVersion},{BuildNumber})
ELSE
	UPDATE [DatabaseVersion] SET [Major]={MajorVersion}, [Minor]={MinorVersion}, [Build]={BuildNumber}
GO

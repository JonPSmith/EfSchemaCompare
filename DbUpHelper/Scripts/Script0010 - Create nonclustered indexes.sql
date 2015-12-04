
CREATE NONCLUSTERED INDEX [IX_DataTopId] ON [dbo].[DataChild] ([DataTopId])
GO

CREATE NONCLUSTERED INDEX [IX_MyInt] ON [dbo].[DataChild] ([MyInt])
GO

CREATE NONCLUSTERED INDEX [IX_DataManyCompKey_ManyKey1_DataManyCompKey_ManyKey2] ON [dbo].[DataManyCompKeyDataTop] ([DataManyCompKey_ManyKey1], [DataManyCompKey_ManyKey2])
GO

CREATE NONCLUSTERED INDEX [IX_DataTop_DataTopId] ON [dbo].[DataManyCompKeyDataTop] ([DataTop_DataTopId])
GO

CREATE NONCLUSTERED INDEX [IX_Key1_Key2] ON [dbo].[DataTop] ([Key1], [Key2])
GO

CREATE NONCLUSTERED INDEX [IX_DataTopId] ON [dbo].[DataZeroOrOne] ([DataTopId])
GO

CREATE NONCLUSTERED INDEX [IX_DataTopId] ON [dbo].[DataSingleton] ([DataTopId])
GO

CREATE NONCLUSTERED INDEX [IX_DataManyChildrenId] ON [dbo].[NonStandardManyToManyTableName] ([DataManyChildrenId])
GO

CREATE NONCLUSTERED INDEX [IX_DataTopId] ON [dbo].[NonStandardManyToManyTableName] ([DataTopId])
GO


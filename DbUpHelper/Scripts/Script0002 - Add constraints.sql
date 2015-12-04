ALTER TABLE $schema$.[NonStandardManyToManyTableName]  
ADD  CONSTRAINT [FK_dbo.NonStandardManyToManyTableName_dbo.DataManyChildren_DataManyChildrenId] FOREIGN KEY([DataManyChildrenId])
REFERENCES $schema$.[DataManyChildren] ([DataManyChildrenId])
ON DELETE CASCADE
GO

ALTER TABLE $schema$.[NonStandardManyToManyTableName]  
ADD  CONSTRAINT [FK_dbo.NonStandardManyToManyTableName_dbo.DataTop_DataTopId] FOREIGN KEY([DataTopId])
REFERENCES $schema$.[DataTop] ([DataTopId])
ON DELETE CASCADE
GO

ALTER TABLE $schema$.[DataTop]
ADD  CONSTRAINT [FK_dbo.DataTop_dbo.NonStandardCompKeyTable_Key1_Key2] FOREIGN KEY([Key1], [Key2])
REFERENCES [dbo].[NonStandardCompKeyTable] ([Key1], [Key2])
ON DELETE CASCADE
GO

ALTER TABLE $schema$.[DataChild]  
ADD  CONSTRAINT [FK_dbo.DataChild_dbo.DataTop_DataTopId] FOREIGN KEY([DataTopId])
REFERENCES $schema$.[DataTop] ([DataTopId])
ON DELETE CASCADE
GO

ALTER TABLE $schema$.[DataSingleton] 
ADD  CONSTRAINT [FK_dbo.DataSingleton_dbo.DataTop_DataTopId] FOREIGN KEY([DataTopId])
REFERENCES $schema$.[DataTop] ([DataTopId])
GO

ALTER TABLE [dbo].[DataManyCompKeyDataTop]
ADD  CONSTRAINT [FK_dbo.DataManyCompKeyDataTop_dbo.DataManyCompKey_DataManyCompKey_ManyKey1_DataManyCompKey_ManyKey2] FOREIGN KEY([DataManyCompKey_ManyKey1], [DataManyCompKey_ManyKey2])
REFERENCES [dbo].[DataManyCompKey] ([ManyKey1], [ManyKey2])
ON DELETE CASCADE
GO


ALTER TABLE [dbo].[DataManyCompKeyDataTop]
ADD  CONSTRAINT [FK_dbo.DataManyCompKeyDataTop_dbo.DataTop_DataTop_DataTopId] FOREIGN KEY([DataTop_DataTopId])
REFERENCES [dbo].[DataTop] ([DataTopId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[DataZeroOrOne]
ADD  CONSTRAINT [FK_dbo.DataZeroOrOne_dbo.DataTop_DataTopId] FOREIGN KEY([DataTopId])
REFERENCES [dbo].[DataTop] ([DataTopId])
ON DELETE CASCADE
GO


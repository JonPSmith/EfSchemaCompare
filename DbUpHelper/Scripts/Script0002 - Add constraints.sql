ALTER TABLE $schema$.[DataManyChildrenDataTop]  
ADD  CONSTRAINT [FK_dbo.DataManyChildrenDataTop_dbo.DataManyChildrens_DataManyChildren_DataManyChildrenId] 
FOREIGN KEY([DataManyChildren_DataManyChildrenId])
REFERENCES $schema$.[DataManyChildren] ([DataManyChildrenId])
ON DELETE CASCADE
GO

ALTER TABLE $schema$.[DataManyChildrenDataTop]  
ADD CONSTRAINT [FK_dbo.DataManyChildrenDataTop_dbo.DataTop_DataTop_DataTopId] 
FOREIGN KEY([DataTop_DataTopId])
REFERENCES $schema$.[DataTop] ([DataTopId])
ON DELETE CASCADE
GO

ALTER TABLE $schema$.[DataChild]  
ADD  CONSTRAINT [FK_dbo.DataChild_dbo.DataTop_DataTopId] FOREIGN KEY([DataTopId])
REFERENCES $schema$.[DataTop] ([DataTopId])
ON DELETE CASCADE
GO

ALTER TABLE $schema$.[DataSingleton] 
ADD  CONSTRAINT [FK_dbo.DataSingleton_dbo.DataTop_DataSingletonId] FOREIGN KEY([DataSingletonId])
REFERENCES $schema$.[DataTop] ([DataTopId])
GO

ALTER TABLE [dbo].[DataCompKeyDataTop]  
ADD  CONSTRAINT [FK_dbo.DataCompKeyDataTop_dbo.DataCompKey_DataCompKey_Key1_DataCompKey_Key2] FOREIGN KEY([DataCompKey_Key1], [DataCompKey_Key2])
REFERENCES [dbo].[DataCompKey] ([Key1], [Key2])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[DataCompKeyDataTop]
ADD  CONSTRAINT [FK_dbo.DataCompKeyDataTop_dbo.DataTop_DataTop_DataTopId] FOREIGN KEY([DataTop_DataTopId])
REFERENCES [dbo].[DataTop] ([DataTopId])
ON DELETE CASCADE
GO

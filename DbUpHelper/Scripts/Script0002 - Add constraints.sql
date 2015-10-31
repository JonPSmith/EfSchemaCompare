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

-- Sort out missing foreign key on [DataChild] table

ALTER TABLE $schema$.[DataChild]  
ADD  CONSTRAINT [FK_dbo.DataChild_dbo.DataTop_DataTopId] FOREIGN KEY([DataTopId])
REFERENCES $schema$.[DataTop] ([DataTopId])
ON DELETE CASCADE
GO



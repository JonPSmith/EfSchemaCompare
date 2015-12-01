create table $schema$.[DataTop](
	[DataTopId] [int] IDENTITY(1,1) PRIMARY KEY,
	[Key1] [int] NOT NULL,
	[Key2] [uniqueidentifier] NOT NULL,
	[MyString] [varchar](25) NULL,
	[DataSingletonId] [int] NULL,
)
go

create table $schema$.[DataChild](
	[DataChildId] [int] IDENTITY(1,1)PRIMARY KEY,
	[MyInt] [int] NOT NULL,
	[MyString] [nvarchar](max) NULL,
	[DataTopId] [int] NOT NULL
)
go

create table $schema$.[DataManyChildren](
	[DataManyChildrenId] [int] IDENTITY(1,1) PRIMARY KEY,
	[MyInt] [int] NOT NULL 
)
go

create table $schema$.[NonStandardCompKeyTable](
	[Key1] [int] NOT NULL,
	[Key2] [uniqueidentifier] NOT NULL,
	[NonStandardColumnName] [int] NOT NULL,
	PRIMARY KEY(Key1, Key2)
)
go

CREATE TABLE $schema$.[DataManyCompKey](
	[ManyKey1] [int] NOT NULL,
	[ManyKey2] [uniqueidentifier] NOT NULL,
	PRIMARY KEY([ManyKey1], [ManyKey2])
)
go

CREATE TABLE $schema$.[DataSingleton](
	[DataSingletonId] [int] IDENTITY(1,1) PRIMARY KEY,
	[MyDateTime] [datetime] NOT NULL,
	[NonStandardForeignKeyName] [int] NULL
)
go

create table $schema$.[NonStandardManyToManyTableName](
	[DataTopId] [int] NOT NULL,
	[DataManyChildrenId] [int] NOT NULL,
	primary key ([DataTopId], [DataManyChildrenId])
)
go

create table $schema$.[DataManyCompKeyDataTop](
	[DataManyCompKey_ManyKey1] [int] NOT NULL,
	[DataManyCompKey_ManyKey2] [uniqueidentifier] NOT NULL,
	[DataTop_DataTopId] [int] NOT NULL,
    PRIMARY KEY ([DataManyCompKey_ManyKey1], [DataManyCompKey_ManyKey2], [DataTop_DataTopId])
)
go

CREATE TABLE [dbo].[DataZeroOrOne](
	[DataTopId] [int] NOT NULL PRIMARY KEY,
	[MyBool] [bit] NOT NULL
)
go
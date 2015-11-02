create table $schema$.[DataTop](
	[DataTopId] [int] IDENTITY(1,1) PRIMARY KEY,
	[MyString] [nvarchar](25) NULL,
	[DataSingletonId] [int] NULL,
)
go

create table $schema$.[DataChild](
	[DataChildId] [int] IDENTITY(1,1)PRIMARY KEY,
	[MyInt] [int] NOT NULL,
	[MyString] [varchar](max) NULL,
	[DataTopId] [int] NOT NULL
)
go

create table $schema$.[DataManyChildren](
	[DataManyChildrenId] [int] IDENTITY(1,1) PRIMARY KEY,
	[MyInt] [int] NOT NULL 
)
go

create table $schema$.[DataCompKey](
	[Key1] [int] NOT NULL,
	[Key2] [uniqueidentifier] NOT NULL,
	[MyEnum] [int] NOT NULL,
	PRIMARY KEY(Key1, Key2)
)
go

CREATE TABLE $schema$.[DataSingleton](
	[DataSingletonId] [int] IDENTITY(1,1) PRIMARY KEY,
	[MyDateTime] [datetime] NOT NULL,
	[NonStandardForeignKeyName] [int] NULL
)
go

create table $schema$.[DataManyChildrenDataTop](
	[DataManyChildren_DataManyChildrenId] [int] NOT NULL,
	[DataTop_DataTopId] [int] NOT NULL,
	primary key (DataManyChildren_DataManyChildrenId, DataTop_DataTopId)
)
go

create table $schema$.[DataCompKeyDataTop](
	[DataCompKey_Key1] [int] NOT NULL,
	[DataCompKey_Key2] [uniqueidentifier] NOT NULL,
	[DataTop_DataTopId] [int] NOT NULL,
    PRIMARY KEY ([DataCompKey_Key1], [DataCompKey_Key2], [DataTop_DataTopId])
)
go


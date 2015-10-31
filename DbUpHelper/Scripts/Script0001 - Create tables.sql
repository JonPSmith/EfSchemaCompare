create table $schema$.[EfViewData](
	[EfViewDataId] [int] IDENTITY(1,1) PRIMARY KEY,
	[MappingHash] [nvarchar](max) NULL,
	[ViewsData] [nvarchar](max) NULL,
	[DateCreatedUtc] [datetime] NOT NULL 
)
go

create table $schema$.[DataTop](
	[DataTopId] [int] IDENTITY(1,1) PRIMARY KEY,
	[MyString] [nvarchar](25) NULL
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

create table $schema$.[DataManyChildrenDataTop](
	[DataManyChildren_DataManyChildrenId] [int] NOT NULL,
	[DataTop_DataTopId] [int] NOT NULL,
	primary key (DataManyChildren_DataManyChildrenId, DataTop_DataTopId)
)
go


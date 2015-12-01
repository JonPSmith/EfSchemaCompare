
CREATE TABLE  $schema$.[DataComplex](
	[DataComplexId] [int] IDENTITY(1,1) PRIMARY KEY,
	[ComplexData_ComplexInt] [int] NOT NULL,
	[ComplexData_ComplexString] [nvarchar](25) NULL,
	[ComplexComplexData_ComplexDateTime] [datetime] NOT NULL,
	[ComplexComplexData_ComplexGuid] [uniqueidentifier] NOT NULL,
	[ComplexComplexData_ComplexData_ComplexInt] [int] NOT NULL,
	[ComplexComplexData_ComplexData_ComplexString] [nvarchar](25) NULL,
)
go


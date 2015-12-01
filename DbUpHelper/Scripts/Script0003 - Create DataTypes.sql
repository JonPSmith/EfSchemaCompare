CREATE TABLE [dbo].[DataDate](
	[DataDateId] [int] IDENTITY(1,1) PRIMARY KEY,
	[DataSqlDate] [date] NOT NULL,
	[DataDateTime] [datetime] NOT NULL,
	[DataDateTime2] [datetime2](7) NOT NULL,
	[DataSmallDateTime] [smalldatetime] NOT NULL,
	[DataDateTimeOffset] [datetimeoffset](7) NOT NULL,
	[DataTimeSpan] [time](7) NOT NULL
)
go

CREATE TABLE [dbo].[DataGuidEnum](
	[DataGuidEnumId] [int] IDENTITY(1,1) PRIMARY KEY,
	[DataGuid] [uniqueidentifier] NOT NULL,
	[ByteEnum] [tinyint] NOT NULL,
	[ShortEnum] [smallint] NOT NULL,
	[NormalEnum] [int] NOT NULL,
	[LongEnum] [bigint] NOT NULL
)
GO

CREATE TABLE [dbo].[DataIntDouble](
	[DataIntDoubleId] [int] IDENTITY(1,1) PRIMARY KEY,
	[DataBool] [bit] NOT NULL,
	[DataByte] [tinyint] NOT NULL,
	[DataShort] [smallint] NOT NULL,
	[DataInt] [int] NOT NULL,
	[DataLong] [bigint] NOT NULL,
	[DataFloat] [real] NOT NULL,
	[DataDouble] [float] NOT NULL,
	[DataDecimalSmallPrecision] [decimal](5, 3) NOT NULL,
	[DataDecimalNormal] [decimal](18, 2) NOT NULL
)
GO

CREATE TABLE [dbo].[DataStringByte](
	[DataStringByteId] [int] IDENTITY(1,1) PRIMARY KEY,
	[DataStringNvarchar] [nvarchar](max) NULL,
	[DataStringNvarchar25] [nvarchar](25) NULL,
	[DataStringNchar25Fixed] [nchar](25) NULL,
	[DataStringNvarchar25NotNull] [nvarchar](25) NOT NULL,
	[DataStringVarchar] [varchar](8000) NULL,
	[DataStringVarchar25] [varchar](25) NULL,
	[DataStringChar25Fixed] [char](25) NULL,
	[DataStringVarchar25NotNull] [varchar](25) NOT NULL,
	[DataByteBinary25] [binary](25) NULL,
	[DataByteVarbinary] [varbinary](8000) NULL,
	[DataByteVarbinary25] [varbinary](25) NULL,
	[DataByteVarbinary25NotNull] [varbinary](25) NOT NULL
)
GO
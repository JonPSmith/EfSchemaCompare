
CREATE TABLE $schema$.[DataPublicPrivate](
	[DataPublicPrivateId] [int] IDENTITY(1,1)  PRIMARY KEY,
	[PublicInt] [int] NOT NULL,
	[PublicProtectedSetInt] [int] NOT NULL,
	[PublicPrivateSetInt] [int] NOT NULL,
	[InternalInt] [int] NOT NULL,
	[InternalPrivateSetInt] [int] NOT NULL,
	[ProtectedInt] [int] NOT NULL,
	[ProtectedPrivateSetInt] [int] NOT NULL,
	[ProtectedInternalInt] [int] NOT NULL,
	[ProtectedInternalPrivateSetInt] [int] NOT NULL,
	[PrivateInt] [int] NOT NULL,
)
GO

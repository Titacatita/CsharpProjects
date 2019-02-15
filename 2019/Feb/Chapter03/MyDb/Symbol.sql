CREATE TABLE [dbo].[Symbol]
(
	[SymbolId] INT identity(1,1) NOT NULL PRIMARY KEY,
	[Ticker] nvarchar (50) not null,
	[Region] nvarchar (50) null,
	[Sector] nvarchar (150) null,
	constraint [UQ_Symbol_Ticker] unique (Ticker) 
)

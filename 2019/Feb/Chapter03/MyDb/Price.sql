CREATE TABLE [dbo].[Price]
(
	[PriceId] INT identity(1,1) NOT NULL PRIMARY KEY,
	[SymbolID] int not null,
	[Date] datetime null,
	[PriceOpen] float null,
	[PriceHigh] float null,
	[PriceLow] float null,
	[PriceClose] float null,
	[PriceAdj] float null,
	[Volume] float null,
	constraint [FK_Price_Symbol] foreign key ([SymbolID]) 
		references [Symbol] ([SymbolID]),
	constraint [UQ_Price] unique ([SymbolID], [Date])	
)


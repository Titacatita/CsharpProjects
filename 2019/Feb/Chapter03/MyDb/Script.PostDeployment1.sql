/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

insert into Symbol (Ticker, Region, Sector)
select 'A', 'US', 'Health Care'
where not exists (select 1 from Symbol where Ticker = 'A');

insert into Symbol (Ticker, Region, Sector)
select 'AA', 'US', 'Materials'
where not exists (select 1 from Symbol where Ticker = 'AA');

insert into Price (SymbolID, [Date], PriceOpen, PriceHigh, PriceLow, PriceClose, PriceAdj, Volume)
select '1', '01/03/2000', '78.75', '78.937', '67.375', '72.46', '46.991788', '4674400'
where not exists (select 1 from Price where SymbolID = '1' and Date = '01/03/2000');

insert into Price (SymbolID, [Date], PriceOpen, PriceHigh, PriceLow, PriceClose, PriceAdj, Volume)
select '1', '01/04/2000', '68.125', '68.875', '64.75', '66.6', '43.40213', '4765100'
where not exists (select 1 from Price where SymbolID = '1' and Date = '01/04/2000');


insert into Price (SymbolID, [Date], PriceOpen, PriceHigh, PriceLow, PriceClose, PriceAdj, Volume)
select '2', '01/03/2000', '83', '83.525', '80.375', '80.9375', '30.672', '3103200'
where not exists (select 1 from Price where SymbolID = '2' and Date = '01/03/2000');

insert into Price (SymbolID, [Date], PriceOpen, PriceHigh, PriceLow, PriceClose, PriceAdj, Volume)
select '2', '01/04/2000', '80.935', '81.8125', '80.3125', '81.3125', '30.81479', '6243200'
where not exists (select 1 from Price where SymbolID = '2' and Date = '01/04/2000');

insert into Price (SymbolID, [Date], PriceOpen, PriceHigh, PriceLow, PriceClose, PriceAdj, Volume)
select '2', '01/05/2000', '81.3125', '86.5', '81', '86', '32.590553', '6243200'
where not exists (select 1 from Price where SymbolID = '2' and Date = '01/05/2000');


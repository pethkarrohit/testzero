USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Currency_List]    Script Date: 25/04/2022 6:12:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[GeneralMaster_List] 27
ALTER Procedure [dbo].[App_Currency_List]
 @RecordKeyId as bigint=0,
 @prefixText as Nvarchar(100)='' 
As 
Begin
	SET NOCOUNT ON;
           
    SELECT CurrencyId,CurrencyName,CurrencyCode,ConversionFactor,CurrencyValue,CurrencySub,BaseCurrency,RecordStatus,
	dbo.GetLastModifedDetails(ModifedBy,ModifedDate) as LastModifed
	FROM App_Currency (NoLock)	
	where (@prefixText = '' Or CurrencyName like '%' + @prefixText + '%' ) 
	--or (@RecordKeyId=0 Or CurrencyId=@RecordKeyId)

End




    
	
		
		






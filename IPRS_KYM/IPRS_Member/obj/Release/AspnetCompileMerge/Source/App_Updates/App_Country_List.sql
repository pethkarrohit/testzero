USE [Dreamsoft_IPRS] 
GO
/****** Object:  StoredProcedure [dbo].[App_Currency_List]    Script Date: 23/12/2021 7:33:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[App_Country_List] 1,0,''
Create Procedure [dbo].[App_Country_List]
 @GeographicalLevel as tinyint=1,
 @RecordKeyId as bigint=0,
 @prefixText as Nvarchar(100)='' 
As 
Begin
	SET NOCOUNT ON;
           
    SELECT GeographicalId,GeographicalName,GeographicalLevel,GroupId,GeographicalCode,RecordStatus	
	FROM App_Geographical (NoLock)	
	where (GeographicalLevel = 1) and ((@prefixText = '' Or GeographicalName like '%' + @prefixText + '%' ) or (@RecordKeyId=0 Or GeographicalId=@RecordKeyId)) 
	
End




    
	
		
		






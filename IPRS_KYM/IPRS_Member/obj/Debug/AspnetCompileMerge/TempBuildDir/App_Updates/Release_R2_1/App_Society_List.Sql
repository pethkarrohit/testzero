USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Society_List]    Script Date: 28/02/2022 10:17:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[App_Society_List] 0
alter Procedure [dbo].[App_Society_List] 
@RecordKeyId as Bigint=0,
@prefixText as Nvarchar(100)='' 
As 
Begin
	
    SELECT SocietyId,SocietyName FROM App_Society where @RecordKeyId=0
	
End




    
	
		
		






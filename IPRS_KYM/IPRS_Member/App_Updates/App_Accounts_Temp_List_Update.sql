USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_Temp_List]    Script Date: 23-03-2023 20:45:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--App_Accounts_Temp_List 3
ALTER Procedure [dbo].[App_Accounts_Temp_List]
 @RecordKeyId as bigint=0
As 
Begin
	SET NOCOUNT ON;
    
	SELECT * FROM App_Accounts_Temp Where (TempAccountId =@RecordKeyId) and AccountRegType in ('C','I','NI','NC','SC','NSC','LH','LHN')
End





USE [Dreamsoft_Temp]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_List_Bank_IPM]    Script Date: 27/12/2021 7:01:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[App_Accounts_List_IPM] 10
alter Procedure [dbo].[App_Accounts_List_Bank_IPM]
 @RecordKeyId as bigint=0
As 
Begin
	SET NOCOUNT ON;
    
	SELECT   AA.BankName, 
	AA.BankAcNo, AA.BankSwift, AA.BankIFSCCode ,AA.BankBranchName,AA.MicrCode,AA.Currency_Id,AA.CurrencyName
	FROM  App_Accounts AS AA WITH (NoLock) 
 
	Where (AccountId=@RecordKeyId) 

End

	



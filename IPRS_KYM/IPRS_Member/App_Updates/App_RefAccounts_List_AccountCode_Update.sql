USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_RefAccounts_List_AccountCode]    Script Date: 2/6/2023 3:58:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Exec [App_Accounts_List_AccountCode] 1212

alter PROCEDURE [dbo].[App_RefAccounts_List_AccountCode] 
	@AccountId as bigint	
AS
BEGIN
	SELECT RefAccountCode from App_Accounts where AccountId=@AccountId

END





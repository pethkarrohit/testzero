USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_List_AccountCode]    Script Date: 07/01/2022 4:17:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Exec [App_Accounts_List_AccountCode] 1188 

ALTER PROCEDURE [dbo].[App_Accounts_List_AccountCode] 
	@AccountId as bigint	
AS
BEGIN
	SELECT AccountCode from App_Accounts where AccountId=@AccountId

END






 
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_List_AccountCode]    Script Date: 24-03-2023 13:48:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Exec [App_Accounts_List_AccountCode] 1188 

create PROCEDURE [dbo].[App_Accounts_List_AccountCode] 
	@AccountId as bigint	
AS
BEGIN
	SELECT AccountCode from App_Accounts where AccountId=@AccountId

END






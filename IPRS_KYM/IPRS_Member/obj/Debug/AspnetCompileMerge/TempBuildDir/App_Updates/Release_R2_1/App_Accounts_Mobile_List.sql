USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_Mobile_List]    Script Date: 27/04/2022 10:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[App_Accounts_Mobile_List] '+91-9936066699'
ALTER Procedure [dbo].[App_Accounts_Mobile_List]	
	@AccountMobile as nvarchar(20) = '',
	@AccountRegType as nvarchar(20)
 
As 
Begin
	SET NOCOUNT ON;



	BEGIN
		SELECT AccountId FROM App_Accounts (NOLOCK) WHERE AccountMobile=@AccountMobile and AccountRegType =@AccountRegType
			

		SELECT TempAccountId FROM App_Accounts_Temp (NOLOCK) WHERE  Mobile=@AccountMobile and AccountRegType =@AccountRegType
			
	END

	
End



USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_Deceased_List]    Script Date: 2/1/2023 12:05:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



--[App_Accounts_Deceased_List] 'C-00074'
alter Procedure [dbo].[App_Accounts_Deceased_List] 
 @RefAccountCode as nvarchar (50)

As 
Begin
	SET NOCOUNT ON;    

	Select	* FROM   App_Accounts Where AccountCode=@RefAccountCode 	

End



USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_Nominee_List]    Script Date: 23/03/2022 12:35:40 PM ******/
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



USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_Login_IPM]    Script Date: 07/01/2022 10:30:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Exec [App_Users_Login] 'DVFUz%2b31bkU%3d'
--Exec [App_Accounts_Login_IPM]  'ansarip99@gmail.com'
create PROCEDURE [dbo].[App_Accounts_List_RefAccountId] 
	@RefAccountCode as NVARCHAR(50)	
AS
BEGIN
	SELECT AccountId from App_Accounts where AccountCode=@RefAccountCode 					 

END






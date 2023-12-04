USE [Dreamsoft_Temp]
GO
/****** Object:  StoredProcedure [dbo].[MemberRoleType_LookUp_Fees_List_IPM]    Script Date: 06/01/2022 11:03:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--select * from App_Accounts
--[App_Accounts_List] 2
create Procedure [dbo].[App_Accounts_List_Deceased]
  @Accountcode as nvarchar(50)=''

As 
Begin
	
    
			Select	*  FROM  App_Accounts Where (AccountCode=@Accountcode)
End




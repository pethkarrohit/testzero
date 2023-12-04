USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_List_Deceased]    Script Date: 15/03/2022 3:46:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--[App_Accounts_List_SocietyName] 
alter Procedure [dbo].[App_Accounts_List_SocietyName]
  @SocietyName as nvarchar(50)=''

As 
Begin	
    
	Select	*  FROM  App_Society Where (SocietyName=@SocietyName)
End




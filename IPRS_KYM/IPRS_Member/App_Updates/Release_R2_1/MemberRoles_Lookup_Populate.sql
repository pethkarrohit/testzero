USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[MemberRoles_Lookup_Populate]    Script Date: 19-12-2022 17:49:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER  PROCEDURE [dbo].[MemberRoles_Lookup_Populate] 
  @BookType as nvarchar(10)=''
AS
Set nocount on;
	Begin
	 	SELECT   [MemberRoleCode] ,[MemberRoleName] from [dbo].[MemberRoles_Lookup](NOLOCK)
			Where MemberRoleStatus='Y'
			Order by MemberRoleId asc
	End
Set nocount off;


USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[MemberRoleType_LookUp_Populate_IPM]    Script Date: 19/01/2022 4:48:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[MemberRoleType_LookUp_Populate_IPM]'I'
ALTER  Procedure [dbo].[MemberRoleType_LookUp_Populate_IPM]

 @RegistrationType as nvarchar(10)=''
As 
Begin
	SET NOCOUNT ON;
           
    SELECT MemberRoleTypeId, MemberRoleType,RoleType,RegistrationType
	FROM MemberRoleType_LookUp (NoLock)
	Where (@RegistrationType='' OR RegistrationType=@RegistrationType )
	Order By sr_no

End



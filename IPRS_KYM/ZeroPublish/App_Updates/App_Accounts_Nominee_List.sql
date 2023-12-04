USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_WorkRegistration_List_IPM]    Script Date: 20/12/2021 11:29:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--select * from App_Accounts
--[App_Accounts_Nominee_List] 870
create Procedure [dbo].[App_Accounts_Nominee_List]
 @RecordKeyId as bigint=0,
 @AccountId as bigint = 0

As 
Begin
	SET NOCOUNT ON;
    

Select	NomineeId, AccountId, [dbo].[GetColumnValue](NomineeName, ' ', 1) FirstName,
 [dbo].[GetColumnValue](NomineeName, ' ', 2) LastName, Relationship, 
 REPLACE(CONVERT(VARCHAR,DOB,106),' ','-')DOB ,NomineeGender, NomineeImage, NomineeEmailId, NomineeMobile,case when Minor=0 then 'Yes' when Minor=1 then 'No' end as Minor,GuardianName,GuardianMobile ,PanNo,AadharNo,Share,CreateDate, CreatedBy, ModifiedBy, 
   ModifiedDate FROM   App_Accounts_Nominee 
	Where (AccountId=@AccountId) and (@RecordKeyId = 0 Or NomineeId = @RecordKeyId)
	Order By NomineeName 



End



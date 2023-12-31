USE [Dreamsoft_Temp]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_Login_IPM]    Script Date: 07/01/2022 10:30:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Exec [App_Users_Login] 'DVFUz%2b31bkU%3d'
--Exec [App_Accounts_Login_IPM]  'ansarip99@gmail.com'
ALTER PROCEDURE [dbo].[App_Accounts_Login_IPM] 
	@LoginName as NVARCHAR(100)=''
	
AS
SET nocount on;
BEGIN

	

	SELECT AA.AccountId,  AA.AccountEmail,AA.AccountPassword,AA.AccountName,AA.ApplicationStatus,AA.RefAccountCode, AA.AccountRegType,AA.RecordStatus,AA.AccountCode,AA.RefAccountCode,AA.ApplicationStatus,AA.BookId,
	AA.BusinessUnitId, AA.BranchId,AB.BookType,APL.AuthorizationLevel,isnull(APL.ApprovalLogcount,0)as ApprovalLogcount
 from App_Accounts AA
Outer apply 
(
  Select   Booktype from App_BookMaster where BookId=AA.BookId
) 
AB
OUTER APPLY
(
			Select top 1 AuthorizationLevel,isnull(count(APL.ApprovalLogId),0) as ApprovalLogcount
				From App_Accounts_ApprovalLog APL
				Where APL.AccountId =AA.AccountId
				group by AuthorizationLevel

)
APL
 where AccountEmail=@LoginName and AccountType='C'
			
		 
END
Set nocount off;





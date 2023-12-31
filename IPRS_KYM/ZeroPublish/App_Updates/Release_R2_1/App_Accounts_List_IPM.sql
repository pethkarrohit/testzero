USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_List_IPM]    Script Date: 13-12-2022 13:34:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[App_Accounts_List_IPM] 870
ALTER Procedure [dbo].[App_Accounts_List_IPM]
 @RecordKeyId as bigint=0
As 
Begin
	SET NOCOUNT ON;
    
	SELECT  AA.AccountId, AA.BusinessUnitId,AA.BranchId, AA.AccountGroupId, AA.AccountCode, AA.RefAccountCode, AA.AccountType, AA.AccountRegType, AA.AccountName, AA.AccountAlias, AA.AccountDetails, AA.AccountCategoryIds, AA.AccountAllianceId, 
	AA.RecordStatus, AA.CreditLimit, AA.CreditDay, AA.CreditPer, AA.Detail1, AA.Detail2, AA.Detail3, AA.Detail4, AA.Detail5, AA.Detail6, AA.Detail7, AA.Detail8, AA.Detail9, AA.Detail10, AA.Detail11, AA.Detail12, AA.BankName, 
	AA.BankAcNo, AA.BankSwift, AA.BankIFSCCode, AA.AccountImage, AA.AccountAddress, AA.GeographicalId,AA.BookId, AA.Pincode, AA.PincodeId, AA.AccountPhone, AA.AccountMobile, AA.AccountEmail, AA.AccountPassword, AA.AccountWeb, 
	AA.TaxMasterId, AA.MessageType, AA.MessagePrompt,AA.RegistrationDate, AA.LedgerType, AA.CreditPrompt, AA.CreateDate, AA.DOB, AA.PlaceOfBirth, AA.Nationality, AA.BankBranchName, AA.MicrCode, AA.CreatedBy, 
	AA.ModifedBy, AA.ModifedDate,GeographicalName, AA.AccountMobile_Alt, AA.RollTypeIds,EntityType, AA.OverseasSocietyName,AA.AssociationName_India,AA.ApplicationStatus,
	AccountAddress_PR,GeographicalId_PR,GeographicalName_PR,Pincode_PR,PincodeId_PR, AA.ReUpdateInfo,AA.AccountEmail,AA.Alt_EmailId,
	isnull(PR. PaymentRecieptId,0)PaymentRecieptId, PR.PaymentRecieptNo, PR.PaymentStatus, PR.PaymentGatewayResponse,PR.PaymentBankName,
	FatherName,FirstName,LastName,Gender,LanguageId,LanguageName,PreferredLanguage,PreferredLanguageId,InternalIdentificationName,IPINumber,ChanlDesc,SocialSecurityno,TRCNo,TenFform,DualNationality,PreCountryId,PreCountryName,PreState,PreCity,
	PreZipCode,PerCountryId,PerCountryName,PerState,PerCity,PerZipCode,TeritoryAppFor,SocietyId,OverseasSocietyName ,AA.Currency_Id,AA.CurrencyName
	FROM  App_Accounts AS AA WITH (NoLock) 
	outer Apply
	(
		Select  PaymentRecieptId, AccountId, PaymentRecieptNo, PaymentStatus, PaymentGatewayResponse,PaymentBankName
		FROM   App_Accounts_RegPayment Where AccountId=@RecordKeyId and PaymentStatus=0
	)PR
	Where (AA.AccountId=@RecordKeyId) 

End

	



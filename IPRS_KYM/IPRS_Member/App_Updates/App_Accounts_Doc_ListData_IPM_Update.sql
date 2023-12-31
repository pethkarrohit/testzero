USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_Doc_ListData_IPM]    Script Date: 2/1/2023 2:52:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[App_Accounts_Doc_ListData_IPM] 43,0
ALTER Procedure [dbo].[App_Accounts_Doc_ListData_IPM]
@AccountId As Bigint,
@DocumentLookupId As Bigint,
@tFlag as tinyint


As
Begin

	

	Begin Try
		Select Isnull(AccountDocId, 0) As AccountDocId, Isnull(AD.DocumentLookupId, 0) As DocumentLookupId,DC.DocumentName,Accountid, Isnull(ApprovalType, 0) As ApprovalType, 
		       Isnull(DocStatus, 0) As DocStatus, Isnull(DocumentCaption, '') As DocumentCaption, DocFileName,convert(varchar, ModifedDate, 106)as ModifedDate
		From App_Accounts_Doc AD
		Left Outer Join Doc_LookUp DC ON DC.DocumentLookupId=AD.DocumentLookupId
		Where (AD.DocumentLookupId = @DocumentLookupId or @DocumentLookupId = 0) and AccountId = @AccountId and DC.tFlag=@tFlag
		Order By AccountDocId Asc
		
		
	End Try
	Begin Catch
		--Setting return variable status for error handing.

	End Catch
End



USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_ValidateProfile_IPMEXE]    Script Date: 15/02/2022 7:02:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--select * from App_Accounts_Address where
--[App_Accounts_ValidateProfile_IPMEXE] 1189
ALTER Procedure [dbo].[App_Accounts_ValidateProfile_IPMEXE]
@ReturnId as BigInt=0 output

As 
Begin


	INSERT INTO App_EmailSMSSchedule
		 (BusinessUnitIds,BranchIds,BookType,EmailType,DocumentId,EmailSMSConfigId,EmailSMSDate,
		ConfigurationType,ReportType,
		EmailSMTPId,EmailFromAddress,EmailToAddress,EmailCCAddress,EmailBCCAddress,EmailSubject,EmailStartLine,EmailContent,EmailReportValue,EmailSignature 
		)

		Select 
		BusinessUnitIds,BranchIds,BookType,EmailType,a.AccountId,EmailSMSConfigId,GETDATE(),
		ConfigurationType,ReportType,
		EmailSMTPId,EmailFromAddress,
		AccountEmail,'',EmailId as BookEmailId, EmailSubject ,REPLACE( EmailStartLine,'{NAME}',AccountName),replace(CAST(EmailContent AS NVARCHAR(MAX)),'{CONTACT US}',ContactUs)EmailContent,A.AccountId,
		EmailSignature
 		from App_EmailSMSConfig(NOLOCK)       
		cross join 
		App_Accounts A left join App_Accounts_RegPayment b  on a.AccountId =b.AccountId 
		join (Select BookId,isnull(ContactUs,'')ContactUs,isnull(EmailId,'')EmailId from App_BookMaster)BM ON BM.BookID=A.BookId
		--left join App_Accounts_Doc  c on a.AccountId=c.AccountId  
		WHERE BookType='AA' And EmailType='AFFR' and  Datediff(d,a.RegistrationDate,GETDATE())  =7
		AND (A.DOB is null or A.LanguageId is null or Isnull(A.BankAcNo,'') = '' or  b.PaymentAmount is null
		or a.AccountId NOT IN (SELECT AccountId FROM App_Accounts_Doc))


		IF @@ROWCOUNT > 0
					BEGIN						
						SET @ReturnId=1
						--set @Returnmessage='Success'
					END
	ELSE
					BEGIN						
						SET @ReturnId=0
						--set @Returnmessage='Failure'
					END


END
	









USE [Dreamsoft_IPRS_Old]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_Manage_Bank_IPM]    Script Date: 27/12/2021 6:42:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[App_Accounts_Manage_Bank_IPM] 1 
ALTER Procedure [dbo].[App_Accounts_Manage_Bank_IPM]
	@hdnRecordKeyId as Bigint =0,	
	@BankName as nvarchar(100) = '',
	@BankAcNo as nvarchar(100) = '',
	@BankIFSCCode as nvarchar(100) = '',
	@BankBranchName as nvarchar(100)='',
	@MicrCode as nvarchar(50)='',
	@CurrencyId as bigint,
	@CurrencyName as Nvarchar(50),
	@BankSwift as Nvarchar(50),
	@UserName as nvarchar(100)='',
	@ReturnMessage as NVARCHAR(255)=''  output,
	@ReturnId as Bigint=0 output
 
As 
Begin
	SET NOCOUNT ON;
	
	 

	BEGIN TRY  
		
			
			BEGIN
		

			IF (len(@BankName)=0 or len(@BankAcNo)=0 Or len(@BankIFSCCode)=0 Or len(@BankBranchName)=0) --Or len(@MicrCode)=0

			BEGIN
				SET @ReturnId=0
				SET @ReturnMessage = 'ENTER BANK DETAILS'
			return
			END

				UPDATE App_Accounts SET
		
				BankName=upper(@BankName),BankAcNo=upper(@BankAcNo),BankIFSCCode=upper(@BankIFSCCode),
				[BankBranchName]=upper(@BankBranchName),[MicrCode]=upper(@MicrCode),CurrencyId=@CurrencyId,CurrencyName=@CurrencyName,BankSwift=@BankSwift,  

				ModifedBy= @UserName, ModifedDate= GetDate()
				WHERE AccountId = @hdnRecordKeyId
				

				IF @@ROWCOUNT > 0
					BEGIN
						SET @ReturnId = @hdnRecordKeyId
						SET @ReturnMessage ='BANK DETAILS UPDATED SUCESSFULLY'
					END
				ELSE
					BEGIN
						SET @ReturnId = 0
						SET @ReturnMessage ='FAILED TO UPDATE RECORD'
					END

			END --IF @hdnRecordKeyId = 0
			
			
	END TRY  
	BEGIN CATCH  
		print('4')
		SET @ReturnId=0
		SET @ReturnMessage = dbo.RemoveSpecialChars(Error_Message())
	END CATCH; 

	print(@ReturnMessage)
End





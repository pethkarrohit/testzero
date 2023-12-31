USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_Temp_Manage]    Script Date: 07/01/2022 4:44:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

	
--	[App_Accounts_Temp_Manage]
--App_Accounts_Manage '0','Jubin Trivedi','C','C','Karta','16/2, Arbudanagar Society, Near Atmajyoti Ashram, Ellorapark','249','SUBHANPURA / VADODARA / Gujarat / India','390023','+91-8849347614','','','kartaarts18@gmail.com','%2fsZnOvq2EY4%3d','682001112951','AEMPT0337F','Karta Arts','4','','0'
ALTER Procedure [dbo].[App_Accounts_Temp_Manage]
	@hdnRecordKeyId as Bigint =0,
	@AccountCode as nvarchar(50) = '',
	@AccountName as nvarchar(100) = '',
	@AccountType as nvarchar(5) = '',
	@AccountRegType as nvarchar(5) = '',
	@AccountAlias as nvarchar(200) = '',
	@Relationship as nvarchar(50) = '',	
	@AccountAddress as nvarchar(255) = '',
	@GeographicalId as bigint,
	@GeographicalName as nvarchar(200) = '',
	@PincodeId as bigint = 0,
	@Pincode as nvarchar(100) = '',
	@Mobile as nvarchar(50) = '',
	@Telephone as nvarchar(50) = '',
	@Alt_EmailId as nvarchar(100)='',
	@AccoutnLogin as nvarchar(100) = '',
	@AccountPassword as nvarchar(100) = '',
	@AadharNo as nvarchar(20) = '',
	@GSTNo as nvarchar(15) = '',
	@PanNo as nvarchar (10) ='',
	@TANNo as nvarchar (10) ='',
	@TINNo as nvarchar (10) ='',
	@CompanyName as nvarchar(100) = '',
	@RollTypeIds as nvarchar(50) = '',
	@ReturnMessage as NVARCHAR(255)=''  output,
	@ReturnId as Bigint=0 output
 
As 
Begin
	SET NOCOUNT ON;

	if @GeographicalId=0
	SET @GeographicalId =NULL

	---CHECK IF RECORD ALREADY EXIST 
	IF @Alt_EmailId != ''
	BEGIN
		SELECT @hdnRecordKeyId=AccountId FROM App_Accounts (NOLOCK) WHERE (Alt_EmailId=@Alt_EmailId or
		AccountEmail=@Alt_EmailId or AccountEmail=@AccoutnLogin) AND AccountType=@AccountType
			IF @@ROWCOUNT > 0
				BEGIN
					SET @ReturnId=0 -- Only Insert
					SET @ReturnMessage = 'Email Id or Alternate Email Id Already Exist. Please click on Forgot password link to get your Login Details00'
					return
				END

		SELECT @hdnRecordKeyId=TempAccountId FROM App_Accounts_Temp (NOLOCK) WHERE Alt_EmailId=@Alt_EmailId or
		AccoutnLogin=@Alt_EmailId or AccoutnLogin=@AccoutnLogin 
			IF @@ROWCOUNT > 0
				BEGIN
					SET @ReturnId=0 -- Only Insert
					SET @ReturnMessage = 'Email Id or Alternate Email Id Already Exist, Please Check  your Mail Box to verify your Registration Process'
					return
				END
	END


	---CHECK IF RECORD ALREADY EXIST 
	SELECT @hdnRecordKeyId=TempAccountId FROM App_Accounts_Temp (NOLOCK) WHERE  
	AccoutnLogin=@AccoutnLogin OR Mobile=@Mobile
	IF @@ROWCOUNT > 0
		BEGIN
			SET @ReturnId=0 -- Only Insert
			SET @ReturnMessage = 'Account Login Or Mobile No Already Exist, Please Check  your Mail Box to verify your  Registration Process'
			return
		END
	
		---CHECK IF RECORD ALREADY EXIST 
	if @AccountRegType='C'
	BEGIN
		SELECT @hdnRecordKeyId=AccountId FROM App_Accounts (NOLOCK) WHERE (
		AccountEmail=@AccoutnLogin OR AccountMobile=@Mobile) AND AccountType=@AccountType 
		IF @@ROWCOUNT > 0
			BEGIN
				SET @ReturnId=0 -- Only Insert
				SET @ReturnMessage = 'Account Login Or Mobile No Already Exist. Please click on Forgot password link to get your Login Details11'
				return
			END
	END
	if @AccountRegType='I'
	BEGIN
		SELECT @hdnRecordKeyId=AccountId FROM App_Accounts (NOLOCK) WHERE (
		AccountEmail=@AccoutnLogin OR AccountMobile=@Mobile) AND AccountType=@AccountType 
		IF @@ROWCOUNT > 0
			BEGIN
				SET @ReturnId=0 -- Only Insert
				SET @ReturnMessage = 'Account Login Or Mobile No Already Exist. Please click on Forgot password link to get your Login Details122'
				return
			END
	END
		-- Getting BookID
	Declare @BookId as Bigint
	if @GeographicalId=0
		BEGIN
		SET @GeographicalId =NULL
		SET @BookId =NULL
		END
	else
		BEGIN
			Select @BookId=dbo.[GetStateBookId] ( @GeographicalId,@AccountRegType)
		END


	---CHECK IF RECORD ALREADY EXIST 
	BEGIN TRY  
		IF @hdnRecordKeyId = 0 ---ADD RECORD
			BEGIN

	
				INSERT INTO  App_Accounts_Temp (AccountCode,AccountName,AccountType,AccountRegType,AccountRegDate,AccountAlias,RelationShip, AccountAddress,GeographicalId,GeographicalName,PincodeId,Pincode,Mobile,Telephone,
				Alt_EmailId,AccoutnLogin,AccountPassword,AadharNo,GSTNo,PanNo,TANNo,TINNo,CompanyName,RollTypeIds,BookId)
				
				 VALUES
				(@AccountCode,@AccountName,@AccountType,@AccountRegType,GETDATE(),@AccountAlias,@Relationship,@AccountAddress,@GeographicalId,@GeographicalName,@PincodeId,@Pincode,@Mobile,@Telephone,
				 @Alt_EmailId,@AccoutnLogin,@AccountPassword,@AadharNo,@GSTNo,@PanNo,@TANNo,@TINNo,@CompanyName,@RollTypeIds,@BookId)
			 

			   IF @@ROWCOUNT > 0
					BEGIN
						SET @ReturnId = @@IDENTITY
						SET @ReturnMessage ='RECORD ADDEDD SUCESSFULLY'
					END
				ELSE
					BEGIN
						SET @ReturnId = 0
						SET @ReturnMessage ='FAILED TO ADD RECORD'
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



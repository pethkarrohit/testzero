USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_Manage_MV_IPM]    Script Date: 27/04/2022 3:29:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--App_Accounts_Manage '0','DreamSoft IT Solutions Pvt. Ltd.','D','13','Aberton Textiles Ltd.','0102-000','3700 St-Patrick Street','282','323','232323','','','','','','','','','','','','','','','0','Administrator','','0'
ALTER Procedure [dbo].[App_Accounts_Manage_MV_IPM]
	@hdnRecordKeyId as Bigint =0,
	@RefAccountCode as nvarchar(20) = '',
	@AccountType as nvarchar(5) = '',
	@AccountRegType as nvarchar(5) = '',
	@GroupId as bigint,
	@BookId as bigint,
	@AccountName as nvarchar(100) = '',
	@FirstName as nvarchar(100) = '',
	@LastName as nvarchar(100) = '',
	@AccountCode as nvarchar(20) = '',
	@Relationship as nvarchar(50) = '',
	@DOB as datetime,
	@DateofDeath as datetime,
	@DeathCertNo as nvarchar(100) = '',
	@AccountAddress as nvarchar(255) = '',
	@AccountAlias as nvarchar(200) = '',
	@GeographicalId as bigint,
	@GeographicalName as nvarchar(200) = '',
	@Pincode as nvarchar(25),
	@PincodeId as bigint=0,
	@AccountPhone as nvarchar(100) = '',
	@AccountMobile as nvarchar(100) = '',
	@AccountEmail as nvarchar(100) = '',
	@Alt_EmailId as nvarchar(100) = '',
	@AccountPassword  as nvarchar(255) = '',
	@AccountWeb as nvarchar(100) = '',
	@Detail1 as nvarchar(100) = '',
	@Detail2 as nvarchar(100) = '',
	@Detail3 as nvarchar(100) = '',
	@Detail4 as nvarchar(100) = '',
	@Detail5 as nvarchar(100) = '',
	@Detail6 as nvarchar(100) = '',
	@Detail7 as nvarchar(100) = '',
	@Detail8 as nvarchar(100) = '',
	@Detail9 as nvarchar(100) = '',
	@Detail10 as nvarchar(100) = '',
	@Detail11 as nvarchar(100) = '',
	@Detail12 as nvarchar(100) = '',
	@RollTypeIds  as nvarchar(50) = '',
	@RecordStatus as tinyint = 0,
	@RegistrationDate as datetime,
	@UserName as nvarchar(100)='',
	@ReturnMessage as NVARCHAR(255)=''  output,
	@ReturnId as Bigint=0 output
 
As 
Begin
	SET NOCOUNT ON;
	
	if @RegistrationDate ='' 
	BEGIN
		SET @RegistrationDate = NULL
	END

	if @PincodeId=0 or @PincodeId='' or @PincodeId=Null 
	BEGIN
		SET @PincodeId = NULL
	END

		-- Getting BookID
	
	if @GeographicalId=0 or @GeographicalId=''
		BEGIN
		SET @GeographicalId =NULL
		SET @BookId =NULL
		END
	else
		BEGIN
			Select @BookId=dbo.[GetStateBookId] ( @GeographicalId,@AccountRegType)
		END
	

	---CHECK IF RECORD ALREADY EXIST 
	SELECT @hdnRecordKeyId=AccountId FROM App_Accounts (NOLOCK) WHERE (AccountEmail=@AccountEmail)
	AND AccountType=@AccountType	 AND (@hdnRecordKeyId=0 OR AccountId <> @hdnRecordKeyId)
	IF @@ROWCOUNT > 0
		BEGIN
			SET @ReturnId=0
			SET @ReturnMessage = 'RECORD ALREADY EXIST'
			return
		END
	---CHECK IF RECORD ALREADY EXIST 
	BEGIN TRY  
		IF @hdnRecordKeyId = 0 ---ADD RECORD
			BEGIN

				--IF @AccountCode = ''
				--	BEGIN
				--		SELECT  @AccountCode = RIGHT('0000000'+CAST(ISNULL(MAX(AccountCode) + 1 ,0) AS VARCHAR),7)   
				--		FROM App_Accounts WHERE AccountType = @AccountType
				--	END
				



					IF @AccountCode = '' 
					BEGIN
						Declare @strFormatted varchar (100)
						Declare @lngMax BIGINT,@CmpCode  NVARCHAR(50)
						Declare @lngId Numeric (18,0)
						DECLARE @strTransactionPrefix AS NVARCHAR(50)
						DECLARE @FullLen as int, @Len as int
				
						 	BEGIN	
								SET @CmpCode = 'C'

								SET @FullLen = 7
								SET @Len = 5
								SET @strTransactionPrefix = @CmpCode +'-'
						

								SELECT @lngMax = Isnull(Max(dbo.App_NumericOnly(Right(AccountCode,@Len))),0) FROM [dbo].[App_Accounts] 
								WHERE AccountType=@CmpCode and Left(AccountCode,Len(AccountCode) - @Len) = @strTransactionPrefix AND Len(AccountCode) = @FullLen 
								SET @lngMax = ISNULL(@lngMax,0)
								IF @lngMax = 0
									SET @lngMax = 1 
								ELSE
									SET @lngMax = @lngMax + 1 						

								Exec App_FormatWithZero @lngMax, '00000', @strFormatted Output
								Set @AccountCode = @strTransactionPrefix + @strFormatted
							END
					END

				INSERT INTO  App_Accounts (refAccountCode,AccountType,AccountRegType,[AccountGroupId],[BookId],[AccountName],[FirstName],[LastName],[AccountCode] ,[RelationShip],[DOB],[DateOfDeath],[DeathCertificateNo],[AccountAddress_PR],[AccountAlias]
				,[GeographicalId_PR],[GeographicalName_PR],[Pincode_PR],[PincodeId_PR],[AccountPhone],[AccountMobile],[AccountEmail],[Alt_EmailId],[AccountPassword],[AccountWeb],
				[Detail1],[Detail2],[Detail3],[Detail4],[Detail5],[Detail6],
				[Detail7],[Detail8],[Detail9],[Detail10],[Detail11],[Detail12],RollTypeIds,
				[RecordStatus],[RegistrationDate],[CreatedBy],[ModifedBy],PreCity,PreZipCode )
				 VALUES
				(@RefAccountCode,@AccountType,@AccountRegType,@GroupId,@BookId,@AccountName,@FirstName,@LastName,@AccountCode,@Relationship,@DOB,@DateOfdeath,@DeathCertNo,@AccountAddress,@AccountAlias
				,@GeographicalId,@GeographicalName,@Pincode,@PincodeId,@AccountPhone,@AccountMobile,@AccountEmail,@Alt_EmailId,@AccountPassword,@AccountWeb,
				 @Detail1,@Detail2,@Detail3,@Detail4,@Detail5,@Detail6
				,@Detail7,@Detail8,@Detail9,@Detail10,@Detail11,@Detail12,@RollTypeIds
				,@RecordStatus,@RegistrationDate,@UserName,@UserName,@GeographicalName,@Pincode)

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
		ELSE ---UPDATE RECORDS
			BEGIN
		
				UPDATE App_Accounts SET RefAccountCode = @RefAccountCode, AccountType= @AccountType,AccountRegType=@AccountRegType,AccountGroupId= @GroupId,BookId=@BookId,  
				AccountAddress_PR=@AccountAddress,AccountAlias=@AccountAlias,
				AccountName= @AccountName,FirstName= @FirstName,LastName= @LastName,AccountCode= @AccountCode,RelationShip=@Relationship,DOB=@DOB,DateOfdeath=@DateOfdeath ,DeathCertificateNo=@DeathCertNo,				    				
				GeographicalId_PR= @GeographicalId,GeographicalName_PR= @GeographicalName,			
				Pincode_PR= @Pincode,PincodeId_PR=@PincodeId,AccountPhone= @AccountPhone, [AccountMobile]=@AccountMobile,
				AccountEmail= @AccountEmail, Alt_EmailId=@Alt_EmailId, AccountPassword=@AccountPassword,AccountWeb= @AccountWeb,
				Detail1= @Detail1, Detail2= @Detail2, Detail3= @Detail3, 
				Detail4= @Detail4, Detail5= @Detail5, Detail6= @Detail6, 
				Detail7= @Detail7, Detail8= @Detail8, Detail9= @Detail9, 
				Detail10= @Detail10, Detail11= @Detail11, Detail12= @Detail12, 
				RecordStatus= @RecordStatus,
				RegistrationDate=@RegistrationDate,
				ModifedBy= @UserName, ModifedDate= GetDate()
				WHERE AccountId = @hdnRecordKeyId

				IF @@ROWCOUNT > 0
					BEGIN
						SET @ReturnId = @hdnRecordKeyId
						SET @ReturnMessage ='RECORD UPDATED SUCESSFULLY'
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



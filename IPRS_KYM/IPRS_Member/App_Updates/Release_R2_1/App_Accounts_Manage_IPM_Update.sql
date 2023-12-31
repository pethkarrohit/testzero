USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_Manage_IPM]    Script Date: 13-12-2022 17:12:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Select * from App_Accounts
--App_Accounts_Manage '0','DreamSoft IT Solutions Pvt. Ltd.','D','13','Aberton Textiles Ltd.','0102-000','3700 St-Patrick Street','282','323','232323','','','','','','','','','','','','','','','0','Administrator','','0'

ALTER Procedure [dbo].[App_Accounts_Manage_IPM]
	@hdnRecordKeyId as Bigint =0,
 	@AccountCode as nvarchar(20) = '',
	@AccountType as nvarchar(5) = '',
	@AccountRegType as nvarchar(5) = '',
	@GroupId as bigint,
	@AccountName as nvarchar(100) = '',
	@AccountAlias as nvarchar(200) = '',	
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
	@AccountAddress as nvarchar(255) = '',
	@GeographicalId as bigint,
	@PincodeId as bigint,
	@Pincode as nvarchar(25),
	@PreZipCode as nvarchar(50),
	@AccountPhone as nvarchar(100) = '',
	@AccountMobile as nvarchar(100) = '',
	@AccountMobile_Alt as nvarchar(100) = '',
	@DOB as  datetime,
	@PlaceOfBirth as nvarchar(100)='',
	@Nationality as nvarchar(50)='',
	@RollTypeIds as nvarchar(50)='',
	@RecordStatus as tinyint = 0,
	@EntityType as nvarchar(10)='',
	@OverseasSocietyName as nvarchar(150)='',
	@SocietyId as bigint,
	@AssociationName_India as nvarchar(150)='',
	@GeographicalName as nvarchar(100) = '',	
	@AccountAddress_PR as nvarchar(255) = '',
	@GeographicalId_PR as bigint,
 	@GeographicalName_PR as nvarchar(100) = '',
	@PincodeId_PR as bigint,
	@Pincode_PR as nvarchar(25),	
	@PerZipCode as nvarchar(50),		
	@AccountEmail as nvarchar(100) = '',						
	@Alt_EmailId as nvarchar(100) = '',	
	@AccountImage  as nvarchar(100)='',
	@FatherName  as nvarchar(100)='',
	@FirstName  as nvarchar(30)='',
	@LastName  as nvarchar(45)='',
	@Gender  as nvarchar(10)='',
	@LanguageId  as bigint,
	@LanguageName as nvarchar(100),
	@PreferredLanguage  as nvarchar(100)='',
	@PreferredLanguageId  as bigint=0,
	@InternalIdentificationName as nvarchar(50),
	@IPINumber as nvarchar(100),
	@ChanlDesc as nvarchar(1000),
	@SocialSecurityNo as nvarchar(100),
	@TRCNo as nvarchar(100),
	@TenFForm as nvarchar(100),
	@DualNationality as tinyint,
	@PreCountryId as bigint,
	@PreCountryName as nvarchar(100),
	@PreState as nvarchar(100),
	@PreCity as nvarchar(100),
	@PerCountryId as bigint,
	@PerCountryName as nvarchar(100),
	@PerState as nvarchar(100),
	@PerCity as nvarchar(100),
	@TeritoryAppFor as nvarchar(50),
	@UserName as nvarchar(100)='',
	@ReturnMessage as NVARCHAR(255)=''  output,
	@ReturnId as Bigint=0 output
 
As 
Begin
	SET NOCOUNT ON;
	
 
	
	if @LanguageId =''  Or  @LanguageId =0
	BEGIN
		SET @LanguageId = NULL
	END

	if @PreferredLanguageId =''  Or  @PreferredLanguageId =0
	BEGIN
		SET @PreferredLanguageId = NULL
	END

	if @DOB=''
	BEGIN
		SET @DOB = NULL
	END

	if @GeographicalId=0
	BEGIN
		SET @GeographicalId = null
	END

	if @PincodeId=0
	BEGIN
		SET @PincodeId = null
	END
	

	if @GeographicalId_PR=0
	BEGIN
		SET @GeographicalId_PR = null
	END

	if @PincodeId_PR=0
	BEGIN
		SET @PincodeId_PR = null
	END

	if @PerCountryId=0
	BEGIN
		SET @PerCountryId = null
	END

	if @PreCountryId=0
	BEGIN
		SET @PreCountryId = null
	END

	


		-- Getting BookID
	Declare @BookId as Bigint
	IF @GEOGRAPHICALID_PR=0
	BEGIN
		SET @GEOGRAPHICALID_PR =NULL
		IF @GEOGRAPHICALID=0
		BEGIN
			SET @GEOGRAPHICALID =NULL
			SET @BOOKID =NULL
		END
		ELSE
		BEGIN
			SELECT @BOOKID=DBO.[GETSTATEBOOKID] ( @GEOGRAPHICALID,@ACCOUNTREGTYPE)
		END
	END
	ELSE
	BEGIN
		SELECT @BookId=dbo.[GetStateBookId] ( @GEOGRAPHICALID_PR,@AccountRegType)
		IF @GEOGRAPHICALID=0
		BEGIN
			SET @GEOGRAPHICALID =NULL			
		END
	END

	---CHECK IF RECORD ALREADY EXIST
	--Select * from  App_Accounts where Alt_EmailId = Null and (75=0 OR AccountId <> 75)
	if @Detail1 != ''
	BEGIN
		SELECT AccountId FROM App_Accounts (NOLOCK) WHERE (AccountEmail=@AccountEmail OR AccountMobile=@AccountMobile OR Detail1 = @Detail1)
		AND AccountType=@AccountType	AND (@hdnRecordKeyId=0 OR AccountId <> @hdnRecordKeyId)
		IF @@ROWCOUNT > 0
			BEGIN
				SET @ReturnId=0
				SET @ReturnMessage = 'RECORD ALREADY EXIST'
				return
			END
	END
	
	if @Detail1 = ''
	BEGIN
		SELECT AccountId FROM App_Accounts (NOLOCK) WHERE (AccountEmail=@AccountEmail OR AccountMobile=@AccountMobile)
		AND AccountType=@AccountType	AND (@hdnRecordKeyId=0 OR AccountId <> @hdnRecordKeyId)
		IF @@ROWCOUNT > 0
			BEGIN
				SET @ReturnId=0
				SET @ReturnMessage = 'RECORD ALREADY EXIST'
				return
			END
	END
	---CHECK IF RECORD ALREADY EXIST

	---CHECK IF RECORD ALREADY EXIST 
	IF @Alt_EmailId != ''
	BEGIN
		SELECT @hdnRecordKeyId=AccountId FROM App_Accounts (NOLOCK) WHERE (Alt_EmailId=@Alt_EmailId or
		AccountEmail=@Alt_EmailId or AccountEmail=@AccountEmail) AND AccountType=@AccountType AND (@hdnRecordKeyId=0 OR AccountId <> @hdnRecordKeyId)
			IF @@ROWCOUNT > 0
				BEGIN
					SET @ReturnId=0 -- Only Insert
					SET @ReturnMessage = 'Email Id or Alternate Email Id Already Exist.'
					return
				END		
	END


	---CHECK IF RECORD ALREADY EXIST  
	BEGIN TRY  
		IF @hdnRecordKeyId = 0 ---ADD RECORD
			BEGIN

	



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
								WHERE Left(AccountCode,Len(AccountCode) - @Len) = @strTransactionPrefix AND Len(AccountCode) = @FullLen and AccountType=@CmpCode
								SET @lngMax = ISNULL(@lngMax,0)
								IF @lngMax = 0
									SET @lngMax = 1 
								ELSE
									SET @lngMax = @lngMax + 1 						

								Exec App_FormatWithZero @lngMax, '00000', @strFormatted Output
								Set @AccountCode = @strTransactionPrefix + @strFormatted
							END
					 
					END

				INSERT INTO  App_Accounts ([AccountCode] ,AccountType,AccountRegType,[AccountGroupId],[AccountName],AccountAlias,
				BookId,[Detail1],[Detail2],[Detail3],[Detail4],[Detail5],[Detail6],[Detail7],[Detail8],[Detail9],[Detail10],[Detail11],[Detail12],
				AccountAddress,[GeographicalId],[PincodeId],[Pincode], [PreZipCode],[AccountPhone],[AccountMobile],[AccountMobile_Alt],
				
				[DOB],[PlaceOfBirth],[Nationality],[RollTypeIds],[RecordStatus],EntityType,[OverseasSocietyName],[SocietyId],[AssociationName_India],
				[GeographicalName],[AccountAddress_PR],[GeographicalId_PR],[GeographicalName_PR],[PincodeId_PR],[Pincode_PR],[PerZipCode],[AccountEmail],[Alt_EmailId],AccountImage,
				[FatherName],[FirstName],[LastName],[Gender],[LanguageId],[LanguageName],[PreferredLanguage],[PreferredLanguageId],[InternalIdentificationName],[IPINumber],[ChanlDesc],
				[SocialSecurityNo],[TRCNo],[TenFForm],[DualNationality],[PreCountryId],[PreCountryName],[PreState],[PreCity],[PerCountryId],[PerCountryName],[PerState],[PerCity],[TeritoryAppFor],
				[CreatedBy],[ModifedBy])
				VALUES				
				(@AccountCode,@AccountType,@AccountRegType,@GroupId,upper(@AccountName),upper(@AccountAlias),
				@BookId,upper(@Detail1),upper(@Detail2),upper(@Detail3),upper(@Detail4),upper(@Detail5),upper(@Detail6),
				upper(@Detail7),upper(@Detail8),upper(@Detail9),upper(@Detail10),upper(@Detail11),upper(@Detail12),
				upper(@AccountAddress),@GeographicalId,@PincodeId,@Pincode,@PreZipcode,@AccountPhone,@AccountMobile,@AccountMobile_Alt,
				
				@DOB,upper(@PlaceOfBirth),upper(@Nationality),@RollTypeIds,@RecordStatus,@EntityType,upper(@OverseasSocietyName),@SocietyId,upper(@AssociationName_India),
				upper(@GeographicalName),upper(@AccountAddress_PR),@GeographicalId_PR,upper(@GeographicalName_PR),@PincodeId_PR,@Pincode_PR,@PerZipCode,@AccountEmail,@Alt_EmailId,@AccountImage,
				@FatherName,@FirstName,@LastName,@Gender,@LanguageId,@LanguageName,@PreferredLanguage,@PreferredLanguageId,@InternalIdentificationName,@IPINumber,@ChanlDesc,@SocialSecurityNo,
				@TRCNo,@TenFForm,@DualNationality,@PreCountryId,@PreCountryName,@PreState,@PreCity,@PerCountryId,@PerCountryName,@PerState,@PerCity,@TeritoryAppFor,
				@UserName,@UserName)

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
		
				UPDATE App_Accounts SET
				
				AccountType=@AccountType,AccountRegType=@AccountRegType,[AccountGroupId]=@GroupId,[AccountName]=upper(@AccountName),AccountAlias=upper(@AccountAlias),
				BookId=@BookId,
				[Detail1]=upper(@Detail1),[Detail2]=upper(@Detail2),[Detail3]=upper(@Detail3),[Detail4]=upper(@Detail4),[Detail5]=upper(@Detail5),[Detail6]=upper(@Detail6),
				[Detail7]=upper(@Detail7),[Detail8]=upper(@Detail8),[Detail9]=upper(@Detail9),[Detail10]=upper(@Detail10),[Detail11]=upper(@Detail11),[Detail12]=upper(@Detail12),
				AccountAddress=upper(@AccountAddress),[GeographicalId]=@GeographicalId,
				[PincodeId]=@PincodeId,[Pincode]=@Pincode,[PreZipCode]= @PreZipCode, [AccountPhone]=@AccountPhone,[AccountMobile]=@AccountMobile,[AccountMobile_Alt]=@AccountMobile_Alt,
				[DOB]=@DOB,[PlaceOfBirth]=upper(@PlaceOfBirth),[Nationality]=upper(@Nationality),
				[RollTypeIds]=@RollTypeIds,RecordStatus=@RecordStatus,EntityType=@EntityType,
				[OverseasSocietyName]=upper(@OverseasSocietyName),
				[SocietyId]=@SocietyId,
				[AssociationName_India]=upper(@AssociationName_India),
				[GeographicalName]=upper(@GeographicalName),[AccountAddress_PR]=upper(@AccountAddress_PR),
				[GeographicalId_PR]=@GeographicalId_PR,[GeographicalName_PR]=upper(@GeographicalName_PR),[PincodeId_PR]=@PincodeId_PR,[Pincode_PR]=@Pincode_PR,[PerZipCode]=@PerZipCode, 
				AccountEmail=@AccountEmail,Alt_EmailId=@Alt_EmailId,
				AccountImage=@AccountImage,
				FatherName=@FatherName,FirstName=@FirstName,LastName=@LastName,Gender=@Gender,LanguageId=@LanguageId,LanguageName=@LanguageName,PreferredLanguage=@PreferredLanguage,PreferredLanguageId=@PreferredLanguageId,
				InternalIdentificationName=@InternalIdentificationName,IPINumber=@IPINumber,ChanlDesc=@ChanlDesc,SocialSecurityNo=@SocialSecurityNo,TRCNo=@TRCNo,TenFForm=@TenFForm,DualNationality=@DualNationality,
				PreCountryId=@PreCountryId,PreCountryName=@PreCountryName,PreState=@PreState,PreCity=@PreCity,PerCountryId=@PerCountryId,PerCountryName=@PerCountryName,PerState=@PerState,PerCity=@PerCity,TeritoryAppFor=@TeritoryAppFor,
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






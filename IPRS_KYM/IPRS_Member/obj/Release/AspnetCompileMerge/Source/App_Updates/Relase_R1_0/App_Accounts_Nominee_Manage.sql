Use [Dreamsoft_IPRS] 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Deven trivedi
-- Create date: 16-11-2021
--Modifed By:Deven
---Modifed date: 
-- Description: 
--Modifed By: 
---Modifed date: 
-- Description:	 
-- =============================================

--Drop PROCEDURE Centres_Manage

alter PROCEDURE App_Accounts_Nominee_Manage
	@NomineeId as Bigint =0,
	@Accountid as Bigint,
	@NomineeName as nvarchar(100),
	@RelationShip as nvarchar(50),
	@DOB as datetime,
	@Minor as tinyint,
	@GuardianName as nvarchar(100),
	@GuardianMobile as nvarchar(100),
	@PanNo as nvarchar(50),
	@AadharNo as nvarchar(50),
	@NomineeGender as nvarchar(20),
	@NomineeImage as nvarchar(100),
	@NomineeEmailId as nvarchar(100),
	@NomineeMobile as nvarchar(100),
	@Share as float,	
	@cokudUserName as nvarchar(200)  = '',
	@ReturnMessage as NVARCHAR(255)=''  output,
	@ReturnId as Bigint=0 output
AS

BEGIN
	SET NOCOUNT ON;
	
		
		
		
		SELECT NomineeId FROM App_Accounts_Nominee (NOLOCK)
		 WHERE  NomineeName=@NomineeName		
		 AND AccountId= @AccountId  AND (@NomineeId=0 OR NomineeId <> @NomineeId)

		 

		IF @@ROWCOUNT > 0
			BEGIN
				SET @ReturnId=@NomineeId
				SET @ReturnMessage =  'RECORD ALREADY EXISTS'				
				RETURN
			END
		
		BEGIN TRY 
		IF @NomineeId=0
			BEGIN
				
		 
			INSERT INTO [App_Accounts_Nominee](AccountId,NomineeName,RelationShip,DOB,Minor,GuardianName,GuardianMobile,PanNo,AadharNo,NomineeGender,NomineeImage,NomineeEmailId,Nomineemobile,Share,CreatedBy,CreateDate)
			VALUES (@AccountId,@NomineeName,@RelationShip,@DOB,@Minor,@GuardianName,@GuardianMobile,@PanNo,@AadharNo,@NomineeGender,@NomineeImage,@NomineeEmailId,@NomineeMobile,@Share,@cokudUserName,GETDATE())
							  
				  
					
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

				
				

				END 
		ELSE 
			BEGIN		 
				
				
				UPDATE [App_Accounts_Nominee]
				SET  					
				AccountId	= @AccountId,
				NomineeName	= @NomineeName,				
				RelationShip=@RelationShip,
				DOB=@DOB,
				Minor=@Minor,
				GuardianName = @GuardianName,
				GuardianMobile=@GuardianMobile,
				PanNo=@PanNo,
				AadharNo=@AadharNo,
				NomineeGender=@NomineeGender, 
				NomineeImage=@NomineeImage,
				NomineeEmailId=@NomineeEmailId,
				Nomineemobile=@Nomineemobile,				
				Share=@Share,
				ModifiedBy=@cokudUserName,
				ModifiedDate=getdate()
				
				WHERE [App_Accounts_Nominee].NomineeId=@NomineeId
				 
					IF @@ROWCOUNT > 0
								BEGIN
									SET @ReturnId = @NomineeId
									SET @ReturnMessage ='RECORD UPDATED SUCESSFULLY'
								END
							ELSE
								BEGIN
									SET @ReturnId = 0
									SET @ReturnMessage ='FAILED TO UPDATE RECORD'
								END
			END 
		
			
	END TRY  
	BEGIN CATCH  
		print('4')
		SET @ReturnId=0
		SET @ReturnMessage = dbo.RemoveSpecialChars(Error_Message())
	END CATCH; 

	print(@ReturnMessage)
End

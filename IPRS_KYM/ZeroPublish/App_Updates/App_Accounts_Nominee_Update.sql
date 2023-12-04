USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_WorkRegistration_Update_IPM]    Script Date: 20/12/2021 10:39:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--[App_Accounts_Nominee_Update]
alter Procedure [dbo].[App_Accounts_Nominee_Update]
	@NomineeId as Bigint =0,	
	@NomineeImage as nvarchar(500) = '',	
	@ReturnMessage as NVARCHAR(255)=''  output,
	@ReturnId as Bigint=0 output
 
As 
Begin
	
	
	BEGIN TRY  
			BEGIN
			    UPDATE App_Accounts_Nominee  SET
							NomineeImage=upper(@NomineeImage)
							WHERE NomineeId = @NomineeId 

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
		SET @ReturnId=0
		SET @ReturnMessage = dbo.RemoveSpecialChars(Error_Message())
	END CATCH; 

	print(@ReturnMessage)
End



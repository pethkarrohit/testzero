USE [Dreamsoft_Temp]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_Update_TraderName]    Script Date: 02-01-2023 18:56:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--[App_Accounts_Update_TraderName]
CREATE Procedure [dbo].[App_Accounts_Update_TraderName]
	@AccountId as Bigint =0,		
	@AccountAlias as nvarchar(200),
	@Relationship as nvarchar(200),
	@ReturnMessage as NVARCHAR(255)=''  output,
	@ReturnId as Bigint=0 output
 
As 
Begin
	
	
	BEGIN TRY  
			BEGIN
			    UPDATE App_Accounts SET AccountAlias=@AccountAlias,Relationship=@Relationship 
							WHERE AccountId = @AccountId 

							IF @@ROWCOUNT > 0
								BEGIN
									SET @ReturnId = @AccountId
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



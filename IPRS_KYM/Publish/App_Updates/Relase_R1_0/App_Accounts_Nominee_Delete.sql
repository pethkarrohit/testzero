USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_WorkRegistration_Delete]    Script Date: 20/12/2021 12:33:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--App_AccountGroup_Delete 
alter Procedure [dbo].[App_Accounts_Nominee_Delete]
 @NomineeId  as bigint,
 @ReturnMessage as NVARCHAR(500)=''  output,
  @ReturnId as bigint=0 output
As 
SET nocount on;
BEGIN
	BEGIN TRY  
		Delete from App_Accounts_Nominee where NomineeId=@NomineeId 
		IF @@ROWCOUNT > 0
					BEGIN
						SET @ReturnId=@NomineeId
						SET @ReturnMessage ='RECORD DELETE SUCESSFULLY'
					END
	END TRY  
	BEGIN CATCH  
		IF @ReturnMessage = ''
			SET @ReturnMessage = dbo.RemoveSpecialChars(Error_Message())
				SET @ReturnId=0
	END CATCH
End




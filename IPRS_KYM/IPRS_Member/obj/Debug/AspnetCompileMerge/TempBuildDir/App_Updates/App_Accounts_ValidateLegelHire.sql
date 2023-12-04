USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_ValidateLegelHire]    Script Date: 21/01/2022 4:19:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[ValidateLegelHire] 'KALYAN','SHARMA','KALYAN SAHAY SHARMA'
create Procedure [dbo].[App_Accounts_ValidateLegelHire]
 @FirstName as nvarchar(30),
 @LastName as nvarchar(45),
 @IPINumber as nvarchar(200),
 @tVal as nvarchar(5)
As 
Begin
	
	if (@tVal='A')
		begin    		
			SELECT FirstName,LastName,AccountCode,IPINumber  FROM App_Accounts Where FirstName = @FirstName or LastName =@LastName 
		end
	else if (@tVal='I')
		begin
			SELECT FirstName,LastName,AccountCode,IPINumber FROM App_Accounts Where IPINumber=@IPINumber 
		end

End






USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[App_AccountCategory_List]    Script Date: 21/12/2021 12:26:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[App_Language_List] 0,'Arab'
create Procedure [dbo].[App_Language_List]
@Status bigint = 0,
@prefixText as Nvarchar(100)='' 
As 
Begin
	SET NOCOUNT ON;
    
	
		SELECT LanguageId,LanguageName + ' (' + NativeName + ')' LanguageName FROM App_Language_LookUp 
		where (@prefixText = '' Or LanguageName like '%' + @prefixText + '%' Or NativeName like '%' + @prefixText + '%')
		
End






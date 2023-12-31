USE [Dreamsoft_Temp]
GO
/****** Object:  UserDefinedFunction [dbo].[GetStateBookId]    Script Date: 24-03-2023 16:56:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- select * from app_geographical where Groupid in (3,4,6,7,10,13,14,15,22,23,24,25,28,32,33,34)
--Select dbo.[GetStateBookId] (472,'SO')
ALTER FUNCTION [dbo].[GetStateBookId] (@GeographicalId AS bigint,@Type as nvarchar(5))
RETURNS Bigint AS  
BEGIN
	DECLARE @BookId AS Bigint
	SELECT   @BookId  =   Bookid from App_BookMaster 
	outer Apply
(
	Select GroupId as StateID from App_Geographical ST
	Where  ST.GeographicalId=@GeographicalId
)
APS
where  ',' + GeographicalIds +','  like ('%,'+ CAST(APS.StateID  as nvarchar)+',%')
and( (@Type='I' and BookType='AA' and BookAlias like '%Individual%' and BookAlias not like '%NRI%')
OR (@Type='C' and BookType='AA' and BookAlias like '%Company%' and BookAlias not like '%NRI%')
OR (@Type='LH' and BookType='AA' and BookAlias like '%-Member%')
OR (@Type='LHN' and BookType='AA' and BookAlias like '%-Non Member%')
OR (@Type='NI' and BookType='AA' and BookAlias like '%NRI-Individual%')
OR (@Type='NC' and BookType='AA' and BookAlias like '%NRI-Company%')
OR (@Type='SC' and BookType='AA' and BookAlias like '%Self-Release%')
OR (@Type='NSC' and BookType='AA' and BookAlias like '%NRI-Self Release%')
OR (@Type='F' and BookType='FC' )
OR ((@Type<>'I' and @Type<>'F' and @Type<> 'C' and @Type<>'LH' and @Type<>'LHN'and @Type<>'NI'and @Type<>'NC' and @Type<>'SC' and @Type<>'NSC'   ) OR BookType=@Type )
)
--And (BookType=@Type)
 
 
	IF @BookId =0
		BEGIN
			SET @BookId =  null
		END
	
	RETURN  @BookId 
END



	--DECLARE @BookId AS Bigint
	--SELECT top 1  @BookId  =   Bookid from App_BookMaster where  ',' + GeographicalIds +','  like ('%,'+ CAST(@GeographicalId  as nvarchar)+',%')


	--IF @BookId =0
	--	BEGIN
	--		SET @BookId =  null
	--	END
	
	--RETURN  @BookId 
--END
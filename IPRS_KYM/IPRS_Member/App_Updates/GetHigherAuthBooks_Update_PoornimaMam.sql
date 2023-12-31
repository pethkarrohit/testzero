USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[GetHigherAuthBooks]    Script Date: 2/2/2023 12:25:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- GetHigherAuthBooks 30,'aa'
ALTER PROCEDURE [dbo].[GetHigherAuthBooks] 
@UserId AS BIGINT,
@BookType AS NVARCHAR(10)='',
@TransactionType AS NVARCHAR(5)=''
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @WorkGroupId AS BIGINT, @WorkGroupName AS NVARCHAR(100), @Bookkey AS NVARCHAR(MAX), @BookName AS NVARCHAR(100)
	DECLARE @MaxAuthLvls AS NVARCHAR(MAX), @MaxReAuthLvls AS NVARCHAR(MAX), @BookIds AS NVARCHAR(MAX)
	DECLARE @MaxAuthBookLvls AS NVARCHAR(MAX), @UserName AS NVARCHAR(100)
	DECLARE @BookNames AS NVARCHAR(MAX),  @MaxADVAuthBookLvls AS NVARCHAR(MAX)

SET @BookIds = STUFF((
					SELECT ','+CAST(ABA.BookID AS NVARCHAR(10)) FROM App_BookMaster_Authorization ABA
					INNER JOIN APP_BOOKMASTER AB ON AB.BOOKID = ABA.BOOKID
					WHERE BOOKTYPE = @BookType and TransactionType=@TransactionType
					and '%,'+ABA.UserIds +',%' LIKE '%,' +CAST(@UserId AS NVARCHAR(10))+',%'
					 ORDER BY ABA.BookID 
					FOR XML PATH('')),1,1,'')

	select @BookNames= STUFF((
					SELECT ','+CAST(BookPrefix AS NVARCHAR(10)) FROM App_BookMaster 
					WHERE  BookID in (select sid from dbo.CSVToTable(@BookIds))
					FOR XML PATH('')),1,1,'')

	SET @MaxAuthLvls = STUFF((SELECT ','+ CAST(AuthorizationLevel AS NVARCHAR(10)) FROM App_BookMaster_Authorization WHERE BOOKID in (SELECT * FROM DBO.CSVTOTABLE(@BookIds)) and UserIds LIKE '%'+CAST(@UserId AS NVARCHAR(10))+'%' FOR XML PATH('')),1,1,'')

	SET @MaxReAuthLvls = STUFF((SELECT ','+CAST(ReAuthorizationLevel AS NVARCHAR(10)) FROM App_BookMaster WHERE BookID IN (SELECT * FROM dbo.CSVToTable(@BookIds)) FOR XML PATH('')),1,1,'')

	SELECT @WorkGroupId = ISNULL(AU.WorkGroupId,0),@WorkGroupName = ISNULL(AW.WorkGroupName,'') ,@UserName=UserName
	FROM App_Users(NOLOCK) AU
	LEFT OUTER JOIN App_WorkGroup(NOLOCK) AW ON AW.WorkgroupId = AU.WorkGroupId
	WHERE UserId = @UserId


	SELECT
	BookID,AuthorizationLevel AS MaxLevel,AdvAuthorizationLevel, ReAuthorizationLevel AS ReMaxLevel,
	BookKey,BookTable,@WorkGroupName AS BookName,@UserName as UserName
	INTO #TEMP
	FROM
	App_BookMaster(NOLOCK)AB
	LEFT OUTER JOIN App_BookMain(NOLOCK)ABM ON ABM.BookType = AB.BookType
	WHERE 
	AB.BookID IN (SELECT * FROM dbo.CSVToTable(@BookIds)) AND
	--(@WorkGroupId=0 OR WorkGroupIds LIKE '%'+CAST(@WorkGroupId AS NVARCHAR(10))+'%') AND 
	AB.BookType = @BookType


	--SET @BookIds = STUFF((SELECT ','+ CAST(BookID AS NVARCHAR(10)) FROM #TEMP FOR XML PATH('')),1,1,'')
	SET @MaxAuthBookLvls = STUFF((SELECT ','+ CAST(MaxLevel AS NVARCHAR(10)) FROM #TEMP FOR XML PATH('')),1,1,'')
	SET @MaxADVAuthBookLvls = STUFF((SELECT ','+ CAST(AdvAuthorizationLevel AS NVARCHAR(10)) FROM #TEMP FOR XML PATH('')),1,1,'')
	--SET @MaxAuthBookLvls = STUFF((SELECT ','+ CAST(ReMaxLevel AS NVARCHAR(10)) FROM #TEMP FOR XML PATH('')),1,1,'')
	
	SELECT TOP 1 @BookIds AS BOOKIDS, @MaxAuthLvls AS MaxLevel, @MaxReAuthLvls AS ReMaxLevel,@MaxAuthBookLvls as MaxBookLvls, @MaxADVAuthBookLvls as MaxADVAuthBookLvls ,BookKey, BookTable, @BookNames as BookName,UserName FROM #TEMP
	DROP TABLE #TEMP
	SET NOCOUNT OFF;
END



USE [Dreamsoft_IPRS]
GO
/****** Object:  StoredProcedure [dbo].[Area_Populate]    Script Date: 1/28/2023 12:07:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--[Area_Populate] 0,'40'
ALTER Procedure [dbo].[Area_Populate]
@GroupId as bigint=0,
@prefixText as Nvarchar(100)=''
As 
Begin
SET NOCOUNT ON;

	Select * from (
	select GeographicalId,(Case When GeographicalCode <> '' Then GeographicalName + ' - ' + GeographicalCode Else GeographicalName End) as GeographicalName,GeographicalCode
    from App_Geographical (NOLock) 
	WHERE GeographicalLevel =5 
	and (@GroupId =0 or GroupId=@GroupId )
	)MT
	where  GeographicalName Like '%' + @prefixText +'%'
	Order by cast(isnumeric(GeographicalCode) as real)
End






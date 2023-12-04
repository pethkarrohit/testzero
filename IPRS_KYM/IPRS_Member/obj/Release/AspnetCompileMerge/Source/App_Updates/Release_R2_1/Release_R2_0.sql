USE [Dreamsoft_IPRS]
GO

INSERT INTO [dbo].[App_EmailType]([EmailType],[EmailTypeName],[Booktype])
SELECT 'DTR1','Deed Tracker Reminder 1','AA'
Union all
SELECT 'DTR2','Deed Tracker Reminder 2','AA'
Union all
SELECT 'DTR3','Deed Tracker Reminder 3','AA'


INSERT INTO [dbo].[App_EmailType]([EmailType],[EmailTypeName],[Booktype])
SELECT 'AFFR','Applicant Form Fillup Reminder','AFFR'





GO



USE [Dreamsoft_IPRS]
GO

/****** Object:  Table [dbo].[App_Accounts_Nominee]    Script Date: 12-12-2022 17:03:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create TABLE [dbo].[App_Accounts_Nominee](
	[NomineeId] [bigint] IDENTITY(1,1) NOT NULL,
	[AccountId] [bigint] NOT NULL,
	[NomineeName] [nvarchar](100) NULL,
	[RelationShip] [nvarchar](50) NULL,
	[DOB] [datetime] NULL,
	[Minor] [tinyint] NULL,
	[GuardianName] [nvarchar](100) NULL,
	[GuardianMobile] [nvarchar](100) NULL,
	[PanNo] [nvarchar](50) NULL,
	[AadharNo] [nvarchar](50) NULL,
	[NomineeGender] [nvarchar](20) NULL,
	[NomineeImage] [nvarchar](500) NULL,
	[NomineeEmailId] [nvarchar](100) NULL,
	[NomineeMobile] [nvarchar](100) NULL,
	[Share] [float] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_App_Accounts_Nominee] PRIMARY KEY CLUSTERED 
(
	[NomineeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[App_Accounts_Nominee]  WITH CHECK ADD  CONSTRAINT [FK_App_Accounts_Nominee_App_Accounts] FOREIGN KEY([AccountId])
REFERENCES [dbo].[App_Accounts] ([AccountId])
GO

ALTER TABLE [dbo].[App_Accounts_Nominee] CHECK CONSTRAINT [FK_App_Accounts_Nominee_App_Accounts]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'yes->1, No ->0 ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'App_Accounts_Nominee', @level2type=N'COLUMN',@level2name=N'Minor'
GO



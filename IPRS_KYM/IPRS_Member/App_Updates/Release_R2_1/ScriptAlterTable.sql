


ALTER TABLE App_Accounts ADD  DateOfDeath datetime;
ALTER TABLE App_Accounts ADD DeathCertificateNo nvarchar(100);
ALTER TABLE App_Accounts ADD Relationship nvarchar(50);
ALTER TABLE App_Accounts ADD Currency_Id bigint;
ALTER TABLE App_Accounts ADD CurrencyName nvarchar(50);
ALTER TABLE App_Accounts ADD LanguageId bigint;
ALTER TABLE App_Accounts ADD LanguageName nvarchar(100);
ALTER TABLE App_Accounts ADD InternalIdentificationName nvarchar(50);
ALTER TABLE App_Accounts ADD IPINumber nvarchar(100);
ALTER TABLE App_Accounts ADD ChanlDesc nvarchar(max);
ALTER TABLE App_Accounts ADD SocialSecurityNo nvarchar(100);
ALTER TABLE App_Accounts ADD TRCNo nvarchar(100);
ALTER TABLE App_Accounts ADD TenFform nvarchar(100);
ALTER TABLE App_Accounts ADD DualNationality tinyint;
ALTER TABLE App_Accounts ADD PreCountryId bigint;
ALTER TABLE App_Accounts ADD PreCountryName nvarchar(100);
ALTER TABLE App_Accounts ADD PreState nvarchar(100);
ALTER TABLE App_Accounts ADD PreCity nvarchar(100);
ALTER TABLE App_Accounts ADD PreZipCode nvarchar(50);
ALTER TABLE App_Accounts ADD PerCountryId bigint;
ALTER TABLE App_Accounts ADD PerCountryName nvarchar(100);
ALTER TABLE App_Accounts ADD PerState nvarchar(100);
ALTER TABLE App_Accounts ADD PerCity nvarchar(100);
ALTER TABLE App_Accounts ADD PerZipCode nvarchar(50);
ALTER TABLE App_Accounts ADD TeritoryAppFor nvarchar(100);
ALTER TABLE App_Accounts ADD MemberRegistrationDate datetime;
ALTER TABLE App_Accounts_Temp ADD AccountCode nvarchar(50);
ALTER TABLE App_Accounts_Temp ADD RelationShip nvarchar(50);
ALTER TABLE App_BookMaster_Authorization ADD  ProductCategoryId bigint;
--ALTER TABLE App_BookMaster_Authorization ADD  ProductCategoryName nvarchar(200);
ALTER TABLE App_BookMaster_Authorization ADD  FromValue money;
ALTER TABLE App_BookMaster_Authorization ADD  ToValue money;
ALTER TABLE App_BookMaster_Authorization ADD  DiscountType tinyint;




GO

CREATE TABLE [dbo].[App_Accounts_Nominee](
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



 
GO

/****** Object:  Table [dbo].[App_Society]    Script Date: 28/02/2022 11:49:57 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[App_Society](
	[SocietyId] [bigint] IDENTITY(1,1) NOT NULL,
	[SocietyName] [nvarchar](50) NULL,
 CONSTRAINT [PK_App_Society] PRIMARY KEY CLUSTERED 
(
	[SocietyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE app_accounts
ADD SocietyId bigint NULL







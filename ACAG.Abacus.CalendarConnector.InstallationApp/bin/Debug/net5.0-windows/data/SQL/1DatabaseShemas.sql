
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'AbacusData' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[AbacusData](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AbacusID] [bigint] NOT NULL,
	[ExchangeID] [nvarchar](500) NOT NULL,
	[InsertDateTime] [datetime] NULL,
	[MailAccount] [nvarchar](50) NULL,
	[Subject] [nvarchar](500) NOT NULL,
	[DateTimeStart] [datetime] NULL,
	[DateTimeEnd] [datetime] NULL,
	[Status] [int] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	 CONSTRAINT [PK_AbacusData] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'AbacusSetting' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[AbacusSetting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[ServiceUrl] [nvarchar](255) NOT NULL,
	[ServicePort] [int] NOT NULL,
	[ServiceUseSSL] [bit] NOT NULL,
	[ServiceUser] [nvarchar](255) NOT NULL,
	[ServiceUserPassword] [nvarchar](50) NOT NULL,
	[HealthStatus] [bit] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	 CONSTRAINT [PK_AbacusSetting] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'AppRole' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[AppRole](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](30) NOT NULL,
	[IsAdministrator] [bit] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	 CONSTRAINT [PK_AppRole] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'AppRoleRel' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[AppRoleRel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AppRoleId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
		CONSTRAINT [PK_AppRoleRel] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'AppSetting' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[AppSetting](
	[Id] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[Status] [bit] NOT NULL,
	 CONSTRAINT [PK_AppSetting] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'AspNetRoleClaims' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'AspNetRoles' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'AspNetUserClaims' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'AspNetUserLogins' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
	 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
	(
		[LoginProvider] ASC,
		[ProviderKey] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'AspNetUserRoles' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
	(
		[UserId] ASC,
		[RoleId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'AspNetUsers' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'AspNetUserTokens' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NULL,
	 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
	(
		[UserId] ASC,
		[LoginProvider] ASC,
		[Name] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'Culture' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[Culture](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	 CONSTRAINT [PK_Culture] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'ExchangeSetting' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[ExchangeSetting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ExchangeVersion] [nvarchar](50) NOT NULL,
	[ExchangeUrl] [nvarchar](255) NOT NULL,
	[LoginType] [int] NOT NULL,
	[AzureTenant] [nvarchar](255) NULL,
	[AzureClientId] [nvarchar](255) NULL,
	[AzureClientSecret] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[EmailAddress] [nvarchar](75) NULL,
	[ServiceUser] [nvarchar](50) NULL,
	[ServiceUserPassword] [nvarchar](50) NULL,
	[HealthStatus] [bit] NULL,
	[IsEnabled] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	 CONSTRAINT [PK_ExchangeSetting] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'PayType' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[PayType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[Code] [int] NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
	[IsAppointmentPrivate] [bit] NOT NULL,
	[IsAppointmentAwayState] [bit] NULL,
	[IsEnabled] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	 CONSTRAINT [PK_PayType] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'Tenant' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[Tenant](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Number] [int] NOT NULL,
	[AbacusSettingId] [int] NULL,
	[ScheduleTimer] [int] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	 CONSTRAINT [PK_Tenant] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'TenantUserRel' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[TenantUserRel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	 CONSTRAINT [PK_TenantUserRel] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'User' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CultureId] [int] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](75) NOT NULL,
	[Password] [nvarchar](500) NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[SaltPassword] [nvarchar](50) NOT NULL,
	[ResetCode] [nvarchar](500) NULL,
	[IsEnabled] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO



-- TABLE_CONSTRAINTS

IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AppRoleRel' AND CONSTRAINT_NAME = 'FK_AppRoleRel_AppRole')
BEGIN
	ALTER TABLE [dbo].[AppRoleRel]  WITH CHECK ADD  CONSTRAINT [FK_AppRoleRel_AppRole] FOREIGN KEY([AppRoleId])
	REFERENCES [dbo].[AppRole] ([Id])
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AppRoleRel' AND CONSTRAINT_NAME = 'FK_AppRoleRel_AppRole')
BEGIN
	ALTER TABLE [dbo].[AppRoleRel] CHECK CONSTRAINT [FK_AppRoleRel_AppRole]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AppRoleRel' AND CONSTRAINT_NAME = 'FK_AppRoleRel_User')
BEGIN
	ALTER TABLE [dbo].[AppRoleRel]  WITH CHECK ADD  CONSTRAINT [FK_AppRoleRel_User] FOREIGN KEY([UserId])
	REFERENCES [dbo].[User] ([Id])
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AppRoleRel' AND CONSTRAINT_NAME = 'FK_AppRoleRel_User')
BEGIN
	ALTER TABLE [dbo].[AppRoleRel] CHECK CONSTRAINT [FK_AppRoleRel_User]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AspNetRoleClaims' AND CONSTRAINT_NAME = 'FK_AspNetRoleClaims_AspNetRoles_RoleId')
BEGIN
	ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
	REFERENCES [dbo].[AspNetRoles] ([Id])
	ON DELETE CASCADE
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AspNetRoleClaims' AND CONSTRAINT_NAME = 'FK_AspNetRoleClaims_AspNetRoles_RoleId')
BEGIN
	ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AspNetUserClaims' AND CONSTRAINT_NAME = 'FK_AspNetUserClaims_AspNetUsers_UserId')
BEGIN
	ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
	REFERENCES [dbo].[AspNetUsers] ([Id])
	ON DELETE CASCADE
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AspNetUserClaims' AND CONSTRAINT_NAME = 'FK_AspNetUserClaims_AspNetUsers_UserId')
BEGIN
	ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AspNetUserLogins' AND CONSTRAINT_NAME = 'FK_AspNetUserLogins_AspNetUsers_UserId')
BEGIN
	ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
	REFERENCES [dbo].[AspNetUsers] ([Id])
	ON DELETE CASCADE
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AspNetUserLogins' AND CONSTRAINT_NAME = 'FK_AspNetUserLogins_AspNetUsers_UserId')
BEGIN
	ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AspNetUserRoles' AND CONSTRAINT_NAME = 'FK_AspNetUserRoles_AspNetRoles_RoleId')
BEGIN
	ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
	REFERENCES [dbo].[AspNetRoles] ([Id])
	ON DELETE CASCADE
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AspNetUserRoles' AND CONSTRAINT_NAME = 'FK_AspNetUserRoles_AspNetRoles_RoleId')
BEGIN
	ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AspNetUserRoles' AND CONSTRAINT_NAME = 'FK_AspNetUserRoles_AspNetUsers_UserId')
BEGIN
	ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
	REFERENCES [dbo].[AspNetUsers] ([Id])
	ON DELETE CASCADE
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AspNetUserRoles' AND CONSTRAINT_NAME = 'FK_AspNetUserRoles_AspNetUsers_UserId')
BEGIN
	ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AspNetUserTokens' AND CONSTRAINT_NAME = 'FK_AspNetUserTokens_AspNetUsers_UserId')
BEGIN
	ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
	REFERENCES [dbo].[AspNetUsers] ([Id])
	ON DELETE CASCADE
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'AspNetUserTokens' AND CONSTRAINT_NAME = 'FK_AspNetUserTokens_AspNetUsers_UserId')
BEGIN
	ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'ExchangeSetting' AND CONSTRAINT_NAME = 'FK_ExchangeSetting_Tenant')
BEGIN
	ALTER TABLE [dbo].[ExchangeSetting]  WITH CHECK ADD  CONSTRAINT [FK_ExchangeSetting_Tenant] FOREIGN KEY([TenantId])
	REFERENCES [dbo].[Tenant] ([Id])
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'ExchangeSetting' AND CONSTRAINT_NAME = 'FK_ExchangeSetting_Tenant')
BEGIN
	ALTER TABLE [dbo].[ExchangeSetting] CHECK CONSTRAINT [FK_ExchangeSetting_Tenant]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'PayType' AND CONSTRAINT_NAME = 'FK_PayType_Tenant')
BEGIN
	ALTER TABLE [dbo].[PayType]  WITH CHECK ADD  CONSTRAINT [FK_PayType_Tenant] FOREIGN KEY([TenantId])
	REFERENCES [dbo].[Tenant] ([Id])
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'PayType' AND CONSTRAINT_NAME = 'FK_PayType_Tenant')
BEGIN
	ALTER TABLE [dbo].[PayType] CHECK CONSTRAINT [FK_PayType_Tenant]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'Tenant' AND CONSTRAINT_NAME = 'FK_Tenant_AbacusSetting')
BEGIN
	ALTER TABLE [dbo].[Tenant]  WITH CHECK ADD  CONSTRAINT [FK_Tenant_AbacusSetting] FOREIGN KEY([AbacusSettingId])
	REFERENCES [dbo].[AbacusSetting] ([Id])
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'Tenant' AND CONSTRAINT_NAME = 'FK_Tenant_AbacusSetting')
BEGIN
	ALTER TABLE [dbo].[Tenant] CHECK CONSTRAINT [FK_Tenant_AbacusSetting]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'TenantUserRel' AND CONSTRAINT_NAME = 'FK_TenantUserRel_Tenant')
BEGIN
	ALTER TABLE [dbo].[TenantUserRel]  WITH CHECK ADD  CONSTRAINT [FK_TenantUserRel_Tenant] FOREIGN KEY([TenantId])
	REFERENCES [dbo].[Tenant] ([Id])
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'TenantUserRel' AND CONSTRAINT_NAME = 'FK_TenantUserRel_Tenant')
BEGIN
	ALTER TABLE [dbo].[TenantUserRel] CHECK CONSTRAINT [FK_TenantUserRel_Tenant]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'TenantUserRel' AND CONSTRAINT_NAME = 'FK_TenantUserRel_User')
BEGIN
	ALTER TABLE [dbo].[TenantUserRel]  WITH CHECK ADD  CONSTRAINT [FK_TenantUserRel_User] FOREIGN KEY([UserId])
	REFERENCES [dbo].[User] ([Id])
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'TenantUserRel' AND CONSTRAINT_NAME = 'FK_TenantUserRel_User')
BEGIN
	ALTER TABLE [dbo].[TenantUserRel] CHECK CONSTRAINT [FK_TenantUserRel_User]
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'User' AND CONSTRAINT_NAME = 'FK_User_Culture')
BEGIN
	ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Culture] FOREIGN KEY([CultureId])
	REFERENCES [dbo].[Culture] ([Id])
END
GO
IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'User' AND CONSTRAINT_NAME = 'FK_User_Culture')
BEGIN
	ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Culture]
END


-- INIT CULTURE

IF NOT EXISTS (SELECT 1 FROM Culture WHERE Code = 'en')
	INSERT INTO Culture (Code, DisplayName, IsEnabled) VALUES ('en', 'English', 1)
GO
IF NOT EXISTS (SELECT 1 FROM Culture WHERE Code = 'de')
	INSERT INTO Culture (Code, DisplayName, IsEnabled) VALUES ('de', 'German', 1)	
GO
IF NOT EXISTS (SELECT 1 FROM Culture WHERE Code = 'fr')
	INSERT INTO Culture (Code, DisplayName, IsEnabled) VALUES ('fr', 'France', 0)
GO
IF NOT EXISTS (SELECT 1 FROM Culture WHERE Code = 'vi')
	INSERT INTO Culture (Code, DisplayName, IsEnabled) VALUES ('vi', 'Vietnam', 0)
GO
IF NOT EXISTS (SELECT 1 FROM Culture WHERE Code = 'it')
	INSERT INTO Culture (Code, DisplayName, IsEnabled) VALUES ('it', 'Italy', 0)
GO


-- INIT ROLES

IF NOT EXISTS (SELECT 1 FROM AppRole WHERE Code = 'Administrator')
	INSERT INTO AppRole (Code, IsAdministrator, IsEnabled, CreatedBy, CreatedDate) VALUES ('Administrator', 1, 1, -1, GETDATE())
GO
IF NOT EXISTS (SELECT 1 FROM AppRole WHERE Code = 'User')
	INSERT INTO AppRole (Code, IsAdministrator, IsEnabled, CreatedBy, CreatedDate) VALUES ('User', 0, 1, -1, GETDATE())
GO


-- DEFAULT ADMIN

IF NOT EXISTS (SELECT * FROM [User] WHERE [Email] = 'admin@admin.com')
BEGIN
	DECLARE @enCultureId INT
	SELECT @enCultureId = Id FROM Culture WHERE Code = 'en'
	
	INSERT INTO [User] (CultureId, UserName, Email, Password, SaltPassword, IsEnabled, CreatedBy, CreatedDate)
	VALUES (@enCultureId, 
			N'administrator', 
			N'admin@admin.com', 
			N'1174567C001FCBB92E2517EFBFBDEFBFBD1D301EEFBFBD31700E75EFBFBDEFBFBDC4B2EFBFBD6756CBA3EFBFBD18', --123456
			N'3f7e5898-a3bf-44e6-8c24-4b52c974efc9', 
			1, -1, GETDATE())
END
GO

-- SET ADMIN FOR ACCOUNT

IF NOT EXISTS (SELECT 1 FROM AppRoleRel WHERE UserId = (SELECT Id FROM [User] WHERE [Email] = 'admin@admin.com') AND AppRoleId = (SELECT Id FROM AppRole WHERE Code = 'Administrator'))
BEGIN
	DECLARE @adminRoleId INT
	DECLARE @adminId INT

	SELECT @adminRoleId = Id FROM [AppRole] WHERE Code = 'Administrator'
	SELECT @adminId  = Id FROM [User] WHERE [Email] = 'admin@admin.com'

	INSERT INTO AppRoleRel (AppRoleId, UserId, CreatedBy, CreatedDate) VALUES (@adminRoleId, @adminId , -1, GETDATE())
END
GO


-- UPDATE SCRIPT
-- =============================================
-- Author:		QuangVV
-- Create date: 22.01.2021
-- Description:	add translation data for key
-- ID Tasks: 
-- =============================================
IF NOT EXISTS (SELECT 1 FROM Culture WHERE Code = 'tr')
	INSERT INTO Culture VALUES ('tr', 'Translation', 1)
GO
IF NOT EXISTS (SELECT 1 FROM AppSetting WHERE Id = 'FOOTER')
	INSERT INTO AppSetting VALUES ('FOOTER', '{"Line1" : "ACAG.Abacus.CalendarConnector",
	"Line2" : "Copyright © 2020-2030 ACAG.Abacus.CalendarConnector",
	"ProductVersion" : "Product Version: 1.0.0.0",
	"DatabaseVersion" : "Database Version: 1.0.0.0"} ', 1)
GO
IF NOT EXISTS (SELECT 1 FROM AppSetting WHERE Id = 'DATABASEVERSION')
	INSERT INTO AppSetting VALUES ('DATABASEVERSION', '1.0.0.0', 1)
GO
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'AbacusData' AND COLUMN_NAME = N'TenantId')
BEGIN
	ALTER TABLE AbacusData
	ADD TenantId int Default 0;
END
GO

-- =============================================
-- Author:		QuangVV
-- Create date: 22.01.2021
-- Description:	create table LogDiary
-- ID Tasks: 
-- =============================================
IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE TABLE_NAME = 'LogDiary' AND TABLE_TYPE = 'BASE TABLE')
BEGIN
	CREATE TABLE [dbo].[LogDiary](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[Data] [nvarchar](max) NULL,
	[Error] [nvarchar](max) NULL,
	[IsEnabled] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	 CONSTRAINT [PK_LogDiary] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS(SELECT 1 From INFORMATION_SCHEMA.TABLE_CONSTRAINTS Where TABLE_NAME = 'LogDiary' AND CONSTRAINT_NAME = 'FK_LogDiary_Tenant')
BEGIN
	ALTER TABLE [dbo].[LogDiary]  WITH CHECK ADD  CONSTRAINT [FK_LogDiary_Tenant] FOREIGN KEY([TenantId])
	REFERENCES [dbo].[Tenant] ([Id])
END
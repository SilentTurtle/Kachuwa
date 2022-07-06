CREATE TABLE dbo.AppUser
(		AppUserId								bigint primary key identity(1,1),
		IdentityUserId							bigint not null,
		FirstName								nvarchar(256) ,
        LastName								nvarchar(256),
        Bio										nvarchar(2000),
        Email									nvarchar(256) not null,
        Address									nvarchar(256)  null,      
        PhoneNumber								nvarchar(256)  null,        
        DOB										nvarchar(256)  null,
        ProfilePicture							nvarchar(256)  null,
		Gender									nvarchar(10) null, -- m:male, f:female, o:other			
		UserName							    nvarchar(256) not null,		
		Designation								nvarchar(256),
		FacebookLink							nvarchar(256),
		LinkedInLink							nvarchar(256),
		TweeterLink								nvarchar(256),
		InstagramLink							nvarchar(256),		
		CoverImage								nvarchar(256),		
		IsActive                                bit NOT NULL Default(1),
		IsDeleted                               bit NOT NULL Default(0),
		AddedOn                                 datetime NOT NULL Default(getDATE()),
		AddedBy                                 bigint not null default(0),
		DeletedBy								bigint not null default(0),
		DeletedOn                               datetime Default(getDATE()),
		UpdatedOn                               datetime NOT NULL Default(getDATE()),
		UpdatedBy                               bigint not null default(0)
);
create TABLE dbo.IdentityRole
(
	Id 										bigint primary key IDENTITY(1,1) NOT NULL,
	ConcurrencyStamp						nvarchar(max) NULL,
	Name									nvarchar(256) NULL,
	NormalizedName							nvarchar(256) NULL,
	IsTemporary								 bit NOT NULL Default(0),
	IsSystem								bit default(0) not null,
	IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(getDATE()),
    AddedBy                                 bigint not null default(0),
	DeletedBy								bigint not null default(0),
	DeletedOn                               datetime Default(getDATE()),
	UpdatedOn                               datetime NOT NULL Default(getDATE()),
    UpdatedBy                               bigint not null default(0)
);
CREATE TABLE dbo.TemporaryRoleDetail
(
	Id 										bigint primary key IDENTITY(1,1) NOT NULL,
	RoleId									bigint NOT NULL default(0),
	StartDate								datetime not null Default(getDATE()),
	EndDate									datetime not null Default(getDATE()),
	Narration								nvarchar(max)
);
CREATE TABLE dbo.IdentityRoleClaim
(
	Id									int primary key IDENTITY(1,1) NOT NULL,
	ClaimType							nvarchar(max) NULL,
	ClaimValue							nvarchar(max) NULL,
	RoleId								bigint references dbo.IdentityRole NOT NULL
);
CREATE TABLE dbo.IdentityUser
(
	Id									bigint primary key identity(1,1) NOT NULL,
	AccessFailedCount					int NOT NULL,
	ConcurrencyStamp					nvarchar(max) NULL,
	Email								nvarchar(256) NULL,
	EmailConfirmed						bit NOT NULL,
	LockoutEnabled						bit NOT NULL,
	LockoutEnd							datetimeoffset(7) NULL,
	NormalizedEmail						nvarchar(256) NULL,
	NormalizedUserName					nvarchar(256) NULL,
	PasswordHash						nvarchar(max) NULL,
	PhoneNumber							nvarchar(max) NULL,
	PhoneNumberConfirmed				bit NOT NULL,
	SecurityStamp						nvarchar(max) NULL,
	TwoFactorEnabled					bit NOT NULL,
	UserName							nvarchar(256) NULL
);

CREATE TABLE dbo.IdentityUserClaim
(
	Id 									bigint  primary key IDENTITY(1,1) NOT NULL,
	ClaimType							nvarchar(max) NULL,
	ClaimValue							nvarchar(max) NULL,
	UserId								bigint  NOT NULL references dbo.IdentityUser
 );

CREATE TABLE dbo.IdentityUserLogin
(
	LoginProvider						nvarchar(450) NOT NULL,--index
	ProviderKey							nvarchar(450) NOT NULL,--
	ProviderDisplayName					nvarchar(max) NULL,
	UserId								bigint  NOT NULL references dbo.IdentityUser
);	
--drop index dbo.IdentityUserLogin.idx_ext_lgn 
--CREATE CLUSTERED  INDEX idx_ext_lgn
--	ON dbo.IdentityUserLogin 					(LoginProvider, ProviderKey);

CREATE TABLE dbo.IdentityUserRole
(
	UserId								bigint NOT NULL references dbo.IdentityUser,
	RoleId								bigint NOT NULL references dbo.IdentityRole
);
--ok
--drop index dbo.IdentityUserRole.idx_usr_lgn 
CREATE CLUSTERED INDEX idx_usr_lgn
	ON dbo.IdentityUserRole 					(UserId, RoleId);

CREATE TABLE dbo.IdentityUserToken
(
	UserId								bigint NOT NULL references dbo.IdentityUser,
	LoginProvider						nvarchar(450) NOT NULL,
	Name								nvarchar(450) NOT NULL,
	Value								nvarchar(max) NULL
);
--drop index dbo.IdentityUserToken.idx_usr_tkns 
--CREATE  INDEX idx_usr_tkns
--	ON dbo.IdentityUserToken 					(UserId, LoginProvider)
--	include (Name)
create table dbo.RTCUser
(
	RTCUserId				int primary key not null identity(1,1),
	IdentityUserId			bigint not null default(0),
	SessionId				nvarchar(500) default(''),
	GroupName				nvarchar(256) default(''),
	UserRoles				nvarchar(1000) default(''),
	IsFromWeb				bit default(0),
	IsFromMobile			bit default(0),
	UserDevice				nvarchar(2000) default(''),--android or useragent
	ConnectionId			nvarchar(500) not null,
	HubName					nvarchar(256) not null,--cart support chat
	AddedOn					datetime not null default(getutcdate())

);

CREATE TABLE dbo.Country(
	CountryId 												int identity(1,1) primary key not null,
	ISO 													nvarchar(2) not null,
	Name 													nvarchar(80) not null,
	NiceName 												nvarchar(80) default(''),
	LocaleName 												ntext default('') ,
	ISO3 													nvarchar(3) default(''),
	Numcode 												smallint default(0),
	PhoneCode 												int default(0),
	CurrencySymbol 											ntext default('') ,
	CurrencyCode 											nvarchar(3) default(''),
	Currency 												nvarchar(256)default('') 
);

CREATE  Table dbo.LocaleRegion
(
	LocaleRegionId								int primary key identity(1,1) not null,
	CountryId                           		int not null default(0),
	Flag										nvarchar(256),
	Culture										varchar(10),
	IsDefault 									bit NOT NULL Default(0),
	IsActive                                    bit NOT NULL Default(1),
	IsDeleted                                  	bit NOT NULL Default(0),
	AddedOn                                    	datetime NOT NULL Default(GETUTCDATE()),
	AddedBy                                    	national character varying(256) not null
);
CREATE TABLE dbo.LocaleResource 
(
    LocaleResourceId				int primary key identity(1,1) not null,
    Culture							varchar (10)    not null,
    Name							varchar (100)   not null,
    Value							nvarchar (4000) not null,
    GroupName						nvarchar(256)
);
CREATE  INDEX idx_localization
	ON dbo.LocaleResource (Culture, GroupName);
--drop index dbo.LocaleResource.idx_localization 

CREATE TABLE dbo.Module
(
	ModuleId								int primary key identity(1,1) not null,
	Name									nvarchar(256) not null,
	Description								nvarchar(max) ,
	Version									nvarchar(256) not null,
	IsInstalled								bit default(0) not null,
	Author									nvarchar(256),
	IsBuiltIn								bit default(0) not null,
	IsActive                                bit NOT NULL Default(1),
    IsDeleted								bit NOT NULL DEFAULT(0),
    AddedOn									datetime NOT NULL DEFAULT(GETUTCDATE()),
    AddedBy									national character VARYING(256) not null

);

create TABLE dbo.Page
(
	PageId									bigint primary key identity(1,1) not null,
	Name									nvarchar(256) NOT NULL,
	URL										nvarchar(256),	
	Content									nvarchar(max) NULL,
	ContentConfig							nvarchar(max)  null,
	UseMasterLayout							bit default(0) not null,
    IsPublished								bit default(0) not null,
	IsBackend								bit default(0) not null,
	Culture									nvarchar (10)  not null,
	LastModified							datetime NOT NULL,
	LastRequested							datetime NULL,
	ModuleId								int not null default(0),
	IsSystem								bit default(1),
	IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(GETUTCDATE()),
    AddedBy                                 national character varying(256) not null,
	DeletedBy								bigint not null default(0),
	DeletedOn                               datetime Default(getDATE()),
	UpdatedOn                               datetime Default(getDATE()),
    UpdatedBy                               bigint not null default(0)
);

create TABLE dbo.MasterRolePagePermission
(
	MasterRolePagePermissionId				bigint primary key identity(1,1) not null,
	PageId									bigint not null default(0),
	RoleId									bigint not null,	
	AllowAccessForAll						bit default(0) not null,
	ListView								bit default(0) not null,
	AddNew									bit default(0) not null,
	Edit									bit default(0) not null,
	[Delete]								bit default(0) not null,
	ChangeStatus							bit default(0) not null,	
	Download								bit default(0) not null,
	Import									bit default(0) not null,
	Export									bit default(0) not null,
	[Print]									bit default(0) not null,
	ViewReport								bit default(0) not null,
	RunReport								bit default(0) not null,
	ApproveLevel							int not null default(0),									
	IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(getDATE()),
    AddedBy                                 bigint not null default(0),
	DeletedBy								bigint not null default(0),
	DeletedOn                               datetime,
	UpdatedOn                               datetime,
    UpdatedBy                               bigint not null default(0)
);

create table dbo.PagePermission
(
	PagePermissionId						bigint primary key identity(1,1) not null,
	PageId									bigint not null default(0),		
	UserId									bigint not null,
	AllowAccessForAll						bit default(0) not null,
	ListView								bit default(0) not null,	
	AddNew									bit default(0) not null,
	Edit									bit default(0) not null,
	[Delete]								bit default(0) not null,
	ChangeStatus							bit default(0) not null,	
	Download								bit default(0) not null,
	Import									bit default(0) not null,
	Export									bit default(0) not null,
	[Print]									bit default(0) not null,
	ViewReport								bit default(0) not null,
	RunReport								bit default(0) not null,
	ApproveLevel							int not null default(0),								
	IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(getDATE()),
    AddedBy                                 bigint not null default(0),
	DeletedBy								bigint not null default(0),
	DeletedOn                               datetime Default(getDATE()),
	UpdatedOn                               datetime NOT NULL Default(getDATE()),
    UpdatedBy                               bigint not null default(0)
);

CREATE  INDEX idx_page
	ON dbo.Page(URL);
	



CREATE TABLE dbo.Plugin
(
	PluginId								int primary key identity(1,1) not null,
	PluginType								int not null,
	Name									nvarchar(256) not null,
	SystemName								nvarchar(256) not null,
	Image									nvarchar(256) ,
	Version									nvarchar(256) not null,
	Author									nvarchar(256) not null,
	Description								nvarchar(max),
	IsInstalled								bit default(0),
	IsActive								bit default(1) not null,
	IsDeleted								bit default(0) not null,
	AddedOn									datetime default(getutcdate()),
	AddedBy									nvarchar(256) not null
);

Create Table dbo.MenuGroup
(
	MenuGroupId								int primary key identity(1,1) not null,
	Name									nvarchar(256) not null,
	Description								nvarchar(max) ,
	IsActive                                bit NOT NULL Default(1),
	IsSystem								bit not null default(0),
    IsDeleted								bit NOT NULL DEFAULT(0),
    AddedOn									datetime NOT NULL DEFAULT(GETUTCDATE()),
    AddedBy									national character VARYING(256) not null
);

Create Table dbo.Menu
(
	MenuId									int primary key identity(1,1) not null,
	Name									nvarchar(256) not null,
	SubTitle								nvarchar(256),
	Url										nvarchar(256) not null,
	Icon									nvarchar(256),
	CssClass								nvarchar(256),
	IsChild									bit default(0) not null,
	ParentId								int default(0) not null,
	MenuOrder								int default(0) not null,
	MenuGroupId								int not null default(0),--main,side side2
	Culture									nvarchar (10)  not null,
	IsBackend								bit default(0) not null,
	IsSystem								bit default(0) not null,
	IsActive								bit default(1) not null,
	IsDeleted								bit default(0) not null,
	AddedOn									datetime default(getutcdate()),
	AddedBy									nvarchar(256) not null
);
Create table dbo.MenuPermission
(
	MenuPermissionId						int primary key identity(1,1) not null,
	MenuId									int not null default(0),
	AllowAccessForAll						bit default(0) not null,
	AllowAccess								bit default(0) not null,
	RoleId									bigint not null,
	AddedOn									datetime default(getutcdate()),
	AddedBy									nvarchar(256) not null
);
Create Table dbo.MenuSetting
(	
	MenuSettingId							int primary key identity(1,1) not null,
	MenuGroupName							nvarchar(256) not null,
	MenuGroupId								int references dbo.MenuGroup,
	ShowMenuAs								nvarchar(256) not null,--sidebar footer footer2,main navigation 
	CssClasses								nvarchar(256)

);

CREATE TABLE dbo.SEO
(
	SEOId									int primary key IDENTITY(1,1),
	MetaTitle								nvarchar(256)  null,
	MetaKeyWords							nvarchar(256)  null,
	MetaDescription							nvarchar(max) null,
	SeoType									nvarchar(256) not null,--page,product..
	LastUrl									nvarchar(256) null,
	Url										nvarchar(256) not null,
	Image									nvarchar(256) ,
	PageName								nvarchar(256) null,
	PageId								 	int default(0),
	ProductId								int null default(0),
	IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(GETUTCDATE()),
    AddedBy                                 national character varying(256) not null,
	DeletedBy								bigint not null default(0),
	DeletedOn                               datetime Default(getDATE()),
	UpdatedOn                               datetime Default(getDATE()),
    UpdatedBy                               bigint not null default(0)

);
CREATE table dbo.Setting
(
	SettingId								int primary key identity(1,1) not null,
	WebsiteName								nvarchar(256) not null,
	Description								nvarchar(500),
	Country									nvarchar(256),	
	Address1								nvarchar(256),
	Address2								nvarchar(256),
	PhoneNumber								nvarchar(256),
	Email									nvarchar(256), 	
	State 									nvarchar(256),
	City									nvarchar(256),
	TimeZoneOffset							nvarchar(10),
	TimeZoneName							nvarchar(100),
	Longitude								decimal(16,4) default(0),
	Lattitude								decimal(16,4) default(0),	
	Logo									nvarchar(256) not null,
	BaseCulture 							nvarchar(10) not null,
	BaseCurrency							nvarchar(5) not null,		
	CurrencyCode							nvarchar(5) not null,
	GoogleAnalyticScript 					nvarchar(1000) null,
	UseHttps								bit default(0) NOT NULL,
	DefaultEmail							nvarchar(256),
	SupportEmail							nvarchar(256),
	SalesEmail								nvarchar(256),
	MarketingEmail							nvarchar(256)

);
Create table dbo.AuditLog
(
	AuditId									bigint primary key identity(1,1) not null,
	Url										nvarchar(500),
	Action									nvarchar(500),
	Duration								int not null default(0),
	UserName								nvarchar(500),
	Role 									nvarchar(500),
	IpAddress								nvarchar(500),
	UserAgent								nvarchar(1000),
	RequestObject							nvarchar(max),
	AddedOn									datetime default(getutcdate())
);

CREATE TABLE dbo.Sessions(  
    Id									nvarchar(449)  PRIMARY KEY NOT NULL,  
    Value								varbinary(max) NOT NULL,  
    ExpiresAtTime						datetimeoffset(7) NOT NULL,  
    SlidingExpirationInSeconds			bigint NULL,  
    AbsoluteExpiration					datetimeoffset(7) NULL
	)

CREATE NONCLUSTERED INDEX [Index_ExpiresAtTime] ON [dbo].[Sessions]  
(  
    [ExpiresAtTime] ASC  
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  


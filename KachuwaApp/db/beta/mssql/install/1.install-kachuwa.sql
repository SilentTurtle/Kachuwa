CREATE TABLE dbo.IdentityRole
(
	Id 									bigint primary key IDENTITY(1,1) NOT NULL,
	ConcurrencyStamp nvarchar			(max) NULL,
	Name								nvarchar(256) NULL,
	NormalizedName						nvarchar(256) NULL,
	IsSytem								bit default(0) not null
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

CREATE TABLE dbo.AppUser
(		AppUserId								bigint primary key identity(1,1),
		IdentityUserId							bigint not null,
		FirstName								nvarchar(256) not null,
        LastName								nvarchar(256) not null,
        Bio										nvarchar(2000),
        Email									nvarchar(256) not null,
        Address									nvarchar(256)  null,      
        PhoneNumber								nvarchar(256)  null,        
        DOB										nvarchar(256)  null,
        ProfilePicture							nvarchar(256)  null,
		Gender									nvarchar(1) null, -- m:male, f:female, o:other
		CompanyId								int default(0) not null,
		TheaterId								int default(0) not null,
		DeviceId								nvarchar(256),
		DeviceOs								nvarchar(256),
		IsActive                                bit NOT NULL Default(1),
		IsDeleted                               bit NOT NULL Default(0),
		AddedOn                                 datetime NOT NULL Default(GETUTCDATE()),
		AddedBy                                 national character varying(256)
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

CREATE TABLE dbo.Page
(
	PageId									bigint primary key identity(1,1) not null,
	Name									nvarchar(256) NOT NULL,
	URL										nvarchar(256),	
	Content									nvarchar(max) NULL,
	ContentConfig							nvarchar(max)  null,
	UseMasterLayout							bit default(0) not null,
    IsPublished								bit default(0) not null,
	Culture									nvarchar (10)  not null,
	LastModified							datetime NOT NULL,
	LastRequested							datetime NULL,
	IsSystem								bit default(1),
	IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(GETUTCDATE()),
    AddedBy                                 national character varying(256)

);
CREATE  INDEX idx_page
	ON dbo.Page(URL);
	
CREATE TABLE dbo.Module
(
	ModuleId								int primary key identity(1,1) not null,
	Name									nvarchar(256) not null,
	Description								nvarchar(max) ,
	Version									nvarchar(256) not null,
	IsInstalled								bit default(0) not null,
	Author									nvarchar(256),
	IsActive                                bit NOT NULL Default(1),
    IsDeleted								bit NOT NULL DEFAULT(0),
    AddedOn									datetime NOT NULL DEFAULT(GETUTCDATE()),
    AddedBy									national character VARYING(256)

);

CREATE TABLE dbo.HtmlContent
(
	HtmlContentId							bigint primary key identity(1,1),
	KeyName									nvarchar(256) not null,
	Content									nvarchar(MAX) ,
	IsMarkDown								bit default(0) not null,
	Culture									varchar (10)  not null,
	IsActive								bit default(1) not null,
	IsDeleted								bit default(0) not null,
	AddedOn									datetime default(getutcdate()),
	AddedBy									nvarchar(256)
);
CREATE INDEX idx_htc on dbo.HtmlContent(KeyName)

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
	AddedBy									nvarchar(256)
);

Create Table dbo.MenuType
(
	MenuTypeId								int primary key identity(1,1) not null,
	Name									nvarchar(256) not null,
	Description								nvarchar(max) ,
	IsActive                                bit NOT NULL Default(1),
    IsDeleted								bit NOT NULL DEFAULT(0),
    AddedOn									datetime NOT NULL DEFAULT(GETUTCDATE()),
    AddedBy									national character VARYING(256)
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
	GroupName								nvarchar(256) not null,--main,side side2
	Culture									nvarchar (10)  not null,
	IsBackend								bit default(0) not null,
	IsSystem								bit default(0) not null,
	IsActive								bit default(1) not null,
	IsDeleted								bit default(0) not null,
	AddedOn									datetime default(getutcdate()),
	AddedBy									nvarchar(256)
);
Create table dbo.MenuPermission
(
	MenuPermissionId						int primary key identity(1,1) not null,
	MenuId									int not null default(0),
	AllowAccessForAll						bit default(0) not null,
	AllowAccess								bit default(0) not null,
	RoleId									bigint not null,
	AddedOn									datetime default(getutcdate()),
	AddedBy									nvarchar(256)
);
Create Table dbo.MenuSetting
(	
	MenuSettingId							int primary key identity(1,1) not null,
	MenuGroupName							nvarchar(256) not null,
	ShowMenuAs								int references dbo.MenuType,--sidebar footer footer2 
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
    AddedBy                                 national character varying(256)

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
	Longitude								decimal(16,4) default(0),
	Lattitude								decimal(16,4) default(0),	
	Logo									nvarchar(256) not null,
	BaseCulture 							nvarchar(10) not null,
	BaseCurrency							nvarchar(5) not null,		
	CurrencyCode							nvarchar(5) not null,
	GoogleAnalyticScript 					nvarchar(1000) null,
	UseHttps								bit default(0) NOT NULL	

);

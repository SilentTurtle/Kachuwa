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
	Content									nvarchar(max) NOT NULL,
	UseMasterLayout							bit default(0) not null,
    IsPublished								bit default(0) not null,
	Culture									varchar (10)  not null,
	LastModified							datetime NOT NULL,
	LastRequested							datetime NULL,
	IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(GETDATE()),
    AddedBy                                 national character varying(256)

);

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
    AddedOn									datetime NOT NULL DEFAULT(GETDATE()),
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
	AddedOn									datetime default(getdate()),
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
	AddedOn									datetime default(getdate()),
	AddedBy									nvarchar(256)
);
Create Table dbo.MenuType
(
	MenuTypeId								int primary key identity(1,1) not null,
	Name									nvarchar(256) not null,
	Description								nvarchar(max) ,
	IsActive                                bit NOT NULL Default(1),
    IsDeleted								bit NOT NULL DEFAULT(0),
    AddedOn									datetime NOT NULL DEFAULT(GETDATE()),
    AddedBy									national character VARYING(256)
)

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
	IsBackend								bit default(0) not null,
	IsActive								bit default(1) not null,
	IsDeleted								bit default(0) not null,
	AddedOn									datetime default(getdate()),
	AddedBy									nvarchar(256)
);
Create Table dbo.MenuSetting
(	
	MenuSettingId							int primary key identity(1,1) not null,
	MenuGroupName							nvarchar(256) not null,
	ShowMenuAs								int references dbo.MenuType,--sidebar footer footer2 
	CssClasses								nvarchar(256)

);
CREATE table dbo.Setting
(
	SettingId								int primary key identity(1,1) not null,
	WebsiteName								nvarchar(256) not null,
	Description								nvarchar(500),
	Country									nvarchar(256),	
	Address1								nvarchar(256),
	Address2								nvarchar(256), 
	State 									nvarchar(256),
	City									nvarchar(256),
	Longitude								decimal(16,4) default(0),
	Lattitude								decimal(16,4) default(0),	
	Logo									nvarchar(256) not null,
	DefaultLanguage							nvarchar(50) not null,
	DefaultCurrency							nvarchar(5) not null,		
	CurrencyCode							nvarchar(5) not null
)


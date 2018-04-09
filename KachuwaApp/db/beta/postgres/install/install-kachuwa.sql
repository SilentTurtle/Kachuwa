CREATE TABLE IdentityRole
(
	Id 									SERIAL PRIMARY KEY,
	ConcurrencyStamp 					national character varying null,
	Name								national character varying(256),
	NormalizedName						national character varying(256) 
);
CREATE TABLE IdentityRoleClaim
(
	Id									SERIAL PRIMARY KEY,
	ClaimType							national character varying,
	ClaimValue							national character varying,
	RoleId								integer references IdentityRole NOT NULL
);
CREATE TABLE IdentityUser
(
	Id									SERIAL PRIMARY KEY,
	AccessFailedCount					integer NOT NULL,
	ConcurrencyStamp					national character varying NULL,
	Email								national character varying(256) NULL,
	EmailConfirmed						boolean not null,
	LockoutEnabled						boolean not null,
	LockoutEnd							timestamp NULL,
	NormalizedEmail						national character varying(256) NULL,
	NormalizedUserName					national character varying(256) NULL,
	PasswordHash						national character varying NULL,
	PhoneNumber							national character varying NULL,
	PhoneNumberConfirmed				boolean not null,
	SecurityStamp						national character varying NULL,
	TwoFactorEnabled					boolean not null,
	UserName							national character varying(256) NULL
);

CREATE TABLE IdentityUserClaim
(
	Id 									SERIAL PRIMARY KEY,
	ClaimType							national character varying NULL,
	ClaimValue							national character varying NULL,
	UserId								integer  NOT NULL references IdentityUser
 );

CREATE TABLE IdentityUserLogin
(
	LoginProvider						national character varying(450) NOT NULL,--index
	ProviderKey							national character varying(450) NOT NULL,--
	ProviderDisplayName					national character varying ,
	UserId								integer  NOT NULL references IdentityUser
);	
--drop index IdentityUserLogin.idx_ext_lgn 
CREATE  INDEX  idx_ext_lgn
	ON IdentityUserLogin(LoginProvider, ProviderKey);

CREATE TABLE IdentityUserRole
(
	UserId								integer NOT NULL references IdentityUser,
	RoleId								integer NOT NULL references IdentityRole
);
--ok
--drop index IdentityUserRole.idx_usr_lgn 
CREATE INDEX idx_usr_lgn
	ON IdentityUserRole 					(UserId, RoleId);

CREATE TABLE IdentityUserToken
(
	UserId								integer NOT NULL references IdentityUser,
	LoginProvider						national character varying(450) NOT NULL,
	Name								national character varying(450) NOT NULL,
	Value								national character varying NULL
);
--drop index IdentityUserToken.idx_usr_tkns 
--CREATE  INDEX idx_usr_tkns
--	ON IdentityUserToken 					(UserId, LoginProvider)
--	include (Name)

CREATE TABLE AppUser
(		AppUserId								SERIAL PRIMARY KEY,
		IdentityUserId							integer not null,
		FirstName								national character varying(256) not null,
        LastName								national character varying(256) not null,
        Bio										national character varying(2000),
        Email									national character varying(256) not null,
        Address									national character varying(256)  null,      
        PhoneNumber								national character varying(256)  null,        
        DOB										national character varying(256)  null,
        ProfilePicture							national character varying(256)  null,		 
		IsActive                                boolean not null Default(true),
		IsDeleted                               boolean not null Default(false),
		AddedOn                                 date NOT NULL Default(now()),
		AddedBy                                 national character varying(256)
);
CREATE TABLE LocaleResource 
(
    LocaleResourceId				SERIAL PRIMARY KEY,
    Culture							varchar (10)    not null,
    Name							varchar (100)   not null,
    Value							national character varying(4000) not null,
    GroupName						national character varying(256)
);
CREATE  INDEX  idx_localization
	ON LocaleResource (Culture, GroupName);
--drop index LocaleResource.idx_localization 

CREATE TABLE Page
(
	PageId									SERIAL PRIMARY KEY,
	Name									national character varying(256) NOT NULL,
	URL										national character varying(256),	
	Content									national character varying NULL,
	ContentConfig							national character varying  null,
	UseMasterLayout							boolean default(false) not null,
    IsPublished								boolean default(false) not null,
	Culture									national character varying(10)  not null,
	LastModified							timestamp NOT NULL,
	LastRequested							timestamp NULL,
	IsActive                                boolean not null Default(true),
	IsDeleted                               boolean not null Default(false),
    AddedOn                                 timestamp NOT NULL Default(now()),
    AddedBy                                 national character varying(256)

);
CREATE  INDEX  idx_page
	ON Page(URL);
	
CREATE TABLE Module
(
	ModuleId								SERIAL PRIMARY KEY,
	Name									national character varying(256) not null,
	Description								national character varying ,
	Version									national character varying(256) not null,
	IsInstalled								boolean default(false) not null,
	Author									national character varying(256),
	IsActive                                boolean not null Default(true),
    IsDeleted								boolean not null DEFAULT(false),
    AddedOn									timestamp NOT NULL DEFAULT(now()),
    AddedBy									national character VARYING(256)

);

CREATE TABLE HtmlContent
(
	HtmlContentId							SERIAL PRIMARY KEY,
	KeyName									national character varying(256) not null,
	Content									national character varying ,
	IsMarkDown								boolean default(false) not null,
	Culture									varchar(10)  not null,
	IsActive								boolean default(true) not null,
	IsDeleted								boolean default(false) not null,
	AddedOn									timestamp default(now()),
	AddedBy									national character varying(256)
);
CREATE INDEX  idx_htc on HtmlContent(KeyName);

CREATE TABLE Plugin
(
	PluginId								SERIAL PRIMARY KEY,
	PluginType								integer not null,
	Name									national character varying(256) not null,
	SystemName								national character varying(256) not null,
	Image									national character varying(256) ,
	Version									national character varying(256) not null,
	Author									national character varying(256) not null,
	Description								national character varying,
	IsInstalled								boolean default(false),
	IsActive								boolean default(true) not null,
	IsDeleted								boolean default(false) not null,
	AddedOn									timestamp default(now()),
	AddedBy									national character varying(256)
);

Create Table MenuType
(
	MenuTypeId								SERIAL PRIMARY KEY,
	Name									national character varying(256) not null,
	Description								national character varying ,
	IsActive                                boolean not null Default(true),
    IsDeleted								boolean not null DEFAULT(false),
    AddedOn									timestamp NOT NULL DEFAULT(now()),
    AddedBy									national character VARYING(256)
);

Create Table Menu
(
	MenuId									SERIAL PRIMARY KEY,
	Name									national character varying(256) not null,
	SubTitle								national character varying(256),
	Url										national character varying(256) not null,
	Icon									national character varying(256),
	CssClass								national character varying(256),
	IsChild									boolean default(false) not null,
	ParentId								integer default(0) not null,
	MenuOrder								integer default(0) not null,
	GroupName								national character varying(256) not null,--main,side side2
	Culture									national character varying(10)  not null,
	IsBackend								boolean default(false) not null,
	IsActive								boolean default(true) not null,
	IsDeleted								boolean default(false) not null,
	AddedOn									timestamp default(now()),
	AddedBy									national character varying(256)
);
Create Table MenuSetting
(	
	MenuSettingId							SERIAL PRIMARY KEY,
	MenuGroupName							national character varying(256) not null,
	ShowMenuAs								integer references MenuType,--sidebar footer footer2 
	CssClasses								national character varying(256)

);
CREATE TABLE SEO
(
	SEOId									SERIAL PRIMARY KEY,
	MetaTitle								national character varying(256),
	MetaKeyWords							national character varying(256),
	MetaDescription							national character varying,
	SeoType									national character varying(256) not null,--page,product..
	LastUrl									national character varying(256),
	Url										national character varying(256)not null,
	Image									national character varying(256),
	PageName								national character varying(256),
	PageId								 	integer default(0),
	ProductId								integer default(0),
	IsActive                                boolean not null Default(true),
    IsDeleted                               boolean not null Default(false),
    AddedOn                                 timestamp NOT NULL Default(now()),
    AddedBy                                 national character varying(256)

);
CREATE table Setting
(
	SettingId								SERIAL PRIMARY KEY,
	WebsiteName								national character varying(256) not null,
	Description								national character varying(500),
	Country									national character varying(256),	
	Address1								national character varying(256),
	Address2								national character varying(256),
	PhoneNumber								national character varying(256),
	Email									national character varying(256), 	
	State 									national character varying(256),
	City									national character varying(256),
	Longitude								decimal(16,4) default(0),
	Lattitude								decimal(16,4) default(0),	
	Logo									national character varying(256) not null,
	BaseCulture 							national character varying(10) not null,
	BaseCurrency							national character varying(5) not null,		
	CurrencyCode							national character varying(5) not null,
	GoogleAnalyticScript 					national character varying(1000) null,
	UseHttps								boolean default(false) NOT NULL	

);

Insert into Page(Name,URL,Content,ContentConfig,UseMasterLayout, IsPublished,Culture,LastModified,LastRequested,IsActive,IsDeleted,
    AddedOn, AddedBy )
    Select 'Home','landing','This is page Content','',true,true,'en-us','2017/1/1','2017/1/1',true,false,'2017/1/1','Admin'	;

Insert into Setting(WebsiteName	,Description,Country,Address1,Address2	,PhoneNumber,Email,State ,City,Longitude,Lattitude,Logo	,
	BaseCulture,BaseCurrency,CurrencyCode,GoogleAnalyticScript ,UseHttps) Select 'Kachuwa Demo Website','This is demo website.','Nepal','Kathmandu','Balkumari','987654321,987654123','info@kachuwaframework.com','Bagmati','Ktm',0,0,'/images/logo.png','en-us',N'$','USD','',false;



Insert into IdentityRole (ConcurrencyStamp ,Name,	NormalizedName	)
Select  NULL, N'Admin', N'ADMIN'
Union  Select  NULL, N'User', N'User'
Union Select NULL, N'Guest', N'GUEST';

INSERT into IdentityUser(AccessFailedCount, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled,
 LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber, PhoneNumberConfirmed, SecurityStamp,
  TwoFactorEnabled, UserName) VALUES (0, N'4b61d65e-b2bf-4d81-825d-7322c885bb06', N'ADMIN@KACHUWAFRAMEWORK.COM', false, true,
   NULL, NULL, NULL, N'AQAAAAEAACcQAAAAEHrwK3kFYF74DKSajkZ89I8sgaB7LKquxP0a4+8bkpgPSQGrg/pFJRXOOXxOeDzP9g==', NULL,
    false, N'df5b1a7e-e735-4164-8fa0-ec96decf5e4a', false, N'admin@kachuwaframework.com');


Insert into IdentityUserRole Select 1, 1 Union Select 1, 2;
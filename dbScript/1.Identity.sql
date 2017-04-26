CREATE TABLE dbo.IdentityRole
(
	Id 									bigint primary key IDENTITY(1,1) NOT NULL,
	ConcurrencyStamp nvarchar			(max) NULL,
	Name								nvarchar(256) NULL,
	NormalizedName						nvarchar(256) NULL
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
	UserId								bigint NOT NULL references dbo.IdentityRole,
	RoleId								bigint NOT NULL references dbo.IdentityUser
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



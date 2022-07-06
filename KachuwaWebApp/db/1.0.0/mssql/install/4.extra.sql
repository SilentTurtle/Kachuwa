CREATE TABLE dbo.SMSGateway
( 
	SMSGatewayId										int primary key identity(1,1) not null,
	Name												nvarchar(256) not null,
	Description										nvarchar(1000),
	Image												nvarchar(256),
	IsDefault											bit default(0) not null,
    IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(getDATE()),
    AddedBy                                 bigint not null default(0),
	DeletedBy								bigint not null default(0),
	DeletedOn                               datetime Default(getDATE()),
	UpdatedOn                               datetime NOT NULL Default(getDATE()),
    UpdatedBy                               bigint not null default(0)

);
CREATE TABLE dbo.SMSGatewaySetting
( 
   SMSGatewaySettingId								int primary key identity(1,1) not null,
   SMSGatewayId										int not null,
   GatewayKey										nvarchar(256) not null,
   GatewayValue										nvarchar(256) not null

);
Create Table dbo.SMSLog
(
	SMSLogId										bigint primary key identity(1,1) not null,
	[From]											nvarchar(256),
	[To]											nvarchar(256),	
	[Body]											nvarchar(max),	
	IsSent											bit default(0),
	IsDelivered										bit default(0),
	SentDate										datetime default(getutcdate()),
	DeliveredDate										datetime not null,
	GatewayResponse										nvarchar(1000),
	AddedOn											datetime default(getutcdate()),
	AddedBy											nvarchar(256) not null
);

CREATE TABLE dbo.EmailServiceProvider
( 
  EmailServiceProviderId									int primary key identity(1,1) not null,
  Name												nvarchar(256) not null,
  Description											nvarchar(1000),
  Image												nvarchar(256),
  IsDefault											bit default(0) not null,
 IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(getDATE()),
    AddedBy                                 bigint not null default(0),
	DeletedBy								bigint not null default(0),
	DeletedOn                               datetime Default(getDATE()),
	UpdatedOn                               datetime NOT NULL Default(getDATE()),
    UpdatedBy                               bigint not null default(0)

);
CREATE TABLE dbo.EmailServiceProviderSetting
( 
   EmailServiceProviderSettingId								int primary key identity(1,1) not null,
   EmailServiceProviderId									int not null,
   ProviderKey											nvarchar(256) not null,
   ProviderValue										nvarchar(256) not null

);
Create Table dbo.EmailLog
(
	EmailLogId										bigint primary key identity(1,1) not null,
	[From]											nvarchar(256),
	[To]											nvarchar(256),
	[Subject]										nvarchar(500),
	[Body]											nvarchar(max),
	[CC]											nvarchar(2000),
	[BCC]											nvarchar(2000),
	IsSent											bit default(0),
	IsDelivered										bit default(0),
	SentDate										datetime default(getutcdate()),
	DeliveredDate										datetime not null,
	GatewayResponse										nvarchar(1000),
	AddedOn											datetime default(getutcdate()) not null,
	AddedBy											nvarchar(256) not null
);
--ipv4 ipv6 domain email creditcard customer
Create Table dbo.RestrictionKey
(
	RestrictionKeyId									int primary key identity(1,1) not null,	 
	Name											nvarchar(256) not null,
	IsSystem										bit default(0) not null,
	IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(getDATE()),
    AddedBy                                 bigint not null default(0),
	DeletedBy								bigint not null default(0),
	DeletedOn                               datetime Default(getDATE()),
	UpdatedOn                               datetime NOT NULL Default(getDATE()),
    UpdatedBy                               bigint not null default(0)
	 
);
Create table dbo.Restriction
(
	RestrictionId										int primary key identity(1,1) not null,
	RestrictionKeyId									int references dbo.RestrictionKey,
	Value											nvarchar(256) not null,
	Reason											nvarchar(500) not null,
	Narration										nvarchar(500),
	IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(getDATE()),
    AddedBy                                 bigint not null default(0),
	DeletedBy								bigint not null default(0),
	DeletedOn                               datetime Default(getDATE()),
	UpdatedOn                               datetime NOT NULL Default(getDATE()),
    UpdatedBy                               bigint not null default(0)
);
create table dbo.AdministrativeIPAccess
(
	AdministrativeIPAccessId						int primary key identity(1,1) not null,
	RoleId											bigint not null,
	AllowIPV4										nvarchar(256) not null,--*
	AllowIPV6										nvarchar(500) not null, --*
	ActivateIPV6									bit default(0) not null,--if true check only ipv6 if not ipv4
	IsRange											bit default(0) not null,
	IPV4Range										nvarchar(500),--192.168.0.1-192.168.0.254
	IPV6Range										nvarchar(1000),--FE80:0:0:0:202:B3FF:FE1E:8329-FE80:0:0:0:202:B3FF:FE1E:8329	
	IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(getDATE()),
    AddedBy                                 bigint not null default(0),
	DeletedBy								bigint not null default(0),
	DeletedOn                               datetime Default(getDATE()),
	UpdatedOn                               datetime NOT NULL Default(getDATE()),
    UpdatedBy                               bigint not null default(0)
);


Create Table dbo.EmailTemplate
(
	Id												int primary key identity(1,1) not null,
	[Key]											nvarchar(500) not null,
	Template										nvarchar(max),
	AvailableDataKeys								nvarchar(3000),
	IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(getDATE()),
    AddedBy                                 bigint not null default(0),
	DeletedBy								bigint not null default(0),
	DeletedOn                               datetime Default(getDATE()),
	UpdatedOn                               datetime NOT NULL Default(getDATE()),
    UpdatedBy                               bigint not null default(0)
);

Create Table dbo.SMSTemplate
(
	Id												int primary key identity(1,1) not null,
	[Key]											nvarchar(500) not null,
	Template										nvarchar(1000),
	AvailableDataKeys								nvarchar(1000),
	IsActive                                bit NOT NULL Default(1),
	IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(getDATE()),
    AddedBy                                 bigint not null default(0),
	DeletedBy								bigint not null default(0),
	DeletedOn                               datetime Default(getDATE()),
	UpdatedOn                               datetime NOT NULL Default(getDATE()),
    UpdatedBy                               bigint not null default(0)
);
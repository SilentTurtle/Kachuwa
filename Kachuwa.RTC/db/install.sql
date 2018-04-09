create table dbo.RTCUser
(
	RTCUserId				int primary key not null,
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
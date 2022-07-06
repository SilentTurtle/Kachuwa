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
create table dbo.Room
(
	RoomId														int primary key not null identity(1,1),	
	Name														nvarchar(256) not null,
	IsActive						                            bit default(1),
	AddedBy							                            nvarchar(256),
	AddedOn							                            datetime default(getdate()),
	UpDatedBy						                            nvarchar(256),
	UpdatedOn						                            datetime default(getdate()),
	IsDeleted						                            bit default(0)
);
create table dbo.ChatContentType
(
	ChatContentTypeId											int primary key not null identity(1,1),
	Code														nvarchar(5) not null,
	Name														nvarchar(256) not null,
	IsActive						                            bit default(1),
	AddedBy							                            nvarchar(256),
	AddedOn							                            datetime default(getdate()),
	UpDatedBy						                            nvarchar(256),
	UpdatedOn						                            datetime default(getdate()),
	IsDeleted						                            bit default(0)
);
create table dbo.ChatContent
(
	ChatContentId												int primary key not null identity(1,1),
	ChatContentCode												nvarchar(5) not null,
	Message														nvarchar(max) not null,
	ContentPath												   nvarchar(500),
	ContentName													nvarchar(256),
	ContentSize													nvarchar(256),
	SenderConnectionId											nvarchar(256),
	SenderId												    bigint not null default(0),
	SenderName													nvarchar(500),
	RecieverId												    bigint not null default(0),
	RecieverConnectionId										nvarchar(256),
	RecieverName												nvarchar(500),
	RoomId												        int not null default(0),
	RoomName													nvarchar(256),
	IsSendToRoom												bit default(0),
	IsActive						                            bit default(1),
	AddedBy							                            nvarchar(256),
	AddedOn							                            datetime default(getdate()),
	UpDatedBy						                            nvarchar(256),
	UpdatedOn						                            datetime default(getdate()),
	IsDeleted						                            bit default(0)
);

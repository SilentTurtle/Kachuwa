CREATE TABLE dbo.UserDevice
(
	UserDeviceId 												bigint not null primary key identity(1,1),
	UserId														bigint not null,
	DeviceId													nvarchar(500) not null,
	IsWeb														bit default(0),
	IsMobile    												bit default(0),	
	Browser	        											nvarchar(256),
	BrowserVersion												nvarchar(50),
	OS															nvarchar(256),
	Version														nvarchar(50),
	IsVerified													bit default(0),
	IsActive						                            bit default(1),
	AddedBy							                            nvarchar(256),
	AddedOn							                            datetime default(getdate()),
	UpDatedBy						                            nvarchar(256),
	UpdatedOn						                            datetime default(getdate()),
	IsDeleted						                            bit default(0)

);
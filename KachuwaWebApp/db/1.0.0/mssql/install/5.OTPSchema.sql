
CREATE TABLE dbo.UserSecretKey
(
UserSecretKeyId				bigint not null primary key identity(1,1),
UserId						bigint not null default(0),
SecretKey					nvarchar(500) not null,
AddedBy							                            nvarchar(256),
	AddedOn							                            datetime default(getdate()),
	UpDatedBy						                            nvarchar(256),
	UpdatedOn						                            datetime default(getdate()),
	IsDeleted						                            bit default(0)
);

CREATE TABLE dbo.OTPSetting
(
	OTPSettingId			bigint not null primary key identity(1,1),
	ExpiryTime				int not null default(60),--in seconds
	SendFromSms				bit default(1) not null,
	SendFromEmail			bit default(1) not null,
AddedBy							                            nvarchar(256),
	AddedOn							                            datetime default(getdate()),
	UpDatedBy						                            nvarchar(256),
	UpdatedOn						                            datetime default(getdate()),
	IsDeleted						                            bit default(0)

);
CREATE TABLE dbo.UserOTP
(
	UserOTPId					bigint not null primary key identity(1,1),
	UserId						bigint not null default(0),
	OTPCode						nvarchar(500) not null,
	IsExpired					bit default(0) not null
);
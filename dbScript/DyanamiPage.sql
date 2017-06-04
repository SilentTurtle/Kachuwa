CREATE TABLE dbo.Page
(
	PageId									bigint primary key identity(1,1) not null,
	Name									nvarchar(256) NOT NULL,
	URL										nvarchar(256),	
	Content									nvarchar(max) NOT NULL,
	IsPublished								bit default(0) not null,
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
	IsActive								bit default(1) not null,
	IsDeleted								bit default(0) not null,
	AddedOn									datetime default(getdate()),
	AddedBy									nvarchar(256)
);
CREATE INDEX idx_htc on dbo.HtmlContent(KeyName)



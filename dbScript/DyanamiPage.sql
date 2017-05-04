CREATE TABLE dbo.Page
(
	PageId									bigint primary key identity(1,1) not null,
	Name									nvarchar(256) NOT NULL,
	URL										nvarchar(256),
	Location								nvarchar(150) NOT NULL,
	Content									nvarchar(max) NOT NULL,
	IsPublished								bit default(0) not null,
	LastModified							datetime NOT NULL,
	LastRequested							datetime NULL,
	IsActive                                bit NOT NULL Default(1),
    IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(GETDATE()),
    AddedBy                                 national character varying(256)

);
CREATE INDEX idx_pge on dbo.Page(Location)
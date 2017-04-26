CREATE TABLE dbo.ViewContent
(
	ViewId									bigint primary key not null,
	Location								nvarchar(150) NOT NULL,
	Content									nvarchar(max) NOT NULL,
	LastModified							datetime NOT NULL,
	LastRequested							datetime NULL,
	IsActive                                bit NOT NULL Default(1),
    IsDeleted                               bit NOT NULL Default(0),
    AddedOn                                 datetime NOT NULL Default(GETDATE()),
    AddedBy                                 national character varying(256)

);
CREATE INDEX idx_view on dbo.ViewContent(Location)
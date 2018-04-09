
CREATE TABLE dbo.HtmlContent
(
	HtmlContentId				bigint primary key identity(1,1),
	KeyName					nvarchar(256) not null,
	Content					nvarchar(MAX) ,
	IsMarkDown				bit default(0) not null,
	IsActive				bit default(1) not null,
	IsDeleted				bit default(0) not null,
	AddedOn					datetime default(getdate()),
	AddedBy					nvarchar(256)
);
CREATE INDEX idx_htc on dbo.HtmlContent(KeyName)
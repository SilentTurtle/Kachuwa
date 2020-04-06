
CREATE TABLE dbo.HtmlContent
(
	HtmlContentId			bigint primary key identity(1,1),
	KeyName					nvarchar(256) not null,
	Content					nvarchar(MAX) ,
	IsMarkDown				bit default(0) not null,
	IsActive				bit default(1) not null,
	IsDeleted				bit default(0) not null,
	AddedOn					datetime default(getdate()),
	AddedBy					nvarchar(256) not null,
	UpdatedOn				datetime,
	UpdatedBy				nvarchar(256)
);
CREATE INDEX idx_htc on dbo.HtmlContent(KeyName)


declare @menuId int,@pageId int;

--dashboard
INSERT INTO [dbo].[Menu] Select 'Html Content','','/admin/html','home','material-icons md-18',0,0,1,1,'en-US',1,1,1,0,GETUTCDATE(),'system';
select @menuId=Scope_Identity();

--superadmin
INSERT INTO MenuPermission 	SELECT @menuId,0,1,1,GETUTCDATE(),'system' ;
--admin
INSERT INTO MenuPermission 	SELECT @menuId,0,1,2,GETUTCDATE(),'system' ;


Insert Into dbo.Page Select 'Html Content','admin/html','','',1,1,1,'en-us',GETUTCDATE(),GETUTCDATE(),1,1,0,GETUTCDATE(),'System';

select @pageId=Scope_Identity();
--superadmin
INSERT INTO PagePermission 	SELECT @pageId,0,1,1,GETUTCDATE(),'system' ;
--admin
INSERT INTO PagePermission 	SELECT @pageId,0,1,2,GETUTCDATE(),'system';
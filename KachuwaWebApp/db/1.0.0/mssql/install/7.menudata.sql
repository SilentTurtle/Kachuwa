truncate table menu;
truncate table menupermission;

declare @bMenuId int

INSERT INTO [dbo].[Menu] Select 'Dashboard','','/admin/dashboard','fas fa-home','material-icons md-18',0,0,1,1,'en-US',1,1,1,0,GETDATE(),'system';

--WEB
INSERT INTO [dbo].[Menu] Select 'Web','','#','language','material-icons md-18',0,0,1,1,'en-US',1,1,1,0,GETDATE(),'system';
set @bMenuId=Scope_Identity();
INSERT INTO [dbo].[Menu] Select 'Page','','/admin/page','pages','material-icons md-18',0,@bMenuId,1,1,'en-US',1,1,1,0,GETDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Menu','','/admin/menu','group_add','material-icons md-18',0,@bMenuId,1,1,'en-US',1,1,1,0,GETDATE(),'system';
--INSERT INTO [dbo].[Menu] Select 'Module','','/admin/module','web','material-icons md-18',0,@bMenuId,1,1,'en-US',1,1,1,0,GETDATE(),'system';
--INSERT INTO [dbo].[Menu] Select 'Plugin','','/admin/plugins','web','material-icons md-18',0,@bMenuId,1,1,'en-US',1,1,1,0,GETDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'User','','/admin/user','people','material-icons md-18',0,@bMenuId,1,1,'en-US',1,1,1,0,GETDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Role','','/admin/role','group_add','material-icons md-18',0,@bMenuId,1,1,'en-US',1,1,1,0,GETDATE(),'system';
--INSERT INTO [dbo].[Menu] Select 'Localization','','/admin/localization','g_translate','material-icons md-18',0,@bMenuId,1,1,'en-US',1,1,1,0,GETDATE(),'system';
--INSERT INTO [dbo].[Menu] Select 'Audit Logs','','/admin/audit','folder_open','material-icons md-18',0,@bMenuId,1,1,'en-US',1,1,1,0,GETDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Settings','','#','settings','material-icons md-18',0,0,1,1,'en-US',1,1,1,0,GETDATE(),'system';
set @bMenuId=Scope_Identity();
INSERT INTO [dbo].[Menu] Select 'Security','','/admin/setting/security','security','material-icons md-18',0,@bMenuId,1,1,'en-US',1,1,1,0,GETDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Web Setting','','/admin/setting/web','settings','material-icons md-18',0,@bMenuId,1,1,'en-US',1,1,1,0,GETDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Caching','','/admin/setting/caching','cached','material-icons md-18',0,@bMenuId,1,1,'en-US',1,1,1,0,GETDATE(),'system';


--superadmin
INSERT INTO MenuPermission 	SELECT MenuId,0,1,1,GETDATE(),'system' FROM dbo.Menu AS l;
--admin
INSERT INTO MenuPermission 	SELECT MenuId,0,1,2,GETDATE(),'system' FROM dbo.Menu AS l;

--INSERT INTO MenuPermission 	SELECT MenuId,0,1,1,GETDATE(),'system' FROM dbo.Menu  where Menuid=70
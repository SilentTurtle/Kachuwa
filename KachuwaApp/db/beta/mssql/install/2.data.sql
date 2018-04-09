
Insert Into dbo.Page Select 'Home','landing','This is page Content','',1,1,'en-us','2017/1/1','2017/1/1',1,1,0,'2017/1/1','Admin';	
Insert Into dbo.SEO Select 'Home','home','This is homepage','page','','/landing','','Home', SCOPE_IDENTITY(),0,1,0,'2017/1/1','Admin';
Insert Into dbo.Page Select 'Access Denied','accessdenied','No Permission','',1,1,'en-us','2017/1/1','2017/1/1',1,1,0,'2017/1/1','Admin';
Insert Into dbo.SEO Select 'Access Denied','Access Denied','No Permission','page','','/accessdenied','','Access Denied',SCOPE_IDENTITY(),0,1,0,'2017/1/1','Admin';
Insert Into dbo.Page Select 'Page Not Found','page-not-found','Page Not Found','',1,1,'en-us','2017/1/1','2017/1/1',1,1,0,'2017/1/1','Admin';
Insert Into dbo.SEO Select 'Page Not Found','Page Not Found','Page Not Found','page','','/page-not-found','','Page Not Found',SCOPE_IDENTITY(),0,1,0,'2017/1/1','Admin';

Insert Into dbo.Setting Select 'Kachuwa Demo Website','This is demo website.','Nepal','Kathmandu','Balkumari','987654321,987654123','info@kachuwaframework.com','Bagmati','Ktm',0,0,'/images/logo.png','en-us',N'$','USD','',0;


Insert Into dbo.IdentityRole Select  NULL, N'SuperAdmin', N'SUPERADMIN',1;
Insert Into dbo.IdentityRole Select  NULL, N'Admin', N'ADMIN',1;
Insert Into dbo.IdentityRole Select  NULL, N'User', N'User',1;
Insert Into dbo.IdentityRole Select NULL, N'Guest', N'GUEST',1;


declare @menuId int; 
--dashboard
INSERT INTO [dbo].[Menu] Select 'Dashboard','','/admin/dashboard','home','material-icons md-18',0,0,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
--WEB
INSERT INTO [dbo].[Menu] Select 'Web','','#','language','material-icons md-18',0,0,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
set @menuId=Scope_Identity();
INSERT INTO [dbo].[Menu] Select 'Page','','/admin/page','pages','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Module','','/admin/module','web','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Plugin','','/admin/plugin','web','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'User','','/admin/user','people','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Role','','/admin/role','group_add','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';

--THEMES
INSERT INTO [dbo].[Menu] Select 'Themes','','#','palette','material-icons md-18',0,0,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
set @menuId=Scope_Identity();
INSERT INTO [dbo].[Menu] Select 'Manage','','/admin/theme/manage','settings','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Customize','','/admin/theme/customize','reorder','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';

--DEV TOOLS
INSERT INTO [dbo].[Menu] Select 'Dev Tools','','#','build','material-icons md-18',0,0,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
set @menuId=Scope_Identity();
INSERT INTO [dbo].[Menu] Select 'Logs','','/admin/dev/log','error','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Sql','','/admin/dev/sql','search','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Cli','','/admin/dev/cli','computer','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
--SETTINGS
INSERT INTO [dbo].[Menu] Select 'Settings','','#','settings','material-icons md-18',0,0,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
set @menuId=Scope_Identity();
INSERT INTO [dbo].[Menu] Select 'Security','','/admin/setting/security','security','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Web Setting','','/admin/setting/web','settings','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';
INSERT INTO [dbo].[Menu] Select 'Caching','','/admin/setting/caching','cached','material-icons md-18',0,@menuId,1,'backendnav','en-US',1,1,1,0,GETUTCDATE(),'system';


--superadmin
INSERT INTO MenuPermission 	SELECT MenuId,0,1,1,GETUTCDATE(),'system' FROM menu AS l;
--admin
INSERT INTO MenuPermission 	SELECT MenuId,0,1,2,GETUTCDATE(),'system' FROM menu AS l ;

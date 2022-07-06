
Insert into dbo.LocaleRegion(CountryId,Flag,Culture,IsDefault,IsActive,IsDeleted,AddedOn, AddedBy) values(226,'en.png','en-us',1,1,0, getdate(), 1);
Insert Into dbo.Setting Select 'Kachuwa App','This is Kachuwa web app..','Nepal','Kathmandu','Balkumari','987654321,987654123','info@kachuwaframework.com','Bagmati','Ktm','+05:45', 'Nepal Standard Time',0,0,'/images/logo.png','en-us',N'$','USD','',0,'info@kachuwaframework.com','support@kachuwaframework.com','sales@kachuwaframework.com','marketing@kachuwaframework.com';
	
Insert Into dbo.IdentityRole Select  NULL, N'SuperAdmin', N'SUPERADMIN',0,1,1,0,getdate(),1,0,'','',0;
Insert Into dbo.IdentityRole Select  NULL, N'Admin', N'ADMIN',0,1,1,0,getdate(),1,0,'','',0;
Insert Into dbo.IdentityRole Select  NULL, N'User', N'User',0,1,1,0,getdate(),1,0,'','',0;
Insert Into dbo.IdentityRole Select NULL, N'Guest', N'GUEST',0,1,1,0,getdate(),1,0,'','',0;

--INSERT dbo.IdentityUser (AccessFailedCount, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled,
-- LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber, PhoneNumberConfirmed, SecurityStamp,
--  TwoFactorEnabled, UserName) VALUES (0, N'4b61d65e-b2bf-4d81-825d-7322c885bb06', N'SUPERUSER@EGTKT.COM', 1, 1,
--   NULL, NULL, NULL, N'AQAAAAEAACcQAAAAEHrwK3kFYF74DKSajkZ89I8sgaB7LKquxP0a4+8bkpgPSQGrg/pFJRXOOXxOeDzP9g==', NULL,
--    0, N'df5b1a7e-e735-4164-8fa0-ec96decf5e4a', 0, N'superuser@egtkt.com');

--Insert Into dbo.IdentityUserRole Select 1, 1;

--Insert into dbo.AppUser (IdentityUserId,FirstName,LastName,Email,Address,Gender,UserName,IsActive,AddedBy,UpdatedBy)
--values (1,'FirstName','LastName','superuser@egtkt.com','KTM','M','superuser',1,1,0);






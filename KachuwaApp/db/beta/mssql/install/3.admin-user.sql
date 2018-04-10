INSERT dbo.IdentityUser (AccessFailedCount, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled,
 LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber, PhoneNumberConfirmed, SecurityStamp,
  TwoFactorEnabled, UserName) VALUES (0, N'4b61d65e-b2bf-4d81-825d-7322c885bb06', N'SUPERUSER@KACHUWAFRAMEWORK.COM', 1, 1,
   NULL, NULL, NULL, N'AQAAAAEAACcQAAAAEHrwK3kFYF74DKSajkZ89I8sgaB7LKquxP0a4+8bkpgPSQGrg/pFJRXOOXxOeDzP9g==', NULL,
    0, N'df5b1a7e-e735-4164-8fa0-ec96decf5e4a', 0, N'superuser@kachuwaframework.com');

Insert Into dbo.IdentityUserRole Select 1, 1;
using System;
using Kachuwa.Identity.ClaimFactory;
using Kachuwa.Identity.Cryptography;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Service;
using Kachuwa.Identity.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Identity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureIdentityCryptography(this IServiceCollection services, IConfigurationSection configuration)
        {
            services.Configure<AESKeys>(configuration);
            services.AddSingleton<EncryptionHelper>();
            return services;
        }

        public static IdentityBuilder UseDapperWithSqlServer(this IdentityBuilder builder)
        {

            UseKachuwaIdentityStores(builder.Services, builder.UserType, builder.RoleType);

            return builder;
        }

        public static IdentityBuilder UseDapperWithSqlServer<TKey>(this IdentityBuilder builder)
        {

            UseKachuwaIdentityStores(builder.Services, builder.UserType, builder.RoleType, typeof(TKey));

            return builder;
        }


        public static IdentityBuilder UseDapperWithSqlServer<TKey, TUserRole, TRoleClaim>(this IdentityBuilder builder)
        {
           
            UseKachuwaIdentityStores(builder.Services, builder.UserType, builder.RoleType, typeof(TKey), typeof(TUserRole), typeof(TRoleClaim));

            return builder;
        }
       

        private static void UseKachuwaIdentityStores(IServiceCollection services, Type userType, Type roleType, Type keyType = null, Type userRoleType = null, Type roleClaimType = null, Type userClaimType = null, Type userLoginType = null)
        {
            keyType = keyType ?? typeof(int);
            userRoleType = userRoleType ?? typeof(KachuwaIdentityUserRole<>).MakeGenericType(keyType);
            roleClaimType = roleClaimType ?? typeof(KachuwaIdentityRoleClaim<>).MakeGenericType(keyType);
            userClaimType = userClaimType ?? typeof(KachuwaIdentityUserClaim<>).MakeGenericType(keyType);
            userLoginType = userLoginType ?? typeof(KachuwaIdentityUserLogin<>).MakeGenericType(keyType);

            var userStoreType = typeof(KachuwaUserStore<,,,,,,>).MakeGenericType(userType, keyType, userRoleType, roleClaimType,
                userClaimType, userLoginType, roleType);
            var roleStoreType = typeof(KachuwaRoleStore<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType);
            
            services.AddScoped<IIdentityRoleService, IdentityRoleService>();
            services.AddScoped<IIdentityUserService, IdentityUserService>();
            services.AddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
            services.AddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);
           
        }

        public static IServiceCollection AddIdentityServerUserClaimsPrincipalFactory<TUser, TRole>(this IServiceCollection services)
          where TUser : class
          where TRole : class
        {
            return services.AddTransient<IUserClaimsPrincipalFactory<TUser>, KachuwaClaimsPrincipalFactory<TUser, TRole>>();
        }


    }
}

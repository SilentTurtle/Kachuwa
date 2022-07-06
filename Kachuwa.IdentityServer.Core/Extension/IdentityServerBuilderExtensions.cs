using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.IdentityServer.Extension;

public static class IdentityServerBuilderExtensions
{
    public static IIdentityServerBuilder AddAspNetIdentity<TUser>(this IIdentityServerBuilder builder)
        where TUser : class
    {

        return builder.AddAspNetIdentity<TUser>("Identity.Application");
    }

    public static IIdentityServerBuilder AddAspNetIdentity<TUser>(this IIdentityServerBuilder builder, string authenticationScheme)
        where TUser : class
    {
        //    builder.Services.Configure<IdentityServerOptions>(options =>
        //    {
        //        options.Authentication. = authenticationScheme;
        //    });

        builder.Services.Configure<IdentityOptions>(options =>
        {

            options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
            options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
            options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;

            //if (options.OnSecurityStampRefreshingPrincipal == null)
            //{
            //    options.OnSecurityStampRefreshingPrincipal = SecurityStampValidatorCallback.UpdatePrincipal;
            //}
        });

        //    builder.AddResourceOwnerValidator<ResourceOwnerPasswordValidator<TUser>>();
        //    builder.AddProfileService<ProfileService<TUser>>();

        return builder;
    }
}
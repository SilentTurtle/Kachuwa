using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.IdentityServer.Security;

public static class IdentityServerIdentityBuilderExtensions
{
    public static IdentityBuilder AddIdentityServerUserClaimsPrincipalFactory(this IdentityBuilder builder)
    {
        var interfaceType = typeof(IUserClaimsPrincipalFactory<>);
        interfaceType = interfaceType.MakeGenericType(builder.UserType);

        var classType = typeof(KachuwaClaimsPrincipalFactory<,>);
        classType = classType.MakeGenericType(builder.UserType, builder.RoleType);

        builder.Services.AddScoped(interfaceType, classType);

        return builder;
    }
}
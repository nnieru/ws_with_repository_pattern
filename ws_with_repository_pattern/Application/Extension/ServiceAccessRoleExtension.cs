using Microsoft.AspNetCore.Authorization;
using ws_with_repository_pattern.Application.AuthorizationRequirement;

namespace ws_with_repository_pattern.Application.Extension;

public static class ServiceAccessRoleExtension
{
    public static void AddAccessRoles(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // role
            options.AddPolicy("AdminPolicy", policy =>
                policy.RequireRole("administrator"));
            options.AddPolicy("UserPolicy", policy =>
                policy.RequireRole("General"));
                
            // permission
            options.AddPolicy("read", policy =>
                policy.Requirements.Add(new PermissionRequirement("READ")));

            options.AddPolicy("write", policy =>
                policy.Requirements.Add(new PermissionRequirement("WRITE")));

            options.AddPolicy("delete", policy =>
                policy.Requirements.Add(new PermissionRequirement("DELETE")));
        });
        
        services.AddSingleton<IAuthorizationHandler, Permissionhandler>();
    }
}
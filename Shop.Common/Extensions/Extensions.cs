using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
namespace Shop.Common.Identity
{
    public static class Extensions
    {
        public static AuthenticationBuilder AddJwtBearerAuthentication(this IServiceCollection services)
        {
            return services.ConfigureOptions<ConfigureJwtBearerOptions>()
                    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        // .AddJwtBearer(options =>
                        // {
                        //     options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        //     {
                        //         //ValidateIssuerSigningKey = false,
                        //         SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                        //         {
                        //             var jwt = new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token);
                        //             return jwt;
                        //         }
                        //     };
                        //     options.Events = new JwtBearerEvents
                        //     {
                        //         OnAuthenticationFailed = async ctx =>
                        //         {
                        //             var putBreakpointHere = true;
                        //             var exceptionMessage = ctx.Exception;
                        //         },
                        //     };
                        // })
                    .AddJwtBearer();
        }
    }
}

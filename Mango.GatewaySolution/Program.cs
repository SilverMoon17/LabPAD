using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {

        options.Authority = builder.Configuration["ServiceUrls:IdentityServer"];
        options.MetadataAddress = builder.Configuration["ServiceUrls:MetadataAddress"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
        options.RequireHttpsMetadata = false;
        options.BackchannelHttpHandler = new JwtBearerBackChannelListener(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });
    });

builder.Services.AddOcelot();

var app = builder.Build();

await app.UseOcelot();

app.Run();
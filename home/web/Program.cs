using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nagiyu.Auth.Web.Controllers;
using Nagiyu.Auth.Web.Middlewares;
using Nagiyu.Common.Auth.Service.Services;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// サービス登録
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<AuthService>();

// 環境ごとの Kestrel 設定をロード
builder.WebHost.ConfigureKestrel(options =>
{
    var environment = builder.Environment.EnvironmentName;

    if (environment == "Production")
    {
        // 本番環境（Let’s Encrypt 証明書を使用）
        options.ConfigureHttpsDefaults(httpsOptions =>
        {
            // .pem ファイルを直接指定
            var certificate = X509Certificate2.CreateFromPemFile(
                builder.Configuration["Auth:Certificates:FullchainPath"],
                builder.Configuration["Auth:Certificates:PrivatekeyPath"]
            );

            // Kestrel に証明書を設定
            httpsOptions.ServerCertificate = certificate;
        });
    }
});

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddApplicationPart(typeof(AccountController).Assembly);

builder.Services
    .AddAuthentication(options =>
    {
        // サインインスキームを Cookies に設定
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie() // Cookie 認証を追加
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Auth:Credentials:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Auth:Credentials:Google:ClientSecret"];
    });

// ポリシー、ハンドラーの設定

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        // ヘルスチェックのパスの場合はリダイレクトしない
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await next();
        }
        else if (!context.Request.IsHttps)
        {
            // HTTPSリダイレクト
            var httpsUrl = $"https://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
            context.Response.Redirect(httpsUrl, permanent: true);
        }
        else
        {
            await next();
        }
    });
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<GoogleAuthMiddleware>();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // ベースプロジェクトのルーティング
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // ヘルスチェック（8080用）
    endpoints.Map("/health", () =>
    {
        return Results.Ok("Healthy");
    }).AllowAnonymous();
});

app.Run();

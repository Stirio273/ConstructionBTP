using Evaluation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Login/LoginUser";
    options.AccessDeniedPath = "/Error/Forbidden";
});


builder.Services.AddMvc(
       o => o.AddStringTrimModelBinderProvider()
);

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// using (StreamReader apacheModRewriteStreamReader =
//     File.OpenText("ApacheModRewrite.txt"))
// using (StreamReader iisUrlRewriteStreamReader =
//     File.OpenText("IISUrlRewrite.xml"))
// {
//     var options = new RewriteOptions()
//         .AddRedirect("redirect-rule/(.*)", "redirected/$1")
//         .AddRewrite(@"^rewrite-rule/(\d+)/(\d+)", "rewritten?var1=$1&var2=$2",
//             skipRemainingRules: true)
//         .AddApacheModRewrite(apacheModRewriteStreamReader)
//         .AddIISUrlRewrite(iisUrlRewriteStreamReader)
//         .Add(MethodRules.RedirectXmlFileRequests)
//         .Add(MethodRules.RewriteTextFileRequests)
//         .Add(new RedirectImageRequests(".png", "/png-images"))
//         .Add(new RedirectImageRequests(".jpg", "/jpg-images"));

//     app.UseRewriter(options);
// }

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

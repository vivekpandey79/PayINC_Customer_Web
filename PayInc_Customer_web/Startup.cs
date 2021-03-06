using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PayInc_Customer_web
{
    public class Startup
    {
        public static IDictionary<string, string> AppSetting = new Dictionary<string, string>();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();
            services.AddSession(opts =>
            {
                opts.Cookie.IsEssential = true; // make the session cookie Essential
            });
            services.AddHttpContextAccessor();
            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();
            
            services.AddControllersWithViews();
            #region SET ALL APP SETTINGS GET AND SET
            AppSetting.Add("WebApiUrl", Configuration.GetSection("AppSettings").GetSection("WebApiUrl").Value);
            AppSetting.Add("FileUploadUrl", Configuration.GetSection("AppSettings").GetSection("FileUploadUrl").Value);
            AppSetting.Add("ManualVideoVerifyUrl", Configuration.GetSection("AppSettings").GetSection("ManualVideoVerifyUrl").Value);
            AppSetting.Add("AadharOTP_RedirectURL", Configuration.GetSection("AppSettings").GetSection("AadharOTP_RedirectURL").Value);
            AppSetting.Add("Aadhar_ReturnBackURL", Configuration.GetSection("AppSettings").GetSection("Aadhar_ReturnBackURL").Value);
            AppSetting.Add("AadharVideoVerifyUrl", Configuration.GetSection("AppSettings").GetSection("AadharVideoVerifyUrl").Value);
            AppSetting.Add("HelpDeskUrl", Configuration.GetSection("AppSettings").GetSection("HelpDeskUrl").Value);
            AppSetting.Add("TransactionType", Configuration.GetSection("TransactionType").Value);
            #endregion
            services.AddMvc(option => option.EnableEndpointRouting = false);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area}/{controller=Home}/{action=Index}/{id?}"
                );
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
            });
            
        }
    }
}

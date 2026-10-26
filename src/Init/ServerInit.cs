using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace NihongoHub.Init{
    public static class ServerInit
    {
        public static void Setup()
        {
            var app = CreateApp();

            ConfigureRoute(app);
            try
            {
                app.Run();
            }
            catch(Exception ex)
            {
                NihongoHub.Logging.LogReporter.Fatal("启动HTTP服务端时出现致命问题 \n 可能是Cert配置问题",Logging.LogCategory.Http,ex);
            }
        }

        private static WebApplication CreateApp()
        {
            //创建一个WebApplicationBuilder实例
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                //指定目录
                ContentRootPath = AppContext.BaseDirectory,
                WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
            });

            ConfigureWebHost(builder);
            return builder.Build();

            //构建
            static void ConfigureWebHost(WebApplicationBuilder builder) {
                builder.WebHost.ConfigureKestrel(ConfigureKestrel);
            }

            //监听端口配置
            static void ConfigureKestrel(KestrelServerOptions options) {
                var server = NihongoHub.Init.ConfigInit.Current == null ? null : NihongoHub.Init.ConfigInit.Current.Server;
                
                if(server == null)
                {
                    NihongoHub.Logging.LogReporter.Fatal("Current配置存在问题",Logging.LogCategory.Unknown);
                }

                if (!System.Net.IPAddress.TryParse(server.Host, out var address))
                {
                    NihongoHub.Logging.LogReporter.Fatal("提供的Host不合法",Logging.LogCategory.Http);
                }

                if (!server.HttpsEnable)
                {
                    options.Listen(address,server.Port);
                    return;
                }

                options.Listen(address,server.Port,listenOptions =>
                {
                    ConfigureHttps(listenOptions, server);
                });
                
            }
            //https配置
            static void ConfigureHttps(ListenOptions listenOptions,NihongoHub.Models.Config.ServerConfig server) {
                listenOptions.UseHttps(server.PfxPath!, server.PfxPassword);
            }
        }

        private static void ConfigureRoute(WebApplication app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace NihongoHub.Init
{
    public static class ConfigInit
    {
        //供调用
        public static NihongoHub.Models.Config.AppConfig? Current {get;private set;}
        public static void Load()
        {
            var config = ReadOrCreateConfig();
            if (config == null)
            {
                Logging.LogReporter.Fatal("Config文件配置错误",Logging.LogCategory.File);
            }
            Current = config;

            ValidateCert();
        }

        private static void ValidateCert()
        {
            if(Current == null)
            {
                Logging.LogReporter.Fatal("Current配置出现问题：为空",Logging.LogCategory.Unknown);
                return;
            }

            if (!Current.Server.HttpsEnable)
            {
                return;
            }

            string certPath = Current.Server.PfxPath == null ? "null" : Current.Server.PfxPath;
            if (!File.Exists(certPath))
            {
                Logging.LogReporter.Fatal("Cert文件不存在/文件错误",Logging.LogCategory.File);
                return;
            }

            return;
        }

        //Json读写配置
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            RespectNullableAnnotations = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };
        private static NihongoHub.Models.Config.AppConfig? ReadOrCreateConfig()
        {
            

            string configPath = FindConfigFile();
            //文件不存在则新建
            if (configPath == "nofile")
            {
                configPath = Path.Combine(AppContext.BaseDirectory,"Config","config.json");
                CreateDefaultConfigFile(configPath);
            }

            try
            {
                string json = File.ReadAllText(configPath);


                var config = JsonSerializer.Deserialize<NihongoHub.Models.Config.AppConfig>(json,JsonOptions);

                if(config == null)
                {
                   return null;
                }

                return config;
            }
            catch (JsonException)
            {
                Logging.LogReporter.Fatal("Json序列化错误",Logging.LogCategory.Json);
                return null;
            }
            catch (Exception)
            {
                Logging.LogReporter.Fatal("Json序列化时的其他错误",Logging.LogCategory.Unknown);
                return null;
            }

            static string FindConfigFile()
            {
                string path = Path.Combine(AppContext.BaseDirectory,"Config");
                Directory.CreateDirectory(path);
                string jsonPath = Path.Combine(path,"config.json");
                if (!File.Exists(jsonPath)) return "nofile";
                return jsonPath;
            }

            static void CreateDefaultConfigFile(string filePath)
            {
                var config = new NihongoHub.Models.Config.AppConfig
                {
                    Server = new()
                    {
                        Host = "0.0.0.0",
                        Port = 5000,
                        HttpsEnable = false,
                        PfxPath = "",
                        PfxPassword = ""   
                    },
                    Database = new()
                    {
                        FileName = Path.Combine("Config","nihongo.db")
                    },
                    Jwt = new()
                    {
                        Key = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
                        Issuer = "NihongoHub",
                        ExpireMinutes = 10080
                    }
                };
                string json = JsonSerializer.Serialize(config,JsonOptions);
                File.WriteAllText(filePath,json);
                
                return;
            }
        }
    }
}
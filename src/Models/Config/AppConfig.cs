namespace NihongoHub.Models.Config
{
    public class AppConfig
    {
        public required ServerConfig Server {get;set;}
        public required DatabaseConfig Database {get;set;}
        public required JwtConfig Jwt {get;set;}
    }
    public class ServerConfig
    {
        public required string Host {get;set;}
        public required int Port {get;set;}
        [System.Text.Json.Serialization.JsonPropertyName("HttpsEnable")]
        public required bool HttpsEnabled {get;set;}
        public string? PfxPath {get;set;}
        public string? PfxPassword {get;set;}
    }

    public class DatabaseConfig
    {
        public required string FileName {get;set;}
    }
    public class JwtConfig
    {
        public required string Key {get;set;}
        public required string Issuer {get;set;}
        [System.Text.Json.Serialization.JsonPropertyName("ExpireMinutes")]
        public required int ExpirationMinutes {get;set;}
    }
}
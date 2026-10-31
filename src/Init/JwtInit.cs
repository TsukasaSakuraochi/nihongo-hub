using Microsoft.IdentityModel.Tokens;

namespace NihongoHub.Init
{
    public class JwtValidationConfig
    {
        public JwtValidationConfig()
        {
            var jwtConfig = ConfigInit.Current.Jwt;

            ValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Convert.FromBase64String(jwtConfig.Key)
                ),
                ValidateIssuer = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.FromSeconds(30),

                ValidAlgorithms = new[]
                {
                     SecurityAlgorithms.HmacSha256
                }
            };
        }

        public TokenValidationParameters ValidationParameters
        {
            get;
        }
    }
    public static class JwtInit
    {
        private static JwtValidationConfig? _jwtConfig;
        public static JwtValidationConfig Current
        {
            get
            {
                if(_jwtConfig == null)
                {
                    Logging.LogReporter.Fatal("[内部错误]Jwt验证配置未能初始化",Logging.LogCategory.Unknown);
                }
                return _jwtConfig;
            }
        }

        public static void Init()
        {
            _jwtConfig = new();
        }
    }
}
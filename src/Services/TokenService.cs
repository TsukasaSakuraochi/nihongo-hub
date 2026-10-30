using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace NihongoHub.Services
{
    /// <summary>
    /// 仅作为传递的封装，不加入Models
    /// </summary>
    public class TokenUserData
    {
        public required int userid {get;set;}
        public required string username {set;get;}
        public required Models.Entities.UserType role {get;set;}
    }

    public static class TokenService
    {
        public static void GenerateToken(TokenUserData userdata,out string token,out DateTime expiresAt)
        {
            ///后续有计划加入关于用户Ip的部分
        
            var jwtConfig = Init.ConfigInit.Current.Jwt;

            //计算签发时间和过期时间
            DateTime validFrom = DateTime.UtcNow;
            expiresAt = validFrom.AddMinutes(jwtConfig.ExpireMinutes);

            //从 Base64 还原签名密钥
            byte[] keyBytes = Convert.FromBase64String(jwtConfig.Key);

            SymmetricSecurityKey securityKey = new(keyBytes);

            //创建签名凭证

            SigningCredentials signingCredentials = new(securityKey,SecurityAlgorithms.HmacSha256);

            //设置 JWT Claims
            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Sub,userdata.userid.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName,userdata.username),
                new Claim("role",userdata.role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            //创建 JWT 对象

            JwtSecurityToken jwt = new(
                issuer: jwtConfig.Issuer, 
                claims: claims, 
                notBefore: validFrom,
                expires: expiresAt,
                signingCredentials: signingCredentials
            );
            token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return;
        }
    }
}
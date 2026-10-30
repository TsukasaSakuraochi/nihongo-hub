using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace NihongoHub.Services
{
    public static class AuthService
    {
        public static bool ValidateToken(string token, ref TokenUserData? userData,ref string hint,ref string errorCode)
        {
            userData = null;
            hint = "";
            errorCode = "";

            if (string.IsNullOrWhiteSpace(token))
            {
                hint = "[ERROR]token为空";
                errorCode = "TOKEN_MISSING";
                return false;
            }

            //获取预先初始化的验证参数
            var validationParameters = Init.JwtInit.Current.ValidationParameters;

            //创建 JWT 处理器
            JwtSecurityTokenHandler handler = new()
            {
                MapInboundClaims = false
            };

            ClaimsPrincipal principal;

            try
            {
                principal = handler.ValidateToken(
                    token,
                    validationParameters,
                    out _
                );
            }
            catch (SecurityTokenExpiredException)
            {
                hint = "[ERROR]token已过期";
                errorCode = "TOKEN_EXPIRED";
                return false;
            }
            catch (SecurityTokenNotYetValidException)
            {
                hint = "[ERROR]token尚未生效";
                errorCode = "TOKEN_NOT_YET_VALID";
                return false;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                hint = "[ERROR]token签名无效";
                errorCode = "TOKEN_INVALID_SIGNATURE";
                return false;
            }
            catch (SecurityTokenInvalidIssuerException)
            {
                hint = "[ERROR]token签发者无效";
                errorCode = "TOKEN_INVALID_ISSUER";
                return false;
            }
            catch (SecurityTokenException)
            {
                hint = "[ERROR]token无效";
                errorCode = "TOKEN_INVALID";
                return false;
            }
            catch (ArgumentException)
            {
                hint = "[ERROR]token格式无效";
                errorCode = "TOKEN_INVALID";
                return false;
            }

            string? useridValue = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            string? username = principal.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;

            string? roleValue = principal.FindFirst("role")?.Value;


            if (!int.TryParse(useridValue, out int userid) || userid <= 0)
            {
                hint = "[ERROR]token中的userid无效";
                errorCode = "TOKEN_CLAIMS_INVALID";
                return false;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                hint = "[ERROR]token中的username无效";
                errorCode = "TOKEN_CLAIMS_INVALID";
                return false;
            }

            if(!Enum.TryParse(roleValue,true,out Models.Entities.UserType role) || !Enum.IsDefined(typeof(Models.Entities.UserType),role))
            {
                hint = "[ERROR]token中的role无效";
                errorCode = "TOKEN_CLAIMS_INVALID";
                return false;
            }

            userData = new()
            {
                userid = userid,
                username = username,
                role = role
            };

            return true;
        }
    }
}
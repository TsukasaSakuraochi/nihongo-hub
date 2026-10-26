namespace NihongoHub.Models.Dtos.Responses
{
    public class LoginResponse
    {
        public required string token {get;set;}
        public required int userid {get;set;}
        public required string usertype {get;set;}
        public required DateTime expiresat {get;set;}
    }
}
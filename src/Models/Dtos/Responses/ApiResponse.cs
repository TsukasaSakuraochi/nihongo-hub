///通用响应基类及公共响应结构

namespace NihongoHub.Models.Dtos.Responses
{
    public class apiresponse<TData>
    {
        public required bool success {get;set;}
        public required int code {get;set;} 
        public string? message {get;set;}
        public TData? data {get;set;}
    }
}
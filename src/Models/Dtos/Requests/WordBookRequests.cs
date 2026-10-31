///词汇本相关请求

namespace NihongoHub.Models.Dtos.Requests
{
    public class wordbookwordsrequestdata
    {
        public required int wordbookid {get;set;}
    }
    public class wordbookwordsrequest : apirequest<wordbookwordsrequestdata>;
}
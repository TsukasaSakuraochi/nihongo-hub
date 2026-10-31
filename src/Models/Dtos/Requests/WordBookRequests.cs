///词汇本相关请求

namespace NihongoHub.Models.Dtos.Requests
{
    public class TanngoCyouWordsRequestData
    {
        public required int tanngoCyouid {get;set;}
    }
    public class TanngoCyouWordsRequest : ApiRequest<TanngoCyouWordsRequestData>;
}
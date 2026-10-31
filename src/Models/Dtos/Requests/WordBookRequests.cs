///词汇本相关请求

namespace NihongoHub.Models.Dtos.Requests
{
    public class WordBookWordsRequestData
    {
        public required int wordbookid {get;set;}
    }
    public class WordBookWordsRequest : ApiRequest<WordBookWordsRequestData>;
}
///单词搜索、单词操作等请求

namespace NihongoHub.Models.Dtos.Requests
{
    public class SearchWordsRequestData
    {
        /// <summary>
        /// 这里的query既可以是kannji、kana，也可以是单词的意思等，需要后端自行判断
        /// </summary>
        public required string query {get;set;}
    }
    public class SearchWordsRequest : ApiRequest<SearchWordsRequestData>;
}
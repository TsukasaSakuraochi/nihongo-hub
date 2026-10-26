namespace NihongoHub.Models.Dtos.Requests
{
    public class SearchWordsRequest
    {
        /// <summary>
        /// 这里的query既可以是KannJi、Kana，也可以是单词的意思等，需要后端自行判断
        /// </summary>
        public required string query {get;set;}
    }
}
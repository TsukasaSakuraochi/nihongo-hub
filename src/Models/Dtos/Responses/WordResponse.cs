namespace NihongoHub.Models.Dtos.Responses
{
    /// <summary>
    /// 这里定义的Word可供其他返回api使用
    /// </summary>
    
    public class WordBookReferenceResponse
    {
        public required int id {get;set;}
        public required string name {get;set;}
        public required List<string> tags {get;set;}
    }
    public class WordResponse
    {
        public required int id {get;set;}
        public string? kannji {get;set;} //可空
        public required string kana {get;set;}
        public int? akusennto {get;set;}
        public required string meaning {get;set;}
        public string? note {get;set;}
        public List<WordBookReferenceResponse>? wordbooklist {get;set;} //表示包含该单词的单词本
        public required DateTime createdat {get;set;} //UTC
        public required DateTime updatedat {get;set;} //UTC
    }
    public class WordBookWordsResponse
    {
        public required List<WordResponse> words {get;set;}
    }
}
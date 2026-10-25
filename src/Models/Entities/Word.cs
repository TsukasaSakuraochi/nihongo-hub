using LiteDB;

namespace NihongoHub.Models.Entities
{
    /// <summary>
    /// 一个单词。嵌套在 TannGoCyou 中,不是独立的集合
    /// </summary>
    
    public class TannGo
    {
        public required Guid id {get;set;} //单词本内唯一,创建时由代码生成
        public string? kannji {get;set;} //汉字，可空
        public required string kana {get;set;} //假名
        public int? akusennto {get;set;} //音调(アクセント
        public required string meaning {get;set;} //词义
        public string? note {get;set;} //备注
        public required DateTime createdat {get;set;} //UTC
        public required DateTime updatedat {get;set;} //UTC
    }
    
    /// <summary>
    /// 单词本。userid 与 User 的 id 一致,表示属于哪个用户
    /// tags 属于单词本(如 "N2"),单词本身没有 tag
    /// </summary>

    public class TannGoCyou
    {
        [BsonId]
        public int id {set;get;}
        public required int userid {set;get;}
        public required string name {get;set;}
        public string? note {get;set;} //表示用户的标记
        public required List<TannGo> tanngolist {get;set;}
        public required List<string> tags {get;set;} //供前端使用的tag
        public required DateTime createdat {get;set;} //UTC
        public required DateTime updatedat {get;set;} //UTC
    }
}
using LiteDB;

namespace NihongoHub.Data
{
    public static class TannGoRepository
    {
        public static Models.Entities.TannGo? FindWordById(int id)
        {
            return Init.DatabaseInit.Current.TannGo.FindById(id);
        }
        public static List<Models.Entities.TannGo> FindWordsByIds(List<int> ids, List<int> errorIds)
        {
            errorIds.Clear();

            List<Models.Entities.TannGo> wordlist = new();

            foreach (int _id in ids)
            {
                var _word =  Init.DatabaseInit.Current.TannGo.FindById(_id);
                if (_word == null) 
                {
                    errorIds.Add(_id);
                    continue;
                }
                wordlist.Add(_word);
            }

            return wordlist;
        }

        public static bool Insert(Models.Entities.TannGo word,ref string hint,ref Logging.LogCategory errortype)
        {
            hint = "";

            try
            {
                Init.DatabaseInit.Current.TannGo.Insert(word);
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.DataBase,Logging.LogLevel.Error,"[ERROR]插入单词时数据库错误",ex);

                hint="插入单词时数据库错误";
                errortype = Logging.LogCategory.DataBase;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.DataBase,Logging.LogLevel.Error,"[ERROR]插入单词时发生未知错误",ex);

                hint="插入单词时发生未知错误";
                errortype = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
        public static bool Update(Models.Entities.TannGo word,ref string hint,ref Logging.LogCategory errortype)
        {
            hint = "";
            try
            {
                if (!Init.DatabaseInit.Current.TannGo.Update(word))
                {
                    hint = "未能更改单词数据(可能是单词不存在)";
                    errortype = Logging.LogCategory.DataBase;
                    return false;
                }
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.DataBase,Logging.LogLevel.Error,"[ERROR]修改单词时数据库错误",ex);

                hint="修改单词时数据库错误";
                errortype = Logging.LogCategory.DataBase;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.DataBase,Logging.LogLevel.Error,"[ERROR]修改单词时发生未知错误",ex);

                hint="修改单词时发生未知错误";
                errortype = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
    }
}
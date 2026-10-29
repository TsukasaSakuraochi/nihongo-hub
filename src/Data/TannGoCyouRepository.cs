using LiteDB;

namespace NihongoHub.Data
{
    public static class TannGoCyouRepository
    {
        public static Models.Entities.TannGoCyou? FindWordListById(int ids)
        {
            return Init.DatabaseInit.Current.TannGoCyou.FindById(ids);
        }
        public static List<Models.Entities.TannGoCyou> FindWordListsByUserId(int userId)
        {
            return Init.DatabaseInit.Current.TannGoCyou.Find(x => x.userid == userId).ToList();
        }

        public static bool Insert(Models.Entities.TannGoCyou wordList,ref string hint,ref Logging.LogCategory errortype)
        {
            hint = "";

            try
            {
                Init.DatabaseInit.Current.TannGoCyou.Insert(wordList);
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.DataBase,Logging.LogLevel.Error,"[ERROR]插入单词本时数据库错误",ex);

                hint="插入单词本时数据库错误";
                errortype = Logging.LogCategory.DataBase;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.DataBase,Logging.LogLevel.Error,"[ERROR]插入单词本时发生未知错误",ex);

                hint="插入单词本时发生未知错误";
                errortype = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
        public static bool Update(Models.Entities.TannGoCyou wordlist,ref string hint,ref Logging.LogCategory errortype)    
        {
            hint = "";

            try
            {
                if (!Init.DatabaseInit.Current.TannGoCyou.Update(wordlist))
                {
                    hint = "未能更改单词本数据(可能是单词本不存在)";
                    errortype = Logging.LogCategory.DataBase;
                    return false;
                }
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.DataBase,Logging.LogLevel.Error,"[ERROR]修改单词本时数据库错误",ex);

                hint="修改单词本时数据库错误";
                errortype = Logging.LogCategory.DataBase;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.DataBase,Logging.LogLevel.Error,"[ERROR]修改单词本时发生未知错误",ex);

                hint="修改单词本时发生未知错误";
                errortype = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
    }
}
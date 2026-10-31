using LiteDB;

namespace NihongoHub.Data
{
    public static class UserRepository
    {
        public static Models.Entities.User? FindUser(string username)
        {
            return Init.DatabaseInit.Current.Users.FindOne(x => x.username == username);
        }

        public static Models.Entities.User? FindUser(int userid)
        {
            return Init.DatabaseInit.Current.Users.FindById(userid);
        }

        ///这里加hint是为了能够返回一些在操作时暴露的问题
        /// hint可以为空

        public static bool Insert(Models.Entities.User user,ref string hint,ref Logging.LogCategory errortype)
        {
            //先清空hint
            hint = "";

            try
            {
                Init.DatabaseInit.Current.Users.Insert(user);
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.DataBase,Logging.LogLevel.Error,"[ERROR]插入用户时数据库错误",ex);

                hint="插入用户时数据库错误";
                errortype = Logging.LogCategory.DataBase;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.DataBase,Logging.LogLevel.Error,"[ERROR]插入用户时发生未知错误",ex);

                hint="插入用户时发生未知错误";
                errortype = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }

        public static bool Update(Models.Entities.User user,ref string hint,ref Logging.LogCategory errortype)
        {
            hint = "";

            try
            {
                if (!Init.DatabaseInit.Current.Users.Update(user))
                {
                    //严格来讲，这里不属于报错
                    hint = "未能更改用户数据(可能是用户不存在)";
                    errortype = Logging.LogCategory.DataBase;
                    return false;
                }
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.DataBase,Logging.LogLevel.Error,"[ERROR]修改用户时数据库错误",ex);

                hint="修改用户时数据库错误";
                errortype = Logging.LogCategory.DataBase;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.DataBase,Logging.LogLevel.Error,"[ERROR]修改用户时发生未知错误",ex);

                hint="修改用户时发生未知错误";
                errortype = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
    }
}
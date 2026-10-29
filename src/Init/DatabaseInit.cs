
namespace NihongoHub.Init
{
    public class DatabaseService : IDisposable
    {
        private readonly LiteDB.LiteDatabase _database;

        public DatabaseService(string databasePath)
        {
            _database = new LiteDB.LiteDatabase(databasePath);
        }

        public LiteDB.ILiteCollection<Models.Entities.User> Users
        {
            get
            {
                return _database.GetCollection<Models.Entities.User>("users");
            }
        }

        public LiteDB.ILiteCollection<Models.Entities.TannGoCyou> TannGoCyou
        {
            get
            {
                return _database.GetCollection<Models.Entities.TannGoCyou>("tanngocyou");
            }
        }

        public LiteDB.ILiteCollection<Models.Entities.TannGo> TannGo
        {
            get
            {
                return _database.GetCollection<Models.Entities.TannGo>("tanngo");
            }
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }

    public static class DatabaseInit
    {
        private static DatabaseService? _databaseService;

        public static DatabaseService Current
        {
            get
            {
                if(_databaseService == null){
                    Logging.LogReporter.Fatal("数据库未能初始化",Logging.LogCategory.DataBase);
                }
                return _databaseService;
            }
        }


        public static void Init()
        {
            _databaseService = new DatabaseService(Path.Combine(AppContext.BaseDirectory,"Config", ConfigInit.Current.Database.FileName));

            ///建立唯一索引
            /// EnsureIndex(?,true)的true表示唯一索引
            _databaseService.Users.EnsureIndex( x => x.username,true);
        }
    }
}

namespace NihongoHub.Init
{
    public class DatabaseContext : IDisposable
    {
        private readonly LiteDB.LiteDatabase _database;

        public DatabaseContext(string databasePath)
        {
            // Keep existing lowercase role values when CLR enum members use PascalCase.
            var mapper = new LiteDB.BsonMapper();
            mapper.RegisterType<Models.Entities.UserType>(
                role => new LiteDB.BsonValue(role.ToString().ToLowerInvariant()),
                value => value.IsInt32
                    ? (Models.Entities.UserType)value.AsInt32
                    : Enum.Parse<Models.Entities.UserType>(value.AsString, ignoreCase: true));
            _database = new LiteDB.LiteDatabase(databasePath, mapper);
        }

        public LiteDB.ILiteCollection<Models.Entities.User> Users
        {
            get
            {
                return _database.GetCollection<Models.Entities.User>("users");
            }
        }

        public LiteDB.ILiteCollection<Models.Entities.TanngoCyou> WordBooks
        {
            get
            {
                return _database.GetCollection<Models.Entities.TanngoCyou>("tanngocyou");
            }
        }

        public LiteDB.ILiteCollection<Models.Entities.Tanngo> Words
        {
            get
            {
                return _database.GetCollection<Models.Entities.Tanngo>("tanngo");
            }
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }

    public static class DatabaseInit
    {
        private static DatabaseContext? _databaseContext;

        public static DatabaseContext Current
        {
            get
            {
                if(_databaseContext == null){
                    Logging.LogReporter.Fatal("数据库未能初始化",Logging.LogCategory.Database);
                }
                return _databaseContext;
            }
        }


        public static void Init()
        {
            _databaseContext = new DatabaseContext(Path.Combine(AppContext.BaseDirectory,"Config", ConfigInit.Current.Database.FileName));

            ///建立唯一索引
            /// EnsureIndex(?,true)的true表示唯一索引
            _databaseContext.Users.EnsureIndex( x => x.username,true);
        }
    }
}

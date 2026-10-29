namespace NihongoHub
{
    public static class Program
    {
        public static void Main()
        {
            //这里执行初始化
            
            Init.Logginginit.Init(); //先进行Log初始化
            Init.ConfigInit.Load(); //config初始化
            Init.DatabaseInit.Init(); //数据库初始化
            Init.ServerInit.Setup(); //服务端初始化
        }
    }
}
using StackExchange.Redis;

namespace gm_redis
{
    public class LuaRedis
    {
        private ConnectionMultiplexer redis;
        public LuaRedis(string conn)
        {
            redis = ConnectionMultiplexer.Connect(conn);
            
        }

        public string GetStatus()
        {
            return redis?.GetStatus();
        }

        public LuaRedisDatabase GetDatabase()
        {
            return new LuaRedisDatabase(redis?.GetDatabase());
        }

        public void GetSubscriber()
        {
            //return redis?.GetSubscriber();
        }
    }


    public class LuaRedisDatabase
    {
        IDatabase Database;
        public LuaRedisDatabase(IDatabase db)
        {
            Database = db;
            //Database.
        }


    }
}

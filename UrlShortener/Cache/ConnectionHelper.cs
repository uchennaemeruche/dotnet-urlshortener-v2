using StackExchange.Redis;
using System;
namespace UrlShortener.Cache
{
	public class ConnectionHelper
	{
        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get => lazyConnection.Value;
        }

        static ConnectionHelper()
		{
            ConnectionHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() => {
                return ConnectionMultiplexer.Connect(ConfigurationManager.AppSetting["RedisURL"]);
            });
        }
	}
}


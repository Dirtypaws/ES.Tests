using System;
using Nest;

namespace ES.Tests.Helpers
{
    public static class ConnectionHelper
    {
        private static Uri _uri;
        private static Uri Uri { get { return _uri ?? (_uri = new Uri("http://localhost:9200"));} }

        private static ConnectionSettings _settings;
        private static ConnectionSettings Settings
        {
            get { return _settings ?? (_settings = new ConnectionSettings(Uri)); }
        }

        private static ElasticClient _client;
        public static ElasticClient Client
        {
            get { return _client ?? (_client = new ElasticClient(Settings)); }
        }
    }
}

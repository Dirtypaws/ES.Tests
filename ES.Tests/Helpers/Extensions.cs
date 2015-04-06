using Nest;

namespace ES.Tests.Helpers
{
    public static class Extensions
    {
        public static void SetUpTestIndex(this ElasticClient client, string index)
        {
            if (client.IndexExists(index).Exists)
                client.DeleteIndex(index);

            client.CreateIndex(index);
        }
    }
}
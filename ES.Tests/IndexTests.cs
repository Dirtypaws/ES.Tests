using System.Net;
using ES.Tests.Helpers;
using Nest;
using NUnit.Framework;

namespace ES.Tests
{
    [TestFixture]
    public class IndexTests
    {
        [Test]
        public void IndexExists()
        {
            var client = ConnectionHelper.Client;

            var ixEx = client.IndexExists("test");
            Assert.AreEqual((int)HttpStatusCode.OK, ixEx.ConnectionStatus.HttpStatusCode, "Received {0} response from server", ixEx.ConnectionStatus.HttpStatusCode);

            if (!ixEx.Exists)
            {
                var result = client.CreateIndex("test");
                Assert.AreEqual((int)HttpStatusCode.OK, result.ConnectionStatus.HttpStatusCode, "Received {0} response from server", result.ConnectionStatus.HttpStatusCode);
                Assert.IsTrue(result.Acknowledged, "Creation of new index was not acknowledged.");

                ixEx = client.IndexExists("test");
            }

            Assert.IsTrue(ixEx.Exists, "test index doesn't exist, and wasn't created.");

        }

        [Test]
        public void CreateIndex()
        {
            var client = ConnectionHelper.Client;

            if (client.IndexExists("test").Exists)
                client.DeleteIndex("test");

            var result = client.CreateIndex("test");
            Assert.AreEqual((int) HttpStatusCode.OK, result.ConnectionStatus.HttpStatusCode, "Received {0} response from server", result.ConnectionStatus.HttpStatusCode);
            Assert.IsTrue(result.Acknowledged, "Creation of new index was not acknowledged.");
        }

        [Test]
        public void Fail_DupIndex()
        {
            var client = ConnectionHelper.Client;

            IIndicesOperationResponse result;
            if (!client.IndexExists("test").Exists)
            {
                result = client.CreateIndex("test");

                Assert.AreEqual((int)HttpStatusCode.OK, result.ConnectionStatus.HttpStatusCode, "Received {0} response from server", result.ConnectionStatus.HttpStatusCode);
                Assert.IsTrue(result.Acknowledged, "Creation of initial index failed.");
            }

            result = client.CreateIndex("test");

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.ConnectionStatus.HttpStatusCode, "Received {0} response from server - Expected 400", result.ConnectionStatus.HttpStatusCode);
            Assert.IsFalse(result.Acknowledged, "Creation of duplicate index was acknowledged.");
        }

        [Test]
        public void DeleteIndex()
        {
            var client = ConnectionHelper.Client;

            if (!client.IndexExists("test").Exists)
            {
                var create = client.CreateIndex("test");

                Assert.AreEqual((int)HttpStatusCode.OK, create.ConnectionStatus.HttpStatusCode, "Received {0} response from server", create.ConnectionStatus.HttpStatusCode);
                Assert.IsTrue(create.Acknowledged, "Creation of initial index failed.");
            }

            var result = client.DeleteIndex("test");

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.ConnectionStatus.HttpStatusCode, "Received {0} response from server - Expected 400", result.ConnectionStatus.HttpStatusCode);
            Assert.IsFalse(result.Acknowledged, "Creation of duplicate index was acknowledged.");
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using ES.Tests.Helpers;
using NUnit.Framework;

namespace ES.Tests
{
    [TestFixture]
    public class ConnectionTests
    {

        [Test]
        public void CheckClusterHealth()
        {
            var client = ConnectionHelper.Client;

            var result = client.ClusterHealth();

            Assert.AreEqual((int)HttpStatusCode.OK, result.ConnectionStatus.HttpStatusCode, "Received {0} response from server", result.ConnectionStatus.HttpStatusCode);

            // We'll treat green as yellow, just to have a minimum yellow threshold
            Assert.AreEqual("yellow", result.Status.Replace("green", "yellow"), "Didn't receive Green status from server");
            Assert.IsTrue(result.ServerError == null, "Received the following errors from server:{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, result.ServerError));

            Debug.WriteLine("Cluster Name: {0}" + Environment.NewLine +
                            "Active Shards: {1}" + Environment.NewLine +
                            "Indices: {2}" + Environment.NewLine +
                            "Nodes: {3}" + Environment.NewLine +
                            "Unassigned Shards: {4}", 
                            result.ClusterName, result.ActiveShards, result.Indices, result.NumberOfNodes, result.UnassignedShards);
        }

        [Test]
        public void CheckNodeHealth()
        {
            var client = ConnectionHelper.Client;

            var result = client.NodesInfo();

            Assert.AreEqual((int)HttpStatusCode.OK, result.ConnectionStatus.HttpStatusCode, "Received {0} response from server", result.ConnectionStatus.HttpStatusCode);
            Debug.WriteLine("Cluster Name: " + result.ClusterName);
            Debug.WriteLine(Environment.NewLine);

            Assert.IsNotNull(result.Nodes, "No nodes were connected to cluster");
            Debug.WriteLine("# of nodes: " + result.Nodes.Count());
            Debug.WriteLine(Environment.NewLine);
            
            Debug.WriteLine("Nodes:");
            foreach (var node in result.Nodes)
                Debug.WriteLine("{0}: {1} ({2}-{3})", node.Key, node.Value.Name, node.Value.Hostname, node.Value.HttpAddress);
        }

        [Test]
        public void CheckIndices()
        {
            var client = ConnectionHelper.Client;

            var result = client.IndicesStats();

            Assert.AreEqual((int)HttpStatusCode.OK, result.ConnectionStatus.HttpStatusCode, "Received {0} response from server", result.ConnectionStatus.HttpStatusCode);
            Assert.IsNotNull(result.Indices, "No indices found on this cluster!");
            Assert.IsTrue(result.Indices.Any(), "An empty array of indices was returned by the server.");

            Debug.WriteLine("# of Indices: " + result.Indices.Count());
            Debug.WriteLine(Environment.NewLine);

            Debug.WriteLine("Indices: " + Environment.NewLine);
            Debug.WriteLine(string.Join(Environment.NewLine, result.Indices.Keys));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using ES.Tests.BusinessObjects;
using ES.Tests.Helpers;
using Foundation.ObjectHydrator;
using Nest;
using Nest.Resolvers.Converters;
using NUnit.Framework;

namespace ES.Tests
{
    public partial class DataTests
    {
        [Test]
        public void GetById()
        {
            /* Set Up */
            var client = ConnectionHelper.Client;
            client.SetUpTestIndex("test");

            var hydra = new Hydrator<IndexItem>();
            var itms = hydra.GetList(100);

            client.IndexMany(itms, "test", "employees");

            var rnd = new Random();
            var itm = itms[rnd.Next(0, 99)];

            /* Execute */
            var result = client.Get<IndexItem>(x => x.Index("test").Type("employees").Id(itm.Id.ToString()));
            var resp = result.Source;

            /* Assert */
            Assert.AreEqual((int)HttpStatusCode.OK, result.ConnectionStatus.HttpStatusCode, "Received {0} response from server", result.ConnectionStatus.HttpStatusCode);
            Assert.IsTrue(itm.Equals(resp), "Incorrect object returned for GUID: " + itm.Id);
            Debug.WriteLine("Local Item: ");
            Debug.WriteLine("Id: {0}" + Environment.NewLine +
                            "Seed Id: {1}" + Environment.NewLine +
                            "First Name: {2}" + Environment.NewLine +
                            "Last Name: {3}" + Environment.NewLine,
                            itm.Id, itm.SeedId, itm.FirstName, itm.LastName);

            Debug.WriteLine("Result Item: ");
            Debug.WriteLine("Id: {0}" + Environment.NewLine +
                            "Seed Id: {1}" + Environment.NewLine +
                            "First Name: {2}" + Environment.NewLine +
                            "Last Name: {3}" + Environment.NewLine,
                            resp.Id, resp.SeedId, resp.FirstName, resp.LastName);
        }
    }
}
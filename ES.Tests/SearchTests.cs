using System;
using System.Collections.Generic;
using System.Threading;
using ES.Tests.BusinessObjects;
using ES.Tests.Helpers;
using Foundation.ObjectHydrator;
using Nest;
using NUnit.Framework;

namespace ES.Tests
{
    public partial class DataTests
    {
        [Test]
        public void GetById()
        {
            var client = ConnectionHelper.Client;
            client.SetUpTestIndex("test");

            var hydra = new Hydrator<IndexItem>();
            var itms = hydra.GetList(100);

            for (int i = 0; i < itms.Count -1; i++)
                itms[i].SeedId = i + 1;

            client.IndexMany(itms, "test", "employees");

            Thread.Sleep(500);

            var rnd = new Random();
            var itm = itms[rnd.Next(0, 99)];

            var result = client.Get<IndexItem>(x => x.Index("test").Type("employee").Id(itm.SeedId).Fields(i => i.FirstName, i=> i.LastName));

            Assert.IsNotNull(result);
        }
    }
}
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
        public void InsertOne()
        {
            var client = ConnectionHelper.Client;
            client.SetUpTestIndex("test");

            var hydra = new Hydrator<IndexItem>();
            var itm = hydra.GetSingle();

            var result = client.Index(itm, x => x.Index("test").Type("employee"));

            Assert.AreEqual((int)HttpStatusCode.Created, result.ConnectionStatus.HttpStatusCode, "Received {0} response from server", result.ConnectionStatus.HttpStatusCode);
            Assert.IsTrue(result.Created, "Server didn't respond with created.");

            Debug.WriteLine("Id: {0}" + Environment.NewLine +
                            "Index: {1}" + Environment.NewLine +
                            "Type: {2}" + Environment.NewLine +
                            "Version: {3}" + Environment.NewLine,
                            result.Id, result.Index, result.Type, result.Version);
        }

        [Test]
        public void InsertOne_Duplicate()
        {
            var client = ConnectionHelper.Client;
            client.SetUpTestIndex("test");

            var hydra = new Hydrator<IndexItem>();
            var itm = hydra.GetSingle();
            var itm2 = itm;

            itm2.FirstName = "Duplicate Test";

            var result = client.Index(itm, x => x.Index("test").Type("employee"));
            var result2 = client.Index(itm2, x => x.Index("test").Type("employee"));

            Assert.AreEqual((int)HttpStatusCode.Created, result.ConnectionStatus.HttpStatusCode, "Received {0} response from server", result.ConnectionStatus.HttpStatusCode);
            Assert.AreEqual((int)HttpStatusCode.OK, result2.ConnectionStatus.HttpStatusCode, "Received {0} response from server", result.ConnectionStatus.HttpStatusCode);

            Assert.IsTrue(result.Created, "Server didn't respond with created.");
            Assert.AreEqual("2", result2.Version, "Version of updated record didn't match");

            Debug.WriteLine("First item:");
            Debug.WriteLine("Id: {0}" + Environment.NewLine +
                            "Index: {1}" + Environment.NewLine +
                            "Type: {2}" + Environment.NewLine +
                            "Version: {3}" + Environment.NewLine,
                result.Id, result.Index, result.Type, result.Version);

            Debug.WriteLine("Second item:");
            Debug.WriteLine("Id: {0}" + Environment.NewLine +
                            "Index: {1}" + Environment.NewLine +
                            "Type: {2}" + Environment.NewLine +
                            "Version: {3}" + Environment.NewLine,
                result2.Id, result2.Index, result2.Type, result2.Version);
        }

        [Test]
        public void InsertArray()
        {
            var client = ConnectionHelper.Client;
            client.SetUpTestIndex("test");

            var hydra = new Hydrator<IndexItem>();
            var itms = hydra.GetList(100);

            var result = client.IndexMany(itms, "test", "employee");
            Assert.IsFalse(result.Errors, "Response listed {0} errors {1}{2}",
                result.ItemsWithErrors.Count(),
                Environment.NewLine,
                string.Join(Environment.NewLine, result.ItemsWithErrors.Select(x => x.Id + ": " + x.Error)));

            Assert.IsTrue(result.Items.Any(), "No items returned");

            Debug.WriteLine("Created: {0}  ---  Updated {1}", result.Items.Count(x => x.Status == 201), result.Items.Count(x => x.Status == 200));
            var unk = result.Items.Where(x => x.Status != 200 && x.Status != 201);
            Debug.WriteLineIf(unk.Any(), string.Format("Unknown: {0}{1}{2}", unk.Count(), Environment.NewLine, string.Join(Environment.NewLine, unk.Select(x => x.Id + ": " + x.Status))));
        }

        [Test]
        public void InsertArray_Duplicates()
        {
            var client = ConnectionHelper.Client;
            client.SetUpTestIndex("test");

            var hydra = new Hydrator<IndexItem>();
            var itms = hydra.GetList(100).ToList();

            // Create 25 duplicate records
            var dups = itms.OrderBy(x => x.Id).Take(25);
            foreach (var dup in dups)
                dup.FirstName = "Duplicate - " + dup.FirstName;

            itms.AddRange(dups);

            var result = client.IndexMany(itms, "test", "employee");
            Assert.IsFalse(result.Errors, "Response listed {0} errors {1}{2}",
                result.ItemsWithErrors.Count(),
                Environment.NewLine,
                string.Join(Environment.NewLine, result.ItemsWithErrors.Select(x => x.Id + ": " + x.Error)));

            Assert.IsTrue(result.Items.Any(), "No items returned");

            Debug.WriteLine("Created: {0}  ---  Updated {1}", result.Items.Count(x => x.Status == 201), result.Items.Count(x => x.Status == 200));
            var unk = result.Items.Where(x => x.Status != 200 && x.Status != 201);
            Debug.WriteLineIf(unk.Any(), string.Format("Unknown: {0}{1}{2}", unk.Count(), Environment.NewLine, string.Join(Environment.NewLine, unk.Select(x => x.Id + ": " + x.Status))));
        }

        [Test, Category("Performance")]
        public void IndexMany()
        {
            var client = ConnectionHelper.Client;
            client.SetUpTestIndex("test");

            var hydra = new Hydrator<IndexItem>();
            var itms = hydra.GetList(100000);

            var sw = new Stopwatch();

            sw.Start();
            var result = client.IndexMany(itms, "test", "employee");
            sw.Stop();

            Debug.WriteLine("Indexed {0} records - {1} milliseconds elapsed", itms.Count(), sw.ElapsedMilliseconds);
            sw.Reset();

            Assert.IsFalse(result.Errors, "Response listed {0} errors {1}{2}",
                result.ItemsWithErrors.Count(),
                Environment.NewLine,
                string.Join(Environment.NewLine, result.ItemsWithErrors.Select(x => x.Id + ": " + x.Error)));

            Assert.IsTrue(result.Items.Any(), "No items returned");

            Debug.WriteLine("Created: {0}  ---  Updated {1}", result.Items.Count(x => x.Status == 201), result.Items.Count(x => x.Status == 200));
            var unk = result.Items.Where(x => x.Status != 200 && x.Status != 201);
            Debug.WriteLineIf(unk.Any(), string.Format("Unknown: {0}{1}{2}", unk.Count(), Environment.NewLine, string.Join(Environment.NewLine, unk.Select(x => x.Id + ": " + x.Status))));
        }
    }
}
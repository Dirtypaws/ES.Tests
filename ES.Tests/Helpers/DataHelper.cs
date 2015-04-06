using System.Collections.Generic;
using System.Linq;
using ES.Tests.BusinessObjects;
using Foundation.ObjectHydrator;

namespace ES.Tests.Helpers
{
    public static class DataHelper
    {
        private static IList<IndexItem> _items;
        private static IEnumerable<IndexItem> Items
        {
            get
            {
                if (_items == null)
                {
                    var hydra = new Hydrator<IndexItem>();
                    _items = hydra.GetList(100000).OrderBy(x => x.Id).ToList();
                }

                return _items;
            }
        }
    }
}
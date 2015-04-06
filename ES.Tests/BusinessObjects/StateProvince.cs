using System;
using System.Collections;
using System.Collections.Generic;

namespace ES.Tests.BusinessObjects
{
    public class StateProvince
    {
        public int StateProvinceId { get; set; }

        public string StateProvinceCode { get; set; }
        public string CountryRegionCode { get; set; }
        public bool IsOnlyStateProvinceFlag { get; set; }
        public string Name { get; set; }
        public int TerritoryId { get; set; }
        public Guid RowGuid { get; set; }
        public string ModifiedDate { get; set; }

        public IList<Address> Address { get; set; }
    }

    public class Address
    {
        public int AddressId { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        public string City { get; set; }
        public int StateProvinceId { get; set; }

        public string PostalCode { get; set; }

        public Guid RowGuid { get; set; }
        public string ModifiedDate { get; set; }
    }
}
using System;

namespace ES.Tests.BusinessObjects
{
    public class IndexItem
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Use this id if you want to track an ID as if it were from a DB with Primary Key.
        /// </summary>
        public int SeedId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool Equals(IndexItem obj)
        {
            return (Id == obj.Id) &&
                   (SeedId == obj.SeedId) &&
                   (FirstName == obj.FirstName) &&
                   (LastName == obj.LastName);
        }
    }
}
using System;

namespace linq2db_sample.EFCore
{
    public class DbContract
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

    }
}
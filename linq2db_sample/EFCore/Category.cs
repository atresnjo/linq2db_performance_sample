using System;
using System.Collections.Generic;

namespace linq2db_sample.EFCore
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<DbContract> Contracts { get; set; }

    }
}
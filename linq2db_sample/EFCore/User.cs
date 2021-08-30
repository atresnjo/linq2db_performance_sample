using System;
using System.Collections.Generic;

namespace linq2db_sample.EFCore
{
    public class User
    {
        public Guid Id { get; set; }
        public string CognitoId { get; set; }
        public List<DbContract> Contracts { get; set; }
      
    }
}
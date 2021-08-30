using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoBogus;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDB.Mapping;

namespace linq2db_sample.Linq2Db
{
    public class Linq2DbService
    {
        private readonly AppDataConnection _connection;

        public Linq2DbService(AppDataConnection connection)
        {
            _connection = connection;
        }

        public async Task Create()
        {
            var companies = new AutoFaker<Company>().RuleFor(x => x.Id, f => Guid.NewGuid()).Generate(5);
            var categories = new AutoFaker<Category>().RuleFor(x => x.Id, f => Guid.NewGuid()).Generate(5);
            var user = new User
            {
                Id = Guid.NewGuid(),
                CognitoId = "123"
            };

            var cbContractFaker = new AutoFaker<Contract>()
                .RuleFor(x => x.Id, f => Guid.NewGuid())
                .RuleFor(x => x.UsersId, user.Id)
                .RuleFor(x => x.CategoryId, f => f.PickRandom(categories).Id)
                .RuleFor(x => x.CompanyId, f => f.PickRandom(companies).Id);

            var contracts = cbContractFaker.Generate(10);

            await _connection.BeginTransactionAsync();
            await _connection.InsertAsync(user);
            await _connection.BulkCopyAsync(new BulkCopyOptions() {UseParameters = true}, companies);
            await _connection.BulkCopyAsync(new BulkCopyOptions() {UseParameters = true}, categories);
            await _connection.BulkCopyAsync(new BulkCopyOptions() {UseParameters = true}, contracts);
            await _connection.CommitTransactionAsync();
        }
    }
    
    public class AppDataConnection : DataConnection
    {
        public AppDataConnection()
        {
        }

        public AppDataConnection(string configuration)
            : base(configuration)
        {
        }

        public AppDataConnection(LinqToDbConnectionOptions options)
            : base(options)
        {
        }

        public AppDataConnection(LinqToDbConnectionOptions<AppDataConnection> options)
            : base(options)
        {
        }

        public ITable<Category> Categories => GetTable<Category>();
        public ITable<Company> Companies => GetTable<Company>();
        public ITable<Contract> Contracts => GetTable<Contract>();
        public ITable<User> Users => GetTable<User>();
    }

    [Table(Schema = "public", Name = "category")]
    public class Category
    {
        [Column("id")] [PrimaryKey] [NotNull] public Guid Id { get; set; } // uuid
        [Column("name")] [NotNull] public string Name { get; set; } // text

        #region Associations

        /// <summary>
        ///     FK_contract_category_category_id_BackReference
        /// </summary>
        [Association(ThisKey = "Id", OtherKey = "CategoryId", CanBeNull = true, Relationship = Relationship.OneToMany,
            IsBackReference = true)]
        public IEnumerable<Contract> Contractids { get; set; }

        #endregion
    }

    [Table(Schema = "public", Name = "company")]
    public class Company
    {
        [Column("id")] [PrimaryKey] [NotNull] public Guid Id { get; set; } // uuid
        [Column("name")] [NotNull] public string Name { get; set; } // text

        #region Associations

        /// <summary>
        ///     FK_contract_company_company_id_BackReference
        /// </summary>
        [Association(ThisKey = "Id", OtherKey = "CompanyId", CanBeNull = true, Relationship = Relationship.OneToMany,
            IsBackReference = true)]
        public IEnumerable<Contract> Contractids { get; set; }

        #endregion
    }

    [Table(Schema = "public", Name = "contract")]
    public class Contract
    {
        [Column("id")] [PrimaryKey] [NotNull] public Guid Id { get; set; } // uuid
        [Column("users_id")] [NotNull] public Guid UsersId { get; set; } // uuid
        [Column("category_id")] [NotNull] public Guid CategoryId { get; set; } // uuid
        [Column("company_id")] [NotNull] public Guid CompanyId { get; set; } // uuid

        #region Associations

        /// <summary>
        ///     FK_contract_category_category_id
        /// </summary>
        [Association(ThisKey = "CategoryId", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne,
            KeyName = "FK_contract_category_category_id", BackReferenceName = "Contractids")]
        public Category Category { get; set; }

        /// <summary>
        ///     FK_contract_company_company_id
        /// </summary>
        [Association(ThisKey = "CompanyId", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne,
            KeyName = "FK_contract_company_company_id", BackReferenceName = "Contractids")]
        public Company Company { get; set; }

        /// <summary>
        ///     FK_contract_users_users_id
        /// </summary>
        [Association(ThisKey = "UsersId", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne,
            KeyName = "FK_contract_users_users_id", BackReferenceName = "Contractids")]
        public User User { get; set; }

        #endregion
    }

    [Table(Schema = "public", Name = "users")]
    public class User
    {
        [Column("id")] [PrimaryKey] [NotNull] public Guid Id { get; set; } // uuid
        [Column("cognito_id")] [NotNull] public string CognitoId { get; set; } // text
        #region Associations

        /// <summary>
        ///     FK_contract_users_users_id_BackReference
        /// </summary>
        [Association(ThisKey = "Id", OtherKey = "UsersId", CanBeNull = true, Relationship = Relationship.OneToMany,
            IsBackReference = true)]
        public IEnumerable<Contract> Contractids { get; set; }

        #endregion
    }
}
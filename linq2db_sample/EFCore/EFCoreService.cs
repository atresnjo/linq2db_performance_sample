using System;
using System.Threading.Tasks;
using AutoBogus;

namespace linq2db_sample.EFCore
{
    public class EFCoreService
    {
        private readonly DatabaseContext _databaseContext;

        public EFCoreService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task Create(int count)
        {
            var companies = new AutoFaker<Company>().Generate(count);
            var categories = new AutoFaker<Category>().Ignore(x => x.Contracts).Generate(count);
            var cbContractFaker = new AutoFaker<DbContract>()
                .Ignore(x => x.User)
                .Ignore(x => x.CompanyId)
                .RuleFor(x => x.Category, f => f.PickRandom(categories))
                .RuleFor(x => x.Company, f => f.PickRandom(companies));

            var contracts = cbContractFaker.Generate(count);

            var user = new User { Contracts = contracts, CognitoId = "123" };
            await _databaseContext.Users.AddAsync(user);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
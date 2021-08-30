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

        public async Task Create()
        {
            var companies = new AutoFaker<Company>().RuleFor(x => x.Id, f => Guid.NewGuid()).Generate(5);
            var categories = new AutoFaker<Category>().RuleFor(x => x.Id, f => Guid.NewGuid()).Generate(5);
            var cbContractFaker = new AutoFaker<DbContract>()
                .RuleFor(x => x.Id, f => Guid.NewGuid())
                .RuleFor(x => x.Category, f => f.PickRandom(categories))
                .RuleFor(x => x.Company, f => f.PickRandom(companies));

            var contracts = cbContractFaker.Generate(10);

            var user = new User { Contracts = contracts, CognitoId = "123" };
            await _databaseContext.Users.AddAsync(user);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
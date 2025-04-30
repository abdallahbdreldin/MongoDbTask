using TodayWebAPi.DAL.Data.Context;
using TodayWebAPi.DAL.Repos.Generic;
using TodayWebAPi.DAL.Repos.Products;

namespace TodayWebAPi.DAL.Repos.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _context;
        public IProductRepo ProductRepo { get; }

        private Dictionary<string, object> _repositories;

        public UnitOfWork(StoreContext context , IProductRepo productRepo)
        {
            _context = context;
            ProductRepo = productRepo;
            _repositories = new Dictionary<string, object>();
        }


        public IGenericRepo<TEntity> Repo<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new GenericRepo<TEntity>(_context);
                _repositories[type] = repositoryInstance;
            }

            return (IGenericRepo<TEntity>)_repositories[type];
        }
    }
}

using TodayWebAPi.DAL.Repos.Generic;
using TodayWebAPi.DAL.Repos.Products;

namespace TodayWebAPi.DAL.Repos.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IProductRepo ProductRepo { get; }
        IGenericRepo<TEntity> Repo<TEntity>() where TEntity : class;
    }
}

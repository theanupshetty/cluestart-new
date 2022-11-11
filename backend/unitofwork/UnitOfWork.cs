using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dal;
using entities;

namespace unitofwork
{
    public class UnitOfWork : IUnitOfWork
    {
       private readonly DataContext _context;
        private Dictionary<string, object> repositories;
        public UnitOfWork(DataContext context)
        {
            _context = context;
        }
        public int Complete()
        {
            return _context.SaveChanges();
        }
        public DataContext GetContext
        {
            get
            {
                return _context;
            }

        }
        
        public void Dispose()
        {
            _context.Dispose();
        }

        public Repository<T> Repository<T>() where T : class
        {
            if (repositories == null)
            {
                repositories = new Dictionary<string, object>();
            }

            var type = typeof(T).Name;

            if (!repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
                repositories.Add(type, repositoryInstance);
            }
            return (Repository<T>)repositories[type];
        }
    }
}
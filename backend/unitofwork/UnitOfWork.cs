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
        private Repository<Users> _users;
        public UnitOfWork(DataContext context)
        {
            _context = context;

        }
        public IRepository<Users> Users
        {
            get
            {
                return _users ??
                    (_users = new Repository<Users>(_context));
            }
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
    }
}
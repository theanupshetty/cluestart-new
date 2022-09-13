using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dal;
using entities;

namespace unitofwork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Users> Users { get; }
        int Complete();
        DataContext GetContext { get; }
    }
}
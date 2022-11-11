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
       int Complete();
        DataContext GetContext { get; }
        Repository<T> Repository<T>() where T : class;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bal.services
{
    public interface IUserService
    {
        Task<object> GetUsers();
    }
}
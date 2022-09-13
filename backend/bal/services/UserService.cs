using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using unitofwork;

namespace bal.services
{
    public class UserService
    {
       private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<object> GetUsers()
        {
            try
            {
                var result = await Task.FromResult(_unitOfWork.Users.GetAll());
                return result;

            }
            catch (Exception ex)
            {
                return ex;
            }


        }

        // public async Task<object> GetDropOutStats(DateTime? fromdate, DateTime? todate,object device)
        // {
        //     try
        //     {
        //         var procParams = new Dictionary<string, object>()
        //     {
        //         {"@FromDate", fromdate},
        //         {"@ToDate", todate},
        //         {"@Device", device}
        //     };
        //         return await _unitOfWork.Stats.ExecuteStoredProc("sp_Get_DropOuts", procParams);
             
        //     }
        //     catch (Exception ex)
        //     {
        //         return ex;
        //     }
        // } 
    }
}
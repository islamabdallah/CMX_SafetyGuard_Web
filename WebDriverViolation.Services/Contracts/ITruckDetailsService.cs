using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Services.Models.APIModels;

namespace WebDriverViolation.Services.Contracts
{
    public interface ITruckDetailsService
    {
        Task<bool> CreateTruckDetails(TruckDetailsApiModel truckDetails);
    }
}

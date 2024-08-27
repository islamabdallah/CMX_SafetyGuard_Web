using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Contracts
{
    public interface IViolationTypeAccuracyLavelService
    {
        Task<ViolationTypeAccuracyLavel> GetViolationAccuracyLavelById(long Id);
        Task<List<ViolationTypeAccuracyLavel>> GetAllViolationTypeAccuracyLavels();

        Task<List<ViolationTypeAccuracyLavel>> GetViolationAccuracyLavelsByType(int type);
     
        Task<List<ViolationTypeAccuracyLavel>> GetViolationAccuracyLavel(long levelId);

        Task<List<ViolationTypeAccuracyLavel>> GetViolationAccuracyLavelsByType(int type, DateTime ViolationDate, int mode);

        ViolationTypeAccuracyLavel GetViolationAccuracyLavelForViolation(int type, double AverageProbability, DateTime ViolationDate, int mode);
    }
}

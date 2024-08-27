using AutoMapper;
using Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Implementation
{
    public class ConfirmationStatusService : IConfirmationStatusService
    {
        private readonly IRepository<ConfirmationStatus, int> _repository;
        private readonly IMapper _mapper;
        public ConfirmationStatusService(IRepository<ConfirmationStatus, int> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public List<ConfirmationStatusModel> GetAllConfirmationStatuses()
        {
            try
            {

               var confirmationStatuses = _repository.Find(s=>s.IsVisible == true);
                if(confirmationStatuses != null)
                {
                    List<ConfirmationStatusModel> confirmationStatusModels   = _mapper.Map<List<ConfirmationStatusModel>>(confirmationStatuses);
                    return confirmationStatusModels;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}

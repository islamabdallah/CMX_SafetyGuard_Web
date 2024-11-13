using Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text.Json;
using Take5.Services.Contracts;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Controllers
{
    public class TruckTrackingController : Controller
    {
        private readonly ITruckRunningTrackingService _truckRunningTrackingService;
        private readonly IRepository<CameraViolation, long> _cvrepository;
        private readonly IRepository<Truck, long> _truckRepository;
        public TruckTrackingController(ITruckRunningTrackingService truckRunningTrackingService,
            IRepository<CameraViolation, long> cvrepository, IRepository<Truck, long> truckRepository) 
        {
            _truckRunningTrackingService = truckRunningTrackingService;
            _cvrepository = cvrepository;
            _truckRepository = truckRepository;
        }
        // GET: TruckTrackingController
        public ActionResult SearchTruckTracking()
        {
            SearchTruckTrackingModel searchTruckTrackingModel = new SearchTruckTrackingModel();
            searchTruckTrackingModel =_truckRunningTrackingService.InitiateTruckTrackingSearchModel(searchTruckTrackingModel);

            return View("TruckTracking", searchTruckTrackingModel);
        }

        [HttpPost]
        public ActionResult SearchTruckTracking(SearchTruckTrackingModel searchTruckTrackingModel)
        {
            try
            {
                if (searchTruckTrackingModel != null)
                {
                    List<TruckRunningTrackingAPIModel> truckRunningTrackingAPIModels = _truckRunningTrackingService.SearchTruckRunningTracking(searchTruckTrackingModel);
                    if (truckRunningTrackingAPIModels != null)
                    {
                        searchTruckTrackingModel.TruckRunningTrackingAPIModels = truckRunningTrackingAPIModels;
                    }
                    searchTruckTrackingModel = _truckRunningTrackingService.InitiateTruckTrackingSearchModel(searchTruckTrackingModel);
                }
                else
                {
                    return RedirectToAction("ERROR404");
                }
                return View("TruckTracking", searchTruckTrackingModel);
            }
            catch(Exception ex)
            {
                return RedirectToAction("ERROR404");
            }

        }

        public ActionResult CameraTracking()
        {
            CameraTrackingModel model = this._truckRunningTrackingService.InitiateCameraSearchModel(new CameraTrackingModel());
            model.Trucks = model.Trucks.Where<TruckModel>((Func<TruckModel, bool>)(t => t.Category == "camera")).ToList<TruckModel>();
            return (ActionResult)this.View(nameof(CameraTracking), (object)model);
        }

        [HttpPost]
        public ActionResult CameraTracking(CameraTrackingModel searchTruckTrackingModel)
        {
            try
            {
                CameraTrackingModel cameraTrackingModel = new CameraTrackingModel();
                if (searchTruckTrackingModel == null)
                    return (ActionResult)this.RedirectToAction("ERROR404");
                List<CameraViolation> list = this._cvrepository.Find((Expression<Func<CameraViolation, bool>>)(t => t.TruckID == searchTruckTrackingModel.SelectedTruckID)).ToList<CameraViolation>();
                if (list.Count > 0)
                {
                    foreach (CameraViolation cameraViolation in list)
                        this._cvrepository.Delete(cameraViolation);
                }
                if (searchTruckTrackingModel.SelectedTypes.Count > 0)
                {
                    foreach (int selectedType in searchTruckTrackingModel.SelectedTypes)
                    {
                        CameraViolation cameraViolation = new CameraViolation();
                        cameraViolation.TruckID = searchTruckTrackingModel.SelectedTruckID;
                        cameraViolation.ViolationTypeID = selectedType;
                        cameraViolation.IsDelted = false;
                        cameraViolation.IsVisible = true;
                        cameraViolation.CreatedDate = DateTime.Now;
                        cameraViolation.UpdatedDate = DateTime.Now;
                        this._cvrepository.Add(cameraViolation);
                    }
                }
                CameraTrackingModel model = _truckRunningTrackingService.InitiateCameraSearchModel(searchTruckTrackingModel);
                model.Trucks = model.Trucks.Where<TruckModel>((Func<TruckModel, bool>)(t => t.Category == "camera")).ToList<TruckModel>();
                model.SelectedTruckID = searchTruckTrackingModel.SelectedTruckID;
                model.SelectedTypes = searchTruckTrackingModel.SelectedTypes;
                return (ActionResult)this.View(nameof(CameraTracking), (object)model);
            }
            catch (Exception ex)
            {
                return (ActionResult)this.RedirectToAction("ERROR404");
            }
        }

        [HttpPost]
        public async Task<string> GetCameraViolations(string id)
        {
            return JsonSerializer.Serialize<List<CameraViolation>>(this._cvrepository.Find((Expression<Func<CameraViolation, bool>>)(i => i.TruckID == id)).ToList<CameraViolation>());
        }

        [HttpPost]
        public async Task<string> AddCamera(string id, string ip)
        {
            var truck=_truckRepository.Find(t=>t.Id == id).FirstOrDefault();
            if (truck == null)
            {
                Truck truck1 = new Truck();
                truck1.Id = id;
                truck1.IsDelted = false;
                truck1.IsVisible = true;
                truck1.CreatedDate = DateTime.Now;
                truck1.UpdatedDate = DateTime.Now;
                truck1.MailList = "";
                truck1.SendMail = true;
                truck1.Company = "RMX";
                truck1.TruckName = id;
                truck1.Category = "camera";
                truck1.CameraIp = ip;
                var result = _truckRepository.Add(truck);
                if(result != null)
                {
                    return JsonSerializer.Serialize(result.Id);
                }
                else
                {
                    return null;
                }
            }
            // return JsonSerializer.Serialize<List<CameraViolation>>(this._cvrepository.Find((Expression<Func<CameraViolation, bool>>)(i => i.TruckID == id)).ToList<CameraViolation>());
            return null;
        }
        // GET: TruckTrackingController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TruckTrackingController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TruckTrackingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TruckTrackingController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TruckTrackingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TruckTrackingController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TruckTrackingController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

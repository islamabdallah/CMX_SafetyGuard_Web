using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Take5.Services.Contracts;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Controllers
{
    public class TruckTrackingController : Controller
    {
        private readonly ITruckRunningTrackingService _truckRunningTrackingService;
        public TruckTrackingController(ITruckRunningTrackingService truckRunningTrackingService) 
        {
            _truckRunningTrackingService = truckRunningTrackingService;
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

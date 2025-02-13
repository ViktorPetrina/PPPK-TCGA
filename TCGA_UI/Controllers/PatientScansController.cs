using DataLayer.Models;
using DataLayer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using TCGA_UI.Models;
using TCGA_UI.ViewModels;

namespace TCGA_UI.Controllers
{
    public class PatientScansController : Controller
    {
        private readonly ISimpleRepository<PatientScan> _repo;

        public PatientScansController(ISimpleRepository<PatientScan> repo)
        {
            _repo = repo;
        }

        public async Task<ActionResult> Index()
        {
            var scans = await _repo.ReadAll();
            var scanVms = scans.ToList().Select(ps => new PatientScanVM
            {
                Id = ps.Id,
                CancerCohort = CohortType.Instances.FirstOrDefault(c => c.Code == ps.CancerCohort)?.Name,
                GeneExpresions = ps.GeneExpresions
            });

            return View(scanVms);
        }

        public async Task<ActionResult> Details(string id)
        {
            var scans = await _repo.ReadAll();
            var geneExpressions = scans.ToList().FirstOrDefault(p => p.Id == id)?.GeneExpresions;

            return View(geneExpressions);
        }
    }
}

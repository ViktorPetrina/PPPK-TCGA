using DataLayer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using TCGA_UI.Models;

namespace TCGA_UI.Controllers
{
    public class PatientScansController : Controller
    {
        private readonly ISimpleRepository<PatientScan> _repo;

        public PatientScansController(ISimpleRepository<PatientScan> repo)
        {
            _repo = repo;
        }

        public async Task<ActionResult> IndexAsync()
        {
            var scans = await _repo.ReadAll();

            return View(scans);
        }

        public async Task<ActionResult> Details(string id)
        {
            var scans = await _repo.ReadAll();
            var geneExpressions = scans.ToList().FirstOrDefault(p => p.Id == id)?.GeneExpresions;

            return View(geneExpressions);
        }
    }
}

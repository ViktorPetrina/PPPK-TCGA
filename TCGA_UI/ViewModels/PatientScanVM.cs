using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using TCGA_UI.Models;

namespace TCGA_UI.ViewModels
{
    public class PatientScanVM
    {
        [Display(Name = "Anonymous patient id")]
        public string Id { get; set; }

        [Display(Name = "Cancer cohort type")]
        public string? CancerCohort { get; set; }

        public GeneExpresion? GeneExpresions { get; set; }
    }
}

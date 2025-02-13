using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TCGA_UI.Models
{
    public class PatientScan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("patient_id")]
        [Display(Name = "Anonymous patient id")]
        public string Id { get; set; }

        [BsonElement("cancer_cohort")]
        [Display(Name = "Cancer cohort type")]
        public string? CancerCohort { get; set; }

        public GeneExpresion? GeneExpresions { get; set; }
    }
}

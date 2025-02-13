using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TCGA_UI.Models
{
    public class PatientScan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("patient_id")]
        public string Id { get; set; }

        [BsonElement("cancer_cohort")]
        public string? CancerCohort { get; set; }

        public GeneExpresion? GeneExpresions { get; set; }
    }
}

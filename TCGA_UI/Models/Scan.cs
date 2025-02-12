namespace TCGA_UI.Models
{
    public class PatientScan
    {
        public long IdScan { get; set; }
        public string? CancerCohort { get; set; }
        public GeneExpresion? GeneExpresions { get; set; }
    }
}

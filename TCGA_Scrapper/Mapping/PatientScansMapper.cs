using System.Text;
using TCGA_UI.Models;

namespace TCGA_UI.Mapping
{
    public static class PatientScansMapper
    {
        public static IEnumerable<PatientScan> Map(IEnumerable<KeyValuePair<string, List<string>>> scans)
        {
            var patientScans = new List<PatientScan>();

            foreach (var scan in scans)
            {
                var cohort = scan.Key;
                var lines = scan.Value;

                foreach (var line in lines)
                {
                    var columns = line.Split('\t').Skip(1).ToArray();
                    var geneExpresion = new GeneExpresion
                    {
                        C6orf150 = double.Parse(columns[0]),
                        CCL5 = double.Parse(columns[1]),
                        CXCL10 = double.Parse(columns[2]),
                        TMEM173 = double.Parse(columns[3]),
                        CXCL9 = double.Parse(columns[4]),
                        CXCL11 = double.Parse(columns[5]),
                        NFKB1 = double.Parse(columns[6]),
                        IKBKE = double.Parse(columns[7]),
                        IRF3 = double.Parse(columns[8]),
                        TREX1 = double.Parse(columns[9]),
                        ATM = double.Parse(columns[10]),
                        IL6 = double.Parse(columns[11]),
                        IL8 = double.Parse(columns[12])
                    };

                    var patientScan = new PatientScan
                    {
                        CancerCohort = cohort,
                        GeneExpresions = geneExpresion
                    };

                    patientScans.Add(patientScan);
                }
            }

            return patientScans;
        }
    }
}

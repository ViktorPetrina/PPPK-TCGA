namespace DataLayer.Models
{
    public class CohortType
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public CohortType(string name, string code)
        {
            Name = name;
            Code = code;
        }

        public static List<CohortType> Instances = new List<CohortType>
        {
        new CohortType("TCGA Acute Myeloid Leukemia", "LAML"),
        new CohortType("TCGA Adrenocortical Cancer", "ACC"),
        new CohortType("TCGA Bile Duct Cancer", "CHOL"),
        new CohortType("TCGA Bladder Cancer", "BLCA"),
        new CohortType("TCGA Breast Cancer", "BRCA"),
        new CohortType("TCGA Cervical Cancer", "CESC"),
        new CohortType("TCGA Colon and Rectal Cancer", "COADREAD"),
        new CohortType("TCGA Colon Cancer", "COAD"),
        new CohortType("TCGA Endometrioid Cancer", "UCEC"),
        new CohortType("TCGA Esophageal Cancer", "ESCA"),
        new CohortType("TCGA Formalin Fixed Paraffin-Embedded Pilot Phase II", "FPPP"),
        new CohortType("TCGA Glioblastoma", "GBM"),
        new CohortType("TCGA Head and Neck Cancer", "HNSC"),
        new CohortType("TCGA Kidney Chromophobe", "KICH"),
        new CohortType("TCGA Kidney Clear Cell Carcinoma", "KIRC"),
        new CohortType("TCGA Kidney Papillary Cell Carcinoma", "KIRP"),
        new CohortType("TCGA Large B-cell Lymphoma", "DLBC"),
        new CohortType("TCGA Liver Cancer", "LIHC"),
        new CohortType("TCGA Lower Grade Glioma", "LGG"),
        new CohortType("TCGA Lower Grade Glioma and Glioblastoma", "GBMLGG"),
        new CohortType("TCGA Lung Adenocarcinoma", "LUAD"),
        new CohortType("TCGA Lung Cancer", "LUNG"),
        new CohortType("TCGA Lung Squamous Cell Carcinoma", "LUSC"),
        new CohortType("TCGA Melanoma", "SKCM"),
        new CohortType("TCGA Mesothelioma", "MESO"),
        new CohortType("TCGA Ocular melanomas", "UVM"),
        new CohortType("TCGA Ovarian Cancer", "OV"),
        new CohortType("TCGA Pan-Cancer", "PANCAN"),
        new CohortType("TCGA Pancreatic Cancer", "PAAD"),
        new CohortType("TCGA Pheochromocytoma & Paraganglioma", "PCPG"),
        new CohortType("TCGA Prostate Cancer", "PRAD"),
        new CohortType("TCGA Rectal Cancer", "READ"),
        new CohortType("TCGA Sarcoma", "SARC"),
        new CohortType("TCGA Stomach Cancer", "STAD"),
        new CohortType("TCGA Testicular Cancer", "TGCT"),
        new CohortType("TCGA Thymoma", "THYM"),
        new CohortType("TCGA Thyroid Cancer", "THCA"),
        new CohortType("TCGA Uterine Carcinosarcoma", "UCS")
        };
    }
}

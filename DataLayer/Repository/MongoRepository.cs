using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCGA_UI.Models;

namespace DataLayer.Repository
{
    public class MongoRepository : ISimpleRepository<PatientScan>
    {
        // makni u konfiguracijski file
        private const string CONNECTION_STRING = 
            @"mongodb+srv://vpetrina:Pa$$w0rd@cluster0.l3cpyxz.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";

        private const string DATABASE_NAME = "PPPK_Project";
        private const string COLLECTION_NAME = "PatientScans";

        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<PatientScan> _patientScans;

        public MongoRepository()
        {
            _client = new MongoClient(CONNECTION_STRING);
            _database = _client.GetDatabase(DATABASE_NAME);
            _patientScans = _database.GetCollection<PatientScan>(COLLECTION_NAME);
        }

        public async Task Insert(PatientScan entity)
        {
            await _patientScans.InsertOneAsync(entity);
        }

        public async Task<IEnumerable<PatientScan>> ReadAll()
        {
            return await _patientScans.Find(_ => true).ToListAsync();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repository
{
    public interface ISimpleFileRepository
    {
        Task Create(string objectName, string filePath, string contentType);
        Task<IEnumerable<string>> ReadAll();
    }
}

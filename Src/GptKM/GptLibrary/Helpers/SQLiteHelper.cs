using Domains.Models;
using EntityModel.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Helpers
{
    public class SQLiteHelper
    {
        public BackendDBContext GetBackendDBContext()
        {
            string connectionString = @"Filename=MyDatabase.db";
            DbContextOptions<BackendDBContext> options = new DbContextOptionsBuilder<BackendDBContext>()
                .UseSqlite(connectionString)
                .Options;
            BackendDBContext BackendDBContext = new BackendDBContext(options);
            return BackendDBContext;
        }
    }
}

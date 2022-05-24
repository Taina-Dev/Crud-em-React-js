using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SeriesFilmesAPI.Models;

namespace SeriesFilmesAPI.Data
{
    public class SeriesFilmesAPIContext : DbContext
    {
        public SeriesFilmesAPIContext (DbContextOptions<SeriesFilmesAPIContext> options)
            : base(options)
        {
        }

        public DbSet<SeriesFilmesAPI.Models.Cadastro> Cadastro { get; set; }
    }
}

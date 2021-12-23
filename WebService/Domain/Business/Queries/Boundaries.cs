using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Domain.Business.Queries
{
    public class Boundaries : IQuery
    {
        string Catalog { get; set; }
        DateTime StartDate{ get; set; }
        DateTime EndDate { get; set; }
    }
}

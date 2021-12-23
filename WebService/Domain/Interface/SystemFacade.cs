using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Authentication;
using WebService.Domain.Business.Queries;

namespace WebService.Domain.Interface
{
    public class SystemFacade
    {
        public QueriesController QueriesController { get; }

        public SystemFacade(QueriesController queriesController)
        {
            QueriesController = queriesController;
        }
    }
}

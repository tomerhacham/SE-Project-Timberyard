using System.Threading.Tasks;
using WebService.Domain.DataAccess;
using WebService.Utils;

namespace WebService.Domain.Business.Queries
{
    public interface IQuery
    {
        public Task<Result<QueryResult>> Execute(LogsAndTestsRepository LogsAndTestsRepository);
    }
}

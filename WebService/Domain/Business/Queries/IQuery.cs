using System.Threading.Tasks;
using WebService.Domain.DataAccess;
using WebService.Utils;

namespace WebService.Domain.Business.Queries
{
    public interface IQuery
    {
        /// <summary>
        /// Each query need to implement this interface and the 'Execute' function.
        /// The function accepts the relevent repository which responsible to the database communication
        /// </summary>
        /// <param name="LogsAndTestsRepository"></param>
        /// <returns></returns>
        public Task<Result<QueryResult>> Execute(ILogsAndTestsRepository LogsAndTestsRepository);
    }
}

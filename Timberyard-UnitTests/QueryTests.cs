using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebService.Domain.Business.Queries;
using WebService.Domain.DataAccess;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests
{    
    public class QueryTests
    {       
        // Properties
        public QueriesController QueriesController;
        public Mock<LogsAndTestsRepository> RepositoryMock = new Mock<LogsAndTestsRepository>();
        public Mock<ILogger> LoggerMock = new Mock<ILogger>();
        // Constructor


        public QueryTests()
        {            
            RepositoryMock.Setup(repository => repository.ExecuteQuery(It.IsAny<CardYield>()))
               .ReturnsAsync(new Result<List<dynamic>>(true, new List<dynamic>(), ""));


            LoggerMock.Setup(logger => logger.Info(It.IsAny<string>(), It.IsAny<Dictionary<LogEntry, string>>(), It.IsAny<string>()));
            LoggerMock.Setup(logger => logger.Warning(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<Dictionary<LogEntry, string>>()));         

            QueriesController = new QueriesController(RepositoryMock.Object, LoggerMock.Object);          
        }


        [Theory]
        [InlineData("X16434", 2000 , 2001, true)]
        [InlineData("", 2000, 2001, false)]
        [InlineData("X16434", 2001, 2000, false)]
        [InlineData("", 2001, 2000, false)]
        public async void CardYield_ValidInputTest(string catalog, int startDate, int endDate, bool result)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateCardYield(catalog, new DateTime(startDate, 12, 12), new DateTime(endDate, 12, 12));
            Assert.Equal(result, queryResult.Status);
        }
    }
}

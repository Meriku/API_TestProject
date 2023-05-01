using API_TestProject;
using API_TestProject.Core.Model;
using API_TestProject.WebApi.Model.Request;
using API_TestProject.WebApi.Model.Response;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace API_TestProjectTests.WebApi.Controller
{
    [TestClass()]
    public class JournalControllerTests
    {
        private WebApplicationFactory<Program> _factory;

        [TestInitialize]
        public async Task Setup()
        {
            // Create a new web app factory for each test
            _factory = new WebApplicationFactory<Program>();

            var httpClient = _factory.CreateDefaultClient();
            var firstTree = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            var secondTree = await httpClient.PostAsync($"/api/tree/get?treeName={SecondTestTreeName}", httpContent);
            var node = await httpClient.PostAsync($"/api/tree/node/create?treeName={TestTreeName}&parentNodeId={-1}&nodeName={TestNodeName}", httpContent);

        }

        [TestCleanup]
        public void Cleanup()
        {
            // Dispose the web app factory after each test
            _factory.Dispose();
        }

        private const string TestTreeName = "Test_Tree";
        private const string SecondTestTreeName = "Second_Test_Tree";
        private const string TestNodeName = "Test_Node";
        private StringContent httpContent = new StringContent("");

        [TestMethod()]
        public async Task ErrorLog_ShouldBeCreated()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var unexistingNodeId = 9999999;
            var nodeName = "ErrorLog_ShouldBeCreatedNode";
            var skip = 0;
            var take = 10;
            var filter = new FilterDTO() { search = ""};
            var content = JsonContent.Create(filter);

            // Act
            var responseGetRange = await httpClient.PostAsync($"/api/journal/getrange?skip={skip}&take={take}", content);
            var responseGetRangeJson = await responseGetRange.Content.ReadAsStringAsync();
            var eventLogListDTO = JsonSerializer.Deserialize<EventLogListDTO>(responseGetRangeJson);
            var errorCountsBefore = eventLogListDTO.count;

            var response = await httpClient.PostAsync($"/api/tree/node/create?treeName={TestTreeName}&parentNodeId={unexistingNodeId}&nodeName={nodeName}", httpContent);

            var responseGetRangeAfter = await httpClient.PostAsync($"/api/journal/getrange?skip={skip}&take={take}", content);
            var responseGetRangeAfterJson = await responseGetRangeAfter.Content.ReadAsStringAsync();
            var eventLogListDTOAfter = JsonSerializer.Deserialize<EventLogListDTO>(responseGetRangeAfterJson);
            var errorCountsAfter = eventLogListDTOAfter.count;

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, responseGetRange.StatusCode);

            Assert.AreEqual(errorCountsBefore + 1, errorCountsAfter);
        }

        [TestMethod()]
        public async Task ErrorLogFilter_ShouldWork()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var unexistingNodeId = 9999999;
            var nodeName = "ErrorLog_ShouldBeCreatedNode";
            var skip = 0;
            var take = 10;
            var filter = new FilterDTO() { search = "" };
            var content = JsonContent.Create(filter);
            var desiredErrorsCount = 100;
            
            // Act
            for (var i = 0; i < desiredErrorsCount; i++)
            {
                var response = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={unexistingNodeId}&nodeName={nodeName}", httpContent);
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }

            var responseGetRange = await httpClient.PostAsync($"/api/journal/getrange?skip={skip}&take={take}", content);

            var responseGetRangeJson = await responseGetRange.Content.ReadAsStringAsync();
            var eventLogListDTO = JsonSerializer.Deserialize<EventLogListDTO>(responseGetRangeJson);
            var errorCounts = eventLogListDTO.count;
            var errorCountsByFilter = eventLogListDTO.items.Count();


            filter = new FilterDTO() { search = "", from = DateTime.Now.AddDays(3).ToUniversalTime().ToString() };
            content = JsonContent.Create(filter);
            var responseGetRangeWithDateFrom = await httpClient.PostAsync($"/api/journal/getrange?skip={skip}&take={take}", content);
            var responseGetRangeWithDateJsonFrom = await responseGetRangeWithDateFrom.Content.ReadAsStringAsync();
            var eventLogListDTOWithDateFrom = JsonSerializer.Deserialize<EventLogListDTO>(responseGetRangeWithDateJsonFrom);
            var errorCountsByFilterWithDateFrom = eventLogListDTOWithDateFrom.items.Count();

            filter = new FilterDTO() { search = "", to = DateTime.Now.AddDays(-3).ToUniversalTime().ToString() };
            content = JsonContent.Create(filter);
            var responseGetRangeWithDateTo = await httpClient.PostAsync($"/api/journal/getrange?skip={skip}&take={take}", content);
            var responseGetRangeWithDateJsonTo = await responseGetRangeWithDateTo.Content.ReadAsStringAsync();
            var eventLogListDTOWithDateTo = JsonSerializer.Deserialize<EventLogListDTO>(responseGetRangeWithDateJsonTo);
            var errorCountsByFilterWithDateTo = eventLogListDTOWithDateTo.items.Count();

            filter = new FilterDTO() { search = "Obviously not an EventId" };
            content = JsonContent.Create(filter);
            var responseGetRangeWithSearch = await httpClient.PostAsync($"/api/journal/getrange?skip={skip}&take={take}", content);
            var responseGetRangeWithJsonSearch = await responseGetRangeWithSearch.Content.ReadAsStringAsync();
            var eventLogListDTOWithSearch = JsonSerializer.Deserialize<EventLogListDTO>(responseGetRangeWithJsonSearch);
            var errorCountsByFilterWithSearch = eventLogListDTOWithSearch.items.Count();

            // Assert
            Assert.AreEqual(errorCounts, desiredErrorsCount);
            Assert.AreEqual(errorCountsByFilter, take);
            Assert.AreEqual(errorCountsByFilterWithDateFrom, 0);
            Assert.AreEqual(errorCountsByFilterWithDateTo, 0);
            Assert.AreEqual(errorCountsByFilterWithSearch, 0);
            Assert.AreEqual(HttpStatusCode.OK, responseGetRangeWithSearch.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, responseGetRangeWithDateFrom.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, responseGetRangeWithDateTo.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, responseGetRange.StatusCode);


        }

        [TestMethod()]
        public async Task GetSingleErrorLog_ShouldWork()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var unexistingNodeId = 9999999;
            var nodeName = "ErrorLog_GetSingleErrorLog";
            var desiredErrorsCount = 100;
            var httpContent = new StringContent("");

            // Act
            for (var i = 0; i < desiredErrorsCount; i++)
            {
                var response = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={unexistingNodeId}&nodeName={nodeName}", httpContent);
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);

                var exceptionLogDTOJson = await response.Content.ReadAsStringAsync();
                var exceptionLogDTO = JsonSerializer.Deserialize<ExceptionLogDTO>(exceptionLogDTOJson);
            }

            var filter = new FilterDTO() { search = "" };
            var content = JsonContent.Create(filter);
            var responseGetRange = await httpClient.PostAsync($"/api/journal/getrange?skip={0}&take={100}", content);
            var responseGetRangeJson = await responseGetRange.Content.ReadAsStringAsync();
            var eventLogListDTO = JsonSerializer.Deserialize<EventLogListDTO>(responseGetRangeJson);


            // Assert
            foreach (var item in eventLogListDTO.items)
            {
                var response = await httpClient.PostAsync($"/api/journal/getsingle?id={item.id}", httpContent);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}

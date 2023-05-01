using System.Diagnostics;
using System.Net;
using API_TestProject;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace API_TestProjectTests.WebApi.Controller
{
    [TestClass()]
    public class TreeControllerTests
    {
        private WebApplicationFactory<Program> _factory;

        [TestInitialize]
        public void Setup()
        {
            // Create a new web app factory for each test
            _factory = new WebApplicationFactory<Program>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Dispose the web app factory after each test
            _factory.Dispose();
        }

        private const string TestTreeName = "Test_Tree";
        private StringContent httpContent = new StringContent("");

        [TestMethod()]
        public async Task RunTreeControllerTests()
        {
            await TreeShouldBeObtained();
            await SameTreesShouldBeEqual();
            await TreesShouldBeCreated();
        }

        public async Task TreeShouldBeObtained()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            
            // Act
            var response = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Debug.WriteLine($"Test TestTreeShouldBeObtainable was done successfully. Tree: {TestTreeName} is created.");
        }

        public async Task SameTreesShouldBeEqual()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();

            // Act
            var response = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            var result = await response.Content.ReadAsStringAsync();

            var response2 = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            var result2 = await response2.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode);
            Assert.AreEqual(result, result2);

            Debug.WriteLine($"Test TheSameTreesShouldBeEqual was done successfully. Tree: {TestTreeName} was equal.");
        }

        public async Task TreesShouldBeCreated()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var treesCount = 30;

            // Act
            var treeNames = new List<string>();
            for (var i = 0; i < treesCount; i++)
            {
                var treeName = Guid.NewGuid().ToString();
                treeNames.Add(treeName);

                var response = await httpClient.PostAsync($"/api/tree/get?treeName={treeName}", httpContent);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            // Assert
            foreach (var treeName in treeNames)
            {
                var response = await httpClient.PostAsync($"/api/tree/get?treeName={treeName}", httpContent);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            Debug.WriteLine($"Test TheSameTreesShouldBeEqual was done successfully. Tree: {TestTreeName} was equal.");
        }
    }
}
using API_TestProject;
using API_TestProject.WebApi.Model.Response;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Text.Json;

namespace API_TestProjectTests.WebApi.Controller
{
    [TestClass()]
    public class NodeControllerTests
    {
        private WebApplicationFactory<Program> _factory;

        [TestInitialize]
        public async Task Setup()
        {
            // Create a new web app factory for each test
            _factory = new WebApplicationFactory<Program>();

            var httpClient = _factory.CreateDefaultClient();
            await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            await httpClient.PostAsync($"/api/tree/get?treeName={SecondTestTreeName}", httpContent);
            await httpClient.PostAsync($"/api/tree/get?treeName={ThirdTestTreeName}", httpContent);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Dispose the web app factory after each test
            _factory.Dispose();
        }

        private const string TestTreeName = "Test_Tree";
        private const string SecondTestTreeName = "Second_Test_Tree";
        private const string ThirdTestTreeName = "Third_Test_Tree";
        private const string TestNodeName = "Test_Node";
        private StringContent httpContent = new StringContent("");

        [TestMethod()]
        public async Task CreateNode_ShouldReturnOk()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var parentNodeId = -1;
            var createNodeName = Guid.NewGuid().ToString();

            // Act
            var treeResponse = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            var treeBodyJson = await treeResponse.Content.ReadAsStringAsync();
            var tree = JsonSerializer.Deserialize<TreeDTO>(treeBodyJson);
            var nodesCount = tree.children.Count();

            var response = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={parentNodeId}&nodeName={createNodeName}", httpContent);
            var treeResponse2 = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            var treeBodyJson2 = await treeResponse2.Content.ReadAsStringAsync();
            var tree2 = JsonSerializer.Deserialize<TreeDTO>(treeBodyJson2);
            var nodesCount2 = tree2.children.Count();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, treeResponse.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, treeResponse2.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Assert.AreEqual(nodesCount + 1, nodesCount2);
        }
        [TestMethod()]
        public async Task RenameNode_ShouldReturnOk()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var nodeNameToRename = "Node-To-Rename";
            var newNodeName = "new-name";
            var parentNodeId = -1;

            // Act
            var responseCreate = await httpClient.PostAsync($"api/tree/node/create?treeName={ThirdTestTreeName}&parentNodeId={parentNodeId}&nodeName={nodeNameToRename}", httpContent);

            var treeResponse = await httpClient.PostAsync($"/api/tree/get?treeName={ThirdTestTreeName}", httpContent);
            var treeBodyJson = await treeResponse.Content.ReadAsStringAsync();
            var tree = JsonSerializer.Deserialize<TreeDTO>(treeBodyJson);
            var nodeToRename = tree.children.First(x => x.name.Equals(nodeNameToRename));

            var response = await httpClient.PostAsync($"/api/tree/node/rename?treeName={ThirdTestTreeName}&nodeId={nodeToRename.id}&newNodeName={newNodeName}", httpContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, responseCreate.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [TestMethod()]
        public async Task DeleteNode_ShouldReturnOk()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var nodeNameToDelete = "Node-To-Delete";
            var parentNodeId = -1;

            // Act
            var responseCreate = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={parentNodeId}&nodeName={nodeNameToDelete}", httpContent);

            var treeResponse = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            var treeBodyJson = await treeResponse.Content.ReadAsStringAsync();
            var tree = JsonSerializer.Deserialize<TreeDTO>(treeBodyJson);
            var nodeToDelete = tree.children.First(x => x.name.Equals(nodeNameToDelete));

            var response = await httpClient.PostAsync($"/api/tree/node/delete?treeName={TestTreeName}&nodeId={nodeToDelete.id}", httpContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [TestMethod()]
        public async Task CreateNodes_ShouldWork()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var parentNodeId = -1;
            var addNodesCount = 50;

            // Act
            var treeResponse = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            var treeBodyJson = await treeResponse.Content.ReadAsStringAsync();
            var tree = JsonSerializer.Deserialize<TreeDTO>(treeBodyJson);
            var nodesCount1 = tree.children.Count();

            for (var i = 0; i < addNodesCount; i++)
            {
                var response = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={parentNodeId}&nodeName={TestNodeName + i}", httpContent);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            var treeResponse2 = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            var treeBodyJson2 = await treeResponse2.Content.ReadAsStringAsync();
            var tree2 = JsonSerializer.Deserialize<TreeDTO>(treeBodyJson2);
            var nodesCount2 = tree2.children.Count();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, treeResponse.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, treeResponse2.StatusCode);
            

            Assert.AreEqual(nodesCount1 + addNodesCount, nodesCount2);
        }
        [TestMethod()]
        public async Task CreateNodes_WitnSameName_ShouldNotBePossible()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var parentNodeId = -1;
            var nodeName = "SameNameTest";

            // Act

            var response = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={parentNodeId}&nodeName={nodeName}", httpContent);
            var response2 = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={parentNodeId}&nodeName={nodeName}", httpContent);


            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response2.StatusCode);
        }
        [TestMethod()]
        public async Task CreateNodes_WitnSameName_InDifferentTrees_ShouldBePossible()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var parentNodeId = -1;
            var nodeName = "Second_SameNameTest";

            // Act
            var response = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={parentNodeId}&nodeName={nodeName}", httpContent);
            var response2 = await httpClient.PostAsync($"api/tree/node/create?treeName={SecondTestTreeName}&parentNodeId={parentNodeId}&nodeName={nodeName}", httpContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode);
        }
        [TestMethod()]
        public async Task AddChildNodeToNode_ShouldBePossible()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
  
            var firstChildNodeName = "ParentAddChildNodeToNodeNode";
            var secondChildNodeName = "ChildAddChildNodeToNodeNode";
            var parentNodeId = -1;

            // Act
            var responseCreateChild = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={parentNodeId}&nodeName={firstChildNodeName}", httpContent);
            
            var treeResponse = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            var treeBodyJson = await treeResponse.Content.ReadAsStringAsync();
            var tree = JsonSerializer.Deserialize<TreeDTO>(treeBodyJson);
            var parentNode = tree.children.First(x => x.name.Equals(firstChildNodeName));

            var response = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={parentNode.id}&nodeName={secondChildNodeName}", httpContent);


            // Assert
            Assert.AreEqual(HttpStatusCode.OK, responseCreateChild.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, treeResponse.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [TestMethod()]
        public async Task AddChildNodeWithSameNameToNode_ShouldNotBePossible()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var treeResponse = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            var treeBodyJson = await treeResponse.Content.ReadAsStringAsync();
            var tree = JsonSerializer.Deserialize<TreeDTO>(treeBodyJson);
            var parentNode = tree.children.First();
            var childNodeName = "AddChildNodeToNodeWithSameName";

            // Act
            var response = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={parentNode.id}&nodeName={childNodeName}", httpContent);
            var response2 = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={parentNode.id}&nodeName={childNodeName}", httpContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response2.StatusCode);
        }
        [TestMethod()]
        public async Task AddChildNodeToAnotherTree_ShouldNotBePossible()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var childNodeName = "ChildNodeInAnotherTree";
            var parentNodeId = -1;

            // Act
            var responseCreateChild = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={parentNodeId}&nodeName={childNodeName}", httpContent);

            var treeResponse = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            var treeBodyJson = await treeResponse.Content.ReadAsStringAsync();
            var tree = JsonSerializer.Deserialize<TreeDTO>(treeBodyJson);
            var parentNode = tree.children.First();

            var response = await httpClient.PostAsync($"api/tree/node/create?treeName={SecondTestTreeName}&parentNodeId={parentNode.id}&nodeName={childNodeName}", httpContent);


            // Assert
            Assert.AreEqual(HttpStatusCode.OK, responseCreateChild.StatusCode);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }
        [TestMethod()]
        public async Task DeleteChildWithChildren_ShouldNotBePossible()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();
            var treeResponse = await httpClient.PostAsync($"/api/tree/get?treeName={TestTreeName}", httpContent);
            var treeBodyJson = await treeResponse.Content.ReadAsStringAsync();
            var tree = JsonSerializer.Deserialize<TreeDTO>(treeBodyJson);
            var childNodeName = "DeleteChildWithChildrenNode";
            var parentNode = tree.children.First();

            // Act
            var responseCreateChild = await httpClient.PostAsync($"api/tree/node/create?treeName={TestTreeName}&parentNodeId={parentNode.id}&nodeName={childNodeName}", httpContent);

            var responseDeleteParent = await httpClient.PostAsync($"/api/tree/node/delete?treeName={TestTreeName}&nodeId={parentNode.id}", httpContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, responseCreateChild.StatusCode);
            Assert.AreEqual(HttpStatusCode.InternalServerError, responseDeleteParent.StatusCode);
        }
        [TestMethod()]
        public async Task DeleteNodeWithUnexistingId_ShouldReturnError()
        {
            // Arrange
            var httpClient = _factory.CreateDefaultClient();

            // Act
            var response = await httpClient.PostAsync($"/api/tree/node/delete?treeName={TestTreeName}&nodeId=999999", httpContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

    }
}

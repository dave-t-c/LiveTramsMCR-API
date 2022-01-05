using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TfGM_API_Wrapper.Controllers;
using TfGM_API_Wrapper.Models;

namespace TfGM_API_Wrapper_Tests.TestControllers
{
    /// <summary>
    /// Test class for the StopsController class
    /// </summary>
    public class TestStopsController
    {
        private const string ValidStopLoaderPath = "../../../Resources/ValidStopLoader.json";
        private StopsController? _testStopController;
        
        [SetUp]
        public void Setup()
        {
            _testStopController = new StopsController(ValidStopLoaderPath);
        }

        /// <summary>
        /// Reset the test Stop Controller to clear any changes made
        /// during the tests.
        /// </summary>
        [TearDown]
        public void Teardown()
        {
            _testStopController = null;
        }
        
        /// <summary>
        /// Test to try and get the expected 200 result code
        /// from the GetAllStops method.
        /// </summary>
        [Test]
        public void TestGetAllStopsExpectedResultCode()
        {
            IActionResult result = _testStopController.GetAllStops();
            Assert.IsNotNull(result);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }
    }
}
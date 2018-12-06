using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Sweater.Api.Controllers;


namespace Sweater.Api.Test.Filters
{
    [TestFixture]
    public class CatchAllExceptionFilter
    {
        private Mock<ILogger<Api.Filters.CatchAllExceptionFilter>> _logger;
        private Api.Filters.CatchAllExceptionFilter _filter;
        private ExceptionContext _exceptionContext;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<Api.Filters.CatchAllExceptionFilter>>();
            _filter = new Api.Filters.CatchAllExceptionFilter(_logger.Object);

            var controller = new IndexerController(null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            _exceptionContext = new ExceptionContext(
                new ActionContext(controller.HttpContext, new RouteData(), new ActionDescriptor())
                , new List<IFilterMetadata>()
            );
        }

        [Test]
        public void CatchAllExceptionFilter_Should_Return_JsonResult()
        {
            _exceptionContext.Exception = new Exception();

            _filter.OnException(_exceptionContext);

            Assert.IsInstanceOf<JsonResult>(_exceptionContext.Result);
        }

        [Test]
        public void CatchAllExceptionFilter_Should_Return_JsonResult_With_400_Status_Code()
        {
            _exceptionContext.Exception = new Exception();

            _filter.OnException(_exceptionContext);

            Assert.AreEqual(400, ((JsonResult) _exceptionContext.Result).StatusCode);
        }
    }
}
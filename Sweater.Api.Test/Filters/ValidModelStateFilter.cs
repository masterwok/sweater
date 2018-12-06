using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Sweater.Api.Controllers;


namespace Sweater.Api.Test.Filters
{
    [TestFixture]
    public class ValidModelStateFilter
    {
        private Api.Filters.ValidModelStateFilter _filter;
        private ControllerContext _controllerContext;

        private static ResultExecutingContext CreateErrorContext(ActionContext context)
        {
            var modelState = new ModelStateDictionary();

            modelState.AddModelError("foo", "bar");

            return new ResultExecutingContext(
                new ActionContext(
                    context.HttpContext
                    , new RouteData()
                    , new ActionDescriptor()
                    , modelState
                )
                , new List<IFilterMetadata>()
                , null
                , context
            );
        }

        [SetUp]
        public void Setup()
        {
            _filter = new Api.Filters.ValidModelStateFilter();

            _controllerContext = new IndexerController(null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            }.ControllerContext;
        }

        [Test]
        public void OnResultExecuting_Should_Not_Set_Result_If_Valid_ModelState()
        {
            var resultExecutingContext = new ResultExecutingContext(
                new ActionContext(
                    _controllerContext.HttpContext
                    , new RouteData()
                    , new ActionDescriptor()
                    , new ModelStateDictionary()
                )
                , new List<IFilterMetadata>()
                , null
                , _controllerContext
            );

            _filter.OnResultExecuting(resultExecutingContext);

            Assert.IsNull(resultExecutingContext.Result);
        }

        [Test]
        public void OnResultExecuting_Should_Set_Result_If_Invalid_ModelState()
        {
            var resultExecutingContext = CreateErrorContext(_controllerContext);

            _filter.OnResultExecuting(resultExecutingContext);

            Assert.IsNotNull(resultExecutingContext.Result);
        }

        [Test]
        public void OnResultExecuting_Should_Return_JsonResult_If_Invalid_ModelState()
        {
            var resultExecutingContext = CreateErrorContext(_controllerContext);

            _filter.OnResultExecuting(resultExecutingContext);

            Assert.IsInstanceOf<JsonResult>(resultExecutingContext.Result);
        }

        [Test]
        public void OnResultExecuting_Should_Return_JsonResult_With_400_StatusCode_If_Invalid_ModelState()
        {
            var resultExecutingContext = CreateErrorContext(_controllerContext);

            _filter.OnResultExecuting(resultExecutingContext);

            Assert.AreEqual(400, ((JsonResult) resultExecutingContext.Result).StatusCode);
        }
    }
}
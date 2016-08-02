﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MediatR;
using XlsToEf.Example.ExampleBaseClassIdField;
using XlsToEf.Example.ExampleCustomMapperField;
using XlsToEf.Example.ExampleCustomMapperField.ProductCategoryFiles;
using XlsToEf.Import;

namespace XlsToEf.Example.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImportModal()
        {
            var importInfoModel = new ImportViewInfo
            {
                UploadUrl = Url.Action("UploadXlsx"),
                UploadTitle = "Import",
                ModalId = "modal"
            };
            return View("ImportModal", importInfoModel);
        }

        [HttpPost]
        public async Task<ActionResult> UploadXlsx(HttpPostedFileBase uploadFile)
        {
            if (uploadFile == null || uploadFile.ContentLength <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "ERROR: No file found");
            }
            var sheetInfo = await _mediator.SendAsync(new SaveAndGetSheetsForFileUpload {File = uploadFile.InputStream});


            sheetInfo.Destinations = new List<UploadDestinationInformation>
            {
                new UploadDestinationInformation
                {
                    Name = "Product",
                    SelectSheetUrl = Url.Action("SelectSheetAndDestinationForProduct"),
                    MatchSubmitUrl = Url.Action("SubmitProductColumnMatches"),
                },
                 new UploadDestinationInformation
                {
                    Name = "Product Category",
                    SelectSheetUrl = Url.Action("SelectSheetAndDestinationForProductCategory"),
                    MatchSubmitUrl = Url.Action("SubmitProductCategoryColumnMatches"),
                },
                new UploadDestinationInformation
                {
                    Name = "Order",
                    SelectSheetUrl = Url.Action("SelectSheetAndDestinationForOrder"),
                    MatchSubmitUrl = Url.Action("SubmitOrderColumnMatches"),
                }
            };


            return Json(sheetInfo);

        }

        public async Task<ActionResult> SelectSheetAndDestinationForProduct(XlsProductColumnMatcherQuery selectedInfo)
        {
            try
            {
                var data = await _mediator.SendAsync(selectedInfo);
                return Json(data);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "ERROR:" + ex.Message.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> SubmitProductColumnMatches(ImportMatchingProductData data)
        {
            var c = new DbContext("XlsToEf");
            var result = await _mediator.SendAsync(data);
            return Json(result);
        }
        public async Task<ActionResult> SelectSheetAndDestinationForProductCategory(XlsxProductCategoryColumnMatcherQuery selectedInfo)
        {
            try
            {
                var data = await _mediator.SendAsync(selectedInfo);
                return Json(data);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "ERROR:" + ex.Message.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> SubmitProductCategoryColumnMatches(ImportMatchingProductCategoryData data)
        {
            var c = new DbContext("XlsToEf");
            var result = await _mediator.SendAsync(data);
            return Json(result);
        }

        public async Task<ActionResult> SelectSheetAndDestinationForOrder(XlsxOrderColumnMatcherQuery selectedInfo)
        {
            try
            {
                var data = await _mediator.SendAsync(selectedInfo);
                return Json(data);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "ERROR:" + ex.Message.ToString());
            }
        }

        public async Task<JsonResult> SubmitOrderColumnMatches(ImportMatchingOrderData data)
        {
            var result = await _mediator.SendAsync(data);
            return Json(result);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Delives.pk.Models;
using Services.Models;
using Services.Services;

namespace Delives.pk.Apis
{
    [Authorize]
    public class DeliveryController : ApiController
    {
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("api/Delivery/OrdersWaitingForPickup")]  
        public ResponseModel OrdersWaitingForPickup(GetOrdersListRequestModel listModel)
        {
            var response = new ResponseModel
            {
                Success = false,
                Messages = new List<string>()
            };
            if (listModel == null)
            {
                response.Messages.Add("Mandatory data can not be empty");
            }
            else if (listModel.CurrentPage <= 0 || listModel.ItemsPerPage <= 0)
            {
                response.Messages.Add("Current page/ItemsPerPage should be greater than 0");
            }
            else
            {
                try
                {
                    var orders = OrderService.GetOrdersWaitingForPickup(listModel);
                    response.Data = orders;
                    response.Messages.Add("Success");
                    response.Success = true;
                }
                catch (Exception excep)
                {
                    response.Messages.Add("Something bad happened.");
                }
            }
            return response;
        }


        [HttpPost]
        [Route("api/Delivery/MyOrders")]
        public ResponseModel MyDeliveredOrders_DBOY(OrderHistoryForDBoyRequesrModel model)
        {
            var response = new ResponseModel
            {
                Success = false,
                Messages = new List<string>()
            };
            if (model == null ||string.IsNullOrEmpty(model.UserId))
            {
                response.Messages.Add("Data not mapped");
                response.Data = model;
            }
            else if (model.Status != OrderHistoryEnu.Canceled.Value && model.Status != OrderHistoryEnu.Placed.Value && model.Status != OrderHistoryEnu.Confirmed.Value &&
               model.Status != OrderHistoryEnu.WaitingForPickup.Value && model.Status != OrderHistoryEnu.PickedUp.Value && model.Status != OrderHistoryEnu.Deliverd.Value)
            {
                response.Messages.Add("Invalid order status");
                response.Data = model;
            }
            else
            {
                try
                {
                    var data =OrderService.MyOrderHistoryDBoy(model);
                    response.Data = data;
                    response.Messages.Add("Success");
                    response.Success = true;
                }
                catch (Exception excep)
                {
                    response.Messages.Add("Something bad happened.");
                }
            }
            return response;
        }
    }
}
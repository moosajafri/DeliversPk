using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Delives.pk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Services.Models;
using Services.Services;

namespace Delives.pk.Apis
{
    [Authorize]
    public class OrderController : ApiController
    {
        [HttpPost]
        [Route("api/order/place")]
        public ResponseModel PlaceOrder(PlaceOrderRequestModel listModel)
        {
            var response = new ResponseModel
            {
                Success = false,
                Messages = new List<string>()
            };
            if (listModel == null || listModel.Items == null || listModel.Items.Count==0 && string.IsNullOrEmpty(listModel.Cords))     
            {
                response.Messages.Add("Data not mapped");
                response.Data = listModel;
            }
            else if (listModel.Cords.Split('_').Length != 2)
            {
                response.Messages.Add("Invalid Cord format. Please specify in Lat_Lang .i.e. '32.202895_74.176716'");
                response.Data = listModel;
            }
            else
            {
                try
                {
                    var contextnew = new ApplicationDbContext();
                    var userStore = new UserStore<ApplicationUser>(contextnew);
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    var user= userManager.FindByName(User.Identity.Name);

                    var item = OrderService.Place(listModel, user.Id);
                    response.Data = item;
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
        [Route("api/order/getDetails")]
        public ResponseModel GetOrderDetails(GetOrderDetailsRequest model)
        {
            var response = new ResponseModel
            {
                Success = false,
                Messages = new List<string>()
            };
            if (model == null || string.IsNullOrEmpty(model.OrderId))
            {
                response.Messages.Add("Data not mapped");
                response.Data = model;
            }
           
            else
            {
                try
                {                   
                    var order = OrderService.GetOrderDetails(model.OrderId);
                    response.Data = order;
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
        [Route("api/order/estimatedDeliveryCharges")]
        public ResponseModel GetDeliveryCharges(PlaceOrderRequestModel listModel)
        {
            var response = new ResponseModel
            {
                Success = false,
                Messages = new List<string>()
            };
            if (listModel == null || listModel.Items == null || listModel.Items.Count == 0 && string.IsNullOrEmpty(listModel.Cords))
            {
                response.Messages.Add("Data not mapped");
                response.Data = listModel;
            }
            else if (listModel.Cords.Split('_').Length != 2)
            {
                response.Messages.Add("Invalid Cord format. Please specify in Lat_Lang .i.e. '32.202895_74.176716'");
                response.Data = listModel;
            }
            else
            {
                try
                {                   
                    response.Data = new { DeliveryAmount=50};
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
        [Route("api/order/updateStatus")]
        public ResponseModel UpdateOrderStatus(ChangeOrderStatusRequesrModel model)
        {
            var response = new ResponseModel
            {
                Success = false,
                Messages = new List<string>()
            };
            if (model == null || string.IsNullOrEmpty(model.OrderId) || string.IsNullOrEmpty(model.NewStatus) || string.IsNullOrEmpty(model.UserId))
            {
                response.Messages.Add("Data not mapped");
                response.Data = model;
            }
            else if (model.NewStatus!=OrderHistoryEnu.Canceled.Value && model.NewStatus != OrderHistoryEnu.Placed.Value && model.NewStatus != OrderHistoryEnu.Confirmed.Value &&
                model.NewStatus != OrderHistoryEnu.WaitingForPickup.Value && model.NewStatus != OrderHistoryEnu.PickedUp.Value && model.NewStatus != OrderHistoryEnu.Deliverd.Value)
            {
                response.Messages.Add("Invalid order status");
                response.Data = model;
            }
            else
            {
                try
                {
                    var currentOrderStatus = OrderService.GetOrderCurrentStatus(model.OrderId);
                    var newOrderStatus = OrderHistoryEnu.GetOrderStatus(model.NewStatus);

                    if(currentOrderStatus==null || newOrderStatus == null)
                    {
                        response.Success = false;
                        response.Messages.Add("Invalid orderId/Order Status.");
                        response.Data = model;
                        return response;
                    }

                    if(newOrderStatus.Order - currentOrderStatus.Order != 1)
                    {
                        response.Success = false;
                        response.Messages.Add("Invalid order status shifting. Current status is :" + currentOrderStatus.Value);
                        response.Data = currentOrderStatus.Value + " -> " + newOrderStatus.Value;
                        return response;
                    }


                    response.Success = OrderService.ChangeOrderStatus(model);
                    if (response.Success)
                    {
                        response.Data = "status has been changed";
                    }
                    else
                    {
                        response.Data = "Order does not exist with id "+model.OrderId;
                    }
                    response.Messages.Add("Success");
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Services.DbContext;
using Services.Models;

namespace Services.Services
{
    public static class OrderService
    {
        public static string Place(PlaceOrderRequestModel request, string orderBy)
        {
            using (var dbContext = new DeliversEntities())
            {
                var totalAmount =
                    request.Items.Sum(i => ItemDetailsService.GetItemDetailLocalById(i.ItemId).Price*i.Quantity);
                var order = new Order
                {
                    Address = request.Address,
                    Instructions = request.Instructions,
                    Status = OrderHistoryEnu.WaitingForPickup.Value,
                    OrderBy = orderBy,
                    Amount = totalAmount,
                    DateTime = DateTime.Now,
                    EstimatedTime = "45 minutes away",
                };
                dbContext.Orders.Add(order);
                foreach (var item in request.Items)
                {
                    var itmObj = new OrderDetail
                    {
                        OrderId = order.Id,
                        ItemId = item.ItemId,
                        Quantity = item.Quantity,                        
                    };
                    dbContext.OrderDetails.Add(itmObj);
                }
                dbContext.OrderHistories.Add(new OrderHistory
                {
                    OrderId = order.Id,
                    DateTime = DateTime.Now,
                    MovedBy = orderBy,
                    Status = OrderHistoryEnu.WaitingForPickup.Value
                });
                dbContext.SaveChanges();
                return order.EstimatedTime;
            }
        }

        public static GetOrdersResponseModel GetOrdersWaitingForPickup(GetOrdersListRequestModel requestModel)
        {
            using (var dbContext = new DeliversEntities())
            {
                requestModel.CurrentPage--;
                var response = new GetOrdersResponseModel();
                var list = dbContext.Orders
                    .Where(
                        od =>
                            od.OrderHistories.Any(str => str.Status == OrderHistoryEnu.WaitingForPickup.Value))
                    .ToList();  
                //&&
                //(string.IsNullOrEmpty(requestModel.SearchTerm) ||
                //(od.OrderDetails.Any(det => det.ItemDetail.ListItem.Name.ToLower().Contains(requestModel.SearchTerm.ToLower())) &&
                //od.OrderHistories.Count == 1 && od.OrderHistories.Any(str => str.Status == (int)OrderHistoryEnum.WaitingForPickup)))).ToList();


                if (list.Any())
                {
                    var take = list.Skip(requestModel.CurrentPage * requestModel.ItemsPerPage).
                        Take(requestModel.ItemsPerPage).ToList();
                    if (take.Any())
                    {
                        var finals = take.Select(obj => obj.MappOrderWaitingForPickup()).ToList();
                        response.Orders = finals;
                    }
                }
                response.ItemsPerPage = requestModel.ItemsPerPage;
                response.CurrentPage++;
                response.TotalItems = list.Count;
                return response;
            }
        }

        public static OrderLocal GetOrderDetails(string orderId)
        {
            using (var dbContext = new DeliversEntities())
            {
                var order = dbContext.Orders.FirstOrDefault(o => o.Id.ToString() == orderId);
                if (order != null)
                {
                   return order.MappOrder();
                }
                return null;
            }
        }

        public static bool ChangeOrderStatus(ChangeOrderStatusRequesrModel model)
        {
            using (var dbContext = new DeliversEntities())
            {
                var order = dbContext.Orders.FirstOrDefault(o => o.Id.ToString() == model.OrderId);
                if (order != null)
                {
                    order.Status = model.NewStatus;
                    dbContext.OrderHistories.Add(new OrderHistory
                    {
                        OrderId = order.Id,
                        DateTime = DateTime.Now,
                        MovedBy = model.UserId,
                        Status = model.NewStatus
                    });
                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static OrderHistoryEnu GetOrderCurrentStatus(string orderid)
        {
            using (var dbContext = new DeliversEntities())
            {
                var order = dbContext.Orders.FirstOrDefault(o => o.Id.ToString() == orderid);
                if (order != null)
                {
                    var orderStatus = OrderHistoryEnu.GetOrderStatus(order.Status);
                    return orderStatus;
                }
                return null;
            }
        }

        #region Delivery Boy

        public static DeliveredOrdersDBoyResponseModel MyOrderHistoryDBoy(OrderHistoryForDBoyRequesrModel requestModel)
        {
            using (var dbContext = new DeliversEntities())
            {
                var response = new DeliveredOrdersDBoyResponseModel();

                var orderStatus = OrderHistoryEnu.GetOrderStatus(requestModel.Status);

                var list = dbContext.Orders
                    .Where(
                        od =>
                            od.OrderHistories.Any(str => str.Status == orderStatus.Value && str.MovedBy==requestModel.UserId))
                    .ToList();
               
                if (list.Any())
                {
                    var finals = list.Select(obj => obj.MappOrder()).ToList();
                    response.Orders = finals;
                }              
                return response;
            }
        }

        #endregion
    }
}

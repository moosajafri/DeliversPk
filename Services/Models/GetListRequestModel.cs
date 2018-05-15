using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class GetListRequestModel : PaggingModel
    {
        public int Type  { get; set; }

        public string Cords { get; set; } // lat_long

        public string SearchTerm { get; set; }

    }

    public class GetMenuRequestModel : PaggingModel
    {
        public long ItemId { get; set; }

        public string SearchTerm { get; set; }

    }

    public class GetItemDetailsRequestModel 
    {
        public long ItemId { get; set; }

    }

    public class PlaceOrderRequestModel
    {
        public List<PlaceOrderItem> Items  { get; set; }

        public string Address { get; set; }

        public string Instructions { get; set; }

        public int PaymentMethod { get; set; }

        public string Cords { get; set; }
    }


    public class GetOrderDetailsRequest
    {
        public string OrderId { get; set; }

    }

    public class PlaceOrderItem
    {
        public long ItemId { get; set; }

        public int Quantity { get; set; }
    }


    public class GetOrdersListRequestModel : PaggingModel
    {
        public string Cords { get; set; } // lat_long

        public string SearchTerm { get; set; }

    }


    public class ChangeOrderStatusRequesrModel     {
        public string OrderId { get; set; } 

        public string NewStatus { get; set; }

        public string UserId { get; set; }

    }



    public class OrderHistoryForDBoyRequesrModel
    {
        public string UserId { get; set; }

        public string Status { get; set; }

    }

}

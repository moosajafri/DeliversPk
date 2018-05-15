using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class GetListResponseModel: PaggingModel
    {
       public List<ListItemLocal> Items { get; set; }
       
    }

    public class GetOrdersResponseModel : PaggingModel
    {
        public List<OrderLocal_waitingForPickup> Orders { get; set; }

    }


    public class DeliveredOrdersDBoyResponseModel
    {
        public List<OrderLocal> Orders { get; set; }

    }
}

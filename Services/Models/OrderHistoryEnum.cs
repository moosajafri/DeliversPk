using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
   
    public class OrderHistoryEnu
    {
        private OrderHistoryEnu(string value, int order) {
            Value = value;
            Order = order;
        }

        public string Value { get; set; }
        public int Order { get; set; }

        public static OrderHistoryEnu Placed { get { return new OrderHistoryEnu("Placed",0); } }
        public static OrderHistoryEnu Canceled { get { return new OrderHistoryEnu("Canceled",1); } }
        public static OrderHistoryEnu Confirmed { get { return new OrderHistoryEnu("Confirmed",1); } }
        public static OrderHistoryEnu WaitingForPickup { get { return new OrderHistoryEnu("WaitingForPickup",2); } }
        public static OrderHistoryEnu PickedUp { get { return new OrderHistoryEnu("PickedUp",3); } }

        public static OrderHistoryEnu Deliverd { get { return new OrderHistoryEnu("Deliverd",4); } }

        public static OrderHistoryEnu GetOrderStatus(string stat)
        {
            stat = stat.ToLower();
            if (stat == "placed")
            {
                return new OrderHistoryEnu("Placed", 0);
            }
            else if (stat == "canceled")
            {
                return new OrderHistoryEnu("Canceled", 1);
            }
            else if (stat == "confirmed")
            {
                return new OrderHistoryEnu("Confirmed", 1);
            }
            else if (stat == "waitingforpickup")
            {
                return new OrderHistoryEnu("WaitingForPickup", 2);
            }
            else if (stat == "pickedup")
            {
                return new OrderHistoryEnu("PickedUp", 3);
            }
            else if (stat == "deliverd")
            {
                return new OrderHistoryEnu("Deliverd", 4);
            }
            else
                return null;
        }
    }
}

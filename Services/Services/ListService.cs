using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.DbContext;
using Services.Models;
using System.Data.Entity.Spatial;

namespace Services.Services
{
    public static class ListService
    {
        public static GetListResponseModel GetItemsForList(GetListRequestModel requestModel)
        {
            DbGeography userLoc = null;
            List<string> latlng = new List<string>();
            if (!string.IsNullOrEmpty(requestModel.Cords) && requestModel.Cords != "")
            {
                latlng = requestModel.Cords.Split('_').ToList();
                if (latlng.Count == 2)
                {
                    userLoc = CommonService.ConvertLatLonToDbGeography(latlng[1], latlng[0]); // lat _ lng
                }
            }
            using (var dbContext = new DeliversEntities())
            {
                requestModel.CurrentPage--;
                var response = new GetListResponseModel();
                var newList = new List<ListItemLocal>();

                var list =dbContext.ListItems.Where(item => item.Type == requestModel.Type && 
                (string.IsNullOrEmpty(requestModel.SearchTerm) || 
                item.Name.ToLower().Contains(requestModel.SearchTerm.ToLower()))).ToList();
                if (list.Any())
                {                                        
                    var take = list.Skip(requestModel.CurrentPage*requestModel.ItemsPerPage).
                        Take(requestModel.ItemsPerPage).ToList();
                    if (take.Any())
                    {
                        var finals = take.Select(obj => obj.MapListItem()).ToList();
                        ///
                        foreach (var rest in finals)
                        {
                            var dist = CommonService.GetDistance((double)userLoc.Latitude, (double)userLoc.Longitude, Convert.ToDouble(rest.LocationObj.Latitude), Convert.ToDouble(rest.LocationObj.Longitude));
                           // if ((int)dist < Convert.ToInt16(12))
                            {
                                var disst = Math.Round((double)dist, 2);
                                rest.LocationObj = null;
                                rest.Distance = disst;
                                rest.Name = rest.Name + "\naddress:" +rest.Address + "| distance: " + disst+" km";
                                newList.Add(rest);
                            }
                        }
                        response.Items = newList.OrderBy( obj => obj.Distance).ToList();
                    }
                }
                response.ItemsPerPage = requestModel.ItemsPerPage;
                response.CurrentPage++;
                response.TotalItems = list.Count;
                return response;
            }
        }

        public static List<ItemDetailLocal_Short> GetMenuByListItemId(long id)
        {
            using (var dbcontext = new DeliversEntities())
            {
                var items = dbcontext.ItemDetails.Where(det => det.ListItemId == id).ToList();
                if (items.Any())
                {
                    return items.Select(obj => obj.ItemDetailShortMapper()).ToList();
                }
                return null;
            }
        }

        public static void AddNewRestaurent(ListItemLocal source)
        {
            using (var dbContext = new DeliversEntities())
            {
                DbGeography loc = null;
                if (!String.IsNullOrEmpty(source.Location) && source.Location != "")
                {
                    var latlng = source.Location.Split('_');
                    if (latlng.Length == 2)
                    {
                        loc = CommonService.ConvertLatLonToDbGeography(latlng[1], latlng[0]); // lat _ lng
                    }
                }
                var dbObj = new ListItem
                {
                    Location = loc,
                    Name = source.Name,
                    Description = source.Description,
                    Phone = source.Phone,
                    LogoImage = source.LogoImage,
                    LastEdit = DateTime.Now,
                    BgImage = source.BgImage,
                    Address = source.Address,
                    Rating = source.Rating,
                    Type = source.Type,
                    Id= source.Id,
                    Status = source.Status,
                    Cords = loc,
                    CreationDate = DateTime.Now                   
                };
                dbContext.ListItems.Add(dbObj);
                dbContext.SaveChanges();
            }
        }
    }
}

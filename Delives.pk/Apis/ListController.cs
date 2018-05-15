﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Delives.pk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Services.Models;
using Services.Services;

namespace Delives.pk.Apis
{
    [Authorize]
    public class ListController : ApiController
    {
        [HttpPost]
        [Route("api/Listing/GetItems")]  // of restaurants, super stores etc
        public ResponseModel GetListOfItems(GetListRequestModel listModel)
        {
            var response = new ResponseModel
            {
                Success = false,
                Messages = new List<string>()
            };
            if (listModel == null || listModel.Type == 0)    // 1. food  2.grocery 
            {
                response.Messages.Add("Mandatory data can not be empty");
            }
            else if (listModel.CurrentPage<=0 || listModel.ItemsPerPage<=0)
            {
                response.Messages.Add("Current page/ItemsPerPage should be greater than 0");
            }
            else
            {
                try
                {
                    var items = ListService.GetItemsForList(listModel);
                    response.Data = items;
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
        [Route("api/Listing/GetMenu")]
        public ResponseModel GetMenu(GetMenuRequestModel listModel)
        {
            var response = new ResponseModel
            {
                Success = false,
                Messages = new List<string>()
            };
            if (listModel == null || listModel.ItemId == 0)    
            {
                response.Messages.Add("ItemId can not be empty");
            }
            else if (listModel.CurrentPage <= 0 || listModel.ItemsPerPage <= 0)
            {
                response.Messages.Add("Current page/ItemsPerPage should be greater than 0");
            }
            else
            {
                try
                {
                    var menuItems = ListService.GetMenuByListItemId(listModel.ItemId);
                    response.Data = menuItems;
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
        [Route("api/Item/AddReview")]
        public ResponseModel AddReview(RatingLocal rating)
        {
            var response = new ResponseModel
            {
                Success = false,
                Messages = new List<string>()
            };
            if (rating == null || string.IsNullOrEmpty(rating.RatedByUserId) ||
                rating.RatedToItem ==0)
            {
                response.Messages.Add("Data values are missing");
            }
            else if ( string.IsNullOrEmpty(rating.Comments) || rating.RatingStar < 0)
            {
                response.Messages.Add("Rating and comments can not be empty");
            }
            else
            {
                try
                {
                    var localUsermanager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    ApplicationUser user = localUsermanager.FindById(rating.RatedByUserId);
                    if (user == null)
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Messages = new List<string> { "User not found with given user id" },
                            Data = rating
                        };
                    }


                    var responseRate = ReviewService.AddReview(rating);
                    response.Data = rating;
                    response.Messages.Add(responseRate.ToString());
                    response.Success = responseRate;
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

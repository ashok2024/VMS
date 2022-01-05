using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using VMS.Models;
using VMS.Models.Account;

namespace VMS.Controllers.Account
{
    [OutputCacheAttribute(Duration = 0, NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
    public class BaseController : Controller
    {
        public static HttpClient httpClient;
        Object CacheObj;
        public UserModel AppUser;
        public long UserId;

        //public BaseController()
        public BaseController()
        {
            string userName = System.Web.HttpContext.Current.Request["userName"];

            long userId = Convert.ToInt64(System.Web.HttpContext.Current.Request["userId"]);

            string CacheKeyNameUser = userName;

            ApplicationCache applicationCache = new ApplicationCache();
            if (CacheKeyNameUser != null)
                CacheObj = applicationCache.GetMyCachedItem(CacheKeyNameUser);
            if (CacheObj != null)
                AppUser = (UserModel)CacheObj;
            if (AppUser != null)
            {
                UserId = AppUser.UserId;
            }
        }
    }
}
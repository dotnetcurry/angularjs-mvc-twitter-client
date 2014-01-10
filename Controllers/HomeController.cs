using GettingStartedWithAngularJS.Models;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GettingStartedWithAngularJS.Controllers
{
    public class HomeController : Controller
    {
        private IOAuthCredentials credentials = new SessionStateCredentials();
        private GettingStartedWithAngularJS.L2TAuthorizer.MvcAuthorizer auth;
        private TwitterContext twitterCtx;
        public ActionResult Index()
        {
            var unAuthorized = Authorize();
            if (unAuthorized == null)
            {
                if (Request.QueryString.Count > 0)
                {
                    return RedirectToAction("Index");
                }
                return View("Index");
            }
            else
            {
                return unAuthorized;
            }
        }
        private ActionResult Authorize()
        {
            if (credentials.ConsumerKey == null || credentials.ConsumerSecret == null)
            {
                credentials.ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"];
                credentials.ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"];
            }
            auth = new GettingStartedWithAngularJS.L2TAuthorizer.MvcAuthorizer
            {
                Credentials = credentials
            };
            auth.CompleteAuthorization(Request.Url);
            if (!auth.IsAuthorized)
            {
                Uri specialUri = new Uri(Request.Url.ToString());
                return auth.BeginAuthorization(specialUri);
            }
            ViewBag.User = auth.Credentials.ScreenName;
            return null;
        }

        [HttpGet]
        public JsonResult Tweet()
        {
            Authorize();
            string screenName = ViewBag.User;
            IEnumerable<TweetViewModel> friendTweets = new List<TweetViewModel>();
            if (string.IsNullOrEmpty(screenName))
            {
                return Json(friendTweets, JsonRequestBehavior.AllowGet);
            }
            twitterCtx = new TwitterContext(auth);
            friendTweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Home &&
                       tweet.ScreenName == screenName &&
                       tweet.IncludeEntities == true
                 select new TweetViewModel
                 {
                     ImageUrl = tweet.User.ProfileImageUrl,
                     ScreenName = tweet.User.Identifier.ScreenName,
                     MediaUrl = GetTweetMediaUrl(tweet),
                     Tweet = tweet.Text,
                     Id = tweet.StatusID
                 })
                .ToList();
            return Json(friendTweets, JsonRequestBehavior.AllowGet);
        }
        private string GetTweetMediaUrl(Status status)
        {
            if (status.Entities != null && status.Entities.MediaEntities.Count > 0)
            {
                return status.Entities.MediaEntities[0].MediaUrlHttps;
            }
            return "";
        }

        [HttpPost]
        public JsonResult Tweet(string tweet)
        {
            Authorize();
            twitterCtx = new TwitterContext(auth);
            try
            {
                Status stat = twitterCtx.UpdateStatus(tweet);
                if (stat != null)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Unknown Error" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult Retweet(string id)
        {
            Authorize();
            twitterCtx = new TwitterContext(auth);
            try
            {
                Status stat = twitterCtx.Retweet(id);
                if (stat != null)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Unknown Error" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }


        [HttpGet]
        public JsonResult Status(string id)
        {
            Authorize();
            string screenName = ViewBag.User;
            IEnumerable<TweetViewModel> friendTweets = new List<TweetViewModel>();
            if (string.IsNullOrEmpty(screenName))
            {
                return Json(friendTweets, JsonRequestBehavior.AllowGet);
            }
            twitterCtx = new TwitterContext(auth);
            friendTweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Show &&
                       tweet.ID == id
                 select GetTweetViewModel(tweet))
                .ToList();
            if (friendTweets.Count() > 0)
                return Json(friendTweets.ElementAt(0), JsonRequestBehavior.AllowGet);
            else
                return Json(new TweetViewModel { Tweet = "Requested Status Not Found" }, JsonRequestBehavior.AllowGet);
        }

        private TweetViewModel GetTweetViewModel(Status tweet)
        {
            var tvm = new TweetViewModel
            {
                ImageUrl = tweet.User.ProfileImageUrl,
                ScreenName = tweet.User.Identifier.ScreenName,
                MediaUrl = GetTweetMediaUrl(tweet),
                Tweet = tweet.Text,
                Id = tweet.StatusID,
                FavoriteCount = tweet.FavoriteCount.ToString(),
                RetweetCount = tweet.RetweetCount.ToString(),

            };
            tvm.HasMedia = !string.IsNullOrEmpty(tvm.MediaUrl);
            return tvm;
        }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GettingStartedWithAngularJS.Models
{
    public class TweetViewModel
    {
        public string ImageUrl { get; set; }
        public string ScreenName { get; set; }
        public string MediaUrl { get; set; }
        public string Tweet { get; set; }
        public string Id { get; set; }

        public string FavoriteCount { get; set; }

        public string RetweetCount { get; set; }

        public bool HasMedia { get; set; }
    }

}
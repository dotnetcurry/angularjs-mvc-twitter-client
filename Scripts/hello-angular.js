var ngTwitter = angular.module("ngTwitter", ['ngResource', 'ngRoute']);
ngTwitter.config(function ($routeProvider) {
    $routeProvider.when(
     "/", {
         templateUrl: "timeline",
         controller: "TimelineController"
     }).when(
        "/status/:id", {
            templateUrl: "status",
            controller: "StatusController"
        });
});

ngTwitter.factory("TwitterService", function ($resource, $http) {
    return {
        timeline: $resource("/Home/Tweet")
    };
});

ngTwitter.directive("retweetButton", function ($http, $routeParams) {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        controller: function ($scope, $element) {
            $element.on("click", function () {
                var resultPromise = $http.post("/Home/Retweet/", $scope.status);
                resultPromise.success(function (data) {ms
                    if (data.success) {
                        alert("Retweeted successfully");
                    }
                    else {
                        alert("ERROR: Retweeted failed! " + data.errorMessage);
                    }
                });
            });

        },
        template: "<button class='btn btn-xs'><span class='glyphicon glyphicon-retweet'>&nbsp;</span><span ng-transclude></span></button>"
    };
});

ngTwitter.controller("StatusController", function ($scope, $http, $routeParams,
TwitterService) {
    var resultPromise = $http.get("/Home/Status/" + $routeParams.id);
    resultPromise.success(function (data) {
        $scope.status = data;
    });
});

ngTwitter.controller("TimelineController", function ($scope, $http, $routeParams, TwitterService) {
    $scope.tweets = TwitterService.timeline.query({}, isArray = true);

    $scope.newTweets = {
        0: "No new Tweets",
        other: "{} new Tweets"
    };

    $scope.sendStatus = function () {
        var tweetText = $scope.statusText;
        var newTimeLine = new TwitterService.timeline(
        {
            Tweet: tweetText
        });
        newTimeLine.$save(function (data, headers) {
            if (data.success && data.success == true) {
                alert("Tweet Sent Successfully!");
                $scope.statusText = "";
            }
            else {
                alert("ERROR: " + data.errorMessage);
            }
        });
    }

    //$scope.retweet = function (item) {
    //    var resultPromise = $http.post("/Home/Retweet/", item);
    //    resultPromise.success(function (data) {
    //        if (data.success) {
    //            alert("Retweeted successfully");
    //        }
    //        else {
    //            alert("ERROR: Retweeted failed! " + data.errorMessage);
    //        }
    //    });
    //};
});
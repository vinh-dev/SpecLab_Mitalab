var myApp = angular.module('myApp', ['ngSanitize']);

var HomeController = function HomeController($scope, $http, $window) {

    $scope.homePageContent = "";

    $scope.getHomePageContent = function () {
        $http({
            method: 'POST',
            url: $window.getContentDetailUrl,
            data: {
                ContentId: $window.contentIds.homePageContentId,
            }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.homePageContent = data.ContentText;
            }
        });
    };

    $scope.getHomePageContent();
};

$(document).ready(function () {
    $('.list-group-item.home').toggleClass("active");
});
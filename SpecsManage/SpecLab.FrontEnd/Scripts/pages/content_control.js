var myApp = angular.module('myApp', []);

var ContentControlController = function ContentControlController($scope, $http, $window) {

    $scope.homePageContent = "";

    $scope.updateContent = function () {

        console.log(CKEDITOR.instances.txtContent.getData());

        $http({
            method: 'POST',
            url: $window.saveContentUrl,
            data: {
                ContentId: $window.contentIds.homePageContentId,
                ContentText: CKEDITOR.instances.txtContent.getData(),
            }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

            }
        });
    };

    $scope.getContentDetail = function () {
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

                CKEDITOR.replace('txtContent');
            }
        });
    };

    $scope.getContentDetail();
};

$(document).ready(function () {
    $('.list-group-item.home').toggleClass("active");
});
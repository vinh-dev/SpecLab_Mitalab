var myApp = angular.module('myApp', []);

var ReportsSpecLabController = function ReportsSpecLabController($scope, $http, $window) {
    $scope.exportHistory = {
        fromDate: $window.exportHistoryDefaultStartDate,
        toDate: $window.exportHistoryDefaultEndDate
    };

    $scope.convertDateUniversalStr = function (strDate) {

        var dateParts = strDate.split("/");
        if (dateParts.length != 3) {
            return "";
        }

        return dateParts[2] + dateParts[1] + dateParts[0];
    };
    
};

$(document).ready(function () {
    $('.list-group-item.report').toggleClass("active");
    
    $('.input-group.date').datepicker({
        format: "dd/mm/yyyy",
        language: "vi",
        todayHighlight: true
    });
});
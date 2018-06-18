var myApp = angular.module('myApp', []);

var StorageRemovalController = function StorageRemovalController($scope, $http, $window) {

    $scope.removalSearchCriteria = {
        fromDate: $window.defaultSearchValue.fromDate,
        toDate: $window.defaultSearchValue.toDate,
        removalId: ""
    };

    $scope.removalList = [
        //{ removalId: 'DEL.20140201.10', removalDate: '11/02/2014 18:00:00', userFrom: 'phuongnh: Nguyen Ha Phuong', numberRemoval: 5 },
    ];

    $scope.selectedRemoval = null;

    $scope.submitFindTubeList = function () {

        for (var i = 0; i < $scope.findTubeList.length; i++) {
            if ($scope.findTubeList[i].selected == true) {
                $scope.newRemoval.tubeList.push($scope.findTubeList[i]);
            }
        }
    };
    
    $scope.getPrintUrl = function (removalId) {
        return $window.printUrl + "/" + removalId;
    };
    
    $scope.showRemovalDetail = function (item) {

        $http({
            method: 'POST',
            url: $window.detailUrl,
            data: {
                RemovalId: item.RemovalId,
            }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.selectedRemoval = {
                    RemovalId: data.RemovalNote.RemovalId,
                    NumberRemoval: data.RemovalNote.NumberRemoval,
                    RemovalDate: data.RemovalNote.RemovalDateDisplay,
                    RemovalUser: data.RemovalNote.RemovalUserId,
                    RemovalReason: data.RemovalNote.RemovalReason,
                    TubeList: []
                };
                for (var i = 0; i < data.RemovalNote.RemovalNoteDetails.length; i++) {
                    $scope.selectedRemoval.TubeList.push(data.RemovalNote.RemovalNoteDetails[i]);
                } 

                $('#dialogRemovalDetail').modal('show');
            }
        });
    };
    
    $scope.searchRemoval = function () {

        $http({
            method: 'POST',
            url: $window.searchUrl,
            data: {
                RemovalId: $scope.removalSearchCriteria.removalId,
                StartDate: $scope.removalSearchCriteria.fromDate,
                EndDate: $scope.removalSearchCriteria.toDate
            }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.removalList = data.ListData;
            }
        });
    };

    $scope.searchRemoval();
};

$(document).ready(function () {
    
    $('#collapseSpecsContent_Search .input-group.date').datepicker({
        format: "dd/mm/yyyy",
        language: "vi",
        todayHighlight: true
    });

    $('#dialogRemovalDetail').modal({ keyboard: true, show: false, backdrop: 'static' });
    $('.list-group-item.removal-list').toggleClass("active");
});
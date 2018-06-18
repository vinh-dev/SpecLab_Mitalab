var myApp = angular.module('myApp', []);

var StorageExportController = function StorageExportController($scope, $http, $window) {

    $scope.exportSearchCriteria = {
        fromDate: $window.defaultSearchValue.fromDate,
        toDate: $window.defaultSearchValue.toDate,
        exportId: ""
    };

    $scope.exportList = [
        //{ exportId: 'EX.20140201.10', exportDate: '11/02/2014 18:00:00', userFrom: 'phuongnh: Nguyen Ha Phuong', userTo: 'phuongnh: Nguyen Ha Phuong', numberExport: 5 },
    ];

    $scope.selectedExport = null;

    $scope.getPrintUrl = function (exportId) {
        return $window.printUrl + "/" + exportId;
    };

    $scope.showExportDetail = function (item) {
        
        $http({
            method: 'POST',
            url: $window.detailUrl,
            data: {
                ExportId: item.ExportId,
            }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.selectedExport = {
                    ExportId: data.ExportNote.ExportId,
                    NumberExport: data.ExportNote.NumberExport,
                    ExportDate: data.ExportNote.ExportDateDisplay,
                    ExportUser: data.ExportNote.ExportUserId,
                    ExportTo: data.ExportNote.ExportForUserId,
                    ExportReason: data.ExportNote.ExportReason,
                    TubeList: []
                };
                for (var i = 0; i < data.ExportNote.ExportNoteDetails.length;i++) {
                    $scope.selectedExport.TubeList.push(data.ExportNote.ExportNoteDetails[i]);
                }
                
                $('#dialogExportDetail').modal('show');
            }
        });
    };

    $scope.submitFindTubeList = function () {

        for (var i = 0; i < $scope.findTubeList.length; i++) {
            if ($scope.findTubeList[i].selected == true) {
                $scope.newExport.tubeList.push($scope.findTubeList[i]);
            }
        }
    };
    
    $scope.searchExport = function () {
                
        $http({
            method: 'POST',
            url: $window.searchUrl,
            data: {
                ExportId: $scope.exportSearchCriteria.exportId,
                StartDate: $scope.exportSearchCriteria.fromDate,
                EndDate: $scope.exportSearchCriteria.toDate
            }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.exportList = data.ListData;
            }
        });
    };

    $scope.searchExport();
};

$(document).ready(function () {
    
    $('#collapseSpecsContent_Search .input-group.date').datepicker({
        format: "dd/mm/yyyy",
        language: "vi",
        todayHighlight: true
    });

    $('#dialogExportDetail').modal({ keyboard: true, show: false, backdrop: 'static' });
    $('.list-group-item.export-list').toggleClass("active");
});
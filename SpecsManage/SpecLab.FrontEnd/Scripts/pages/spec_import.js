var myApp = angular.module('myApp', []);

var SpecsImportController = function SpecsImportController($scope, $http, $window) {

    $scope.importSearchCriteria = {
        fromDate: $window.defaultSearchValue.fromDate,
        toDate: $window.defaultSearchValue.toDate,
        importId: ""
    };

    $scope.importList = [
        //{ importId: 'EX.20140201.10', importDate: '11/02/2014 18:00:00', userFrom: 'phuongnh: Nguyen Ha Phuong', userTo: 'phuongnh: Nguyen Ha Phuong', numberImport: 5 },
    ];

    $scope.selectedImport = {
        importId: 'EX.20140201.10',
        numberImport: 5,
        importDate: '11/02/2014 18:00:00',
        importUser: 'phuongnh: Nguyen Ha Phuong',
        tubeList: [
            { specId: '20131230_001', tubeId: '20131230_001:1', storageId: 'TU:01_NGAN:01', locationNum: '1' },
		    { specId: '20131230_001', tubeId: '20131230_001:2', storageId: 'TU:01_NGAN:01', locationNum: '2' },
		    { specId: '20131230_001', tubeId: '20131230_001:3', storageId: 'TU:01_NGAN:01', locationNum: '3' }
        ]
    };

    $scope.getPrintUrl = function (importId) {
        return $window.printUrl + "/" + importId;
    };

    $scope.showImportDetail = function (item) {

        $http({
            method: 'POST',
            url: $window.detailUrl,
            data: {
                ImportId: item.ImportId,
            }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.selectedImport = {
                    ImportId: data.ImportNote.ImportId,
                    NumberImport: data.ImportNote.NumberImport,
                    ImportDate: data.ImportNote.ImportDateDisplay,
                    ImportUser: data.ImportNote.ImportUserId,
                    TubeList: []
                };
                for (var i = 0; i < data.ImportNote.ImportNoteDetails.length; i++) {
                    $scope.selectedImport.TubeList.push(data.ImportNote.ImportNoteDetails[i]);
                }

                $('#dialogImportDetail').modal('show');
            }
        });
    };

    $scope.submitFindTubeList = function () {

        for (var i = 0; i < $scope.findTubeList.length; i++) {
            if ($scope.findTubeList[i].selected == true) {
                $scope.newImport.tubeList.push($scope.findTubeList[i]);
            }
        }
    };

    $scope.searchImport = function () {

        $http({
            method: 'POST',
            url: $window.searchUrl,
            data: {
                ImportId: $scope.importSearchCriteria.importId,
                StartDate: $scope.importSearchCriteria.fromDate,
                EndDate: $scope.importSearchCriteria.toDate
            }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.importList = data.ListData;
            }
        });
    };

    $scope.searchImport();
};

$(document).ready(function () {

    $('#collapseSpecsContent_Search .input-group.date').datepicker({
        format: "dd/mm/yyyy",
        language: "vi",
        todayHighlight: true
    });

    $('#dialogImportDetail').modal({ keyboard: true, show: false, backdrop: 'static' });
    $('.list-group-item.import-list').toggleClass("active");
});
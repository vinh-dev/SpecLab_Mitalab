var myApp = angular.module('myApp', []);

var SpecExportController = function SpecExportController($scope, $http, $window) {

    $scope.warningThreshold = $window.warningThreshold;

    //$scope.exportThreshold = {
    //    warningThreshold: $window.warningThreshold,
    //    isThresholdChecked: false,
    //    hasRightOverride: $window.hasRightOverride,
    //    tubeListCount: [
    //        //{TubeId: "", NumberExport: 0},
    //    ]
    //};

    $scope.userList = $window.userList;

    $scope.statusAllowList = $window.statusAllowList;

    $scope.getDefaultUserId = function () {

        if ($scope.userList.length > 0) {
            return $scope.userList[0].UserId
        }

        return "";
    }

    //$scope.newExport = {
    //    exportId: $window.exportId,
    //    exportDate: $window.exportDateDisplay,
    //    exportUser: $window.currentUser,
    //    exportToValue: $scope.getDefaultUserId(),
    //    exportReason: '',
    //    errorEmptyExportReason: false,
    //    errorEmptyExportToValue: false,
    //    warningExportOverThreshold: false,
    //    bussinessError: '',
    //    importSuccessful: false,
    //    tubeList: [
    //    ]
    //};
    $scope.newExport = {
        exportId: '',
        exportDate:'',
        exportUser: '',
        exportToValue: $scope.getDefaultUserId(),
        exportReason: '',
        errorEmptyExportReason: false,
        errorEmptyExportToValue: false,
        warningExportOverThreshold: false,
        bussinessError: '',
        importSuccessful: false,
        tubeList: [
        ]
    };


    $scope.refreshNewExport = function () {
        //$.ajax({
        //    url: $window.IncreateEX,
        //    type: 'GET',
        //    dataType:'Json',
        //    success: function (data) {
        //        $window.exportId = data.ExportId;
        //        $window.exportDateDisplay = data.ExportDateDisplay;
        //    }
        //});

        var onSuccess = function (data) {
            $scope.newExport = {
                exportId: data.ExportId,
                exportDate: data.ExportDateDisplay,
                exportUser: $window.currentUser,
                exportToValue: $scope.getDefaultUserId(),
                exportReason: '',
                errorEmptyExportReason: false,
                errorEmptyExportToValue: false,
                bussinessError: '',
                importSuccessful: false,
                tubeList: [
                ]
            };
        };
        var onError = function (err) {
            alert('Lỗi' + err);
        };
        $http.post($window.IncreateEX).success(onSuccess).error(onError);
        //$scope.exportThreshold.isThresholdChecked = false;
        $scope.exportThreshold.tubeListCount = [];

        $scope.resetError();
    };

    $scope.getPrintUrl = function (exportId) {
        return $window.printUrl + "/" + exportId;
    };

    $scope.findTubeCriteria = {
        //inputDateFrom: $window.fromDate,
        //inputDateTo: $window.toDate,
        specId: '',
        tubeId: '',
        storageId: '',
        locationNum: ''
    };

    $scope.findTubeList = [
        //{ selected: true, specId: '20131230_001', tubeId: '20131230_001:1', storageId: 'TU:01_NGAN:01', locationNum: '1' },
    ];

    $scope.showFindTubeToExport = function () {
        $scope.findTubeList = [];
        $('#dialogFindTubeToExport').modal('show');
    };

    $scope.submitFindTubeList = function () {

        for (var i = 0; i < $scope.findTubeList.length; i++) {
            if ($scope.findTubeList[i].selected == true) {

                var foundDuplicate = false;
                for (var j = 0; j < $scope.newExport.tubeList.length && foundDuplicate == false; j++) {
                    if ($scope.findTubeList[i].TubeId == $scope.newExport.tubeList[j].TubeId) {
                        foundDuplicate = true;
                    }
                }

                if (foundDuplicate == false) {
                    $scope.newExport.tubeList.push($scope.findTubeList[i]);
                }
            }
        }
        //$scope.exportThreshold.isThresholdChecked = false;
    };

    $scope.removeCreatedTubeItem = function (tube) {

        var addTubeList = [];
        for (var i = 0; i < $scope.newExport.tubeList.length; i++) {
            if ($scope.newExport.tubeList[i].TubeId != tube.TubeId) {
                addTubeList.push($scope.newExport.tubeList[i]);
            }
        }

        $scope.newExport.tubeList = addTubeList;
        //$scope.exportThreshold.isThresholdChecked = false;
    };

    $scope.resetError = function () {

        $scope.newExport.errorEmptyExportReason = false;
        $scope.newExport.errorEmptyExportToValue = false;
        $scope.newExport.bussinessError = '';
    };

    $scope.hasOverLimit = function () {
        for (var i = 0; i < $scope.newExport.tubeList.length; i++) {
            if ($scope.newExport.tubeList[i].NumberExport >= $scope.warningThreshold) {
                return true;
            }
        }

        return false;
    };

    //$scope.checkExportCount = function() {

    //    if ($scope.validateSubmitExport()) {

    //        var countTubeExportRequest = {
    //            ListTubeId: [],
    //        };

    //        for (var i = 0; i < $scope.newExport.tubeList.length; i++) {
    //            countTubeExportRequest.ListTubeId.push($scope.newExport.tubeList[i].TubeId);
    //        }

    //        $http({
    //            method: 'POST',
    //            url: $window.countTubeExport,
    //            data: countTubeExportRequest
    //        })
    //        .success(function(data) {

    //            if (data.ErrorDescription != null && data.ErrorDescription != "") {
    //                $scope.newExport.bussinessError = data.ErrorDescription;
    //            } else {
    //                $scope.exportThreshold.isThresholdChecked = true;
    //                $scope.exportThreshold.tubeListCount = data.CountDetails;

    //                // if there is no tube over limit, then going through
    //                // else stop there, require a second commit
    //                if(!$scope.hasOverLimit()) {
    //                    $scope.sendSubmitExport();
    //                }
    //            }
    //        });
    //    }
    //};

    $scope.validateSubmitExport = function () {

        $scope.resetError();

        if ($scope.newExport.exportReason == "") {
            $scope.newExport.errorEmptyExportReason = true;
            return false;
        }

        if ($scope.newExport.exportToValue == "") {
            $scope.newExport.errorEmptyExportToValue = true;
            return false;
        }

        return true;
    };

    $scope.sendSubmitExport = function () {

        if ($scope.validateSubmitExport()
            && $scope.canSubmitExport()) {

            var exportNoteRequest = {
                ExportId: $scope.newExport.exportId,
                ExportForUserId: $scope.newExport.exportToValue,
                ExportUserId: $scope.newExport.exportUser,
                ExportReason: $scope.newExport.exportReason,
                TubeExportIds: []
            };

            for (var i = 0; i < $scope.newExport.tubeList.length; i++) {
                exportNoteRequest.TubeExportIds.push($scope.newExport.tubeList[i].TubeId);
            }

            $http({
                method: 'POST',
                url: $window.createUrl,
                data: exportNoteRequest
            })
            .success(function (data) {

                if (data.ErrorDescription != null && data.ErrorDescription != "") {
                    $scope.newExport.bussinessError = data.ErrorDescription;
                } else {
                    $scope.newExport.importSuccessful = true;
                }
            });
        }
    };

    $scope.canSubmitExport = function () {

        if ($scope.newExport.tubeList.length > 0) {
            //if (!$scope.exportThreshold.isThresholdChecked) {
            //    return true;
            //} else
            if (!$scope.hasOverLimit()) {
                return true;
            } else if ($scope.hasOverLimit() && $scope.exportThreshold.hasRightOverride) {
                return true;
            }
        }

        return false;
    };

    $scope.submitExport = function () {

        //if (!$scope.exportThreshold.isThresholdChecked) {
        //    $scope.checkExportCount();
        //} else {
        $scope.sendSubmitExport();
        //}
    };

    $scope.hasSampleOutOfVolume = function () {

        for (var i = 0; i < $scope.newExport.tubeList.length; i++) {
            if ($scope.newExport.tubeList[i].Volume <= 0) {
                return true;
            }
        }

        return false;
    };

    $scope.showWarning = function () {

        if (//$scope.exportThreshold.isThresholdChecked == true &&
            $scope.hasOverLimit()
            && !$scope.newExport.importSuccessful) {

            return true;
        } else if ($scope.hasSampleOutOfVolume()
            && !$scope.newExport.importSuccessful) {

            return true;
        }

        return false;
    };

    $scope.searchTube = function () {
        $scope.findTubeCriteria.locationNum = $("#dialogFindTubeToExport_txtSearchLocationNum").val();
        $http({
            method: 'POST',
            url: $window.findTubeUrl,
            data: {
                SpecId: $scope.findTubeCriteria.specId,
                TubeId: $scope.findTubeCriteria.tubeId,
                StorageId: $scope.findTubeCriteria.storageId,
                LocationId: $scope.findTubeCriteria.locationNum,
                FilterStatus: $scope.statusAllowList
            }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.findTubeList = data.ListData;
                for (var i = 0; i < $scope.findTubeList.length; i++) {
                    $scope.findTubeList[i].selected = true;
                }
            }
        });
    };
    //Hiển thị lưới đựng mẫu để chọn
    $scope.ViewCheckLocationNum = function (findTubeCriteria) {
        var strStorageID = findTubeCriteria.storageId;
        var strlocationNum = findTubeCriteria.locationNum;
        $.ajax({
            url: '/StoragesControl/GetStorageInfo?storageid=' + strStorageID,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                var rows = '';
                var thead = '';
                if (data.ErrorDescription != null && data.ErrorDescription != "") {
                    alert(data.ErrorDescription);
                } else {
                    if (data.StorageInfos.length == 1) {
                        var _storageid = data.StorageInfos[0].StorageId;
                        var _row = data.StorageInfos[0].NumRows;
                        var _col = data.StorageInfos[0].NumColumn;
                        //Tao tieu de cột
                        thead += "<tr>";
                        thead += "<th class=\"col-md-1\"></th>";
                        for (var j = 1; j <= _col; j++) {
                            thead += "<th class=\"col-md-1\">" + j.toString() + "</th>";
                        }
                        thead += "</tr>";
                        var _stt = 1;
                        for (var i = 1; i <= _row; i++) {
                            rows += "<tr>";
                            var chr = (String.fromCharCode(96 + i)).toUpperCase();
                            for (var j = 1; j <= _col; j++) {
                                if (j == 1) {
                                    rows += "<td style=\"font-weight: bold;text-align: center;\"><p>" + chr + "<p></td>";
                                }
                                rows += "<td align=\"center\" id=\"_LocationNum" + _stt.toString() + "\" style=\"height:60px;\"></td>";
                                _stt++;
                            }
                            rows += "</tr>";
                        }
                        $("#theadstorage").html(thead);
                        $("#tbodystorage").html(rows);
                        $("#dialogSelectLocationNum").modal('show');
                        _stt = 1;
                        for (var i = 1; i <= _row; i++) {
                            var chr = (String.fromCharCode(96 + i)).toUpperCase();
                            for (var j = 1; j <= _col; j++) {
                                $scope.ViewTubeNum(_storageid, _stt, chr, j)
                                _stt++;
                            }
                        }
                    }
                    else {
                        alert("Mã hộp lưu trữ không tồn tại !");
                    }
                }
            },
            error: function (err) {
                alert("Error: " + err.responseText);
                return;
            }
        });
    };
    //Tim kiếm trạng vị trí mẫu
    $scope.ViewTubeNum = function (_storeid, _locationNum, _colunm, _row) {
        $http({
            method: 'POST',
            url: $window.findTubeUrl,
            data: {
                StorageId: _storeid,
                LocationId: _locationNum,
            }
        })
        .success(function (data) {
            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
                return null;
            } else {
                if (data.ListData.length > 0) {
                    //if (data.ListData[0].Condition == 0) {
                    //    $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle0\"><button type=\"button\" data-dismiss=\"modal\"  id=\"btnSelectSamlpe_Export0\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    //}
                    if (data.ListData[0].Condition == 1) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle1\"><button type=\"button\" data-dismiss=\"modal\"  id=\"btnSelectSamlpe_Export1\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                    if (data.ListData[0].Condition == 2) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle2\"><p class=\"centertext_in\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</p></div>");
                    }
                    if (data.ListData[0].Condition == 3) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle3\"><button type=\"button\" data-dismiss=\"modal\"  id=\"btnSelectSamlpe_Export3\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                    if (data.ListData[0].Condition == 4) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle4\"><button type=\"button\" data-dismiss=\"modal\"  id=\"btnSelectSamlpe_Export4\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                    if (data.ListData[0].Condition == 5) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle5\"><button type=\"button\" data-dismiss=\"modal\" id=\"btnSelectSamlpe_Export5\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                    if (data.ListData[0].Condition == 6) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle6\"><button type=\"button\" data-dismiss=\"modal\" id=\"btnSelectSamlpe_Export6\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                }
                else {
                    $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle2\"><p class=\"centertext_in\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "<p></div>");
                }
            }
        });
    };
};

$(document).ready(function () {

    $('#collapseSpecsContent_Search .input-group.date').datepicker({
        format: "dd/mm/yyyy",
        language: "vi",
        todayHighlight: true
    });

    $('#dialogFindTubeToExport .input-group.date').datepicker({
        format: "dd/mm/yyyy",
        language: "vi",
        todayHighlight: true
    });

    $('#dialogFindTubeToExport').modal({ keyboard: true, show: false, backdrop: 'static' });
    $("#dialogFindTubeToExport_txtSearchLocationNum").numeric({ decimal: false, negative: false });
    $('.list-group-item.export-test').toggleClass("active");
});
function SetValuesLocationNum_V2(_value) {
    $("#dialogFindTubeToExport_txtSearchLocationNum").focus();
    $("#dialogFindTubeToExport_txtSearchLocationNum").select();
    $("#dialogFindTubeToExport_txtSearchLocationNum").val(_value);
    $('#dialogFindTubeToExport').modal('show');
}
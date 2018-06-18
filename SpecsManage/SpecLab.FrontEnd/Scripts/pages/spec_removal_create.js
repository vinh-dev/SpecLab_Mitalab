var myApp = angular.module('myApp', []);

var SpecRemovalController = function SpecRemovalController($scope, $http, $window) {

    $scope.statusAllowList = $window.statusAllowList;

    $scope.newRemoval = {
        removalId: '',
        removalDate: '',
        removalUser: '',
        removalReason: '',
        errorReasonEmpty: false,
        bussinessError: '',
        importSuccessful: false,
        tubeList: [
        ]
    };


    $scope.refreshNewRemoval = function () {
        var onSuccess = function (data) {
            $scope.newRemoval = {
                removalId: data.RemovalId,
                removalDate: data.RemovalDateDisplay,
                removalUser: $window.currentUser,
                removalReason: '',
                errorReasonEmpty: false,
                bussinessError: '',
                importSuccessful: false,
                tubeList: [
                ]
            };
        };
        var onError = function (err) {
            alert('Lỗi' + err);
        };
        $http.post($window.IncreateRE).success(onSuccess).error(onError);
    };
    $scope.getPrintUrl = function (removalId) {
        return $window.printUrl + "/" + removalId;
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

    $scope.showFindTubeToRemoval = function () {
        $scope.findTubeList = [];
        $('#dialogFindTubeToRemoval').modal('show');
    };

    $scope.submitFindTubeList = function () {

        for (var i = 0; i < $scope.findTubeList.length; i++) {
            if ($scope.findTubeList[i].selected == true) {

                var foundDuplicate = false;
                for (var j = 0; j < $scope.newRemoval.tubeList.length && foundDuplicate == false; j++) {
                    if ($scope.findTubeList[i].TubeId == $scope.newRemoval.tubeList[j].TubeId) {
                        foundDuplicate = true;
                    }
                }

                if (foundDuplicate == false) {
                    $scope.newRemoval.tubeList.push($scope.findTubeList[i]);
                }
            }
        }
    };

    $scope.removeCreatedTubeItem = function (tube) {

        var addTubeList = [];
        for (var i = 0; i < $scope.newRemoval.tubeList.length; i++) {
            if ($scope.newRemoval.tubeList[i].TubeId != tube.TubeId) {
                addTubeList.push($scope.newRemoval.tubeList[i]);
            }
        }

        $scope.newRemoval.tubeList = addTubeList;
    };

    $scope.submitRemoval = function () {

        $scope.newRemoval.errorReasonEmpty = false;
        $scope.newRemoval.bussinessError = '';
        
        if ($scope.newRemoval.removalReason == "") {
            $scope.newRemoval.errorReasonEmpty = true;
        } else {
            var removalNoteRequest = {
                RemovalId: $scope.newRemoval.removalId,
                RemovalReason: $scope.newRemoval.removalReason,
                RemovalUserId: $scope.newRemoval.removalUser,
                TubeRemovalIds: []
            };

            for (var i = 0; i < $scope.newRemoval.tubeList.length; i++) {
                removalNoteRequest.TubeRemovalIds.push($scope.newRemoval.tubeList[i].TubeId);
            }



            $http({
                method: 'POST',
                url: $window.createUrl,
                data: removalNoteRequest
            })
            .success(function (data) {

                if (data.ErrorDescription != null && data.ErrorDescription != "") {
                    $scope.newRemoval.bussinessError = data.ErrorDescription;
                } else {

                    $scope.newRemoval.importSuccessful = true;
                }
            });
        }
    };

    $scope.searchTube = function () {
        $scope.findTubeCriteria.locationNum = $("#dialogFindTubeToRemoval_txtSearchLocationNum").val();
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
                    if (data.ListData[0].Condition == 0) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle0\"><button type=\"button\" data-dismiss=\"modal\"  id=\"btnSelectSamlpe_Export0\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
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
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle5\"><button type=\"button\" data-dismiss=\"modal\"  id=\"btnSelectSamlpe_Export5\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
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

    $('#dialogFindTubeToRemoval .input-group.date').datepicker({
        format: "dd/mm/yyyy",
        language: "vi",
        todayHighlight: true
    });

    $('#dialogFindTubeToRemoval').modal({ keyboard: true, show: false, backdrop: 'static' });
    $("#dialogFindTubeToRemoval_txtSearchLocationNum").numeric({ decimal: false, negative: false });
    $('.list-group-item.remove-test').toggleClass("active");
});
function SetValuesLocationNum_V2(_value) {
    $("#dialogFindTubeToRemoval_txtSearchLocationNum").focus();
    $("#dialogFindTubeToRemoval_txtSearchLocationNum").select();
    $("#dialogFindTubeToRemoval_txtSearchLocationNum").val(_value);
    $('#dialogFindTubeToRemoval').modal('show');
}
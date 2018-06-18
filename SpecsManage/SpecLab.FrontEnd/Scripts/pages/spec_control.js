var myApp = angular.module('myApp', []);

var SpecsController = function SpecsController($scope, $http, $window) {
    
    $scope.hasOverrideRight = $window.hasOverrideRight;
        
    $scope.statusList = $window.statusList;
    
    $scope.specsList = [];

    $scope.selectedSpec = {
        
    };

    $scope.updateTube = {
        
    };

    $scope.selectedHistoryList = [];

    $scope.searchCriteria = {
        fromDate: $window.defaultSearchValue.fromDate,
        toDate: $window.defaultSearchValue.toDate,
        specId: $window.defaultSearchValue.specId,
        tubeId: $window.defaultSearchValue.tubeId,
        storageId: '',
        locationId: $window.defaultSearchValue.locationId
    };

    $scope.isNoData = function () {

        if ($scope.specsList.length == 0) {
            return true;
        }

        return false;
    };

    $scope.showUpdate = function (spec) {

        $scope.updateTube.SpecId = spec.SpecId;
        $scope.updateTube.TubeId = spec.TubeId;
        $scope.updateTube.Volume = spec.VolumeDisplay;
        $scope.updateTube.OldVolume = spec.VolumeDisplay;
        $scope.updateTube.StorageId = spec.StorageId;
        $scope.updateTube.LocationNum = spec.LocationNum;
        $scope.updateTube.Status = spec.Status;
        $scope.updateTube.TubeType = spec.TubeType;
        $scope.updateTube.UpdateSuccessful = false;
        $scope.updateTube.ErrorLocationZero = false;
        $scope.updateTube.ErrorLocationOutbound = false;
        $scope.updateTube.BussinessError = "";

        $('#dialogUpdateTube').modal('show');
    };

    $scope.submitUpdateTube = function () {
        $scope.updateTube.LocationNum = $("#dialogUpdateTube_txtLocationNum").val();
        if ($scope.validateUpdateSample()) {
            
            $scope.updateTube.ErrorLocationZero = false;
            $scope.updateTube.ErrorLocationOutbound = false;
            $scope.updateTube.BussinessError = "";

            // kiem tra vi tri co valid 0
            if ($scope.updateTube.LocationNum <= 0) {
                $scope.updateTube.ErrorLocationZero = true;
            } else {

                //khong co loi, save xuong db
                if ($scope.updateTube.ErrorLocationOutbound == false) {
                    
                    if ($scope.updateTube.Volume <= 0) {
                        if(!confirm("Xác nhận cập nhật thể tích mẫu đã hết.")) {
                            return;
                        }
                    }
                    
                    $http({
                        method: 'POST',
                        url: $window.updateUrl,
                        data: {
                            TubeId: $scope.updateTube.TubeId,
                            LocationNum: $scope.updateTube.LocationNum,
                            StorageId: $scope.updateTube.StorageId,
                            Volume: $scope.updateTube.Volume,
                            Status: $scope.updateTube.Status
                        }
                    })
                    .success(function (data) {

                        if (data.ErrorDescription != null && data.ErrorDescription != "") {
                            $scope.updateTube.BussinessError = data.ErrorDescription;
                        } else {

                            $scope.updateTube.UpdateSuccessful = true;

                            // refresh list
                            $scope.searchSpec();
                        }
                    });
                }
            }
        }
    };

    $scope.showHistory = function (spec) {

        $scope.selectedSpec = spec;
        
        $http({
            method: 'POST',
            url: $window.historyUrl,
            data: {
                TubeId: spec.TubeId
            }
        })
        .success(function (data) {
            
            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.selectedHistoryList = data.HistoryInfos;
                
                $('#dialogHistorySpecs').modal('show');
            }
        });
    };

    $scope.searchSpec = function () {
        $scope.searchCriteria.locationId = $("#txtSearchLocationNum").val();
        $http({ 
            method: 'POST', 
            url: $window.searchUrl, 
            data: $scope.searchCriteria
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.specsList = data.ListData;
            }
        });
    };
    
    $scope.validateUpdateSample = function () {

        if (hasOverrideRight) {
            return true;
        }
        
        if($scope.updateTube.Volume > $scope.updateTube.OldVolume) {
            return false;
        }

        return true;
    };

    $scope.canUpdateSample = function (spec) {
        if (spec.Status != 2) { // status = Remove
            return true;
        }
        
        return false;
    };
    
    $scope.canUpdateSampleRemoved = function (spec) {
        if (spec.Status == 2) { // status = Remove
            return true;
        }

        return false;
    };

    $scope.listStatusUpdatable = function(spec) {

        var listStatus = [];

        //Good = 0,
        //InUse = 1,
        //Remove = 2,
        //Corrupt = 3

        for (var i = 0; i < $scope.statusList.length; i++) {
            if (spec.Status == $scope.statusList[i].Status) {
                listStatus.push({ Status: $scope.statusList[i].Status, Description: $scope.statusList[i].Description });
            }
        }
        
        for (var i = 0; i < $scope.statusList.length; i++) {
            if (spec.Status != $scope.statusList[i].Status
                && ($scope.statusList[i].Status == 0 || $scope.statusList[i].Status == 3)) {
                listStatus.push({ Status: $scope.statusList[i].Status, Description: $scope.statusList[i].Description });
            }
        }

        return listStatus;
    };
    
    $scope.searchSpec();
    //Hiển thị lưới đựng mẫu để chọn
    $scope.ViewCheckLocationNum = function (searchCriteria) {
        var strStorageID = searchCriteria.storageId;
        var strlocationNum = searchCriteria.locationNum;
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
            url: $window.searchUrl,
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
                    //    $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle0\"><button type=\"button\" data-dismiss=\"modal\" id=\"btnSelectSamlpe_Export0\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    //}
                    if (data.ListData[0].Condition == 1) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle1\"><button type=\"button\" data-dismiss=\"modal\"  id=\"btnSelectSamlpe_Export1\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                    if (data.ListData[0].Condition == 2) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle2\"><button type=\"button\" data-dismiss=\"modal\" id=\"btnSelectSamlpe_Export2\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div>");
                    }
                    if (data.ListData[0].Condition == 3) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle3\"><button type=\"button\" data-dismiss=\"modal\" id=\"btnSelectSamlpe_Export3\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
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
                    $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle2\"><button type=\"button\" data-dismiss=\"modal\" id=\"btnSelectSamlpe_Export2\" onclick=\"return SetValuesLocationNum_V2('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div>");
                }
            }
        });
    };
    //Hiển thị lưới đựng mẫu để chọn
    $scope.ViewCheckLocationNumChangeStorage = function (updateTube) {
        var strStorageID = updateTube.StorageId;
        var strlocationNum = updateTube.LocationNum;
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
                                $scope.ViewTubeNumChangeStorage(_storageid, _stt, chr, j)
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
    $scope.ViewTubeNumChangeStorage = function (_storeid, _locationNum, _colunm, _row) {
        $http({
            method: 'POST',
            url: $window.searchUrl,
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
                    //    $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle0\"><p class=\"centertext_in\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</p></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    //}
                    if (data.ListData[0].Condition == 1) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle1\"><p class=\"centertext_in\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</p></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                    if (data.ListData[0].Condition == 2) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle2\"><button type=\"button\" data-dismiss=\"modal\" class=\"btn btn-info\" id=\"btnSelectSamlpe_Export2\" onclick=\"return SetValuesLocationNumChangeStorage('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div>");
                    }
                    if (data.ListData[0].Condition == 3) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle3\"><p class=\"centertext_in\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</p></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                    if (data.ListData[0].Condition == 4) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle4\"><p class=\"centertext_in\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</p></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                    if (data.ListData[0].Condition == 5) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle5\"><p class=\"centertext_in\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</p></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                    if (data.ListData[0].Condition == 6) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle6\"><p class=\"centertext_in\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</p></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                }
                else {
                    $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle2\"><button type=\"button\" data-dismiss=\"modal\" class=\"btn btn-info\" id=\"btnSelectSamlpe_Export2\" onclick=\"return SetValuesLocationNumChangeStorage('" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div>");
                }
            }
        });
    };
};

$(document).ready(function () {
    $('#collapseSpecsContent .input-group.date').datepicker({
        format: "dd/mm/yyyy",
        language: "vi",
        todayHighlight: true
    });
   
    $("#dialogUpdateTube_txtLocationNum").numeric({ decimal: false, negative: false });
    $("#dialogUpdateTube_txtVolume").numeric({ decimal: false, negative: false });
    $('#dialogUpdateTube').modal({ keyboard: true, show: false, backdrop: 'static' }).on('shown.bs.modal', function (e) {
        $('#dialogUpdateTube_txtVolume').focus();
        $('#dialogUpdateTube_txtVolume').select();
    });
    $('#dialogHistorySpecs').modal({ keyboard: true, show: false, backdrop: 'static' });
    $('.list-group-item.test-control').toggleClass("active");
});
function SetValuesLocationNum_V2(_value) {
    $("#txtSearchLocationNum").focus();
    $("#txtSearchLocationNum").select();
    $("#txtSearchLocationNum").val(_value);
}
function SetValuesLocationNumChangeStorage(_value) {
    $("#dialogUpdateTube_txtLocationNum").focus();
    $("#dialogUpdateTube_txtLocationNum").select();
    $("#dialogUpdateTube_txtLocationNum").val(_value);
    $('#dialogUpdateTube').modal('show');
}
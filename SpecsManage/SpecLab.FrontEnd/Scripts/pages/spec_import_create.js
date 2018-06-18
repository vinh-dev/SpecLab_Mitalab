var myApp = angular.module('myApp', []).directive('tubeListChange', function () {
    return {
        link: function (scope, element, attrs, ctrl) {
            $(element[0]).find('.volume_input, .location_input').numeric({ decimal: false, negative: false });
        }
    };
});

var SpecsImportController = function SpecsImportController($scope, $http, $window) {

    $scope.sexSelectList = $window.sexSelectList;
    $scope.sampleTypeList = $window.sampleTypeList;

    $scope.specImport = {
        specId: '',
        patientName: '',
        sex: '',
        yearOfBirth: '',
        source: '',
        sampleType: '',
        tubeNumber: 0,
        dateInput: $window.currentInput,

        importSuccessful: false
    };

    $scope.tubeList = [
        //{ tubeNum: 1, tubeId: '', volume: 0, storageId: 'TU:01_NGAN:01', locationNum: '', sampleType: 1}
    ];

    $scope.labconnFind = {
        sid: '', patientName: '', dateInput: $window.todaySearch, sequence: ''
    };

    $scope.labconnFindResult = [
        //{ sid: '051213-13M00930', patientName: 'NGUYEN THI HOA', sex: 'Nữ', yearOfBirth: '1987', dateInput: '05/12/2013', locationId: '' },
    ];

    $scope.resetError = function () {

        $scope.specImport.errorDuplicateLocation = false;
        $scope.specImport.errorLocationZero = false;
        $scope.specImport.errorNoTube = false;
        $scope.specImport.errorLocationOutbound = false;
        $scope.specImport.errorImportTubeEmptyVolume = false;
        $scope.specImport.bussinessError = '';
        
        for(var i=0;i<$scope.tubeList.length;i++) {
            $scope.tubeList[i].errorDuplicateLocation = false;
            $scope.tubeList[i].errorLocationZero = false;
            $scope.tubeList[i].errorNoTube = false;
            $scope.tubeList[i].errorLocationOutbound = false;
            $scope.tubeList[i].errorImportTubeEmptyVolume = false;
        }
    };

    $scope.newImport = function () {

        $scope.specImport = {
            specId: '',
            patientName: '',
            sex: '',
            yearOfBirth: '',
            source: '',
            sampleType: '',
            tubeNumber: 0,
            dateInput: $window.currentInput,

            importSuccessful: false
        };

        $scope.resetError();
        $scope.tubeList = [];
    };

    $scope.getPrintUrl = function (specId) {
        //return $window.printUrl + "?specId=" + specId;
        return $window.printUrl + "/" + specId;
    };

    $scope.isNoData = function () {

        if ($scope.specsList.length == 0) {
            return true;
        }

        return false;
    };

    $scope.changeNumOfTube = function () {

        if ($scope.specImport.tubeNumber == "") {
            $scope.specImport.tubeNumber = 0;
        }

        if ($scope.specImport.tubeNumber < $scope.tubeList.length) {

            var tempList = $scope.tubeList;
            // remove all
            $scope.tubeList = [];
            for (var i = 0; i < $scope.specImport.tubeNumber; i++) {
                $scope.tubeList.push(tempList[i]);
            }
        }
        else if ($scope.specImport.tubeNumber > $scope.tubeList.length) {

            var tubeLength = $scope.tubeList.length;
            var numberAdd = $scope.specImport.tubeNumber - $scope.tubeList.length;

            for (var i = 0; i < numberAdd; i++) {
                $scope.tubeList.push({
                    tubeNum: (i + tubeLength + 1),
                    tubeId: $scope.specImport.specId + ':' + (i + tubeLength + 1),
                    volume: 0,
                    storageId: '',
                    sampleType: $scope.sampleTypeList[0].Code,
                    locationNum: 0
                });
            }
        }

        $scope.$broadcast('dataloaded');
    };

    $scope.searchLabconn = function () {

        $http({
            method: 'POST',
            url: $window.searchLabconnUrl,
            data: {
                SID: $scope.labconnFind.sid,
                PatientName: $scope.labconnFind.patientName,
                DateSearch: $scope.labconnFind.dateInput,
                Sequence: $scope.labconnFind.sequence
            }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.labconnFindResult = [];

                for (var i = 0; i < data.SampleInfos.length; i++) {
                    $scope.labconnFindResult.push({
                        sid: data.SampleInfos[i].SID,
                        patientName: data.SampleInfos[i].Patientname,
                        sex: data.SampleInfos[i].SexDisplay,
                        sexValue: data.SampleInfos[i].Sex,
                        yearOfBirth: data.SampleInfos[i].Age,
                        dateInput: data.SampleInfos[i].DateInputDisplay,
                        locationId: data.SampleInfos[i].LocationId
                    });
                }
            }
        });
    };

    $scope.labconnChoose = function (labItem) {

        $scope.specImport.specId = labItem.sid;
        $scope.specImport.patientName = labItem.patientName;
        $scope.specImport.sex = labItem.sexValue;
        $scope.specImport.yearOfBirth = labItem.yearOfBirth;
        $scope.specImport.source = labItem.locationId;

        $scope.refreshTubeId();
    };

    $scope.showLabconnFind = function (spec) {
        //$scope.searchLabconn();
        $('#dialogLabconnFind').modal('show');
    };

    $scope.saveTubeList = function () {

        $scope.resetError();

        if ($scope.specImport.tubeNumber == ''
            || $scope.specImport.tubeNumber == 0
            || $scope.tubeList.length == 0) {
            $scope.specImport.errorNoTube = true;
        }

        for (var i = 0; i < $scope.tubeList.length; i++) {
            $scope.tubeList[i].locationNum=$("#tubeInfo_" + $scope.tubeList[i].tubeNum.toString() + "_txtLocationNum").val();
            if ($scope.tubeList[i].locationNum == ''
                || $scope.tubeList[i].locationNum <= 0) {

                $scope.specImport.errorLocationZero = true;
                $scope.tubeList[i].errorLocationZero = true;
            }
            
            if ($scope.tubeList[i].volume == ''
                || $scope.tubeList[i].volume <= 0) {
                
                $scope.specImport.errorImportTubeEmptyVolume = true;
                $scope.tubeList[i].errorImportTubeEmptyVolume = true;
            }
        }

        for (var i = 0; i < $scope.tubeList.length - 1; i++) {
            for (var j = i + 1; j < $scope.tubeList.length; j++) {

                if ($scope.tubeList[i].storageId == $scope.tubeList[j].storageId
                    && $scope.tubeList[i].locationNum == $scope.tubeList[j].locationNum) {

                    $scope.specImport.errorLocationZero = true;
                    $scope.tubeList[i].errorDuplicateLocation = true;
                    $scope.tubeList[j].errorDuplicateLocation = true;
                }
            }
        }

        if (!$scope.specImport.errorDuplicateLocation
            && !$scope.specImport.errorLocationZero
            && !$scope.specImport.errorLocationOutbound
            && !$scope.specImport.errorNoTube
            && !$scope.specImport.errorImportTubeEmptyVolume
            && !$scope.specImport.errorInvalidLocationNum) {

            var dataSubmit = {
                SampleSpecId: $scope.specImport.specId,
                PatientName: $scope.specImport.patientName,
                Sex: $scope.specImport.sex,
                YearOfBirth: $scope.specImport.yearOfBirth,
                LocationId: $scope.specImport.source,
                SampleType: $scope.specImport.sampleType,
                TubeSampleSpecs: [
                ]
            };

            for (var k = 0; k < $scope.tubeList.length; k++) {
                dataSubmit.TubeSampleSpecs.push({
                    SampleSpecId: $scope.specImport.specId,
                    Status: 0,
                    Volume: $scope.tubeList[k].volume,
                    TubeId: $scope.tubeList[k].tubeId,
                    StorageId: $scope.tubeList[k].storageId,
                    LocationNum: $scope.tubeList[k].locationNum,
                    SampleType: $scope.tubeList[k].sampleType
                });
            }

            if (confirm("Vui lòng xác nhận nhập Mã bệnh phẩm: " + $scope.specImport.specId)) {
                
                $http({
                    method: 'POST',
                    url: $window.importUrl,
                    data: dataSubmit
                })
                .success(function (data) {

                    if (data.ErrorDescription != null && data.ErrorDescription != "") {

                        $scope.specImport.bussinessError = data.ErrorDescription;
                    } else {

                        $scope.specImport.importSuccessful = true;
                    }
                });
            }
        }
    };

    $scope.refreshTubeId = function () {
        for (var i = 0; i < $scope.tubeList.length; i++) {
            $scope.tubeList[i].tubeId = $scope.specImport.specId + ':' + (i + 1);
        }
    };
    //Hiển thị lưới đựng mẫu để chọn
    $scope.ViewCheckLocationNum = function (tube) {
        var strStorageID = tube.storageId;
        var strlocationNum = tube.locationNum;
        var strIndexSample = tube.tubeNum;
        
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
                                $scope.searchTube(strIndexSample, _storageid, _stt, chr, j)
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
    $scope.SetValuesLocationNum = function (tube, _value) {
        tube.locationNum = _value;
    }
    $scope.resetError();
    //Kiểm tra xem vị trí có được sử dụng chưa
    $scope.searchTube = function (strIndexSample, _storeid, _locationNum, _colunm, _row) {
        var strIndexSample1 = strIndexSample;
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
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle0\"><p class=\"centertext_in\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</p></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                    if (data.ListData[0].Condition == 1) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle1\"><p class=\"centertext_in\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</p></div><br><p class=\"centertext_out\">" + data.ListData[0].TubeId + "</p>");
                    }
                    if (data.ListData[0].Condition == 2) {
                        $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle2\"><button type=\"button\" data-dismiss=\"modal\" id=\"btnSelectSamlpe_Export2\" onclick=\"return SetValuesLocationNum_V2('" + strIndexSample1 + "','" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div>");
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
                }
                else {
                    $("#_LocationNum" + _locationNum.toString()).html("<div class=\"circle2\"><button type=\"button\" data-dismiss=\"modal\" id=\"btnSelectSamlpe_Export2\" onclick=\"return SetValuesLocationNum_V2('" + strIndexSample1 + "','" + _locationNum.toString() + "')\">" + _colunm + "-" + _row.toString() + "<br>" + _locationNum.toString() + "</button></div>");
                }
            }
        });
    };
};

$(document).ready(function () {
    
    $("#txtYearOfBirth").numeric({ decimal: false, negative: false });
    $("#txtNumTube").numeric({ decimal: false, negative: false });
    $('#dialogLabconnFind').modal({ keyboard: true, show: false, backdrop: 'static' }).on('shown.bs.modal', function (e) {
        $('#dialogLabconnFind_txtSequence').focus();
        $('#dialogLabconnFind_txtSequence').select();
    });
    
    $('#dialogLabconnFind .input-group.date').datepicker({
        format: "dd/mm/yyyy",
        language: "vi",
        todayHighlight: true
    });

    $('.list-group-item.import-test').toggleClass("active");
});
function SetValuesLocationNum_V2(tobeNumIndex, _value) {
    $("#tubeInfo_" + tobeNumIndex + "_txtLocationNum").focus();
    $("#tubeInfo_" + tobeNumIndex + "_txtLocationNum").select();
    $("#tubeInfo_" + tobeNumIndex + "_txtLocationNum").val(_value);
}
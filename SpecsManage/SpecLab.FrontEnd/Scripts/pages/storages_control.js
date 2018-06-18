var myApp = angular.module('myApp', ['ui.bootstrap']).directive('storageListChange', function () {
    return {
        link: function (scope, element, attrs, ctrl) {
            $(element[0]).find('.txtEditLocationNum').numeric({ decimal: false, negative: false });
        }
    };
});

var StorageControlController = function StorageControlController($scope, $http, $window) {

    $scope.storageList = [];
    $scope.storagefilter = $window.storagefilter;

    $scope.findStorageIndex = function (storage) {

        for (var i = 0; i < $scope.storageList.length; i++) {
            if ($scope.storageList[i] == storage) {
                return i;
            }
        }

        return -1;
    };

    $scope.removeLocalList = function (user) {
        var tempList = [];
        for (var i = 0; i < $scope.storageList.length; i++) {
            if ($scope.storageList[i] != user) {
                tempList.push($scope.storageList[i]);
            }
        }

        $scope.storageList = tempList;
    };

    $scope.editStorage = function (storage) {
        storage.editing = true;

        var index = $scope.findStorageIndex(storage);
        if (index >= 0) {
            $("#txtEditLocationNum_" + index).focus();
            $("#txtEditLocationNum_" + index).select();
        }
    };

    $scope.removeStorage = function (storage) {

        if (confirm("Vui lòng xác nhận xóa mã lưu trữ:" + storage.storageId + " !!!")) {
            $http({
                method: 'POST',
                url: $window.deleteUrl,
                data: {
                    StorageId: storage.storageId,
                    NumberStorage: storage.locationNum,
                    NumRows: storage.numRows,
                    NumColumn: storage.numColumn
                }
            })
            .success(function (data) {

                if (data.ErrorDescription != null && data.ErrorDescription != "") {
                    alert(data.ErrorDescription);
                } else {

                    $scope.removeLocalList(storage);
                }
            });
        }
    };

    $scope.cancelEditStorage = function (storage) {

        if (storage.isNew == false) {
            storage.editLocationNum = storage.locationNum;
            storage.editNumRows = storage.numRows;
            storage.editNumColumn = storage.numColumn;
            storage.editing = false;
        } else {
            $scope.removeLocalList(storage);
        }
    };

    $scope.saveStorage = function (storage) {
        if (storage.storageId == "") {
            storage.storageIdError = true;
        }
        else
            if ((storage.editNumRows * storage.editNumColumn) != storage.editLocationNum) {
                alert("Số vị trí so với số hàng và cột không khớp !");
            }
            else {

                var numStorageIdDuplicate = 0;
                for (var i = 0; i < $scope.storageList.length; i++) {
                    if ($scope.storageList[i].storageId.toLowerCase() == storage.storageId.toLowerCase()) {
                        numStorageIdDuplicate++;
                    }
                }

                if (numStorageIdDuplicate < 2) {

                    if (!storage.isNew) {
                        $http({
                            method: 'POST',
                            url: $window.updateUrl,
                            data: {
                                StorageId: storage.storageId,
                                NumberStorage: storage.editLocationNum,
                                NumRows: storage.editNumRows,
                                NumColumn: storage.editNumColumn
                            }
                        })
                        .success(function (data) {

                            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                                alert(data.ErrorDescription);
                            } else {
                                storage.storageIdError = false;
                                storage.locationNum = storage.editLocationNum;
                                storage.numRows = storage.editNumRows;
                                storage.numColumn = storage.editNumColumn;
                                storage.isNew = false;
                                storage.editing = false;
                            }
                        });
                    } else {
                        $http({
                            method: 'POST',
                            url: $window.addUrl,
                            data: {
                                StorageId: storage.storageId,
                                NumberStorage: storage.editLocationNum,
                                NumRows: storage.editNumRows,
                                NumColumn: storage.editNumColumn
                            }
                        })
                        .success(function (data) {

                            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                                alert(data.ErrorDescription);
                            } else {
                                storage.storageIdError = false;
                                storage.locationNum = storage.editLocationNum;
                                storage.numRows = storage.editNumRows;
                                storage.numColumn = storage.editNumColumn;
                                storage.isNew = false;
                                storage.editing = false;
                            }
                        });
                    }
                } else {
                    storage.storageIdError = true;
                }
            }
    };

    $scope.addNewStorage = function () {
        $scope.storageList.push({
            storageId: '',
            locationNum: 100,
            editLocationNum: 100,
            editNumRows: 10,
            editNumColumn: 10,
            editing: true,
            isNew: true,
            storageIdError: false
        });
    };



    $scope.refreshStorageList = function () {

        var _storagefilter = $scope.storagefilter;
        $http({
            method: 'POST',
            url: $window.listUrl,
            data: { storagefilter: _storagefilter }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            }
            else {
                var lengthlist = 10;
                if (data.StorageInfos.length < 10) {
                    lengthlist = data.StorageInfos.length;
                }
                $scope.rowdataview = lengthlist.toString() + "/" + data.StorageInfos.length.toString();

                //paging
                $scope.storageListAll = data.StorageInfos;

                $scope.totalItems = $scope.storageListAll.length;
                $scope.itemsPerPage = 10;
                $scope.currentPage = 1;
                $scope.maxSize = 3;

                $scope.pageCount = function () {
                    return Math.ceil($scope.storageListAll.length / $scope.itemsPerPage);
                };

                $scope.$watch('currentPage + itemsPerPage', function () {
                    var begin = (($scope.currentPage - 1) * $scope.itemsPerPage),
                        end = begin + $scope.itemsPerPage;
                    $scope.storageList = [];
                    for (var i = begin; i < end; i++) {
                        if ($scope.storageListAll[i] != null) {
                            $scope.storageList.push({
                                STT:i+1,
                                storageId: $scope.storageListAll[i].StorageId,
                                locationNum: $scope.storageListAll[i].NumberStorage,
                                editLocationNum: $scope.storageListAll[i].NumberStorage,
                                numRows: $scope.storageListAll[i].NumRows,
                                editNumRows: $scope.storageListAll[i].NumRows,
                                numColumn: $scope.storageListAll[i].NumColumn,
                                editNumColumn: $scope.storageListAll[i].NumColumn,
                                editing: false,
                                isNew: false,
                                storageIdError: false
                            });
                        } else { break;}
                    }
                    //$scope.storageList = $scope.storageListAll.slice(begin, end); // cat du lieu 
                });
                // end paging
            }
        });
       
      


    };

    $scope.refreshStorageList();

  
};

$(document).ready(function () {
    $('#sidebar .list-group a.list-group-item.test-storage').toggleClass("active");
});
var myApp = angular.module('myApp', []);

var UserControlController = function UserControlController($scope, $http, $window) {

    $scope.userList = [
        //{ userId: 'phuongnh', userName: 'Nguyễn Hà Phương', editUserName: 'Nguyễn Hà Phương', editing: false, isNew: false, userIdError: false },
    ];

    $scope.rightList = [
        //{ rightId: 'RIGHT001', rightNameDisplay: 'Đăng nhập', selected: false },
    ];

    $scope.changePass = {
        userId: '',
        newPass: '', 
        confirmPass: '',
        successful: false
    };

    $scope.selectedUser = null;

    $scope.findUserIndex = function(user) {
         
        for (var i = 0; i < $scope.userList.length; i++) {
            if ($scope.userList[i] == user) {
                return i;
            }
        }

        return -1;
    };

    $scope.editUser = function (user) {
        user.editing = true;

        var index = $scope.findUserIndex(user);
        if (index >= 0) {
            $("#txtEditLocationNum_" + index).focus();
            $("#txtEditLocationNum_" + index).select();
        }
    };
    
    $scope.removeLocalList = function(user) {
        var tempList = [];
        for (var i = 0; i < $scope.userList.length; i++) {
            if ($scope.userList[i] != user) {
                tempList.push($scope.userList[i]);
            }
        }

        $scope.userList = tempList;
    };

    $scope.removeUser = function (user) {

        if (confirm("Vui lòng xác nhận xóa người dùng:" + user.userId + " !!!")) {
            
            $http({
                method: 'POST',
                url: $window.deleteUrl,
                data: {
                    UserId: user.userId
                }
            })
            .success(function (data) {

                if (data.ErrorDescription != null && data.ErrorDescription != "") {
                    alert(data.ErrorDescription);
                } else {

                    $scope.removeLocalList(user);
                }
            });
        } 
    };

    $scope.cancelEditUser = function (user) {

        if (user.isNew == false) {
            user.editUserName = user.userName;
            user.editing = false;
        } else {
            $scope.removeLocalList(user);
        }
    };

    $scope.saveUser = function (user) {

        if (user.userId == "") {
            user.userIdError = true;
        } else {

            var numUserIdDuplicate = 0;
            for (var i = 0; i < $scope.userList.length; i++) {
                if ($scope.userList[i].userId.toLowerCase() == user.userId.toLowerCase()) {
                    numUserIdDuplicate++;
                }
            }

            if (numUserIdDuplicate < 2) {
                

                if(!user.isNew) {
                    $http({
                        method: 'POST',
                        url: $window.updateUrl,
                        data: {
                            UserId: user.userId,
                            NewName: user.editUserName,
                        }
                    })
                    .success(function (data) {

                        if (data.ErrorDescription != null && data.ErrorDescription != "") {
                            alert(data.ErrorDescription);
                        } else {
                            user.userIdError = false;
                            user.userName = user.editUserName;
                            user.isNew = false;
                            user.editing = false;
                        }
                    });
                } else {
                    $http({
                        method: 'POST',
                        url: $window.addUrl,
                        data: {
                            UserId: user.userId,
                            FullName: user.editUserName,
                        }
                    })
                    .success(function (data) {

                        if (data.ErrorDescription != null && data.ErrorDescription != "") {
                            alert(data.ErrorDescription);
                        } else {
                            user.userIdError = false;
                            user.userName = user.editUserName;
                            user.isNew = false;
                            user.editing = false;
                        }
                    });
                }
                
            } else {
                user.userIdError = true;
            }
        }
    };

    $scope.addNewUser = function () {
        $scope.userList.push({
            userId: '', userName: '', editUserName: '', editing: true, isNew: true, userIdError: false
        });
    };

    $scope.changePassUser = function (user) {

        $scope.changePass.newPass = '';
        $scope.changePass.confirmPass = '';
        $scope.changePass.successful = false;
        $scope.changePass.userId = user.userId;
        $('#dialogResetPass').modal('show');
    };
    
    $scope.changePassUserSubmit = function () {

        $http({
            method: 'POST',
            url: $window.resetPassUrl,
            data: {
                UserId: $scope.changePass.userId,
                NewPassword: $scope.changePass.newPass
            }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {
                $scope.changePass.successful = true;
            }
        });
    };

    $scope.updateRights = function() {

        var requestParam = {
            UserId: $scope.selectedUser.userId,
            RightCodes: []
        };
        
        for (var i = 0; i < $scope.rightList.length; i++) {
            if($scope.rightList[i].selected == true) {
                requestParam.RightCodes.push($scope.rightList[i].rightId);
            }
        }

        $http({
            method: 'POST',
            url: $window.updateRightUrl,
            data: requestParam
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.selectedUser.updateRightSuccess = true;
            }
        });
    };

    $scope.changeRights = function (user) {

        $scope.selectedUser = user;
        $http({
            method: 'POST',
            url: $window.listRightUrl,
            data: {
                UserId: user.userId,
            }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {

                $scope.selectedUser.updateRightSuccess = false;
                for (var i = 0; i < $scope.rightList.length; i++) {

                    var found = false;
                    for (var j = 0; j < data.RightCodes.length && found == false; j++) {
                        if ($scope.rightList[i].rightId == data.RightCodes[j]) {
                            $scope.rightList[i].selected = true;
                            found = true;
                        }
                    }
                    
                    if(found == false) {
                        $scope.rightList[i].selected = false;
                    }
                }
                
                $('#dialogUpdateRights').modal('show');
            }
        });
    };

    $scope.listAllUser = function () {

        $scope.userList = [];

        $http({
            method: 'POST',
            url: $window.listUrl,
            data: { }
        })
        .success(function (data) {

            if (data.ErrorDescription != null && data.ErrorDescription != "") {
                alert(data.ErrorDescription);
            } else {
                for (var i = 0; i < data.ListItems.length; i++) {
                    $scope.userList.push({
                        userId: data.ListItems[i].UserId,
                        userName: data.ListItems[i].FullName,
                        editUserName: data.ListItems[i].FullName,
                        editing: false, isNew: false, userIdError: false
                    });
                }
            }
        });
    };

    $scope.refreshRightList = function() {

        $scope.rightList = [];
        for(var i=0;i<$window.rightList.length;i++) {
            $scope.rightList.push({
                rightId: $window.rightList[i].RightCode,
                rightNameDisplay: $window.rightList[i].RightName,
                selected: false
            });
        }
    };


    $scope.refreshRightList();
    $scope.listAllUser();
};

$(document).ready(function () {
    $('#dialogResetPass').modal({ keyboard: true, show: false, backdrop: 'static' });
    $('#dialogUpdateRights').modal({ keyboard: true, show: false, backdrop: 'static' });
    $('#sidebar .list-group a.users-control').toggleClass('active');
});
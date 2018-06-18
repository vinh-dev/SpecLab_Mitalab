var myApp = angular.module('myApp', []);

var ChangePassController = function ChangePassController($scope, $http, $window) {
    
    $scope.oldPassword = "";
    
    $scope.newPassword = "";
    
    $scope.confirmPassword = "";

    $scope.errorMessage = "";
    
    $scope.successChange = false;
    
    $scope.validate = function () {

        $('.validation-summary-errors #VerifyPasswordNotMatch').hide();
        
        if ($scope.newPassword != "" && $scope.confirmPassword != "") {
            if ($scope.newPassword != $scope.confirmPassword) {
                $('.validation-summary-errors #VerifyPasswordNotMatch').show();
                return false;
            } else {
                $('.validation-summary-errors #VerifyPasswordNotMatch').hide();
                return true;
            }
        } else {
            return false;
        }
    };
    
    $scope.submitChange = function () {

        if ($scope.validate()) {
            
            $scope.errorMessage = "";
            
            $http({
                method: 'POST',
                url: $window.changePassUrl,
                data: {
                    OldPassword: $scope.oldPassword,
                    NewPassword: $scope.newPassword
                }
            })
            .success(function (data) {

                $scope.successChange = data.ChangeResult;
                if (!data.ChangeResult) {
                    $scope.errorMessage = data.ErrorDescription;
                }
            });
        }
    };
};

$(document).ready(function () {

});
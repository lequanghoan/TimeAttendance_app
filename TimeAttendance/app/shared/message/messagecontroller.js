/**
 * 
 */
/**
 * 
 */
angular.module('app').controller('messageCtrl', [
		'$scope',
		'messageinfo', '$uibModalInstance',
		function ($scope, messageinfo, $uibModalInstance) {
		    var vm = this; // Declare view model

		    // Thông tin xác nhận
		    // title: Tiêu đề thông báo
		    // ok: nội dụng hiển thị nút ok
		    // content: nội dung hiển thị trên thông báo
		    // type: 0: Thông báo bình thường; 1: Thông báo lỗi; 2: Thông báo cảnh báo
		    vm.Messageinfo = messageinfo;


		    vm.FnConfirm = fnConfirm;
		    vm.FnCloseModal = fnCloseModal;

		    // Click xác nhận
		    function fnConfirm() {
		        fnCloseModal();
		    }

		    function fnCloseModal() {
		        $uibModalInstance.dismiss('cancel');
		    }

		    $scope.$on('$locationChangeStart', function () {
		        fnCloseModal();
		    });
		}]);
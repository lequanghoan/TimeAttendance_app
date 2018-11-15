angular.module('app').factory('message', [ '$uibModal', function($uibModal) {

	var ShowMessageFactory ={
			ShowMessage:fnShowMessage
	};
	
	return ShowMessageFactory;
	
	function fnShowMessage(message, type) {
		var modalInstance = $uibModal.open({
			templateUrl : 'app/shared/message/view.html',
			controller : 'messageCtrl',
			controllerAs : 'vmMessage',
			resolve : {
				messageinfo : function() {
					return {
						content : message,
						ok : "Đóng",
						title : "Thông báo",
						type : type
					};
				}
			}
		});
		
	}
} ]);

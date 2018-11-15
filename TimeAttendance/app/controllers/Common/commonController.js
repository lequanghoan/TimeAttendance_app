angular.module('app').factory('common', [function ($uibModal) {

    var CommonFactory = {
        //Số bản ghi trên một trạng
        NumPerPage: 10,
        //Hiển thi số trạng
        MaxSize: 5,
        ListStatus: [{ Status: 0, StatusText: "Đang kích hoạt" }, { Status: 1, StatusText: "Đang khóa" }],
        //Đang mở khóa
        UnLock: 0,
        UnLockText: 'Đang kích hoạt',
        //Đang khóa
        Lock: 1,
        LockText: 'Đang khóa',

        ListStatusTransporter: [{ Status: 1, StatusText: "Đang làm việc" }, { Status: 0, StatusText: "Đã nghỉ việc" }],
        //Đang mở khóa
        Actived: 1,
        ActivedText: 'Đang làm việc',
        //Đang khóa
        DeActived: 0,
        DeActivedText: 'Đã nghỉ việc',
       
        //List trạng thái ShippingCar
        //Đang mở khóa
        ShippingCarActived: 1,
        ShippingCarActivedText: 'Đang hoạt động',
        ShippingCarDeActived: 0,
        ShippingCarDeActivedText: 'Không hoạt động',

        //List trạng thái xóa
        ListDeleteFlg: [{ Status: 0, StatusText: "Chưa xóa" }, { Status: 1, StatusText: "Đã xóa" }],
        //Chưa xóa
        DeleteFalse: 0,
        //Đã xóa
        DeleteTrue: 1,
        //List trạng thái hoạt động Device
        ListStatusActive: [{ Id: 0, Name: "Đang hoạt động" }, { Id: 1, Name: "Bị hỏng" }],
        //Đang hoạt động
        StatusActive: 0,
        //Bị hỏng
        StatusNotActive: 1,
        ErrorName: "Đang hoạt động",
        //Chưa xóa
        NotAuto: 0,
        //Đã xóa
        Auto: 1,
        //
        ListDayOfWeek: [{ Id: 1, Name: "Thứ 2", Select: false }, { Id: 2, Name: "Thứ 3", Select: false }, { Id: 3, Name: "Thứ 4", Select: false },
            { Id: 4, Name: "Thứ 5", Select: false }, { Id: 5, Name: "Thứ 6", Select: false }, { Id: 6, Name: "Thứ 7", Select: false }, { Id: 7, Name: "Chủ nhật", Select: false }],
        Disconnect: 0,
        Connect: 1,
        NoAvatar: "/img/no-avatar.png",
        NoImage: "/img/no-image.png",
        PointCircle: "/img/point-circle.png",
        //Min
        MinSizeIcon: 16,
        //Max
        MaxSizeIcon: 64,
        TypeLook: 4,
        TypeCabinet: 3,

        //Giá trị center default của map
        CenterLatitude: "10.549671",
        CenterLongitude: "106.369422",
        CenterPositinDefault: "10.549671, 106.369422",
        StartPositionDefault: "10.439766, 106.310850",
        EndPositinDefault: "10.685322, 106.561171",
        ZoomDefault: "14",
        AccountCompany: "1",
        AccountWarehouse: "2",
        AccountCustomer: "3",
        //List trạng thái hoạt động Device
        ListUserType: [{ Id: "1", Name: "Người dùng Phương Anh" }, { Id: "2", Name: "Quản lý kho" }, { Id: "3", Name: "Người dùng hãng xe" }],
    };

    return CommonFactory;
}]);
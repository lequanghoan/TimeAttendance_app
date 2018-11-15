// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Cognitive Services: http://www.microsoft.com/cognitive
// 
// Microsoft Cognitive Services Github:
// https://github.com/Microsoft/Cognitive
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TimeAttendance.Client.AzureStorage;
using TimeAttendance.Client.Model;
using TimeAttendance.Client.ServiceBus;
using TimeAttendance.Client.Util;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TimeAttendance.Client.Views
{
    public sealed partial class SettingConfig : Page
    {
        ServiceUtil serviceUtil = new ServiceUtil();
        public ObservableCollection<CameraModel> CameraInCurrent { get; set; } = new ObservableCollection<CameraModel>();
        bool IsAdd = true;
        int ServerId = 0;
        bool IsUpdateApi = false;
        private CameraModel cameraSelect = new CameraModel();
        private List<CameraTypeModel> listTypeCamera = new List<CameraTypeModel>();

        public SettingConfig()
        {
            this.InitializeComponent();
            LoadData();
            try
            {
                AzureStorageUploadFiles.GetInstance();
            }
            catch (Exception ex)
            {
                new MessageDialog("Thông tin cấu hình Azure Storage không đúng.").ShowAsync();
            }

            //if (!ServiceBusSetting.CheckServiceBus())
            //{
            //    new MessageDialog("Thông tin cấu hình Service Bus không đúng.").ShowAsync();
            //}
        }

        public void LoadData()
        {
            CameraTypeModel typeModel = new CameraTypeModel()
            {
                TypeId = "Hikvision",
                TypeName = "Hikvision IP Camera"
            };
            listTypeCamera.Add(typeModel);
            typeModel = new CameraTypeModel()
            {
                TypeId = "Panasonic",
                TypeName = "Panasonic Ip Camera"
            };
            listTypeCamera.Add(typeModel);
            typeModel = new CameraTypeModel()
            {
                TypeId = "HdCloud",
                TypeName = "Hd Cloud Ip Camera"
            };
            listTypeCamera.Add(typeModel);
            typeModel = new CameraTypeModel()
            {
                TypeId = "Other",
                TypeName = "Camera Other"
            };
            listTypeCamera.Add(typeModel);
            cameraType.ItemsSource = listTypeCamera;
            cameraType.DisplayMemberPath = "TypeName";
            cameraType.SelectedValuePath = "TypeId";

            try
            {
                var rs = serviceUtil.GetCameraAPI();
                if (rs.ServiceBase != null)
                {
                    ServiceBase.Text = rs.ServiceBase;
                    TotalFrame.Text = rs.TotalFrame.ToString();
                    //Thông tin Queue
                    QueueURL.Text = rs.QueueURL;
                    AccessKeyName.Text = rs.AccessKeyName;
                    AccessKeyValue.Text = rs.AccessKeyValue;
                    SBTopicName.Text = rs.BusTopicName;
                    SBKeySend.Text = rs.BusKeySend;
                    //Thông tin Azure
                    ASAccountName.Text = rs.AzureName;
                    ASAccountKey.Text = rs.AzureKey;
                    ASContainer.Text = rs.AzureContainer;
                    ASUrlHost.Text = rs.AzureUrlHost;
                    ServerId = rs.ServerId;
                    IsUpdateApi = true;
                }
                else
                {
                    IsUpdateApi = false;
                }
                if (rs.ListCamera != null)
                    itemListView.ItemsSource = rs.ListCamera;
            }
            catch (Exception ex)
            {

            }
        }

        private void EditCameraButton_Click(object sender, RoutedEventArgs e)
        {
            if (cameraSelect == null || string.IsNullOrEmpty(cameraSelect.CameraId))
            {
                //thông báo chưa chọn
                cameraFlyout.Hide();
                var dialog = new MessageDialog("Bạn chưa chọn camera");
                dialog.ShowAsync();
            }
            else
            {
                saveButton.Content = "Cập nhật";
                cameraFlyout.ShowAt(this.EditCameraButton);
                cameraIP.Text = cameraSelect.CameraIP;
                cameraType.SelectedValue = cameraSelect.CameraType;
                cameraUser.Text = cameraSelect.CameraUser;
                cameraPass.Text = cameraSelect.CameraPass;
                cameraURI.Text = cameraSelect.StreaURI;
                cameraIndex.Text = cameraSelect.IndexView;
                boxWidth.Text = cameraSelect.BoxWidth.ToString();
                boxHeight.Text = cameraSelect.BoxHeight.ToString();
                boxPointX.Text = cameraSelect.BoxPointX.ToString();
                boxPointY.Text = cameraSelect.BoxPointY.ToString();
                IsAdd = false;
            }
        }

        /// <summary>
        /// Thêm mới camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCameraButton_Click(object sender, RoutedEventArgs e)
        {
            saveButton.Content = "Thêm mới";
            IsAdd = true;
            ClearDataForm();
        }

        /// <summary>
        /// Xóa dữ liệu 
        /// </summary>
        public void ClearDataForm()
        {
            this.cameraIP.Text = "";
            this.cameraType.SelectedValue = "";
            this.cameraUser.Text = "";
            this.cameraPass.Text = "";
            this.cameraURI.Text = "";
            this.cameraIndex.Text = "";
            this.boxWidth.Text = "0";
            this.boxHeight.Text = "0";
            this.boxPointX.Text = "0";
            this.boxPointY.Text = "0";
        }

        /// <summary>
        /// Xóa camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelCameraButton_Click(object sender, RoutedEventArgs e)
        {
            if (cameraSelect == null && string.IsNullOrEmpty(cameraSelect.CameraId))
            {
                var dialog = new MessageDialog("Bạn chưa chọn camera");
                dialog.ShowAsync();
            }
            else
            {
                serviceUtil.DeleteCamera(cameraSelect);
                itemListView.ItemsSource = serviceUtil.GetListCamera();
                var dialog = new MessageDialog("Xóa camera thành công");
                dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Select 1 row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cameraSelect = (CameraModel)itemListView.SelectedItem;
        }

        /// <summary>
        /// Đóng giao diện thêm mới
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            ClearDataForm();
            cameraFlyout.Hide();
        }

        /// <summary>
        /// Lưu camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int.Parse(cameraIndex.Text);
                if (IsAdd)
                {
                    CameraModel cameraModel = new CameraModel()
                    {
                        CameraIP = cameraIP.Text,
                        CameraType = ((CameraTypeModel)cameraType.SelectedItem).TypeId,
                        CameraUser = cameraUser.Text,
                        CameraPass = cameraPass.Text,
                        StreaURI = cameraURI.Text,
                        IndexView = cameraIndex.Text,
                        BoxWidth = uint.Parse(boxWidth.Text),
                        BoxHeight = uint.Parse(boxHeight.Text),
                        BoxPointX = uint.Parse(boxPointX.Text),
                        BoxPointY = uint.Parse(boxPointY.Text)
                    };
                    serviceUtil.AddCamera(cameraModel);
                    var dialog = new MessageDialog("Thêm mới camera thành công");
                    dialog.ShowAsync();
                }
                else
                {
                    cameraSelect.CameraIP = cameraIP.Text;
                    cameraSelect.CameraType = ((CameraTypeModel)cameraType.SelectedItem).TypeId;
                    cameraSelect.CameraUser = cameraUser.Text;
                    cameraSelect.CameraPass = cameraPass.Text;
                    cameraSelect.StreaURI = cameraURI.Text;
                    cameraSelect.IndexView = cameraIndex.Text;
                    cameraSelect.BoxWidth = uint.Parse(boxWidth.Text);
                    cameraSelect.BoxHeight = uint.Parse(boxHeight.Text);
                    cameraSelect.BoxPointX = uint.Parse(boxPointX.Text);
                    cameraSelect.BoxPointY = uint.Parse(boxPointY.Text);

                    serviceUtil.UpdateCamera(cameraSelect);

                    var dialog = new MessageDialog("Cập nhật camera thành công");
                    dialog.ShowAsync();
                }
            }
            catch (Exception)
            {

                var dialogErro = new MessageDialog("Thứ tự hoặc Giới hạn vùng phân tích hình ảnh phải là số");
                dialogErro.ShowAsync();
            }
            itemListView.ItemsSource = serviceUtil.GetListCamera();
        }

        /// <summary>
        /// Lưu thông tin api
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAPI_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ServiceBase.Text) || string.IsNullOrEmpty(TotalFrame.Text) || string.IsNullOrEmpty(QueueURL.Text)
                    || string.IsNullOrEmpty(AccessKeyName.Text) || string.IsNullOrEmpty(AccessKeyValue.Text) || string.IsNullOrEmpty(ASAccountName.Text) || string.IsNullOrEmpty(ASAccountKey.Text)
                    || string.IsNullOrEmpty(ASContainer.Text) || string.IsNullOrEmpty(ASUrlHost.Text))
                {
                    var dialogValidate = new MessageDialog("Điền đầy đủ các thông tin bắt buộc (*)");
                    dialogValidate.ShowAsync();
                    return;
                }

                int totalFrame = int.Parse(TotalFrame.Text);

                if (totalFrame <= 0)
                {
                    var dialogValidate = new MessageDialog("Số mẫu phân tích/1ms phải lớn hơn hoặc bằng 1");
                    dialogValidate.ShowAsync();
                    return;
                }

                APIModel apiModel = new APIModel()
                {
                    ServiceBase = ServiceBase.Text,
                    TotalFrame = totalFrame,
                    ServerId = ServerId,
                    IsUpdate = IsUpdateApi,
                    QueueURL = QueueURL.Text,
                    AccessKeyName = AccessKeyName.Text,
                    AccessKeyValue = AccessKeyValue.Text,
                    BusTopicName = SBTopicName.Text,
                    BusKeySend = SBKeySend.Text,
                    AzureName = ASAccountName.Text,
                    AzureKey = ASAccountKey.Text,
                    AzureContainer = ASContainer.Text,
                    AzureUrlHost = ASUrlHost.Text
                };
                serviceUtil.SaveInfoAPI(apiModel);
                IsUpdateApi = true;
                var dialog = new MessageDialog("Lưu thông tin thành công");
                dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var dialogErro = new MessageDialog("Phát sinh lỗi: " + ex.Message);
                dialogErro.ShowAsync();
            }

        }

        private void CameraType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateCameraURI();
        }

        private void cameraInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            CreateCameraURI();
        }

        private void CreateCameraURI()
        {
            if (cameraType.SelectedItem != null && !string.IsNullOrEmpty(cameraUser.Text) && !string.IsNullOrEmpty(cameraPass.Text) && !string.IsNullOrEmpty(cameraIP.Text))
            {
                var itemSelect = (CameraTypeModel)cameraType.SelectedItem;
                if (itemSelect.TypeId.Equals("Hikvision"))
                {
                    cameraURI.Text = string.Format("rtsp://{0}:{1}@{2}:554/Streaming/channels/101/", cameraUser.Text, cameraPass.Text, cameraIP.Text);
                }
                else if (itemSelect.TypeId.Equals("Panasonic"))
                {
                    cameraURI.Text = string.Format("rtsp://{0}:{1}@{2}:554/cam/realmonitor?channel=1&subtype=0", cameraUser.Text, cameraPass.Text, cameraIP.Text);
                }
                else if (itemSelect.TypeId.Equals("HdCloud"))
                {
                    cameraURI.Text = string.Format("rtsp://{0}:{1}@{2}:554/live/ch1", cameraUser.Text, cameraPass.Text, cameraIP.Text);
                }
                else if (itemSelect.TypeId.Equals("Other"))
                {
                    cameraURI.Text = string.Format("rtsp://{0}:{1}@{2}:554/Streaming/channels/101/", cameraUser.Text, cameraPass.Text, cameraIP.Text);
                }
            }
        }

        private void cameraIndex_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            double dtemp;
            if (!double.TryParse(sender.Text, out dtemp) && sender.Text != "")
            {
                int pos = sender.SelectionStart - 1;
                sender.Text = sender.Text.Remove(pos, 1);
                sender.SelectionStart = pos;
            }
        }

        private void closeSetting_Click(object sender, RoutedEventArgs e)
        {
            settingFlyout.Hide();
        }
    }
}
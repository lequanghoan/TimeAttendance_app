using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using TimeAttendance.Client.Model;

namespace TimeAttendance.Client.Util
{
    public class ServiceUtil
    {
        private SqliteConnection db = new SqliteConnection(Constants.PathDatabase);
        /// <summary>
        /// Gét thông tin
        /// </summary>
        /// <returns></returns>
        public InfoSettingModel GetCameraAPI()
        {
            InfoSettingModel model = new InfoSettingModel();
            try
            {
                using (SqliteConnection db = new SqliteConnection(Constants.PathDatabase))
                {
                    db.Open();
                    using (SqliteCommand selectConfigServer = new SqliteCommand("SELECT * from ConfigServer", db))
                    {
                        using (SqliteDataReader query = selectConfigServer.ExecuteReader())
                        {
                            while (query.Read())
                            {
                                model.ServerId = query.GetInt32(0);
                                model.ServiceBase = query.GetString(1);
                                model.TotalFrame = query.GetInt32(2);
                                model.QueueURL = query.GetString(3);
                                model.AccessKeyName = query.GetString(4);
                                model.AccessKeyValue = query.GetString(5);
                                model.BusTopicName = query.GetString(6);
                                model.BusKeySend = query.GetString(7);
                                model.AzureName = query.GetString(8);
                                model.AzureKey = query.GetString(9);
                                model.AzureContainer = query.GetString(10);
                                model.AzureUrlHost = query.GetString(11);

                                if (model.TotalFrame == 0)
                                    model.TotalFrame = 1;
                                break;
                            }
                        }
                    }

                    using (SqliteCommand selectConfigCamera = new SqliteCommand("SELECT * from ConfigCamera order by IndexView", db))
                    {
                        using (SqliteDataReader query = selectConfigCamera.ExecuteReader())
                        {

                            model.ListCamera = new List<CameraModel>();
                            CameraModel cameraModel;
                            int index = 1;
                            while (query.Read())
                            {
                                cameraModel = new CameraModel()
                                {
                                    Index = index.ToString(),
                                    CameraId = query.GetInt32(0).ToString(),
                                    CameraIP = query.GetString(1),
                                    CameraType = query.GetString(2),
                                    CameraUser = query.GetString(3),
                                    CameraPass = query.GetString(4),
                                    StreaURI = query.GetString(5),
                                    IndexView = query.GetInt32(6).ToString(),
                                    BoxWidth = (uint)query.GetInt32(7),
                                    BoxHeight = (uint)query.GetInt32(8),
                                    BoxPointX = (uint)query.GetInt32(9),
                                    BoxPointY = (uint)query.GetInt32(10),

                                };
                                if (cameraModel.BoxWidth == 0 && cameraModel.BoxHeight == 0 && cameraModel.BoxPointX == 0 && cameraModel.BoxPointY == 0)
                                {
                                    cameraModel.BoxWidth = InfoSettingFix.FixTakeWidth;
                                    cameraModel.BoxHeight = InfoSettingFix.FixTakeHeight;
                                }
                                model.ListCamera.Add(cameraModel);
                                index++;
                            }
                        }
                    }

                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }

        /// <summary>
        /// Lưu thông tin API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void SaveInfoAPI(APIModel model)
        {
            bool IsUpdate = model.IsUpdate;
            try
            {
                using (SqliteConnection db = new SqliteConnection(Constants.PathDatabase))
                {
                    db.Open();
                    using (SqliteCommand sqlCommand = new SqliteCommand())
                    {
                        sqlCommand.Connection = db;
                        if (IsUpdate)
                        {
                            sqlCommand.CommandText = "UPDATE ConfigServer SET ServiceBase=@ServiceBase,TotalFrame=@TotalFrame,QueueURL=@QueueURL,AccessKeyName=@AccessKeyName,AccessKeyValue=@AccessKeyValue,BusTopicName=@BusTopicName,BusKeySend=@BusKeySend,AzureName=@AzureName,AzureKey=@AzureKey,AzureContainer=@AzureContainer,AzureUrlHost=@AzureUrlHost where ServerId=@ServerId;";
                            sqlCommand.Parameters.AddWithValue("@ServiceBase", model.ServiceBase);
                            sqlCommand.Parameters.AddWithValue("@TotalFrame", model.TotalFrame);
                            sqlCommand.Parameters.AddWithValue("@QueueURL", model.QueueURL);
                            sqlCommand.Parameters.AddWithValue("@AccessKeyName", model.AccessKeyName);
                            sqlCommand.Parameters.AddWithValue("@AccessKeyValue", model.AccessKeyValue);
                            sqlCommand.Parameters.AddWithValue("@BusTopicName", model.BusTopicName);
                            sqlCommand.Parameters.AddWithValue("@BusKeySend", model.BusKeySend);
                            sqlCommand.Parameters.AddWithValue("@AzureName", model.AzureName);
                            sqlCommand.Parameters.AddWithValue("@AzureKey", model.AzureKey);
                            sqlCommand.Parameters.AddWithValue("@AzureContainer", model.AzureContainer);
                            sqlCommand.Parameters.AddWithValue("@AzureUrlHost", model.AzureUrlHost);
                            sqlCommand.Parameters.AddWithValue("@ServerId", model.ServerId);
                            sqlCommand.ExecuteNonQuery();
                        }
                        //Thêm mới nếu chưa có
                        else
                        {
                            sqlCommand.CommandText = "INSERT INTO ConfigServer VALUES (NULL, @ServiceBase, @TotalFrame, @QueueURL, @AccessKeyName, @AccessKeyValue, @BusTopicName, @BusKeySend, @AzureName, @AzureKey, @AzureContainer, @AzureUrlHost);";
                            sqlCommand.Parameters.AddWithValue("@ServiceBase", model.ServiceBase);
                            sqlCommand.Parameters.AddWithValue("@TotalFrame", model.TotalFrame);
                            sqlCommand.Parameters.AddWithValue("@QueueURL", model.QueueURL);
                            sqlCommand.Parameters.AddWithValue("@AccessKeyName", model.AccessKeyName);
                            sqlCommand.Parameters.AddWithValue("@AccessKeyValue", model.AccessKeyValue);
                            sqlCommand.Parameters.AddWithValue("@BusTopicName", model.BusTopicName);
                            sqlCommand.Parameters.AddWithValue("@BusKeySend", model.BusKeySend);
                            sqlCommand.Parameters.AddWithValue("@AzureName", model.AzureName);
                            sqlCommand.Parameters.AddWithValue("@AzureKey", model.AzureKey);
                            sqlCommand.Parameters.AddWithValue("@AzureContainer", model.AzureContainer);
                            sqlCommand.Parameters.AddWithValue("@AzureUrlHost", model.AzureUrlHost);
                            sqlCommand.ExecuteNonQuery();
                        }
                    }

                }
                InfoSettingFix.InfoSetting = this.GetCameraAPI();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///Danh sách camera
        /// </summary>
        /// <returns></returns>
        public List<CameraModel> GetListCamera()
        {
            List<CameraModel> listCamera = new List<CameraModel>();
            try
            {
                using (SqliteConnection db = new SqliteConnection(Constants.PathDatabase))
                {
                    db.Open();
                    using (SqliteCommand selectConfigCamera = new SqliteCommand("SELECT * from ConfigCamera order by IndexView", db))
                    {
                        using (SqliteDataReader query = selectConfigCamera.ExecuteReader())
                        {
                            CameraModel cameraModel;
                            int index = 1;
                            while (query.Read())
                            {
                                cameraModel = new CameraModel()
                                {
                                    Index = index.ToString(),
                                    CameraId = query.GetInt32(0).ToString(),
                                    CameraIP = query.GetString(1),
                                    CameraType = query.GetString(2),
                                    CameraUser = query.GetString(3),
                                    CameraPass = query.GetString(4),
                                    StreaURI = query.GetString(5),
                                    IndexView = query.GetInt32(6).ToString(),
                                    BoxWidth = (uint)query.GetInt32(7),
                                    BoxHeight = (uint)query.GetInt32(8),
                                    BoxPointX = (uint)query.GetInt32(9),
                                    BoxPointY = (uint)query.GetInt32(10),
                                };
                                listCamera.Add(cameraModel);
                                index++;
                            }
                        }
                    }
                };

                return listCamera;
            }
            catch (Exception ex)
            {
                return listCamera;
            }
        }

        /// <summary>
        /// Thêm mới camera
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void AddCamera(CameraModel model)
        {
            try
            {
                using (SqliteConnection db = new SqliteConnection(Constants.PathDatabase))
                {
                    db.Open();
                    using (SqliteCommand sqlCommand = new SqliteCommand())
                    {
                        sqlCommand.Connection = db;
                        sqlCommand.CommandText = "INSERT INTO ConfigCamera VALUES (NULL, @CameraIP, @CameraType, @CameraUser, @CameraPass, @StreaURI, @IndexView, @BoxWidth, @BoxHeight, @BoxPointX, @BoxPointY);";
                        sqlCommand.Parameters.AddWithValue("@CameraIP", model.CameraIP);
                        sqlCommand.Parameters.AddWithValue("@CameraType", model.CameraType);
                        sqlCommand.Parameters.AddWithValue("@CameraUser", model.CameraUser);
                        sqlCommand.Parameters.AddWithValue("@CameraPass", model.CameraPass);
                        sqlCommand.Parameters.AddWithValue("@StreaURI", model.StreaURI);
                        sqlCommand.Parameters.AddWithValue("@IndexView", Int32.Parse(model.IndexView));
                        sqlCommand.Parameters.AddWithValue("@BoxWidth", (int)model.BoxWidth);
                        sqlCommand.Parameters.AddWithValue("@BoxHeight", (int)model.BoxHeight);
                        sqlCommand.Parameters.AddWithValue("@BoxPointX", (int)model.BoxPointX);
                        sqlCommand.Parameters.AddWithValue("@BoxPointY", (int)model.BoxPointY);
                        sqlCommand.ExecuteNonQuery();
                    }
                }

                InfoSettingFix.InfoSetting = this.GetCameraAPI();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Cập nhật camera
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void UpdateCamera(CameraModel model)
        {
            try
            {
                using (SqliteConnection db = new SqliteConnection(Constants.PathDatabase))
                {
                    db.Open();
                    using (SqliteCommand sqlCommand = new SqliteCommand())
                    {
                        sqlCommand.Connection = db;
                        sqlCommand.CommandText = "UPDATE ConfigCamera SET CameraIP=@CameraIP, CameraType=@CameraType, CameraUser=@CameraUser, CameraPass=@CameraPass, StreaURI=@StreaURI, IndexView=@IndexView, BoxWidth=@BoxWidth, BoxHeight=@BoxHeight, BoxPointX=@BoxPointX, BoxPointY=@BoxPointY where CameraId=@CameraId;";
                        sqlCommand.Parameters.AddWithValue("@CameraIP", model.CameraIP);
                        sqlCommand.Parameters.AddWithValue("@CameraType", model.CameraType);
                        sqlCommand.Parameters.AddWithValue("@CameraUser", model.CameraUser);
                        sqlCommand.Parameters.AddWithValue("@CameraPass", model.CameraPass);
                        sqlCommand.Parameters.AddWithValue("@StreaURI", model.StreaURI);
                        sqlCommand.Parameters.AddWithValue("@IndexView", model.IndexView);
                        sqlCommand.Parameters.AddWithValue("@BoxWidth", model.BoxWidth);
                        sqlCommand.Parameters.AddWithValue("@BoxHeight", model.BoxHeight);
                        sqlCommand.Parameters.AddWithValue("@BoxPointX", model.BoxPointX);
                        sqlCommand.Parameters.AddWithValue("@BoxPointY", model.BoxPointY);
                        sqlCommand.Parameters.AddWithValue("@CameraId", model.CameraId);
                        sqlCommand.ExecuteNonQuery();
                    }
                }

                InfoSettingFix.InfoSetting = this.GetCameraAPI();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Xóa camera
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void DeleteCamera(CameraModel model)
        {
            try
            {
                using (SqliteConnection db = new SqliteConnection(Constants.PathDatabase))
                {
                    db.Open();
                    using (SqliteCommand sqlCommand = new SqliteCommand())
                    {
                        sqlCommand.Connection = db;
                        sqlCommand.CommandText = "delete from ConfigCamera where CameraId=@CameraId;";
                        sqlCommand.Parameters.AddWithValue("@CameraId", model.CameraId);
                        sqlCommand.ExecuteNonQuery();
                    }
                }

                InfoSettingFix.InfoSetting = this.GetCameraAPI();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

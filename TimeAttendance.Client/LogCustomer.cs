using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceVip
{
    public class LogCustomer
    {
        private SqliteConnection db = new SqliteConnection("Filename=VipCustomer.db");
        //Thống kê log
        public List<LogCustomerModel> StatisticLog()
        {
            List<LogCustomerModel> listResult = new List<LogCustomerModel>();
            try
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT * from LogCustomer", db);
                SqliteDataReader query;

                query = selectCommand.ExecuteReader();

                LogCustomerModel model;
                while (query.Read())
                {
                    model = new LogCustomerModel()
                    {
                        LogId = query.GetString(0),
                        CustomerName = query.GetString(1),
                        DateIn = query.GetDateTime(2),
                        Type = query.GetString(3),
                    };
                    listResult.Add(model);
                }
                db.Close();
                return listResult;
            }
            catch (SqliteException error)
            {
                //Handle error
                return listResult;
            }
            catch (Exception ex)
            {
                //Handle error
                return listResult;
            }
        }

        //Thêm log
        public async Task AddLog(LogCustomerModel model)
        {
            try
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                //Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO LogCustomer VALUES (NULL, @CustomerName, @DateIn, @Type);";
                insertCommand.Parameters.AddWithValue("@CustomerName", model.CustomerName);
                insertCommand.Parameters.AddWithValue("@DateIn", model.DateIn);
                insertCommand.Parameters.AddWithValue("@Type", model.Type);

                insertCommand.ExecuteReader();
                db.Close();
            }
            catch (SqliteException error)
            {

            }
            catch (Exception ex)
            {

            }
        }
    }
}

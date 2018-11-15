using FaceVipService;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FaceVip.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StatisticLog : Page
    {
        private ServiceFaceVip logCustomer = new ServiceFaceVip();
        public List<LogCustomerModel> ListCustomerVip = new List<LogCustomerModel>();

        public StatisticLog()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.DataContext = this;

            IEnumerable<PersonGroup> personGroups = await FaceServiceHelper.ListPersonGroupsAsync(SettingsHelper.Instance.WorkspaceKey);

            List<CustomerModel> listCustomer = new List<CustomerModel>();
            CustomerModel modelCus;
            foreach (var itemGrop in personGroups)
            {
                var personsTemp = await FaceServiceHelper.GetPersonsAsync(itemGrop.PersonGroupId);
                foreach (var itemper in personsTemp)
                {
                    modelCus = new CustomerModel();
                    PersonFace personFace = null;
                    foreach (Guid face in itemper.PersistedFaceIds)
                    {
                        personFace = await FaceServiceHelper.GetPersonFaceAsync(itemGrop.PersonGroupId, itemper.PersonId, face);
                        break;
                    }
                    if (personFace != null)
                        modelCus.LinkImage = personFace.UserData;
                    modelCus.Name = itemper.Name;

                    listCustomer.Add(modelCus);
                }
            }
            //PersonFace personFace = await FaceServiceHelper.GetPersonFaceAsync(this.CurrentPersonGroup.PersonGroupId, this.SelectedPerson.PersonId, face);

            List<LogCustomerModel> listResult = logCustomer.StatisticLogCustomer().Where(r => r.DateIn.Date == DateTime.Now.Date).ToList();

            this.ListCustomerVip = listResult.Where(r => r.Type.Equals("1")).ToList();
            var listTemp = this.ListCustomerVip.GroupBy(info => info.CustomerName)
                        .Select(group => new
                        {
                            CustomerName = group.Key,
                            Count = group.Count()
                        })
                        .OrderBy(x => x.Count);
            int max = listTemp != null && listTemp.Count() > 0 ? listTemp.Max(r => r.Count) : 0;

            List<StatisticVip> listVip1 = new List<StatisticVip>();
            List<StatisticVip> listVip2 = new List<StatisticVip>();
            StatisticVip statisticVip;
            int index = 1;
            foreach (var item in listTemp)
            {
                var tenpCustomer = listCustomer.Where(r => r.Name.Equals(item.CustomerName)).FirstOrDefault();
                statisticVip = new StatisticVip()
                {
                    CustomerName = item.CustomerName,
                    Count = item.Count,
                    Max = max,
                    LinkImge = tenpCustomer != null ? tenpCustomer.LinkImage : string.Empty
                };
                if (index % 2 == 0)
                    listVip2.Add(statisticVip);
                else
                    listVip1.Add(statisticVip);

                index++;
            }
            this.customerVip.ItemsSource = listVip1;
            this.customerVip2.ItemsSource = listVip2;

            List<PointModel> data = new List<PointModel>();
            List<PointModel> dataVip = new List<PointModel>();
            PointModel hourValue;
            for (int hour = 1; hour <= 24; hour++)
            {
                hourValue = new PointModel()
                {
                    Name = hour.ToString() + "h",
                    Amount = listResult.Where(r => r.DateIn.Hour == hour).Count(),
                };
                data.Add(hourValue);
                hourValue = new PointModel()
                {
                    Name = hour.ToString() + "h",
                    Amount = listResult.Where(r => r.DateIn.Hour == hour && r.Type.Equals("1")).GroupBy(g => g.CustomerName).Count(),
                };
                dataVip.Add(hourValue);
            }

            List<LineSeries> listLineSeries = new List<LineSeries>();
            var seriesLine = new LineSeries()
            {
                Title = "Tổng lưu lượng khách hàng",
                IndependentValuePath = "Name",
                DependentValuePath = "Amount",
                ItemsSource = data
            };
            listLineSeries.Add(seriesLine);
            seriesLine = new LineSeries()
            {
                Title = "Lưu lượng khách hàng Vip",
                IndependentValuePath = "Name",
                DependentValuePath = "Amount",
                ItemsSource = dataVip
            };
            listLineSeries.Add(seriesLine);
            LineChart.Series.AddRange(listLineSeries);

            base.OnNavigatedTo(e);
        }

        public class PointModel
        {
            public string Name { get; set; }
            public int Amount { get; set; }
        }
    }
}

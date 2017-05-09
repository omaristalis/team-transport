using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static private string BaseUrl = "http://webapplication120170507042553.azurewebsites.net/api/carriages"; //"http://localhost:1331/api/values"; // "http://webapplication120170507042553.azurewebsites.net/api/values";

        private HttpClient client;

        public enum BusyType
        {
            Light = 0,
            Medium,
            Heavy
        };

        private MainWindow.BusyType _busyType = BusyType.Light;

        private DispatcherTimer timer;

        Dictionary<BusyType, string> images = new Dictionary<BusyType, string>();

        public MainWindow()
        {
            

            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeImagesDictionary();

            this.client = new HttpClient();

            this.timer = new System.Windows.Threading.DispatcherTimer();
            this.timer.Tick += new EventHandler(UpdateBackground);
            this.timer.Interval = new TimeSpan(0, 0, 2);
            this.timer.Start();

            this.Background = new ImageBrush(new BitmapImage(GetImageUri(this._busyType)));
        }


        private void InitializeImagesDictionary()
        {
            var dir = Directory.GetCurrentDirectory();


            this.images.Add(BusyType.Light, System.IO.Path.Combine(dir, "image_empty_small.png"));
            this.images.Add(BusyType.Medium, System.IO.Path.Combine(dir, "image_medium_small.png"));
            this.images.Add(BusyType.Heavy, System.IO.Path.Combine(dir, "image_full_small.png"));
        }

        private Uri GetImageUri(BusyType busyType)
        {
            return new Uri(this.images[busyType]);
        }

        private void UpdateBackground(object source, EventArgs e)
        {
            //if (this._busyType == BusyType.Heavy)
            //{
            //    this._busyType = BusyType.Light;
            //}
            //else
            //{
            //    this._busyType++;
            //}

            //var carriageDto = new CarriageDto() { Id = 1, Status = (int)this._busyType };
            //var content = new StringContent(JsonConvert.SerializeObject(carriageDto), Encoding.UTF8, "application/json");

            //var response = client.PutAsync(BaseUrl + "/" + 1, content).Result;

            var response = this.client.GetAsync(BaseUrl + "/" + 1).Result;

            var text = response.Content.ReadAsStringAsync().Result;
            CarriageDto dto = JsonConvert.DeserializeObject<CarriageDto>(text);

            this._busyType = (BusyType)dto.Status;

            var image = this.GetImageUri(this._busyType);

            this.Background = new ImageBrush(new BitmapImage(image));
        }

    }
}

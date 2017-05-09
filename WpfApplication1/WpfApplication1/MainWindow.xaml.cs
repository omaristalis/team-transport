using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    using System.Net.Http;

    using Newtonsoft.Json;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string baseUrl = "http://webapplication120170507042553.azurewebsites.net/api/carriages";

        public MainWindow()
        {
            InitializeComponent();
            Respond();
        }

        private void DrawRectangle(HttpClient client, int offset, Canvas layoutRoot)
        {
            var response = client.GetAsync(baseUrl + "/" + offset).Result;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return;
            }

            var text = response.Content.ReadAsStringAsync().Result;
            CarriageDto dto = JsonConvert.DeserializeObject<CarriageDto>(text);

            Color color;
            switch (dto.Status)
            {
                case 0:
                    color = Colors.Green;
                    break;
                case 1:
                    color = Colors.Orange;
                    break;
                case 2:
                    color = Colors.Red;
                    break;
                default:
                    color = Colors.Gray;
                    break;
            }

            layoutRoot.Children.Clear();

            Rectangle exampleRectangle = new Rectangle();
                exampleRectangle.Width = layoutRoot.ActualWidth;
                exampleRectangle.Height = layoutRoot.ActualHeight;
                // Create a SolidColorBrush and use it to
                // paint the rectangle.
                SolidColorBrush myBrush = new SolidColorBrush(color);
                exampleRectangle.Stroke = Brushes.White;
                exampleRectangle.StrokeThickness = 4;
                exampleRectangle.Fill = myBrush;
            layoutRoot.Children.Insert(0, exampleRectangle);
        }

        private async void Respond()
        {
            await Task.Delay(2000);

            while (true)
            {
                try
                {
                    var client = new HttpClient();

                    DrawRectangle(client, 0, this.LayoutRoot0);
                    DrawRectangle(client, 1, this.LayoutRoot1);

                    await Task.Delay(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
    }
}

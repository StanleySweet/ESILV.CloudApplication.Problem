using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ESILV.CloudApplication.Problem
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : Window
    {
        public Chart(string data, int maxCities)
        {
            InitializeComponent();
            LoadBarChartData(data, maxCities);
        }

        private void LoadBarChartData(string data , int max)
        {
            var obj = JArray.Parse(data);

            var values = new List<KeyValuePair<string, int>>();
            var count = 0;
            foreach (var city in obj)
            {
                ++count;
                values.Add(new KeyValuePair<string, int>(city["_id"].ToString(), int.Parse(city["count"].ToString())));
                if (count == max)
                    break;
            }
            ((BarSeries)mcChart.Series[0]).ItemsSource = values;
            ((BarSeries)mcChart.Series[0]).FontSize = 12;
            //grid.RenderSize = ((BarSeries)mcChart.Series[0]).DesiredSize;
            /*var chartArea = ((BarSeries)mcChart.Series[0]).ChartAreas[series.ChartArea];

            // set view range to [0,max]
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisY.Maximum = 10;
            chartArea.CursorY.AutoScroll = true;
            chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;*/
            grid.Height = max * 25;
        }
    }
}

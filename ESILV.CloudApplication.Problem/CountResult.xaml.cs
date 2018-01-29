using Newtonsoft.Json.Linq;
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
using System.Windows.Shapes;

namespace ESILV.CloudApplication.Problem
{
    /// <summary>
    /// Interaction logic for CountResult.xaml
    /// </summary>
    public partial class CountResult : Window
    {
        public CountResult()
        {
            InitializeComponent();
        }
        public CountResult(string count)
        {
            var obj = JArray.Parse(count);

            InitializeComponent();
            this.ResultTextBlock.Text = string.Format("Il y a {0} élément(s) dans la base de données.", obj[0]["count"]);
        }
    }
}

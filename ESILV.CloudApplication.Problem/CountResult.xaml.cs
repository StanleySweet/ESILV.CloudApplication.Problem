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
        public CountResult(string count, string key = "count",string finalString= "élément(s) dans la base de données")
        {
            var obj = JArray.Parse(count);

            InitializeComponent();
            if (obj.Count <1) {
                ResultTextBlock.Text = "Il n'y a aucun résultat disponible.";
            } else {
                ResultTextBlock.Text = string.Format("Il y a {0} " + finalString + ".", obj[0][key]);
            }
        }
    }
}

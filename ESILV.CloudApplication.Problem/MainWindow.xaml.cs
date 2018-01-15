using ESILV.CloudApplication.Problem.MongoDBWrapper;
using MongoDB.Bson;
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


namespace ESILV.CloudApplication.Problem
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MongoDBWrapper.MongoDBWrapper _mongoDBWrapper;

        public MainWindow()
        {
            InitializeComponent();
            _mongoDBWrapper = new MongoDBWrapper.MongoDBWrapper();


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _mongoDBWrapper.Connect("test");
            var queryResult = new List<BsonDocument>();
            Task<List<BsonDocument>> t = _mongoDBWrapper.QueryDatabase();
            t.ContinueWith((result) => queryResult = result.Result);
            t.Wait();
        }
    }
}

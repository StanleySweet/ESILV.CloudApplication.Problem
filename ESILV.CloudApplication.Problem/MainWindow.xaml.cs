using MongoDB.Bson;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;


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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Debug.WriteLine(openFileDialog1.FileName);
                StreamReader sr = new
                StreamReader(openFileDialog1.FileName);
                System.Windows.Forms.MessageBox.Show(sr.ReadToEnd());
                sr.Close();

                DataTable csvTable = new DataTable();

                MongoClient client = new MongoClient("mongodb://127.0.0.1:27017/test"); // local database
                var db = client.GetDatabase("test");

                var reader = new StreamReader(File.OpenRead(@""+openFileDialog1.FileName+"")); // where <full path to csv> is the file path, of course
                IMongoCollection<BsonDocument> csvFile = db.GetCollection<BsonDocument>("test");

                //reader.ReadLine(); // to skip header
                string line = sr.ReadLine();
                var columnNames = Regex.Split(line, ",");
                while ((line = sr.ReadLine()) != null)
                {
                    BsonDocument row = new BsonDocument();
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] cols = Regex.Split(line, ",");
                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            row.Add(columnNames[i],cols[i]);
                        }
                    }
                    csvFile.InsertOne(row);
                }
            }
        }
    }
}


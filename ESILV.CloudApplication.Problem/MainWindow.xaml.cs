namespace ESILV.CloudApplication.Problem
{
    using MongoDB.Bson;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;
    using MongoDB.Driver;
    using ESILV.CloudApplication.Problem.MongoDBWrapper;

    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MongoDBWrapper.MongoDBWrapper _mongoDBWrapper;

        public MainWindow()
        {
            InitializeComponent();
            _mongoDBWrapper = new MongoDBWrapper.MongoDBWrapper(Constants.DATABASE_NAME, Constants.COLLECTION_NAME);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var queryResult = _mongoDBWrapper.QueryDatabase();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Debug.WriteLine(openFileDialog1.FileName);
                IMongoCollection<BsonDocument> collection = _mongoDBWrapper.GetCollection(Constants.COLLECTION_NAME);
                // where <full path to csv> is the file path, of course
                // StreamReader is IDisposable, so dispose it properly.
                using (StreamReader csvFile = new StreamReader(File.OpenRead(@"" + openFileDialog1.FileName + "")))
                {
                    string line = csvFile.ReadLine();
                    string[] columnNames = Regex.Split(line, ",");
                    while ((line = csvFile.ReadLine()) != null)
                    {
                        BsonDocument row = new BsonDocument();
                        string[] cols = Regex.Split(line, ",");
                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            row.Add(columnNames[i], cols[i]);
                        }
                        collection.InsertOne(row);
                    }
                    System.Windows.Forms.MessageBox.Show(openFileDialog1.FileName + " correctement chargé dans la base de données", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}


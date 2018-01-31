namespace ESILV.CloudApplication.Problem
{
    using MongoDB.Bson;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;
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
            var queryResult = _mongoDBWrapper.CountLines();
            var win2 = new CountResult(queryResult.ToJson());
            win2.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Debug.WriteLine(openFileDialog1.FileName);
                // where <full path to csv> is the file path, of course
                // StreamReader is IDisposable, so dispose it properly.
                using (var csvFile = new StreamReader(File.OpenRead(@"" + openFileDialog1.FileName + "")))
                {
                    _mongoDBWrapper.ImportCsvIntoCollection(csvFile, Constants.COLLECTION_NAME);
                    System.Windows.Forms.MessageBox.Show(openFileDialog1.FileName + " correctement chargé dans la base de données", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Sort the top 10 most used destinations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var success = int.TryParse(Month1.Text, out int result);
            var queryResult = _mongoDBWrapper.FirstQuery(success ? result : 10);
            var chartWindow = new Chart(queryResult.ToJson());
            chartWindow.Show();
        }

        /// <summary>
        /// Sort the top 10 less used destinations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var success = int.TryParse(Month2.Text, out int result);
            var queryResult = _mongoDBWrapper.SecondQuery(success ? result : 10);
            var chartWindow = new Chart(queryResult.ToJson());
            chartWindow.Show();
        }

        /// <summary>
        /// Map reduce to get the number of km
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var success = int.TryParse(Month3.Text, out int result);
            var queryResult = _mongoDBWrapper.ThirdQuery(success ? result : 1);
            var countResultWindow = new CountResult(queryResult.ToJson(), "value");
            countResultWindow.Show();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var queryResult = _mongoDBWrapper.FifthQuery(!string.IsNullOrEmpty(City1.Text) && City1.Text != "Nom de la ville" ? City1.Text : "Detroit, MI");
            var countResultWindow = new JsonViewer(queryResult.ToJson());
            countResultWindow.Show();
        }
    }
}


using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ESILV.CloudApplication.Problem
{
    /// <summary>
    /// Interaction logic for JsonViewer.xaml
    /// </summary>
    public partial class JsonViewer : Window
    {
        private string _jsonText;
        public JsonViewer(String text)
        {
            InitializeComponent();
            _jsonText = text;
            RootObject rootObject = new RootObject();


            if (_jsonText != "")
            {
                rootObject = JsonConvert.DeserializeObject<RootObject>("{ \"array\": "+_jsonText+" }");
            }
            listViewJson.ItemsSource = rootObject.array;
        }
    }

    public class Process
    {
        public string _id { get; set; }
        public string value { get; set; }
    }

    public class RootObject
    {
        public List<Process> array { get; set; }
    }
}

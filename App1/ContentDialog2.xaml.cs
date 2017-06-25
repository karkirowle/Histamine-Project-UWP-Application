using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    public sealed partial class ContentDialog2 : ContentDialog
    {
        public List<PatientSheet> patientList = new List<PatientSheet> { };
        public string username;
        public string patientToTreat;
        public string patientDisplayName;
        public ContentDialog2()
        {
            this.InitializeComponent();
        }
        public ContentDialog2(string welcomename)
        {
            // Initializes the class with the welcome name
         
            this.InitializeComponent();
            // TODO: display doctor full name
           // textBlock.Text = String.Format("Hello {0}!", username);
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void textBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void patientsuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                var items = patientList.Where(x => x.patientName.Contains(sender.Text));
                //List<string> items = new List<string> { };
                var items2 = new List<string> { };
                if (!items.Any())
                {
                    items2 = new List<string> { "No Results" }; // some no results indication would be useful here
                }
                else
                {
                    items2 = new List<string> { };
                    foreach (var record in items)
                    {
                        items2.Add(record.patientName + ',' + record.patientDOB + ',' +record.patientUser);
                    }
                }
                sender.ItemsSource = items2;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string[] namedob = patientsuggest.Text.Split(',');
            Debug.WriteLine(namedob[0]);
            Debug.WriteLine(namedob[1]);
            PatientSheet Item = patientList.Find(c => (c.patientName == namedob[0]) && (c.patientDOB == namedob[1]));
            patientToTreat = Item.patientUser;
            patientDisplayName = patientsuggest.Text;

            this.Hide();
        }

        
    }
}

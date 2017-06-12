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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    public sealed partial class ContentDialog2 : ContentDialog
    {
        public List<string> patientList = new List<string> { };
        public string username;
        public string patientToTreat;
        public ContentDialog2()
        {
            this.InitializeComponent();
        }
        public ContentDialog2(string welcomename)
        {
            // Initializes the class with the welcome name
         
            this.InitializeComponent();
            textBlock.Text = String.Format("Hello {0}!", username);
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
                var items = patientList.Where(x => x.Contains(sender.Text));
                //List<string> items = new List<string> { };
                if (!items.Any())
                    items = new List<string> { "No Results" };
                sender.ItemsSource = items;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            patientToTreat = patientsuggest.Text;
            this.Hide();
        }
    }
}

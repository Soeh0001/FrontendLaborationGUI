using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryUWP
{
    public sealed partial class DeleteAuthorPage : Page
    {
        private const string BASE_URL = @"http://localhost:3265/"; // För min localhost-adress
        public DeleteAuthorPage()
        {
            this.InitializeComponent();
            LoadAuthors();
        }

        private async void LoadAuthors()
        {
            try
            {
                string URL = BASE_URL + "Authors";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(URL));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var authorList = JsonConvert.DeserializeObject<List<Author>>(content);

                    cmbAuthor.ItemsSource = authorList;
                    cmbAuthor.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                var dialog = new MessageDialog("The connection to the database failed.");
                await dialog.ShowAsync();
            }
        }

        #region DeleteAuthor(DELETE)
        public async System.Threading.Tasks.Task DeleteAuthor()
        {
            try
            {
                HttpClient client = new HttpClient();
                Author selectedAuthor = (Author)cmbAuthor.SelectedItem;

                string URL = BASE_URL + "Authors/" + selectedAuthor.Id;
                var response = await client.DeleteAsync(URL);
                var responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                var dialog = new MessageDialog("The connection to the database failed.");
                await dialog.ShowAsync();
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            await DeleteAuthor();
            var dialog = new MessageDialog("The author are now deleted.");
            await dialog.ShowAsync();
        }
        #endregion

        #region Navigation
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navMain.IsEnabled = this.Frame.CanGoBack;
        }

        private void NavMain_GoBack(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        private bool On_BackRequested()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            return false;
        }

        private void MnuHome_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void MnuAddBook_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddBookPage));
        }

        private void MnuUpdateBook_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UpdateBookPage));
        }

        private void MnuDeleteBook_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DeleteBookPage));
        }

        private void MnuAddAuthor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddAuthorPage));
        }

        private void MnuUpdateAuthor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UpdateAuthorPage));
        }
        #endregion
    }
}

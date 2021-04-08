using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Net.Http;
using Windows.UI.Popups;
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryUWP
{
    public sealed partial class UpdateAuthorPage : Page
    {
        private const string BASE_URL = @"http://localhost:3265/"; // För min localhost-adress
        public UpdateAuthorPage()
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
                    cmbAuthors.ItemsSource = authorList;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                var dialog = new MessageDialog("The connection to the database failed.");
                await dialog.ShowAsync();
            }
        }

        private void CmbAuthors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAuthors.SelectedItem != null)
            {
                Author selectedAuthor = (Author)cmbAuthors.SelectedItem;
                txtFirstname.Text = selectedAuthor.FirstName;
                txtLastname.Text = selectedAuthor.LastName;
                txtCountry.Text = selectedAuthor.Country;
                string books = "";
                foreach (var book in selectedAuthor.Books)
                {
                    books += book.Title + " ";
                }
                txtBook.Text = books;
            }
        }

        #region UpdateAuthor(PUT)
        public async System.Threading.Tasks.Task UpdateAuthor()
        {
            try
            {
                HttpClient client = new HttpClient();

                Author updatedAuthor = (Author)cmbAuthors.SelectedItem;
                updatedAuthor.FirstName = txtFirstname.Text;
                updatedAuthor.LastName = txtLastname.Text;
                updatedAuthor.Country = txtCountry.Text;

                string jsonString = JsonConvert.SerializeObject(updatedAuthor);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                string URL = BASE_URL + "Authors/" + updatedAuthor.Id;
                var response = await client.PutAsync(URL, content);
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

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            await UpdateAuthor();
            var dialog = new MessageDialog("The author are now updated.");
            await dialog.ShowAsync();
            LoadAuthors();
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

        private void MnuDeleteAuthor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DeleteAuthorPage));
        }
        #endregion
    }
}

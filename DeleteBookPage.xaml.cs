using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Net.Http;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryUWP
{
    public sealed partial class DeleteBookPage : Page
    {
        private const string BASE_URL = @"http://localhost:3265/"; // För min localhost-adress
        public DeleteBookPage()
        {
            this.InitializeComponent();
            LoadBooks();
        }

        private async void LoadBooks()
        {
            try
            {
                string URL = BASE_URL + "Books";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(URL));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var bookList = JsonConvert.DeserializeObject<List<Book>>(content);

                    cmbBook.ItemsSource = bookList;
                    cmbBook.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                var dialog = new MessageDialog("The connection to the database failed.");
                await dialog.ShowAsync();
            }
        }

        #region DeleteBook(DELETE)
        public async System.Threading.Tasks.Task DeleteBook()
        {
            try
            {
                HttpClient client = new HttpClient();
                Book selectedBook = (Book)cmbBook.SelectedItem;

                string URL = BASE_URL + "Books/" + selectedBook.Id;
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
            await DeleteBook();
            var dialog = new MessageDialog("The book are now deleted.");
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

        private void MnuAddAuthor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddAuthorPage));
        }

        private void MnuUpdateAuthor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UpdateAuthorPage));
        }

        private void MnuDeleteAuthor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DeleteAuthorPage));
        }
        #endregion
    }
}

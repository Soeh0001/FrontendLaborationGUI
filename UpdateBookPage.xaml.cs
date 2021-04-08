using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryUWP
{
    public sealed partial class UpdateBookPage : Page
    {
        private const string BASE_URL = @"http://localhost:3265/"; // För min localhost-adress
        public UpdateBookPage()
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
                    cmbBooks.ItemsSource = bookList;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                var dialog = new MessageDialog("The connection to the database failed.");
                await dialog.ShowAsync();
            }
        }

        private void CmbBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBooks.SelectedItem != null)
            {
                Book selectedBook = (Book)cmbBooks.SelectedItem;
                txtTitle.Text = selectedBook.Title;
                txtLanguage.Text = selectedBook.Language;
                txtYear.Text = selectedBook.Published.ToString();
                txtGenre.Text = selectedBook.Genre;
                string authors = "";
                foreach (var author in selectedBook.Authors)
                {
                    authors += author.FirstName + " " + author.LastName + " ";
                }
                txtAuthor.Text = authors;
            }
        }

        #region UpdateBook(PUT)
        public async System.Threading.Tasks.Task UpdateBook()
        {
            try
            {
                HttpClient client = new HttpClient();

                Book updatedBook = (Book)cmbBooks.SelectedItem;
                updatedBook.Title = txtTitle.Text;
                updatedBook.Language = txtLanguage.Text;
                updatedBook.Genre = txtGenre.Text;
                updatedBook.Published = Int32.Parse(txtYear.Text);


                string jsonString = JsonConvert.SerializeObject(updatedBook);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                string URL = BASE_URL + "Books/" + updatedBook.Id;
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
            await UpdateBook();
            var dialog = new MessageDialog("The book are now updated.");
            await dialog.ShowAsync();
            LoadBooks();
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

        private void MnuDeleteAuthor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DeleteAuthorPage));
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryUWP
{
    public sealed partial class AddBookPage : Page
    {
        private const string BASE_URL = @"http://localhost:3265/"; // För min localhost-adress
        public AddBookPage()
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

                    cmbAuthor.ItemsSource = GetPresentationList(authorList);
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

        private List<AuthorPresentation> GetPresentationList(List<Author> authorList)
        {
            List<AuthorPresentation> returnList = new List<AuthorPresentation>();
            foreach (var author in authorList)
            {
                AuthorPresentation presentationData = new AuthorPresentation { Id = author.Id, FirstName = author.FirstName, LastName = author.LastName };
                returnList.Add(presentationData);
            }
            return returnList;
        }
        
        #region AddBook(POST)
        public async System.Threading.Tasks.Task AddBook()
        {
            try
            {
                HttpClient client = new HttpClient();
                AuthorPresentation selectedAuthor = (AuthorPresentation)cmbAuthor.SelectedItem;
                Author bookAuthor = new Author { Id = selectedAuthor.Id, FirstName = selectedAuthor.FirstName, LastName = selectedAuthor.LastName, Country = selectedAuthor.Country }; // ToDo: Glöm ej Country

                Book newBook = new Book();
                newBook.Title = txtTitle.Text;
                newBook.Language = txtLanguage.Text;
                newBook.Genre = txtGenre.Text;
                newBook.Published = Int32.Parse(txtYear.Text);
                newBook.Authors = new List<Author>();
                newBook.Authors.Add(bookAuthor);

                string jsonString = JsonConvert.SerializeObject(newBook);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                string URL = BASE_URL + "Books";
                var response = await client.PostAsync(URL, content);
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

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            await AddBook();
            var dialog = new MessageDialog("The book are now created and saved.");
            await dialog.ShowAsync();
        }
        #endregion

        #region Navigation
        // "Gå tillbaka"-funktion https://docs.microsoft.com/sv-se/windows/uwp/design/basics/navigation-history-and-backwards-navigation (hämtat 7 oktober 2020)
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

        private void MnuDeleteAuthor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DeleteAuthorPage));
        }
        #endregion
    }
}

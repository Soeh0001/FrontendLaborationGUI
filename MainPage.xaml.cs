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
using System.Net.Http;
using Newtonsoft.Json;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
 
namespace LibraryUWP
{
    // Sofia Forsberg Ehn - Avancerad C# Laboration i GUI
    public sealed partial class MainPage : Page
    {
        private const string BASE_URL = @"http://localhost:3265/"; // För min localhost-adress till WebbAPI2 (LibraryAPI)
        public MainPage()
        {
            this.InitializeComponent();
        }

        #region ReadBooks(GET)
        // Genom listan av Books som visas i vyn visar jag även upp alla de Authors som är kopplade till ett specifikt Book-objekt.
        // På så sätt tänker jag att jag även visar upp Authors (även om dem inte visas upp via en egen lista).
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadBooks();
        }

        public async void LoadBooks()
        {
            try
            {
                prgRing.IsActive = true;
                string URL = BASE_URL + "Books";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(URL));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var bookList = JsonConvert.DeserializeObject<List<Book>>(content);

                    lstBooks.ItemsSource = bookList;
                    prgRing.IsActive = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                var dialog = new MessageDialog("The connection to the database failed.");
                await dialog.ShowAsync();
            }
        }
        #endregion

        #region Navigation
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

        private void MnuDeleteAuthor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DeleteAuthorPage));
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PointsOfInterest
{
    /// <summary>
    /// Interaction logic for Places.xaml
    /// </summary>
    public partial class Places : Window
    {
        public Places()
        {
            InitializeComponent();

            var isAdmin = this.IsAdmin();
            if (!isAdmin)
            {
                this.HideLabels();
            }

            places.ItemsSource = LoadCollectionData();
        }

        private bool IsAdmin()
        {
            try
            {
                string currentUserEmail = ConfigurationManager.AppSettings["CurrentUser"];

                using (var db = new PointsOfInterestContext())
                {
                    var currentUser = db.Users.SingleOrDefault(x => x.Email == currentUserEmail);
                    var isAdmin = currentUser.IsAdmin ?? false;

                    if (isAdmin)
                    {
                        return true;
                    }
                }
            }
            catch
            {

            }


            return false;
        }

        private void HideLabels()
        {
            PlaceName.Visibility = Visibility.Hidden;
            PlaceDes.Visibility = Visibility.Hidden;
            PlaceImageName.Visibility = Visibility.Hidden;
            AddPlace.Visibility = Visibility.Hidden;
            NameLabel.Visibility = Visibility.Hidden;
            DesLabel.Visibility = Visibility.Hidden;
            ImgLabel.Visibility = Visibility.Hidden;
            DeletePlace.Visibility = Visibility.Hidden;
        }

        private List<Place> LoadCollectionData()
        {
            var placesToReturn = new List<Place>();
            using (var db = new PointsOfInterestContext())
            {
                placesToReturn = db.Places
                    .Where(x => x.IsDeleted == false || x.IsDeleted == null)
                    .ToList();

                foreach (var item in placesToReturn)
                {
                    try
                    {
                        item.Rate = item.Rates.Average(x => x.RateValue);
                    }
                    catch
                    {
                        item.Rate = 0.00m;
                    }
                }
            }

            return placesToReturn;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = places.SelectedItem as Place;
            
            var page = new PlaceWindow(selectedItem.Id.ToString());
            page.Show();

            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var placeName = PlaceName.Text;
            var placeDes = PlaceDes.Text;
            var imageName = PlaceImageName.Text;

            if (!String.IsNullOrEmpty(placeName) && !String.IsNullOrEmpty(placeDes)
                && !String.IsNullOrEmpty(imageName))
            {
                var place= new Place();
                place.PlaceName = placeName;
                place.Descripiton = placeDes;
                place.ImageUrl = imageName;

                using (var db = new PointsOfInterestContext())
                {
                    db.Places.Add(place);

                    db.SaveChanges();
                }

                PlaceName.Text = "";
                PlaceDes.Text = "";
                PlaceImageName.Text = "";
            }
            else
            {
                MessageBox.Show("Name, Description, Image Name cannot be empty");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = places.SelectedItem as Place;
                using (var db = new PointsOfInterestContext())
                {
                    var deletedItem = db.Places.SingleOrDefault(x => x.Id == selectedItem.Id);
                    deletedItem.IsDeleted = true;

                    db.SaveChanges();
                }
            }
            catch
            {

            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var searchTerm = SearchTerm.Text.ToLower();

            var placesToReturn = new List<Place>();

            using (var db = new PointsOfInterestContext())
            {
                placesToReturn = db.Places
                    .Where(x => (x.IsDeleted == false || x.IsDeleted == null)
                    && x.PlaceName.ToLower().Contains(searchTerm))
                    .ToList();

                foreach (var item in placesToReturn)
                {
                    try
                    {
                        item.Rate = item.Rates.Average(x => x.RateValue);
                    }
                    catch
                    {
                        item.Rate = 0.00m;
                    }
                }
            }

            places.ItemsSource = placesToReturn;
            places.Items.Refresh();

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            window.Show();
            this.Close();
        }
    }
}

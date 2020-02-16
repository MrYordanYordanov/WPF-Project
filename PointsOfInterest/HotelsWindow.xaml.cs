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
    /// Interaction logic for HotelsWindow.xaml
    /// </summary>
    public partial class HotelsWindow : Window
    {
        public HotelsWindow()
        {
            InitializeComponent();

            var isAdmin = this.IsAdmin();
            if (!isAdmin)
            {
                this.HideLabels();
            }

            hotels.ItemsSource = LoadCollectionData();
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
            HotelName.Visibility = Visibility.Hidden;
            HotelDes.Visibility = Visibility.Hidden;
            HotelImageName.Visibility = Visibility.Hidden;
            AddHotel.Visibility = Visibility.Hidden;
            NameLabel.Visibility = Visibility.Hidden;
            DesLabel.Visibility = Visibility.Hidden;
            ImgLabel.Visibility = Visibility.Hidden;
            DeleteHotel.Visibility = Visibility.Hidden;
            PlaceLabel.Visibility = Visibility.Hidden;
            PriceLabel.Visibility = Visibility.Hidden;
            HotelPlace.Visibility = Visibility.Hidden;
            HotelPrice.Visibility = Visibility.Hidden;
        }

        private List<Hotel> LoadCollectionData()
        {
            var hotelsToReturn = new List<Hotel>();
            using (var db = new PointsOfInterestContext())
            {
                hotelsToReturn = db.Hotels
                    .Where(x => x.IsDeleted == false || x.IsDeleted==null)
                    .ToList();
                
                foreach (var item in hotelsToReturn)
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

            return hotelsToReturn;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = hotels.SelectedItem as Hotel;

            var page = new HotelWindow(selectedItem.Id.ToString());
            page.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var name = HotelName.Text;
            var des = HotelDes.Text;
            var imageName = HotelImageName.Text;
            var place = HotelPlace.Text;
            var price = HotelPrice.Text;

            var parsedPriceNumber = 0.00m;

            if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(des)
                && !String.IsNullOrEmpty(imageName) && !String.IsNullOrEmpty(place)
                && !String.IsNullOrEmpty(price))
            {
                var parsedPrice = Decimal.TryParse(price, out parsedPriceNumber);

                if (parsedPrice)
                {
                    if (parsedPriceNumber < 0)
                    {
                        MessageBox.Show("Price cannot be a negative number");
                    }
                    else
                    {
                        var hotel = new Hotel();
                        hotel.HotelName = name;
                        hotel.Descripiton = des;
                        hotel.ImageUrl = imageName;
                        hotel.Place = place;
                        hotel.Price = parsedPriceNumber;

                        using (var db = new PointsOfInterestContext())
                        {
                            db.Hotels.Add(hotel);

                            db.SaveChanges();
                        }

                        HotelName.Text = "";
                        HotelDes.Text = "";
                        HotelImageName.Text = "";
                        HotelPlace.Text = "";
                        HotelPrice.Text = "";
                    }
                    
                }
                else
                {
                    MessageBox.Show("Price must be a number");
                }
            }
            else
            {
                MessageBox.Show("Name, Description, Image Name, Place and Price cannot be empty");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = hotels.SelectedItem as Hotel;
                using (var db = new PointsOfInterestContext())
                {
                    var deletedHotel = db.Hotels.SingleOrDefault(x => x.Id == selectedItem.Id);
                    deletedHotel.IsDeleted = true;

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

            var placesToReturn = new List<Hotel>();

            using (var db = new PointsOfInterestContext())
            {
                placesToReturn = db.Hotels
                    .Where(x => (x.IsDeleted == false || x.IsDeleted == null)
                    && x.HotelName.ToLower().Contains(searchTerm))
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

            hotels.ItemsSource = placesToReturn;
            hotels.Items.Refresh();

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            window.Show();
            this.Close();
        }
    }
}

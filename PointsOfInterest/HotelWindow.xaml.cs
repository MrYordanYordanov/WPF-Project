using System;
using System.Collections.Generic;
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
    /// Interaction logic for Museum.xaml
    /// </summary>
    public partial class HotelWindow : Window
    {
        public Hotel Hotl { get; set; }

        public HotelWindow()
        {
            InitializeComponent();
        }

        public HotelWindow(string id)
        {
            InitializeComponent();
            this.LoadData(id);
        }

        private void LoadData(string id)
        {
            try
            {
                var idToNumber = int.Parse(id);
                using (var db = new PointsOfInterestContext())
                {
                    var hotel = db.Hotels.SingleOrDefault(x => x.Id == idToNumber);

                    try
                    {
                        hotel.Rate = hotel.Rates.Average(x => x.RateValue);
                    }
                    catch
                    {
                        hotel.Rate = 0.00m;
                    }

                    if (hotel == null)
                    {
                        throw new ArgumentNullException("Invalid hotel Id");
                    }
                    else
                    {
                        this.Hotl = hotel;

                        Rate.Content = "Average Rate: " + hotel.Rate;
                        HotelDes.Content = hotel.Descripiton;
                        HotelName.Content = hotel.HotelName;
                        HotelPlace.Content = hotel.Place;
                        HotelPrice.Content = "Price: " + hotel.Price;

                        var dir = System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString();
                        var path = System.IO.Path.GetDirectoryName(dir);
                        var imagePath = System.IO.Path.Combine(path + "/Images/Hotels/" + hotel.ImageUrl + ".jpg");
                        //Uri resourceUri = new Uri(imagePath, UriKind.Relative);
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(imagePath);
                        bitmap.EndInit();
                        HotelImg.Source = bitmap;
                    }
                }
            }
            catch
            {

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var parsedPriceNumber = 0.00m;

            var parsedPrice = Decimal.TryParse(HotelRate.Text, out parsedPriceNumber);
            if (parsedPrice)
            {
                if (parsedPriceNumber < 1 || parsedPriceNumber > 5)
                {
                    MessageBox.Show("rate must be between 1 and 5");
                }
                else
                {
                    using (var db = new PointsOfInterestContext())
                    {
                        var currentRate = new Rate();
                        currentRate.RateValue = parsedPriceNumber;

                        var hotel = db.Hotels.SingleOrDefault(x => x.Id == this.Hotl.Id);
                        hotel.Rates.Add(currentRate);

                        db.SaveChanges();
                    }
                }
            }
            else
            {
                MessageBox.Show("rate must be a number");
            }

            HotelRate.Text = "";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var commentText = CommentVal.Text;

            if (!string.IsNullOrEmpty(commentText))
            {
                using (var db = new PointsOfInterestContext())
                {
                    var comment = new Comment();
                    comment.Name = commentText;

                    var hotel = db.Hotels.SingleOrDefault(x => x.Id == this.Hotl.Id);
                    hotel.Comments.Add(comment);

                    db.SaveChanges();
                }
            }

            CommentVal.Text = "";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var comments = new List<Comment>();
            using (var db = new PointsOfInterestContext())
            {
                var hotel = db.Hotels.SingleOrDefault(x => x.Id == this.Hotl.Id);
                comments = hotel.Comments.ToList();
            }
                var commentWindow = new CommentWindow(comments);
            commentWindow.Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var window = new HotelsWindow();
            window.Show();
            this.Close();
        }
    }
}

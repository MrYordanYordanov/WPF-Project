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
    /// Interaction logic for PlaceWindow.xaml
    /// </summary>
    public partial class PlaceWindow : Window
    {
        public Place Plc { get; set; }

        public PlaceWindow()
        {
            InitializeComponent();
        }

        public PlaceWindow(string id)
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
                    var place = db.Places.SingleOrDefault(x => x.Id == idToNumber);

                    try
                    {
                        place.Rate = place.Rates.Average(x => x.RateValue);
                    }
                    catch
                    {
                        place.Rate = 0.00m;
                    }


                    if (place == null)
                    {
                        throw new ArgumentNullException("Invalid place Id");
                    }
                    else
                    {
                        this.Plc = place;

                        PlRate.Content = "Average Rate: " + place.Rate;
                        PlaceDes.Content = place.Descripiton;
                        PlaceName.Content = place.PlaceName;

                        var dir = System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString();
                        var path = System.IO.Path.GetDirectoryName(dir);
                        var imagePath = System.IO.Path.Combine(path + "/Images/Places/" + place.ImageUrl + ".jpg");
                        //Uri resourceUri = new Uri(imagePath, UriKind.Relative);
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(imagePath);
                        bitmap.EndInit();
                        PlaceImg.Source = bitmap;
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

            var parsedPrice = Decimal.TryParse(PlaceRate.Text, out parsedPriceNumber);

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

                        var place = db.Places.SingleOrDefault(x => x.Id == this.Plc.Id);
                        place.Rates.Add(currentRate);

                        db.SaveChanges();
                    }
                }
            }
            else
            {
                MessageBox.Show("rate must be a number");
            }

            PlaceRate.Text = "";

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

                    var place = db.Places.SingleOrDefault(x => x.Id == this.Plc.Id);
                    place.Comments.Add(comment);

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
                var place = db.Places.SingleOrDefault(x => x.Id == this.Plc.Id);
                comments = place.Comments.ToList();
            }
            var commentWindow = new CommentWindow(comments);
            commentWindow.Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var window = new Places();
            window.Show();
            this.Close();
        }
    }
}

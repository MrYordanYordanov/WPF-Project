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
    public partial class MuseumWindow : Window
    {
        public Museum Mus { get; set; }

        public MuseumWindow()
        {
            InitializeComponent();
        }

        public MuseumWindow(string id)
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
                    var museum = db.Museums.SingleOrDefault(x => x.Id == idToNumber);

                    try
                    {
                        museum.AverageRate = museum.Rates.Average(x => x.RateValue);
                    }
                    catch
                    {
                        museum.AverageRate = 0.00m;
                    }

                    if (museum == null)
                    {
                        throw new ArgumentNullException("Invalid museum Id");
                    }
                    else
                    {
                        this.Mus = museum;

                        MuseumRate.Content = "Average Rate: " + museum.AverageRate;
                        MusDes.Content = museum.Descripiton;
                        MusName.Content = museum.MuseumName;

                        var dir = System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString();
                        var path = System.IO.Path.GetDirectoryName(dir);
                        var imagePath = System.IO.Path.Combine(path + "/Images/Museums/" + museum.ImageUrl + ".jpg");
                        //Uri resourceUri = new Uri(imagePath, UriKind.Relative);
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(imagePath);
                        bitmap.EndInit();
                        MusImg.Source = bitmap;
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

            var parsedPrice = Decimal.TryParse(MusRate.Text, out parsedPriceNumber);

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

                        var museum = db.Museums.SingleOrDefault(x => x.Id == this.Mus.Id);
                        museum.Rates.Add(currentRate);

                        db.SaveChanges();
                    }
                }
            }
            else
            {
                MessageBox.Show("rate must be a number");
            }

            MusRate.Text = "";


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

                    var museum = db.Museums.SingleOrDefault(x => x.Id == this.Mus.Id);
                    museum.Comments.Add(comment);

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
                var museum = db.Museums.SingleOrDefault(x => x.Id == this.Mus.Id);
                comments = museum.Comments.ToList();
            }
            var commentWindow = new CommentWindow(comments);
            commentWindow.Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var window = new Museums();
            window.Show();
            this.Close();
        }
    }
}

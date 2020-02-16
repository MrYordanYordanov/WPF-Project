using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Windows;

namespace PointsOfInterest
{
    /// <summary>
    /// Interaction logic for Museums.xaml
    /// </summary>
    public partial class Museums : Window
    {
        public Museums()
        {
            InitializeComponent();
            
            var isAdmin=this.IsAdmin();
            if (!isAdmin)
            {
                this.HideLabels();
            }
           
            museums.ItemsSource = LoadCollectionData();

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
            MusName.Visibility = Visibility.Hidden;
            MusDes.Visibility = Visibility.Hidden;
            MusImageName.Visibility = Visibility.Hidden;
            AddMus.Visibility = Visibility.Hidden;
            NameLabel.Visibility = Visibility.Hidden;
            DesLabel.Visibility = Visibility.Hidden;
            Imglabel.Visibility = Visibility.Hidden;
            DeleteMus.Visibility = Visibility.Hidden;
        }

        private List<Museum> LoadCollectionData()
        {
            var museumsToReturn = new List<Museum>();
            using (var db = new PointsOfInterestContext())
            {
                museumsToReturn = db.Museums
                    .Where(x => x.IsDeleted == false || x.IsDeleted == null)
                    .ToList();

                foreach (var item in museumsToReturn)
                {
                    try
                    {
                        item.AverageRate = item.Rates.Average(x => x.RateValue);
                    }
                    catch
                    {
                        item.AverageRate = 0.00m;
                    }
                }
            }

            return museumsToReturn;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = museums.SelectedItem as Museum;

            var page = new MuseumWindow(selectedItem.Id.ToString());
            page.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var musName = MusName.Text;
            var musDes = MusDes.Text;
            var imageName = MusImageName.Text;

            if (!String.IsNullOrEmpty(musName) && !String.IsNullOrEmpty(musDes)
                && !String.IsNullOrEmpty(imageName))
            {
                var museum = new Museum();
                museum.MuseumName = musName;
                museum.Descripiton = musDes;
                museum.ImageUrl = imageName;

                using (var db = new PointsOfInterestContext())
                {
                    db.Museums.Add(museum);

                    db.SaveChanges();
                }

                MusName.Text = "";
                MusDes.Text = "";
                MusImageName.Text = "";
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
                var selectedItem = museums.SelectedItem as Museum;
                using (var db = new PointsOfInterestContext())
                {
                    var deletedMuseum = db.Museums.SingleOrDefault(x => x.Id == selectedItem.Id);
                    deletedMuseum.IsDeleted = true;

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

            var placesToReturn = new List<Museum>();

            using (var db = new PointsOfInterestContext())
            {
                placesToReturn = db.Museums
                    .Where(x => (x.IsDeleted == false || x.IsDeleted == null)
                    && x.MuseumName.ToLower().Contains(searchTerm))
                    .ToList();

                foreach (var item in placesToReturn)
                {
                    try
                    {
                        item.AverageRate = item.Rates.Average(x => x.RateValue);
                    }
                    catch
                    {
                        item.AverageRate = 0.00m;
                    }
                }
            }

            museums.ItemsSource = placesToReturn;
            museums.Items.Refresh();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            window.Show();
            this.Close();
        }
    }
}

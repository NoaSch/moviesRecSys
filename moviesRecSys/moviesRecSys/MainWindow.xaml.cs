using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace moviesRecSys
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RecSys system;
        string movieToRecommend;
        private List<string> results; 
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            system = new RecSys();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            movieToRecommend = textBox.Text;
            //  how many movies to return in the result- if the algorithm finds less movies, it will retrun less
            int numOfMoviesToReturn = 8;

            //matches- string =the name of the movies,  double=  The match between the two movies-
            //Any movie in the data base with  movieToRecommend. 1 the best match. -1 worst  match. 0 There are no people who recommended the two movies. 
            if (system.checkMovieInDB(movieToRecommend))
            {
                Dictionary<string, double> matches = system.TopMatches(movieToRecommend, numOfMoviesToReturn);
                results = new List<string>();
                foreach (var item in matches)
                {
                    results.Add(item.Key);
                }

                listView.ItemsSource = results;

            }
            else
                MessageBox.Show("The movie does not exists in the system");



        }



    }
}

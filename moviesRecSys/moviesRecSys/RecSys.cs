using System;
using System.Collections.Generic;
using System.Linq;


namespace moviesRecSys
{
    public class RecSys
    {
        //The 
         Dictionary<string, List<Recommendation>> movietRecommendations = new Dictionary<string, List<Recommendation>>();

        public RecSys()
        {
            Init();

        }

        //for noga and eli
        //here you should load the DB.
        //for each movie there is list of recommendations/
        /// <summary>
        /// each recomendation contain the name of the user and the rating
        /// פשוט תשנו את הפונצקציה הזו לפי הקבצי DB
        /// </summary>
        /// 
        
        internal void Init()
        {
            List<Recommendation> list = new List<Recommendation>();
            list.Add(new Recommendation() { Name = "Wile E Coyote", Rating = 4.5 });//each user rating
            list.Add(new Recommendation() { Name = "Bugs Bunny", Rating = 2.5 });
            list.Add(new Recommendation() { Name = "Elmer Fudd", Rating = 5.0 });
            list.Add(new Recommendation() { Name = "Foghorn Leghorn", Rating = 2.0 });
            movietRecommendations.Add("ACME Industrial Rocket Pack", list);//The Name of the movie


            list = new List<Recommendation>();
            list.Add(new Recommendation() { Name = "Wile E Coyote", Rating = 5.0 });
            list.Add(new Recommendation() { Name = "Bugs Bunny", Rating = 3.5 });
            list.Add(new Recommendation() { Name = "Elmer Fudd", Rating = 1.0 });
            list.Add(new Recommendation() { Name = "Foghorn Leghorn", Rating = 3.5 });
            list.Add(new Recommendation() { Name = "Daffy Duck", Rating = 1.0 });
            movietRecommendations.Add("ACME Super Sling Shot", list);

            list = new List<Recommendation>();
            list.Add(new Recommendation() { Name = "Wile E Coyote", Rating = 1.0 });
            list.Add(new Recommendation() { Name = "Bugs Bunny", Rating = 3.5 });
            list.Add(new Recommendation() { Name = "Elmer Fudd", Rating = 5.0 });
            list.Add(new Recommendation() { Name = "Foghorn Leghorn", Rating = 4.0 });
            list.Add(new Recommendation() { Name = "Daffy Duck", Rating = 4.0 });
            movietRecommendations.Add("ACME X-Large Catapult", list);

            list = new List<Recommendation>();
            list.Add(new Recommendation() { Name = "Bugs Bunny", Rating = 3.5 });
            list.Add(new Recommendation() { Name = "Elmer Fudd", Rating = 4.0 });
            list.Add(new Recommendation() { Name = "Foghorn Leghorn", Rating = 5.0 });
            list.Add(new Recommendation() { Name = "Daffy Duck", Rating = 2.5 });
            movietRecommendations.Add("ACME Super Glue", list);

            list = new List<Recommendation>();
            list.Add(new Recommendation() { Name = "Wile E Coyote", Rating = 4.5 });
            list.Add(new Recommendation() { Name = "Bugs Bunny", Rating = 5.0 });
            list.Add(new Recommendation() { Name = "Foghorn Leghorn", Rating = 3.0 });
            GeMovieRecommendations().Add("ACME Jet Powered Roller Skates", list);


        }
        

        //return the list of recommendations
        private Dictionary<string, List<Recommendation>> GeMovieRecommendations()
        {
            return movietRecommendations;
        }

        //check if the movie is in the system
        public bool checkMovieInDB(String movieName)
        {
            if (movietRecommendations.ContainsKey(movieName))
                return true;
            else return false; 

        }

        //return the X best similar movies
        public Dictionary<string, double> TopMatches(string name, int numOfBestResults)
        {
            // grab of list of products that *excludes* the item we're searching for
            var sortedList = movietRecommendations.Where(x => x.Key != name);

            sortedList.OrderByDescending(x => x.Key);

            Dictionary<string, double> sortedRecommendationsDic = new Dictionary<string, double>();
            List<Recommendation> recommendations = new List<Recommendation>();


            // go through the list and calculate the Pearson score for each product
            foreach (var entry in sortedList)
            {
                sortedRecommendationsDic.Add(entry.Key, CalculatePearsonCorrelation(name, entry.Key));
                //recommendations.Add(new Recommendation() { Name = entry.Key, Rating = CalculatePearsonCorrelation(name, entry.Key) });
            }

           
            var topX = sortedRecommendationsDic.OrderByDescending(pair => pair.Value).Take(numOfBestResults);
            var bottomX = sortedRecommendationsDic.OrderBy(pair => pair.Value).Take(numOfBestResults);
            var result  = (Dictionary<string, double>)sortedRecommendationsDic.OrderByDescending(pair => pair.Value).Take(numOfBestResults)
               .ToDictionary(pair => pair.Key, pair => pair.Value);
            return result;
        }

        //Calculate peason correlation
        double CalculatePearsonCorrelation(string movie1, string movie2)
        {
            List<Recommendation> shared_items = new List<Recommendation>();

            // collect a list of products have have reviews in common
            foreach (var item in movietRecommendations[movie1])
            {
                if (movietRecommendations[movie2].Where(x => x.Name == item.Name).Count() != 0)
                {
                    shared_items.Add(item);
                }
            }

            if (shared_items.Count == 0)
            {
                // they have nothing in common exit with a zero
                return 0;
            }

            // sum up all the preferences
            double movie1_review_sum = 0.00f;
            foreach (Recommendation item in shared_items)
            {
                movie1_review_sum += movietRecommendations[movie1].Where(x => x.Name == item.Name).FirstOrDefault().Rating;
            }

            double movie2_review_sum = 0.00f;
            foreach (Recommendation item in shared_items)
            {
                movie2_review_sum += movietRecommendations[movie2].Where(x => x.Name == item.Name).FirstOrDefault().Rating;
            }

            // sum up the squares
            double movie1_rating = 0f;
            double movie2_rating = 0f;
            foreach (Recommendation item in shared_items)
            {
                movie1_rating += Math.Pow(movietRecommendations[movie1].Where(x => x.Name == item.Name).FirstOrDefault().Rating, 2);
                movie2_rating += Math.Pow(movietRecommendations[movie2].Where(x => x.Name == item.Name).FirstOrDefault().Rating, 2);
            }

            //sum up the products
            double critics_sum = 0f;
            foreach (Recommendation item in shared_items)
            {
                critics_sum += movietRecommendations[movie1].Where(x => x.Name == item.Name).FirstOrDefault().Rating *
                                movietRecommendations[movie2].Where(x => x.Name == item.Name).FirstOrDefault().Rating;

            }

            //calculate pearson score
            double num = critics_sum - (movie1_review_sum * movie2_review_sum / shared_items.Count);

            double density = (double)Math.Sqrt((movie1_rating - Math.Pow(movie1_review_sum, 2) / shared_items.Count) * ((movie2_rating - Math.Pow(movie2_review_sum, 2) / shared_items.Count)));

            if (density == 0)
                return 0;

            return num / density;
        }


    }
}


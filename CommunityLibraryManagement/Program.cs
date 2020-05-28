using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using LibraryManagement;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace CommunityLibrary
{
    class Program
    {
        public static MovieCollection movieCollection = new MovieCollection();
        public static Member[] members = new Member[10];
        public static Movie[] movieArray = new Movie[25]; // assuming a maximum of 25 movies in the system
        
        static void DisplayMainMenu()
        {
            // display main menu
            Console.WriteLine("Welcome to the Community Library");
            Console.WriteLine("=========== Main Menu ===========");
            Console.WriteLine("1. Staff Login");
            Console.WriteLine("2. Member Login");
            Console.WriteLine("0. Exit");
            Console.WriteLine("=================================");
            Console.WriteLine("Please make a selection (1-2, or 0 to exit):");
        }

        static void DisplayStaffLogin()
        {
            string username;
            string password;

            bool attempted = false;

            do {
                Console.Clear();

                // error message if user input was incorrect
                if (attempted)
                {
                    Console.WriteLine("Incorrect username or password. Insert 0 to return to the main menu or ENTER to try again.");
                    string response = Console.ReadLine();

                    if (response == "0")
                    {
                        FunctionalMainMenu();
                    }
                }

                // input username
                Console.Write("Please enter your staff username: ");
                username = Console.ReadLine().ToString();

                // input password 
                Console.Write("Please enter your staff password: ");
                password = Console.ReadLine().ToString();

                attempted = true; // counter to indicate that the error message should be shown next time (if input was incorrect)

            } while (username != "staff" || password != "today123"); // default username and password for all staff members

            FunctionalStaffMenu();
        }

        static void DisplayStaffMenu()
        {
            // display staff menu
            Console.WriteLine("=========== Staff Menu ===========");
            Console.WriteLine("1. Add a new movie DVD");
            Console.WriteLine("2. Remove a movie DVD");
            Console.WriteLine("3. Register a new member");
            Console.WriteLine("4. Find a registered member's phone number"); 
            Console.WriteLine("0. Return to main menu");
            Console.WriteLine("==================================");
            Console.WriteLine("Please make a selection (1-4 or 0 to return to main menu)");
        }

        static void AddMovie()
        {
            // Clear the console
            Console.Clear();

            // Entering movie details

            // heading
            Console.WriteLine("1. Add a new movie");
            Console.WriteLine("");

            // insert movie title
            Console.Write("Movie title: ");
            string ttl = Console.ReadLine().ToString();
            ttl = ttl.Trim(); // remove spaces at the start and end of string

            // insert starring actors
            // -- need to account for no input
            // -- need to account for double comma
            Console.Write("Starring actors (separated by commas): ");
            string star = Console.ReadLine().ToString(); 
            string[] starArray = star.Split(','); // split according to the commas
            for (int i = 0; i < starArray.Length; i++)
            {
                starArray[i] = starArray[i].Trim(); // remove spaces at the start and end of string
            }

            // insert director
            Console.Write("Director: ");
            string dir = Console.ReadLine().ToString();
            dir = dir.Trim(); // remove spaces at the start and end of string

            // insert duration
            Console.Write("Duration (min): ");
            string durInput = Console.ReadLine();
            int dur;
            while (!int.TryParse(durInput, out dur))
            {
                Console.Write("Error: Duration must be an integer. Please input the movie's duration in minutes: ");
                durInput = Console.ReadLine();
            }

            // insert genre
            Console.WriteLine("Choose the movie's genre from the selection below (0-8):");
            Console.WriteLine("0. Drama");
            Console.WriteLine("1. Adventure");
            Console.WriteLine("2. Family");
            Console.WriteLine("3. Action");
            Console.WriteLine("4. Sci Fi");
            Console.WriteLine("5. Comedy");
            Console.WriteLine("6. Animated");
            Console.WriteLine("7. Thriller");
            Console.WriteLine("8. Other");

            Movie.Genre gen;

            if (int.TryParse(Console.ReadLine(), out int genInput))
            {
                if (Enum.IsDefined(typeof(Movie.Genre), genInput))
                {
                    gen = (Movie.Genre)genInput;
                } 
                else
                {
                    gen = 0;
                }
            }
            else
            {
                gen = 0;
            }

            // insert classification
            Console.WriteLine("Choose the movie's classification from the selection below (0-3):");
            Console.WriteLine("0. General (G)");
            Console.WriteLine("1. Parental Guidance (PG)");
            Console.WriteLine("2. Mature (M)");
            Console.WriteLine("3. Mature Acccompanied (MA)");

            Movie.Classification classif;

            if (int.TryParse(Console.ReadLine(), out int classifInput))
            {
                if (Enum.IsDefined(typeof(Movie.Classification), classifInput))
                {
                    classif = (Movie.Classification)classifInput;
                } 
                else
                {
                    classif = 0;
                }
            }
            else
            {
                classif = 0;
            }

            // insert release date
            Console.Write("Release Date (dd-mm-yyyy): ");
            string relDate = Console.ReadLine().ToString();

            // insert available copies
            Console.Write("Available copies: ");
            string copyInput = Console.ReadLine();
            int availCopies;
            while (!int.TryParse(copyInput, out availCopies))
            {
                Console.Write("Error: Number of available copies must be an integer. Please try again: ");
                copyInput = Console.ReadLine();
            }

            // use all user input to create a Movie instance
            Movie addedMovie = new Movie(ttl, starArray, dir, dur, gen, classif, relDate, availCopies); // add available copies

            bool status = MovieCollection.AddMovieToTree(movieCollection, addedMovie);

            // if the movie was successfully added to the BST
            if (status)
            {
                Console.WriteLine();
                Console.WriteLine("Movie successfully added. Press any key to return to the staff menu.");
                Console.ReadKey();
            } 
            else
            {
                Console.WriteLine();
                Console.WriteLine("Movie not added. Press any key to return to the staff menu.");
                Console.ReadKey();                 
            }
        }

        static void RemoveMovie()
        {
            Console.Clear();
            Console.WriteLine("Input the name of the movie you would like removed:");
            string input = Console.ReadLine();

            if (MovieCollection.FindMovieInTree(input) != null)
            {
                Movie removedMovie = MovieCollection.FindMovieInTree(input).data;

                if (removedMovie.TotalCopies != removedMovie.CopiesAvailable)
                {
                    Console.WriteLine("All movie copies must be returned before removing a movie from the system. Please try again later.");
                }
                else
                {
                    MovieCollection.RemoveMovieFromTree(removedMovie);
                    Console.WriteLine(removedMovie.Title + " was successfully removed. Press any key to return to the staff menu.");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Sorry, there are no records of " + input + " in our movie system.");
                Console.ReadKey();
            }


        }

        static void RegisterMember()
        {
            // Clear the console
            Console.Clear();

            // Entering member details
            Console.WriteLine("3. Register a new member");
            Console.WriteLine("");
            
            Console.Write("First name: ");
            string fname = Console.ReadLine().ToString();

            Console.Write("Last name: ");
            string lname = Console.ReadLine().ToString();
            
            Console.Write("Address: ");
            string addr = Console.ReadLine().ToString();
            
            Console.Write("Phone number: ");
            string num = Console.ReadLine().ToString();

            Member addedMember = new Member(fname, lname, addr, num);
            bool addStatus = MemberCollection.AddMemberToArray(addedMember, members);

            if (addStatus)
            {
                Console.WriteLine();
                Console.WriteLine("Member registration successful.");
                Console.WriteLine("The member's username is " + lname + fname + ".");
                Console.WriteLine("The member will be prompted to choose a 4-digit password when first logging in.");
                Console.WriteLine();
            }

            Console.WriteLine("Press any key to return to the staff menu.");
            Console.ReadKey();
        }
        
        static void FindPhoneNumber()
        {
            Console.Clear();

            Console.WriteLine("4. Find a registered member's phone number");
            Console.WriteLine();
            Console.Write("Please enter the member's full name: ");
            
            string userInput = Console.ReadLine();
            MemberCollection.SearchForMemberInArray(userInput, members);

            Console.WriteLine("Press any key to return to the staff menu.");
            Console.ReadKey();
        }

        static void BorrowMovie(Member borrowingMember)
        {
            Console.Clear();

            // heading
            Console.WriteLine("Please input the name of the movie that you would like to borrow.");
            string desiredMovie = Console.ReadLine();

            bool alreadyBorrowed = false;
            bool errorGenerated = false;

            if (MovieCollection.FindMovieInTree(desiredMovie) != null)
            {
                Movie searchedMovie = MovieCollection.FindMovieInTree(desiredMovie).data;

                if (borrowingMember.Movies != null)
                {
                    for (int i = 0; i < borrowingMember.Movies.Length; i++)
                    {
                        if (searchedMovie == borrowingMember.Movies[i]) // the logged in member has already borrowed this movie
                        {
                            alreadyBorrowed = true;
                            Console.WriteLine("You have already borrowed " + desiredMovie + ". You must return it before you borrow another.");
                            errorGenerated = true;
                        }
                    }
                }

                if (borrowingMember.Movies != null && borrowingMember.Movies.Length > 9 && !errorGenerated)
                {
                    Console.WriteLine("You have already borrowed the maximum of 10 movies. Please return one before borrowing more.");
                    errorGenerated = true;
                }
                
                if (searchedMovie.CopiesAvailable > 0 && !alreadyBorrowed && !errorGenerated)
                {
                    if (borrowingMember.Movies != null)
                    {
                        List<Movie> movieList = borrowingMember.Movies.ToList();
                        movieList.Add(searchedMovie);
                        borrowingMember.Movies = movieList.ToArray();
                    }
                    else
                    {
                        List<Movie> movieList = new List<Movie>();
                        movieList.Add(searchedMovie);
                        borrowingMember.Movies = movieList.ToArray();
                    }

                    searchedMovie.CopiesAvailable -= 1;
                    searchedMovie.BorrowHistory += 1;
                    Console.WriteLine("You have successfully borrowed " + desiredMovie + ".");
                    errorGenerated = true;
                }
                else if (!errorGenerated)
                {
                    Console.WriteLine("Sorry, there are currently no copies of " + desiredMovie + " available.");
                }
            }
            else
            {
                Console.WriteLine("Sorry, there are no records of " + desiredMovie + " on our records.");
            }

            Console.WriteLine("Press any key to return to the member menu.");
            Console.ReadKey();
        }

        static void ReturnMovie(Member returningMember)
        {
            Console.Clear();

            // heading
            Console.WriteLine("Please input the name of the movie that you would like to return.");
            string returningMovie = Console.ReadLine();

            if (MovieCollection.FindMovieInTree(returningMovie) != null)
            {
                Movie searchedMovie = MovieCollection.FindMovieInTree(returningMovie).data;
                bool ableToReturn = false;

                if (returningMember.Movies != null)
                {
                    for (int i = 0; i < returningMember.Movies.Length; i++)
                    {
                        if (returningMember.Movies[i].Title == returningMovie)
                        {
                            ableToReturn = true;
                            var movieList = returningMember.Movies.ToList();
                            movieList.Remove(searchedMovie);
                            returningMember.Movies = movieList.ToArray();
                            Console.WriteLine("You have successfully returned " + returningMovie + ".");
                        }
                    }
                }

                if (!ableToReturn)
                {
                    Console.WriteLine("Movie return unsuccessful. You are not able to return a movie that you have not borrowed.");
                }
            }
            else
            {
                Console.WriteLine("Sorry, there are no records of " + returningMovie + " on our records.");
            }

            Console.WriteLine("Press any key to return to the member menu.");
            Console.ReadKey();
        }

        static void ListCurrentlyBorrowedMovies(Member givenMember)
        {
            Console.Clear();

            if(givenMember.Movies == null || givenMember.Movies.Length == 0 || givenMember.Movies[0] == null)
            {
                Console.WriteLine("You are not currently borrowing any movies.");
            }
            else
            {
                Console.WriteLine("The movie titles that you currently are borrowing are: ");
                for (int i = 0; i < givenMember.Movies.Length; i++)
                {
                    Console.WriteLine(givenMember.Movies[i].Title);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to return to the member menu.");
            Console.ReadKey();
        }

        static void DisplayMemberLogin()
        {
            string username;
            string password;

            bool attemptedLogin = false;
            bool attemptedPasswordSet = false;
            bool validPassword = true;

            do
            {
                if (attemptedLogin)
                {
                    Console.WriteLine("Incorrect username or password. Insert 0 to return to the main menu or ENTER to try again.");
                    string response = Console.ReadLine();

                    if (response == "0")
                    {
                        FunctionalMainMenu();
                    }
                }

                Console.Clear();
                // input username
                Console.Write("Please enter your member username: ");
                username = Console.ReadLine().ToString();

                // looping through all members
                for (int i = 0; i < members.Length; i++)
                {
                    // if the username exists but the password has not yet been set
                    if (members[i] != null && username == members[i].Username && members[i].Password == "none")
                    {
                        Console.WriteLine("You have not yet set your password. Please enter a 4-digit pin to continue.");
                        do
                        {
                            if (attemptedPasswordSet)
                            {
                                Console.Clear();
                                Console.WriteLine("This is an invalid password. Passwords must be a 4-digit pin. Please try again.");
                            }

                            password = Console.ReadLine().ToString();

                            Regex rgx = new Regex(@"^[0-9]{4}$"); // password must be 4 digits 
                            validPassword = rgx.IsMatch(password);
                            attemptedPasswordSet = true;

                        } while (!validPassword);

                        members[i].Password = password; 
                        Console.WriteLine("Password was successfully set. Press any key to continue to the member menu.");
                        Console.ReadKey();

                        FunctionalMemberMenu(members[i]);
                        break;
                    }
                }

                Console.Write("Please enter your member password: ");
                password = Console.ReadLine().ToString();

                for (int i = 0; i < members.Length; i++)
                {
                    if (members[i] != null && username == members[i].Username && password == members[i].Password)
                    {
                        FunctionalMemberMenu(members[i]);
                        break;
                    }
                    else
                    {
                        attemptedLogin = true;
                    }
                }
            } while (attemptedLogin); 
        }

        static void DisplayMemberMenu()
        {
            // Display member menu 
            Console.WriteLine("=========== Member Menu ===========");
            Console.WriteLine("1. Display all movies");
            Console.WriteLine("2. Borrow a movie DVD");
            Console.WriteLine("3. Return a movie DVD");
            Console.WriteLine("4. List currently borrowed movie DVDs");
            Console.WriteLine("5. Display top 10 most popular movies");
            Console.WriteLine("0. Return to main menu");
            Console.WriteLine("===================================");
            Console.WriteLine("Please make a selection (1-5 or 0 to return to main menu)");
        }

        static void FunctionalMainMenu()
        {
            bool invalid = false;
            string mainMenuSelection;
            do
            {
                Console.Clear();

                if (invalid)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    Console.WriteLine();
                }

                DisplayMainMenu();
                mainMenuSelection = Console.ReadLine().ToString();

                switch (mainMenuSelection)
                {
                    case "1":
                        DisplayStaffLogin(); // display staff login
                        break;

                    case "2":
                        DisplayMemberLogin(); // display member login
                        break;

                    case "0":
                        break; // exit

                    default:
                        invalid = true; // this will prompt an error message on the next do while loop
                        break;
                }
            } while (mainMenuSelection != "0");
        }

        static void FunctionalStaffMenu()
        {
            string staffMenuSelection;
            bool invalid = false;

            do
            {
                Console.Clear();

                if (invalid)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    Console.WriteLine();
                }

                DisplayStaffMenu();
                staffMenuSelection = Console.ReadLine().ToString();

                switch (staffMenuSelection)
                {
                    case "1":
                        AddMovie(); // add a new movie DVD
                        break;

                    case "2":
                        RemoveMovie(); // remove a movie DVD
                        break;

                    case "3":
                        RegisterMember(); // register new member
                        break;

                    case "4":
                        FindPhoneNumber(); // find a registered member's phone number
                        break;

                    case "0":
                        FunctionalMainMenu(); // return to main menu
                        break;

                    default:
                        invalid = true;
                        break;
                }
            } while (staffMenuSelection != "0");
        }

        static void FunctionalMemberMenu(Member loggedInUser)
        {
            string memberMenuSelection;
            bool invalid = false;

            do
            {
                Console.Clear();

                if (invalid)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    Console.WriteLine();
                }

                DisplayMemberMenu();
                memberMenuSelection = Console.ReadLine().ToString();

                switch (memberMenuSelection)
                {
                    case "1": // display all movies
                        Console.Clear();
                        MovieCollection.DisplayAllMoviesInTree();
                        Console.WriteLine("Press any key to return to the member menu.");
                        Console.ReadKey();
                        break;

                    case "2": // borrow a movie
                        BorrowMovie(loggedInUser);
                        break;

                    case "3": // return a movie
                        ReturnMovie(loggedInUser);
                        break;

                    case "4": // list currently borrowed movies
                        ListCurrentlyBorrowedMovies(loggedInUser);
                        break;

                    case "5": // show top 10 popular movies
                        
                        Movie[] movsWithNull = MovieCollection.PutAllMoviesInArray(movieArray); // in-order tree traversal
                        int index = 0;

                        for (int i = 0; i < movsWithNull.Length; i++) // this loop counts how many non-null elements are in the array
                        {
                            if (movsWithNull[i] != null)
                            {
                                index += 1; 
                            }
                        }

                        Movie[] movs = new Movie[index]; // creating an array with no null elements

                        for (int i = 0; i < index; i++)
                        {
                            movs[i] = movsWithNull[i];
                        }

                        Movie[] sortedMovs = MovieCollection.SortByPopularity(movs);

                        Console.Clear();
                        Console.WriteLine("The top 10 most popular movies are:");

                        for (int i = 0; i < 10; i++)
                        {
                            Console.WriteLine(sortedMovs[i].Title);
                            Console.WriteLine(sortedMovs[i].BorrowHistory);
                            Console.WriteLine();
                        }

                        Console.WriteLine("Press any key to return to the member menu.");
                        Console.ReadKey();

                        break;

                    case "0":
                        FunctionalMainMenu(); // return to main menu
                        break;

                    default:
                        invalid = true;
                        break;
                }
            } while (memberMenuSelection != "0");
        }

        static void CreateDummyUsers()
        {
            // creating dummy users to help with testing and showing implementations

            Member member1 = new Member("Bec", "McMahon", "34 Fairfield Avenue, Norman Gardens", "0447635285");
            member1.Password = "1234";
            MemberCollection.AddMemberToArray(member1, members);

            Member member2 = new Member("Sam", "Olive", "3 Treseder Street, The Range", "0466659547");
            member2.Password = "3456";
            MemberCollection.AddMemberToArray(member2, members);

            Member member3 = new Member("Natalie", "Robina", "7 Flanagan Street, Frenchville", "0422433309");
            member3.Password = "9598";
            MemberCollection.AddMemberToArray(member3, members);
        }

        static void CreateDummyMovies()
        {
            // creating dummy movies to help with testing and showing implementations

            Movie movie1 = new Movie("Forrest Gump", new string[]{ "Tom Hanks", "Robin Wright", "Gary Sinise" }, "Robert Zemeckis", 142, Movie.Genre.Family, Movie.Classification.ParentalGuidance, "17-11-1994", 3);
            movie1.BorrowHistory = 4;
            MovieCollection.AddMovieToTree(movieCollection, movie1);

            Movie movie2 = new Movie("School of Rock", new string[] { "Jack Black", "Miranda Cosgrove" }, "Richard Linklater", 109, Movie.Genre.Comedy, Movie.Classification.General, "20-11-2003", 2);
            movie2.BorrowHistory = 2;
            MovieCollection.AddMovieToTree(movieCollection, movie2);

            Movie movie3 = new Movie("Finding Nemo", new string[] { "Albert Brooks", "Alexander Gould", "Barry Humphries" }, "Andrew Stanton", 100, Movie.Genre.Animated, Movie.Classification.General, "28-08-2003", 3);
            movie3.BorrowHistory = 5;
            MovieCollection.AddMovieToTree(movieCollection, movie3);

            Movie movie4 = new Movie("Avatar", new string[] { "Zoe Saldana", "Sam Worthington", "Michelle Rodriguez" }, "James Cameron", 162, Movie.Genre.SciFi, Movie.Classification.Mature, "17-12-2009", 4);
            movie4.BorrowHistory = 1;
            MovieCollection.AddMovieToTree(movieCollection, movie4);

            Movie movie5 = new Movie("Ten Things I Hate About You", new string[] { "Heath Ledger", "Julia Stiles"}, "Gill Junger", 99, Movie.Genre.Comedy, Movie.Classification.Mature, "31-03-1999", 2);
            movie5.BorrowHistory = 4;
            MovieCollection.AddMovieToTree(movieCollection, movie5);

            Movie movie6 = new Movie("Bridge to Terabithia", new string[] { "AnnaSophia Robb", "Josh Hutcherson", "Bailee Madison" }, "Gabor Csupo", 96, Movie.Genre.Adventure, Movie.Classification.Mature, "14-06-2007", 1);
            movie6.BorrowHistory = 0;
            MovieCollection.AddMovieToTree(movieCollection, movie6);

            Movie movie7 = new Movie("Titanic", new string[] { "Leonardo DiCaprio", "Kate Winslet" }, "James Cameron", 195, Movie.Genre.Drama, Movie.Classification.Mature, "17-12-1997", 1);
            movie7.BorrowHistory = 2;
            MovieCollection.AddMovieToTree(movieCollection, movie7);

            Movie movie8 = new Movie("Back to the Future", new string[] { "Michael J. Fox", "Christopher Lloyd", "Lea Thompson" }, "Robert Zemeckis", 116, Movie.Genre.Adventure, Movie.Classification.ParentalGuidance, "15-18-1985", 2);
            movie8.BorrowHistory = 3;
            MovieCollection.AddMovieToTree(movieCollection, movie8);

            Movie movie9 = new Movie("The Wolf of Wall Street", new string[] { "Margot Robbie", "Leonardo DiCaprio" }, "Martin Scorsese", 180, Movie.Genre.Drama, Movie.Classification.MatureAccompanied, "23-01-2014", 2);
            movie9.BorrowHistory = 5;
            MovieCollection.AddMovieToTree(movieCollection, movie9);

            Movie movie10 = new Movie("Jurassic Park", new string[] { "Jeff Goldblum", "Laura Dern", "Sam Neill" }, "Steven Spielberg", 127, Movie.Genre.Adventure, Movie.Classification.ParentalGuidance, "02-09-1993", 1);
            movie10.BorrowHistory = 0;
            MovieCollection.AddMovieToTree(movieCollection, movie10);

            Movie movie11 = new Movie("Die Hard", new string[] { "Bruce Willis", "Alan Rickman" }, "John McTiernan", 132, Movie.Genre.Action, Movie.Classification.MatureAccompanied, "06-10-1988", 1);
            movie11.BorrowHistory = 0;
            MovieCollection.AddMovieToTree(movieCollection, movie11);

            Movie movie12 = new Movie("Easy A", new string[] { "Emma Stone", "Amanda Bynes", "Penn Badgley" }, "Will Gluck", 92, Movie.Genre.Comedy, Movie.Classification.Mature, "16-09-2010", 3);
            movie12.BorrowHistory = 3;
            MovieCollection.AddMovieToTree(movieCollection, movie12);

            Movie movie13 = new Movie("Shrek", new string[] { "Mike Myers", "Eddie Murphy", "Cameron Diaz" }, "Andrew Adamson", 95, Movie.Genre.Animated, Movie.Classification.General, "21-06-2001", 2);
            movie13.BorrowHistory = 4;
            MovieCollection.AddMovieToTree(movieCollection, movie13);

            Movie movie14 = new Movie("Ferris Bueller's Day Off", new string[] { "Matthew Broderick", "Mia Sara" }, "John Hughes", 103, Movie.Genre.Comedy, Movie.Classification.ParentalGuidance, "21-08-1986", 3);
            movie14.BorrowHistory = 2;
            MovieCollection.AddMovieToTree(movieCollection, movie14);

            Movie movie15 = new Movie("Jumanji", new string[] { "Karen Gillan", "Dwayne Johnson" }, "Jake Kasdan", 123, Movie.Genre.Action, Movie.Classification.Mature, "26-12-2019", 1);
            movie15.BorrowHistory = 0;
            MovieCollection.AddMovieToTree(movieCollection, movie15);
        }

        static void Main(string[] args)
        {
            CreateDummyUsers();
            CreateDummyMovies();

            FunctionalMainMenu(); // program entry point
        }
    }
}

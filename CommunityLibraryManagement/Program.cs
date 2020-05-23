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
        
        static void DisplayMainMenu()
        {
            // reset the console
            Console.Clear();

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
                // reset the console
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
            // reset the console
            Console.Clear();

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
            // need to check validity of each input individually
            
            // heading
            Console.WriteLine("1. Add a new movie");
            Console.WriteLine("");

            // insert movie title
            Console.Write("Movie title: ");
            string ttl = Console.ReadLine().ToString();
            ttl = ttl.Trim(); // remove spaces at the start and end of string

            // insert starring actors
            // need to account for no input
            // need to account for double comma
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
            Console.Write("Duration: ");
            int dur = int.Parse(Console.ReadLine());

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
            int availCopies = Convert.ToInt32(Console.ReadLine());

            // use all user input to create a Movie instance
            Movie addedMovie = new Movie(ttl, starArray, dir, dur, gen, classif, relDate, availCopies); // add available copies

            bool status = MovieCollection.AddMovieToTree(movieCollection, addedMovie);

            // if the movie was successfully added to the BST
            if (status)
            {
                Console.WriteLine("");
                Console.WriteLine("Movie successfully added. Press any key to return to the staff menu.");
                Console.ReadKey();
            }
        }

        static void RemoveMovie()
        {
            // ?
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
            Movie searchedMovie = MovieCollection.FindMovieInTree(desiredMovie).data;
            
            if (borrowingMember.Movies != null)
            {
                for (int i = 0; i < borrowingMember.Movies.Length; i++)
                {
                    if (searchedMovie == borrowingMember.Movies[i]) // the logged in member has already borrowed this movie
                    {
                        alreadyBorrowed = true;
                        Console.WriteLine("You have already borrowed " + desiredMovie + ". You must return it before you borrow another. Press any key to return to the member menu.");
                    }
                }
            }

            if (borrowingMember.Movies != null && borrowingMember.Movies.Length > 10)
            {
                Console.WriteLine("You have already borrowed the maximum of 10 movies. Please return one before borrowing more.");
            }
            else if (searchedMovie.CopiesAvailable > 0 && !alreadyBorrowed) 
            {
                if (borrowingMember.Movies != null)
                {
                    borrowingMember.Movies.Append(searchedMovie);
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
            } 
            else
            {
                Console.WriteLine("Sorry, there are currently no copies of " + desiredMovie + " available.");
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

            Console.WriteLine("Press any key to return to the member menu.");
            Console.ReadKey();
        }

        static void ListCurrentlyBorrowedMovies(Member givenMember)
        {
            Console.Clear();

            if(givenMember.Movies == null || givenMember.Movies[0] == null)
            {
                Console.WriteLine("You are not currently borrowing any movies.");
            }
            else
            {
                Console.WriteLine("The movie titles that you currently are borrowing are: ");
                for (int i = 0; i < givenMember.Movies.Length; i++)
                {
                    Console.WriteLine(givenMember.Movies[i].Title);
                    Console.WriteLine();
                }
            }

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
                // Reset the console
                Console.Clear();

                if (attemptedLogin)
                {
                    Console.WriteLine("Incorrect username or password. Insert 0 to return to the main menu or ENTER to try again.");
                    string response = Console.ReadLine();

                    if (response == "0")
                    {
                        FunctionalMainMenu();
                    }
                }

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
            // Reset the console
            Console.Clear();

            // Display member menu 
            Console.WriteLine("=========== Member Menu ===========");
            Console.WriteLine("1. Display all movies");
            Console.WriteLine("2. Borrow a movie DVD");
            Console.WriteLine("3. Return a movie DVD");
            Console.WriteLine("4. List currently borrowed movie DVDs");
            Console.WriteLine("5. Display top 10 most popular movies");
            Console.WriteLine("0. Return to main menu");
            Console.WriteLine("===================================");
            Console.WriteLine("Please make a selection (1-5 or 0 to return to main menu");
        }

        static void FunctionalMainMenu()
        {
            string mainMenuSelection;
            do
            {
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
                }
            } while (mainMenuSelection != "0");
        }

        static void FunctionalStaffMenu()
        {
            string staffMenuSelection;

            do
            {
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
                }
            } while (staffMenuSelection != "0");
        }

        static void FunctionalMemberMenu(Member loggedInUser)
        {
            string memberMenuSelection;

            do
            {
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
                        break;

                    case "0":
                        FunctionalMainMenu(); // return to main menu
                        break;
                }
            } while (memberMenuSelection != "0");

        }

        static void Main(string[] args)
        {
            FunctionalMainMenu();
        }
    }
}

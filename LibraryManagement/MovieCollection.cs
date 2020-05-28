using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace LibraryManagement
{
    public class MovieCollection
    {
        private static TreeNode root;
        public static int index = 0;

        // getters and setters
        public TreeNode Root
        {
            get { return root; }
            set { root = value; }
        }

        public static bool AddMovieToTree(MovieCollection movCol, Movie movie)
        {
            string movieNodeName = movie.Title;

            // using the root as the starting point
            TreeNode before = null;
            TreeNode after = movCol.Root;

            // traversing to find its spot in the tree
            // note that if there is no root, then this while loop does not run (as after == null)
            while (after != null)
            {
                before = after;
                if (string.Compare(movieNodeName, after.data.Title) == -1) // movieNodeName is first alphabetically, so move to the left of the BST
                {
                    after = after.left;
                } 
                else if (string.Compare(movieNodeName, after.data.Title) == 1) 
                { // after.data.Title is first alphabetically, so move to the right of the BST
                    after = after.right;
                } 
                else // movies have the same title and are rejected (no duplicate titles allowed)
                {
                    return false;
                }
            }

            TreeNode insertedNode = new TreeNode(); // create new tree node
            insertedNode.data = movie; // giving new treenode the new movie as its data

            if (movCol.Root == null) // if this is the beginning of the tree (i.e. there is no previous root)
            {
                movCol.Root = insertedNode; // the new node becomes the root
            } 
            else 
            {
                if (string.Compare(movieNodeName, before.data.Title) == -1) 
                {
                    before.left = insertedNode; // movieNodeName is first alphabetically, so place it to the left of the BST
                } else {
                    before.right = insertedNode; // after.data.Tile is first alphabetically, so place it to the right of the BST
                }
            }
            return true;
        }


        public static void RemoveMovieFromTree(Movie movie)
        { 
            root = RemoveMovieFromTree(root, movie);
        }

        private static TreeNode RemoveMovieFromTree(TreeNode parent, Movie movie)
        {
            if (parent == null)
            {
                return parent;
            }

            if (string.Compare(movie.Title, parent.data.Title) == -1) // if movie comes first alphabetically
            {
                parent.left = RemoveMovieFromTree(parent.left, movie);
            }
            else if (string.Compare(movie.Title, parent.data.Title) == 1) // if parent comes first alphabetically
            {
                parent.right = RemoveMovieFromTree(parent.right, movie);
            }
            // if value is same as parent's value, then this is the node to be deleted  
            else
            {
                // node with only one child or no child  
                if (parent.left == null)
                    return parent.right;
                else if (parent.right == null)
                    return parent.left;

                // node with two children: Get the first alphabetically in the right subtree
                parent.data = FirstAlph(parent.right);

                // Delete the first alphabetically in the right subtree  
                parent.right = RemoveMovieFromTree(parent.right, parent.data);
            }

            return parent;
        }

        private static Movie FirstAlph(TreeNode node)
        {
            Movie FAlph = node.data;

            while (node.left != null)
            {
                FAlph = node.left.data;
                node = node.left;
            }
            return FAlph;
        }

        public static TreeNode FindMovieInTree(string title)
        {
            return FindMovieInTree(title, root);
        }

        private static TreeNode FindMovieInTree(string title, TreeNode parent)
        {
            if (parent != null)
            {
                if (title == parent.data.Title)
                {
                    return parent;
                }
                if (string.Compare(title, parent.data.Title) == -1)
                {
                    return FindMovieInTree(title, parent.left);
                }
                else
                {
                    return FindMovieInTree(title, parent.right);
                }
            }
            return null;
        }

        public static void DisplayAllMoviesInTree()
        {
            // in-order tree traversal to display movies alphabetically
            DisplayAllMoviesInTree(root);
        }

        private static void DisplayAllMoviesInTree(TreeNode Root)
        {
            // in-order tree traversal to display movies alphabetically
            if (Root == null)
            {
                return;
            }
            
            DisplayAllMoviesInTree(Root.left);
            TreeNode.DisplayData(Root);
            DisplayAllMoviesInTree(Root.right);
        }

        public static Movie[] PutAllMoviesInArray(Movie[] movies)
        {
            return PutAllMoviesInArray(root, movies);
        }

        private static Movie[] PutAllMoviesInArray(TreeNode Root, Movie[] movies)
        {
            // in order traversal to put all movies into an array. note that the array is pre-defined
            if (Root == null)
            {
                return null;
            }

            PutAllMoviesInArray(Root.left, movies);
            movies[index++] = Root.data;
            PutAllMoviesInArray(Root.right, movies);

            return movies;
        }

        public static Movie[] SortByPopularity(Movie[] movArray)
        {
            // implementing bubble sort to sort the movie array in descending order

            // this is not yet right... have not specified that we want to display popularity??

            for (int i = 0; i < movArray.Length; i++)
            {
                for (int j = 0; j < movArray.Length; j++)
                {
                    if (movArray[i].BorrowHistory > movArray[j].BorrowHistory)
                    {
                        Movie temporarySwap = movArray[i];
                        movArray[i] = movArray[j];
                        movArray[j] = temporarySwap;
                    }
                }
            }
            return movArray;  
        }



    }
}




        

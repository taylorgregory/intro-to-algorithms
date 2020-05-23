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
                } else if (string.Compare(movieNodeName, after.data.Title) == 1) { // after.data.Title is first alphabetically, so move to the right of the BST
                    after = after.right;
                } else // movies have the same title and are rejected (no duplicate titles allowed)
                {
                    return false;
                }
            }

            TreeNode insertedNode = new TreeNode(); // create new tree node
            insertedNode.data = movie; // giving new treenode the new movie as its data

            if (movCol.Root == null) // if this is the beginning of the tree (i.e. there is no previous root)
            {
                movCol.Root = insertedNode; // the new node becomes the root
            } else {
                if (string.Compare(movieNodeName, before.data.Title) == -1) 
                {
                    before.left = insertedNode; // movieNodeName is first alphabetically, so place it to the left of the BST
                } else {
                    before.right = insertedNode; // after.data.Tile is first alphabetically, so place it to the right of the BST
                }
            }

            return true;
        }

        //public bool RemoveMovieFromTree(Movie movie)
        //{
        // remove a movie
        // search binary search tree to find corresponding tree node 
        // remove the node from the tree
        //}

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
            DisplayAllMoviesInTree(root);
        }

        private static void DisplayAllMoviesInTree(TreeNode Root) // return type
        {
            if (Root == null)
            {
                return;
            }
            
            DisplayAllMoviesInTree(Root.left);
            TreeNode.DisplayData(Root);
            DisplayAllMoviesInTree(Root.right);
        }

        //public bool DisplayTopTenInTree()
        //{
            // display the top 10 most frequently borrowed movies
            // use tree traversal to transform tree into array
            // sort array based on no. of times borrowed
        //}



    }
}




        

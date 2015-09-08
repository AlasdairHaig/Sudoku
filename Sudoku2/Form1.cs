using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Soduku
{
    public partial class Soduku : Form
    {
        public Soduku()
        {
            InitializeComponent();
        }

        int biggestPermutation = 0;

        string savePath = @"C:\Users\b.wood\Desktop\Entwicklung\SaveFile.txt";
        string patternPath = @"C:\Users\b.wood\Desktop\Entwicklung\SudukoPattern.txt";
        // Soduku grid in array
        int[, ,] grid = new int[9, 9, 12];

        /* the above Array is set with the first 2 dimensions refering to X and Y position.
         * the last dimension uses the first 9 for storing the probable numbers of 1 - 9 for recording
         * if those numbers have been used. If a number has been used, then that element is set to 10.
         * Numbers that have been set by the user, are stored in dimension 3, slot 10.
         * Slot 11 of dimension 3 is used to store the amount of permutations that remain for each slot.
         * Slot 12 holds the value that is being tested for each slot
        */




        // ///////////////////////////////////////////////////////////////////////////////////////////
        // //////// methods used overall ////////////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////////////////////////////////////////

        public void msTimer()
        {

        }

        public TextBox textBoxNameReturn(char l, int n) // (A,1)(A,2)...(B,1)
        {

            string textBoxName = l.ToString() + n.ToString();
            Control[] match = this.Controls.Find(textBoxName, true);
            if (match.Length > 0 && match[0] is TextBox)
            {
                TextBox tb = (TextBox)match[0];
                return tb;
            }
            else
            {
                return null;
            }
        }

        public void loadGridIntoArray()
        {
            for (char b = 'A'; b <= 'I'; b++)
            {
                for (int n = 1; n <= 9; n++)
                {
                    int index = (int)(char.ToUpper(b) - 65);
                    if (textBoxNameReturn(b, n).Text != "")
                    {
                        grid[index, n - 1, 9] = Int32.Parse(textBoxNameReturn(b, n).Text);
                    }
                    else
                    {
                        grid[index, n - 1, 9] = 0;
                    }
                }
            }
        }

        public void printOutResults()
        {

            using (StreamWriter sw = new StreamWriter(savePath))

                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        sw.Write(grid[x, y, 11]);
                    }
                    sw.WriteLine();
                }

        }

        public void readInSavedFile()
        {
            using (StreamReader sr = new StreamReader(patternPath))
            {
                for (char b = 'A'; b <= 'I'; b++)
                {
                    string oneLineAtATime = sr.ReadLine();

                    for (int n = 1; n <= 9; n++)
                    {
                        if ((oneLineAtATime[n - 1]).ToString() != "0")
                        {
                            textBoxNameReturn(b, n).Text = (oneLineAtATime[n - 1]).ToString();
                        }
                        else
                        {
                            textBoxNameReturn(b, n).Text = "";
                        }
                    }
                }
            }
        }


        // method for checking columns
        public bool columnCheck(int x, int y, int n, int C) // (column, row, number to be checked)
        {
            for (int row = 0; row <= 8; row++)
            {
                if ((n + 1) == grid[x, row, C] && y != row)
                {
                    return true; // returns true if the numbers has already been used
                }
            }
            return false;
        }

        // method for checking rows
        public bool rowCheck(int x, int y, int n, int C) // (row, column, number to be checked)
        {
            for (int column = 0; column <= 8; column++)
            {
                if ((n + 1) == grid[column, y, 9] && x != column)
                {
                    return true; // returns true if the numbers has already been used
                }
            }
            return false;
        }

        // method for checking boxes
        public bool boxCheck(int x, int y, int n, int C) // (x coordinates, y coordinates, number being tested)
        {
            int a = x / 3;
            int b = y / 3;

            a = a * 3;
            b = b * 3;

            for (int c = a; c < (a + 3); c++)
            {
                for (int d = b; d < (b + 3); d++)
                {
                    if ((n + 1) == grid[c, d, 9] && x != c && y != d)
                    {
                        return true; // returns true if the numbers has already been used
                    }
                }
            }
            return false;
        }


        // ///////////////////////////////////////////////////////////////////////////////////////////
        // //////// methods for creating game ///////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////////////////////////////////////////

        public void generateNumbers()
        {


        }






        // ///////////////////////////////////////////////////////////////////////////////////////////
        // /////////////methods for solving game ////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////////////////////////////////////////


        //removes all used numbers
        public void reducePosibilities()
        {
            //
            for (int x = 0; x <= 8; x++)
            {
                for (int y = 0; y <= 8; y++)
                {
                    if (grid[x, y, 9] == 0)
                    {
                        //says there is 9 possiblities to each xy coordinate
                        grid[x, y, 10] = 9;
                        // this set the 9 numbers to each xy coordinate
                        for (int n = 0; n <= 8; n++)
                        {
                            // saving 0-8 to each slot
                            grid[x, y, n] = n;
                        }
                        //this goes through the 9 numbers and removes any number that have been used already
                        for (int n = 0; n <= 8; n++)
                        {
                            //removes any numbers that are in the same row, column, or box; from that slot's inventory
                            if (boxCheck(x, y, n, 9) || columnCheck(x, y, n, 9) || rowCheck(x, y, n, 9))
                            {
                                grid.SetValue(10, x, y, n);
                                //take a permutation away
                                grid[x, y, 10] = grid[x, y, 10] - 1;
                            }
                        }

                        //if this slot has only one possibility remaining
                        if (grid[x, y, 10] == 1)
                        {
                            //find the only remaining posibility
                            for (int n = 0; n <= 8; n++)
                            {
                                if (grid[x, y, n] != 10)
                                {
                                    // putting the last possibility in slot
                                    grid[x, y, 9] = (n + 1);
                                }
                            }
                            grid[x, y, 10] = grid[x, y, 10] - 1;
                        }

                    }
                    displayNumber(x, y);
                    Application.DoEvents();
                }
            }
        }

        public bool remainingPermutations()
        {
            for (int x = 0; x <= 8; x++)
            {
                for (int y = 0; y <= 8; y++)
                {
                    if (grid[x, y, 10] != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public void displayNumber(int x, int y)
        {
            char l = Convert.ToChar(x + 65);
            textBoxNameReturn(l, y + 1).Text = Convert.ToString(grid[x, y, 9]);
        }



        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void solveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime dt;
            DateTime dt1;
            TimeSpan tm;

            loadGridIntoArray();
            dt = DateTime.Now;
            do
            {
                reducePosibilities();
            } while (remainingPermutations());
            printOutResults();
            dt1 = DateTime.Now;
            tm = dt1.Subtract(dt);

            MessageBox.Show("Benötigte zeit: " + tm.Milliseconds.ToString() + " ms");

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadGridIntoArray();
            printOutResults();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readInSavedFile();
        }
    }

}

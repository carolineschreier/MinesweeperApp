using System.Data.Common;
using System.Diagnostics;
using System.Security.Cryptography.Xml;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Minesweeper_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Members

        /// <summary>
        /// Holds the current results of an active game
        /// </summary>
        private MarkType[,] mResults;

        /// <summary>
        /// How many times a button has been clicked
        /// </summary>
        private int[,] clickedTimes;

        /// <summary>
        /// Whether each cell is a Mine
        /// </summary>
        private bool[,] isMine;

        /// <summary>
        /// Whether a cell has been flagged as a mine by the user
        /// </summary>
        private bool[,] isFlagged;

        /// <summary>
        /// How many mines each cell is adjacent to, -1 if a cell is a mine
        /// </summary>
        private int[,] mineAdjacencies;

        /// <summary>
        /// Rows and columns for each mine location on the board
        /// </summary>
        private int[,] mineLocations;

        /// <summary>
        /// Randomly generated integers giving starting row once the start button is pressed
        /// </summary>
        private int startRow;

        /// <summary>
        /// Randomly generated integers giving starting column once the start button is pressed
        /// </summary>
        private int startCol;

        /// <summary>
        /// Number of mines the board will hold, based on the size of the board
        /// </summary>
        private int numMines;

        /// <summary>
        /// Whether or not the start button has been pressed - determines whether the user is allowed to push the rest of the buttons
        /// </summary>
        private bool startPressed;

        /// <summary>
        /// Number of rows on the board held constant
        /// </summary>
        private const int NUM_ROWS = 8;

        /// <summary>
        /// Number of columns on the board held constant
        /// </summary>
        private const int NUM_COLS = 8;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor, creates the window the game is played in then calls NewGame for setting up and runnning the game
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            NewGame();
        }

        #endregion

        #region Getters and Setters
        // Because the code is all part of the main window class, the getters and setters are used for testing exclusively

        /// <summary>
        /// Sets isMine boolean at a specific position row and column value in the game board
        /// </summary>
        public void setIsMine(int rowVal, int colVal, bool truthVal)
        {
            isMine[rowVal, colVal] = truthVal;
        }

        /// <summary>
        /// Returns the adjacency value at a specific row and column value in the game board
        /// </summary>
        public int getMineAdjacency(int rowVal, int colVal)
        {
            return mineAdjacencies[rowVal, colVal];
        }


        /// <summary>
        /// Sets the adjacency value at a specific row and column in the game board
        /// </summary>
        public void setMineAdjacency(int rowVal, int colVal, int adjVal)
        {
            mineAdjacencies[rowVal, colVal] = adjVal;
        }

        /// <summary>
        /// Sets the position of a mine in the gameboard
        /// </summary>
        public void setMineLoc(int mineNum, int rowVal, int colVal)
        {
            mineLocations[mineNum, 0] = rowVal;
            mineLocations[mineNum, 1] = colVal;
        }

        /// <summary>
        /// Sets a mine as flagged on the gameboard
        /// </summary>
        public void setIsFlagged(int flagRow, int flagCol, bool flaggedVal)
        {
            isFlagged[flagRow, flagCol] = flaggedVal;
        }


        #endregion

        #region Game Setup
        /// <summary>
        /// Starts a new game and clears all values back to their starting values, then sets up the game board with new values
        /// </summary>
        public void NewGame()
        {   
            // Create a new blank array of unclicked cells
            mResults = new MarkType[NUM_ROWS,NUM_COLS];

            // Create a new array indicating that each cell hasn't been clicked yet - default value is 0
            clickedTimes = new int[NUM_ROWS, NUM_COLS];

            // Create a new array indicating whether a cell is a mine - default value is false
            isMine = new bool[NUM_ROWS, NUM_COLS];

            // Create a new array indicating how many mines each cell is touching
            mineAdjacencies = new int[NUM_ROWS, NUM_COLS];

            // determine how many mines the board should have
            numMines = (int)(NUM_ROWS*NUM_COLS*0.2);

            // Create an array for row, column pairs
            mineLocations = new int[numMines, 2];

            // create an array the size of the game board which reflects whether each cell is flagged
            isFlagged = new bool[NUM_ROWS, NUM_COLS];

            // Set each cell's value to being unclicked to start
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    mResults[i,j] = MarkType.Unclicked;
                }
            }

            // Set startPressed to be false since the game just started
            startPressed = false;

            // Set the background and text colors for every button, and ensure that every button is blank to start
            Container.Children.Cast<Button>().ToList().ForEach(button =>
            {
                button.Content = string.Empty;
                button.Background = Brushes.LightGray;
                button.Foreground = Brushes.MediumPurple;

            });

            // Set Locations for Mines on the Board
            SetMinesLocations();

            // Set how many mines each position is associated with
            setMineAdjacencies();

        }

        #endregion

        #region Event Handling for Buttons
        /// <summary>
        /// Handles a button click event -- need to change once I know how to do different types of button clicks
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        public void Single_Click(object sender, RoutedEventArgs e)
        {
            // Cast the sender to a button
            var button = (Button)sender;

            // Find the buttons position in the array
            var column = Grid.GetColumn(button);
            var row = Grid.GetRow(button);

            // Marks that the button has been clicked
            clickedTimes[row, column] = clickedTimes[row, column] + 1;

            // if the user hasn't started the game, prompt them to push the start button
            if (startPressed == false)
            {
                MessageBox.Show("Please hit the Start button to start a new game!");
            }
            // if the cell is currently flagged, one click will unflag it
            else if (mResults[row, column] == MarkType.Flagged)
            {
                mResults[row, column] = MarkType.Unclicked;
                isFlagged[row, column] = false;
                button.Content = string.Empty;
            }
            // if the cell is currently unclicked, one click will flag it
            else if (mResults[row, column] == MarkType.Unclicked)
            {
                button.Content = "🚩";
                mResults[row, column] = MarkType.Flagged;
                isFlagged[row, column] = true;
            }

            // check whether the user has won the game based on their most recent click
            bool gameWon = checkGameStatus();
            
            // if they've won, display a message and restart to a new game
            if (gameWon == true)
            {
                MessageBox.Show("Congratulations, you've correctly identified all of the mines. You win!");
                NewGame();
            }
        }

        /// <summary>
        /// When a button is double clicked, it should reveal to the user how many mines that cell is adjacent to, or should inform the user they've clicked on a mine
        /// </summary>
        private void Button_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Cast the sender to a button
            var button = (Button)sender;

            // Find the buttons position in the array
            var column = Grid.GetColumn(button);
            var row = Grid.GetRow(button);

            // if the user hasn't pressed the start button, prompt them to do so
            if (startPressed == false)
            {
                MessageBox.Show("Please hit the Start button to start a new game!");
            }
            // if the button hadn't been pushed yet, update clicked buttons and assess what type of cell was clicked and reveal it to the user
            else if (mResults[row, column] == MarkType.Unclicked)
            {
                mResults[row, column] = MarkType.Clicked;
                if (mineAdjacencies[row, column] == -1)
                {
                    button.Content = "💣";
                    MessageBox.Show("You clicked on a Mine. You Lose!");
                    NewGame();
                }
                else
                {
                    button.Content = mineAdjacencies[row, column];
                }
            }
            // if the button was flagged previously, remove the cell as a flagged cell, then assess what type of cell was clicked and reveal it to the user
            else if (mResults[row, column] == MarkType.Flagged)
            {
                mResults[row, column] = MarkType.Clicked;
                isFlagged[row, column] = false;
                if (mineAdjacencies[row, column] == -1)
                {
                    button.Content = "💣";
                    MessageBox.Show("You clicked on a Mine. You Lose!");
                    NewGame();
                }
                else
                {
                    button.Content = mineAdjacencies[row, column];
                }
            }

            // check if the gamestatus has changed as a result of the most recent click
            bool gameWon = checkGameStatus();

            if (gameWon == true)
            {
                MessageBox.Show("Congratulations, you've correctly identified all of the mines. You win!");
                NewGame();
            }
        }

        /// <summary>
        /// When user pushes start, we need to create a random starting point for them to be able to see some adjacencies
        /// </summary>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            startPressed = true;

            // identify a range around the random start point and show the user this range
            Container.Children.Cast<Button>().ToList().ForEach(button =>
            {
                if ((Grid.GetColumn(button) >= startCol - 1) && (Grid.GetColumn(button) <= startCol + 1) && (Grid.GetRow(button) >= startRow - 1) && (Grid.GetRow(button) <= startRow + 1))
                {
                    if (mineAdjacencies[Grid.GetRow(button), Grid.GetColumn(button)] == -1)
                    {
                        button.Content = "💣";
                    }
                    else
                    {
                        button.Content = mineAdjacencies[Grid.GetRow(button), Grid.GetColumn(button)];
                    }
                    // set the state of all revealed buttons to clicked so that their state will not be changed again if someone pressed those buttons
                    mResults[Grid.GetRow(button), Grid.GetColumn(button)] = MarkType.Clicked;
                    clickedTimes[Grid.GetRow(button), Grid.GetColumn(button)] = 2;
                }
            });
        }

        /// <summary>
        /// When user pushes restart, we reset the game
        /// </summary>
        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        #endregion

        #region Game Logic

        /// <summary>
        /// Generate random mine location
        /// </summary>
        private void SetMinesLocations()
        {
            // set starting location with random integer
            Random rnd = new Random();
            startRow = rnd.Next(8);
            startCol = rnd.Next(8);

            // while we haven't created enough mines, create a new mine
            int i = 0;
            while (i < 12)
            {
                // identify two random integers to make a new mine location
                int mineCol = rnd.Next(8);
                int mineRow = rnd.Next(8);

                // if there's no mine where we're trying to put one
                if (isMine[mineRow, mineCol] == false)
                {
                    // if the location is outside the range of where the user can see at start
                    if ((mineRow > startRow + 1) || (mineRow < startRow - 1) || (mineCol > startCol + 1) || (mineCol < startCol - 1))
                    {
                        // place that mine and update arrays to show that it's there
                        mineLocations[i, 0] = mineRow;
                        mineLocations[i, 1] = mineCol;
                        isMine[mineRow, mineCol] = true;
                        i = i + 1;
                    }
                }
            }
        }

        /// <summary>
        /// Identify how many mines each cell is adjacent to
        /// </summary>
        public void setMineAdjacencies()
        {
            int currentCol = 0;
            int currentRow = 0;            
            // Loop through positions of mines
            for (int mineCounter = 0; mineCounter < numMines; mineCounter++)
            {
                currentRow = mineLocations[mineCounter, 0];
                currentCol = mineLocations[mineCounter, 1];
                // Loop through the entire game board
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (mineAdjacencies[i, j] != -1)
                        {
                            // Mark locations with mine as -1
                            if (i == currentRow && j == currentCol)
                            {
                                mineAdjacencies[i, j] = -1;
                            }
                            // Check location above and to the left of the mine
                            else if (i == currentRow - 1 && j == currentCol - 1)
                            {
                                mineAdjacencies[i, j]++;

                            }
                            // check location to the left of the mine
                            else if (i == currentRow && j == currentCol - 1)
                            {
                                mineAdjacencies[i, j]++;
                            }
                            // check location above the mine
                            else if (i == currentRow - 1 && j == currentCol)
                            {
                                mineAdjacencies[i, j]++;
                            }
                            // check location below the mine
                            else if (i == currentRow + 1 && j == currentCol)
                            {
                                mineAdjacencies[i, j]++;
                            }
                            // check location to the right of the mine
                            else if (i == currentRow && j == currentCol + 1)
                            {
                                mineAdjacencies[i, j]++;
                            }
                            // check location below and to the left of the mine
                            else if (i == currentRow + 1 && j == currentCol - 1)
                            {
                                mineAdjacencies[i, j]++;
                            }
                            // check location above and to the right of the mine
                            else if (i == currentRow - 1 && j == currentCol + 1)
                            {
                                mineAdjacencies[i, j]++;
                            }
                            // check location below and to the right of the mine
                            else if (i == currentRow + 1 && j == currentCol + 1)
                            {
                                mineAdjacencies[i, j]++;
                            }
                        }
                    }
                }
            }
        }

        // Checks after every time a person has clicked a button to see whether they have correctly flagged all the mines
        public bool checkGameStatus()
        {
            // Check whether every square that has a mine has been flagged as such
            for (int rowVal = 0; rowVal < NUM_ROWS; rowVal++)
            {
                for (int colVal = 0; colVal < NUM_COLS; colVal++)
                {
                    if (isFlagged[rowVal, colVal] != isMine[rowVal, colVal])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion
    }
}


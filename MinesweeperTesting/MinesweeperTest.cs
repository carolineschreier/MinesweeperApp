using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using Minesweeper_App;

namespace MinesweeperTesting
{
    // These tests focus on the logic of determining adjacencies and whether the user has won
    // With more time, I would have liked to implement automated UI testing to test other components of the game which deal more with user interaction
    [TestClass]
    public class MinesweeperTestClass
    {
        // Tests the most likely case (mines are scattered around the board)
        [WpfTestMethod]
        public void TestRandomLocations()
        {
            // set up window for testing
            var mainWindow = new Minesweeper_App.MainWindow();

            // clear values from previous tests
            for (int i = 0; i < 12; i++)
            {
                mainWindow.setMineLoc(i, 0, 0);
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    mainWindow.setMineAdjacency(i, j, 0);
                }
            }

            // set mines in random locations around the board
            mainWindow.setMineLoc(0, 1, 5);
            mainWindow.setMineLoc(1, 2, 3);
            mainWindow.setMineLoc(2, 7, 0);
            mainWindow.setMineLoc(3, 1, 4);
            mainWindow.setMineLoc(4, 5, 7);
            mainWindow.setMineLoc(5, 3, 6);
            mainWindow.setMineLoc(6, 6, 1);
            mainWindow.setMineLoc(7, 2, 2);
            mainWindow.setMineLoc(8, 5, 5);
            mainWindow.setMineLoc(9, 7, 5);
            mainWindow.setMineLoc(10, 3, 0);
            mainWindow.setMineLoc(11, 0, 0);

            // test setting adjacencies with mine locations above
            mainWindow.setMineAdjacencies();
            Assert.AreEqual(3, mainWindow.getMineAdjacency(1, 3));
            Assert.AreEqual(2, mainWindow.getMineAdjacency(4, 7));
            Assert.AreEqual(0, mainWindow.getMineAdjacency(7, 7));
            Assert.AreEqual(2, mainWindow.getMineAdjacency(2, 6));
            Assert.AreEqual(1, mainWindow.getMineAdjacency(4, 0));
        }

        // Tests the case where there are many mines near a single cell (in this case, there is a mine in every cell surrounding one)
        [WpfTestMethod]
        public void TestMineCluster()
        {
            // set up window for testing
            var mainWindow = new Minesweeper_App.MainWindow();
            
            // clear values from previous tests
            for (int i = 0; i < 12; i++)
            {
                mainWindow.setMineLoc(i, 0, 0);
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    mainWindow.setMineAdjacency(i, j, 0);
                }
            }

            // set mines in every direction surrounding cell (3, 3)
            // set mine above/left
            mainWindow.setMineLoc(0, 2, 2);
            // set mine above
            mainWindow.setMineLoc(1, 2, 3);
            // set mine above/right
            mainWindow.setMineLoc(2, 2, 4);
            // set mine left
            mainWindow.setMineLoc(3, 3, 2);
            // set mine right
            mainWindow.setMineLoc(4, 3, 4);
            // set mine below/left
            mainWindow.setMineLoc(5, 4, 2);
            // set mine below
            mainWindow.setMineLoc(6, 4, 3);
            // set mine below/right
            mainWindow.setMineLoc(7, 4, 4);

            // test setting adjacencies with mine locations above
            mainWindow.setMineAdjacencies();
            Assert.AreEqual(8, mainWindow.getMineAdjacency(3,3));
        }

        // test corner cases for issues with edge indices
        [WpfTestMethod]
        public void TestCornerAdjacencies()
        {
            // set up window for testing
            var mainWindow = new Minesweeper_App.MainWindow();

            // clear values from previous tests
            for (int i = 0; i < 12; i++)
            {
                mainWindow.setMineLoc(i, 0, 0);
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    mainWindow.setMineAdjacency(i, j, 0);
                }
            }

            // Set mines in upper right corner
            mainWindow.setMineLoc(0, 0, 6);
            mainWindow.setMineLoc(2, 1, 6);
            mainWindow.setMineLoc(3, 1, 7);

            // Set mines in lower left corner
            mainWindow.setMineLoc(4, 6, 0);
            mainWindow.setMineLoc(5, 6, 1);
            mainWindow.setMineLoc(6, 7, 1);

            // Determine adjacencies given new mine positions
            mainWindow.setMineAdjacencies();

            // Test whether the right number of mines were identified in these corners
            Assert.AreEqual(3, mainWindow.getMineAdjacency(0, 7));
            Assert.AreEqual(3, mainWindow.getMineAdjacency(7, 0));
        }

        /// <summary>
        /// Test that if the user has correctly flagged all of the mines, they will win
        /// </summary>
        [WpfTestMethod]
        public void TestGameWin()
        {
            var mainWindow = new Minesweeper_App.MainWindow();

            // Reset isFlagged and isMine values to default (false)
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    mainWindow.setIsFlagged(i, j, false);
                    mainWindow.setIsMine(i, j, false);
                }
            }

            // set flagged locations
            mainWindow.setIsFlagged(1, 1, true);
            mainWindow.setIsFlagged(4, 5, true);
            mainWindow.setIsFlagged(6, 7, true);
            mainWindow.setIsFlagged(3, 0, true);
            mainWindow.setIsFlagged(0, 4, true);
            mainWindow.setIsFlagged(2, 2, true);
            mainWindow.setIsFlagged(3, 7, true);
            mainWindow.setIsFlagged(1, 3, true);
            mainWindow.setIsFlagged(0, 6, true);
            mainWindow.setIsFlagged(4, 3, true);
            mainWindow.setIsFlagged(6, 2, true);
            mainWindow.setIsFlagged(5, 5, true);

            // set mine locations to be the same (the user should win)
            mainWindow.setIsMine(1, 1, true);
            mainWindow.setIsMine(4, 5, true);
            mainWindow.setIsMine(6, 7, true);
            mainWindow.setIsMine(3, 0, true);
            mainWindow.setIsMine(0, 4, true);
            mainWindow.setIsMine(2, 2, true);
            mainWindow.setIsMine(3, 7, true);
            mainWindow.setIsMine(1, 3, true);
            mainWindow.setIsMine(0, 6, true);
            mainWindow.setIsMine(4, 3, true);
            mainWindow.setIsMine(6, 2, true);
            mainWindow.setIsMine(5, 5, true);

            // Check that the user did win
            Assert.AreEqual(true, mainWindow.checkGameStatus());
        }

        // Test that if the user has identified the correct number of mines, but has not identified all of the correct cells, they will not win
        [WpfTestMethod]
        public void TestGameLoss()
        {
            var mainWindow = new Minesweeper_App.MainWindow();

            // Reset isFlagged and isMine values to default (false)
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    mainWindow.setIsFlagged(i, j, false);
                    mainWindow.setIsMine(i, j, false);
                }
            }

            // set flagged locations
            mainWindow.setIsFlagged(1, 1, true);
            mainWindow.setIsFlagged(4, 5, true);
            mainWindow.setIsFlagged(3, 7, true);
            mainWindow.setIsFlagged(3, 0, true);
            mainWindow.setIsFlagged(0, 4, true);
            mainWindow.setIsFlagged(0, 2, true);
            mainWindow.setIsFlagged(3, 7, true);
            mainWindow.setIsFlagged(1, 3, true);
            mainWindow.setIsFlagged(0, 6, true);
            mainWindow.setIsFlagged(4, 1, true);
            mainWindow.setIsFlagged(6, 2, true);
            mainWindow.setIsFlagged(5, 5, true);

            // set mine locations to be different (they should not win)
            mainWindow.setIsMine(1, 1, true);
            mainWindow.setIsMine(4, 5, true);
            mainWindow.setIsMine(6, 7, true);
            mainWindow.setIsMine(3, 0, true);
            mainWindow.setIsMine(0, 4, true);
            mainWindow.setIsMine(2, 2, true);
            mainWindow.setIsMine(3, 7, true);
            mainWindow.setIsMine(1, 3, true);
            mainWindow.setIsMine(0, 6, true);
            mainWindow.setIsMine(4, 3, true);
            mainWindow.setIsMine(6, 2, true);
            mainWindow.setIsMine(5, 5, true);

            // Check that the user did win
            Assert.AreEqual(false, mainWindow.checkGameStatus());
        }

    }

// This class allows for testing using the main window
public class WpfTestMethodAttribute : TestMethodAttribute
    {
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                return Invoke(testMethod);

            TestResult[] result = null;
            var thread = new Thread(() => result = Invoke(testMethod));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return result;
        }
        private TestResult[] Invoke(ITestMethod testMethod)
        {
            return new[] { testMethod.Invoke(null) };
        }
    }

}
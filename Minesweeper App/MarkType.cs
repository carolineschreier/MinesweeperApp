
namespace Minesweeper_App
{
    /// <summary>
    /// The type of value a cell in the game is currently at
    /// </summary>
    public enum MarkType
    {
        /// <summary>
        /// The cell hasn't been clicked
        /// </summary>
        Unclicked,
        /// <summary>
        /// The cell has been flagged
        /// </summary>
        Flagged,
        /// <summary>
        /// The cell has been clicked
        /// </summary>
        Clicked
    }
}

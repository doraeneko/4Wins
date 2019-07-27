using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace QuinShier
{
    public enum GameStates
    {
        Init,
        RedsTurn,
        BluesTurn,
        WinRed,
        WinBlue,
        Remis
    }

    public class GameBoard : AbsoluteLayout 
    {
        const int ROWS = 6;
        const int COLS = 7;
        Tile[,] tiles = new Tile[ROWS, COLS];

        public event PropertyChangedEventHandler StateChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            StateChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        GameStates state;
        internal GameStates State
        {
            get
            {
                return state;
            }
            set
            {
                if (value != this.state)
                {
                    state = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public GameBoard()
        {
            State = GameStates.Init;
            tiles = new Tile[ROWS, COLS];
            for (int x = 0; x < ROWS; ++x)
                for (int y = 0; y < COLS; ++y)
                {
                    var tile = new Tile(x, y, this);
                    tiles[x, y] = tile;
                    //tile.TileStatusChanged += OnTileStatusChanged;
                    Children.Add(tile.TileView);
                }

            SizeChanged += (sender, args) =>
            {
                double tileWidth = this.Width / COLS;
                double tileHeight = this.Height / ROWS;

                foreach (Tile tile in tiles)
                {
                    Rectangle bounds = new Rectangle(tile.Col * tileWidth,
                                                     tile.Row * tileHeight,
                                                     tileWidth, tileHeight);
                    SetLayoutBounds(tile.TileView, bounds);
                }
            };
        }

        void ResetBoard()
        {
            foreach (Tile tile in tiles)
            {
                tile.Reset();
            }
        }

        // Helpers for game management

        /// <summary>
        /// Find the row position of the disk after sinking inthe column. Returns
        /// false iff the column is full.
        /// </summary>
        private bool TryFindSinkedPlace(int clickedCol, out int row)
        {
            if (clickedCol >= COLS)
            {
                row = -1;
                return false; // ERROR
            }
            int firstSetRow = 0;
            while (firstSetRow < ROWS
                   && tiles[firstSetRow, clickedCol].TheKind == TileKind.Empty)
            {
                firstSetRow++;
            }
            if (firstSetRow != 0)
            {
                row = firstSetRow - 1;
                return true;
            }
            else
            {
                row = -1;
                return false;
            }
        }

        bool IsRemis()
        {
            // TODO: introduce a counter
            foreach (var tile in tiles)
            {
                if (tile.TheKind == TileKind.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        bool IsPartOfFourInARow(Tile tile)
        {
            int GetNrOfElementsHavingThisKindInDirection(TileKind theKind, int startRow, int startCol, int rowDelta,
                int colDelta)
            {
                int count = 1;
                int theRow = startRow + rowDelta;
                int theCol = startCol + colDelta;
                while(theRow >= 0 && theRow < ROWS && theCol >= 0 && theCol < COLS)
                {
                    if (tiles[theRow, theCol].TheKind != theKind)
                        break;
                    theRow += rowDelta;
                    theCol += colDelta;
                    count += 1;
                }
                theRow = startRow - rowDelta;
                theCol = startCol - colDelta;
                while (theRow >= 0 && theRow < ROWS && theCol >= 0 && theCol < COLS)
                {
                    if (tiles[theRow, theCol].TheKind != theKind)
                        break;
                    theRow -= rowDelta;
                    theCol -= colDelta;
                    count += 1;
                }
                return count;
            }

            void MarkTiles(TileKind theKind, int startRow, int startCol, int rowDelta,
               int colDelta)
            {
                int theRow = startRow + rowDelta;
                int theCol = startCol + colDelta;
                
                while (theRow >= 0 && theRow < ROWS && theCol >= 0 && theCol < COLS)
                {
                    if (tiles[theRow, theCol].TheKind != theKind)
                        break;
                    tiles[theRow, theCol].TheKind = theKind == TileKind.Red ? TileKind.RedMarked : TileKind.BlueMarked;
                    theRow += rowDelta;
                    theCol += colDelta;
                }
                theRow = startRow - rowDelta;
                theCol = startCol - colDelta;
                while (theRow >= 0 && theRow < ROWS && theCol >= 0 && theCol < COLS)
                {
                    if (tiles[theRow, theCol].TheKind != theKind)
                        break;
                    tiles[theRow, theCol].TheKind = theKind == TileKind.Red ? TileKind.RedMarked : TileKind.BlueMarked;
                    theRow -= rowDelta;
                    theCol -= colDelta;
                }
                tiles[startRow, startCol].TheKind = theKind == TileKind.Red ? TileKind.RedMarked : TileKind.BlueMarked;
            }

            TileKind kind = tile.TheKind;
            if (kind != TileKind.Blue && kind != TileKind.Red)
            {
                return false;
            }
            var (row, col) = (tile.Row, tile.Col);

            // first look for horizontals
            int horizontal
                = GetNrOfElementsHavingThisKindInDirection(kind, row, col, 0, 1);
            if (horizontal >= 4)
            {
                MarkTiles(kind, row, col, 0, 1);
                return true;
            }
            // vertical
            int vertical
                = GetNrOfElementsHavingThisKindInDirection(kind, row, col, 1, 0);
            if (vertical >= 4)
            {

                MarkTiles(kind, row, col, 1, 0);
                return true;
            }
            // diagonal, two ways
            int diagonal1
                = GetNrOfElementsHavingThisKindInDirection(kind, row, col, 1, 1);
            if (diagonal1 >= 4)
            {

                MarkTiles(kind, row, col, 1, 1);
                return true;
            }
            int diagonal2
                = GetNrOfElementsHavingThisKindInDirection(kind, row, col, 1, -1);
            if (diagonal2 >= 4)
            {

                MarkTiles(kind, row, col, 1, -1);
                return true;
            }
            return false;
        }

        // Transitions 

        public void NewTwoPlayerGame()
        {
            State = GameStates.Init; // just to reset the external elements
            ResetBoard();
            State = GameStates.RedsTurn;
        }

        public void TileClicked(Tile tile)
        {
            switch (State)
            {
                case GameStates.Init:
                    break;
                case GameStates.RedsTurn:
                    {
                        int row;
                        bool foundPos = TryFindSinkedPlace(tile.Col, out row);
                        if (foundPos)
                        {
                            tiles[row, tile.Col].TheKind = TileKind.Red;
                            if (IsPartOfFourInARow(tiles[row, tile.Col]))
                            {
                                State = GameStates.WinRed;
                            }
                            else if (IsRemis())
                            {
                                State = GameStates.Remis;
                            }
                            else
                            {
                                State = GameStates.BluesTurn;
                            }
                        }
                        break;
                    }
                case GameStates.BluesTurn:
                    {
                        int row;
                        bool foundPos = TryFindSinkedPlace(tile.Col, out row);
                        if (foundPos)
                        {
                            tiles[row, tile.Col].TheKind = TileKind.Blue;
                            if (IsPartOfFourInARow(tiles[row, tile.Col]))
                            {
                                State = GameStates.WinBlue;
                            }
                            else if (IsRemis())
                            {
                                State = GameStates.Remis;
                            }
                            else
                            {
                                State = GameStates.RedsTurn;
                            }
                        }
                        break;
                    }
                case GameStates.WinRed:
                    break;
                case GameStates.WinBlue:
                    break;
                case GameStates.Remis:
                    break;
                default:
                    break;
            }
        }

        // End Transitions
    }
}

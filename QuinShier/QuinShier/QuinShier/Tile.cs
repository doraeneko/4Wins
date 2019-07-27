using Xamarin.Forms;

namespace QuinShier
{
    public enum TileKind
    {
        Empty,
        Red,
        Blue,
        RedMarked,
        BlueMarked,
    }

    public class Tile
    {

        public int Col { get; set; }
        public int Row { get; set; }

        TileKind theKind;
        public TileKind TheKind {
            get
            {
                return theKind;
            }

            set
            {
                theKind = value;
                UpdateVisual();
            }
        }

        public ContentView TileView { private set; get; }

        private GameBoard Board { get; set; } 

        public void UpdateVisual()
        {
            switch (theKind)
            {
                case TileKind.Empty:
                    TileView.Content = neutralLabel;
                    break;
                case TileKind.Red:
                    TileView.Content = redLabel;
                    break;
                case TileKind.Blue:
                    TileView.Content = blueLabel;
                    break;
                case TileKind.RedMarked:
                    TileView.Content = redMarkedLabel;
                    break;
                case TileKind.BlueMarked:
                    TileView.Content = blueMarkedLabel;
                    break;
                default:
                    break;
            }
        }

        Label neutralLabel = new Label
        {
            Text = "",
            TextColor = Color.Black,
            BackgroundColor = Color.LightGray,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
        };

        Label redLabel = new Label
        {
            Text = "",
            TextColor = Color.Black,
            BackgroundColor = Color.Red,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
        };

        Label blueLabel = new Label
        {
            Text = "",
            TextColor = Color.Black,
            BackgroundColor = Color.Blue,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
        };

        Label redMarkedLabel = new Label
        {
            Text = "X",
            TextColor = Color.White,
            BackgroundColor = Color.Red,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
        };

        Label blueMarkedLabel = new Label
        {
            Text = "X",
            TextColor = Color.White,
            BackgroundColor = Color.Blue,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
        };

        public Tile(int row, int col, GameBoard board)
        {
            Row = row;
            Col = col;
            Board = board;
            TileView = new Frame
            {
                Content = neutralLabel,
                BackgroundColor = Color.Black,
                BorderColor = Color.Black,
                Padding = new Thickness(1)
            };
            TapGestureRecognizer singleTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            singleTap.Tapped += (sender, args) =>
            {
                this.OnTapped(sender, args);
            };
            TileView.GestureRecognizers.Add(singleTap);
            Reset();
        }

        public void Reset()
        {
            TheKind = TileKind.Empty;
            TileView.Content = neutralLabel;
        }

        public void OnTapped(object sender, object EventArgs)
        {
            Board.TileClicked(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QuinShier
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Board.StateChanged += BoardStateChanged;
            ResetDisplay();
        }

        public void ResetDisplay()
        {
            congratulationsBlue.IsVisible = false;
            congratulationsRed.IsVisible = false;
            remisMessage.IsVisible = false;        }

        private void BoardStateChanged(object sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                switch (Board.State)
                {
                    case GameStates.Init:
                        ResetDisplay();
                        break;
                    case GameStates.RedsTurn:
                        break;
                    case GameStates.BluesTurn:
                        break;
                    case GameStates.WinRed:
                        congratulationsRed.IsVisible = true;
                        break;
                    case GameStates.WinBlue:
                        congratulationsBlue.IsVisible = true;
                        break;
                    case GameStates.Remis:
                        remisMessage.IsVisible = true;
                        break;
                    default:
                        break;
                }

            }
        }

        public void OnTwoPlayersButtonClicked(object sender, EventArgs e)
        {
            Board.NewTwoPlayerGame();
        }
    }
}

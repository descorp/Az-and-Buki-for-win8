using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace levelupspace
{
    
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class GamePage : levelupspace.Common.LayoutAwarePage
    {
        GameClass game;

        Popup GameParamsPopup;
        Popup AnswerResultPopup;

        public static GamePage Current;

        public GamePage()
        {
            this.InitializeComponent();
            Current = this;
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации. Также предоставляется любое сохраненное состояние
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="navigationParameter">Значение параметра, передаваемое
        /// <see cref="Frame.Navigate(Type, Object)"/> при первоначальном запросе этой страницы.
        /// </param>
        /// <param name="pageState">Словарь состояния, сохраненного данной страницей в ходе предыдущего
        /// сеанса. Это значение будет равно NULL при первом посещении страницы.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (pageState == null)
            {
                if (GameParamsPopup == null)
                {
                    GameParamsPopup = new Popup();
                    GameParamsPopup.Closed += (senderPopup, argsPopup) =>
                    {
                        GameParamsPopup = null;
                    };

                    GameParamsPopup.HorizontalOffset = (Window.Current.Bounds.Width - 600) / 2;
                    GameParamsPopup.VerticalOffset = (Window.Current.Bounds.Height - 440) / 2;

                    // set the content to our UserControl
                    GameParamsPopup.Child = new GameParamsControl();

                    // open the Popup
                    GameParamsPopup.IsOpen = true;
                    
                    
                }
            }
        }

        /// <summary>
        /// Сохраняет состояние, связанное с данной страницей, в случае приостановки приложения или
        /// удаления страницы из кэша навигации. Значения должны соответствовать требованиям сериализации
        /// <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">Пустой словарь, заполняемый сериализуемым состоянием.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }


        public void SetGameState(int difficulty, string abcId)
        {
            game = new GameClass((Dificulty)difficulty, abcId, DBconnectionPath.Local);            
            this.DefaultViewModel["Words"] = game.Level.words;

            tbQuestion.Text = game.Level.Question;
            pbLevels.Maximum = game.LevelsCount;
        }

        public void GoBack( )
        {
            if (GameParamsPopup!=null) GameParamsPopup.IsOpen = false;
            this.Frame.Navigate(typeof(ChooseGamePage));
        }

        private new void GoBack(object sender, RoutedEventArgs e)
        {
            if (AnswerResultPopup != null)
            {
                AnswerResultPopup.IsOpen = false;
            }
            GoBack();
        }



        private void btnNextLevel_Click(object sender, RoutedEventArgs e)
        {
            if (GameParamsPopup != null) return;
            if (this.game.NextLevel(this.gvAnswers.SelectedIndex))
            {
                if (AnswerResultPopup == null)
                {
                    AnswerResultPopup = new Popup();
                    AnswerResultPopup.Closed += (senderPopup, argsPopup) =>
                    {
                        AnswerResultPopup = null;
                    };

                    AnswerResultPopup.HorizontalOffset = 30;
                    AnswerResultPopup.VerticalOffset = (Window.Current.Bounds.Height - 500);

                    // set the content to our UserControl
                    AnswerResultPopup.Child = new GameAnswerControl(true);

                    // open the Popup
                    AnswerResultPopup.IsOpen = true;

                }
                audioCongrats.Play();              

                this.DefaultViewModel["Words"] = game.Level.words;
                tbQuestion.Text = game.Level.Question;
                tbScores.Text = game.Score.ToString();
                pbLevels.Value++;
            }

            else
            {

                if (AnswerResultPopup == null)
                {
                    AnswerResultPopup = new Popup();
                    AnswerResultPopup.Closed += (senderPopup, argsPopup) =>
                    {
                        AnswerResultPopup = null;
                    };

                    AnswerResultPopup.HorizontalOffset = 30;
                    AnswerResultPopup.VerticalOffset = (Window.Current.Bounds.Height - 500 );

                    // set the content to our UserControl
                    AnswerResultPopup.Child = new GameAnswerControl(false);

                    
                    // open the Popup
                    AnswerResultPopup.IsOpen = true;
                }
                audioSorry.Play();
            }

            if (game.IsLastLevel)
            {
                if (AnswerResultPopup != null)
                {
                    AnswerResultPopup.IsOpen = false;
                }
               
                this.Frame.Navigate(typeof(GameResultPage), (double)game.Score / game.MaxScore);
            }
        }

        

        private async void gvAnswers_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (WordItem)e.ClickedItem;
            gvAnswers.SelectedItem = clickedItem;

            if (game.DifficultyLevel == Dificulty.Easy||game.DifficultyLevel == Dificulty.Medium)
            {                
                MediaElement snd = new MediaElement();
                snd.AudioCategory = AudioCategory.GameEffects;
                StorageFile file = await StorageFile.GetFileFromPathAsync(clickedItem.Sound);
                var stream = await file.OpenAsync(FileAccessMode.Read);
                snd.SetSource(stream, file.ContentType);                
                snd.Play();
            }
        }

        private void audioCongrats_MediaEnded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (AnswerResultPopup != null)
            {
                AnswerResultPopup.IsOpen = false;
            }
        }

        private void pageRoot_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            if (e.Key == Windows.System.VirtualKey.Escape) this.GoBack(this, args);
            else if (e.Key == Windows.System.VirtualKey.Enter) this.btnNextLevel_Click(this, args);
        }

    }
}

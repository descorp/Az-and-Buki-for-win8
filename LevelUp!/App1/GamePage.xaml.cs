using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
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
        Popup DownLoadABCPopup;

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
            if (pageState == null||pageState.Count==0)
            {

                if (ContentManager.AlphabetsCount(DBconnectionPath.Local) == 0)
                {
                    pbLevels.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    btnNextLevel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;


                    if (DownLoadABCPopup == null)
                    {
                        // create the Popup in code
                        DownLoadABCPopup = new Popup();


                        // we are creating this in code and need to handle multiple instances
                        // so we are attaching to the Popup.Closed event to remove our reference
                        DownLoadABCPopup.Closed += (senderPopup, argsPopup) =>
                        {
                            DownLoadABCPopup = null;
                        };

                        DownLoadABCPopup.HorizontalOffset = (Window.Current.Bounds.Width - 600) / 2;
                        DownLoadABCPopup.VerticalOffset = 350;

                        // set the content to our UserControl
                        var res = new ResourceLoader();

                        var chidPopup = new TextPopup(res.GetString("ABCIsEmptyMessage"), true);
                        chidPopup.OKClickEvent += DownLoadABCClicked;
                        DownLoadABCPopup.Child = chidPopup;

                        // open the Popup
                        DownLoadABCPopup.IsOpen = true;
                    }
                }
                else
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
            else
            {
                game = new GameClass();
                game.LoadGameState(pageState);
                this.DefaultViewModel["Words"] = game.Level.words;
                tbQuestion.Text = game.Level.Question;
                pbLevels.Maximum = game.LevelsCount;
                pbLevels.Value = game.LevelNum;
                tbScores.Text = game.Score.ToString();
            }
        }

        private void DownLoadABCClicked(object sender, EventArgs args)
        {
            if (!HttpProvider.IsInternetConnection())
            {
                var res = new ResourceLoader();
                Logger.ShowMessage(res.GetString("NoInternetConnectionError"));
                return;
            }
            this.Frame.Navigate(typeof(DownloadsPage), (int)DownloadPageState.ChoosePacks);
        }

        /// <summary>
        /// Сохраняет состояние, связанное с данной страницей, в случае приостановки приложения или
        /// удаления страницы из кэша навигации. Значения должны соответствовать требованиям сериализации
        /// <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">Пустой словарь, заполняемый сериализуемым состоянием.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            if (game != null) game.SaveGameState(pageState);
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

using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237
using Windows.UI.Xaml.Input;
using levelupspace.Common;
using levelupspace.DataModel;

namespace levelupspace
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class AlphabetDetailsPage : LayoutAwarePage
    {
        public AlphabetDetailsPage()
        {
            InitializeComponent();
            
        }

        private String _abCid;

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
               _abCid = (String) navigationParameter;
            else _abCid = (String)pageState["alphabetDetailPageID"];
            
            var abc = ContentManager.GetAlphabet(_abCid, DBconnectionPath.Local);
            if (abc != null)
            {
                DefaultViewModel["Alphabet"] = abc;
                DefaultViewModel["Items"] = abc.LetterItems;
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
            if (_abCid.Length > 0) pageState["alphabetDetailPageID"] = _abCid;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((LetterItem)e.ClickedItem).UniqueId;
            Frame.Navigate(typeof(LetterPage), itemId);
        }

        private void pageRoot_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var args = new RoutedEventArgs();
            if (e.Key == VirtualKey.Escape) GoBack(this, args);
        }
    }
}

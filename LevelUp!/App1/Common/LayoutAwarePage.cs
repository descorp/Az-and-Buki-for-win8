using System.Collections;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace levelupspace.Common
{
    /// <summary>
    /// Обычная реализация объекта Page, предоставляющая несколько важных и удобных возможностей:
    /// <list type="bullet">
    /// <item>
    /// <description>Сопоставление состояния просмотра приложения с визуальным состоянием</description>
    /// </item>
    /// <item>
    /// <description>Обработчики событий GoBack, GoForward и GoHome</description>
    /// </item>
    /// <item>
    /// <description>Сочетания клавиш и щелчки мышью для навигации</description>
    /// </item>
    /// <item>
    /// <description>Управление состоянием для навигации и управления жизненным циклом процессов</description>
    /// </item>
    /// <item>
    /// <description>Модель представления по умолчанию</description>
    /// </item>
    /// </list>
    /// </summary>
    [WebHostHidden]
    public class LayoutAwarePage : Page
    {
        /// <summary>
        /// Определяет свойство зависимостей <see cref="DefaultViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty DefaultViewModelProperty =
            DependencyProperty.Register("DefaultViewModel", typeof(IObservableMap<String, Object>),
            typeof(LayoutAwarePage), null);

        private List<Control> _layoutAwareControls;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="LayoutAwarePage"/>.
        /// </summary>
        public LayoutAwarePage()
        {
            if (DesignMode.DesignModeEnabled) return;

            // Создание пустой модели представления по умолчанию
            DefaultViewModel = new ObservableDictionary<String, Object>();

            // Если данная страница является частью визуального дерева, возникают два изменения:
            // 1) Сопоставление состояния просмотра приложения с визуальным состоянием для страницы.
            // 2) Обработка запросов навигации с помощью мыши и клавиатуры.
            Loaded += (sender, e) =>
            {
                StartLayoutUpdates(sender, e);

                // Навигация с помощью мыши и клавиатуры применяется, только если страница занимает все окно
                if (ActualHeight == Window.Current.Bounds.Height &&
                    ActualWidth == Window.Current.Bounds.Width)
                {
                    // Непосредственное прослушивание окна, поэтому фокус не требуется
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                        CoreDispatcher_AcceleratorKeyActivated;
                    Window.Current.CoreWindow.PointerPressed +=
                        CoreWindow_PointerPressed;
                }
            };

            // Отмена тех же изменений, когда страница перестает быть видимой
            Unloaded += (sender, e) =>
            {
                StopLayoutUpdates(sender, e);
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -=
                    CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -=
                    CoreWindow_PointerPressed;
            };
        }

        /// <summary>
        /// Реализация интерфейса <see cref="IObservableMap&lt;String, Object&gt;"/>, предназначенного для
        /// использования в качестве тривиальной модели представления.
        /// </summary>
        protected IObservableMap<String, Object> DefaultViewModel
        {
            get
            {
                return GetValue(DefaultViewModelProperty) as IObservableMap<String, Object>;
            }

            set
            {
                SetValue(DefaultViewModelProperty, value);
            }
        }

        #region Поддержка навигации

        /// <summary>
        /// Вызывается как обработчик событий для перехода назад в связанном со страницей фрейме
        /// <see cref="Frame"/> до достижения верхнего элемента стека навигации.
        /// </summary>
        /// <param name="sender">Экземпляр, инициировавший событие.</param>
        /// <param name="e">Данные события, описывающие условия, которые привели к возникновению события.</param>
        protected virtual void GoHome(object sender, RoutedEventArgs e)
        {
            // Используйте фрейм навигации для возврата на самую верхнюю страницу
            if (Frame != null)
            {
                while (Frame.CanGoBack) Frame.GoBack();
            }
        }

        /// <summary>
        /// Вызывается как обработчик событий для перехода назад в стеке навигации,
        /// связанном со фреймом <see cref="Frame"/> данной страницы.
        /// </summary>
        /// <param name="sender">Экземпляр, инициировавший событие.</param>
        /// <param name="e">Данные события, описывающие условия, которые привели к
        /// возникновению события.</param>
        protected virtual void GoBack(object sender, RoutedEventArgs e)
        {
            // Используйте фрейм навигации для возврата на предыдущую страницу
            if (Frame != null && Frame.CanGoBack) Frame.GoBack();
        }

        /// <summary>
        /// Вызывается как обработчик событий для перехода вперед в стеке навигации
        /// связанном со фреймом <see cref="Frame"/> данной страницы.
        /// </summary>
        /// <param name="sender">Экземпляр, инициировавший событие.</param>
        /// <param name="e">Данные события, описывающие условия, которые привели к
        /// возникновению события.</param>
        protected virtual void GoForward(object sender, RoutedEventArgs e)
        {
            // Используйте фрейм навигации для перехода на следующую страницу
            if (Frame != null && Frame.CanGoForward) Frame.GoForward();
        }

        /// <summary>
        /// Вызывается при каждом нажатии клавиши, включая системные клавиши, такие как клавиша ALT, если
        /// данная страница активна и занимает все окно. Используется для обнаружения навигации с помощью клавиатуры
        /// между страницами, даже если сама страница не имеет фокуса.
        /// </summary>
        /// <param name="sender">Экземпляр, инициировавший событие.</param>
        /// <param name="args">Данные события, описывающие условия, которые привели к возникновению события.</param>
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender,
            AcceleratorKeyEventArgs args)
        {
            var virtualKey = args.VirtualKey;

            // Дальнейшее изучение следует выполнять, только если нажата клавиша со стрелкой влево или вправо либо назначенная клавиша "Назад" или
            // "Вперед"
            if ((args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                args.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                const CoreVirtualKeyStates downState = CoreVirtualKeyStates.Down;
                var menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                var controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                var shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                var noModifiers = !menuKey && !controlKey && !shiftKey;
                var onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // Переход назад при нажатии клавиши "Назад" или сочетания клавиш ALT+стрелка влево
                    args.Handled = true;
                    GoBack(this, new RoutedEventArgs());
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // Переход вперед при нажатии клавиши "Вперед" или сочетания клавиш ALT+стрелка вправо
                    args.Handled = true;
                    GoForward(this, new RoutedEventArgs());
                }
            }
        }

        /// <summary>
        /// Вызывается при каждом щелчке мыши, касании сенсорного экрана или аналогичном действии, если эта
        /// страница активна и занимает все окно. Используется для обнаружения нажатий мышью кнопок "Вперед" и
        /// "Назад" в браузере для перехода между страницами.
        /// </summary>
        /// <param name="sender">Экземпляр, инициировавший событие.</param>
        /// <param name="args">Данные события, описывающие условия, которые привели к возникновению события.</param>
        private void CoreWindow_PointerPressed(CoreWindow sender,
            PointerEventArgs args)
        {
            var properties = args.CurrentPoint.Properties;

            // Пропуск сочетаний кнопок, включающих левую, правую и среднюю кнопки
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // Если нажата кнопка "Назад" или "Вперед" (но не обе), выполняется соответствующий переход
            var backPressed = properties.IsXButton1Pressed;
            var forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                args.Handled = true;
                if (backPressed) GoBack(this, new RoutedEventArgs());
                if (forwardPressed) GoForward(this, new RoutedEventArgs());
            }
        }

        #endregion

        #region Переключение визуальных состояний

        /// <summary>
        /// Вызывается в качестве обработчика событий, как правило, для события <see cref="FrameworkElement.Loaded"/>
        /// элемента управления <see cref="Control"/> на странице для указания того, что отправитель должен
        /// начать получать изменения управления визуальным состоянием, соответствующие изменениям состояния просмотра
        /// приложения.
        /// </summary>
        /// <param name="sender">Экземпляр <see cref="Control"/>, который поддерживает управление состоянием просмотра,
        /// соответствующее состояниям просмотра.</param>
        /// <param name="e">Данные события, описывающие способ выполнения запроса.</param>
        /// <remarks>Текущее состояние просмотра будет немедленно использоваться для задания соответствующего
        /// визуального состояния при запросе обновлений макета. Настоятельно рекомендуется
        /// использовать обработчик событий <see cref="FrameworkElement.Unloaded"/>, подключенный к
        /// объекту <see cref="StopLayoutUpdates"/>. Экземпляры
        /// <see cref="LayoutAwarePage"/> автоматически вызывают эти обработчики в своих событиях Loaded и
        /// Unloaded.</remarks>
        /// <seealso cref="DetermineVisualState"/>
        /// <seealso cref="InvalidateVisualState"/>
        public void StartLayoutUpdates(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            if (_layoutAwareControls == null)
            {
                // Запуск прослушивания изменений состояния просмотра при наличии элементов управления, заинтересованных в обновлениях
                Window.Current.SizeChanged += WindowSizeChanged;
                _layoutAwareControls = new List<Control>();
            }
            _layoutAwareControls.Add(control);

            // Задает начальное визуальное состояние элемента управления
            VisualStateManager.GoToState(control, DetermineVisualState(ApplicationView.Value), false);
        }

        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            InvalidateVisualState();
        }

        /// <summary>
        /// Вызывается в качестве обработчика событий, как правило, для события <see cref="FrameworkElement.Unloaded"/>
        /// элемента управления <see cref="Control"/> для указания того, что отправитель должен начать получать
        /// изменения управления визуальным состоянием, соответствующие изменениям состояния просмотра приложения.
        /// </summary>
        /// <param name="sender">Экземпляр <see cref="Control"/>, который поддерживает управление состоянием просмотра,
        /// соответствующее состояниям просмотра.</param>
        /// <param name="e">Данные события, описывающие способ выполнения запроса.</param>
        /// <remarks>Текущее состояние просмотра будет немедленно использоваться для задания соответствующего
        /// визуальное состояние при запросе обновлений макета.</remarks>
        /// <seealso cref="StartLayoutUpdates"/>
        public void StopLayoutUpdates(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null || _layoutAwareControls == null) return;
            _layoutAwareControls.Remove(control);
            if (_layoutAwareControls.Count == 0)
            {
                // Остановка прослушивания изменений состояния просмотра при отсутствии элементов управления, заинтересованных в обновлениях
                _layoutAwareControls = null;
                Window.Current.SizeChanged -= WindowSizeChanged;
            }
        }

        /// <summary>
        /// Преобразует значения <see cref="ApplicationViewState"/> в строки для управления визуальным состоянием
        /// на странице. Реализация по умолчанию использует имена значений перечисления.
        /// Этот метод может переопределяться подклассами для управления используемой схемой сопоставления.
        /// </summary>
        /// <param name="viewState">Состояние просмотра, для которого требуется визуальное состояние.</param>
        /// <returns>Имя визуального состояния, используемое для инициирования
        /// <see cref="VisualStateManager"/></returns>
        /// <seealso cref="InvalidateVisualState"/>
        protected virtual string DetermineVisualState(ApplicationViewState viewState)
        {
            return viewState.ToString();
        }

        /// <summary>
        /// Обновляет все элементы управления, прослушивающие изменения визуального состояния, соответствующим
        /// визуальным состоянием.
        /// </summary>
        /// <remarks>
        /// Обычно используется вместе с переопределяющим <see cref="DetermineVisualState"/> для
        /// указания на возможность возвращения другого значения даже при отсутствии изменений состояния
        /// просмотра.
        /// </remarks>
        public void InvalidateVisualState()
        {
            if (_layoutAwareControls != null)
            {
                var visualState = DetermineVisualState(ApplicationView.Value);
                foreach (var layoutAwareControl in _layoutAwareControls)
                {
                    VisualStateManager.GoToState(layoutAwareControl, visualState, false);
                }
            }
        }

        #endregion

        #region Управление жизненным циклом процесса

        private String _pageKey;

        /// <summary>
        /// Вызывается перед отображением этой страницы во фрейме.
        /// </summary>
        /// <param name="e">Данные о событиях, описывающие, каким образом была достигнута эта страница.  Свойство Parameter
        /// задает группу для отображения.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += Settings_CommandRequested;

            // Возвращение к кэшированной странице во время навигации не должно инициировать загрузку состояния
            if (_pageKey != null) return;

            var frameState = SuspensionManager.SessionStateForFrame(Frame);
            _pageKey = "Page-" + Frame.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                // Очистка существующего состояния для перехода вперед при добавлении новой страницы в
                // стек навигации
                var nextPageKey = _pageKey;
                var nextPageIndex = Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                // Передача параметра навигации на новую страницу
                LoadState(e.Parameter, null);
            }
            else
            {
                // Передача на страницу параметра навигации и сохраненного состояния страницы с использованием
                // той же стратегии загрузки приостановленного состояния и повторного создания страниц, удаленных
                // из кэша
                LoadState(e.Parameter, (Dictionary<String, Object>)frameState[_pageKey]);
            }
        }

        /// <summary>
        /// Вызывается, если данная страница больше не отображается во фрейме.
        /// </summary>
        /// <param name="e">Данные о событиях, описывающие, каким образом была достигнута эта страница.  Свойство Parameter
        /// задает группу для отображения.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= Settings_CommandRequested;
            var frameState = SuspensionManager.SessionStateForFrame(Frame);
            var pageState = new Dictionary<String, Object>();
            SaveState(pageState);
            frameState[_pageKey] = pageState;
        }

        private void Settings_CommandRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var res = new ResourceLoader();
            var viewPrivacyPage = new SettingsCommand("", res.GetString("PrivacyStatementCaption"), cmd => Launcher.LaunchUriAsync(new Uri(res.GetString("UriPrivacyPolicy"), UriKind.Absolute)));
            args.Request.ApplicationCommands.Add(viewPrivacyPage);

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
        protected virtual void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Сохраняет состояние, связанное с данной страницей, в случае приостановки приложения или
        /// удаления страницы из кэша навигации. Значения должны соответствовать требованиям сериализации
        /// <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">Пустой словарь, заполняемый сериализуемым состоянием.</param>
        protected virtual void SaveState(Dictionary<String, Object> pageState)
        {
        }

        #endregion

        /// <summary>
        /// Реализация интерфейса IObservableMap, поддерживающего повторный вход для использования в качестве модели представления
        /// по умолчанию.
        /// </summary>
        private class ObservableDictionary<TK, V> : IObservableMap<TK, V>
        {
            private class ObservableDictionaryChangedEventArgs : IMapChangedEventArgs<TK>
            {
                public ObservableDictionaryChangedEventArgs(CollectionChange change, TK key)
                {
                    CollectionChange = change;
                    Key = key;
                }

                public CollectionChange CollectionChange { get; private set; }
                public TK Key { get; private set; }
            }

            private readonly Dictionary<TK, V> _dictionary = new Dictionary<TK, V>();
            public event MapChangedEventHandler<TK, V> MapChanged;

            private void InvokeMapChanged(CollectionChange change, TK key)
            {
                var eventHandler = MapChanged;
                if (eventHandler != null)
                {
                    eventHandler(this, new ObservableDictionaryChangedEventArgs(change, key));
                }
            }

            public void Add(TK key, V value)
            {
                _dictionary.Add(key, value);
                InvokeMapChanged(CollectionChange.ItemInserted, key);
            }

            public void Add(KeyValuePair<TK, V> item)
            {
                Add(item.Key, item.Value);
            }

            public bool Remove(TK key)
            {
                if (_dictionary.Remove(key))
                {
                    InvokeMapChanged(CollectionChange.ItemRemoved, key);
                    return true;
                }
                return false;
            }

            public bool Remove(KeyValuePair<TK, V> item)
            {
                V currentValue;
                if (_dictionary.TryGetValue(item.Key, out currentValue) &&
                    Equals(item.Value, currentValue) && _dictionary.Remove(item.Key))
                {
                    InvokeMapChanged(CollectionChange.ItemRemoved, item.Key);
                    return true;
                }
                return false;
            }

            public V this[TK key]
            {
                get
                {
                    return _dictionary[key];
                }
                set
                {
                    _dictionary[key] = value;
                    InvokeMapChanged(CollectionChange.ItemChanged, key);
                }
            }

            public void Clear()
            {
                var priorKeys = _dictionary.Keys.ToArray();
                _dictionary.Clear();
                foreach (var key in priorKeys)
                {
                    InvokeMapChanged(CollectionChange.ItemRemoved, key);
                }
            }

            public ICollection<TK> Keys
            {
                get { return _dictionary.Keys; }
            }

            public bool ContainsKey(TK key)
            {
                return _dictionary.ContainsKey(key);
            }

            public bool TryGetValue(TK key, out V value)
            {
                return _dictionary.TryGetValue(key, out value);
            }

            public ICollection<V> Values
            {
                get { return _dictionary.Values; }
            }

            public bool Contains(KeyValuePair<TK, V> item)
            {
                return _dictionary.Contains(item);
            }

            public int Count
            {
                get { return _dictionary.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public IEnumerator<KeyValuePair<TK, V>> GetEnumerator()
            {
                return _dictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _dictionary.GetEnumerator();
            }

            public void CopyTo(KeyValuePair<TK, V>[] array, int arrayIndex)
            {
                var arraySize = array.Length;
                foreach (var pair in _dictionary)
                {
                    if (arrayIndex >= arraySize) break;
                    array[arrayIndex++] = pair;
                }
            }
        }
    }
}

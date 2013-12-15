using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// Документацию по шаблону элемента "Элемент управления на основе шаблона" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234235

namespace levelupspace.Controls
{
    [TemplatePart(Name = "fvCH1", Type = typeof(FlipView))]
    [TemplatePart(Name = "fvCH2", Type = typeof(FlipView))]
    [TemplatePart(Name = "fvCH3", Type = typeof(FlipView))]
    [TemplatePart(Name = "fvCH4", Type = typeof(FlipView))]
    public sealed class SlotMachineControl : Control
    {
        private static readonly string[] FlipViewName = {"fvCH1", "fvCH2", "fvCH3", "fvCH4"};
        

        private readonly List<FlipView> _fwList;

        public SlotMachineControl()
        {
            DefaultStyleKey = typeof(SlotMachineControl);
            _fwList = new List<FlipView>();
        }

        public Brush fvBorderBrush
        {

            get { return (Brush)GetValue(fvBorderBrushProperty); }

            set { SetValue(fvBorderBrushProperty, value); }

        }
        
        public static readonly DependencyProperty fvBorderBrushProperty =
            DependencyProperty.Register("fvBorderBrush", typeof(Brush), typeof(SlotMachineControl), new PropertyMetadata(null));

        public DataTemplate SlotMashineItemTemplate
        {

            get { return (DataTemplate)GetValue(SlotMashineItemTemplateProperty); }

            set { SetValue(SlotMashineItemTemplateProperty, value); }

        }

        public static readonly DependencyProperty SlotMashineItemTemplateProperty =
            DependencyProperty.Register("SlotMashineItemTemplate", typeof(DataTemplate), typeof(SlotMachineControl), new PropertyMetadata(null));

        public Object SMItemSource1
        {

            get { return _fwList[0].ItemsSource; }

            set { _fwList[0].ItemsSource = value; }

        }


        public Object SMItemSource2
        {

            get { return _fwList[1].ItemsSource; }

            set { _fwList[1].ItemsSource = value; }

        }


        public Object SMItemSource3
        {

            get { return _fwList[2].ItemsSource; }

            set { _fwList[2].ItemsSource = value; }

        }

       
        public Object SMItemSource4
        {

            get { return _fwList[3].ItemsSource; }

            set { _fwList[3].ItemsSource = value; }

        }

        
        public string Key
        {
            
            get
            {
                return _fwList.Aggregate("", (current, t) => current + t.SelectedIndex.ToString());
            }
        }
        protected override void OnApplyTemplate()
        {
            for (var i = 0; i < FlipViewName.Count(); i++ )
                _fwList.Add((FlipView)GetTemplateChild(FlipViewName[i]));
            base.OnApplyTemplate();
        }
    }

    
}

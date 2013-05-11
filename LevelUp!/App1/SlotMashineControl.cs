using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// Документацию по шаблону элемента "Элемент управления на основе шаблона" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234235

namespace SlotMachineControl
{
    [TemplatePart(Name = "fvCH1", Type = typeof(FlipView))]
    [TemplatePart(Name = "fvCH2", Type = typeof(FlipView))]
    [TemplatePart(Name = "fvCH3", Type = typeof(FlipView))]
    [TemplatePart(Name = "fvCH4", Type = typeof(FlipView))]
    public sealed class SlotMachineControl : Control
    {
        private static string[] FlipViewName = new string[4] {"fvCH1", "fvCH2", "fvCH3", "fvCH4"};
        

        private List<FlipView> fwList;

        public SlotMachineControl()
        {
            this.DefaultStyleKey = typeof(SlotMachineControl);
            fwList = new List<FlipView>();
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

            get { return this.fwList[0].ItemsSource; }

            set { this.fwList[0].ItemsSource = value; }

        }


        public Object SMItemSource2
        {

            get { return this.fwList[1].ItemsSource; }

            set { this.fwList[1].ItemsSource = value; }

        }


        public Object SMItemSource3
        {

            get { return this.fwList[2].ItemsSource; }

            set { this.fwList[2].ItemsSource = value; }

        }

       
        public Object SMItemSource4
        {

            get { return this.fwList[3].ItemsSource; }

            set { this.fwList[3].ItemsSource = value; }

        }

        
        public string Key
        {
            
            get {
                String key = "";
                for (int i = 0; i < fwList.Count; i++)
                    key += fwList[i].SelectedIndex.ToString();
                    return key; 
            }
        }
        protected override void OnApplyTemplate()
        {
            for (int i = 0; i < FlipViewName.Count(); i++ )
                this.fwList.Add((FlipView)this.GetTemplateChild(FlipViewName[i]));
            base.OnApplyTemplate();
        }
    }

    
}

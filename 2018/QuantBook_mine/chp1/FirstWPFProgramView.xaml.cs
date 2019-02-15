using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace QuantBook1_mine.Ch01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FirstWPFProgramView : UserControl 
    {

        public FirstWPFProgramView()
        {
            InitializeComponent();
        }

        private void txBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            txBlock.Text = txBox.Text ;
        }

        private void btnChangeColor_Click(object sender, RoutedEventArgs e)
        {
            if (txBlock.Foreground == Brushes.Black)
                txBlock.Foreground = Brushes.BlueViolet;
            else
                txBlock.Foreground = Brushes.Black;
        }

        private void btnChangeSize_Click(object sender, RoutedEventArgs e)
        {
            if (txBlock.FontSize != 11)
            {
                txBlock.FontSize = 11;
            }
            else
                txBlock.FontSize = 24;

        }
    }
}

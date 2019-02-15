using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuantBook_mine.Ch01
{
    /// <summary>
    /// Interaction logic for CodeOnlyView.xaml
    /// </summary>
    public partial class CodeOnlyView : UserControl
    {
        //declaring fields
        private TextBlock txBlock;
        private TextBox txBox;

        //constructor
        public CodeOnlyView()
        {
            Initialization();
        }

        private void Initialization()
        {
            //configure the UserControl
            this.Height = 450;
            this.Width = 800;
            
            //create a grid and stackPanel and add them to the user control
            var grid = new Grid();
            var stackPanel = new StackPanel();
            grid.Children.Add(stackPanel);
            this.Content = grid; // the grid is assigned to this (UserControl) content

            // add a text block to the stack panel
            txBlock = new TextBlock();
            txBlock.Margin = new Thickness(5);
            txBlock.Height = 30;
            txBlock.TextAlignment = TextAlignment.Center;
            txBlock.Text = "Hello WPF";
            stackPanel.Children.Add(txBlock);

            // add text box to the stack panel
            txBox = new TextBox();
            txBox.Margin = new Thickness(5);
            txBox.TextAlignment = TextAlignment.Center;
            txBox.TextChanged += OnTextChanged;
            stackPanel.Children.Add(txBox);

            //add button to the stackpanel for changing text color
            var btnColor = new Button();
            btnColor.Margin = new Thickness(5);  
            btnColor.Width = 100;
            btnColor.Content = "Change Text Color";
            btnColor.Click += btnChangeColor_Click;
            stackPanel.Children.Add(btnColor);

            var btnSize = new Button();
            btnSize.Margin = new Thickness(5);
            btnSize.Width = 100;
            btnSize.Content = "Change Text Size";
            btnSize.Click += btnChangeSize_Click;
            stackPanel.Children.Add(btnSize);




        }

       

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            txBlock.Text = txBox.Text;
        }

        private void btnChangeColor_Click(object sender, RoutedEventArgs e)
        {
            if (txBlock.Foreground == Brushes.Black)
                txBlock.Foreground = Brushes.Red;
            else
            {
                txBlock.Foreground = Brushes.Black;
            }
        }

        private void btnChangeSize_Click(object sender, RoutedEventArgs e)
        {
           if(txBox.FontSize == 11)
               txBox.FontSize = 14;
           else
           {
               txBox.FontSize = 11;
           }
        }
    }
}

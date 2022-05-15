using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Notepad__.View
{
    public partial class SearchBox : Window
    {
        public SearchBox()
        {
            InitializeComponent();
        }

        public List<string> GetInputList()
        {
            IEnumerable<TextBox> textBoxes = SearchBoxGrid.Children.OfType<TextBox>();
            var inputs = new List<string>();
            foreach (var textBox in textBoxes)
                inputs.Add(textBox.Text);
            inputs.Add(SearchAll.IsChecked == true ? "True" : "False");
            return inputs;
        }

        public void CreateSearchBox(List<string> values, bool enableCheckBox)
        {
            SearchBoxWindow.Height = (values.Count * 75) + 100;
            if (values.Count == 1)
                SearchBoxWindow.Height += 25;
            int rowIndex = 1;

            if (!enableCheckBox)
                SearchAll.Visibility = Visibility.Hidden;

            foreach (var value in values)
            {
                var textBlock = new TextBlock();
                var textBox = new TextBox();

                ThicknessConverter thicknessConverter = new ThicknessConverter();
                textBlock.Text = value;
                textBlock.FontSize = 14;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBox.Margin = (Thickness)thicknessConverter.ConvertFrom(5);

                SearchBoxGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                SearchBoxGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                Grid.SetRow(textBlock, rowIndex++);
                SearchBoxGrid.Children.Add(textBlock);

                Grid.SetRow(textBox, rowIndex++);
                SearchBoxGrid.Children.Add(textBox);
            }
        }
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBoxWindow.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}

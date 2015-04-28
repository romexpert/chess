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
using System.Windows.Shapes;
using Common;

namespace Chess
{
    /// <summary>
    /// Interaction logic for ChooseFeagure.xaml
    /// </summary>
    public partial class ChooseFeagure : Window
    {
        public ChooseFeagure()
        {
            InitializeComponent();
        }

        public FeagureType? _type;
        public FeagureType SelectedFeagure { get { return _type.Value; } }
        
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var feagures = new object[] { "Select feagure...", FeagureType.Bishop, FeagureType.Knight, FeagureType.Queen, FeagureType.Rook};

            ((ComboBox) sender).ItemsSource = feagures;
            ((ComboBox) sender).SelectedIndex = 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_type.HasValue)
                e.Cancel = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox) sender).SelectedIndex == 0)
                return;

            _type = (FeagureType)e.AddedItems[0];
            Close();
        }
    }
}

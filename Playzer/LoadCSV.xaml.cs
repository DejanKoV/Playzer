using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Playzer
{
    /// <summary>
    /// Interaction logic for LoadCSV.xaml
    /// </summary>
    public partial class LoadCSV : Window
    {
        public LoadCSV(ObservableCollection<TableItem> lista,string fileName)
        {
            InitializeComponent();
            Title = fileName;
            dg.ItemsSource = lista;
        }
    }
}

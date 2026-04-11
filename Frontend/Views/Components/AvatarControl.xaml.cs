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
using Shared.DTOs;

namespace Frontend.Views.Components
{
    /// <summary>
    /// Logique d'interaction pour AvatarControl.xaml
    /// </summary>
    public partial class AvatarControl : UserControl
    {
        

        public AvatarControl()
        {
            InitializeComponent();
            DataContext = new ViewModels.AvatarControlViewModel();
        }
    }
}

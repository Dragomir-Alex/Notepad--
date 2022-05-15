using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Notepad__.View
{
    public partial class AboutBox : Window
    {
        public AboutBox()
        {
            InitializeComponent();
            EmailHyperlink.RequestNavigate += (sender, e) =>
            {
                System.Diagnostics.Process.Start(e.Uri.ToString());
            };
        }
    }
}

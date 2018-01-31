using System.Windows.Controls;

namespace CouplandUI
{
    /// <summary>
    /// Interaction logic for RoundedCapsuleLabelTest.xaml
    /// </summary>
    public partial class RoundedCapsuleLabelText : UserControl
    {
        public RoundedCapsuleLabelText()
        {
            InitializeComponent();
        }
        public RoundedCapsuleLabelText(string caption, string text)
        {
            InitializeComponent();
            LabelContent.Content = caption;
            TextContent.Text = text;            
        }
    }
}

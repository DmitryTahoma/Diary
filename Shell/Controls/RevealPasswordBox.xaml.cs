using System.Windows.Controls;

namespace Shell.Controls
{
    /// <summary>
    /// Логика взаимодействия для RevealPasswordBox.xaml
    /// </summary>
    public partial class RevealPasswordBox : UserControl
    {
        public delegate void OnEnterKeyUpContainer();
        public event OnEnterKeyUpContainer OnEnterKeyUp;

        public RevealPasswordBox()
        {
            InitializeComponent();
            DataContext.OnEnterKeyUp += () => { OnEnterKeyUp?.Invoke(); };
        }
    }
}

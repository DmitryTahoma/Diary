namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class RevealPasswordBoxViewModel : ViewModelBase
    {
        public delegate void OnEnterKeyUpContainer();
        public event OnEnterKeyUpContainer OnEnterKeyUp;

        private TextBox textBox;
        private PasswordBox passwordBox;
        private string password = "";
        private bool isUpdating = false;
        public RevealPasswordBoxViewModel()
        {
            UpdatePassword = new Command(OnUpdatePasswordExecute);
            BoxKeyUp = new Command<KeyEventArgs>(OnBoxKeyUpExecute);
            MyGotFocus = new Command(OnMyGotFocusExecute);
            FindBoxes = new Command<Grid>(OnFindBoxesExecute);
            MouseEnter = new Command<Image>(OnMouseEnterExecute);
            MouseLeave = new Command<Image>(OnMouseLeaveExecute);
            Click = new Command<Image>(OnClickExecute);
        }

        #region Properties

        public bool IsShown
        {
            get { return GetValue<bool>(IsShownProperty); }
            set { SetValue(IsShownProperty, value); OnUpdateIsShown(); }
        }
        public static readonly PropertyData IsShownProperty = RegisterProperty(nameof(IsShown), typeof(bool), false);

        public Visibility TextBoxVisibility
        {
            get { return GetValue<Visibility>(TextBoxVisibilityProperty); }
            set { SetValue(TextBoxVisibilityProperty, value); }
        }
        public static readonly PropertyData TextBoxVisibilityProperty = RegisterProperty(nameof(TextBoxVisibility), typeof(Visibility), Visibility.Collapsed);

        public Visibility PasswordBoxVisibility
        {
            get { return GetValue<Visibility>(PasswordBoxVisibilityProperty); }
            set { SetValue(PasswordBoxVisibilityProperty, value); }
        }
        public static readonly PropertyData PasswordBoxVisibilityProperty = RegisterProperty(nameof(PasswordBoxVisibility), typeof(Visibility), Visibility.Visible);

        public double Size
        {
            get { return GetValue<double>(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }
        public static readonly PropertyData SizeProperty = RegisterProperty(nameof(Size), typeof(double));

        #endregion

        #region Commands

        public Command UpdatePassword { get; private set; }
        private void OnUpdatePasswordExecute()
        {
            if (!isUpdating)
            {
                isUpdating = true;
                if (IsShown)
                {
                    password = textBox.Text;
                    passwordBox.Password = password;
                }
                else
                {
                    password = passwordBox.Password;
                    textBox.Text = password;
                }
                isUpdating = false;
            }
        }

        public Command<KeyEventArgs> BoxKeyUp { get; private set; }
        private void OnBoxKeyUpExecute(KeyEventArgs e)
        {
            if (e.Key == Key.Enter && OnEnterKeyUp != null)
                OnEnterKeyUp();
        }

        public Command MyGotFocus { get; private set; }
        private void OnMyGotFocusExecute()
        {
            if (IsShown)
                textBox.Focus();
            else
                passwordBox.Focus();
        }

        public Command<Grid> FindBoxes { private set; get; }
        private void OnFindBoxesExecute(Grid content)
        {
            passwordBox = (PasswordBox)content.Children[0];
            textBox = (TextBox)content.Children[1];
        }

        public Command<Image> MouseEnter { private set; get; }
        private void OnMouseEnterExecute(Image sender)
        {
            if (!IsShown)
                sender.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Resources/Icons/RPBbgEntered.png"));
            else
                sender.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Resources/Icons/RPBbgCheckedEntered.png"));
        }

        public Command<Image> MouseLeave { private set; get; }
        private void OnMouseLeaveExecute(Image sender)
        {
            if (!IsShown)
                sender.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Resources/Icons/RPBbg.png"));
            else
                sender.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Resources/Icons/RPBbgChecked.png"));
        }

        public Command<Image> Click { private set; get; }
        private void OnClickExecute(Image sender)
        {
            IsShown = !IsShown;
            OnMouseEnterExecute(sender);
            if (IsShown)
            {
                textBox.Focus();
                textBox.SelectionStart = textBox.Text.Length;
            }
            else
            {
                passwordBox.Focus();
                passwordBox.GetType()
                    .GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(passwordBox, new object[] { passwordBox.Password.Length, 0 });
            }
        }

        #endregion

        private void OnUpdateIsShown()
        {
            TextBoxVisibility = IsShown ? Visibility.Visible : Visibility.Collapsed;
            PasswordBoxVisibility = IsShown ? Visibility.Collapsed : Visibility.Visible;
        }

        public string GetPassword() => password;
    }
}
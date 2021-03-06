﻿namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class RevealPasswordBoxViewModel : ViewModelBase
    {
        public delegate void VoidHandler();

        public event VoidHandler OnEnterKeyUp;
        public event VoidHandler OnEscapeKeyUp;
        public event VoidHandler PasswordChanged;

        private TextBox textBox;
        private PasswordBox passwordBox;
        private string password = "";
        private bool isUpdating = false;
        public RevealPasswordBoxViewModel()
        {
            UpdatePassword = new Command(OnUpdatePasswordExecute);
            BoxKeyUp = new Command<KeyEventArgs>(OnBoxKeyUpExecute);
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
            PasswordChanged?.Invoke();
        }

        public Command<KeyEventArgs> BoxKeyUp { get; private set; }
        private void OnBoxKeyUpExecute(KeyEventArgs e)
        {
            if (e.Key == Key.Enter && OnEnterKeyUp != null)
                OnEnterKeyUp();
            else if (e.Key == Key.Escape && OnEscapeKeyUp != null)
                OnEscapeKeyUp();
        }

        public Command<Grid> FindBoxes { private set; get; }
        private void OnFindBoxesExecute(Grid content)
        {
            passwordBox = (PasswordBox)content.Children[0];
            passwordBox.Password = password;
            textBox = (TextBox)content.Children[1];
            textBox.Text = password;
        }

        public Command<Image> MouseEnter { private set; get; }
        private void OnMouseEnterExecute(Image sender)
        {
            if (!IsShown)
                sender.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Resources/Images/RPBbgEntered.png"));
            else
                sender.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Resources/Images/RPBbgCheckedEntered.png"));
        }

        public Command<Image> MouseLeave { private set; get; }
        private void OnMouseLeaveExecute(Image sender)
        {
            if (!IsShown)
                sender.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Resources/Images/RPBbg.png"));
            else
                sender.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Resources/Images/RPBbgChecked.png"));
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

        public void SetPassword(string password)
        {
            this.password = password;
        }

        public void ClearPassword()
        {
            password = "";
            textBox.Text = "";
            passwordBox.Password = "";
            IsShown = false;
        }

        public void Focus()
        {
            if (IsShown)
                textBox.Focus();
            else
                passwordBox.Focus();
        }
    }
}
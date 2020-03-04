namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using System.Windows;

    public class CheckTextBoxViewModel : ViewModelBase
    {
        public CheckTextBoxViewModel()
        {            
        }

        #region Properties

        public bool IsChecked
        {
            get { return GetValue<bool>(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); Decorations = IsChecked ? TextDecorations.Strikethrough : null; }
        }
        public static readonly PropertyData IsCheckedProperty = RegisterProperty(nameof(IsChecked), typeof(bool), false);

        public TextDecorationCollection Decorations
        {
            get { return GetValue<TextDecorationCollection>(DecorationProperty); }
            set { SetValue(DecorationProperty, value); }
        }
        public static readonly PropertyData DecorationProperty = RegisterProperty(nameof(Decorations), typeof(TextDecorationCollection), null);

        public string Text
        {
            get { return GetValue<string>(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly PropertyData TextProperty = RegisterProperty(nameof(Text), typeof(string), null);

        #endregion

        #region Commands

        #endregion
    }
}
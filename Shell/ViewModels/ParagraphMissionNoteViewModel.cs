namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Shell.Controls;
    using ShellModel.Context;
    using System.Linq;
    using System.Windows.Controls;

    public class ParagraphMissionNoteViewModel : ViewModelBase
    {
        StackPanel points = null;

        public ParagraphMissionNoteViewModel()
        {
            AddNew = new Command(OnAddNewExecute);
            Note = new SimpleNoteViewModel();
            BindStackPanel = new Command<StackPanel>(OnBindStackPanelExecute);
        }

        #region Properties

        public SimpleNoteViewModel Note
        {
            get { return GetValue<SimpleNoteViewModel>(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }
        public static readonly PropertyData NoteProperty = RegisterProperty(nameof(Note), typeof(SimpleNoteViewModel), null);

        public ParagraphMission Context
        {
            get { return GetValue<ParagraphMission>(ContextProperty); }
            set { SetValue(ContextProperty, value); Note.Note = value; }
        }
        public static readonly PropertyData ContextProperty = RegisterProperty(nameof(Context), typeof(ParagraphMission), null);

        #endregion

        #region Commands

        public Command AddNew { get; private set; }
        private void OnAddNewExecute()
        {
            Context.Paragraph.AddPoint(new Point("", true));
            CheckTextBox pointControl = new CheckTextBox();
            pointControl.DataContext.ContextPoint = Context.Paragraph.Items.Last();
            points.Children.Insert(points.Children.Count - 1, pointControl);
        }

        public Command<StackPanel> BindStackPanel { get; private set; }
        private void OnBindStackPanelExecute(StackPanel stackPanel)
        {
            points = stackPanel;
            LoadPoints();
        }

        #endregion

        private void LoadPoints()
        {
            foreach(Point point in Context.Paragraph.Items)
            {
                CheckTextBox check = new CheckTextBox();
                check.DataContext.IsChecked = point.IsChecked;
                check.DataContext.Text = point.Text;
                points.Children.Insert(points.Children.Count - 1, check);
            }
        }
    }
}
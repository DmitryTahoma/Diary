﻿namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Shell.Controls;
    using ShellModel.Context;
    using System.Linq;
    using System.Windows.Controls;

    public class ParagraphMissionNoteViewModel : ViewModelBase
    {
        public delegate void VoidHandler();
        public event VoidHandler Deleting;
        public delegate void NoteHandler(Note note);
        public event NoteHandler SelectDate;

        StackPanel points = null;

        public ParagraphMissionNoteViewModel()
        {
            AddNew = new Command(OnAddNewExecute);
            Note = new SimpleNoteViewModel();
            BindStackPanel = new Command<StackPanel>(OnBindStackPanelExecute);
            BindBaseNote = new Command<SimpleNote>(OnBindBaseNoteExecute);
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
            Point point = new Point("", true);
            Context.Paragraph.AddPoint(point);
            CheckTextBox pointControl = new CheckTextBox();
            pointControl.DataContext.ContextPoint = Context.Paragraph.Items.Last();
            points.Children.Insert(points.Children.Count - 1, pointControl);
            pointControl.DataContext.OnDeleteMe += () => 
            {
                Context.Paragraph.RemovePoint(point.Id);
                points.Children.Remove(pointControl);
            };
        }

        bool isBindedStackPanel = false;
        public Command<StackPanel> BindStackPanel { get; private set; }
        private void OnBindStackPanelExecute(StackPanel stackPanel)
        {
            if (!isBindedStackPanel)
            {
                points = stackPanel;
                LoadPoints();
                isBindedStackPanel = true;
            }
        }

        bool isBindedBaseNote = false;
        public Command<SimpleNote> BindBaseNote { get; private set; }
        private void OnBindBaseNoteExecute(SimpleNote simpleNote)
        {
            if (!isBindedBaseNote)
            {
                simpleNote.DataContext.Note = Context;
                simpleNote.DataContext.Deleting += () => { Deleting?.Invoke(); };
                simpleNote.DataContext.SelectDate += (n) => { SelectDate?.Invoke(Context); };
                isBindedBaseNote = true;
            }
        }

        #endregion

        private void LoadPoints()
        {
            for(int i = 0; i < Context.Paragraph.Items.Count; ++i)
            {
                Point point = Context.Paragraph.Items[i];
                Context.Paragraph.AddPoint(point, false);
                CheckTextBox check = new CheckTextBox();
                check.DataContext.ContextPoint = point;
                points.Children.Insert(points.Children.Count - 1, check);
                check.DataContext.OnDeleteMe += () =>
                {
                    Context.Paragraph.RemovePoint(point.Id);
                    points.Children.Remove(check);
                };
            }
        }
    }
}
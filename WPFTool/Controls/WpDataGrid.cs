using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Commands;

namespace WPFTool.Controls
{
    public class WpDataGrid : DataGrid
    {
        static WpDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WpDataGrid),
                new FrameworkPropertyMetadata(typeof(WpDataGrid)));
        }

        public WpDataGrid()
        {
            GoToFirstPageCommand = new DelegateCommand<object>(OnGoToPage);
            GoToLastPageCommand = new DelegateCommand<object>(OnGoToPage);
            GoToPreviousPageCommand = new DelegateCommand(OnGoToPrePage);
            GoToPreviousMultiPageCommand = new DelegateCommand(OnGoToPreMultiPage);
            GoToNextPageCommand = new DelegateCommand(OnGoToNextPage);
            GoToNextMutiPageCommand = new DelegateCommand(OnGoToNextMultiPage);
            LostFocusCommand = new DelegateCommand(ExecuteLostFocus);
        }

        //public ICommand GoToFirstPageCommand { get; }

        #region Properties
        // DependencyProperty cho paging
        public static readonly DependencyProperty PagedItemsSourceProperty =
            DependencyProperty.Register(nameof(PagedItemsSource), typeof(IEnumerable), typeof(WpDataGrid),
                new PropertyMetadata(null));
        public IEnumerable PagedItemsSource
        {
            get => (IEnumerable)GetValue(PagedItemsSourceProperty);
            set => SetValue(PagedItemsSourceProperty, value);
        }

        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(int), typeof(WpDataGrid),
                new PropertyMetadata(1, OnPagingChanged));
        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(WpDataGrid),
                new PropertyMetadata(10, OnPagingChanged));
        public int PageSize
        {
            get => (int)GetValue(PageSizeProperty);
            set => SetValue(PageSizeProperty, value);
        }

        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register(nameof(TotalPages), typeof(int), typeof(WpDataGrid),
                new PropertyMetadata(1, OnTotalPagesChanged));
        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            set => SetValue(TotalPagesProperty, value);
        }

        public static readonly DependencyProperty SourceItemsProperty =
            DependencyProperty.Register(nameof(SourceItems), typeof(IEnumerable), typeof(WpDataGrid),
                new PropertyMetadata(null, OnPagingChanged));
        public IEnumerable SourceItems
        {
            get => (IEnumerable)GetValue(SourceItemsProperty);
            set => SetValue(SourceItemsProperty, value);
        }

        private static readonly DependencyProperty PageListProperty =
            DependencyProperty.Register(nameof(PageList), typeof(List<int>), typeof(WpDataGrid),
                new PropertyMetadata(null));
        public List<int> PageList
        {
            get => (List<int>)GetValue(PageListProperty);
            set => SetValue(PageListProperty, value);

        }
        
        private static readonly DependencyProperty PageSizeListProperty =
            DependencyProperty.Register(nameof(PageSizeList), typeof(int), typeof(WpDataGrid),
                new PropertyMetadata(null));
        public int PageSizeList
        {
            get => (int)GetValue(PageSizeListProperty);
            set => SetValue(PageSizeListProperty, value);

        }

        private static readonly DependencyProperty StartRowProperty =
            DependencyProperty.Register(nameof(StartRow), typeof(int), typeof(WpDataGrid),
                new PropertyMetadata(null));
        public int StartRow
        {
            get => (int)GetValue(StartRowProperty);
            set => SetValue(StartRowProperty, value);

        }

        private static readonly DependencyProperty EndRowProperty =
            DependencyProperty.Register(nameof(EndRow), typeof(int), typeof(WpDataGrid),
                new PropertyMetadata(null));
        public int EndRow
        {
            get => (int)GetValue(EndRowProperty);
            set => SetValue(EndRowProperty, value);

        }
        
        private static readonly DependencyProperty TotalRowProperty =
            DependencyProperty.Register(nameof(TotalRow), typeof(int), typeof(WpDataGrid),
                new PropertyMetadata(null));
        public int TotalRow
        {
            get => (int)GetValue(TotalRowProperty);
            set => SetValue(TotalRowProperty, value);

        }
        
        #endregion

        #region Command
        public static readonly DependencyProperty GoToFirstPageCommandProperty =
            DependencyProperty.Register(nameof(GoToFirstPageCommand), typeof(ICommand), typeof(WpDataGrid));
        public ICommand GoToFirstPageCommand
        {
            get => (ICommand)GetValue(GoToFirstPageCommandProperty);
            set => SetValue(GoToFirstPageCommandProperty, value);
        }

        public static readonly DependencyProperty GoToLastPageCommandProperty =
            DependencyProperty.Register(nameof(GoToLastPageCommand), typeof(ICommand), typeof(WpDataGrid));
        public ICommand GoToLastPageCommand
        {
            get => (ICommand)GetValue(GoToLastPageCommandProperty);
            set => SetValue(GoToLastPageCommandProperty, value);
        }

        public static readonly DependencyProperty GoToPreviousPageCommandProperty =
            DependencyProperty.Register(nameof(GoToPreviousPageCommand), typeof(ICommand), typeof(WpDataGrid));
        public ICommand GoToPreviousPageCommand
        {
            get => (ICommand)GetValue(GoToPreviousPageCommandProperty);
            set => SetValue(GoToPreviousPageCommandProperty, value);
        }

        public static readonly DependencyProperty GoToNextPageCommandProperty =
            DependencyProperty.Register(nameof(GoToNextPageCommand), typeof(ICommand), typeof(WpDataGrid));
        public ICommand GoToNextPageCommand
        {
            get => (ICommand)GetValue(GoToNextPageCommandProperty);
            set => SetValue(GoToNextPageCommandProperty, value);
        }

        public static readonly DependencyProperty GoToPreviousMultiPageCommandProperty =
            DependencyProperty.Register(nameof(GoToPreviousMultiPageCommand), typeof(ICommand), typeof(WpDataGrid));
        public ICommand GoToPreviousMultiPageCommand
        {
            get => (ICommand)GetValue(GoToPreviousMultiPageCommandProperty);
            set => SetValue(GoToPreviousMultiPageCommandProperty, value);
        }

        public static readonly DependencyProperty GoToNextMultiPageCommandProperty =
            DependencyProperty.Register(nameof(GoToNextMutiPageCommand), typeof(ICommand), typeof(WpDataGrid));
        public ICommand GoToNextMutiPageCommand
        {
            get => (ICommand)GetValue(GoToNextMultiPageCommandProperty);
            set => SetValue(GoToNextMultiPageCommandProperty, value);
        }
        
        public ICommand LostFocusCommand { get; }

        #endregion

        #region Method
        private static void OnPagingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as WpDataGrid;
            control?.ApplyPaging();
        }

        private void ApplyPaging()
        {
            if (SourceItems == null) return;

            var list = SourceItems.Cast<object>().ToList();
            if (PageSize <= 0) return;

            int itemCount = (int)Math.Ceiling((double)list.Count);
            if (PageSize >= itemCount)
            {
                CurrentPage = 1;
                PageSize = itemCount - 1;
            }
            else
                TotalPages = itemCount / PageSize;

            var paged = list.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
            PagedItemsSource = paged;
            ItemsSource = PagedItemsSource;

            // Update Start, End and Total Row;
            StartRow = (CurrentPage - 1) * PageSize + 1;
            EndRow = (CurrentPage - 1) * PageSize + paged.Count();
            TotalRow = (int)Math.Ceiling((double)list.Count) + 1;
        }

        private void OnGoToPage(object page)
        {
            var p = Convert.ToInt32(page);
            if (p == -1)
                CurrentPage = TotalPages; // Last page.
            else
                CurrentPage = p;
        }

        private void OnGoToPrePage()
        {
            if (CurrentPage > 1)
                CurrentPage = CurrentPage - 1;
        }
        private void OnGoToPreMultiPage()
        {
            var newPage = CurrentPage - 5;

            if (newPage <= 0)
                CurrentPage = 1;
            else
                CurrentPage = newPage;
        }

        private void OnGoToNextPage()
        {
            if (CurrentPage < TotalPages)
                CurrentPage = CurrentPage + 1;
        }

        private void OnGoToNextMultiPage()
        {
            var newPage = CurrentPage + 5;

            if (newPage > TotalPages)
                CurrentPage = TotalPages;
            else
                CurrentPage = newPage;
        }

        private static void OnTotalPagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as WpDataGrid;
            grid?.UpdatePageList();
        }

        private void UpdatePageList()
        {
            // Gọi PropertyChanged cho PageList
            
            SetValue(PageListProperty, Enumerable.Range(1, TotalPages).ToList());
        }

        private void ExecuteLostFocus()
        {
            Keyboard.ClearFocus();
            //SetValue(PageSizeProperty, value);
        }

        private void GetStartRow()
        {
            StartRow = (CurrentPage - 1) * PageSize + 1;
        }

        private void GetEndRow()
        {
            EndRow = StartRow + PageSize - 1;
        }
        #endregion
    }

    public static class WpDataGridCommands
    {
        public static readonly RoutedCommand FirstPage = new RoutedCommand("FirstPage", typeof(WpDataGrid));
    }
}

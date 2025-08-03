using Microsoft.Win32;
using PROS.Module.Home.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OfficeOpenXml;
using System.Collections.ObjectModel;

namespace PROS.Module.Home.ViewModels
{
    public class HomeViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        private string _pOSFilePath;
        public string POSFilePath
        {
            get => _pOSFilePath;
            set => SetProperty(ref _pOSFilePath, value);
        }

        private string _shopeeFilePath;
        public string ShopeeFilePath
        {
            get => _shopeeFilePath;
            set => SetProperty(ref _shopeeFilePath, value);
        }

        private ObservableCollection<OrderModel> _orderList;
        public ObservableCollection<OrderModel> OrderList
        {
            get => _orderList;
            set => SetProperty(ref _orderList, value);
        }

        private Dictionary<string, OrderModel> _orderDict;
        private string _sheetName = "sheet";

        public HomeViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            this.LoadPOSFileCommand = new DelegateCommand(LoadPOSFile);
            this.LoadShopeeFileCommand = new DelegateCommand(LoadShopeeFile);
            this.AnalyzeCommand = new DelegateCommand(Analyze);
        }

        public ICommand LoadPOSFileCommand { get; }
        public ICommand LoadShopeeFileCommand { get; }
        public ICommand AnalyzeCommand { get; }

        private void LoadPOSFile()
        {
            POSFilePath = LoadData();
        }
        private void LoadShopeeFile()
        {
            ShopeeFilePath = LoadData();
        }

        private void Analyze()
        {
            LoadPOSSheet();
        }

        #region Navigation
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
        }
        #endregion
    


        private string LoadData()
        {
            string pathFile = "";
            var dialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|CSV files (*.csv)|*.csv",
                Title = "Chọn file Excel"
            };

            if (dialog.ShowDialog() == true)
            {
                pathFile = dialog.FileName;

            }
            return pathFile;
        }

        public void LoadPOSSheet()
        {
            var ordersById = new Dictionary<string, OrderModel>();
            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(POSFilePath));
            var worksheet = package.Workbook.Worksheets[_sheetName];

            if (worksheet == null)
            {
                MessageBox.Show($"Sheet '{_sheetName}' not found.");
                return;
            }

            var rowCount = worksheet.Dimension.Rows;

            // Giả định hàng 1 là header, dữ liệu bắt đầu từ hàng 2
            for (int row = 2; row <= rowCount; row++)
            {
                var order = new OrderModel
                {
                    OrderId = worksheet.Cells[row, 28].Text,
                    SoLuong = ParseInt(worksheet.Cells[row, 13].Text),
                    DonGia = ParseDecimal(worksheet.Cells[row, 14].Text),
                    PhiVCThuCuaKhach = ParseDecimal(worksheet.Cells[row, 11].Text),
                    PhuThu = ParseDecimal(worksheet.Cells[row, 24].Text),
                    GiamGiaTuDonHang = ParseDecimal(worksheet.Cells[row, 8].Text),
                    GiamGiaTungSanPham = ParseDecimal(worksheet.Cells[row, 12].Text),
                    PhiSan = ParseDecimal(worksheet.Cells[row, 15].Text),
                    SanTroGia = ParseDecimal(worksheet.Cells[row, 18].Text),
                    ChenhLechPhiVC_SanTMDT = ParseDecimal(worksheet.Cells[row, 17].Text),
                    PhiDichVu_SanTMDT = ParseDecimal(worksheet.Cells[row, 20].Text),
                    PhiThanhToan_SanTMDT = ParseDecimal(worksheet.Cells[row, 21].Text),
                    PhiHoaHongNenTang_SanTMDT = ParseDecimal(worksheet.Cells[row, 22].Text),
                    PhiCoDinh_GiaoDich_SanTMDT = ParseDecimal(worksheet.Cells[row, 23].Text),
                    SoLuongHoan = ParseInt(worksheet.Cells[row, 16].Text),
                    TrangThai = worksheet.Cells[row, 31].Text,
                    //CongThuc = worksheet.Cells[row, 17].Text,
                    //COD = ParseDecimal(worksheet.Cells[row, 18].Text),
                    //Diff = ParseDecimal(worksheet.Cells[row, 19].Text)
                };

                if (!string.IsNullOrWhiteSpace(order.OrderId))
                    ordersById[order.OrderId] = order;
            }
            OrderList = new ObservableCollection<OrderModel>(
                ordersById.Select(kvp => 
                {
                    //kvp.Value.OrderId = kvp.Key;
                    return kvp.Value;
                })
            );


        }


        private int ParseInt(string text)
        {
            return int.TryParse(text, out int result) ? result : 0;
        }

        private decimal ParseDecimal(string text)
        {
            return decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result) ? result : 0;
        }

    }
}

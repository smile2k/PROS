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

        public readonly record struct TrangThai(string Value)
        {
            public static readonly TrangThai BanBestHoanPOS = new("Ok bán BEST hoàn do POS đã nhảy số lượng sau hoàn");
            public static readonly TrangThai BiHoanSinhCongNoShopee = new("Ok bị hoàn và sinh ra công nợ với Shopee");
            public static readonly TrangThai BiHoanCongNoShopeeGiamGia = new("Ok bị hoàn và sinh ra công nợ với Shopee, đc trả thêm giảm giá đơn hàng");
            public static readonly TrangThai BiThuPhiHoanHangLech = new("Ok bị thu phí hoàn hàng lệch do đc trả lại phần giảm giá đơn hàng");
            public static readonly TrangThai GHTK = new("Ok GHTK");
            public static readonly TrangThai GHTKLechPhiSanPOS = new("Ok GHTK lệch do phí sàn đồng bộ về POS sai");
            public static readonly TrangThai GHTKKhongCongNoShopee = new("Ok GHTK và không sinh ra công nợ với Shopee");
            public static readonly TrangThai MatPhi = new("Ok mất phí");
            public static readonly TrangThai MatPhiVaSPHoanBest = new("Ok mất phí và các sản phẩm hoàn bán BEST");
            public static readonly TrangThai KhongCongNoShopee = new("Ok và không sinh ra công nợ với Shopee");
            public static readonly TrangThai DoiSoatKySau = new("Ok đối soát kỳ sau");
            public static readonly TrangThai POSThieuShopeeCo = new("POS thiếu Shopee có");
            public static readonly TrangThai LechDoTraThemGiaDonHang = new("Ok lệch được do trả thêm giảm giá đơn hàng");
            public static readonly TrangThai LechDoPhiSanDongBoVePOSSai = new("Ok lệch do phí sàn đồng bộ về POS sai");
            public static readonly TrangThai LechDoDuocTraThemGiamGiaDonHangVaBanHoanBest = new("Ok lệch do đc trả thêm giảm giá đơn hàng và bán hoàn BEST");
            public static readonly TrangThai LechDoPhiSanDongBoVePOSSaiVaBanHoanBest = new("Ok lệch do phí sàn đồng bộ về POS sai và bán hoàn BEST");
            public static readonly TrangThai CheckSanViDonHoanKhongTinhPhiSan = new("Check lại sàn vì đơn hoàn không được tính phí sàn");
            public static readonly TrangThai GHTKLechDoTraThemGiamGiaDonHang = new("Ok GHTK lệch do đc trả thêm giảm giá đơn hàng");
            public static readonly TrangThai GHTKLechPhiSanDongBoPOSSauVaBanHoanBest = new("Ok GHTK lệch do phí sàn đồng bộ về POS sai và bán hoàn BEST");
            public static readonly TrangThai GHTKDoPosNhaySoLuongSauHoan = new("Ok GHTK do POS đã nhảy số lượng sau hoàn");
            public static readonly TrangThai GHTKSinhRaCongNoShopee = new("OK GHTK và sinh ra công nợ với Shopee");
            public static readonly TrangThai ChuaRo = new("0");

            public override string ToString() => Value;
        }

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
            LoadShopeeSheet();
            Judgement();

            OrderList = new ObservableCollection<OrderModel>(
                _orderDict.Select(kvp => 
                {
                    //kvp.Value.OrderId = kvp.Key;
                    return kvp.Value;
                })
            );
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

            if (string.IsNullOrEmpty(ShopeeFilePath))
            {
                MessageBox.Show($"POS file can not found!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
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
                    //TriGiaHoan = ParseDecimal(worksheet.Cells[row, 32].Text),
                    //CongThuc = worksheet.Cells[row, 17].Text,
                    //COD = ParseDecimal(worksheet.Cells[row, 18].Text),
                    //Diff = ParseDecimal(worksheet.Cells[row, 19].Text)
                };

                order.CongThuc = CalculateEstimatedIncome(order);

                if (!string.IsNullOrWhiteSpace(order.OrderId))
                    ordersById[order.OrderId] = order;
            }

             _orderDict = ordersById; 

            //OrderList = new ObservableCollection<OrderModel>(
            //    ordersById.Select(kvp => 
            //    {
            //        //kvp.Value.OrderId = kvp.Key;
            //        return kvp.Value;
            //    })
            //);

        }

        private void LoadShopeeSheet()
        {
            var ordersById = new Dictionary<string, OrderModel>();
            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (string.IsNullOrEmpty(ShopeeFilePath))
            {
                MessageBox.Show($"Shopee file can not found!");
                return;
            }
            using var package = new ExcelPackage(new FileInfo(ShopeeFilePath));
            var worksheet1 = package.Workbook.Worksheets["Doanh thu - 1"];
            var x = package.Workbook.Worksheets.Count;
            var allSheets = package.Workbook.Worksheets;

            for (int i = 2; i < allSheets.Count; i++)
            {
                var worksheet = allSheets[i];
                int startRow = 4;
                int endRow = worksheet.Dimension.End.Row;

                for (int row = startRow; row <= endRow; row++)
                {
                    if (worksheet.Cells[row, 2].Text == "Sku") continue;
                    
                    var orderId = worksheet.Cells[row, 3].Text;
                    if (!_orderDict.ContainsKey(orderId))
                    {
                        _orderDict.Add(orderId, new OrderModel());
                    }

                    _orderDict[orderId].COD = ParseDecimal(worksheet.Cells[row, 18].Text);

                }
            }

        }

        private decimal CalculateEstimatedIncome(OrderModel order)
        {
            //int soLuong;
            //decimal DonGia;
            //decimal PhiVCThuCuaKhach;
            //decimal PhuThu;
            //decimal GiamGiaTuDonHang;
            //decimal GiamGiaTungSanPham;
            //decimal PhiSan;
            //decimal SanTroGia;
            //decimal ChenhLechPhiVC_SanTMDT;
            //decimal PhiDichVu_SanTMDT;
            //decimal PhiThanhToan_SanTMDT;
            //decimal PhiHoaHongNenTang_SanTMDT;
            //decimal PhiCoDinh_GiaoDich_SanTMDT;

            var estimatedIncome = order.SoLuong * order.DonGia
                                  + order.PhiVCThuCuaKhach
                                  + order.PhuThu
                                  - order.GiamGiaTuDonHang
                                  - order.GiamGiaTungSanPham
                                  - order.PhiSan
                                  + order.SanTroGia
                                  - order.SoLuongHoan * order.DonGia;
                                  //- order.SoLuongHoan * order.GiamGiaTungSanPham / order.SoLuong; //FIXME
            return estimatedIncome;
        }

        private void Judgement()
        {
            foreach (var order in _orderDict.Values)
            {
                order.Diff = order.CongThuc - order.COD;

                if (order.COD == 0 && order.CongThuc != 0)
                {
                    order.Result = TrangThai.DoiSoatKySau.Value;
                }
                else if (order.CongThuc == 0 && order.COD != 0)
                {
                    order.Result = TrangThai.POSThieuShopeeCo.Value;
                }
                else
                {
                    // Find cause.
                    if (order.TrangThai=="Đã thu tiền" || order.TrangThai=="Đã nhận" || order.TrangThai=="Hoàn một phần")
                    {
                        if (order.Diff > -4 && order.Diff < 4){
                            order.Result = TrangThai.MatPhi.Value;
                        }
                        else{
                            if (order.Diff == order.GiamGiaTuDonHang){
                                order.Result = TrangThai.LechDoTraThemGiaDonHang.Value;
                            }
                            else{
                                if (order.Diff == -Math.Abs(order.PhiSan - order.ChenhLechPhiVC_SanTMDT - order.PhiDichVu_SanTMDT - order.PhiThanhToan_SanTMDT - order.PhiHoaHongNenTang_SanTMDT - order.PhiCoDinh_GiaoDich_SanTMDT) ){
                                    order.Result = TrangThai.LechDoPhiSanDongBoVePOSSai.Value;
                                }
                                else{
                                    order.Result = TrangThai.ChuaRo.Value;
                                }
                                
                            }
                        }
                    }
                    //
                    if (order.TrangThai=="Đã đổi" || order.TrangThai=="Đổi một phần")
                    {
                        if (order.Diff > -4 && order.Diff < 4){
                            order.Result = TrangThai.MatPhiVaSPHoanBest.Value;
                        }
                        else {
                            if (order.Diff == order.GiamGiaTuDonHang){
                                order.Result = TrangThai.LechDoDuocTraThemGiamGiaDonHangVaBanHoanBest.Value;
                            }
                            else{
                                if (order.Diff == -Math.Abs(order.PhiSan - order.ChenhLechPhiVC_SanTMDT - order.PhiDichVu_SanTMDT - order.PhiThanhToan_SanTMDT - order.PhiHoaHongNenTang_SanTMDT - order.PhiCoDinh_GiaoDich_SanTMDT)){
                                    order.Result = TrangThai.LechDoDuocTraThemGiamGiaDonHangVaBanHoanBest.Value; //"ok lệch do phí sàn đồng bộ về POS sai và bán hoàn BEST";
                                }
                                else{
                                    if (order.SoLuong == 0) order.SoLuong = 1;
                                    if ((order.Diff == order.SoLuongHoan*order.DonGia - order.GiamGiaTungSanPham/order.SoLuong - order.GiamGiaTuDonHang) ||
                                        (order.Diff == order.SoLuongHoan*order.DonGia - order.GiamGiaTungSanPham/order.SoLuong) ){
                                        order.Result = TrangThai.BanBestHoanPOS.Value; //"OK bán BEST hoàn do POS đã nhảy số lượng sau hoàn";
                                    }
                                    else{
                                        order.Result = TrangThai.ChuaRo.Value;
                                    }
                                }
                            }
                        }
                    }
                    //
                    if (order.TrangThai=="Hoàn một phần"){
                        if (order.Diff > -4 && order.Diff < 4){
                            order.Result = TrangThai.KhongCongNoShopee.Value; //"OK và không sinh ra công nợ với Shopee";
                        }
                        else {
                            if (order.Diff == 999999 + order.GiamGiaTuDonHang){ //FIXME V2
                                order.Result = TrangThai.BiHoanCongNoShopeeGiamGia.Value; //"ok bị hoàn và sinh ra công nợ với Shopee, đc trả thêm giảm giá đơn hàng";
                            }
                            else{
                                if (order.Diff == 999999){
                                    order.Result = TrangThai.BiHoanSinhCongNoShopee.Value; //"ok bị hoàn và sinh ra công nợ với Shopee";
                                }
                                else{
                                    order.Result = TrangThai.ChuaRo.Value;
                                }
                            }
                        }
                    }
                    //
                    if (order.PhiSan == 0 && order.TrangThai == "Đã hoàn (thu phí)"){
                        if (order.Diff > -4 && order.Diff < 4){
                            order.Result = TrangThai.MatPhi.Value; //"OK mất phí";
                        }
                        else {
                            if (order.Diff == order.GiamGiaTuDonHang){
                                order.Result = TrangThai.BiThuPhiHoanHangLech.Value; //"ok bị thu phí hoàn hàng lệch do đc trả lại phần giảm giá đơn hàng";
                            }
                            else{
                                if (order.Diff == -Math.Abs(order.PhiSan - order.ChenhLechPhiVC_SanTMDT - order.PhiDichVu_SanTMDT - order.PhiThanhToan_SanTMDT - order.PhiHoaHongNenTang_SanTMDT - order.PhiCoDinh_GiaoDich_SanTMDT)){
                                    order.Result = TrangThai.LechDoPhiSanDongBoVePOSSai.Value; //"ok lệch do phí sàn đồng bộ về POS sai";
                                }
                                else{
                                    if (order.PhiSan != 0 && order.TrangThai == "Đã hoàn (thu phí)"){ // Fixme PhiSan
                                        order.Result = TrangThai.CheckSanViDonHoanKhongTinhPhiSan.Value; //"check lại sàn vì đơn hoàn không được tính phí sàn";
                                    }
                                    else{
                                        order.Result = TrangThai.ChuaRo.Value;
                                    }
                                }
                            }
                        }
                    }
                    //
                    else if (order.TrangThai == "Đang đổi"){
                        if (order.Diff > -4 && order.Diff < 4){
                            order.Result = TrangThai.GHTK.Value; //"OK GHTK";
                        }
                        else {
                            if (order.Diff == order.GiamGiaTuDonHang){
                                order.Result = TrangThai.GHTKLechDoTraThemGiamGiaDonHang.Value; //"ok GHTK lệch do đc trả thêm giảm giá đơn hàng";
                            }
                            else{
                                if (order.Diff == -Math.Abs(order.PhiSan - order.ChenhLechPhiVC_SanTMDT - order.PhiDichVu_SanTMDT - order.PhiThanhToan_SanTMDT - order.PhiHoaHongNenTang_SanTMDT - order.PhiCoDinh_GiaoDich_SanTMDT)){
                                    order.Result = TrangThai.GHTKLechPhiSanDongBoPOSSauVaBanHoanBest.Value; //"ok GHTK lệch do phí sàn đồng bộ về POS sai và bán hoàn BEST";
                                }
                                else{
                                    if (order.SoLuong == 0) order.SoLuong = 1;
                                    if ((order.Diff == order.SoLuongHoan*order.DonGia - order.GiamGiaTungSanPham/order.SoLuong - order.GiamGiaTuDonHang) || 
                                        (order.Diff == order.SoLuongHoan*order.DonGia - order.GiamGiaTungSanPham/order.SoLuong - order.GiamGiaTuDonHang)){
                                        order.Result = TrangThai.GHTKDoPosNhaySoLuongSauHoan.Value; //"OK GHTK do POS đã nhảy số lượng sau hoàn";
                                    }
                                    else{
                                        order.Result = TrangThai.ChuaRo.Value;
                                    }
                                }
                            }
                        }
                    }
                    //
                    else if (order.TrangThai == "Đang hoàn"){
                        if (order.Diff > -4 && order.Diff < 4){
                            order.Result = TrangThai.GHTKSinhRaCongNoShopee.Value; //"OK GHTK và sinh ra công nợ với Shopee";
                        }
                        else {
                            if (order.Diff == order.SoLuongHoan*order.DonGia - order.GiamGiaTungSanPham/order.SoLuong - order.GiamGiaTuDonHang){
                                order.Result = TrangThai.GHTKKhongCongNoShopee.Value; //"ok GHTK và không sinh ra công nợ với Shopee";
                            }
                            else{
                                if (order.Diff == -(order.SoLuongHoan*order.DonGia - order.GiamGiaTungSanPham/order.SoLuong*order.SoLuongHoan)){
                                    order.Result = TrangThai.GHTKKhongCongNoShopee.Value; //"ok GHTK và không sinh ra công nợ với Shopee";
                                }
                                else{
                                    if (order.Diff == -order.GiamGiaTuDonHang){
                                        order.Result = TrangThai.GHTKLechDoTraThemGiamGiaDonHang.Value; //"ok GHTK lệch do đc trả thêm giảm giá đơn hàng";
                                    }
                                    else{
                                        if (order.Diff == -Math.Abs(order.PhiSan - order.ChenhLechPhiVC_SanTMDT - order.PhiDichVu_SanTMDT - order.PhiThanhToan_SanTMDT - order.PhiHoaHongNenTang_SanTMDT - order.PhiCoDinh_GiaoDich_SanTMDT)){
                                            order.Result = TrangThai.GHTKLechPhiSanPOS.Value; //"ok GHTK lệch do phí sàn đồng bộ về POS sai";
                                        }
                                        else{
                                            order.Result = TrangThai.ChuaRo.Value;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //
                }
            }

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

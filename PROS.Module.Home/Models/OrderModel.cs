using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROS.Module.Home.Models
{
    public class OrderModel
    {
        public string OrderId { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal PhiVCThuCuaKhach { get; set; }
        public decimal PhuThu { get; set; }
        public decimal GiamGiaTuDonHang { get; set; }
        public decimal GiamGiaTungSanPham { get; set; }
        public decimal PhiSan { get; set; }
        public decimal SanTroGia { get; set; }
        public decimal ChenhLechPhiVC_SanTMDT { get; set; }
        public decimal PhiDichVu_SanTMDT { get; set; }
        public decimal PhiThanhToan_SanTMDT { get; set; }
        public decimal PhiHoaHongNenTang_SanTMDT { get; set; }
        public decimal PhiCoDinh_GiaoDich_SanTMDT { get; set; }
        public int SoLuongHoan { get; set; }
        public string TrangThai { get; set; }
        public string CongThuc { get; set; }
        public decimal COD { get; set; }
        public decimal Diff { get; set; }
    }
}

using System.Collections.Generic;
using DoAnCuoiKy_Nhom1.Models;

namespace DoAnCuoiKy_Nhom1.Models
{
    public class DanhMucLoaiSanPham
    {
        public IEnumerable<SanPham> SanPhamList { get; set; }
        public IEnumerable<DanhMucSanPham> DanhMucSanPhams { get; set; }
        public IEnumerable<LoaiSanPham> LoaiSanPhams { get; set; }
        //public IEnumerable<SP_NoiComDien> SP_NoiComDiens { get; set; }
        //public IEnumerable<SP_BepDienTu> SP_BepDienTus { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}

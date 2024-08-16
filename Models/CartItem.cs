using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnCuoiKy_Nhom1.Models
{
    public class CartItem
    {
        public string iMaSanPham { get; set; }
        public string sHinhAnh { get; set; }
        public string sTenSanPham { get; set; }
        public double sGiaBan { get; set; }
        public int iSoLuong { get; set; }
        public double ThanhTien
        {
            get { return iSoLuong * sGiaBan; }
        }

        DataClasses1DataContext data = new DataClasses1DataContext();

        public CartItem(string MaSanPham)
        {
            SanPham sp = data.SanPhams.Single(n => n.maSanPham == MaSanPham);
            if (sp != null)
            {
                iMaSanPham = MaSanPham;
                sHinhAnh = sp.hinhAnh;
                sTenSanPham = sp.tenSanPham;
                sGiaBan = double.Parse(sp.giaBan.ToString());
                iSoLuong = 1;
            }
        }

        public CartItem(string MaSanPham, int sl)
        {
            SanPham sp = data.SanPhams.Single(n => n.maSanPham == MaSanPham);
            if (sp != null)
            {
                iMaSanPham = MaSanPham;
                sHinhAnh = sp.hinhAnh;
                sTenSanPham = sp.tenSanPham;
                sGiaBan = double.Parse(sp.giaBan.ToString());
                iSoLuong = sl;
            }
        }
    }

    public class GioHang
    {
        public List<CartItem> lst;
        public GioHang()
        {
            lst = new List<CartItem>();
        }
        public int SoSP()
        {
            return lst.Count;
        }
        public int TongSLSP()
        {
            return lst.Sum(a => a.iSoLuong);
        }
        public double TongThanhTien()
        {
            return lst.Sum(t => t.ThanhTien);
        }
        public int Them(string MaSanPham)
        {
            CartItem sanpham = lst.Find(n => n.iMaSanPham == MaSanPham);
            if (sanpham == null)
            {
                CartItem sp = new CartItem(MaSanPham);
                if (sp == null)
                {
                    return -1;
                }
                lst.Add(sp);
            }
            else
            {
                sanpham.iSoLuong++;
            }
            return 1;
        }

        public int Them(string MaSanPham, int sl)
        {
            CartItem sanpham = lst.Find(n => n.iMaSanPham == MaSanPham);
            if (sanpham == null)
            {
                CartItem sp = new CartItem(MaSanPham, sl);
                if (sp == null)
                {
                    return -1;
                }
                lst.Add(sp);
            }
            else
            {
                sanpham.iSoLuong = sanpham.iSoLuong + sl;
            }
            return 1;
        }

        public int Xoa(string MaSanPham)
        {
            CartItem sanpham = lst.Find(n => n.iMaSanPham == MaSanPham);
            if (sanpham == null)
            {
                return -1;
            }
            else
            {
                lst.Remove(sanpham);
            }
            return 1;
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using DoAnCuoiKy_Nhom1.Models;
using PagedList;
namespace DoAnCuoiKy_Nhom1.Controllers
{
    public class HomeController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        public ActionResult Customer(string typeId, int? page)
        {
            var danhMucList = db.DanhMucSanPhams.ToList();
            var loaiSanPhamList = db.LoaiSanPhams.Take(11).ToList();
            Random rand = new Random();
            List<SanPham> listSanPham;

            if (string.IsNullOrEmpty(typeId))
            {
                listSanPham = db.SanPhams.ToList().OrderBy(x => rand.Next()).ToList();
            }
            else
            {
                listSanPham = db.SanPhams.Where(sp => sp.maLoai == typeId).ToList();
            }
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            IPagedList<SanPham> pagedProducts = listSanPham.ToPagedList(pageNumber, pageSize);

            var model = new DanhMucLoaiSanPham
            {
                DanhMucSanPhams = danhMucList,
                LoaiSanPhams = loaiSanPhamList
            };

            ViewBag.Products = pagedProducts;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = pagedProducts.PageCount;

            return View(model);
        }
        public ActionResult KhuyenMai()
        {
            Random random = new Random();
            List<KhuyenMai> listKhuyenMai = db.KhuyenMais.ToList().OrderBy(x => random.Next()).ToList();

            return PartialView(listKhuyenMai);
        }
        public ActionResult DangKyTaiKhoan()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKyTaiKhoan(DangKiTaiKhoan dangKy)
        {
            DangKiTaiKhoan checkEmail = db.DangKiTaiKhoans.FirstOrDefault(a => a.email == dangKy.email);
            if (checkEmail != null)
            {
                ViewBag.ThongBao = "Email đã tồn tại!";
                return View(dangKy);
            }

            if (string.IsNullOrEmpty(dangKy.hoVaTen) && string.IsNullOrEmpty(dangKy.soDienThoai) && string.IsNullOrEmpty(dangKy.email) && string.IsNullOrEmpty(dangKy.matKhau) && string.IsNullOrEmpty(dangKy.nhapLaiMatKhau) && string.IsNullOrEmpty(dangKy.diaChi))
            {
                ViewBag.ThongBao1 = "Vui lòng điền đầy đủ thông tin!";
                return View(dangKy);
            }
            if (dangKy.matKhau != dangKy.nhapLaiMatKhau)
            {
                ViewBag.ThongBao3 = "Mật khẩu không trùng khớp!";
                return View(dangKy);
            }

            db.DangKiTaiKhoans.InsertOnSubmit(dangKy);
            db.SubmitChanges();

            var taiKhoan = new TaiKhoan
            {
                email = dangKy.email,
                matKhau = dangKy.matKhau
            };

            db.TaiKhoans.InsertOnSubmit(taiKhoan);
            db.SubmitChanges();

            ViewBag.ThongBao2 = "Đăng ký thành công!";
            return View();
        }
        public ActionResult DangNhapTaiKhoan()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhapTaiKhoan(string email, string matkhau, TaiKhoan taiKhoan, string hoVaTen)
        {
            taiKhoan = db.TaiKhoans.FirstOrDefault(c => c.email == email && c.matKhau == matkhau);
            if (taiKhoan != null)
            {
                ViewBag.TB = "Đăng nhập thành công";
                var khachHang = db.DangKiTaiKhoans.FirstOrDefault(n => n.email == email);
                if (khachHang != null)
                {
                    Session["taikhoan"] = taiKhoan;
                    Session["tenKhachHang"] = khachHang.hoVaTen;
                }
                return RedirectToAction("Customer", "Home");
            }
            else
            {
                ViewBag.TB = "Email và mật khẩu bạn đã nhập không hợp lệ! Xin vui lòng thử lại.";
                return View();
            }
        }
        public ActionResult QuenMatKhau()
        {
            return View();
        }
        [HttpPost]
        public ActionResult QuenMatKhau(string email, string matKhau, string nhapLaiMatKhau)
        {
            var khachHang = db.DangKiTaiKhoans.FirstOrDefault(n => n.email == email);
            if (khachHang == null)
            {
                ViewBag.ThongBao = "Email không tồn tại!";
                return View();
            }

            if (string.IsNullOrEmpty(matKhau) || string.IsNullOrEmpty(nhapLaiMatKhau))
            {
                ViewBag.ThongBao = "Vui lòng nhập mật khẩu mới!";
                return View();
            }

            if (matKhau != nhapLaiMatKhau)
            {
                ViewBag.ThongBao = "Mật khẩu không trùng khớp!";
                return View();
            }

            khachHang.matKhau = matKhau;
            khachHang.nhapLaiMatKhau = nhapLaiMatKhau;
            db.SubmitChanges();

            var taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.email == email);
            if (taiKhoan != null)
            {
                taiKhoan.matKhau = matKhau;
                db.SubmitChanges();
            }

            ViewBag.ThongBao = "Đổi mật khẩu thành công!";
            return View();
        }
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Customer", "Home");
        }
        public ActionResult ChiTietSP(string id)
        {
          
            SanPham sanPham = db.SanPhams.SingleOrDefault(n => n.maSanPham == id);
            List<SanPham> listSP = db.SanPhams.Where(i => i.maSanPham == id || i.maLoai==sanPham.maLoai).ToList();
            ViewBag.listSP = listSP;

            List<DanhGia> listDanhGia = db.DanhGias.Where(dg => dg.maSanPham == id).ToList();
            ViewBag.DanhGiaSanPhams = listDanhGia;

            
            return View(sanPham);
        }
        [HttpPost]
        public ActionResult ThemDanhGiaSanPham(FormCollection f, string maSanPham)
        {
            var noiDung = f["txtNoiDung"];
            var hoTen = f["txtHoTen"];
            var sdt = f["txtSoDienThoai"];

            if (string.IsNullOrEmpty(noiDung) || string.IsNullOrEmpty(hoTen))
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin");
                return RedirectToAction("ChiTietSP", new { id = maSanPham });
            }

            DanhGia danhGia = new DanhGia
            {
                hoTenKhachHang = hoTen,
                soDienThoai = sdt,
                danhGiaSanPham = noiDung,
                maSanPham = maSanPham
            };
            
            db.DanhGias.InsertOnSubmit(danhGia);
            db.SubmitChanges();

            return RedirectToAction("ChiTietSP", new { id = maSanPham });
        }
        public ActionResult SearchName(string tensp, string loai)
        {
            List<SanPham> searchName = null;

            if (!string.IsNullOrEmpty(tensp))
            {
                searchName = db.SanPhams.Where(n => n.tenSanPham.Contains(tensp)).ToList();
            }
            else if (!string.IsNullOrEmpty(loai))
            {
                searchName = db.SanPhams.Where(sp => sp.maThuongHieu == loai).ToList();
            }
            else
            {
                searchName = db.SanPhams.ToList();
            }

            int sl = searchName.Count;
            ViewBag.SL = sl;
            List<SanPham> listSanPham = searchName;

            return View(searchName);
        }

        public ActionResult ThemSanPham(string msp)
        {
            GioHang gh = (GioHang)Session["gh"];
            if (gh == null)
                gh = new GioHang();
            int kq = gh.Them(msp);
            Session["gh"] = gh;
            return RedirectToAction("Customer");
        }

        public ActionResult CapNhatGioHang(string maSanPham, int soLuong)
        {
            GioHang gh = (GioHang)Session["gh"];
            if (gh == null)
                gh = new GioHang();

            CartItem sanpham = gh.lst.Find(n => n.iMaSanPham == maSanPham);
            if (sanpham != null)
            {
                sanpham.iSoLuong = soLuong;
            }

            Session["gh"] = gh;
            return RedirectToAction("XemGioHang");
        }

        public ActionResult ThemSanPhamVaoGio(string msp)
        {
            GioHang gh = (GioHang)Session["gh"];
            if (gh == null)
                gh = new GioHang();
            int kq = gh.Them(msp);
            Session["gh"] = gh;
            return RedirectToAction("XemGioHang", gh);
        }

        public ActionResult XemGioHang()
        {
            if (Session["taikhoan"] == null)
            {
                return RedirectToAction("DangNhapTaiKhoan", "Home");
            }
            GioHang gh = (GioHang)Session["gh"];
            TaiKhoan tk = (TaiKhoan)Session["taikhoan"];
            return View(gh);
        }

        [HttpPost]
        public ActionResult XLDatHang(FormCollection c)
        {
            if (Session["taikhoan"] == null)
            {
                return RedirectToAction("DangNhapTaiKhoan", "Home");
            }
            if (Session["gh"] == null)
            {
                ViewBag.tb = 0;
            }
            else
            {
                GioHang gh = Session["gh"] as GioHang;
                TaiKhoan tk = Session["taikhoan"] as TaiKhoan;
                HoaDon hd = new HoaDon();
                hd.maTaiKhoan = tk.maTaiKhoan;
                hd.ngayDatHang = DateTime.Now;
                hd.tongTien = (decimal)gh.TongThanhTien();
                hd.tinhTrangDonHang = "Đang xử lý";
                db.HoaDons.InsertOnSubmit(hd);
                db.SubmitChanges();


                foreach (CartItem item in gh.lst)
                {
                    ChiTietHoaDon ct = new ChiTietHoaDon();
                    ct.maHoaDon = hd.maHoaDon;
                    ct.maSanPham = item.iMaSanPham;
                    ct.soluongMua = item.iSoLuong;
                    ct.thanhTienHoaDon = (decimal)item.ThanhTien;

                    db.ChiTietHoaDons.InsertOnSubmit(ct);
                    db.SubmitChanges();
                }

                Session["gh"] = null;
                ViewBag.tb = 1;
            }
            return View();
        }
   
        public ActionResult XoaGioHang(string msp)
        {
            GioHang gh = (GioHang)Session["gh"];
            if (gh == null)
                gh = new GioHang();
            int kq = gh.Xoa(msp);
            Session["gh"] = gh;
            return RedirectToAction("XemGioHang");
        }

        public ActionResult XemLichSuMuaHang()
        {
            if (Session["taikhoan"] == null)
            {
                return RedirectToAction("DangNhapTaiKhoan", "Home");
            }
            TaiKhoan tk = (TaiKhoan)Session["taikhoan"];
            List<ChiTietHoaDon> cthd = db.ChiTietHoaDons.Where(c => c.HoaDon.maTaiKhoan == tk.maTaiKhoan).ToList();
            return View(cthd);
        }

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using QuanLyThuVien_N8.Models;
using PagedList;

namespace QuanLyThuVien_N8.Controllers
{
    public class ThuThuController : Controller
    {
        //
        // GET: /ThuThu/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllNguoiDung()
        {
            List<NguoiDung> listUser = new List<NguoiDung>();

            using (QuanLyThuVienEntities ql = new QuanLyThuVienEntities())
            {
                listUser = ql.NguoiDungs.ToList();
            }
            return View(listUser);
        }

        public JsonResult GetNguoiDungWithParameter(String prefix)
        {
            List<NguoiDung> listUser = new List<NguoiDung>();

            using (QuanLyThuVienEntities ql = new QuanLyThuVienEntities())
            {
                listUser = ql.NguoiDungs.Where(a => a.HoTen.Contains(prefix)).ToList();
            }
            return new JsonResult { Data = listUser, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Goi view Quan Ly Doc Gia: co chuc nang tim kiem doc gia
        public ActionResult QLDGia()
        {
            QuanLyThuVienEntities data = new QuanLyThuVienEntities();
            var kq = from ngd in data.NguoiDungs
                     select ngd;
            return View(kq);
        }

        //[ChildActionOnly] xu ly tim kiem doc gia
        public ActionResult KQTKiem(string TuKhoa, string KieuTim)
        {
            QuanLyThuVienEntities data = new QuanLyThuVienEntities();
            var kq = from ngd in data.NguoiDungs
                     where (ngd.HoTen.Contains(TuKhoa) || ngd.TenDangNhap.Contains(TuKhoa))
                     select ngd;

            if (KieuTim == "TenDG")
            {
                kq = from ngd in data.NguoiDungs
                     where (ngd.HoTen.Contains(TuKhoa))
                     select ngd;
            }

            if (KieuTim == "MaDG")
            {
                kq = from ngd in data.NguoiDungs
                     where (ngd.TenDangNhap.Contains(TuKhoa))
                     select ngd;
            }

            return View(kq);
        }

        // Goi view Them Doc Gia
        public ActionResult themDGia()
        {
            return View();
        }

        // Xu ly them doc gia roi goi view Quan Ly Doc Gia
        public ActionResult themdg(string hoten, string cmnd, DateTime ngsinh, string mssv, DateTime ngayhh, string loaingdung, string tendn, string mkhau)
        {
            QuanLyThuVienEntities data = new QuanLyThuVienEntities();
            NguoiDung dg = new NguoiDung();
            dg.HoTen = hoten;
            dg.SoCMND = cmnd;
            dg.NgaySinh = ngsinh;
            dg.MSSV = mssv;
            dg.NgayHetHan = ngayhh;
            dg.LoaiND = loaingdung;
            dg.TenDangNhap = tendn;
            dg.MatKhau = mkhau;
            data.NguoiDungs.Add(dg);
            data.SaveChanges();
            return RedirectToAction("QLDGia");
        }

        // Goi view Cap Nhat Doc Gia
        public ActionResult suadg(int id)
        {
            QuanLyThuVienEntities data = new QuanLyThuVienEntities();
            NguoiDung ngd = (from ngds in data.NguoiDungs
                             where ngds.MaNguoiDung == id
                             select ngds).First();
            return View("suaDGia", ngd);
        }

        //Xu ly cap nhat doc gia
        public ActionResult suaDGia(FormCollection f)
        {
            int id = int.Parse(f["MaNguoiDung"]);
            QuanLyThuVienEntities data = new QuanLyThuVienEntities();
            NguoiDung ngd = (from ngds in data.NguoiDungs
                             where ngds.MaNguoiDung == id
                             select ngds).First();
            ngd.HoTen = f["HoTen"];
            ngd.SoCMND = f["SoCMND"];
            ngd.NgaySinh = DateTime.Parse(f["NgaySinh"]);
            ngd.MSSV = f["MSSV"];
            ngd.NgayHetHan = DateTime.Parse(f["NgayHetHan"]);
            ngd.LoaiND = f["LoaiND"];
            ngd.TenDangNhap = f["TenDangNhap"];
            ngd.MatKhau = f["MatKhau"];
            data.SaveChanges();
            return RedirectToAction("QLDGia");
        }

        //Xu ly xoa doc gia
        public ActionResult xoaDGia(int id)
        {
            QuanLyThuVienEntities data = new QuanLyThuVienEntities();
            NguoiDung ngd = (from ngds in data.NguoiDungs
                             where ngds.MaNguoiDung == id
                             select ngds).First();
            data.NguoiDungs.Remove(ngd);
            data.SaveChanges();
            return RedirectToAction("QLDGia");
        }

        //Goi view Chi Tiet Doc Gia
        public ActionResult chitietDGia(int id)
        {
            QuanLyThuVienEntities data = new QuanLyThuVienEntities();
            NguoiDung ngd = (from ngds in data.NguoiDungs
                             where ngds.MaNguoiDung == id
                             select ngds).First();
            return View("chitietDGia", ngd);
        }


        public ActionResult capnhatAnh(string id)
        {
            QuanLyThuVienEntities data = new QuanLyThuVienEntities();
            NguoiDung ngd = (from ngds in data.NguoiDungs
                             where ngds.TenDangNhap == id
                             select ngds).First();
            return View("capnhatAnh", ngd);
        }

        [HttpPost]
        public ActionResult cnAnh(HttpPostedFileBase file, FormCollection f)
        {
            string mand = f["TenDangNhap"];
            if (ModelState.IsValid)
            {
                string temp = "";
                if (file != null)
                {
                    var tenAnh = mand + ".png";
                    var path = Path.Combine(Server.MapPath("~/Images"), tenAnh);
                    file.SaveAs(path);
                    temp = tenAnh;
                }
            }
            return RedirectToAction("QLDGia");
        }

        public ActionResult qlHinhAnh()
        {
            QuanLyThuVienEntities data = new QuanLyThuVienEntities();
            List<NguoiDung> lst = (from ngd in data.NguoiDungs select ngd).ToList();
            return View(lst);
        }

        public ActionResult QuanLySach(int? page)
        {
            QuanLyThuVienEntities data = new QuanLyThuVienEntities();
            var sach = from s in data.Saches select s;
            sach = sach.OrderBy(s => s.MaSach);
            int pageSize = 1;
            int pageNumber = (page ?? 1);
            return View(sach.ToPagedList(pageNumber, pageSize));
        }
    }
}

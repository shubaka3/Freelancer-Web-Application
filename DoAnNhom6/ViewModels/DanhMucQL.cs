namespace DoAnNhom6.ViewModels
{
    public class DanhMucQL
    {

        public double? DoanhThu { get; set; }
        public int SoNguoiDangKy { get; set; }
        public int SoNguoiDangLamViec { get; set; }
        //---- Biểu đồ cột --
        public List<TopJobInfo> TopJobsByMonth { get; set; }
        public class TopJobInfo
        {
            public int Thang { get; set; }
            public List<CongViecInfo> TopCongViecs { get; set; }
        }

        public class CongViecInfo
        {
            public string TenCongViec { get; set; }
            public int SoLanXuatHien { get; set; }
        }

        //---- Biểu đồ tròn---
        public int Goi100k { get; set; }
        public int Goi300k { get; set; }
        public int Goi500k { get; set; }
        //--- Tiến độ công việc --
        public int CongViecChuaDuyet { get; set; }
        public int ReportChuaRep { get; set; }

        
    }
}

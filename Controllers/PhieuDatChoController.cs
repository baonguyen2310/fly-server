using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class PhieuDatChoController : ControllerBase
{
    private readonly string _connectionString;
    private readonly string _MaCauHinh;

    public PhieuDatChoController(SystemConfigurationService systemConfigurationService)
    {
        _connectionString = systemConfigurationService.connectionString;
        _MaCauHinh = systemConfigurationService.MaCauHinh;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PhieuDatCho>> Get()
    {
        List<PhieuDatCho> phieuDatChoList = new List<PhieuDatCho>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM PhieuDatCho";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PhieuDatCho phieuDatCho = new PhieuDatCho
                        {
                            MaPhieuDatCho = Guid.Parse(reader["MaPhieuDatCho"].ToString()),
                            MaHanhKhach = Convert.ToInt32(reader["MaHanhKhach"]),
                            MaHangVe = Convert.ToInt32(reader["MaHangVe"]),
                            MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"]),
                            NgayDatCho = Convert.ToDateTime(reader["NgayDatCho"]),
                            MaTrangThai = Convert.ToInt32(reader["MaTrangThai"])
                        };
                        phieuDatChoList.Add(phieuDatCho);
                    }
                }
            }
        }

        return phieuDatChoList;
    }

    [HttpGet("{id}")]
    public ActionResult<PhieuDatCho> Get(Guid id)
    {
        PhieuDatCho phieuDatCho = new PhieuDatCho();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM PhieuDatCho WHERE MaPhieuDatCho = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        phieuDatCho.MaPhieuDatCho = Guid.Parse(reader["MaPhieuDatCho"].ToString());
                        phieuDatCho.MaHanhKhach = Convert.ToInt32(reader["MaHanhKhach"]);
                        phieuDatCho.MaHangVe = Convert.ToInt32(reader["MaHangVe"]);
                        phieuDatCho.MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"]);
                        phieuDatCho.NgayDatCho = Convert.ToDateTime(reader["NgayDatCho"]);
                        phieuDatCho.MaTrangThai = Convert.ToInt32(reader["MaTrangThai"]);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        return phieuDatCho;
    }

    [HttpPost]
    public IActionResult Post([FromBody] PhieuDatCho phieuDatCho)
    {
        // Lấy thông tin về số lượng ghế còn trống cho hạng vé chuyến bay từ mã hạng vé và mã lịch chuyến bay
        int maHangVe = phieuDatCho.MaHangVe;
        int maLichChuyenBay = phieuDatCho.MaLichChuyenBay;

        // Lấy thông tin số lượng ghế còn trống cho hạng vé chuyến bay từ cơ sở dữ liệu
        int soLuongGheConTrong = GetSoLuongGheConTrongByMaHangVeVaMaLichChuyenBay(maHangVe, maLichChuyenBay);

        if (soLuongGheConTrong <= 0)
        {
            return BadRequest("Không còn chỗ trống cho loại hạng vé này.");
        }

        int thoiGianDatVeChamNhat = GetThoiGianDatVeChamNhatFromConfig();

        // Lấy thời gian cất cánh từ chuyến bay tương ứng
        DateTime thoiGianCatCanh = GetThoiGianCatCanhByMaLichChuyenBay(phieuDatCho.MaLichChuyenBay);

        if (thoiGianCatCanh == DateTime.MinValue)
        {
            return BadRequest("Không tìm thấy chuyến bay tương ứng.");
        }

        // So sánh thời gian hiện tại với thời gian cất cánh để kiểm tra xem có thể đặt vé không
        DateTime thoiGianHienTai = DateTime.Now;
        if ((thoiGianCatCanh - thoiGianHienTai).TotalDays < thoiGianDatVeChamNhat)
        {
            return BadRequest("Thời gian đặt vé đã kết thúc.");
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO PhieuDatCho (MaHanhKhach, MaHangVe, MaLichChuyenBay, NgayDatCho, MaTrangThai) " +
                                 "VALUES (@maHanhKhach, @maHangVe, @maLichChuyenBay, @ngayDatCho, @maTrangThai)";
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@maHanhKhach", phieuDatCho.MaHanhKhach);
                command.Parameters.AddWithValue("@maHangVe", phieuDatCho.MaHangVe);
                command.Parameters.AddWithValue("@maLichChuyenBay", phieuDatCho.MaLichChuyenBay);
                command.Parameters.AddWithValue("@ngayDatCho", phieuDatCho.NgayDatCho);
                command.Parameters.AddWithValue("@maTrangThai", phieuDatCho.MaTrangThai);

                command.ExecuteNonQuery();
            }
        }

        return CreatedAtAction("Get", new { id = phieuDatCho.MaPhieuDatCho }, phieuDatCho);
    }

    [HttpPut("{id}")]
    public IActionResult Put(Guid id, [FromBody] PhieuDatCho phieuDatCho)
    {
        if (phieuDatCho.MaTrangThai == 2) //cancelled
        {
            int thoiGianHuyDatVeChamNhat = GetThoiGianHuyDatVeChamNhatFromConfig();

            // Lấy thời gian cất cánh từ chuyến bay tương ứng
            DateTime thoiGianCatCanh = GetThoiGianCatCanhByMaLichChuyenBay(phieuDatCho.MaLichChuyenBay);

            if (thoiGianCatCanh == DateTime.MinValue)
            {
                return BadRequest("Không tìm thấy chuyến bay tương ứng.");
            }

            // So sánh thời gian hiện tại với thời gian cất cánh để kiểm tra xem có thể huỷ đặt vé không
            DateTime thoiGianHienTai = DateTime.Now;
            if ((thoiGianCatCanh - thoiGianHienTai).TotalDays < thoiGianHuyDatVeChamNhat)
            {
                return BadRequest("Thời gian huỷ đặt vé đã kết thúc.");
            }
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string updateQuery = "UPDATE PhieuDatCho SET MaHanhKhach = @maHanhKhach, MaHangVe = @maHangVe, " +
                                 "MaLichChuyenBay = @maLichChuyenBay, NgayDatCho = @ngayDatCho, MaTrangThai = @maTrangThai " +
                                 "WHERE MaPhieuDatCho = @id";
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@maHanhKhach", phieuDatCho.MaHanhKhach);
                command.Parameters.AddWithValue("@maHangVe", phieuDatCho.MaHangVe);
                command.Parameters.AddWithValue("@maLichChuyenBay", phieuDatCho.MaLichChuyenBay);
                command.Parameters.AddWithValue("@ngayDatCho", phieuDatCho.NgayDatCho);
                command.Parameters.AddWithValue("@maTrangThai", phieuDatCho.MaTrangThai);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    return NotFound();
                }
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string deleteQuery = "DELETE FROM PhieuDatCho WHERE MaPhieuDatCho = @id";
            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    return NotFound();
                }
            }
        }

        return NoContent();
    }

    // Phương thức để lấy thời gian đặt vé chậm nhất từ cấu hình hệ thống
    private int GetThoiGianDatVeChamNhatFromConfig()
    {
        int thoiGianDatVeChamNhat = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT ThoiGianDatVeChamNhat FROM CauHinhHeThong WHERE MaCauHinh = @maCauHinh";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maCauHinh", _MaCauHinh);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    thoiGianDatVeChamNhat = Convert.ToInt32(result);
                }
            }
        }

        return thoiGianDatVeChamNhat;
    }

    // Phương thức để lấy thời gian huỷ đặt vé chậm nhất từ cấu hình hệ thống
    private int GetThoiGianHuyDatVeChamNhatFromConfig()
    {
        int thoiGianHuyDatVeChamNhat = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT ThoiGianHuyDatVeChamNhat FROM CauHinhHeThong WHERE MaCauHinh = @maCauHinh";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maCauHinh", _MaCauHinh);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    thoiGianHuyDatVeChamNhat = Convert.ToInt32(result);
                }
            }
        }

        return thoiGianHuyDatVeChamNhat;
    }

    // Phương thức để lấy thời gian cất cánh từ MaLichChuyenBay
    private DateTime GetThoiGianCatCanhByMaLichChuyenBay(int maLichChuyenBay)
    {
        DateTime thoiGianCatCanh = DateTime.MinValue;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT ThoiGianCatCanh FROM LichChuyenBay WHERE MaLichChuyenBay = @maLichChuyenBay";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maLichChuyenBay", maLichChuyenBay);
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    thoiGianCatCanh = Convert.ToDateTime(result);
                }
            }
        }

        return thoiGianCatCanh;
    }
    private int GetSoLuongGheConTrongByMaHangVeVaMaLichChuyenBay(int maHangVe, int maLichChuyenBay)
    {
        int soLuongGheConTrong = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT SoLuongGhe - ISNULL(SUM(SoLuongDat), 0) AS SoLuongGheConTrong " +
                           "FROM HangVeChuyenBay " +
                           "LEFT JOIN (SELECT MaHangVeChuyenBay, COUNT(*) AS SoLuongDat " +
                           "            FROM PhieuDatCho " +
                           "            WHERE MaHangVeChuyenBay = @maHangVeChuyenBay " +
                           "            GROUP BY MaHangVeChuyenBay) AS PhieuDatCho " +
                           "ON HangVeChuyenBay.MaHangVeChuyenBay = PhieuDatCho.MaHangVeChuyenBay " +
                           "WHERE HangVeChuyenBay.MaHangVe = @maHangVe " +
                           "AND HangVeChuyenBay.MaLichChuyenBay = @maLichChuyenBay";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maHangVe", maHangVe);
                command.Parameters.AddWithValue("@maLichChuyenBay", maLichChuyenBay);

                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    soLuongGheConTrong = Convert.ToInt32(result);
                }
            }
        }

        return soLuongGheConTrong;
    }

}
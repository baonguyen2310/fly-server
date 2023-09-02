using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class SanBayTrungGianController : ControllerBase
{
    private readonly string _connectionString;
    private readonly string _MaCauHinh;

    public SanBayTrungGianController(SystemConfigurationService systemConfigurationService)
    {
        _connectionString = systemConfigurationService.connectionString;
        _MaCauHinh = systemConfigurationService.MaCauHinh;
    }

    [HttpGet]
    public ActionResult<IEnumerable<SanBayTrungGian>> Get()
    {
        List<SanBayTrungGian> sanBayTrungGianList = new List<SanBayTrungGian>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM SanBayTrungGian";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SanBayTrungGian sanBayTrungGian = new SanBayTrungGian
                        {
                            MaSanBayTrungGian = Convert.ToInt32(reader["MaSanBayTrungGian"]),
                            MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"]),
                            MaSanBay = reader["MaSanBay"].ToString(),
                            ThoiGianBatDauDung = Convert.ToDateTime(reader["ThoiGianBatDauDung"]),
                            ThoiGianKetThucDung = Convert.ToDateTime(reader["ThoiGianKetThucDung"]),
                            GhiChu = reader["GhiChu"].ToString()
                        };
                        sanBayTrungGianList.Add(sanBayTrungGian);
                    }
                }
            }
        }

        return sanBayTrungGianList;
    }

    [HttpGet("{id}")]
    public ActionResult<SanBayTrungGian> Get(int id)
    {
        SanBayTrungGian sanBayTrungGian = new SanBayTrungGian();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM SanBayTrungGian WHERE MaSanBayTrungGian = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        sanBayTrungGian.MaSanBayTrungGian = Convert.ToInt32(reader["MaSanBayTrungGian"]);
                        sanBayTrungGian.MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"]);
                        sanBayTrungGian.MaSanBay = reader["MaSanBay"].ToString();
                        sanBayTrungGian.ThoiGianBatDauDung = Convert.ToDateTime(reader["ThoiGianBatDauDung"]);
                        sanBayTrungGian.ThoiGianKetThucDung = Convert.ToDateTime(reader["ThoiGianKetThucDung"]);
                        sanBayTrungGian.GhiChu = reader["GhiChu"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        return sanBayTrungGian;
    }

    [HttpGet("ByMaLichChuyenBay/{maLichChuyenBay}")]
    public ActionResult<IEnumerable<SanBayTrungGian>> GetByMaLichChuyenBay(int maLichChuyenBay)
    {
        List<SanBayTrungGian> sanBayTrungGianList = new List<SanBayTrungGian>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM SanBayTrungGian WHERE MaLichChuyenBay = @maLichChuyenBay";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maLichChuyenBay", maLichChuyenBay);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SanBayTrungGian sanBayTrungGian = new SanBayTrungGian
                        {
                            MaSanBayTrungGian = Convert.ToInt32(reader["MaSanBayTrungGian"]),
                            MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"]),
                            MaSanBay = reader["MaSanBay"].ToString(),
                            ThoiGianBatDauDung = Convert.ToDateTime(reader["ThoiGianBatDauDung"]),
                            ThoiGianKetThucDung = Convert.ToDateTime(reader["ThoiGianKetThucDung"]),
                            GhiChu = reader["GhiChu"].ToString()
                        };
                        sanBayTrungGianList.Add(sanBayTrungGian);
                    }
                }
            }
        }

        return sanBayTrungGianList;
    }


    [HttpPost]
    public IActionResult Post([FromBody] SanBayTrungGian sanBayTrungGian)
    {
        // Kiểm tra số sân bay trung gian tối đa dựa trên cấu hình hệ thống
        int soSanBayTrungGianToiDa = GetSoSanBayTrungGianToiDaFromConfig();

        // Kiểm tra thời gian dừng tối thiểu và thời gian dừng tối đa dựa trên cấu hình hệ thống
        int thoiGianDungToiThieu = GetThoiGianDungToiThieuFromConfig();
        int thoiGianDungToiDa = GetThoiGianDungToiDaFromConfig();

        // Tính thời gian dừng của sân bay trung gian
        int thoiGianDung = CalculateDungTime(sanBayTrungGian.ThoiGianBatDauDung, sanBayTrungGian.ThoiGianKetThucDung);

        if (soSanBayTrungGianToiDa > 0 && thoiGianDung < thoiGianDungToiThieu && thoiGianDung > thoiGianDungToiDa)
        {
            return BadRequest("Số sân bay trung gian hoặc thời gian dừng không đúng theo cấu hình hệ thống.");
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO SanBayTrungGian (MaLichChuyenBay, MaSanBay, ThoiGianBatDauDung, ThoiGianKetThucDung, GhiChu) " +
                                 "VALUES (@maLichChuyenBay, @maSanBay, @thoiGianBatDauDung, @thoiGianKetThucDung, @ghiChu)";
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@maLichChuyenBay", sanBayTrungGian.MaLichChuyenBay);
                command.Parameters.AddWithValue("@maSanBay", sanBayTrungGian.MaSanBay);
                command.Parameters.AddWithValue("@thoiGianBatDauDung", sanBayTrungGian.ThoiGianBatDauDung);
                command.Parameters.AddWithValue("@thoiGianKetThucDung", sanBayTrungGian.ThoiGianKetThucDung);
                command.Parameters.AddWithValue("@ghiChu", sanBayTrungGian.GhiChu);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return CreatedAtAction("Get", new { id = sanBayTrungGian.MaSanBayTrungGian }, sanBayTrungGian);
                }
                else
                {
                    return BadRequest("Thêm sân bay trung gian thất bại.");
                }
            }
        }
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] SanBayTrungGian sanBayTrungGian)
    {
        // Kiểm tra số sân bay trung gian tối đa dựa trên cấu hình hệ thống
        int soSanBayTrungGianToiDa = GetSoSanBayTrungGianToiDaFromConfig();

        // Kiểm tra thời gian dừng tối thiểu và thời gian dừng tối đa dựa trên cấu hình hệ thống
        int thoiGianDungToiThieu = GetThoiGianDungToiThieuFromConfig();
        int thoiGianDungToiDa = GetThoiGianDungToiDaFromConfig();

        // Tính thời gian dừng của sân bay trung gian
        int thoiGianDung = CalculateDungTime(sanBayTrungGian.ThoiGianBatDauDung, sanBayTrungGian.ThoiGianKetThucDung);

        if (soSanBayTrungGianToiDa > 0 && thoiGianDung < thoiGianDungToiThieu && thoiGianDung > thoiGianDungToiDa)
        {
            return BadRequest("Số sân bay trung gian hoặc thời gian dừng không đúng theo cấu hình hệ thống.");
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string updateQuery = "UPDATE SanBayTrungGian SET MaLichChuyenBay = @maLichChuyenBay, MaSanBay = @maSanBay, " +
                                 "ThoiGianBatDauDung = @thoiGianBatDauDung, ThoiGianKetThucDung = @thoiGianKetThucDung, GhiChu = @ghiChu " +
                                 "WHERE MaSanBayTrungGian = @id";
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@maLichChuyenBay", sanBayTrungGian.MaLichChuyenBay);
                command.Parameters.AddWithValue("@maSanBay", sanBayTrungGian.MaSanBay);
                command.Parameters.AddWithValue("@thoiGianBatDauDung", sanBayTrungGian.ThoiGianBatDauDung);
                command.Parameters.AddWithValue("@thoiGianKetThucDung", sanBayTrungGian.ThoiGianKetThucDung);
                command.Parameters.AddWithValue("@ghiChu", sanBayTrungGian.GhiChu);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string deleteQuery = "DELETE FROM SanBayTrungGian WHERE MaSanBayTrungGian = @id";
            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
        }
    }

    // Phương thức để lấy số sân bay trung gian tối đa từ cấu hình hệ thống
    private int GetSoSanBayTrungGianToiDaFromConfig()
    {
        int soSanBayTrungGianToiDa = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT SoSanBayTrungGianToiDa FROM CauHinhHeThong WHERE MaCauHinh = @maCauHinh";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maCauHinh", _MaCauHinh);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    soSanBayTrungGianToiDa = Convert.ToInt32(result);
                }
            }
        }

        return soSanBayTrungGianToiDa;
    }

    // Phương thức để lấy thời gian dừng tối thiểu từ cấu hình hệ thống
    private int GetThoiGianDungToiThieuFromConfig()
    {
        int thoiGianDungToiThieu = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT ThoiGianDungToiThieu FROM CauHinhHeThong WHERE MaCauHinh = @maCauHinh";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maCauHinh", _MaCauHinh);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    thoiGianDungToiThieu = Convert.ToInt32(result);
                }
            }
        }

        return thoiGianDungToiThieu;
    }

    // Phương thức để lấy thời gian dừng tối đa từ cấu hình hệ thống
    private int GetThoiGianDungToiDaFromConfig()
    {
        int thoiGianDungToiDa = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT ThoiGianDungToiDa FROM CauHinhHeThong WHERE MaCauHinh = @maCauHinh";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maCauHinh", _MaCauHinh);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    thoiGianDungToiDa = Convert.ToInt32(result);
                }
            }
        }

        return thoiGianDungToiDa;
    }

    // Phương thức tính thời gian dừng dựa trên thời gian bắt đầu và kết thúc dừng
    private int CalculateDungTime(DateTime thoiGianBatDauDung, DateTime thoiGianKetThucDung)
    {
        // Tính khoảng thời gian dừng (phút) từ thời gian bắt đầu và kết thúc dừng
        TimeSpan duration = thoiGianKetThucDung - thoiGianBatDauDung;
        return (int)duration.TotalMinutes;
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class LichChuyenBayController : ControllerBase
{
    private readonly string _connectionString;
    private readonly string _MaCauHinh;

    public LichChuyenBayController(SystemConfigurationService systemConfigurationService)
    {
        _connectionString = systemConfigurationService.connectionString;
        _MaCauHinh = systemConfigurationService.MaCauHinh;
    }

    [HttpGet]
    public ActionResult<IEnumerable<LichChuyenBay>> Get()
    {
        List<LichChuyenBay> lichChuyenBayList = new List<LichChuyenBay>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM LichChuyenBay";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LichChuyenBay lichChuyenBay = new LichChuyenBay
                        {
                            MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"]),
                            MaChuyenBay = reader["MaChuyenBay"].ToString(),
                            SanBayDi = reader["SanBayDi"].ToString(),
                            SanBayDen = reader["SanBayDen"].ToString(),
                            ThoiGianCatCanh = Convert.ToDateTime(reader["ThoiGianCatCanh"]),
                            ThoiGianHaCanh = Convert.ToDateTime(reader["ThoiGianHaCanh"])
                        };
                        lichChuyenBayList.Add(lichChuyenBay);
                    }
                }
            }
        }

        return lichChuyenBayList;
    }

    [HttpGet("{id}")]
    public ActionResult<LichChuyenBay> Get(int id)
    {
        LichChuyenBay lichChuyenBay = new LichChuyenBay();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM LichChuyenBay WHERE MaLichChuyenBay = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lichChuyenBay.MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"]);
                        lichChuyenBay.MaChuyenBay = reader["MaChuyenBay"].ToString();
                        lichChuyenBay.SanBayDi = reader["SanBayDi"].ToString();
                        lichChuyenBay.SanBayDen = reader["SanBayDen"].ToString();
                        lichChuyenBay.ThoiGianCatCanh = Convert.ToDateTime(reader["ThoiGianCatCanh"]);
                        lichChuyenBay.ThoiGianHaCanh = Convert.ToDateTime(reader["ThoiGianHaCanh"]);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        return lichChuyenBay;
    }

    [HttpPost]
    public IActionResult Post([FromBody] LichChuyenBay lichChuyenBay)
    {
        // Kiểm tra thời gian bay tối thiểu dựa trên cấu hình hệ thống
        int thoiGianBayToiThieu = GetThoiGianBayToiThieuFromConfig();

        if (CalculateFlightDuration(lichChuyenBay.ThoiGianCatCanh, lichChuyenBay.ThoiGianHaCanh) < thoiGianBayToiThieu)
        {
            return BadRequest("Thời gian bay không đủ tối thiểu.");
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO LichChuyenBay (MaChuyenBay, SanBayDi, SanBayDen, ThoiGianCatCanh, ThoiGianHaCanh) " +
                                 "VALUES (@maChuyenBay, @sanBayDi, @sanBayDen, @thoiGianCatCanh, @thoiGianHaCanh)";
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@maChuyenBay", lichChuyenBay.MaChuyenBay);
                command.Parameters.AddWithValue("@sanBayDi", lichChuyenBay.SanBayDi);
                command.Parameters.AddWithValue("@sanBayDen", lichChuyenBay.SanBayDen);
                command.Parameters.AddWithValue("@thoiGianCatCanh", lichChuyenBay.ThoiGianCatCanh);
                command.Parameters.AddWithValue("@thoiGianHaCanh", lichChuyenBay.ThoiGianHaCanh);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return CreatedAtAction("Get", new { id = lichChuyenBay.MaLichChuyenBay }, lichChuyenBay);
                }
                else
                {
                    return BadRequest("Thêm lịch chuyến bay thất bại.");
                }
            }
        }
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] LichChuyenBay lichChuyenBay)
    {
        // Kiểm tra thời gian bay tối thiểu dựa trên cấu hình hệ thống
        int thoiGianBayToiThieu = GetThoiGianBayToiThieuFromConfig();

        if (CalculateFlightDuration(lichChuyenBay.ThoiGianCatCanh, lichChuyenBay.ThoiGianHaCanh) < thoiGianBayToiThieu)
        {
            return BadRequest("Thời gian bay không đủ tối thiểu.");
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string updateQuery = "UPDATE LichChuyenBay SET MaChuyenBay = @maChuyenBay, SanBayDi = @sanBayDi, " +
                                 "SanBayDen = @sanBayDen, ThoiGianCatCanh = @thoiGianCatCanh, ThoiGianHaCanh = @thoiGianHaCanh " +
                                 "WHERE MaLichChuyenBay = @maLichChuyenBay";
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@maLichChuyenBay", id);
                command.Parameters.AddWithValue("@maChuyenBay", lichChuyenBay.MaChuyenBay);
                command.Parameters.AddWithValue("@sanBayDi", lichChuyenBay.SanBayDi);
                command.Parameters.AddWithValue("@sanBayDen", lichChuyenBay.SanBayDen);
                command.Parameters.AddWithValue("@thoiGianCatCanh", lichChuyenBay.ThoiGianCatCanh);
                command.Parameters.AddWithValue("@thoiGianHaCanh", lichChuyenBay.ThoiGianHaCanh);

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
            string deleteQuery = "DELETE FROM LichChuyenBay WHERE MaLichChuyenBay = @maLichChuyenBay";
            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@maLichChuyenBay", id);

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

    // Phương thức để lấy thời gian bay tối thiểu từ cấu hình hệ thống
    private int GetThoiGianBayToiThieuFromConfig()
    {
        int thoiGianBayToiThieu = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT ThoiGianBayToiThieu FROM CauHinhHeThong WHERE MaCauHinh = @maCauHinh";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maCauHinh", _MaCauHinh);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    thoiGianBayToiThieu = Convert.ToInt32(result);
                }
            }
        }

        return thoiGianBayToiThieu;
    }

    // Phương thức tính thời gian bay dựa trên thời gian cất cánh và thời gian hạ cánh
    private int CalculateFlightDuration(DateTime thoiGianCatCanh, DateTime thoiGianHaCanh)
    {
        // Tính khoảng thời gian bay (phút) từ thời gian cất cánh và thời gian hạ cánh
        TimeSpan duration = thoiGianHaCanh - thoiGianCatCanh;
        return (int)duration.TotalMinutes;
    }

}

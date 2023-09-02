using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class CauHinhHeThongController : ControllerBase
{
    private readonly string _connectionString;

    public CauHinhHeThongController(SystemConfigurationService systemConfigurationService)
    {
        _connectionString = systemConfigurationService.connectionString;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CauHinhHeThong>> Get()
    {
        List<CauHinhHeThong> cauHinhList = new List<CauHinhHeThong>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM CauHinhHeThong";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CauHinhHeThong cauHinh = new CauHinhHeThong
                        {
                            MaCauHinh = reader["MaCauHinh"].ToString(),
                            SoLuongSanBay = Convert.ToInt32(reader["SoLuongSanBay"]),
                            ThoiGianBayToiThieu = Convert.ToInt32(reader["ThoiGianBayToiThieu"]),
                            SoSanBayTrungGianToiDa = Convert.ToInt32(reader["SoSanBayTrungGianToiDa"]),
                            ThoiGianDungToiThieu = Convert.ToInt32(reader["ThoiGianDungToiThieu"]),
                            ThoiGianDungToiDa = Convert.ToInt32(reader["ThoiGianDungToiDa"]),
                            SoLuongHangVe = Convert.ToInt32(reader["SoLuongHangVe"]),
                            ThoiGianDatVeChamNhat = Convert.ToInt32(reader["ThoiGianDatVeChamNhat"]),
                            ThoiGianHuyDatVeChamNhat = Convert.ToInt32(reader["ThoiGianHuyDatVeChamNhat"])
                        };
                        cauHinhList.Add(cauHinh);
                    }
                }
            }
        }

        return cauHinhList;
    }

    [HttpGet("{id}")]
    public ActionResult<CauHinhHeThong> Get(string id)
    {
        CauHinhHeThong cauHinh = new CauHinhHeThong();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM CauHinhHeThong WHERE MaCauHinh = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cauHinh.MaCauHinh = reader["MaCauHinh"].ToString();
                        cauHinh.SoLuongSanBay = Convert.ToInt32(reader["SoLuongSanBay"]);
                        cauHinh.ThoiGianBayToiThieu = Convert.ToInt32(reader["ThoiGianBayToiThieu"]);
                        cauHinh.SoSanBayTrungGianToiDa = Convert.ToInt32(reader["SoSanBayTrungGianToiDa"]);
                        cauHinh.ThoiGianDungToiThieu = Convert.ToInt32(reader["ThoiGianDungToiThieu"]);
                        cauHinh.ThoiGianDungToiDa = Convert.ToInt32(reader["ThoiGianDungToiDa"]);
                        cauHinh.SoLuongHangVe = Convert.ToInt32(reader["SoLuongHangVe"]);
                        cauHinh.ThoiGianDatVeChamNhat = Convert.ToInt32(reader["ThoiGianDatVeChamNhat"]);
                        cauHinh.ThoiGianHuyDatVeChamNhat = Convert.ToInt32(reader["ThoiGianHuyDatVeChamNhat"]);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        return cauHinh;
    }

    [HttpPost]
    public IActionResult Post([FromBody] CauHinhHeThong cauHinh)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO CauHinhHeThong (MaCauHinh, SoLuongSanBay, ThoiGianBayToiThieu, SoSanBayTrungGianToiDa, ThoiGianDungToiThieu, ThoiGianDungToiDa, SoLuongHangVe, ThoiGianDatVeChamNhat, ThoiGianHuyDatVeChamNhat) VALUES (@maCauHinh, @soLuongSanBay, @thoiGianBayToiThieu, @soSanBayTrungGianToiDa, @thoiGianDungToiThieu, @thoiGianDungToiDa, @soLuongHangVe, @thoiGianDatVeChamNhat, @thoiGianHuyDatVeChamNhat)";
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@maCauHinh", cauHinh.MaCauHinh);
                command.Parameters.AddWithValue("@soLuongSanBay", cauHinh.SoLuongSanBay);
                command.Parameters.AddWithValue("@thoiGianBayToiThieu", cauHinh.ThoiGianBayToiThieu);
                command.Parameters.AddWithValue("@soSanBayTrungGianToiDa", cauHinh.SoSanBayTrungGianToiDa);
                command.Parameters.AddWithValue("@thoiGianDungToiThieu", cauHinh.ThoiGianDungToiThieu);
                command.Parameters.AddWithValue("@thoiGianDungToiDa", cauHinh.ThoiGianDungToiDa);
                command.Parameters.AddWithValue("@soLuongHangVe", cauHinh.SoLuongHangVe);
                command.Parameters.AddWithValue("@thoiGianDatVeChamNhat", cauHinh.ThoiGianDatVeChamNhat);
                command.Parameters.AddWithValue("@thoiGianHuyDatVeChamNhat", cauHinh.ThoiGianHuyDatVeChamNhat);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return CreatedAtAction("Get", new { id = cauHinh.MaCauHinh }, cauHinh);
                }
                else
                {
                    return BadRequest("Thêm cấu hình hệ thống thất bại.");
                }
            }
        }
    }

    [HttpPut("{id}")]
    public IActionResult Put(string id, [FromBody] CauHinhHeThong cauHinh)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string updateQuery = "UPDATE CauHinhHeThong SET SoLuongSanBay = @soLuongSanBay, ThoiGianBayToiThieu = @thoiGianBayToiThieu, SoSanBayTrungGianToiDa = @soSanBayTrungGianToiDa, ThoiGianDungToiThieu = @thoiGianDungToiThieu, ThoiGianDungToiDa = @thoiGianDungToiDa, SoLuongHangVe = @soLuongHangVe, ThoiGianDatVeChamNhat = @thoiGianDatVeChamNhat, ThoiGianHuyDatVeChamNhat = @thoiGianHuyDatVeChamNhat WHERE MaCauHinh = @maCauHinh";
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@maCauHinh", id);
                command.Parameters.AddWithValue("@soLuongSanBay", cauHinh.SoLuongSanBay);
                command.Parameters.AddWithValue("@thoiGianBayToiThieu", cauHinh.ThoiGianBayToiThieu);
                command.Parameters.AddWithValue("@soSanBayTrungGianToiDa", cauHinh.SoSanBayTrungGianToiDa);
                command.Parameters.AddWithValue("@thoiGianDungToiThieu", cauHinh.ThoiGianDungToiThieu);
                command.Parameters.AddWithValue("@thoiGianDungToiDa", cauHinh.ThoiGianDungToiDa);
                command.Parameters.AddWithValue("@soLuongHangVe", cauHinh.SoLuongHangVe);
                command.Parameters.AddWithValue("@thoiGianDatVeChamNhat", cauHinh.ThoiGianDatVeChamNhat);
                command.Parameters.AddWithValue("@thoiGianHuyDatVeChamNhat", cauHinh.ThoiGianHuyDatVeChamNhat);

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
    public IActionResult Delete(string id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string deleteQuery = "DELETE FROM CauHinhHeThong WHERE MaCauHinh = @maCauHinh";
            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@maCauHinh", id);

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
}

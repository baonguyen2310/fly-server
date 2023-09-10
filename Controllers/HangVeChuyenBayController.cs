using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class HangVeChuyenBayController : ControllerBase
{
    private readonly string _connectionString;

    public HangVeChuyenBayController(SystemConfigurationService systemConfigurationService)
    {
        _connectionString = systemConfigurationService.connectionString;
    }

    [HttpGet]
    public ActionResult<IEnumerable<HangVeChuyenBay>> Get()
    {
        List<HangVeChuyenBay> hangVeChuyenBayList = new List<HangVeChuyenBay>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM HangVeChuyenBay";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        HangVeChuyenBay hangVeChuyenBay = new HangVeChuyenBay
                        {
                            MaHangVeChuyenBay = Convert.ToInt32(reader["MaHangVeChuyenBay"]),
                            MaHangVe = Convert.ToInt32(reader["MaHangVe"]),
                            MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"]),
                            SoLuongGhe = Convert.ToInt32(reader["SoLuongGhe"]),
                            DonGiaVe = Convert.ToInt32(reader["DonGiaVe"])
                        };
                        hangVeChuyenBayList.Add(hangVeChuyenBay);
                    }
                }
            }
        }

        return hangVeChuyenBayList;
    }

    [HttpGet("{id}")]
    public ActionResult<HangVeChuyenBay> Get(int id)
    {
        HangVeChuyenBay hangVeChuyenBay = new HangVeChuyenBay();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM HangVeChuyenBay WHERE MaHangVeChuyenBay = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hangVeChuyenBay.MaHangVeChuyenBay = Convert.ToInt32(reader["MaHangVeChuyenBay"]);
                        hangVeChuyenBay.MaHangVe = Convert.ToInt32(reader["MaHangVe"]);
                        hangVeChuyenBay.MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"]);
                        hangVeChuyenBay.SoLuongGhe = Convert.ToInt32(reader["SoLuongGhe"]);
                        hangVeChuyenBay.DonGiaVe = Convert.ToInt32(reader["DonGiaVe"]);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        return hangVeChuyenBay;
    }

    [HttpPost]
    public IActionResult Post([FromBody] HangVeChuyenBay hangVeChuyenBay)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO HangVeChuyenBay (MaHangVe, MaLichChuyenBay, SoLuongGhe, DonGiaVe) " +
                                 "VALUES (@maHangVe, @maLichChuyenBay, @soLuongGhe, @donGiaVe)";
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@maHangVe", hangVeChuyenBay.MaHangVe);
                command.Parameters.AddWithValue("@maLichChuyenBay", hangVeChuyenBay.MaLichChuyenBay);
                command.Parameters.AddWithValue("@soLuongGhe", hangVeChuyenBay.SoLuongGhe);
                command.Parameters.AddWithValue("@donGiaVe", hangVeChuyenBay.DonGiaVe);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return CreatedAtAction("Get", new { id = hangVeChuyenBay.MaHangVeChuyenBay }, hangVeChuyenBay);
                }
                else
                {
                    return BadRequest("Thêm hạng vé chuyến bay thất bại.");
                }
            }
        }
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] HangVeChuyenBay hangVeChuyenBay)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string updateQuery = "UPDATE HangVeChuyenBay SET MaHangVe = @maHangVe, " +
                                 "MaLichChuyenBay = @maLichChuyenBay, SoLuongGhe = @soLuongGhe, DonGiaVe = @donGiaVe " +
                                 "WHERE MaHangVeChuyenBay = @maHangVeChuyenBay";
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@maHangVeChuyenBay", id);
                command.Parameters.AddWithValue("@maHangVe", hangVeChuyenBay.MaHangVe);
                command.Parameters.AddWithValue("@maLichChuyenBay", hangVeChuyenBay.MaLichChuyenBay);
                command.Parameters.AddWithValue("@soLuongGhe", hangVeChuyenBay.SoLuongGhe);
                command.Parameters.AddWithValue("@donGiaVe", hangVeChuyenBay.DonGiaVe);

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
            string deleteQuery = "DELETE FROM HangVeChuyenBay WHERE MaHangVeChuyenBay = @maHangVeChuyenBay";
            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@maHangVeChuyenBay", id);

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

    [HttpGet("ByLichChuyenBay/{maLichChuyenBay}")]
    public ActionResult<IEnumerable<HangVeChuyenBay>> GetByLichChuyenBay(int maLichChuyenBay)
    {
        List<HangVeChuyenBay> hangVeChuyenBayList = new List<HangVeChuyenBay>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM HangVeChuyenBay WHERE MaLichChuyenBay = @maLichChuyenBay";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maLichChuyenBay", maLichChuyenBay);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        HangVeChuyenBay hangVeChuyenBay = new HangVeChuyenBay
                        {
                            MaHangVeChuyenBay = Convert.ToInt32(reader["MaHangVeChuyenBay"]),
                            MaHangVe = Convert.ToInt32(reader["MaHangVe"]),
                            MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"]),
                            SoLuongGhe = Convert.ToInt32(reader["SoLuongGhe"]),
                            DonGiaVe = Convert.ToInt32(reader["DonGiaVe"])
                        };
                        hangVeChuyenBayList.Add(hangVeChuyenBay);
                    }
                }
            }
        }

        return hangVeChuyenBayList;
    }
}

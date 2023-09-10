using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class VeController : ControllerBase
{
    private readonly string _connectionString;

    public VeController(SystemConfigurationService systemConfigurationService)
    {
        _connectionString = systemConfigurationService.connectionString;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Ve>> Get()
    {
        List<Ve> veList = new List<Ve>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Ve";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Ve ve = new Ve
                        {
                            MaVe = Guid.Parse(reader["MaVe"].ToString()),
                            MaHanhKhach = Convert.ToInt32(reader["MaHanhKhach"]),
                            MaHangVe = Convert.ToInt32(reader["MaHangVe"]),
                            MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"])
                        };
                        veList.Add(ve);
                    }
                }
            }
        }

        return veList;
    }

    [HttpGet("{id}")]
    public ActionResult<Ve> Get(Guid id)
    {
        Ve ve = new Ve();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Ve WHERE MaVe = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ve.MaVe = Guid.Parse(reader["MaVe"].ToString());
                        ve.MaHanhKhach = Convert.ToInt32(reader["MaHanhKhach"]);
                        ve.MaHangVe = Convert.ToInt32(reader["MaHangVe"]);
                        ve.MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"]);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        return ve;
    }

    [HttpGet("ByMaLichChuyenBay/{maLichChuyenBay}")]
    public ActionResult<IEnumerable<Ve>> GetByMaLichChuyenBay(int maLichChuyenBay)
    {
        List<Ve> veList = new List<Ve>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Ve WHERE MaLichChuyenBay = @maLichChuyenBay";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maLichChuyenBay", maLichChuyenBay);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Ve ve = new Ve
                        {
                            MaVe = Guid.Parse(reader["MaVe"].ToString()),
                            MaHanhKhach = Convert.ToInt32(reader["MaHanhKhach"]),
                            MaHangVe = Convert.ToInt32(reader["MaHangVe"]),
                            MaLichChuyenBay = Convert.ToInt32(reader["MaLichChuyenBay"])
                        };
                        veList.Add(ve);
                    }
                }
            }
        }

        return veList;
    }


    [HttpPost]
    public IActionResult Post([FromBody] Ve ve)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO Ve (MaVe, MaHanhKhach, MaHangVe, MaLichChuyenBay) " +
                                 "VALUES (@maVe, @maHanhKhach, @maHangVe, @maLichChuyenBay)";
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@maVe", Guid.NewGuid());
                command.Parameters.AddWithValue("@maHanhKhach", ve.MaHanhKhach);
                command.Parameters.AddWithValue("@maHangVe", ve.MaHangVe);
                command.Parameters.AddWithValue("@maLichChuyenBay", ve.MaLichChuyenBay);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return CreatedAtAction("Get", new { id = ve.MaVe }, ve);
                }
                else
                {
                    return BadRequest("Thêm vé thất bại.");
                }
            }
        }
    }

    [HttpPut("{id}")]
    public IActionResult Put(Guid id, [FromBody] Ve ve)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string updateQuery = "UPDATE Ve SET MaHanhKhach = @maHanhKhach, MaHangVe = @maHangVe, MaLichChuyenBay = @maLichChuyenBay " +
                                 "WHERE MaVe = @id";
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@maHanhKhach", ve.MaHanhKhach);
                command.Parameters.AddWithValue("@maHangVe", ve.MaHangVe);
                command.Parameters.AddWithValue("@maLichChuyenBay", ve.MaLichChuyenBay);

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
    public IActionResult Delete(Guid id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string deleteQuery = "DELETE FROM Ve WHERE MaVe = @id";
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
}

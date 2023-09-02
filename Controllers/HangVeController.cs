using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class HangVeController : ControllerBase
{
    private readonly string _connectionString;

    public HangVeController(SystemConfigurationService systemConfigurationService)
    {
        _connectionString = systemConfigurationService.connectionString;
    }

    [HttpGet]
    public ActionResult<IEnumerable<HangVe>> Get()
    {
        List<HangVe> hangVeList = new List<HangVe>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM HangVe";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        HangVe hangVe = new HangVe
                        {
                            MaHangVe = Convert.ToInt32(reader["MaHangVe"]),
                            TenHangVe = reader["TenHangVe"].ToString()
                        };
                        hangVeList.Add(hangVe);
                    }
                }
            }
        }

        return hangVeList;
    }

    [HttpGet("{id}")]
    public ActionResult<HangVe> Get(int id)
    {
        HangVe hangVe = new HangVe();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM HangVe WHERE MaHangVe = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hangVe.MaHangVe = Convert.ToInt32(reader["MaHangVe"]);
                        hangVe.TenHangVe = reader["TenHangVe"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        return hangVe;
    }

    [HttpPost]
    public IActionResult Post([FromBody] HangVe hangVe)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO HangVe (TenHangVe) VALUES (@tenHangVe)";
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@tenHangVe", hangVe.TenHangVe);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return CreatedAtAction("Get", new { id = hangVe.MaHangVe }, hangVe);
                }
                else
                {
                    return BadRequest("Thêm hạng vé thất bại.");
                }
            }
        }
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] HangVe hangVe)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string updateQuery = "UPDATE HangVe SET TenHangVe = @tenHangVe WHERE MaHangVe = @id";
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@tenHangVe", hangVe.TenHangVe);

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
            string deleteQuery = "DELETE FROM HangVe WHERE MaHangVe = @id";
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

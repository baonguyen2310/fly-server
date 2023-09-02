using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class HanhKhachController : ControllerBase
{
    private readonly string _connectionString;

    public HanhKhachController(SystemConfigurationService systemConfigurationService)
    {
        _connectionString = systemConfigurationService.connectionString;
    }

    [HttpGet]
    public ActionResult<IEnumerable<HanhKhach>> Get()
    {
        List<HanhKhach> hanhKhachList = new List<HanhKhach>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM HanhKhach";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        HanhKhach hanhKhach = new HanhKhach
                        {
                            MaHanhKhach = Convert.ToInt32(reader["MaHanhKhach"]),
                            TenHanhKhach = reader["TenHanhKhach"].ToString(),
                            CMND = reader["CMND"].ToString(),
                            SDT = reader["SDT"].ToString()
                        };
                        hanhKhachList.Add(hanhKhach);
                    }
                }
            }
        }

        return hanhKhachList;
    }

    [HttpGet("{id}")]
    public ActionResult<HanhKhach> Get(int id)
    {
        HanhKhach hanhKhach = new HanhKhach();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM HanhKhach WHERE MaHanhKhach = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hanhKhach.MaHanhKhach = Convert.ToInt32(reader["MaHanhKhach"]);
                        hanhKhach.TenHanhKhach = reader["TenHanhKhach"].ToString();
                        hanhKhach.CMND = reader["CMND"].ToString();
                        hanhKhach.SDT = reader["SDT"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        return hanhKhach;
    }

    [HttpPost]
    public IActionResult Post([FromBody] HanhKhach hanhKhach)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO HanhKhach (TenHanhKhach, CMND, SDT) " +
                                 "VALUES (@tenHanhKhach, @cmnd, @sdt)";
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@tenHanhKhach", hanhKhach.TenHanhKhach);
                command.Parameters.AddWithValue("@cmnd", hanhKhach.CMND);
                command.Parameters.AddWithValue("@sdt", hanhKhach.SDT);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return CreatedAtAction("Get", new { id = hanhKhach.MaHanhKhach }, hanhKhach);
                }
                else
                {
                    return BadRequest("Thêm hành khách thất bại.");
                }
            }
        }
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] HanhKhach hanhKhach)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string updateQuery = "UPDATE HanhKhach SET TenHanhKhach = @tenHanhKhach, CMND = @cmnd, SDT = @sdt " +
                                 "WHERE MaHanhKhach = @id";
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@tenHanhKhach", hanhKhach.TenHanhKhach);
                command.Parameters.AddWithValue("@cmnd", hanhKhach.CMND);
                command.Parameters.AddWithValue("@sdt", hanhKhach.SDT);

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
            string deleteQuery = "DELETE FROM HanhKhach WHERE MaHanhKhach = @id";
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

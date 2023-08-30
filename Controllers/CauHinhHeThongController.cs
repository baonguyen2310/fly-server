using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;


[Route("api/[controller]")]
[ApiController]
public class CauHinhHeThongController : ControllerBase
{
    private readonly string _connectionString;

    public CauHinhHeThongController(string connectionString)
    {
        _connectionString = connectionString;
    }

    // GET: api/CauHinhHeThong
    [HttpGet]
    public ActionResult<IEnumerable<CauHinhHeThongModel>> Get()
    {
        List<CauHinhHeThongModel> configList = new List<CauHinhHeThongModel>();
        
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
                        CauHinhHeThongModel config = new CauHinhHeThongModel
                        {
                            MaCauHinh = (int)reader["MaCauHinh"],
                            TenCauHinh = reader["TenCauHinh"].ToString(),
                            GiaTri = reader["GiaTri"].ToString()
                        };
                        configList.Add(config);
                    }
                }
            }
        }

        return configList;
    }

    // GET: api/CauHinhHeThong/5
    [HttpGet("{id}")]
    public ActionResult<CauHinhHeThongModel> Get(int id)
    {
        CauHinhHeThongModel config = new CauHinhHeThongModel();
        
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
                        config.MaCauHinh = (int)reader["MaCauHinh"];
                        config.TenCauHinh = reader["TenCauHinh"].ToString();
                        config.GiaTri = reader["GiaTri"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        return config;
    }

    // POST: api/CauHinhHeThong
    [HttpPost]
    public IActionResult Post([FromBody] CauHinhHeThongModel config)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "INSERT INTO CauHinhHeThong (MaCauHinh, TenCauHinh, GiaTri) VALUES (@maCauHinh, @ten, @giaTri)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maCauHinh", config.MaCauHinh);
                command.Parameters.AddWithValue("@ten", config.TenCauHinh);
                command.Parameters.AddWithValue("@giaTri", config.GiaTri);
                command.ExecuteNonQuery();
            }
        }

        return CreatedAtAction("Get", new { id = config.MaCauHinh }, config);
    }

    // PUT: api/CauHinhHeThong/5
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] CauHinhHeThongModel config)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "UPDATE CauHinhHeThong SET TenCauHinh = @ten, GiaTri = @giaTri WHERE MaCauHinh = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@ten", config.TenCauHinh);
                command.Parameters.AddWithValue("@giaTri", config.GiaTri);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    return NotFound();
                }
            }
        }

        return NoContent();
    }

    // DELETE: api/CauHinhHeThong/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "DELETE FROM CauHinhHeThong WHERE MaCauHinh = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
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
}

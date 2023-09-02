using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class SanBayController : ControllerBase
{
    private readonly string _connectionString;
    private readonly string _MaCauHinh;

    public SanBayController(SystemConfigurationService systemConfigurationService)
    {
        _connectionString = systemConfigurationService.connectionString;
        _MaCauHinh = systemConfigurationService.MaCauHinh;
    }

    [HttpGet]
    public ActionResult<IEnumerable<SanBay>> Get()
    {
        List<SanBay> sanBayList = new List<SanBay>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM SanBay";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SanBay sanBay = new SanBay
                        {
                            MaSanBay = reader["MaSanBay"].ToString(),
                            TenSanBay = reader["TenSanBay"].ToString(),
                            DiaChi = reader["DiaChi"].ToString()
                        };
                        sanBayList.Add(sanBay);
                    }
                }
            }
        }

        return sanBayList;
    }

    [HttpGet("{id}")]
    public ActionResult<SanBay> Get(string id)
    {
        SanBay sanBay = new SanBay();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM SanBay WHERE MaSanBay = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        sanBay.MaSanBay = reader["MaSanBay"].ToString();
                        sanBay.TenSanBay = reader["TenSanBay"].ToString();
                        sanBay.DiaChi = reader["DiaChi"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        return sanBay;
    }

    [HttpPost]
    public IActionResult Post([FromBody] SanBay sanBay)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO SanBay (MaSanBay, TenSanBay, DiaChi) VALUES (@maSanBay, @tenSanBay, @diaChi)";
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@maSanBay", sanBay.MaSanBay);
                command.Parameters.AddWithValue("@tenSanBay", sanBay.TenSanBay);
                command.Parameters.AddWithValue("@diaChi", sanBay.DiaChi);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return CreatedAtAction("Get", new { id = sanBay.MaSanBay }, sanBay);
                }
                else
                {
                    return BadRequest("Thêm sân bay thất bại.");
                }
            }
        }
    }

    [HttpPut("{id}")]
    public IActionResult Put(string id, [FromBody] SanBay sanBay)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string updateQuery = "UPDATE SanBay SET TenSanBay = @tenSanBay, DiaChi = @diaChi WHERE MaSanBay = @maSanBay";
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@maSanBay", id);
                command.Parameters.AddWithValue("@tenSanBay", sanBay.TenSanBay);
                command.Parameters.AddWithValue("@diaChi", sanBay.DiaChi);

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
            string deleteQuery = "DELETE FROM SanBay WHERE MaSanBay = @maSanBay";
            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@maSanBay", id);

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

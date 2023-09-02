using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class TrangThaiDatChoController : ControllerBase
{
    private readonly string _connectionString;

    public TrangThaiDatChoController(SystemConfigurationService systemConfigurationService)
    {
        _connectionString = systemConfigurationService.connectionString;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TrangThaiDatCho>> Get()
    {
        List<TrangThaiDatCho> trangThaiDatChoList = new List<TrangThaiDatCho>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM TrangThaiDatCho";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TrangThaiDatCho trangThaiDatCho = new TrangThaiDatCho
                        {
                            MaTrangThai = Convert.ToInt32(reader["MaTrangThai"]),
                            TenTrangThai = reader["TenTrangThai"].ToString()
                        };
                        trangThaiDatChoList.Add(trangThaiDatCho);
                    }
                }
            }
        }

        return trangThaiDatChoList;
    }

    [HttpGet("{id}")]
    public ActionResult<TrangThaiDatCho> Get(int id)
    {
        TrangThaiDatCho trangThaiDatCho = new TrangThaiDatCho();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM TrangThaiDatCho WHERE MaTrangThai = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        trangThaiDatCho.MaTrangThai = Convert.ToInt32(reader["MaTrangThai"]);
                        trangThaiDatCho.TenTrangThai = reader["TenTrangThai"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        return trangThaiDatCho;
    }

    [HttpPost]
    public IActionResult Post([FromBody] TrangThaiDatCho trangThaiDatCho)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO TrangThaiDatCho (TenTrangThai) " +
                                 "VALUES (@tenTrangThai)";
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@tenTrangThai", trangThaiDatCho.TenTrangThai);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return CreatedAtAction("Get", new { id = trangThaiDatCho.MaTrangThai }, trangThaiDatCho);
                }
                else
                {
                    return BadRequest("Thêm trạng thái đặt chỗ thất bại.");
                }
            }
        }
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] TrangThaiDatCho trangThaiDatCho)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string updateQuery = "UPDATE TrangThaiDatCho SET TenTrangThai = @tenTrangThai " +
                                 "WHERE MaTrangThai = @id";
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@tenTrangThai", trangThaiDatCho.TenTrangThai);

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
            string deleteQuery = "DELETE FROM TrangThaiDatCho WHERE MaTrangThai = @id";
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

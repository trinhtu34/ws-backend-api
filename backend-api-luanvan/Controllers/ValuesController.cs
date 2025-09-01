using Microsoft.AspNetCore.Mvc;

namespace backend_api_luanvan.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private static readonly List<string> Values = new List<string> { "value1", "value2", "value3" };

    // GET api/values
    [HttpGet]
    public ActionResult<IEnumerable<string>> Get()
    {
        return Ok(Values);
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public ActionResult<string> Get(int id)
    {
        if (id < 0 || id >= Values.Count)
        {
            return NotFound("Value not found.");
        }
        return Ok(Values[id]);
    }

    // POST api/values
    [HttpPost]
    public ActionResult Post([FromBody] ValueDto valueDto)
    {
        if (string.IsNullOrWhiteSpace(valueDto?.Value))
        {
            return BadRequest("Invalid value.");
        }
        Values.Add(valueDto.Value);
        return CreatedAtAction(nameof(Get), new { id = Values.Count - 1 }, valueDto.Value);
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public ActionResult Put(int id, [FromBody] ValueDto valueDto)
    {
        if (id < 0 || id >= Values.Count || string.IsNullOrWhiteSpace(valueDto?.Value))
        {
            return BadRequest("Invalid ID or value.");
        }
        Values[id] = valueDto.Value;
        return NoContent();
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        if (id < 0 || id >= Values.Count)
        {
            return NotFound("Value not found.");
        }
        Values.RemoveAt(id);
        return NoContent();
    }
}

// DTO (Data Transfer Object) class để nhận JSON body
public class ValueDto
{
    public string Value { get; set; }
}
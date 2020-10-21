using BandAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Controllers
{
  [Route("api")]
  [ApiController]
  public class RootController : ControllerBase
  {

    [HttpGet(Name ="GetRoot")]
    public IActionResult GetRoot()
    {
      var links = new List<LinkDto>();

      links.Add(
        new LinkDto(Url.Link("GetRoot", new { }),
        "self",
        "GET"));

      links.Add(
        new LinkDto(Url.Link("GetBands", new { }),
        "bands",
        "GET"));

      links.Add(
        new LinkDto(Url.Link("CreateBand", new { }),
        "create_band",
        "POST"));

      return Ok(links);
    }
  }
}

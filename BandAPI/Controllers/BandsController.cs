using AutoMapper;
using BandAPI.Helpers;
using BandAPI.Models;
using BandAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BandAPI.Controllers
{
  [ApiController]
  [Route("api/bands")]
  public class BandsController : ControllerBase
  {
    private readonly IBandAlbumRepository _bandAlbumRepository;
    private readonly IMapper _mapper;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly IPropertyValidationService _propertyValidationService;

    public BandsController(IBandAlbumRepository bandAlbumRepository, IMapper mapper,
                            IPropertyMappingService propertyMappingService,
                            IPropertyValidationService propertyValidationService)
    {
      _bandAlbumRepository = bandAlbumRepository ??
          throw new ArgumentNullException(nameof(bandAlbumRepository));
      _mapper = mapper ??
          throw new ArgumentNullException(nameof(mapper));
      _propertyMappingService = propertyMappingService ??
          throw new ArgumentNullException(nameof(propertyMappingService));
      _propertyValidationService = propertyValidationService ??
          throw new ArgumentNullException(nameof(propertyValidationService));
    }

    [HttpGet(Name = "GetBands")]
    [HttpHead]
    public IActionResult GetBands(
                        [FromQuery] BandsResourceParameters bandsResourceParameters)
    {
      if (!_propertyMappingService.ValidMappingExists<BandDto, Entities.Band>
                      (bandsResourceParameters.OrderBy))
        return BadRequest();

      if (!_propertyValidationService.HasValidProperties<BandDto>
                          (bandsResourceParameters.Fields))
        return BadRequest();

      var bandsFromRepo = _bandAlbumRepository.GetBands(bandsResourceParameters);      

      var metaData = new
      {
        totalCount = bandsFromRepo.TotalCount,
        pageSize = bandsFromRepo.PageSize,
        currentPage = bandsFromRepo.CurrentPage,
        totalPages = bandsFromRepo.TotalPages
      };

      Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

      var links = CreateLinksForBands(bandsResourceParameters,bandsFromRepo.HasNext, bandsFromRepo.HasPrevious);
      var shapedBands = _mapper.Map<IEnumerable<BandDto>>(bandsFromRepo)
        .ShapeData(bandsResourceParameters.Fields);

      var shapedBandsWithLinks = shapedBands.Select(band =>
      {
        var bandAsDictionary = band as IDictionary<string, object>;
        var bandLinks = CreateLinksForBand((Guid)bandAsDictionary["Id"], null);
        bandAsDictionary.Add("links", bandLinks);
        return bandAsDictionary;
      });

      var linkedCollectionResource = new
      {
        value = shapedBandsWithLinks,
        links
      };

      return Ok(linkedCollectionResource);
    }

    [HttpGet("{bandId}", Name = "GetBand")]
    public IActionResult GetBand(Guid bandId, string fields)
    {
      if (!_propertyValidationService.HasValidProperties<BandDto>(fields))
        return BadRequest();

      var bandFromRepo = _bandAlbumRepository.GetBand(bandId);

      if (bandFromRepo == null)
        return NotFound();

      var links = CreateLinksForBand(bandId, fields);
      var linkedResourceToReturn =
        _mapper.Map<BandDto>(bandFromRepo).ShapeData(fields)
        as IDictionary<string, object>;

      linkedResourceToReturn.Add("links", links);

      return Ok(linkedResourceToReturn);
    }

    [HttpPost(Name = "CreateBand")]
    public ActionResult<BandDto> CreateBand([FromBody] BandForCreatingDto band)
    {
      var bandEntity = _mapper.Map<Entities.Band>(band);
      _bandAlbumRepository.AddBand(bandEntity);
      _bandAlbumRepository.Save();

      var bandToReturn = _mapper.Map<BandDto>(bandEntity);

      var links = CreateLinksForBand(bandToReturn.Id, null);
      var linkedResourceToReturn = bandToReturn.ShapeData(null)
        as IDictionary<string, object>;

      linkedResourceToReturn.Add("links", links);

      return CreatedAtRoute("GetBand", new { bandId = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
    }

    [HttpOptions]
    public IActionResult GetBandsOptions()
    {
      Response.Headers.Add("Allow", "GET,POST,DELETE,HEAD,OPTIONS");
      return Ok();
    }

    [HttpDelete("{bandId}", Name ="DeleteBand")]
    public ActionResult DeleteBand(Guid bandId)
    {
      var bandFromRepo = _bandAlbumRepository.GetBand(bandId);
      if (bandFromRepo == null)
        return NotFound();

      _bandAlbumRepository.DeleteBand(bandFromRepo);
      _bandAlbumRepository.Save();

      return NoContent();
    }

    private string CreateBandsUri(BandsResourceParameters bandsResourceParameters,
                                UriType uriType)
    {
      switch (uriType)
      {
        case UriType.PreviousPage:
          return Url.Link("GetBands", new
          {
            fields = bandsResourceParameters.Fields,
            orderBy = bandsResourceParameters.OrderBy,
            pageNumber = bandsResourceParameters.PageNumber - 1,
            pageSize = bandsResourceParameters.PageSize,
            mainGenre = bandsResourceParameters.MainGenre,
            searchQuery = bandsResourceParameters.SearchQuery
          });
        case UriType.NextPage:
          return Url.Link("GetBands", new
          {
            fields = bandsResourceParameters.Fields,
            orderBy = bandsResourceParameters.OrderBy,
            pageNumber = bandsResourceParameters.PageNumber + 1,
            pageSize = bandsResourceParameters.PageSize,
            mainGenre = bandsResourceParameters.MainGenre,
            searchQuery = bandsResourceParameters.SearchQuery
          });
        case UriType.Current:
        default:
          return Url.Link("GetBands", new
          {
            fields = bandsResourceParameters.Fields,
            orderBy = bandsResourceParameters.OrderBy,
            pageNumber = bandsResourceParameters.PageNumber,
            pageSize = bandsResourceParameters.PageSize,
            mainGenre = bandsResourceParameters.MainGenre,
            searchQuery = bandsResourceParameters.SearchQuery
          });
      }
    }

    private IEnumerable<LinkDto> CreateLinksForBand(Guid bandId, string fields)
    {
      var links = new List<LinkDto>();

      if (string.IsNullOrWhiteSpace(fields))
      {
        links.Add(
          new LinkDto(Url.Link("GetBand", new { bandId}),
          "self",
          "GET"
          ));
      }
      else
      {
        links.Add(
          new LinkDto(Url.Link("GetBand", new { bandId, fields }),
          "self",
          "GET"
          ));
      }

      links.Add(
        new LinkDto(Url.Link("DeleteBand", new { bandId }),
        "delete_band",
        "DELETE"));

      links.Add(
        new LinkDto(Url.Link("CreateAlbumForBand", new { bandId }),
        "create_album_for_band",
        "POST"));

      links.Add(
        new LinkDto(Url.Link("GetAlbumsForBand", new { bandId}),
        "albums",
        "GET"));

      return links;
    }

    private IEnumerable<LinkDto> CreateLinksForBands(BandsResourceParameters bandsResourceParameters, bool hasNext, bool hasPrevious)
    {
      var links = new List<LinkDto>();

      links.Add(
        new LinkDto(CreateBandsUri(bandsResourceParameters, UriType.Current),
        "self",
        "GET"));

      if (hasNext)
      {
        links.Add(
        new LinkDto(CreateBandsUri(bandsResourceParameters, UriType.NextPage),
        "nextPage",
        "GET"));
      }

      if (hasPrevious)
      {
        links.Add(
        new LinkDto(CreateBandsUri(bandsResourceParameters, UriType.PreviousPage),
        "previousPage",
        "GET"));        
      }

      return links;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BandAPI.DbContexts;
using BandAPI.Entities;
using BandAPI.Helpers;

namespace BandAPI.Services
{
    public class BandAlbumRepository : IBandAlbumRepository
    {
        private readonly BandAlbumContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public BandAlbumRepository(BandAlbumContext context, 
            IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMappingService = propertyMappingService;
        }

        public void AddAlbum(Guid bandId, Album album)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            if (album == null)
                throw new ArgumentNullException(nameof(album));

            album.BandId = bandId;
            _context.Albums.Add(album);
        }

        public void AddBand(Band band)
        {
            if (band == null)
                throw new ArgumentNullException(nameof(band));

            _context.Bands.Add(band);
        }

        public bool AlbumExists(Guid albumId)
        {
            if (albumId == Guid.Empty)
                throw new ArgumentNullException(nameof(albumId));

            return _context.Albums.Any(a => a.Id == albumId);
        }

        public bool BandExists(Guid bandId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            return _context.Bands.Any(b => b.Id == bandId);
        }

        public void DeleteAlbum(Album album)
        {
            if (album == null)
                throw new ArgumentNullException(nameof(album));

            _context.Albums.Remove(album);
        }

        public void DeleteBand(Band band)
        {
            if (band == null)
                throw new ArgumentNullException(nameof(band));

            _context.Bands.Remove(band);
        }

        public Album GetAlbum(Guid bandId, Guid albumId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            if (albumId == Guid.Empty)
                throw new ArgumentNullException(nameof(albumId));

            return _context.Albums.Where(a => a.BandId == bandId && a.Id == albumId).FirstOrDefault();
        }

        public IEnumerable<Album> GetAlbums(Guid bandId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            return _context.Albums.Where(a => a.BandId == bandId)
                                  .OrderBy(a => a.Title).ToList();
        }

        public Band GetBand(Guid bandId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            return _context.Bands.FirstOrDefault(b => b.Id == bandId);
        }

        public IEnumerable<Band> GetBands()
        {
            return _context.Bands.ToList();
        }

        public IEnumerable<Band> GetBands(IEnumerable<Guid> bandIds)
        {
            if (bandIds == null)
                throw new ArgumentNullException(nameof(bandIds));

            return _context.Bands.Where(b => bandIds.Contains(b.Id))
                            .OrderBy(b => b.Name).ToList();
        }

        public PagedList<Band> GetBands(BandsResourceParameters bandsResourceParameters)
        {
            if (bandsResourceParameters == null)
                throw new ArgumentNullException(nameof(bandsResourceParameters));

            var collection = _context.Bands as IQueryable<Band>;

            if (!string.IsNullOrWhiteSpace(bandsResourceParameters.MainGenre))
            {
                var mainGenre = bandsResourceParameters.MainGenre.Trim();
                collection = collection.Where(b => b.MainGenre == mainGenre);
            }

            if (!string.IsNullOrWhiteSpace(bandsResourceParameters.SearchQuery))
            {
                var searchQuery = bandsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(b => b.Name.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(bandsResourceParameters.OrderBy))
            {
                var bandPropertyMappingDictionary =
                _propertyMappingService.GetPropertyMapping<Models.BandDto, Entities.Band>();

                collection = collection.ApplySort(bandsResourceParameters.OrderBy, 
                                     bandPropertyMappingDictionary);
            }            

            return PagedList<Band>.Create(collection, bandsResourceParameters.PageNumber, bandsResourceParameters.PageSize);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateAlbum(Album album)
        {
            //not implemented
            //Repository Pattern - is an abstraction that reduces complexity and aims to make the code safe for the repository implementation, persistance ignorant.
        }

        public void UpdateBand(Band band)
        {
            //not implemented
        }
    }
}

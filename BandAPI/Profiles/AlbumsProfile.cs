using AutoMapper;
using BandAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Profiles
{
    public class AlbumsProfile : Profile
    {
        public AlbumsProfile()
        {
            CreateMap<Entities.Album, Models.AlbumsDto>().ReverseMap();
            CreateMap<AlbumForCreatingDto, Entities.Album>();
            CreateMap<Models.AlbumForUpdatingDto, Entities.Album>().ReverseMap();
        }
    }
}

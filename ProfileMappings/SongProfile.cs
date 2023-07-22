using AutoMapper;
using LyricsFinder.NET.Models;
using LyricsFinder.NET.Models.DTOs;

namespace LyricsFinder.NET.ProfileMappings
{
    public class SongProfile : Profile
    {
        public SongProfile()
        {
            CreateMap<Song, SongReadDTO>();

            CreateMap<SongCreateDTO, Song>();

            CreateMap<SongEditDTO, Song>();

            CreateMap<SongUpdateDTO, Song>();

            CreateMap<Song, SongUpdateDTO>();
        }
    }
}

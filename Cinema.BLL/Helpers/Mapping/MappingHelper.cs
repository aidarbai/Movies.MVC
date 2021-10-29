using Cinema.BLL.DTOs.Movie;
using Cinema.COMMON.Responses;
using Cinema.DAL.Models;
using System;

namespace Cinema.BLL.Helpers.Mapping
{
    public static class MappingHelper
    {
        public static BaseResponse AutoMapperInit()
        {
            BaseResponse response = new();

            try
            {
                AutoMapper.Mapper.Initialize(cfg =>
                {
                    cfg.ValidateInlineMaps = false;
                    cfg.CreateMissingTypeMaps = true;
                    cfg.CreateMap<Comment, CommentDTO>()
                        .ForMember(t => t.MovieId, t => t.MapFrom(p => p.Movie.Id));
                    cfg.CreateMap<Vote, VoteDTO>()
                        .ForMember(t => t.MovieId, t => t.MapFrom(p => p.Movie.Id));
                    cfg.CreateMap<MovieDTO, Movie>()
                       .ReverseMap();
                });
                response.Message = "Mapping has been done successfully";
            }
            catch(Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
            }

            return response;
        }
    }
}

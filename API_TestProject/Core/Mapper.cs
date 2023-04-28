using API_TestProject.DataBase.Model;
using API_TestProject.WebApi.Model.Response;
using AutoMapper;

namespace API_TestProject.Core
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Node, NodeDTO>();
            CreateMap<Tree, TreeDTO>().ForMember(dest => dest.children, opt => opt.MapFrom(src => src.Nodes));
        }
    }
}

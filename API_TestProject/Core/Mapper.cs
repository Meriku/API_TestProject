using API_TestProject.Core.Model;
using API_TestProject.DataBase.Model;
using API_TestProject.WebApi.Model.Response;
using AutoMapper;

namespace API_TestProject.Core
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Node, NodeDTO>().
                ForMember(dest => dest.id, opt => opt.MapFrom(src => src.NodeId)).
                ForMember(dest => dest.children, opt => opt.MapFrom(src => src.ChildrenNodes));
            CreateMap<Tree, TreeDTO>().
                ForMember(dest => dest.id, opt => opt.MapFrom(src => src.TreeId)).
                ForMember(dest => dest.children, opt => opt.MapFrom(src => src.Nodes));
            CreateMap<TreeExtended, TreeDTO>().
                ForMember(dest => dest.id, opt => opt.MapFrom(src => src.TreeId)).
                ForMember(dest => dest.children, opt => opt.MapFrom(src => src.RootNodes));
        }
    }
}

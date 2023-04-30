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
            CreateMap<ExceptionLog, ExceptionLogDTO>().
                ForMember(dest => dest.type, opt => opt.MapFrom(src => src.Type)).
                ForMember(dest => dest.id, opt => opt.MapFrom(src => src.EventId)).
                ForMember(dest => dest.data, opt => opt.MapFrom(src => 
                    new ExceptionLogDataDTO() { message = src.Type.Equals(nameof(SecureException)) ? src.Message : $"Internal server error ID = {src.EventId}" }));
        }
    }
}

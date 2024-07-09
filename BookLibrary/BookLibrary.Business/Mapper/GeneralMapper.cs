using BookLibrary.Data.Entities;
using BookLibrary.Models;
using Mapster;

namespace BookLibrary.Business.Mapper
{
    public class GeneralMapper : IRegister
    {
                
        public void Register(TypeAdapterConfig config)   
        {

            TypeAdapterConfig<User, UserDto>.NewConfig()
            .Map(dest => dest.Role, src => src.Role != null ? src.Role.Name : null);

        }
    }
}

using AutoMapper;
using SaitynaiNamoValdymoSIstema.DataDB;
using SaitynaiNamoValdymoSIstema.DTOs;

namespace SaitynaiNamoValdymoSIstema.Profiles
{
    public class Mapper:Profile
    {
        public Mapper()
        {
            CreateMap<Flat, FlatDTO>();
            CreateMap<FlatDTO, Flat>();
            CreateMap<FloorDTO, Floor>();
            CreateMap<Floor,FloorDTO>();
            CreateMap<MessageDTO, Messagee>();
            CreateMap<Messagee, MessageDTO>();
            CreateMap<PersonDTO,Person>().ForMember(x => x.Password, opt => opt.Ignore());
            CreateMap<Person, PersonDTO>().ForMember(x => x.Password, opt => opt.Ignore());
        }
    }
}

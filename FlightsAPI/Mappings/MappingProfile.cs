using AutoMapper;
using FlightsAPI.Models;

namespace FlightsAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TestFlight1, Flight>()
                .ForMember(d => d.Source, o => o.MapFrom("TestAviaProvider1"))
                .ForMember(d => d.FlightId, o => o.MapFrom(s => s.GuId))
                .ForMember(d => d.FlightCompanyName, o => o.MapFrom(s => s.AviaCompanyName))
                .ForMember(d => d.DepartureAirport, o => o.MapFrom(s => s.DepartureAirportName))
                .ForMember(d => d.DepartureDateTime, o => o.MapFrom(s => s.DepartureTime))
                .ForMember(d => d.ArrivalAirport, o => o.MapFrom(s => s.DestinationAirportName))
                .ForMember(d => d.ArrivalDateTime, o => o.MapFrom(s => s.DestinationTime))
                .ForMember(d => d.Transfers, o => o.MapFrom(s => s.NumberOfTransfers))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.TotalAmount));

            CreateMap<TestFlight2, Flight>()
                .ForMember(d => d.Source, o => o.MapFrom("TestAviaProvider2"))
                .ForMember(d => d.FlightId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.FlightCompanyName, o => o.MapFrom(s => s.CompanyName))
                .ForMember(d => d.DepartureAirport, o => o.MapFrom(s => s.DepAirport))
                .ForMember(d => d.DepartureDateTime, o => o.MapFrom(s => s.DepTime))
                .ForMember(d => d.ArrivalAirport, o => o.MapFrom(s => s.DestAirport))
                .ForMember(d => d.ArrivalDateTime, o => o.MapFrom(s => s.DestTime))
                .ForMember(d => d.Transfers, o => o.MapFrom(s => s.TransfersQuantity))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.FlightCost));
        }
    }
}

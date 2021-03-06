using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vega.Domain;
using Vega.Resources;

namespace Vega.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Feature, FeatureResource>();
            CreateMap<Model, ModelResource>();
            CreateMap<Make, MakeResource>();

            CreateMap<VehicleResource, Vehicle>()
                .ForMember(v => v.Id, opt => opt.Ignore())
                .ForMember(v => v.Features, opt => opt.Ignore())
                .ForMember(v => v.ContactEmail, opt => opt.MapFrom(vr => vr.Contact.Email))
                .ForMember(v => v.ContactName, opt => opt.MapFrom(vr => vr.Contact.Name))
                .ForMember(v => v.ContactPhone, opt => opt.MapFrom(vr => vr.Contact.Phone))
                .AfterMap((vr, v) => {
                    var removedFeatures = v.Features
                        .Where(f => !vr.Features.Contains(f.FeatureId));
                    
                    foreach (var f in removedFeatures)
                        v.Features.Remove(f);

                    var addedFeatures = vr.Features
                        .Where(id => !v.Features.Any(f => f.FeatureId == id))
                        .Select(id => new VehicleFeature { FeatureId = id });  

                    foreach (var f in addedFeatures)                       
                        v.Features.Add(f);                
                });

            CreateMap<Vehicle, VehicleResource>()
                .ForMember(vr => vr.Features, opt => opt.MapFrom(v => v.Features.Select(f => f.FeatureId)))
                .ForMember(vr => vr.Contact, opt => opt.MapFrom(v => new ContactResource { Phone = v.ContactPhone, Name = v.ContactName, Email = v.ContactEmail }));
        }
    }
}
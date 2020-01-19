﻿using AnimalRescue.Contracts.BusinessLogic.Models.Configurations;
using AnimalRescue.DataAccess.Mongodb.Models.Configurations;
using AnimalRescue.DataAccess.Mongodb.Models.Configurations.Nested;

using AutoMapper;

namespace AnimalRescue.BusinessLogic.Configurations.MappingProfiles
{
    internal class CmsConfigurationMappingProfile : Profile
    {
        public CmsConfigurationMappingProfile()
        {
            CreateMap<Configuration<Contacts>, CmsConfigurationDto>()
                .ForMember(x => x.Phones, o => o.MapFrom(cms => cms.Data.Phones))
                .ForMember(x => x.SocialLinks, o => o.MapFrom(cms => cms.Data.SocialLinks));

            CreateMap<CmsConfigurationDto, Configuration<Contacts>>()
                .ForPath(x => x.Data.Phones, o => o.MapFrom(cms => cms.Phones))
                .ForPath(x => x.Data.SocialLinks, o => o.MapFrom(cms => cms.SocialLinks));
        }
    }
}

using AutoMapper;
using FCMS.Domain.Models;
using FCMS.Models;
using FCMS.Mongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCMS
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map for Employee <-> EmployeeModel, ensuring that EmployeeModel properties are always mapped.
            CreateMap<Employee, EmployeeModel>(MemberList.Destination);
            CreateMap<EmployeeModel, Employee>(MemberList.Source);

            // Map for CreateEmployeeModel <-> Employee, ensuring that CreateEmployeeModel properties are always mapped and dates are populated.
            CreateMap<ICreateEmployeeModel, Employee>(MemberList.Source)
                .ForMember(x => x.Created, opt => opt.MapFrom(x => DateTime.UtcNow))
                .ForMember(x => x.Modified, opt => opt.MapFrom(x => DateTime.UtcNow));
        }
    }
}

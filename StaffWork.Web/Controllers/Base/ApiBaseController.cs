using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffWork.Core.Interfaces;

namespace StaffWork.Api.Controllers
{
    [Authorize]
	public class ApiBaseController<Model> : Controller where Model : class
	{
		public readonly IServicesBase<Model> BussinesService;
		public readonly IMapper _mapper;

        public ApiBaseController(IServicesBase<Model> servicesBase, IMapper mapper)
        {
            BussinesService = servicesBase;
            _mapper = mapper;
        }
    }
}

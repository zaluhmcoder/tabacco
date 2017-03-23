using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NLog;
using TeamTrack.Api.Models;
using TeamTrack.Core.Infrastructure;

namespace TeamTrack.Api.Controllers
{
    public class BaseController : Controller
    {
        private readonly IRepository _repository;
        private Logger _logger;
        private ModelFactory _modelFactory;
        private IMapper _autoMapper;

        public BaseController(IRepository repository, IMapper autoMapper)
        {
            _repository = repository;
            _autoMapper = autoMapper;
        }

        protected IRepository Repository
        {
            get
            {
                return _repository;
            }
        }

        protected Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LogManager.GetCurrentClassLogger();
                }
                return _logger;
            }
        }

        protected ModelFactory ModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(_repository, _autoMapper);
                }
                return _modelFactory;
            }
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TeamTrack.Core.Entities;
using TeamTrack.Core.Infrastructure;

namespace TeamTrack.Api.Models
{
    public class ModelFactory
    {
        #region Constructors

        private readonly IRepository _repository;
        private IMapper _mapper;
        public ModelFactory(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        #endregion

        public UserModel Create(UrlHelper urlHelper,User user)
        {
            if (user == null)
            {
                return null;
            }
            var userModel = _mapper.Map<User, UserModel>(user);
            userModel.Url = urlHelper.Link("UserById", new { Id = user.Id });
            return userModel;
        }
    }
}

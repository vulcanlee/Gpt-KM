using AutoMapper;
using Backend.AdapterModels;
using Domains.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using BAL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Services.Interfaces;

namespace Backend.Services
{
    public class RootFileUploadService : IRootFileUploadService
    {
        private readonly BackendDBContext context;

        public IMapper Mapper { get; }
        public AuthenticationStateProvider AuthenticationStateProvider { get; }

        public RootFileUploadService(BackendDBContext context, IMapper mapper,
            AuthenticationStateProvider authenticationStateProvider)
        {
            this.context = context;
            Mapper = mapper;
            AuthenticationStateProvider = authenticationStateProvider;
        }

    }
}

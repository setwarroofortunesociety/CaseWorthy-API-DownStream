using System;
using System.Threading.Tasks;
using AutoMapper;
using CW.ClientLibrary.Data;
using CW.ClientLibrary.Models;
using CW.ClientLibrary.Services.Client;
using CW.Library.Controllers;
using CW.Library.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;



namespace CW.ClientAPI.Controllers
{
    [Route("")]
    [Route("api/ClientDemo")]
    [ApiController]
    public class ClientDemoController : ControllerBase

    {
        private readonly IDemographic _demographic;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientDemoController> _logger;
        public ClientDemoController(IDemographic demographic, IMapper mapper, ILogger<ClientDemoController> logger)
        {
            try
            {
                _demographic = demographic;
                _mapper = mapper;
                _logger = logger;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical Error {0}", ex.Message);
            }

        }


        #region ClientDemograph
        //api/ClientDemo/{entityId}
        [Route("api/ClientDemo/{entityId}")]
        //[HttpGet("{entityId}", Name = "GetClient")]
        public async Task<ActionResult<StgClientDemographicModel>> GetClient(int entityId)
        {

            CWHelper.InitializeClient();

            var client = await ClientDemoProcessor.ClientByEntityId(entityId);

            if (client == null)
            {
                _logger.LogWarning("CW Client entityId({Id}) NOT FOUND", entityId);
                return NotFound();


            }

            var fixClientDemo = _mapper.Map<StgClientDemographicModel>(client);

            if (fixClientDemo == null)
            {
                _logger.LogWarning("CW Client ({client}) NOT FOUND", client);
                return NotFound();
            }

            var fixClient = _mapper.Map<StgClientDemographic>(fixClientDemo);


            _demographic.AddClientDemographic(fixClient);
            _demographic.Save();

            _logger.LogInformation("CW Client ({client}) FOUND", client);

            return Ok(fixClientDemo);

        }


        //api/ClientDemo/{firstname}{lastname}
        public async Task<ActionResult> GetClientByFirstLastName(string firstname, string lastname)
        {

            CWHelper.InitializeClient();

            var client = await ClientDemoProcessor.GetClientByFistLastName(firstname, lastname);

            _logger.LogInformation("CW Client ({0} {1}) FOUND", firstname, lastname);

            return Ok(client);


        }

        #endregion

    }
}
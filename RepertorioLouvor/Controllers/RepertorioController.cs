using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using RepertorioLouvor.Handlers;
using Microsoft.Extensions.Configuration;

namespace RepertorioLouvor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepertorioController : ControllerBase
    {
        private static GoogleSheetsHandler sheetsHandler;
        private IGoogleSheetsHandler _sheetsHandler;

        public RepertorioController(IConfiguration _configuration, IGoogleSheetsHandler sheetsHandler)
        {
            _sheetsHandler = sheetsHandler;

        }

        // GET: api/Repertorio
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var listaMusicas = await sheetsHandler.GetMusicsListAsync();
                return new OkObjectResult(listaMusicas);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);                
            }
             
        }

        // PUT: api/Repertorio/5
        [HttpPut("{id}")]
           public async Task<IActionResult> UpdateNota(int id, [FromQuery] int qtdVotosAtual, [FromQuery] int nota, [FromQuery] int notaAtual)
        {
            try
            {
                var resultUpdateNota = await _sheetsHandler.UpdateNotaAsync(qtdVotosAtual, nota, notaAtual, id);
                return new OkObjectResult(resultUpdateNota);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
                throw;
            }
        }

       
    }
}

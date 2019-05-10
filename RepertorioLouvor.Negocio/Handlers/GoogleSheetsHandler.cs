using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using RepertorioLouvor.Models;
using Microsoft.Extensions.Configuration;

namespace RepertorioLouvor.Handlers
{
    public class GoogleSheetsHandler : IGoogleSheetsHandler
    {
        #region Colunas
        static string COLUNA_NOME = "B";
        static string COLUNA_VOTOS = "E";
        static string COLUNA_MEDIANOTA = "F";
        static string COLUNA_QTDTOCADA = "G";
        static string COLUNA_ULTIMAVEZTOCADA = "H";
        static string COLUNA_DELIMITADORA = "I";
        #endregion

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        private string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private const string ApplicationName = "RepertorioLouvor";
        SheetsService service;
        private string spreadSheetId; 
        private string tabName;
        

        public GoogleSheetsHandler(IConfiguration configuration)
        {
            spreadSheetId = configuration["SheetsConfig:sheetId"];
            tabName = configuration["SheetsConfig:tabName"];            
        }

        UserCredential credential;        
        
        public GoogleSheetsHandler()
        {
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public async Task<int?> UpdateVotosAsync(int qtdVotosAtual, int rowNumber)
        {
            int? resultUpdate = await Task.Run(() =>
            {
                var body = GenerateValuesBody(qtdVotosAtual + 1);
                SpreadsheetsResource.ValuesResource.UpdateRequest updataReq = service.Spreadsheets.Values.Update(body, spreadSheetId, $"{tabName}!{COLUNA_VOTOS}{rowNumber}");
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption = (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum)2;
                updataReq.ValueInputOption = valueInputOption;

                var responseUpadate = updataReq.Execute();
                return responseUpadate.UpdatedCells;
            });

            return resultUpdate;
        }

        public async Task<int?> UpdateNotaAsync(int qtdVotosAtual, int nota, double notaAtual, int rowNumber)
        {
            int? resultUpdate = await Task.Run(() =>
            {
                qtdVotosAtual += 1;
                var mediaNota = (nota + notaAtual) / qtdVotosAtual;
                var body = GenerateValuesBody(mediaNota);
                SpreadsheetsResource.ValuesResource.UpdateRequest updataReq = service.Spreadsheets.Values.Update(body, spreadSheetId, $"{tabName}!{COLUNA_MEDIANOTA}{rowNumber}");
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption = (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum)2;
                updataReq.ValueInputOption = valueInputOption;

                var responseUpadate = updataReq.Execute();
                return responseUpadate.UpdatedCells;
            });

            return resultUpdate;
        }

        public async Task<int?> UpdateQtdVezesTocadaAsync(int qtdVezesTocadaAtual, int rowNumber)
        {
            int? resultUpdate = await Task.Run(() =>
            {
                qtdVezesTocadaAtual += 1;
                var body = GenerateValuesBody(qtdVezesTocadaAtual);
                SpreadsheetsResource.ValuesResource.UpdateRequest updataReq = service.Spreadsheets.Values.Update(body, spreadSheetId, $"{tabName}!{COLUNA_QTDTOCADA}{rowNumber}");
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption = (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum)2;
                updataReq.ValueInputOption = valueInputOption;

                var responseUpadate = updataReq.Execute();
                return responseUpadate.UpdatedCells;
            });

            return resultUpdate;
        }

        public async Task<int?> UpdateUltimaVezTocadaAsync(DateTime dataTocada, int rowNumber)
        {
            int? resultUpdate = await Task.Run(() =>
            {
                var body = GenerateValuesBody(dataTocada);
                SpreadsheetsResource.ValuesResource.UpdateRequest updataReq = service.Spreadsheets.Values.Update(body, spreadSheetId, $"{tabName}!{COLUNA_ULTIMAVEZTOCADA}{rowNumber}");
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption = (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum)2;
                updataReq.ValueInputOption = valueInputOption;

                var responseUpadate = updataReq.Execute();
                return responseUpadate.UpdatedCells;
            });

            return resultUpdate;
        }

        public async Task<List<Musica>> GetMusicsListAsync()
        {

            var musicas = new List<Musica>();

            var listaMusicas = await Task.Run(() =>
            {
                var values = GetBodyValuesList();

                if (values != null && values.Count > 0)
                {
                    int indexMusica = 2;
                    Musica musica = new Musica();
                    foreach (var row in values)
                    {
                        // Print columns A and E, which correspond to indices 0 and 4.
                        if (row.Count == 0)
                        {
                            break;
                        }
                        musica.Id = indexMusica;
                        musica.Nome = row[0].ToString();
                        musica.Artista = row[1].ToString();
                        musica.Link = row[2].ToString();
                        musica.Votos = string.IsNullOrEmpty(row[3].ToString()) ? 0 : Convert.ToInt32(row[3]);
                        musica.MediaNota = string.IsNullOrEmpty(row[4].ToString()) ? 0 : Convert.ToDouble(row[4]);
                        musica.QtdTocada = string.IsNullOrEmpty(row[5].ToString()) ? 0 : Convert.ToInt32(row[5]);
                        musica.UltimaVezTocada = string.IsNullOrEmpty(row[6].ToString()) ? DateTime.MinValue : Convert.ToDateTime(row[6]);

                        musicas.Add(musica);
                        musica = new Musica();
                        indexMusica++;
                    }
                }

                return musicas;
            });

            return listaMusicas;
        }

        private IList<IList<object>> GetBodyValuesList()
        {
            String range = $"{tabName}!{COLUNA_NOME}2:{COLUNA_DELIMITADORA}";
            SpreadsheetsResource.ValuesResource.GetRequest getRequest =
                    service.Spreadsheets.Values.Get(spreadSheetId, range);

            ValueRange response = getRequest.Execute();
            IList<IList<Object>> values = response.Values;
            return values;
        }

        private static ValueRange GenerateValuesBody(object valor)
        {
            var listaValor = new List<object>();

            var listaDelistaDeValor = new List<IList<object>>();

            listaValor.Add(valor);

            listaDelistaDeValor.Add(listaValor);

            IList<IList<object>> valores = listaDelistaDeValor;

            ValueRange body = new ValueRange()
            {
                Values = valores
            };

            return body;
        }
    }
}

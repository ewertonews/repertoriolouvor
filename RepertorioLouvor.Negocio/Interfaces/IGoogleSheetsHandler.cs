using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepertorioLouvor.Models;

namespace RepertorioLouvor.Handlers
{
    public interface IGoogleSheetsHandler
    {
        Task<List<Musica>> GetMusicsListAsync();
        Task<int?> UpdateNotaAsync(int qtdVotosAtual, int nota, double notaAtual, int rowNumber);
        Task<int?> UpdateQtdVezesTocadaAsync(int qtdVezesTocadaAtual, int rowNumber);
        Task<int?> UpdateUltimaVezTocadaAsync(DateTime dataTocada, int rowNumber);
        Task<int?> UpdateVotosAsync(int qtdVotosAtual, int rowNumber);
    }
}
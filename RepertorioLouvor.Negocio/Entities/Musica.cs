using System;

namespace RepertorioLouvor.Models
{
    public class Musica
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Artista { get; set; }
        public string Link { get; set; }
        public int Votos { get; set; }
        public Double MediaNota { get; set; }
        public int QtdTocada { get; set; }
        public DateTime? UltimaVezTocada { get; set; }

    }
}
